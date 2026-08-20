<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class LeagueRule extends Model
{
    use HasFactory;

    protected $fillable = ['league_id', 'rule_id', 'created_at', 'updated_at'];

    public function rules() {
        return $this->belongsTo(Rule::class, 'rule_id', 'id');
    }
}