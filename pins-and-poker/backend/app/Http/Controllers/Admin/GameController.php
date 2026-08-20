<?php

namespace App\Http\Controllers\Admin;

use App\Http\Controllers\Controller;
use App\Models\{Game, GameScore};
use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class GameController extends Controller
{
    public function index()
    {
        $games = Game::with('user', 'league')->get();

        $pageTitle = 'Game List';
        return view('admin.game.index', compact('games', 'pageTitle'));
    }

    public function edit($id)
    {
        $game = Game::whereId($id)->first();
        
        $pageTitle = 'Edit Game';
        return view('admin.game.edit', compact('game', 'pageTitle'));
    }
    
    public function update(Request $request)
    {
        $this->validate($request, [
            'game_id'     => 'required|exists:games,id',
            'name'        => 'required|string|max:255',
            'lane'        => 'required|numeric|digits_between:1,20',
            'start_time'  => 'required|string|min:4|max:255',
            'status'      => 'required|in:started,ended,pending'
        ], [
            'game_id.exists' => 'The game id does not exists in our records.'
        ]);

        try {
            DB::beginTransaction();

            $game = Game::whereId($request->game_id)->first();
            if (empty($game)) return $this->errorResponse('Game not found.');

            $game->update([
                'name' => $request->name,
                'lane' => $request->lane,
                'start_time' => timeIntoString($request->start_time),
                'status'  => $request->status
            ]);
            
            DB::commit();
            return $this->successResponse('Game Updated Succesfully.');

        } catch (\Exception $e) {
            DB::rollBack();
            return $this->errorResponse('Oops! Something went wrong.');
        }
    }

    public function destroy(Request $request)
    {
        $this->validate($request, [
            'game_id' => 'required|exists:games,id'
        ], [
            'game_id.exists' => 'The league id does not exists in our records.'
        ]);

        try {
            DB::beginTransaction();

            $game = Game::with(
                    'game_requests', 'game_scores',
                    'game_scores.scores', 'disputes',
                    'disputes.chats'
                )
                ->whereId($request->game_id)
                ->first();

            if (empty($game)) return $this->errorResponse('League not found.');
    
            foreach($game->game_requests as $game_req) { $game_req->delete(); } // League Game Requests

            // League Game Score
            foreach($game->game_scores as $score) {
                foreach($score->scores as $details) { $details->delete(); }    // League Game Score Details
                $score->delete();
            }

            // League Game Disputes
            foreach($game->disputes as $dispute) {
                foreach($dispute->chats as $chat) { $chat->delete(); }    // League Game Dispute Chats
                $dispute->delete();
            }
            
            $game->delete();
            
            DB::commit();
            return $this->successResponse('Game deleted successfully.');
        } catch (\Exception $e) {
            DB::rollBack();
            return $this->errorResponse('Oops! Something went wrong.');
        }
    }

    public function winner_index()
    {
        $scores = GameScore::with('game.league')->where('is_winner', '1')->get();
        // return $scores;
        $pageTitle = 'Game Winner List';
        return view('admin.game.winner.index', compact('scores', 'pageTitle'));
    }
}
