<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Notification extends Model
{
    use HasFactory;

    protected $fillable = ['user_id', 'title', 'body', 'created_by', 'device_token', 'created_at', 'updated_at'];

    public function user()
    {
        return $this->belongsTo(User::class, 'user_id', 'id');
    }
    
    public function user_seen()
    {
        return $this->hasOne(UserNotification::class, 'notifiable_id', 'id')->where('user_id', auth()->user()->id);
    }
}
