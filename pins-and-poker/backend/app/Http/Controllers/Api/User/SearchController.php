<?php

namespace App\Http\Controllers\Api\User;

use App\Http\Controllers\Controller;
use App\Http\Resources\GameResource;
use App\Http\Resources\LeagueResource;
use App\Models\{Game, League};
use Illuminate\Http\Request;

class SearchController extends Controller
{
    public final function leagues_and_games(Request $request)
    {
        $this->validate($request, [
            'search_term' => 'required|string|max:255'
        ]);

        $search = $request->search_term;
        try {
            $leagues = League::where('name', 'like', "%{$search}%")->get();
            $games = Game::where('name', 'like', "%{$search}%")->get();

            $data = [
                'leagues' => LeagueResource::collection($leagues),
                'games'   => GameResource::collection($games)
            ];

            if ($leagues->isEmpty() && $games->isEmpty()) {
                return $this->successResponse("No match result found for '{$search}'.");
            }
            return $this->successDataResponse($data, "Search results retrieved successfully.");

        } catch (\Exception $e) {
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }
}
 