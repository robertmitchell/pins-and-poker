<?php

namespace App\Http\Controllers\Admin;

use App\Constants\RoleType;
use App\Http\Controllers\Controller;
use App\Models\{Game, League, Rule, User};
use Illuminate\Http\Request;

class DashboardController extends Controller
{
    public function dashboard()
    {
        $users = User::orderBy('created_at', 'DESC')->get();
        $leagueCount = League::count();
        $rulesCount = Rule::where('created_by', RoleType::ADMIN)->count();
        $gameCount = Game::count();
        
        $pageTitle = 'Dashboard';
        return view('admin.dashboard', compact('users', 'rulesCount', 'gameCount', 'leagueCount', 'pageTitle'));
    }
}
