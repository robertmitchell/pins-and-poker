<?php

namespace App\Services;

use Illuminate\Support\Collection;
use App\Models\Card;

class PokerHandEvaluator
{
    const RANKS = [
        'Two' => 2,
        'Three' => 3,
        'Four' => 4,
        'Five' => 5,
        'Six' => 6,
        'Seven' => 7,
        'Eight' => 8,
        'Nine' => 9,
        'Ten' => 10,
        'Jack' => 11,
        'Queen' => 12,
        'King' => 13,
        'Ace' => 14
    ];

    const POKER_HANDS = [
        'HighCard' => 1,
        'OnePair' => 2,
        'TwoPair' => 3,
        'ThreeOfAKind' => 4,
        'Straight' => 5,
        'Flush' => 6,
        'FullHouse' => 7,
        'FourOfAKind' => 8,
        'StraightFlush' => 9,
        'RoyalFlush' => 10
    ];

    /**
     * Evaluate the best poker hand for a given set of card IDs.
     *
     * @param array $cardIds
     * @return string
     */
    public static function evaluateHand(array $cardIds)
    {
        // Fetch cards from the database
        $cards = Card::whereIn('id', $cardIds)->get();

        if ($cards->isEmpty()) {
            throw new \InvalidArgumentException("No valid cards found for the provided IDs.");
        }

        // Separate Jokers from other cards
        $jokers = $cards->filter(fn($card) => in_array($card->id, [53, 54]));
        $nonJokerCards = $cards->filter(fn($card) => !in_array($card->id, [53, 54]));

        // Evaluate the hand
        if ($jokers->isNotEmpty()) {
            $rank = self::evaluateHandWithJokers($nonJokerCards, $jokers);
        } else {
            $rank = self::evaluateNormalHand($nonJokerCards);
        }

        // Return the hand name
        return array_flip(self::POKER_HANDS)[$rank];
    }

    /**
     * Evaluate a hand without jokers.
     *
     * @param Collection $cards
     * @return int
     */
    private static function evaluateNormalHand(Collection $cards)
    {
        // Sort by rank and group by rank and suit
        $sortedCards = $cards->sortBy('rank');
        $rankGroups = $sortedCards->groupBy('rank')->sortByDesc(fn($group) => $group->count())->values();
        $suitGroups = $sortedCards->groupBy('suit');

        // Check for flush
        $isFlush = $suitGroups->contains(fn($group) => $group->count() >= 5);

        // Check for straight
        $isStraight = self::isStraight($sortedCards->pluck('rank')->toArray());

        // Determine hand ranking
        if ($isStraight && $isFlush && $sortedCards->last()->rank == self::RANKS['Ace']) {
            return self::POKER_HANDS['RoyalFlush'];
        }

        if ($isStraight && $isFlush) {
            return self::POKER_HANDS['StraightFlush'];
        }

        if ($rankGroups[0]->count() == 4) {
            return self::POKER_HANDS['FourOfAKind'];
        }

        if ($rankGroups[0]->count() == 3 && $rankGroups->get(1)?->count() == 2) {
            return self::POKER_HANDS['FullHouse'];
        }

        if ($isFlush) {
            return self::POKER_HANDS['Flush'];
        }

        if ($isStraight) {
            return self::POKER_HANDS['Straight'];
        }

        if ($rankGroups[0]->count() == 3) {
            return self::POKER_HANDS['ThreeOfAKind'];
        }

        if ($rankGroups[0]->count() == 2 && $rankGroups->get(1)?->count() == 2) {
            return self::POKER_HANDS['TwoPair'];
        }

        if ($rankGroups[0]->count() == 2) {
            return self::POKER_HANDS['OnePair'];
        }

        return self::POKER_HANDS['HighCard'];
    }

    private static function evaluateHandWithJokers(Collection $nonJokerCards, Collection $jokers)
    {
        $bestRank = self::POKER_HANDS['HighCard'];
        $jokerCount = $jokers->count();
    
        // Extract non-joker ranks and suits
        $nonJokerRanks = $nonJokerCards->pluck('rank')->toArray();
        $nonJokerSuits = $nonJokerCards->pluck('suit')->toArray();
    
        // Generate optimized possible hands with jokers
        $allPossibleHands = self::generateOptimizedHandsWithJokers($nonJokerRanks, $nonJokerSuits, $jokerCount);
        // $dd = [];
        // Evaluate each hand and find the best rank
        foreach ($allPossibleHands as $hand) {
            $rank = self::checkHand(collect($hand));
            // $dd[] = collect($hand); 
            if ($rank > $bestRank) {
                $bestRank = $rank;
            }
        }
        // return $dd;
        return $bestRank;
    }
    
    /**
     * Generate optimized possible hands by substituting jokers.
     *
     * @param array $nonJokerRanks
     * @param array $nonJokerSuits
     * @param int $jokerCount
     * @return array
     */
    private static function generateOptimizedHandsWithJokers(array $nonJokerRanks, array $nonJokerSuits, int $jokerCount)
    {
        $allRanks = array_values(self::RANKS);
        $allSuits = ['Hearts', 'Diamonds', 'Clubs', 'Spades'];
    
        // Step 1: Start with the base hand
        $hands = [[
            'ranks' => $nonJokerRanks,
            'suits' => $nonJokerSuits
        ]];
    
        // Step 2: Use jokers to strengthen existing groups
        for ($i = 0; $i < $jokerCount; $i++) {
            $newHands = [];
    
            foreach ($hands as $hand) {
                $currentRanks = $hand['ranks'];
                $currentSuits = $hand['suits'];
                $rankCounts = array_count_values($currentRanks);
    
                // Add jokers to form stronger combinations
                foreach ($allRanks as $rank) {
                    $newRanks = $currentRanks;
                    $newSuits = $currentSuits;
    
                    // Try to form Four of a Kind, Three of a Kind, etc.
                    if (isset($rankCounts[$rank]) && $rankCounts[$rank] < 4) {
                        $newRanks[] = $rank;
                        $newSuits[] = 'Wildcard'; // Joker doesn't need a specific suit
                    } else {
                        // Substitute joker for missing cards in straights
                        $newRanks[] = $rank;
                        $newSuits[] = 'Wildcard';
                    }
    
                    $newHands[] = [
                        'ranks' => $newRanks,
                        'suits' => $newSuits
                    ];
                }
            }
    
            $hands = $newHands; // Update hands for the next iteration
        }
    
        return $hands;
    }

    // private static function checkHand(Collection $hand)
    // {
    //     $ranks = $hand['ranks']; // Directly access ranks
    //     $suits = $hand['suits']; // Directly access suits
    
    //     // Count occurrences of each rank and suit
    //     $rankCounts = array_count_values($ranks);
    //     arsort($rankCounts); // Sort by frequency of ranks (descending)
    //     $suitCounts = array_count_values($suits);
    
    //     // Check for Flush (5 cards of the same suit)
    //     $isFlush = max($suitCounts) >= 5;
    
    //     // Check for Straight (5 consecutive ranks)
    //     $isStraight = self::isStraight($ranks);
        
    //     // Check for Straight Flush (both Straight and Flush)
    //     if ($isFlush && $isStraight) {
    //         // Check if it's a Royal Flush (10, J, Q, K, A of the same suit)
    //         $royalRanks = [10, 11, 12, 13, 14]; // 10 to Ace
    //         if (empty(array_diff($royalRanks, $ranks))) {
    //             return self::POKER_HANDS['RoyalFlush'];
    //         }
    //         return self::POKER_HANDS['StraightFlush'];
    //     }
    
    //     // Check for Four of a Kind (4 cards of the same rank)
    //     if (max($rankCounts) === 4) {
    //         return self::POKER_HANDS['FourOfAKind'];
    //     }
    
    //     // Check for Full House (3 of a kind + a pair)
    //     if (max($rankCounts) === 3 && count($rankCounts) >= 2) {
    //         return self::POKER_HANDS['FullHouse'];
    //     }
    
    //     // Check for Flush (if not already detected in Straight Flush logic)
    //     if ($isFlush) {
    //         return self::POKER_HANDS['Flush'];
    //     }
    
    //     // Check for Straight (if not already detected in Straight Flush logic)
    //     if ($isStraight) {
    //         return self::POKER_HANDS['Straight'];
    //     }
    
    //     // Check for Three of a Kind (3 cards of the same rank)
    //     if (max($rankCounts) === 3) {
    //         return self::POKER_HANDS['ThreeOfAKind'];
    //     }
    
    //     // Check for Two Pair
    //     $pairs = array_filter($rankCounts, function ($count) {
    //         return $count === 2;
    //     });
    //     if (count($pairs) >= 2) {
    //         return self::POKER_HANDS['TwoPair'];
    //     }
    
    //     // Check for One Pair
    //     if (max($rankCounts) === 2) {
    //         return self::POKER_HANDS['OnePair'];
    //     }
    
    //     // High Card (default case)
    //     return self::POKER_HANDS['HighCard'];
    // }
    
    private static function checkHand(Collection $hand)
    {
        $ranks = $hand['ranks']; // Directly access ranks
        $suits = $hand['suits']; // Directly access suits
    
        // Count occurrences of each rank and suit
        $rankCounts = array_count_values($ranks);
        arsort($rankCounts); // Sort by frequency of ranks (descending)
        $suitCounts = array_count_values(array_filter($suits, fn($suit) => $suit !== 'Wildcard')); // Exclude wildcards
    
        // Count the number of wildcards in the hand
        $wildcardCount = count(array_filter($suits, fn($suit) => $suit === 'Wildcard'));
    
        // Check for Flush (5 cards of the same suit, considering wildcards)
        $isFlush = false;
        foreach ($suitCounts as $suit => $count) {
            if ($count + $wildcardCount >= 5) {
                $isFlush = true;
                break;
            }
        }
    
        // Check for Straight (5 consecutive ranks, considering wildcards)
        $isStraight = self::isStraightWithWildcards($ranks, $wildcardCount);
    
        // Check for Straight Flush (both Straight and Flush)
        if ($isFlush && $isStraight) {
            // Check if it's a Royal Flush (10, J, Q, K, A of the same suit)
            $royalRanks = [10, 11, 12, 13, 14]; // 10 to Ace
            if (empty(array_diff($royalRanks, $ranks))) {
                return self::POKER_HANDS['RoyalFlush'];
            }
            return self::POKER_HANDS['StraightFlush'];
        }
    
        // Check for Four of a Kind (4 cards of the same rank)
        if (max($rankCounts) + $wildcardCount >= 4) {
            return self::POKER_HANDS['FourOfAKind'];
        }
    
        // Check for Full House (3 of a kind + a pair)
        if (max($rankCounts) + $wildcardCount >= 3 && count($rankCounts) >= 2) {
            return self::POKER_HANDS['FullHouse'];
        }
    
        // Check for Flush (if not already detected in Straight Flush logic)
        if ($isFlush) {
            return self::POKER_HANDS['Flush'];
        }
    
        // Check for Straight (if not already detected in Straight Flush logic)
        if ($isStraight) {
            return self::POKER_HANDS['Straight'];
        }
    
        // Check for Three of a Kind (3 cards of the same rank)
        if (max($rankCounts) + $wildcardCount >= 3) {
            return self::POKER_HANDS['ThreeOfAKind'];
        }
    
        // Check for Two Pair
        $pairs = array_filter($rankCounts, function ($count) use ($wildcardCount) {
            return $count === 2 || ($count + $wildcardCount >= 2);
        });
        if (count($pairs) >= 2) {
            return self::POKER_HANDS['TwoPair'];
        }
    
        // Check for One Pair
        if (max($rankCounts) + $wildcardCount >= 2) {
            return self::POKER_HANDS['OnePair'];
        }
    
        // High Card (default case)
        return self::POKER_HANDS['HighCard'];
    }
    
    private static function isStraightWithWildcards(array $ranks, int $wildcardCount): bool
    {
        $uniqueRanks = array_unique($ranks);
        sort($uniqueRanks);
    
        $maxStreak = 0;
        $currentStreak = 1;
        $wildcardsUsed = 0;
    
        for ($i = 1; $i < count($uniqueRanks); $i++) {
            if ($uniqueRanks[$i] === $uniqueRanks[$i - 1] + 1) {
                $currentStreak++;
            } else if ($uniqueRanks[$i] > $uniqueRanks[$i - 1] + 1) {
                $gap = $uniqueRanks[$i] - $uniqueRanks[$i - 1] - 1;
                if ($wildcardsUsed + $gap <= $wildcardCount) {
                    $wildcardsUsed += $gap;
                    $currentStreak += $gap + 1;
                } else {
                    $maxStreak = max($maxStreak, $currentStreak);
                    $currentStreak = 1;
                    $wildcardsUsed = 0;
                }
            }
        }
    
        $maxStreak = max($maxStreak, $currentStreak);
        return $maxStreak + $wildcardCount >= 5;
    }



    /**
     * Check if ranks form a straight.
     *
     * @param array $ranks
     * @return bool
     */
    private static function isStraight(array $ranks)
    {
        $ranks = array_unique($ranks);
        sort($ranks);

        for ($i = 0; $i < count($ranks) - 4; $i++) {
            if ($ranks[$i + 4] - $ranks[$i] == 4) {
                return true;
            }
        }

        return false;
    }
}
