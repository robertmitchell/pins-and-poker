<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Chat extends Model
{
    use HasFactory;

    protected $fillable = ['disputer_id','disputed_against_id','moderator_id','sended_by','message','type','seen','group_id'];
}
