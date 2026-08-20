<?php

namespace App\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;

class DisputeResource extends JsonResource
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
            'league_id'             => $this->game->league->id,
            'league_name'           => $this->game->league->name,
            'game_id'               => $this->game_id,
            'game_name'             => $this->game->name,
            'moderator_id'          => $this->moderator_id,
            'disputer_id'           => $this->disputer_id,
            'disputer_name'         => $this->disputer->username,
            'disputed_against_id'   => $this->disputed_against_id,
            'disputed_against_name' => $this->disputed_against->username,
            'group_id'              => $this->dispute_group_id,
            'cell_index'            => $this->cell_index,
            'status'                => $this->status,
            'created_at'            => format_date($this->created_at)
        ];
    }
}
