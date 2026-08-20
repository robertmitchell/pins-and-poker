<?php

namespace App\Http\Controllers\Api;

use App\Constants\RoleType;
use App\Constants\Status;
use App\Http\Controllers\Controller;
use App\Http\Requests\Game\Score\UpdateRequest;

use App\Models\Card;
use App\Models\GameScore;
use App\Models\GameScoreDetail;
use App\Models\League;
use App\Models\User;
use App\Services\PokerHandEvaluator;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Log;

class ScoreController extends Controller
{
    final public function update(UpdateRequest $request)
    {
        $authUser = auth()->user();
        
        try {
            DB::beginTransaction();

            // Validate authorization and fetch user/game/league
            $user = User::where('player_id', $request->player_id)->firstOrFail();
            
            if ($user->user_type !== RoleType::PLAYER ||
                ($authUser->user_type === RoleType::PLAYER && $request->player_id != $authUser->player_id)) {
                return $this->errorResponse('Unauthorized access.', 403);
            }

            $league = League::whereId($request->league_id)->first();
            if (empty($league)) {
                return $this->errorResponse('League Not Found.', 404);
            }

            $game = $league->games()->whereId($request->game_id)->first();
            if (empty($game)) {
                return $this->errorResponse('The requested game does not exist in the given league.', 404);
            }

            $rules = $league->rules()->pluck('rule_id')->toArray();
            $isRestrictCards = in_array(3, $rules);
            $isAllowCards = in_array(4, $rules);
            $isNoCards = !$isRestrictCards && !$isAllowCards;
            $rolls = json_decode($request->rolls, true);
            
            // Fetch or initialize `game_scores`
            $gameScore = GameScore::firstOrNew(
                ['game_id' => $game->id, 'user_id' => $user->id],
                ['total_score' => 0, 'cards' => json_encode([]), 'exchange_card' => 0, 'last_exchanged_card' => 0]
            );

            $cell_scores = [];
            $existingCards = json_decode($gameScore->cards, true) ?? [];
            $currentCardCount = count($existingCards);
            
            $newCardsToAssign = 0;
            $exchangeCardTriggered = false;
            // Process rolls in pairs
            $cumulativeScore = 0;

            foreach (array_chunk($rolls, 2) as $roundIndex => $chunk) {
                $rollOne = $chunk[0] ?? null;
                $rollTwo = $chunk[1] ?? null;
                $indexSum = $roundIndex + 1;
                if ($rollOne == 10 && $indexSum != 10) {
                    $rollTwo = 0;
                }
                
                $roundSum = $rollOne + $rollTwo;
                
                $existingDetail = GameScoreDetail::where('game_score_id', $gameScore->id)
                    ->where('round_index', $indexSum)
                    ->first();
                
                $cumulativeScore += $roundSum;
                $assignedCard = false;

                if (!$isNoCards) {
                    if ($rollOne == 10 || $rollTwo == 10 || $roundSum >= 10) {
                        $newCardsToAssign++;
                        $assignedCard = true;
                    }
                    
                    while ($currentCardCount < $newCardsToAssign) {
                        if ($isRestrictCards && $currentCardCount >= 5) {
                            if ($rollOne == 10 || $rollTwo == 10 || $roundSum >= 10){
                                if(empty($existingDetail)){
                                    $exchangeCardTriggered = true;
                                }elseif($existingDetail->card_assigned == 0){
                                    $exchangeCardTriggered = true;
                                }
                            }
                            break;
                        }
                        
                        $newCard = $this->assignCard($game->id, $user->id, $rules);
                        $existingCards[] = $newCard;
                        $currentCardCount++;
                    }
                }
                
                if ($existingDetail) {
                    // Update existing entry
                    if ($existingDetail->round_index == 10) {
                        if ($roundSum <= 9) {
                            GameScoreDetail::where('game_score_id', $gameScore->id)
                                ->where('round_index', $roundIndex + 2)
                                ->delete();
                        }
                        $existingDetail->update([
                            'roll_one' => $rollOne,
                            'roll_two' => $rollTwo,
                            'sum' => $roundSum,
                            'cumulative_score' => $cumulativeScore,
                        ]);
                    } else {
                    // elseif ($existingDetail->round_index == 11) {
                    //     if ($rollOne == 0) {
                    //         $existingDetail->delete();
                    //     }
                    // } else {
                        
                        $existingDetail->update([
                            'roll_one' => $rollOne,
                            'roll_two' => $rollTwo,
                            'sum' => $roundSum,
                            'cumulative_score' => $cumulativeScore,
                            'card_assigned' => $assignedCard,
                        ]);
                    }
                } else {

                    $dd = GameScoreDetail::where('game_score_id', $gameScore->id)
                        ->where('round_index', $roundIndex)
                        ->first();
                    
                    if (isset($dd) && $dd->sum <= 9 && $roundIndex + 1 == 11) {
                        
                    } else {
                        // Create new entry
                        GameScoreDetail::create([
                            'game_score_id' => $gameScore->id,
                            'round_index' => $roundIndex + 1,
                            'roll_one' => $rollOne,
                            'roll_two' => $rollTwo,
                            'sum' => $roundSum,
                            'cumulative_score' => $cumulativeScore,
                            'card_assigned' => $assignedCard,
                        ]);
                    }
                }

                $cell_scores[] = $cumulativeScore;
            }

            // Update `game_scores`
            $gameScore->update([
                'total_score' => $cumulativeScore,
                'cards' => json_encode($existingCards),
                'exchange_card' => $exchangeCardTriggered ? '1' : '0',
            ]);

            $gameScoreDetails = GameScoreDetail::where('game_score_id', $gameScore->id)->orderBy('round_index');
            $cumulative_score = $gameScoreDetails->pluck('cumulative_score');
            $rolls_get = $gameScoreDetails->get();
            
            // Combine `roll_one` and `roll_two` into a single array
            $rollsArray = $rolls_get->flatMap(function ($detail) {
                if (isset($detail->roll_two)) {
                    if ($detail->round_index == 11 || $detail->round_index == 10 && !isset($detail->roll_two)) {
                        return [$detail->roll_one];
                    } else{
                        return [$detail->roll_one, $detail->roll_two];
                    }
                } else {
                    return [$detail->roll_one];
                }
            })->toArray();

            $score_array = [];
            $score_array[] = [
                'player_id' => $user->player_id,
                'username'  => $user->username,
                'image' => $user->avatar_image,
                'rolls' => $rollsArray,
                'cell_scores' => $cumulative_score,
                'cards' => json_decode($gameScore->cards),
                'exchange_cards' => ($gameScore->exchange_card === '1') ? true : false,
                'poker_hands' => $gameScore->poker_hands,
                'is_winner' => ($gameScore->is_winner === '1') ? true : false,
            ];
            $data = [
                'status' => $game->status,
                'score' => $score_array,
            ];

            DB::commit();
            return $this->successDataResponse($data, 'Game Score Updated Successfully.');
        } catch (\Exception $e) {
            DB::rollBack();
            return $this->errorResponse("Error: {$e->getMessage()}", 500);
        }
    }

    final private function checkUserEligibility($user, $league, $game)
    {
        if ($user->user_type === RoleType::MODERATOR) {
            // Moderator checks: They must manage their own leagues and games
            if ($league->user_id !== $user->id) {
                return $this->errorResponse('You are not authorized to manage this league.', 403);
            }

            if ($game->user_id !== $user->id) {
                return $this->errorResponse('You are not authorized to manage this league game.', 403);
            }

        } else {
            // Player checks: They must be a member of the league and game
            $hasJoinedLeague = $league->league_requests()->where('user_id', $user->id)->where('status', Status::ACCEPTED)->exists();
            $hasJoinedGame = $game->game_requests()->where('user_id', $user->id)->where('status', Status::ACCEPTED)->exists();

            if (empty($hasJoinedLeague)) {
                return $this->errorResponse("You're not a participant in this league or your request has not been accepted.");
            }

            if (empty($hasJoinedGame)) {
                return $this->errorResponse("You're not a participant in this league game or your request has not been accepted.");
            }

        }

        return null; // Return null if eligibility is valid
    }

    final private function validateRolls($rolls)
    {
        // Check if the decoded data is an array
        if (!is_array($rolls)) {
            return $this->errorResponse('Invalid rolls format. Please provide an array.', 422);
        }

        // Check if the array is empty
        if (empty(($rolls))) {
            return $this->errorResponse('The provided array is empty. Please provide valid data.', 422);
        }

        // Check the array has not greater than 20 values
        if (count($rolls) > 20) {
            return $this->errorResponse('The array must contain 20 values or less.', 422);
        }

        // Check if the array value is integer
        foreach ($rolls as $roll) {
            if (!is_int($roll)) {
                return $this->errorResponse("The roll value '{$roll}' must be an integer.", 422);
            }

            if ($roll < 0 || $roll > 10) {
                return $this->errorResponse("The roll value '{$roll}' must be between 0 and 10.", 422);
            }

        }

        return null; // return null if validated
    }

    final public function get_game_scores(Request $request)
    {
        $this->validate($request, [
            'league_id' => 'required|numeric|digits_between:1,20|exists:leagues,id',
            'game_id' => 'required|numeric|digits_between:1,20|exists:games,id',
        ], [
            'league_id.exists' => 'The league id does not exist in our records.',
            'game_id.exists' => 'The game id does not exist in our records.',
        ]);

        $authUser = auth()->user();
        
        try {
            DB::beginTransaction();
            $league = League::whereId($request->league_id)->first();
            if (empty($league)) {
                return $this->errorResponse('League Not Found.', 404);
            }
            
            $game = $league->games()->whereId($request->game_id)->first();
            if (empty($game)) {
                return $this->errorResponse('The requested game does not exist in the given league.', 404);
            }
            
            // Check eligibility based on user role (MODERATOR or PLAYER)
            $userEligibility = $this->checkUserEligibility($authUser, $league, $game);
            if ($userEligibility) {
                return $userEligibility;
            }

            $hand_rankings = [
                'RoyalFlush' => 10,
                'StraightFlush' => 9,
                'FourOfAKind' => 8,
                'FullHouse' => 7,
                'Flush' => 6,
                'Straight' => 5,
                'ThreeOfAKind' => 4,
                'TwoPair' => 3,
                'OnePair' => 2,
                'HighCard' => 1,
            ];

            // Fetch all game scores for the given game
            $scores = GameScore::where('game_id', $game->id)->get();
            if ($authUser->user_type === RoleType::PLAYER) {
                $evaluated_hands = [];
                $highest_hand_rank = -1;
                $winner = null;

                $allRollsAreTwenty = true;

                foreach ($scores as $score) {
                    $gameScoreDetails = GameScoreDetail::where('game_score_id', $score->id)->orderBy('round_index')->get();

                    // Combine `roll_one` and `roll_two` into a single array
                    $rolls = $gameScoreDetails->flatMap(function ($detail) {
                        if (isset($detail->roll_two)) {
                            return [$detail->roll_one, $detail->roll_two];
                        } else {
                            return [$detail->roll_one];
                        }
                    })->toArray();

                    if (!is_array($rolls) || count($rolls) < 20) {
                        // If any roll count is not 20, set $allRollsAreTwenty to false and break the loop
                        $allRollsAreTwenty = false;
                        break;
                    }

                    // Ensure rolls is an array
                    if (is_array($rolls) && count($rolls) >= 20) {
                        // Evaluate the hand using the PokerHandEvaluator
                        $hand = PokerHandEvaluator::evaluateHand(json_decode($score->cards));

                        // Update the 'poker_hands' column with the evaluated hand
                        $score->poker_hands = $hand;
                        $score->save();

                        // Store the evaluated hands and score
                        $evaluated_hands[] = [
                            'score' => $score,
                            'hand' => $hand,
                        ];

                        // Check if this hand is better than the current highest hand
                        if (isset($hand_rankings[$hand]) && $hand_rankings[$hand] > $highest_hand_rank) {
                            $highest_hand_rank = $hand_rankings[$hand];
                            $winner = $score;
                        }
                    }
                }

                $created_score = $scores->where('user_id', $authUser->id)->first();

                if (empty($created_score)) {
                    $empty_array = json_encode([]);

                    GameScore::create([
                        'game_id' => $game->id,
                        'user_id' => $authUser->id,
                        'rolls' => $empty_array,
                        'cell_scores' => $empty_array,
                        'cards' => $empty_array,
                    ]);
                }

                $game_scores = GameScore::where('game_id', $game->id)->get();

                $scoreee = [];
                foreach ($game_scores as $item) {
                    $scores = $item->scores;
                    $cell_indexs = $scores->flatMap(function ($detail) {
                        return [$detail->cumulative_score];
                    })->toArray();

                    $rolls = $scores->flatMap(function ($detail) {
                        if (isset($detail->roll_two)) {
                            if ($detail->round_index == 11 || $detail->round_index == 10 && !isset($detail->roll_two)) {
                                return [$detail->roll_one];
                            } else{
                                return [$detail->roll_one, $detail->roll_two];
                            }
                        } else {
                            return [$detail->roll_one];
                        }
                    })->toArray();

                    $scoreee[] = [
                        'player_id' => $item->user->player_id,
                        'username'  => $item->user->username,
                        'image' => $item->user->avatar_image,
                        'rolls' => $rolls,
                        'cell_scores' => $cell_indexs,
                        'cards' => json_decode($item->cards),
                        'exchange_cards' => ($item->exchange_card === '1') ? true : false,
                        'poker_hands' => $item->poker_hands,
                        'is_winner' => ($item->is_winner === '1') ? true : false,
                    ];
                }
                
                $winnerMade = false; // Default value
                foreach ($game_scores as $item) {
                    $scores = $item->scores;
                    
                    // Check for round 10 and round 11 conditions
                    $round10 = $scores->firstWhere('round_index', 10); // Get round_index 10
                    $round11 = $scores->firstWhere('round_index', 11); // Get round_index 11 (if it exists)
                    
                    if(isset($round10)){
                        if($round10->sum >= 10){
                            if ($round11) {
                                $winnerMade = true;
                            } else {
                                $winnerMade = false;
                                break;
                            }
                        }else{
                            if(isset($round10->roll_one) && isset($round10->roll_two)){
                                $winnerMade = true;
                            }else{
                                $winnerMade = false;
                                break;
                            }
                        }
                    }else{
                        $winnerMade = false;
                        break;
                    }
                    
                }
                
                // Once the loop is complete, set the 'is_winner' field for the player with the highest hand
                if ($winner && $winnerMade) {
                    $winner->is_winner = Status::GAME_WIN;
                    $winner->save();

                    $game->status = 'ended';
                    $game->save();
                }
            } else {
                $game_scores = GameScore::where('game_id', $game->id)->get();

                $scoreee = [];
                foreach ($game_scores as $item) {
                    $scores = $item->scores;
                    $cell_indexs = $scores->flatMap(function ($detail) {
                        return [$detail->cumulative_score];
                    })->toArray();

                    $rolls = $scores->flatMap(function ($detail) {
                        if (isset($detail->roll_two)) {
                            if ($detail->round_index == 11 || $detail->round_index == 10 && !isset($detail->roll_two)) {
                                return [$detail->roll_one];
                            } else{
                                return [$detail->roll_one, $detail->roll_two];
                            }
                        } else {
                            return [$detail->roll_one];
                        }
                    })->toArray();

                    $scoreee[] = [
                        'player_id' => $item->user->player_id,
                        'username' => $item->user->username,
                        'image' => $item->user->avatar_image,
                        'rolls' => $rolls,
                        'cell_scores' => $cell_indexs,
                        'cards' => json_decode($item->cards),
                        'exchange_cards' => ($item->exchange_card === '1') ? true : false,
                        'poker_hands' => $item->poker_hands,
                        'is_winner' => ($item->is_winner === '1') ? true : false,
                    ];
                }
            }

            $data = [
                'status' => $game->status,
                'score' => $scoreee,
            ];

            $message = "Game Score Fetched Successfully.";
            DB::commit();
            return $this->successDataResponse($data, $message);

        } catch (\Exception $e) {
            DB::rollBack();
            return $this->errorResponse("Error: {$e->getMessage()}", 500);
            // return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }

    final private function assignCard($game_id, $user_id, $rules)
    {
        // Check if rule 5 is allowed
        $isAllowCards = in_array(5, $rules);
        // Determine the cards to fetch based on $rules
        if ($isAllowCards) {
             $available_cards_query = Card::query(); // Fetch all cards
        } else {
             // Fetch cards with IDs 1 to 52
            $available_cards_query = Card::whereBetween('id', [1, 52]);
        }
        // Get the user's score record
        $score = GameScore::where('game_id', $game_id)->where('user_id', $user_id)->first();
        $user_cards = (!empty($score) && !empty($score->cards)) ? json_decode($score->cards, true) : [];
        // Exclude already assigned cards
        if (!empty($user_cards)) {
            $available_cards_query->whereNotIn('id', $user_cards);
        }
        $available_cards = $available_cards_query->get();
        // If there are available cards to assign, pick a random one
        if ($available_cards->isNotEmpty()) {
            $random_card = $available_cards->random(); // Pick a random card

            return $random_card->id; // Return the assigned card ID
        }

        return null; // No cards available to assign
    }
}
