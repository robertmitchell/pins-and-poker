<?php

namespace Database\Seeders;

use App\Models\League;
use Illuminate\Database\Seeder;

class LeagueSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        $timestamps = \Carbon\Carbon::now();
        $leagues = collect([
            [
                'user_id'      => 4,
                'name'         => 'Pirate_League',
                'prize_pool'   => 200,
                'image'        => 'uploads/images/moderator/leagues/default.jpg',
                'start_time'   => "12:00 AM",
                'created_at'   => $timestamps,
                'updated_at'   => $timestamps
            ],
            [
                'user_id'      => 5,
                'name'         => 'Premier_League',
                'prize_pool'   => 5000,
                'image'        => 'uploads/images/moderator/leagues/default.jpg',
                'start_time'   => "05:00 AM",
                'created_at'   => $timestamps,
                'updated_at'   => $timestamps
            ]
        ]);

        $leagues->each(function ($league) {
            League::insert($league);
        });
    }
}