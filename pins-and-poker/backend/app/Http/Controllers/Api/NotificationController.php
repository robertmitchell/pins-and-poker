<?php

namespace App\Http\Controllers\Api;

use App\Constants\Status;
use App\Http\Controllers\Controller;
use App\Models\Notification;
use App\Models\UserNotification;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Http;
use Google\Client; 

class NotificationController extends Controller
{
    // public final function create_notification(Request $request)
    // {
    //     $this->validate($request, [
    //         'title' => 'required|string|max:255',
    //         'body'  => 'required|string'
    //     ]);

    //     $authUser = auth()->user();

    //     try {
            
    //     } catch (\Exception $e) {
    //         return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
    //     }
    // }
    
    public static function create_notification(Request $request)
    {
        $path = base_path(env('FIREBASE_CREDENTIALS'));

        // Get OAuth2 Token
        $googleClient = new Client();
        $googleClient->setAuthConfig($path);
        $googleClient->addScope('https://www.googleapis.com/auth/firebase.messaging');
        $accessToken = $googleClient->fetchAccessTokenWithAssertion()['access_token'];
        
        // FCM endpoint for v1
        $firebaseURL = 'https://fcm.googleapis.com/v1/projects/pins-and-poker-1485f/messages:send';
    
        // Payload for FCM v1
        $payload = [
            'message' => [
                'token' => $request->device_id,
                'notification' => [
                    'title' => $request->title,
                    'body' => $request->body,
                ],
                'data' => [
                    'click_action' => 'FLUTTER_NOTIFICATION_CLICK',
                ],
            ],
        ];
    
        // Send HTTP POST request to FCM
        $response = Http::withHeaders([
            'Authorization' => 'Bearer ' . $accessToken,
            'Content-Type' => 'application/json',
        ])->post($firebaseURL, $payload);
    
        // Return response
        return $response->json();
    }

    public final function get_notifications()
    {
        $user = auth()->user();

        $notifications = Notification::with('user_seen')->where('user_id', $user->id)
            ->orWhere('user_id', Status::IS_ADMIN)
            ->get();

        $data = $notifications->map(function ($notify) {
            return [
                'id'        => $notify->id,
                'player_id' => $notify->user->player_id,
                'title'     => $notify->title,
                'body'      => $notify->body,
                'is_read'   => $notify->user_seen ? $notify->user_seen->is_read : '0',
                'created_at' => format_date($notify->created_at),
            ];
        });

        return $this->successDataResponse(collect($data), 'Notifications Fetched Successfuly.');
    }
    
    public final function markAsRead(Request $request)
    {
        $this->validate($request, [
            'notify_id' => 'required|exists:notifications,id',
            'is_read' => 'required|in:1',
        ]);
        
        $authUser = auth()->user();

        try {
            
            $notify = UserNotification::where('notifiable_id', $request->notify_id)
                    ->where('user_id', $authUser->id)
                    ->first();
            
            if (!empty($notify) && $notify->is_read == '1') 
            return $this->errorResponse('Already Readed.', 400);
            
            $notification = UserNotification::create([
                'user_id'       => $authUser->id,
                'notifiable_id' => $request->notify_id,
                'is_read'       => $request->is_read,
            ]);
    
            return $this->successDataResponse($notification, 'Mark as read successfuly.');
    
        } catch (\Exception $e) {
            return $e->getMessage();
            return $this->errorResponse('Oops! Something went wrong.', 500);
        }
    }
}
