<?php

namespace App\Http\Controllers\Api;

use App\Constants\{FileInfo, RoleType, Status};
use App\Helpers\FileHelper;
use App\Http\Controllers\Controller as Controller;
use App\Http\Resources\UserResource;
use App\Models\User;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Hash;

class AuthController extends Controller
{
    public final function login(Request $request)
    {
        $this->validate($request, [
            'login_type' => 'required|string|in:guest,social'
        ]);

        $player_id = rand(20000000000, 99999999999);

        switch($request->login_type) {
            case 'guest':
                return $this->guest($request, $player_id);

            case 'social':                                                                                                                                                                                               
                return $this->social($request, $player_id);

            default:
                return $this->errorResponse('The login type is invalid.', 400);
        }
    }
    
    private function guest(Request $request, $player_id)
    {
        $this->validate($request, [
            'auth_provider' => 'required|in:guest',
            'platform'      => 'required|in:android,ios',
            'device_token'  => 'required|string'
        ]);
        // Find user by device token
        $user = User::where([
            ['device_token', $request->device_token], ['is_social', Status::NOT_SOCIAL]
        ])->withTrashed()->first();
        // Check for deleted user
        if ($user && $user->trashed()) {
            return $this->errorResponse('This account has been permanently removed from our system.', 410);
        }
        
        if (empty($user)) {
            $this->validate($request, [
                'username' => 'required|string|min:3|max:255',
                'image'    => 'nullable|image|mimes:jpeg,png,jpg|max:2048'
            ]);
        }

        // HANDLE IMAGE UPLOAD
        $file = $request->file('image');
        $path = $request->hasFile('image')
                ? FileHelper::handleImageUpload($file, $request->user_type, $request->user_type)
                : FileHelper::getDefaultImage($request->user_type);

        try {
            DB::beginTransaction();

            // Ask about that a guest user can login into another device or not ?
            if(empty($user)) {
                // Create new account
                $email = strtolower(trim($request->username)) . "{$player_id}@pap.com";
                $user = User::create([
                    'player_id'     => $player_id,
                    'username'      => $request->username,
                    'email'         => $email,
                    'user_type'     => RoleType::PLAYER,
                    'avatar_image'  => $path,
                    'auth_provider' => $request->auth_provider,
                    'platform'      => $request->platform,
                    'device_token'  => $request->device_token,
                    'fcm_token'     => $request->fcm_token
                ]);

                $message = 'Your account has been successfully created.';
            } else {
                // Login existing account
                if ($user->user_type !== RoleType::PLAYER) {
                    return $this->errorResponse('Please log in with the correct account type.', 403);
                }

                if ($user->is_blocked == Status::BLOCKED) {
                    return $this->errorResponse('This account has been blocked and cannot be accessed.', 403);
                }

                if ($user->is_social !== Status::NOT_SOCIAL) {
                    $user->update(['platform' => $request->platform, 'device_token' => $request->device_token]);
                }

                $message = 'You are logged in successfully.';
            }

            $accessToken = $user->createToken('PinsAndPoker')->plainTextToken;
            $user->setAttribute('access_token', $accessToken);

            DB::commit();
            return $this->successDataResponse(new UserResource($user), $message);
        } catch (\Exception $e) {
            DB::rollback();
            // return $this->errorResponse($e->getMessage(), 500);
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }

    private function social(Request $request, $player_id)
    {
        $this->validate($request, [
            'auth_provider' => 'required|string|max:255|in:google,apple',
            'platform'      => 'required|string|max:255|in:android,ios',
            'social_id'     => 'required|string',
            'device_token'  => 'required|string'
        ]);

        $user = User::where([
            ['social_id', $request->social_id],['is_social', Status::SOCIAL]
        ])->withTrashed()->first();

        if ($user && $user->trashed()) {
            return $this->errorResponse('This account has been permanently removed from our system.', 410);
        }

        if (empty($user)) {
            $this->validate($request, [
                'username' => 'required|string|min:3|max:255',
                'email'    => 'required|string|max:255|email:rfc,dns|unique:users,email',
                'image'    => 'nullable|image|mimes:jpeg,png,jpg|max:2048',
            ]);
        }

        // HANDLE IMAGE UPLOAD
        $file = $request->file('image');
        $path = $request->hasFile('image')
                ? FileHelper::handleImageUpload($file, $request->user_type, $request->user_type)
                : FileHelper::getDefaultImage($request->user_type);

        try {
            DB::beginTransaction();

            if (empty($user)) {
                // ACCOUNT CREATED
                $user = User::create([
                    'player_id'     => $player_id,
                    'username'      => $request->username,
                    'email'         => $request->email,
                    'avatar_image'  => $path,
                    'user_type'     => RoleType::PLAYER,
                    'auth_provider' => $request->auth_provider,
                    'platform'      => $request->platform,
                    'is_social'     => Status::SOCIAL,
                    'social_id'     => $request->social_id,
                    'device_token'  => $request->device_token,
                    'fcm_token'     => $request->fcm_token
                ]);

                $message = 'Your account has been successfully created.';
            } else {
                // ACCOUNT LOGGED IN
                if ($user->user_type !== $request->user_type) {
                    return $this->errorResponse('Please log in with the correct account type..', 403);
                }

                if ($user->is_blocked == Status::BLOCKED) {
                    return $this->errorResponse('This account has been blocked and cannot be accessed.', 403);
                }

                $user->update(['platform' => $request->platform, 'device_token' => $request->device_token]);
                if ($user->tokens()->count() > 0) { $user->tokens()->delete(); }
                
                $message = 'You are logged in successfully.';
            }

            $accessToken = $user->createToken('PinsAndPoker')->plainTextToken;  
            $user->setAttribute('access_token', $accessToken);

            DB::commit();
            return $this->successDataResponse(new UserResource($user), $message);
        } catch (\Exception $e) {
            DB::rollback();
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }

    public final function connectWithSocial(Request $request)
    {
        $this->validate($request, [
            'username'      => 'required|string|min:3|max:255',
            'email'         => 'required|string|max:255|email:rfc,dns|unique:users,email',
            'image'         => 'nullable|image|mimes:jpeg,png,jpg|max:2048',
            'auth_provider' => 'required|string|max:255|in:guest,google,apple',
            'platform'      => 'required|string|max:255|in:android,ios',
            'social_id'     => 'required|string',
            'device_token'  => 'required|string'
        ]);

        $authUser = auth()->user();
        // HANDLE IMAGE UPLOAD
        $file = $request->file('image');
        $uploadImage = FileHelper::handleImageUpload($file, $authUser->user_type, $authUser->user_type);
        $path = $file ? $uploadImage : null;

        // DEFAULT IMAGE
        $defaultImage = ($authUser->user_type == RoleType::MODERATOR) 
            ? FileInfo::MODERATOR_DEFAULT_IMAGE
            : FileInfo::PLAYER_DEFAULT_IMAGE;

        try {
            DB::beginTransaction();

            $user = User::where([
                ['player_id', $authUser->player_id],['device_token', $request->device_token]
            ])->first();

            if (empty($user)) {
                return $this->errorResponse("Sorry, we couldn't find the requested player.", 404);
            }

            $existing_social_user = User::where([
                ['social_id', $request->social_id], ['is_social', Status::SOCIAL]
            ])->first();

            if (!empty($existing_social_user)) {
                // In this case i have to overwrite social account and all user details return $this->overwriteAccount($player_id);
                $error_msg = 'This ' . $existing_social_user->auth_provider . ' account is already associated with another user.';
                return $this->errorResponse($error_msg, 409);
            }

            $oldImage = $user->avatar_image;
            $user->update([
                'username'      => $request->username,
                'email'         => $request->email,
                'avatar_image'  => $path ?? $oldImage,
                'auth_provider' => $request->auth_provider,
                'platform'      => $request->platform,
                'is_social'     => Status::SOCIAL,
                'social_id'     => $request->social_id,
                'device_token'  => $request->device_token,
                'fcm_token'     => $request->fcm_token
            ]);

            // DELETE OLD IMAGE
            if (!empty($path) && $oldImage !== $defaultImage) {
                FileHelper::removeOldImage($oldImage);
            }

            $success_msg = 'Your guest account has been successfully linked with your '. $request->auth_provider .' account.';
            if ($user->tokens()->count() > 0) { $user->tokens()->delete(); }
            $accessToken = $user->createToken('PinsAndPoker')->plainTextToken;
            $user->setAttribute('access_token', $accessToken);

            DB::commit();
            return $this->successDataResponse(new UserResource($user), $success_msg);
        } catch (\Exception $e) {
            DB::rollback();
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }

    public final function moderatorLogin(Request $request)
    {
        $this->validate($request, [
            'email'         => 'required|string|max:255|email:rfc,dns|exists:users,email',
            'password'      => 'required|string|max:255',
            'auth_provider' => 'required|string|max:255|in:normal',
            'platform'      => 'required|string|max:255|in:android,ios',
            'device_token'  => 'required|string'
        ], [
            'email.exists' => 'The email does not exist in our records.',
        ]);

        $user = User::where('email', $request->email) 
        ->where('user_type', RoleType::MODERATOR)
        ->withTrashed()->first();

        if (empty($user)) {
            return $this->errorResponse('The email you entered is incorrect.', 422);
        }

        if ($user && $user->trashed()) {
            return $this->errorResponse('This account has been permanently removed from our system.', 410);
        }

        if ($user->is_blocked == Status::BLOCKED) {
            return $this->errorResponse('This account has been blocked and cannot be accessed.', 403);
        }

        try {
            DB::beginTransaction();
            
            // ACCOUNT LOGGED IN
           if (Hash::check($request->password, $user->password)) {
            // if ($request->password == $user->password) {
                $user->update([
                    'auth_provider' => $request->auth_provider,
                    'platform'      => $request->platform,
                    'device_token'  => $request->device_token,
                    'fcm_token'     => $request->fcm_token
                ]);
                if ($user->tokens()->count() > 0) { $user->tokens()->delete(); }

            } else {
                return $this->errorResponse('The password you entered is incorrect.', 422);
            }
            
            $message = 'You are logged in successfully.';
            $accessToken = $user->createToken('PinsAndPoker')->plainTextToken;  
            $user->setAttribute('access_token', $accessToken);

            DB::commit();
            return $this->successDataResponse(new UserResource($user), $message);
        } catch (\Exception $e) {
            DB::rollback();
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }

    public final function logout()
    {
        $user = auth()->user();
        if (empty($user)) {
            return $this->errorResponse('Unauthenticated.', 401);
        }
        
        $tokensDeleted = $user->tokens()->delete();
        return ($tokensDeleted)
            ? $this->successResponse('You have been successfully logged out.')
            : $this->errorResponse('Oops! Something went wrong. Please try again later.', 401);
    }
}
