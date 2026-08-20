<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class GameScore extends Model
{
    use HasFactory;

    
    protected $fillable = ['game_id', 'user_id', 'total_score', 'cards', 'exchange_card', 'poker_hands', 'is_winner', 'created_at', 'updated_at','last_exchanged_card'];
    
    protected $casts = [
        'rolls' => 'array',
        'cell_scores' => 'array',
        'cards' => 'array',
    ];

    public function user()
    {
        return $this->belongsTo(User::class, 'user_id', 'id');
    }

    public function game()
    {
        return $this->belongsTo(Game::class, 'game_id', 'id');
    }
    
    public function scores()
    {
        return $this->hasMany(GameScoreDetail::class, 'game_score_id', 'id');
    }
}
