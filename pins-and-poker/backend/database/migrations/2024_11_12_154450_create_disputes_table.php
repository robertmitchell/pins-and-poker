<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateDisputesTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('disputes', function (Blueprint $table) {
            $table->id();

            $table->unsignedBigInteger('moderator_id');
            $table->foreign('moderator_id')->references('player_id')->on('users')->onUpdate('cascade')->onDelete('cascade');

            $table->foreignId('game_id')->constrained()->onUpdate('cascade')->onDelete('cascade');

            $table->unsignedBigInteger('disputer_id');
            $table->foreign('disputer_id')->references('player_id')->on('users')->onUpdate('cascade')->onDelete('cascade');

            $table->unsignedBigInteger('disputed_against_id');
            $table->foreign('disputed_against_id')->references('player_id')->on('users')->onUpdate('cascade')->onDelete('cascade');

            $table->string('cell_index')->nullable();

            $table->string('dispute_group_id')->nullable();

            $table->enum('status', ['pending','resolved'])->default('pending');
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
        Schema::dropIfExists('disputes');
    }
}
