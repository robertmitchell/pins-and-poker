<?php

use Illuminate\Support\Facades\Route;

/*
|--------------------------------------------------------------------------
| API Routes
|--------------------------------------------------------------------------
|
| Here is where you can register API routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| is assigned the "api" middleware group. Enjoy building your API!
|
*/

Route::prefix('v1')->group(function () {
    Route::get('check', function () { return 'ACCESS GRANTED'; }); // Testing API

    Route::controller('AuthController')->prefix('auth')->group(function () {
        Route::post('login', 'login')->name('login'); // Player Login

        Route::prefix('moderator')->group(function () {
            Route::post('login', 'moderatorLogin'); // Moderator Login
        });

        Route::middleware('auth:sanctum')->group(function () {
            Route::controller('ProfileController')->prefix('profile')->group(function () {
                Route::post('update', 'update');
            });

            // Route::post('connect', 'connectWithSocial');  // CURRENTLY NOT IN USE
            Route::post('logout', 'logout')->name('logout');
        });
    });

    Route::middleware('auth:sanctum')->group(function () {
        
        // ------------- FOR BOTH MODERATOR AND USER -------------
        Route::controller('ProfileController')->prefix('profile')->group(function () {
            Route::delete('delete-account', 'delete');
        });

        Route::controller('NotificationController')->group(function () {
            Route::get('notifications', 'get_notifications');
            Route::post('notification/create', 'create_notification');
            Route::post('notification/seen', 'markAsRead');
        });
        
        // LEAGUE ROUTES
        Route::prefix('league')->group(function () {
            Route::controller('ParticipantController')->prefix('participants')->group(function () {
                Route::get('/', 'get_league_participants');
            });
        });
        
        // GAME ROUTES
        Route::prefix('game')->group(function () {
            Route::controller('ParticipantController')->prefix('participants')->group(function () {
                Route::get('/', 'get_game_participants');
            });

            // GAME SCORE ROUTES
            Route::controller('ScoreController')->prefix('score')->group(function () {
                Route::get('/', 'get_game_scores');
                Route::post('update', 'update');
            });
        });

        // DISPUTE ROUTES
        Route::controller('DisputeController')->prefix('dispute')->group( function() {
            Route::get('/', 'getDisputes');
            Route::post('create', 'create');
            Route::post('upload-image', 'upload_image');
        });


        // USER ROUTES
        Route::namespace('User')->middleware('role:user')->prefix('user')->group(function () {
            // LEAGUE ROUTES
            Route::controller('LeagueController')->prefix('league')->group(function () {
                Route::get('/', 'user_leagues');
                Route::get('all', 'get_all_leagues');
                Route::post('join', 'join');
                Route::post('cancel', 'cancel');
            });
            
            // GAME ROUTES
            Route::controller('GameController')->prefix('game')->group(function () {
                Route::get('/', 'get_league_games');
                Route::post('join', 'join');
                Route::post('cancel', 'cancel');
            });

            // EXCHANGE CARD ROUTE
            Route::controller('CardController')->prefix('card')->group(function () {
                Route::post('exchange', 'exchange_card');
            });

            // SEARCH ROUTE
            Route::controller('SearchController')->prefix('search')->group(function () {
                Route::get('/', 'leagues_and_games');
            });
        });

        // MODERATOR ROUTES
        Route::namespace('Moderator')->middleware('role:moderator')->prefix('moderator')->group(function () {
            // LEAGUE ROUTES
            Route::controller('LeagueController')->group(function () {
                Route::get('rules', 'get_admin_rules');
            });

            Route::controller('LeagueController')->prefix('league')->group(function () {
                Route::get('/', 'get_leagues_data');
                Route::get('requests', 'get_leagues_requests');
                Route::post('manage-request', 'manage_requests');
                Route::post('manage-request-all', 'manage_all_requests');
                Route::post('create', 'create');
                Route::post('update', 'update');
                // Route::post('info', 'league_info');
            });
            
            // GAME ROUTES
            Route::controller('GameController')->prefix('game')->group(function () {
                Route::get('/', 'get_games_data');
                Route::get('requests', 'get_game_requests');
                Route::post('manage-request', 'manage_requests');
                Route::post('manage-request-all', 'manage_all_requests');
                Route::post('status', 'manage_game_status');
                Route::post('create', 'create');
                Route::post('update', 'update'); // Update game lane
                Route::post('assign-lane', 'update_assigned_lane'); // Update game lane
            });

            // REMOVE PARTICIPANTS
            Route::controller('ParticipantController')->prefix('participant')->group(function () {
                Route::post('remove', 'remove_participants');
            });

            // DISPUTE ROUTE
            Route::controller('DisputeController')->prefix('dispute')->group( function() {
                Route::post('status', 'changeStatus');
            });
        });
    });
});
