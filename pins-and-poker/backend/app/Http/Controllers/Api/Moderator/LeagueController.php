<?php

namespace App\Http\Controllers\Api\Moderator;

use App\Constants\{FileInfo, Rule as RuleConst, Status};
use App\Helpers\FileHelper;
use App\Http\Controllers\Controller as Controller;
use App\Http\Requests\League\{CreateRequest, UpdateRequest};
use App\Http\Resources\RuleResource;
use App\Models\{League, LeagueRequest, LeagueRule, Rule, User, Game, GameRequest};
use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class LeagueController extends Controller
{
    public function create(CreateRequest $request)
    {
        $authUser = auth()->user();
        $currentTime = \Carbon\Carbon::now();

        // HANDLE IMAGE UPLOAD
        $file = $request->file('image');
        $path = $request->hasFile('image')
                ? FileHelper::handleImageUpload($file, $authUser->user_type, 'league', 'leagues')
                : FileHelper::getDefaultImage($authUser->user_type, 'leagues');

        try {
            DB::beginTransaction();

            // Validating rulesIDs in Array format
            $ruleIDs = json_decode($request->special_rules, true);
            if (!is_array($ruleIDs)) {
                // Check if the decoded data is an array
                return $this->errorResponse('Invalid special rules format. Please provide an array.', 422);
            } else if (empty(($ruleIDs))) {
                // Check if the array is empty
                return $this->errorResponse('You must select at least one special rule.', 422);
            }

            // Validate that each rule ID is an integer and collect invalid IDs
            foreach ($ruleIDs as $ruleID) {
                if (!is_int($ruleID)) {
                    return $this->errorResponse("The rule ID {$ruleID} must be an integer.", 422);
                }
                $rules = Rule::whereId($ruleID)->exists();
                if (empty($rules)) {
                    return $this->errorResponse("The rule ID {$ruleID} does not exists in our records.", 404);
                }
            }

            $league = League::create([
                'user_id'    => $authUser->id,
                'name'       => $request->name,
                'prize_pool' => $request->prize_pool,
                'image'      => $path,
                'start_time' => $request->start_time
            ]);

            // Assigning Special Rules To League
            $created = $this->set_special_league_rules($ruleIDs, $league->id, $currentTime);
            if (empty($created)) {
                return $this->errorResponse('An error occured while processing your request.', 500);
            }

            $this->set_general_league_rules($authUser->id, $league->id, $request->general_rules, $currentTime);

            $data = ['id'  => $league->id];
            DB::commit();
            return $this->successDataResponse($data, 'The league has been successfully created.');
        } catch (\Exception $e) {
            DB::rollBack();
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }

    public final function update(UpdateRequest $request)
    {
        $authUser = auth()->user();
        $currentTime = \Carbon\Carbon::now();

        try {
            DB::beginTransaction();

            // Validating rulesIDs in Array format
            $ruleIDs = json_decode($request->special_rules, true);
            if (!is_array($ruleIDs)) {
                // Check if the decoded data is an array
                return $this->errorResponse('Invalid special rules format. Please provide an array.', 400);
            } else if (empty(($ruleIDs))) {
                // Check if the array is empty
                return $this->errorResponse('The provided array is empty. Please provide valid data.', 400);
            }

            // Validate that each rule ID is an integer and collect invalid IDs
            foreach ($ruleIDs as $ruleID) {
                if (!is_int($ruleID)) {
                    return $this->errorResponse("The rule ID {$ruleID} must be an integer.", 400);
                }
                $rules = Rule::whereId($ruleID)->exists();
                if (empty($rules)) {
                    return $this->errorResponse("The rule ID {$ruleID} does not exists in our records.", 400);
                }
            }

            $league = League::whereId($request->league_id)->where('user_id', $authUser->id)->first();
            if (empty($league)) {
                return $this->errorResponse('League not found.', 404);
            }

            // HANDLE IMAGE UPLOAD
            $file = $request->file('image');
            $uploadImage = !empty($file) ? FileHelper::handleImageUpload($file, $authUser->user_type, 'league', 'leagues') : null;
            $path = !empty($uploadImage) ? $uploadImage : null;
            // DEFAULT IMAGE
            $defaultImage = FileInfo::LEAGUE_DEFAULT_IMAGE;

            $oldImage = $league->image;
            $league->update([
                'name'       => $request->name,
                'prize_pool' => $request->prize_pool,
                'image'      => $path ?? $oldImage,
                'start_time' => $request->start_time,
                'updated_at' => $currentTime
            ]); 

            // DELETE OLD LEAGUE IMAGE
            if (!empty($path) && $oldImage !== $defaultImage) {
                FileHelper::removeOldImage($oldImage);
            }

            // Updating Special Rules
            $updated = $this->update_special_league_rules($ruleIDs, $league->id, $currentTime);
            if (empty($updated)) {
                return $this->errorResponse('An error occured while processing your request.', 500);
            }

            // Updating General Rules
            $result = $this->update_general_league_rules($authUser->id, $league->id, $request->general_rules, $currentTime);
            
            $data = ["id" => $league->id];

            DB::commit();
            return $this->successDataResponse($data, 'The league has been successfully updated.');
        } catch (\Exception $e) {
            DB::rollBack();
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }

    private function set_special_league_rules(array $ruleIDs, int $leagueId, $time)
    {
        $leagueRules = [];
        foreach ($ruleIDs as $ruleId) {
            $specialRule = Rule::where('id', $ruleId)->where('type', RuleConst::SPECIAL)->first();
            if (empty($specialRule)) {
                return $this->errorResponse("The special rules ID {$ruleId} in the array does not exist in our records.", 404);
            }

            if ($specialRule->type !== RuleConst::SPECIAL) {
                return $this->errorResponse("The special rules ID {$ruleId} in the array is not a recognized special rule.", 400);
            }

            $leagueRules[] = [
                'league_id'  => $leagueId,
                'rule_id'    => $specialRule->id,
                'created_at' => $time,
                'updated_at' => $time
            ];
        }

        $created = DB::table('league_rules')->insert($leagueRules);
        return $created;
    }

    private function update_special_league_rules($ruleIDs, $leagueId, $time)
    {
        // First, delete existing special rules for the league
        $results = DB::table('league_rules')->where('league_id', $leagueId)->delete();

        return $this->set_special_league_rules($ruleIDs, $leagueId, $time);
    }

    private function set_general_league_rules($user_id, $league_id, $rules, $time)
    {
        // Creating General Rules
        // if (condition) { // Editable and Uneditable
            $generalRule = Rule::create([
                'user_id'     => $user_id,
                'type'        => 'general',
                'description' => $rules,
                'updated_at'  => $time
            ]);
        // }

        // Assigning General Rule To League
        LeagueRule::create([
            'league_id'  => $league_id,
            'rule_id'    => $generalRule->id,
            'updated_at' => $time
        ]);
    }

    private function update_general_league_rules($user_id, $league_id, $rules, $time)
    {
        $leagueRule = LeagueRule::with('rules')->where('league_id', $league_id)->first();
        if (empty($leagueRule)) { return $this->errorResponse('League not found.', 404); }

        $generalRule = $leagueRule->rules->where(['user_id'=> $user_id, 'type' => 'general'])->first();
        if (empty($generalRule)) { return $this->errorResponse('Rule not found.', 404); }
        // Updating General Rule
        $generalRule->update(['description' => $rules, 'updated_at' => $time]);
        // Assigning General Rule To League
        LeagueRule::create(['league_id' => $league_id, 'rule_id' => $generalRule->id, 'updated_at' => $time]);
    }

    public final function get_leagues_data()
    {
        $authUser = auth()->user();

        try {
            $leagues = League::where('user_id', $authUser->id)
            ->get()
            ->map(function ($league) {
                $league_info = $league->league_requests->where('status', 'pending')->count();
                
                $game_info = $league->games->sum(function ($game) {
                    return $game->game_requests->where('status', 'pending')->count();
                });
                
                return [
                    'id'           => $league->id,
                    'player_id'    => $league->user->player_id,
                    'name'         => $league->name,
                    'image'        => $league->image,
                    'participants' => $league->participants,
                    'prize_pool'   => $league->prize_pool,
                    'start_time'   => $league->start_time,
                    'league_info'  => $league_info + $game_info,
                    'created_at'   => format_date($league->created_at),
                    'rules'        => RuleResource::collection($league->rules)
                ];
            })->toArray();

            if (empty($leagues)) {
                return $this->errorResponse('Leagues Not Found.', 404);
            }
    
            return $this->successDataResponse($leagues, 'Your league have been successfully fetched.');
        } catch (\Exception $e) {
            // return $e->getMessage();
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }

    public final function get_leagues_requests(Request $request)
    {
        $authUser = auth()->user();

        $this->validate($request, [
            'league_id' => 'required|string|numeric|digits_between:1,20|exists:leagues,id'
        ], [
            'league_id.exists' => 'The league id does not exist in our records.'
        ]);

        try {
            $requests = LeagueRequest::where([
                ['league_id', $request->league_id], ['status', Status::PENDING]
            ])
            ->get()
            ->map(function ($request) {
                return [
                    'status' => $request->status,
                    'created_at' => format_date($request->created_at),
                    'user'  => [
                        'player_id' => $request->user->player_id,
                        'username'  => $request->user->username,
                        'image'     => $request->user->avatar_image,
                    ]
                ];
            })->toArray();

            if (empty($requests)) {
                return $this->errorResponse('League Requests Not Found.', 404);
            }

            return $this->successDataResponse($requests, 'Your league requests have been successfully fetched.');
        } catch (\Exception $e) {
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }

    public final function manage_requests(Request $request)
    {
        $this->validate($request, [
            'league_id' => 'required|numeric|digits_between:1,20|exists:leagues,id',
            'player_id' => 'required|numeric|digits_between:10,12|exists:users,player_id',
            'status'    => 'required|in:accepted,declined'
        ], [
            'league_id.exists' => 'The league id does not exist in our records.',
            'player_id.exists' => 'The player id does not exist in our records.'
        ]);

        $authUser = auth()->user();

        try {
            DB::beginTransaction();
            // Fetch the league with its requests to join by user
            $league = League::whereId($request->league_id)->with('league_requests.user')->first();
            if (empty($league)) {
                return $this->errorResponse('League not found', 404);
            }

            if ($league->user_id !== $authUser->id) {
                return $this->errorResponse('You are not authorized to manage this league.', 403);
            }

            $user = User::where('player_id', $request->player_id)->first();
            $leagueRequest = $league->league_requests()->where('user_id', $user->id)->first();
            if (empty($leagueRequest)) {
                return $this->errorResponse('League request not found', 404);
            }

            if ($leagueRequest->status == Status::ACCEPTED) {
                return $this->errorResponse('The league request has already been accepted.', 409);
            }

            if ($request->status === Status::ACCEPTED) {
                $leagueRequest->update(['status' => Status::ACCEPTED]);
                $league->increment('participants');
                $message = "You've accepted the league request.";
            } else {
                $leagueRequest->delete();
                $message = "You've rejected the league request.";
            }
            
            DB::commit();
            return $this->successResponse($message);
        } catch (\Exception $e) {
            DB::rollBack();
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }

    public final function get_admin_rules()
    {
        $is_admin = '1';
        try {
            $rules = Rule::where('user_id', $is_admin)->get();

            return $this->successDataResponse(RuleResource::collection($rules), 'Rules Fetched Successfully.');

        } catch (\Exception $e) {
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }
    
    // public function league_info(Request $request)
    // {
    //     $this->validate($request, [
    //         'league_id' => 'required|numeric|digits_between:1,20|exists:leagues,id'
    //     ]);
        
    //     $authUser = auth()->user();
    //     $game_data = [];
    //     $game_count = 0;
    //     $league_count = LeagueRequest::where('league_id', $request->league_id)->where('status','pending')->count();
    //     $games = Game::where('league_id',$request->league_id)->get();
    //     if(count($games) > 0){
    //         foreach($games as $item)
    //         {
    //             $count = GameRequest::where('game_id',$item->id)->where('status','pending')->count();
    //             $game_count += $count;
    //         }
    //     }
    //     $total_count = $league_count + $game_count;
    //     $data = [
    //         'total_count'  => $total_count
    //     ];
        
    //     return $this->successDataResponse($data,'counts successfully fetched');
    // }
    
    public function manage_all_requests(Request $request)
    {
        $this->validate($request, [
            'league_id' => 'required|numeric|digits_between:1,20|exists:leagues,id',
            'status'    => 'required|in:accepted,rejected'
        ], [
            'league_id.exists' => 'The league id does not exist in our records.'
        ]);
        
        $authUser = auth()->user();
        
        try {
            DB::beginTransaction();
            
            $league = League::where('user_id', $authUser->id)
                        ->whereId($request->league_id)
                        ->first();
            
            if (empty($league)) return $this->errorResponse('Leagues Not Found.', 404);
            
            $leagueRequests = $league->league_requests()->where('status', Status::PENDING)->get();
             if ($leagueRequests->isEmpty()) {
                return $this->errorResponse('No pending league requests found.', 404);
            }
            
            if ($request->status == Status::ACCEPTED) {
                foreach ($leagueRequests as $league_req) {
                    $league->increment('participants');
                    $league_req->update(["status" => Status::ACCEPTED]);
                }
                $message = "You've accepted all league requests.";
                
            } else if ($request->status == "rejected") {
                foreach ($leagueRequests as $league_req) {
                    $league_req->delete();
                }
                $message = "You've rejected all league requests";
            }

            DB::commit();
            return $this->successResponse($message);
        } catch (\Exception $e) {
            DB::rollBack();
            return $this->errorResponse('Oops! Something went wrong. Please try again later.', 500);
        }
    }
}