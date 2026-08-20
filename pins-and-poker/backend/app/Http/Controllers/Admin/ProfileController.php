<?php

namespace App\Http\Controllers\Admin;

use App\Http\Controllers\Controller;
use App\Models\Admin;
use Illuminate\Support\Facades\Auth;

class ProfileController extends Controller
{
    public function show()
    {
        $authAdmin = Auth::guard('admin')->user();
        
        $admin = Admin::find($authAdmin->id);
        if (empty($admin)) return $this->errorResponse('Admin not found.');

        $pageTitle = 'Profile';
        return view('admin.profile.show', compact('admin', 'pageTitle'));
    }
}
