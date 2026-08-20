<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateChatsTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('chats', function (Blueprint $table) {
            $table->id();

            $table->unsignedBigInteger('disputer_id');
            $table->foreign('disputer_id')->references('player_id')->on('users')->onUpdate('cascade')->onDelete('cascade');

            $table->unsignedBigInteger('disputed_against_id');
            $table->foreign('disputed_against_id')->references('player_id')->on('users')->onUpdate('cascade')->onDelete('cascade');

            $table->unsignedBigInteger('moderator_id');
            $table->foreign('moderator_id')->references('player_id')->on('users')->onUpdate('cascade')->onDelete('cascade');
            
            $table->unsignedBigInteger('sended_by');
            $table->foreign('sended_by')->references('player_id')->on('users')->onUpdate('cascade')->onDelete('cascade');
            
            $table->string('group_id')->nullable();

            $table->longText('message')->nullable();
            $table->enum('type', ['text', 'gif', 'image', 'voice'])->default('text');
            $table->enum('seen', ['0', '1'])->default('0');

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
        Schema::dropIfExists('chats');
    }
}
