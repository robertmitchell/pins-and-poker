-- Active: 1689727177185@@127.0.0.1@3306@pinsandpoker_main
<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Str;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;

class CreateUsersTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('users', function (Blueprint $table) {
            $table->id();
            
            $table->unsignedBigInteger('player_id')->unique()->nullable();

            $table->string('username')->nullable();
            $table->string('email')->unique()->nullable();
            $table->string('password')->nullable();
            $table->string('phone')->nullable();
            $table->enum('user_type', ['user', 'moderator', 'admin'])->default('user');
            $table->string('avatar_image')->nullable();

            $table->enum('auth_provider', ['guest', 'normal','google', 'apple'])->nullable();
            $table->enum('platform', ['android', 'ios'])->nullable();

            $table->enum('is_social', ['0', '1'])->default('0');
            $table->enum('is_blocked', ['0', '1'])->default('0');

            $table->string('social_id')->unique()->nullable();
            $table->longText('device_token')->nullable();

            $table->timestamp('email_verified_at')->nullable();
            $table->softDeletes();
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
        Schema::dropIfExists('users');
    }
}
