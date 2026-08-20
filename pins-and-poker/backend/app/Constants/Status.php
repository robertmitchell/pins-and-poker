<?php

namespace App\Constants;

class Status {
    // ADMIN ID
    const IS_ADMIN = '1';

    // SOCIAL STATUS
    const NOT_SOCIAL = '0';
    const SOCIAL = '1';
    
    // DELETION STATUS
    const NOT_DELETED = '0';
    const DELETED = '1';

    // BLOCKED STATUS
    const BLOCKED = '1';
    const NOT_BLOCKED = '0';

    // LEAGUE REQUEST STATUS
    const PENDING = 'pending';
    const ACCEPTED = 'accepted';

    // GAME STATUS
    const PENDING_GAME = 'pending';
    const GAME_STARTED = 'started';
    const GAME_ENDED = 'ended';

    // DISPUTE STATUS
    const PENDING_DISPUTE = 'pending';
    const RESOLVED = 'resolved';

    // EXCHANGE STATUS
    const IS_EXCHANGE = '1';
    const NOT_EXCHANGE = '0';

    // WINNER
    const GAME_WIN = '1';
    const GAME_LOSE = '0';
}