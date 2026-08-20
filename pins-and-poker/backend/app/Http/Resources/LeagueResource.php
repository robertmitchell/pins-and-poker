<?php

namespace App\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;

class LeagueResource extends JsonResource
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
            'participants' => $this->participants,
            'prize_pool'   => $this->prize_pool,
            'image'        => $this->image,
            'start_time'   => $this->start_time,
            'created_at'   => format_date($this->created_at),
            'rules'        => RuleResource::collection($this->rules),
            'requests'     => $this->league_requests->map(function ($req) {
                return [
                    'status' => $req->status,
                    'created_at' => format_date($req->created_at),
                    'user'  => [
                        'player_id' => $req->user->player_id,
                        'username'  => $req->user->username,
                        'image'     => $req->user->avatar_image,
                    ],
                ];
            })
            // 'requests'     => $this->league_requests->map(function ($req) {
            //     return [
            //         'status' => $req->status,
            //         'created_at' => format_date($req->created_at),
            //         'user'  => $req->user ? [
            //             'player_id' => $req->user->player_id,
            //             'username'  => $req->user->username,
            //             'image'     => $req->user->avatar_image,
            //         ] : [],
            //     ];
            // })
        ];
    }
}