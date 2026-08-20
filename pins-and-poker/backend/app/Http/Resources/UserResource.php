<?php

namespace App\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;

class UserResource extends JsonResource
{
    /**
     * Transform the resource into an array.
     *
     * @param  \Illuminate\Http\Request  $request
     * @return array|\Illuminate\Contracts\Support\Arrayable|\JsonSerializable
     */
    public function toArray($request)
    {        
        $data = [
            'player_id'     => $this->player_id,
             
            'username'      => $this->username,
            'email'         => $this->email,
            'phone'         => $this->phone,
            'image'         => $this->avatar_image,

            'user_type'     => $this->user_type,
            'auth_provider' => $this->auth_provider,
            'platform'      => $this->platform,

            'social_id'     => $this->social_id,
            'device_token'  => $this->device_token,

            'created_at'    => format_date($this->created_at),
            // 'updated_at'    => format_date($this->updated_at)
        ];

        if ($this->access_token !== null) {
            $data['access_token'] = $this->access_token;
        }

        return $data;
    }
}
