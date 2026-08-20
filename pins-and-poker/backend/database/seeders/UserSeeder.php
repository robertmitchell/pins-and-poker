<?php

namespace Database\Seeders;

use App\Constants\RoleType;
use App\Models\User;
use Illuminate\Database\Seeder;
use Illuminate\Support\Str;
use Illuminate\Support\Facades\File;
use Illuminate\Support\Facades\Hash;

class UserSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        $json = File::get('database/json/users.json');
        $users = collect(json_decode($json));
        $timestamps = \Carbon\Carbon::now();

        $users->each(function ($user) use ($timestamps) {

            if ($user->user_type === RoleType::MODERATOR) {
                $password = Hash::make('user1234');
            }

            User::create([
                'player_id'     => $user->player_id,
                'username'      => $user->username,
                'email'         => $user->email,
                'password'      => $password ?? null,
                'phone'         => $user->phone ?? null,
                'avatar_image'  => 'uploads/images/user/default.png',
                'user_type'     => $user->user_type ?? 'user',
                'auth_provider' => $user->auth_provider,
                'platform'      => $user->platform,
                'is_social'     => $user->is_social ?? '0',
                'social_id'     => $user->social_id ?? null,
                'device_token'  => $user->device_token ?? Str::random(40),
                'email_verified_at' => $timestamps,
                'created_at'    => $timestamps,
                'updated_at'    => $timestamps
            ]);
        });
    }
}
