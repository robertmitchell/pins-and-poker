<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateLeagueRulesTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('league_rules', function (Blueprint $table) {
            $table->id();
            $table->foreignId('league_id')->references('id')->on('leagues')->onUpdate('cascade')->onDelete('cascade');
            $table->foreignId('rule_id')->references('id')->on('rules')->onUpdate('cascade')->onDelete('cascade');
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
        Schema::dropIfExists('league_rules');
    }
}
