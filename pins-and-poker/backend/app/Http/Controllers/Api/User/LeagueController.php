<?php

namespace App\Http\Controllers\Api\User;

use App\Constants\Status;
use App\Http\Controllers\Controller as Controller;
use App\Http\Requests\League\{CancelRequest, JoinRequest};
use App\Http\Resources\LeagueResource;
use App\Http\Resources\RuleResource;
use App\Models\{League, LeagueRequest};
use Illuminate\Support\Facades\DB;

class LeagueController extends Controller
{
    public function join(JoinRequest $request)
    {
        $authUser = auth()->user();

        try {
            DB::beginTransaction();

            $hasRequested = LeagueRequest::where('league_id', $request->league_id)
            ->where('user_id', $authUser->id)
            ->first();
            
            if (!empty($hasRequested)) {
                $data = ['status' => $hasRequested->status];
                if ($hasRequested->status === Status::ACCEPTED) {
                    return $this->successResponse("Your request has already been accepted. Thank you for your patience!");
                } else {
                    return $this->successDataResponse($data, "Your request is currently under review.");
                }
            }

            $leagueRequest = LeagueRequest::create([
                'league_id' => $request->league_id,
                'user_id'   => $authUser->id,
                'status'    => Status::PENDING
            ]);
            $data = ['status' => $leagueRequest->status];
            
            DB::commit();
            $message = 'Your request to join this league is under review. Please wait for approval.';
            return $this->successDataResponse($data, $message);
        } catch (\Exception $e) {
            DB::rollBack(); 
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }

    public final function cancel(CancelRequest $request)
    {
        $authUser = auth()->user();

        try {
            DB::beginTransaction();

            $leagueRequest = LeagueRequest::where('league_id', $request->league_id)
                            ->where('user_id', $authUser->id)
                            ->first();

            if (empty($leagueRequest)) {
                return $this->errorResponse("Sorry, we couldn't find your request in the league.", 404);
            }

            if ($leagueRequest->status === Status::ACCEPTED) {
                return $this->errorResponse("You cannot cancel your request as it has already been accepted into the league.");
            }

            $leagueRequest->delete();

            DB::commit();
            return $this->successResponse('You have successfully cancelled your participation in the league!');
        } catch (\Exception $e) {
            DB::rollBack();
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }

    public final function get_all_leagues()
    {
        $authUser = auth()->user();
        try {
            $leagues = League::whereDoesntHave('league_requests', function ($q) use ($authUser) {
                $q->where('user_id', $authUser->id);
            })->get();
            
            if ($leagues->isEmpty()) {
                return $this->errorResponse('No league records found.', 404);
            }

            $message = "The league records have been retrieved successfully.";
            return $this->successDataResponse(LeagueResource::collection($leagues), $message);
        } catch (\Exception $e) {
            // return $this->errorResponse($e->getMessage(), 500);
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }

    public final function user_leagues()
    {
        $authUser = auth()->user();
        
        try {
            // Get user leagues
            $leagues = League::with(['league_requests' => function ($q) use ($authUser) {
                $q->where('user_id', $authUser->id);
            }])->whereHas('league_requests', function ($q) use ($authUser) {
                $q->where('user_id', $authUser->id);
            })->get();

            if ($leagues->isEmpty()) {
                return $this->errorResponse('No league records found.', 404);
            }

            $data = $leagues->map(function ($league) {
                return [
                    'id'           => $league->id,
                    'player_id'    => $league->user->player_id,
                    'name'         => $league->name,
                    'image'        => $league->image,
                    'participants' => $league->participants,
                    'prize_pool'   => $league->prize_pool,
                    'start_time'   => $league->start_time,
                    'created_at'   => format_date($league->created_at),
                    'rules'        => RuleResource::collection($league->rules),
                    'requests'     => $league->league_requests->map(function ($req) {
                        return [
                            'status' => $req->status,
                            'created_at' => format_date($req->created_at),
                            'user'  => [
                                'player_id' => $req->user->player_id,
                                'username'  => $req->user->username,
                                'image'     => $req->user->avatar_image,
                            ]
                        ];
                    })
                ];
            })->toArray();

            return $this->successDataResponse($data, 'User data retrieved successfully.');
        } catch (\Exception $e) {
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }
}