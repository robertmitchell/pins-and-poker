<?php

namespace App\Http\Controllers\Api;

use App\Constants\RoleType;
use App\Constants\Status;
use App\Http\Controllers\Controller;
use App\Models\{Game, League};
use Illuminate\Http\Request;

class ParticipantController extends Controller
{
    public final function get_league_participants(Request $request)
    {
        $this->validate($request, [
            'league_id' => 'required|numeric|digits_between:1,20|exists:leagues,id'
        ], [
            'league_id.exists' => 'The league id does not exist in our records.'
        ]);

        $authUser = auth()->user();

        try {
            $league = League::whereId($request->league_id)->first();
            if (empty($league)) { return $this->errorResponse('League Not Found.', 404); }
            
            if ($authUser->user_type === RoleType::MODERATOR) {
                // Check if the league is created by moderator.
                if ($league->user_id !== $authUser->id) {
                    return $this->errorResponse('You are not authorized to manage this league.', 403);
                }

            } else {
                // League Joined Check
                $hasJoinedLeague = $league->whereHas('league_requests', function ($q) use ($authUser) {
                    $q->where('user_id', $authUser->id)->where('status', Status::ACCEPTED);
                })->exists();

                if (empty($hasJoinedLeague)) {
                    return $this->errorResponse("You're not a participant in this league or your request has not been accepted.");
                }
            }

            $data = $league->league_requests->where('status', Status::ACCEPTED)
            ->map(function ($req) {
                return [
                    'player_id' => $req->user->player_id,
                    'username'  => $req->user->username,
                    'image'     => $req->user->avatar_image
                ];
            });

            $message = "League Participants fetched successfully.";
            return $this->successDataResponse(collect($data), $message);
        } catch (\Exception $e) {
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }

    public final function get_game_participants(Request $request)
    {
        $this->validate($request, [
            'league_id' => 'required|numeric|digits_between:1,20|exists:leagues,id',
            'game_id'   => 'required|numeric|digits_between:1,20|exists:games,id'
        ], [
            'league_id.exists' => 'The league id does not exist in our records.',
            'game_id.exists'   => 'The game id does not exist in our records.'
        ]);

        $authUser = auth()->user();

        try {
            $league = League::whereId($request->league_id)->first();
            if (empty($league)) { return $this->errorResponse('League Not Found.', 404); }

            // Game Joined Check
            $game = Game::whereId($request->game_id)->where('league_id', $request->league_id)->first();
            if (empty($game)) {
                return $this->errorResponse("The requested game does not exist in the given league.", 404);
            }

            if ($authUser->user_type === RoleType::MODERATOR) {
                // Check if the league is created by moderator.
                if ($league->user_id !== $authUser->id) {
                    return $this->errorResponse('You are not authorized to manage this league.', 403);
                }
                // Check if the league game is created by moderator.
                if ($game->user_id !== $authUser->id) {
                    return $this->errorResponse('You are not authorized to manage this league game.', 403);
                }

            } else {
                // League Joined Check
                $hasJoinedLeague = $league->whereHas('league_requests', function ($q) use ($authUser) {
                    $q->where('user_id', $authUser->id)->where('status', Status::ACCEPTED);
                })->exists();

                if (empty($hasJoinedLeague)) {
                    return $this->errorResponse("You're not a participant in this league or your request has not been accepted.");
                }
                
                $hasJoinedGame = $game->whereHas('game_requests', function ($q) use ($authUser) {
                    $q->where('user_id', $authUser->id)->where('status', Status::ACCEPTED);
                })->exists();

                if (empty($hasJoinedGame)) {
                    return $this->errorResponse("You're not a participant in this league game or your request has not been accepted.");
                }
            }

            $data = $game->game_requests->where('status', Status::ACCEPTED)
            ->map(function ($req) {
                return [
                    'player_id' => $req->user->player_id,
                    'username'  => $req->user->username,
                    'image'     => $req->user->avatar_image
                ];
            });

            $message = "Game Participants fetched successfully.";
            return $this->successDataResponse(collect($data), $message);
        } catch (\Exception $e) {
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }
}
