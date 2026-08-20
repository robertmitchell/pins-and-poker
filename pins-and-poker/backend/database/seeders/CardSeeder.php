<?php

namespace Database\Seeders;

use App\Models\Card;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\File;

class CardSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {   
        $json = File::get('database/json/cards.json');
        $cards = collect(json_decode($json));
        $timestamps = \Carbon\Carbon::now();

        $cards->each(function ($card) use ($timestamps) {
            Card::create([
                'name'       => $card->name,
                'suit'       => $card->suit,
                'rank'       => $card->rank,
                'created_at' => $timestamps,
                'updated_at' => $timestamps
            ]);
        });
    }
}
