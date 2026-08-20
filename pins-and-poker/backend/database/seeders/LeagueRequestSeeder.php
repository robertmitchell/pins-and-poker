<?php

namespace Database\Seeders;

use App\Models\LeagueRequest;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\File;

class LeagueRequestSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        $json = File::get('database/json/league_requests.json');
        $requests = collect(json_decode($json));
        $timestamps = \Carbon\Carbon::now();

        $requests->each(function ($request) use ($timestamps) {
            LeagueRequest::create([
                'league_id'   => $request->league_id,
                'user_id'     => $request->user_id,
                'status'      => $request->status,
                'created_at'  => $timestamps,
                'updated_at'  => $timestamps
            ]);
        });
    }
}
