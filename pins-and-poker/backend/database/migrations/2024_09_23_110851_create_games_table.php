<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateGamesTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('games', function (Blueprint $table) {
            $table->id();
            $table->foreignId('user_id')->constrained('users')->onUpdate('cascade')->onDelete('cascade');
            $table->foreignId('league_id')->constrained('leagues')->onUpdate('cascade')->onDelete('cascade');
            $table->integer('participants')->default('0');
            $table->string('name')->unique()->nullable();
            $table->integer('lane')->nullable();
            $table->string('start_time')->nullable();
            $table->enum('status', ['pending', 'started', 'ended'])->default('pending');
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
        Schema::dropIfExists('games');
    }
}
