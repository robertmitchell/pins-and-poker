<?php

namespace App\Http\Controllers\Api\User;

use App\Constants\Status;
use App\Http\Controllers\Controller;
use App\Http\Requests\Game\{CancelRequest, JoinRequest};
use App\Http\Resources\GameResource;
use App\Models\{Game, GameRequest, League};
use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class GameController extends Controller
{
    public final function join(JoinRequest $request)
    {
        $authUser = auth()->user();
        try {
            DB::beginTransaction();
            
            $league = League::whereId($request->league_id)->first();
            if (empty($league)) return $this->errorResponse('League Not Found.', 404);

            $hasJoinedLeague = $league->league_requests()->where('user_id', $authUser->id)->where('status', Status::ACCEPTED)->exists();
            if (empty($hasJoinedLeague)) 
            return $this->errorResponse("You're not a participant in this league or your request has not been accepted.", 403);
            
            $game = Game::whereId($request->game_id)->where('league_id', $request->league_id)->first();
            if (empty($game)) return $this->errorResponse("The requested game does not exist in the given league.", 404);

            $hasRequested = GameRequest::where('game_id', $request->game_id)->where('user_id', $authUser->id)->first();

            if (!empty($hasRequested)) {
                $data = ['status' => $hasRequested->status];
                if ($hasRequested->status === Status::ACCEPTED) {
                    return $this->successResponse("Your request has already been accepted. Thank you for your patience!");
                } else {
                    return $this->successDataResponse($data, "Your request is currently under review.");
                }
            }

            $gameRequest = GameRequest::create([
                'game_id' => $request->game_id,
                'user_id' => $authUser->id,
                'status'    => Status::PENDING
            ]);
            $data = ['status' => $gameRequest->status];

            DB::commit();
            $message = 'Your request to join this league game is under review. Please wait for approval.';
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

            $league = League::whereId($request->league_id)->first();
            if (empty($league)) { return $this->errorResponse('League Not Found.', 404); }

            $hasJoinedLeague = $league->league_requests()->where('user_id', $authUser->id)->where('status', Status::ACCEPTED)->exists();
            if (empty($hasJoinedLeague)) 
            return $this->errorResponse("You're not a participant in this league or your request has not been accepted.", 403);

            $game = Game::whereId($request->game_id)->where('league_id', $request->league_id)->first();
            if (empty($game)) return $this->errorResponse("The requested game does not exist in the given league.", 404);

            $gameRequest = GameRequest::where('game_id', $request->game_id)->where('user_id', $authUser->id)->first();
            if (empty($gameRequest)) return $this->errorResponse("Sorry, we couldn't find the request in the league game.", 404);

            if ($gameRequest->status === Status::ACCEPTED)
            return $this->errorResponse("You cannot cancel your request as it has already been accepted into the league game.");

            $gameRequest->delete();

            DB::commit();
            return $this->successResponse('You have successfully cancelled your participation in the league game!');
        } catch (\Exception $e) {
            DB::rollBack();
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }

    public final function get_league_games(Request $request)
    {
        $this->validate($request, [
            'league_id' => 'required|numeric|digits_between:1,20|exists:leagues,id'
        ], [
            'league_id.exists' => 'The league id does not exist in our records.'
        ]);

        $authUser = auth()->user();
        try {
            $league = League::whereId($request->league_id)->first();
            
            $games = Game::with(['game_requests' => function ($q) use ($authUser) {
                $q->where('user_id', $authUser->id);
            }])
            ->where('league_id', $request->league_id)
            ->where('status','!=', 'ended')
            ->get();
            
            $data = [
                "league_participants" => $league->participants,
                "games" => GameResource::collection($games)
            ];
            
            $message = "Your games have been successfully fetched.";
            return $this->successDataResponse($data, $message);
        } catch (\Exception $e) {
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }
}
