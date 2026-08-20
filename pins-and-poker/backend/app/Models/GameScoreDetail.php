<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class GameScoreDetail extends Model
{
    use HasFactory;
    
    protected $fillable = [
        'game_score_id',
        'roll_one',
        'roll_two',
        'sum',
        'cumulative_score',
        'card_assigned','round_index'
    ];
}
