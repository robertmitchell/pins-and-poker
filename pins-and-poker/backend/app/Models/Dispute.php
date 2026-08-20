<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Dispute extends Model
{
    use HasFactory;

    protected $fillable = [
        'moderator_id',
        'game_id',
        'disputer_id',
        'disputed_against_id',
        'cell_index',
        'dispute_group_id',
        'status'
    ];

    public function game()
    {
        return $this->belongsTo(Game::class, 'game_id','id');
    }

    public function disputer()
    {
        return $this->belongsTo(User::class, 'disputer_id', 'player_id');
    }

    public function disputed_against()
    {
        return $this->belongsTo(User::class, 'disputed_against_id','player_id');
    }

    public function chats()
    {
        return $this->hasMany(Chat::class, 'group_id','dispute_group_id');
    }
}
