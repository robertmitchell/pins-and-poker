<?php

namespace Database\Seeders;

use App\Models\Rule;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\File;

class RuleSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        $json = File::get('database/json/rules.json');
        $rules = collect(json_decode($json));
        $timestamps = \Carbon\Carbon::now();

        $rules->each(function ($rule) use ($timestamps) {
            Rule::create([
                'user_id'     => $rule->user_id,
                'type'        => $rule->type,
                'description' => $rule->description,
                'created_by'  => $rule->created_by ?? 'moderator',
                'created_at'  => $timestamps,
                'updated_at'  => $timestamps
            ]);
        });
    }
}
