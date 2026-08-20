<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class LeagueRequest extends Model
{
    use HasFactory;

    protected $fillable = ['league_id', 'user_id', 'status', 'created_at', 'updated_at'];
    
    public function user()
    {
        return $this->belongsTo(User::class, 'user_id', 'id');
    }

    public function league()
    {
        return $this->belongsTo(League::class, 'league_id', 'id');
    }
}
