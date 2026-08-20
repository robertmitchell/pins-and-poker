<?php

namespace App\Http\Controllers\Admin;

use App\Constants\{Status, RoleType, Rule as ConstantsRule};
use App\Http\Controllers\Controller;
use App\Models\{Rule};
use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class RuleController extends Controller
{
    public function index()
    {
        $rules = Rule::where('user_id', Status::IS_ADMIN)->where('created_by', RoleType::ADMIN)->get();

        $pageTitle = 'Rules List';
        return view('admin.rule.index', compact('rules', 'pageTitle'));
    }

    public function create()
    {
        $pageTitle = 'Create Rule';
        return view('admin.rule.create', compact('pageTitle'));
    }

    public function store(Request $request)
    {
        $this->validate($request, [
            'description' => 'required|string|max:500'
        ]);

        try {
            Rule::create([
                'user_id'     => Status::IS_ADMIN,
                'type'        => ConstantsRule::SPECIAL,
                'description' => $request->description,
                'created_by'  => RoleType::ADMIN
            ]);

            return $this->successResponse('Rule Created Succesfully.');

        } catch (\Exception $e) {
            return $this->errorResponse('Oops! Something went wrong.');
        }
    }

    public function show($id)
    {
        $rule = Rule::find($id);

        $pageTitle = 'Rule Detail';
        return view('admin.rule.show', compact('rule', 'pageTitle'));
    }

    public function edit($id)
    {
        $rule = Rule::whereId($id)->first();
        
        $pageTitle = 'Edit Rule';
        return view('admin.rule.edit', compact('rule', 'pageTitle'));
    }
    
    public function update(Request $request)
    {
        $this->validate($request, [
            'rule_id'     => 'required|exists:rules,id',
            'description' => 'required|string|max:500'
        ], [
            'rule_id.exists' => 'The rule id does not exists in our records.'
        ]);

        try {
            $rule = Rule::whereId($request->rule_id)->where('created_by', RoleType::ADMIN)->first();
            if (empty($rule)) return $this->errorResponse('Rule not found.');

            $rule->update(['description' => $request->description]);

            return $this->successResponse('Rule Updated Succesfully.');

        } catch (\Exception $e) {
            return $this->errorResponse('Oops! Something went wrong.');
        }
    }

    public function destroy(Request $request)
    {
        $this->validate($request, [
            'rule_id' => 'required|exists:rules,id'
        ], [
            'rule_id.exists' => 'The rule id does not exists in our records.'
        ]);

        // try {
        //     DB::beginTransaction();

        //     $rule = Rule::whereId($request->rule_id)->where('created_by', RoleType::ADMIN)->first();
        //     if (empty($rule)) return $this->errorResponse('Rule not found.');

        //     $rule->delete();

        //     DB::commit();
        //     return $this->successResponse('Rule deleted successfully.');
        // } catch (\Exception $e) {
        //     DB::rollBack();
        //     return $this->errorResponse('Oops! Something went wrong.');
        // }
    }
}
