<?php

namespace Database\Seeders;

use App\Models\LeagueRule;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\File;

class LeagueRuleSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        $json = File::get('database/json/league_rules.json');
        $leagueRules = collect(json_decode($json));
        $timestamps = \Carbon\Carbon::now();

        $leagueRules->each(function ($rule) use ($timestamps) {
            LeagueRule::create([
                'league_id'   => $rule->league_id,
                'rule_id'     => $rule->rule_id,
                'created_at'  => $timestamps,
                'updated_at'  => $timestamps
            ]);
        });
    }
}
