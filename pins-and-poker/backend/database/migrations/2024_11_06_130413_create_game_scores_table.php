<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateGameScoresTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('game_scores', function (Blueprint $table) {
            $table->id();
            $table->foreignId('game_id')->references('id')->on('games')->onUpdate('cascade')->onDelete('cascade');
            $table->foreignId('user_id')->references('id')->on('users')->onUpdate('cascade')->onDelete('cascade');
            $table->integer('total_score')->nullable();
            $table->json('cards')->nullable();
            $table->enum('exchange_card', ['0', '1'])->default('0');
            $table->integer('last_exchanged_card')->nullable();
            $table->enum('poker_hands', ['none','RoyalFlush','StraightFlush','FourOfAKind','FullHouse','Flush','Straight','ThreeOfAKind','TwoPair','OnePair','HighCard'])->default('none');
            $table->enum('is_winner', ['0', '1'])->default('0');
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('game_scores');
    }
}
