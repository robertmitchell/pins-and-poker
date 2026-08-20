<?php

namespace App\Http\Controllers\Api;

use App\Constants\{FileInfo, RoleType};
use App\Helpers\FileHelper;
use App\Http\Controllers\Controller as Controller;
use App\Http\Resources\UserResource;
use App\Models\User;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class ProfileController extends Controller
{
    public final function update(Request $request)
    {
        $authUser = auth()->user();

        // Define validation rules based on user type
        $rules = $this->getValidationRules($authUser->user_type);
        $this->validate($request, $rules);
        
        // HANDLE IMAGE UPLOAD
        $file = $request->file('image');
        $uploadImage = !empty($file) ? FileHelper::handleImageUpload($file, $authUser->user_type, $authUser->user_type) : null;
        $path = !empty($uploadImage) ? $uploadImage : null;

        // DEFAULT IMAGE
        $defaultImage = ($authUser->user_type == RoleType::MODERATOR) 
            ? FileInfo::MODERATOR_DEFAULT_IMAGE
            : FileInfo::PLAYER_DEFAULT_IMAGE;

        try {
            DB::beginTransaction();

            $user = User::where('player_id', $authUser->player_id)->first();
            if (empty($user)) return $this->errorResponse('Unauthenticated', 401);

            $oldImage = $user->avatar_image;
            $user->update([
                'username'     => $request->username,
                // 'email'        => $request->email ?? $user->email,
                'avatar_image' => $path ?? $oldImage,
                'phone'        => !empty($request->phone) ? $request->phone : $user->phone ?? null,
                'fcm_token'    => $request->fcm_token
            ]);

            // DELETE OLD IMAGE
            if (!empty($path) && $oldImage !== $defaultImage) {
                FileHelper::removeOldImage($oldImage);
            }

            $accessToken = $user->createToken('PinsAndPoker')->plainTextToken;  
            $user->setAttribute('access_token', $accessToken);

            DB::commit();
            return $this->successDataResponse(new UserResource($user), 'Your profile has been updated successfully.');

        } catch (\Exception $e) {
            DB::rollBack();
            // return $this->errorResponse($e->getMessage(), 500);
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }

    private final function getValidationRules($userType)
    {
        $baseRules = [
            'username' => 'required|string|min:3|max:255',
            'image'    => 'sometimes|image|mimes:jpeg,png,jpg|max:2048',
        ];

        // Add user type specific rules
        if ($userType === RoleType::MODERATOR) {
            return array_merge($baseRules, [
                // 'email' => 'required|string|max:255|email:rfc,dns|unique:users,email',
                'phone' => 'required|numeric|digits_between:10,12'
            ]);
        }

        return $baseRules;
    }

    public function delete(Request $request)
    {
        $authUser = auth()->user();
        
        try {
            DB::beginTransaction();

            $user = User::whereId($authUser->id)->first();
            if (empty($user)) {
                return $this->errorResponse('Unauthenticated.', 401);
            }

            $user->tokens()->delete();
            $user->update(['device_token' => null]);
            $user->update(['deleted_at' => \Carbon\Carbon::now()]);

            DB::commit();
            return $this->successResponse('Your account has been successfully deleted.', 200);

        } catch (\Exception $e) {
            DB::rollBack();
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }
}
