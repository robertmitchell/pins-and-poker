<?php

namespace App\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;

class GameResource extends JsonResource
{
    /**
     * Transform the resource into an array.
     *
     * @param  \Illuminate\Http\Request  $request
     * @return array|\Illuminate\Contracts\Support\Arrayable|\JsonSerializable
     */
    public function toArray($request)
    {
        return [
            'id'           => $this->id,
            'player_id'    => $this->user->player_id,
            'name'         => $this->name,
            'lane'         => $this->lane,
            'start_time'   => $this->start_time,
            'participants' => $this->participants,
            'status'       => $this->status,
            'created_at'   => format_date($this->created_at),
            'requests'     => $this->game_requests->map(function ($req) {
                return [
                    'status'        => $req->status,
                    'assigned_lane' => $req->assigned_lane,
                    'created_at'    => format_date($req->created_at),
                    'user'          => [
                        'player_id' => $req->user->player_id,
                        'username'  => $req->user->username,
                        'image'     => $req->user->avatar_image,
                    ]
                ];
            })
        ];
    }
}
