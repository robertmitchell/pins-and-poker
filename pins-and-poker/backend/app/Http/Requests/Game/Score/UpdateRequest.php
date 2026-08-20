<?php

namespace App\Http\Requests\Game\Score;

use Illuminate\Foundation\Http\FormRequest;

class UpdateRequest extends FormRequest
{
    /**
     * Determine if the user is authorized to make this request.
     *
     * @return bool
     */
    public function authorize()
    {
        return true;
    }

    /**
     * Get the validation rules that apply to the request.
     *
     * @return array
     */
    public function rules()
    {
        return [
            'player_id' => 'required|numeric|digits_between:10,12|exists:users,player_id',
            'league_id' => 'required|numeric|digits_between:1,20|exists:leagues,id',
            'game_id'   => 'required|numeric|digits_between:1,20|exists:games,id',
            'rolls'     => 'required',
            'rolls.*'   => 'required|numeric|digits_between:1,20'
        ];
    }

    /**
     * Get the custom error messages for the validator.
     *
     * @return array
     */
    public function messages()
    {
        return [
            'player_id.exists' => 'The player id does not exist in our records.',
            'league_id.exists' => 'The league id does not exist in our records.',
            'game_id.exists'   => 'The game id does not exist in our records.'
        ];
    }
}
