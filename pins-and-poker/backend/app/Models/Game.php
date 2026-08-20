<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Game extends Model
{
    use HasFactory;

    protected $fillable = ['user_id', 'league_id', 'participants', 'name', 'lane', 'start_time', 'status', 'created_at', 'updated_at'];

    public function user()
    {
        return $this->belongsTo(User::class, 'user_id','id');
    }

    public function league()
    {
        return $this->belongsTo(League::class, 'league_id','id');
    }

    public function game_requests()
    {
        return $this->hasMany(GameRequest::class, 'game_id','id');
    }
    
    public function pending_game_requests()
    {
        return $this->hasMany(GameRequest::class, 'game_id','id')
                    ->where('status', '!=', 'accepted');
    }

    public function game_scores()
    {
        return $this->hasMany(GameScore::class, 'game_id','id');
    }

    public function disputes()
    {
        return $this->hasMany(Dispute::class, 'game_id','id');
    }
}
