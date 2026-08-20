<?php

namespace App\Http\Resources;

use Illuminate\Http\Resources\Json\JsonResource;

class GameScoreResource extends JsonResource
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
            'player_id'      => $this->user->player_id,
            'username'       => $this->user->username,
            'image'          => $this->user->avatar_image,
            'rolls'          => json_decode($this->rolls),
            'cell_scores'    => json_decode($this->cell_scores),
            'cards'          => json_decode($this->cards),
            // 'rolls'          => $this->rolls,
            // 'cell_scores'    => $this->cell_scores,
            // 'cards'          => $this->cards,
            'exchange_cards' => ($this->exchange_card === '1') ? true : false,
            'poker_hands'    => $this->poker_hands,
            'is_winner'      => ($this->is_winner === '1') ? true : false
        ];
    }
}
