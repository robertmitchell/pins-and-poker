<?php

namespace Database\Seeders;

use App\Models\Game;
use Illuminate\Database\Seeder;

class GameSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        $timestamps = \Carbon\Carbon::now();
        $games = collect([
            [
                'user_id'      => 4,
                'league_id'    => 1,
                'participants' => 1,
                'name'         => 'Poker',
                'lane'         => 1,
                'start_time'   => "12:00 AM",
                'created_at'   => $timestamps,
                'updated_at'   => $timestamps
            ],
            [
                'user_id'      => 5,
                'league_id'    => 2,
                'participants' => 1,
                'name'         => 'Rock Carding',
                'lane'         => 1,
                'start_time'   => "05:00 AM",
                'created_at'   => $timestamps,
                'updated_at'   => $timestamps
            ]
        ]);

        $games->each(function ($game) {
            Game::insert($game);
        });
    }
}
