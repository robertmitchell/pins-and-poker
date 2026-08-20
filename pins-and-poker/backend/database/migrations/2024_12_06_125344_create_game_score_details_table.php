<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateGameScoreDetailsTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('game_score_details', function (Blueprint $table) {
            $table->id();
            $table->foreignId('game_score_id')->references('id')->on('game_scores')->onUpdate('cascade')->onDelete('cascade');
            $table->integer('round_index')->comment('Unique identifier for each round');
            $table->integer('roll_one');
            $table->integer('roll_two')->nullable();
            $table->integer('sum');
            $table->integer('round_index');
            $table->integer('cumulative_score');
            $table->boolean('card_assigned')->default(false);
            
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
        Schema::dropIfExists('game_score_details');
    }
}
