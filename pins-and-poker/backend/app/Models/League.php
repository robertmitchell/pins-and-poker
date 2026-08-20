<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class League extends Model
{
    use HasFactory;

    protected $fillable = ['user_id', 'name', 'participants', 'prize_pool', 'image', 'start_time', 'created_at', 'updated_at'];

    public function user()
    {
        return $this->belongsTo(User::class, 'user_id', 'id');
    }

    public function rules()
    {
        return $this->hasManyThrough(Rule::class, LeagueRule::class, 'league_id', 'id', 'id', 'rule_id');
    }

    public function league_requests()
    {
        return $this->hasMany(LeagueRequest::class, 'league_id','id');
    }
    
    public function pending_league_requests()
    {
        return $this->hasMany(LeagueRequest::class, 'league_id','id')
                    ->where('status', '!=', 'accepted');
    }
    
    public function games()
    {
        return $this->hasMany(Game::class, 'league_id','id');
    }
}
