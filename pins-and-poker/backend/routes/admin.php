<?php

use Illuminate\Support\Facades\Route;

/*
|--------------------------------------------------------------------------
| Admin Routes
|--------------------------------------------------------------------------
|
| Here is where you can register admin routes for your application. These
| routes are loaded by the RouteServiceProvider and all of them will
| be assigned to the "web" middleware group. Make something great!
|
*/

// Auth::route();
Route::get('/', function(){ return redirect()->route('admin.loginForm'); });

Route::middleware('admin.guest')->group(function () {
    Route::namespace('Auth')->controller('LoginController')->group(function () {
        Route::get('login', 'showLoginForm')->name('loginForm');
        Route::post('login', 'login')->name('login');
    });

});


Route::middleware('admin.auth')->group(function () {
    Route::namespace('Auth')->controller('LoginController')->group(function () {
        Route::post('logout', 'logout')->name('logout');
    });

    Route::controller('DashboardController')->group(function () {
        Route::get('dashboard', 'dashboard')->name('dashboard');
    });

    Route::controller('ProfileController')->prefix('profile')->name('profile.')->group(function () {
        Route::get('/', 'show')->name('show');
        // Route::get('edit/{id}', 'edit')->name('edit');
        // Route::post('update', 'update')->name('update');
    });

    Route::controller('UserController')->name('user.')->group(function () {
        Route::get('users', 'index')->name('index');
        
        Route::prefix('user')->group(function () {
            Route::get('detail/{id}', 'show')->name('show');
            Route::get('create', 'create')->name('create');
            Route::post('store', 'store')->name('store');
            Route::get('edit/{id}', 'edit')->name('edit');
            Route::post('update', 'update')->name('update');
        });
    });

    Route::controller('RuleController')->name('rule.')->group(function () {
        Route::get('rules', 'index')->name('index');

        Route::prefix('rule')->group(function () {
            Route::get('create', 'create')->name('create');
            Route::post('store', 'store')->name('store');
            Route::get('edit/{id}', 'edit')->name('edit');
            Route::post('update', 'update')->name('update');
            Route::get('detail/{id}', 'show')->name('show');
            Route::delete('delete', 'destroy')->name('destroy');
        });
    });

    Route::controller('LeagueController')->name('league.')->group(function () {
        Route::get('leagues', 'index')->name('index');
    
        Route::prefix('league')->group(function () {
            Route::get('edit/{id}', 'edit')->name('edit');
            Route::post('update', 'update')->name('update');
            Route::delete('delete', 'destroy')->name('destroy');
        });
    });

    Route::controller('GameController')->name('game.')->group(function () {
        Route::get('games', 'index')->name('index');
    
        Route::prefix('game')->group(function () {
            Route::get('edit/{id}', 'edit')->name('edit');
            Route::post('update', 'update')->name('update');
            Route::delete('delete', 'destroy')->name('destroy');
            Route::get('winners', 'winner_index')->name('winner.index');
        });
    });
});
