<?php

namespace App\Http\Requests\Game;

use Illuminate\Foundation\Http\FormRequest;

class CancelRequest extends FormRequest
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
            'league_id' => 'required|numeric|digits_between:1,20|exists:leagues,id',
            'game_id'   => 'required|numeric|digits_between:1,20|exists:games,id',
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
            'league_id.exists' => 'The league id does not exist in our records.',
            'game_id.exists'   => 'The game id does not exist in our records.'
        ];
    }
}
