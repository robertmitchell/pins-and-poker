<?php

namespace App\Http\Controllers\Api\User;

use App\Constants\Status;
use App\Http\Controllers\Controller;
use App\Models\Card;
use App\Models\GameScore;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class CardController extends Controller
{
    public final function exchange_card(Request $request)
    {
        $this->validate($request, [
            'game_id'    => 'required|numeric|digits_between:1,20|exists:games,id',
            'card_index' => 'required|numeric|digits_between:1,20'
        ], [
            'game_id.exists' => 'The game id does not exist in our records.'
        ]);

        $authUser = auth()->user();

        try {
            DB::beginTransaction();

            $score = GameScore::where('game_id', $request->game_id)->where('user_id', $authUser->id)->first();
            
            if ($score->exchange_card === Status::NOT_EXCHANGE) {
                return $this->errorResponse('You do not have a card to exchange.');
            }

            $user_cards = (!empty($score) && !empty($score->cards)) ? json_decode($score->cards, true) : [];
            
            if (!isset($user_cards[$request->card_index]))
            return $this->errorResponse('The card index does not exists in the array.');
            
            if (empty($user_cards)) {
                $available_cards = Card::all();
            } else {
                $available_cards = Card::whereNotIn('id', $user_cards)->get();
            }

            if ($available_cards->isEmpty()) {
                return $this->errorResponse('No cards left to exchange.');
            } 

            $random_card = $available_cards->random();
            $user_cards[$request->card_index] = $random_card->id;

            $score->cards = json_encode($user_cards);
            $score->exchange_card = Status::NOT_EXCHANGE;
            $score->save();

            $data = ['cards' => json_decode($score->cards)];

            DB::commit();
            return $this->successDataResponse($data, "You've successfully exchanged your card.");

        } catch (\Exception $e) {
            DB::rollBack();
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }
}