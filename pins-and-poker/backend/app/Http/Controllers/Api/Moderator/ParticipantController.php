<?php

namespace App\Http\Controllers\Api\Moderator;

use App\Constants\RoleType;
use App\Constants\Status;
use App\Http\Controllers\Controller;
use App\Models\{Game, GameRequest, League, LeagueRequest, User};
use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class ParticipantController extends Controller
{
    public final function remove_participants(Request $request)
    {
        $this->validate($request, [
            'player_id'   => 'required|numeric|digits_between:10,12|exists:users,player_id',
            'remove_from' => 'required|in:league,game',
        ], [
            'player_id.exists' => 'The player id does not exist in our records.'
        ]);

        $user = User::where('player_id', $request->player_id)->first();
        if ($user->user_type != RoleType::PLAYER) {
            return $this->errorResponse('This player id is not authorized to make changes.', 403);
        }

        switch ($request->remove_from) {
            case 'league':
                return $this->remove_from_league($request, $user->id);

            case 'game':
                return $this->remove_game_league($request, $user->id);
        }
    }

    private function remove_from_league(Request $request, $user_id)
    {
        $this->validate($request, [
            'league_id' => 'required|numeric|digits_between:1,20|exists:leagues,id'
        ], [
            'league_id.exists' => 'The league id does not exist in our records.'
        ]);

        try {
            DB::beginTransaction();

            // Remove user from league
            $league = League::whereId($request->league_id)->first();
            if (empty($league)) return $this->errorResponse('League Not Found');

            $isLeagueParticipant = $league->league_requests()->where('user_id', $user_id)
            ->where('status', Status::ACCEPTED)->first();
    
            if (empty($isLeagueParticipant))
            return $this->errorResponse("This player is not a participant in the league.");

            $league->decrement('participants');
            $isLeagueParticipant->delete();

            // Remove user from game
            $games = $league->games()->get();
            if ($games->isEmpty()) return $this->errorResponse('No games found for this league.');

            foreach ($games as $game) {
                if ($game->participants > 0) { $game->decrement('participants'); }
                $game->game_requests()->where('user_id', $user_id)->delete(); // deleting game requests
                $game->game_scores()->where('user_id', $user_id)->delete();// deleting game scores
            }

            DB::commit();
            $message = "You have successfully removed '{$isLeagueParticipant->user->username}' from league participant.";
            return $this->successResponse($message);

        } catch (\Exception $e) {
            DB::rollBack();
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }

    private function remove_game_league(Request $request, $user_id)
    {
        $this->validate($request, [
            'game_id'   => 'required|numeric|digits_between:1,20|exists:games,id'
        ], [
            'game_id.exists'   => 'The game id does not exist in our records.'
        ]);

        try {
            DB::beginTransaction();

            $game = Game::whereId($request->game_id)->first();
            if (empty($game)) return $this->errorResponse('League Game Not Found');

            $isParticipant = $game->game_requests()->where('user_id', $user_id)
            ->where('status', Status::ACCEPTED)->first();

            if (empty($isParticipant))
            return $this->errorResponse("This player is not a participant in the league.");

            if ($game->participants > 0) { $game->decrement('participants'); }
            $isParticipant->delete();

            DB::commit();
            $message = "You have successfully removed '{$isParticipant->user->username}' from league game participant.";
            return $this->successResponse($message);

        } catch (\Exception $e) {
            DB::rollBack();
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }
}
