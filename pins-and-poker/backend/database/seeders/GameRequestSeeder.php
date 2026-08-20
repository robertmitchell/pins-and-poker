<?php

namespace Database\Seeders;

use App\Models\GameRequest;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\File;

class GameRequestSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        $json = File::get('database/json/game_requests.json');
        $game_requests = collect(json_decode($json));
        $timestamps = \Carbon\Carbon::now();

        $game_requests->each(function ($game_request) use ($timestamps) {
            GameRequest::create([
                'game_id'       => $game_request->game_id,
                'user_id'       => $game_request->user_id,
                'assigned_lane' => $game_request->assigned_lane ?? null,
                'status'        => $game_request->status,
                'created_at'    => $timestamps,
                'updated_at'    => $timestamps
            ]);
        });
    }
}
