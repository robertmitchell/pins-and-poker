<?php

namespace App\Http\Controllers\Admin;

use App\Constants\RoleType;
use App\Http\Controllers\Controller;
use App\Models\User;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Hash;
use Illuminate\Support\Facades\Validator;

class UserController extends Controller
{
    public function index()
    {
        $users = User::whereNotNull('username')->orderBy('created_at', 'DESC')->get();

        $pageTitle = 'User List';
        return view('admin.user.index', compact('users', 'pageTitle'));
    }

    public function create()
    {
        $pageTitle = 'Create Moderator';
        return view('admin.user.create', compact('pageTitle'));
    }

    public function store(Request $request)
    {
        $validator = Validator::make($request->all(), [
            'password' => 'required|string|max:255',
            'email'    => 'required|string|email:rfc,dns|unique:users,email',
        ]);
        
        if ($validator->fails()) {
            return back()->withErrors($validator)->withInput();
        }

        $player_id = rand(20000000000, 99999999999);

        try {
            DB::beginTransaction();
            
            User::create([
                'player_id' => $player_id,
                'email' => $request->email,
                'password' => Hash::make($request->password),
                'user_type' => RoleType::MODERATOR
            ]);

            DB::commit();
            $notify[] = ['success', 'Moderator Created Successfully.'];
            return redirect()->route('admin.user.index')->withNotify($notify);

        } catch (\Exception $e) {
            DB::rollBack();
            $notify[] = ['error', 'Oops! Something went wrong.'];
            return back()->withNotify($notify);
        }
    }
    
    public function edit($id)
    {
        $user = User::whereId($id)->first();
        
        $pageTitle = 'Edit Moderator';
        return view('admin.user.edit', compact('user', 'pageTitle'));
    }
    
    public function update(Request $request)
    {
        //
    }

    public function show($id)
    {
        $user = User::find($id);

        $pageTitle = 'User Detail';
        return view('admin.user.show', compact('user', 'pageTitle'));
    }
}
