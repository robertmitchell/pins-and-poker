<?php

namespace App\Http\Controllers\Admin;

use App\Constants\FileInfo;
use App\Helpers\FileHelper;
use App\Http\Controllers\Controller;
use App\Models\{League};
use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class LeagueController extends Controller
{
    public function index()
    {
        $leagues = League::with('user')->get();

        $pageTitle = 'League List';
        return view('admin.league.index', compact('leagues', 'pageTitle'));
    }

    public function edit($id)
    {
        $league = League::whereId($id)->first();
        
        $pageTitle = 'Edit League';
        return view('admin.league.edit', compact('league', 'pageTitle'));
    }
    
    public function update(Request $request)
    {
        $this->validate($request, [
            'league_id'   => 'required|exists:leagues,id',
            'name'        => 'required|string|max:255',
            'prize_pool'  => 'required|numeric|digits_between:1,20',
            'start_time'  => 'required|string|min:4|max:255',
            'image'       => 'nullable|image|mimes:jpeg,png,jpg|max:2048',
        ], [
            'league_id.exists' => 'The league id does not exists in our records.'
        ]);
        
        try {
            DB::beginTransaction();

            $league = League::whereId($request->league_id)->first();
            if (empty($league)) return $this->errorResponse('League not found.');

            $file = $request->file('image') ?? null;
            $uploadImage = !empty($file) ? FileHelper::handleImageUpload($file, 'moderator', 'league', 'leagues') : null;
            $path = !empty($uploadImage) ? $uploadImage : null;
            $defaultImage = FileInfo::LEAGUE_DEFAULT_IMAGE;
            
            $oldImage = $league->image;
            $league->update([
                'name' => $request->name,
                'prize_pool' => $request->prize_pool,
                'start_time' => timeIntoString($request->start_time),
                'image' => $path ?? $oldImage
            ]);
            
            // DELETE OLD LEAGUE IMAGE
            if (!empty($path) && $oldImage !== $defaultImage) {
                FileHelper::removeOldImage($oldImage);
            }

            DB::commit();
            return $this->successResponse('League Updated Succesfully.');

        } catch (\Exception $e) {
            DB::rollBack();
            return $this->errorResponse('Oops! Something went wrong.');
        }
    }

    public function destroy(Request $request)
    {
        $this->validate($request, [
            'league_id' => 'required|exists:leagues,id'
        ], [
            'league_id.exists' => 'The league id does not exists in our records.'
        ]);

        try {
            DB::beginTransaction();

            $league = League::
                with('rules', 'league_requests',
                    'games.game_requests', 'games.game_scores',
                    'games.game_scores.scores', 'games.disputes',
                    'games.disputes.chats'
                )
                ->whereId($request->league_id)
                ->first();

            if (empty($league)) return $this->errorResponse('League not found.');
            
            foreach($league->rules as $rule) { $rule->delete(); }         // League Rules
            foreach($league->league_requests as $req) { $req->delete(); } // League Requests

            // League Games
            foreach($league->games as $game) {
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
            }
            // League
            $league->delete();
            
            DB::commit();
            return $this->successResponse('League deleted successfully.');
        } catch (\Exception $e) {
            DB::rollBack();
            return $this->errorResponse('Oops! Something went wrong.');
        }
    }
}
