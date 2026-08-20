<?php

namespace App\Http\Requests\Dispute;

use Illuminate\Foundation\Http\FormRequest;

class CreateDisputeRequest extends FormRequest
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
            'moderator_id'          => 'required|numeric|digits_between:1,20|exists:users,player_id',
            'game_id'               => 'required|numeric|digits_between:1,20|exists:games,id',
            'disputer_id'           => 'required|numeric|digits_between:1,20|exists:users,player_id',
            'disputed_against_id'   => 'required|numeric|digits_between:1,20|exists:users,player_id',
            'cell_index'            => 'required|string|min:1|max:255',
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
            'moderator_id.exists'           => 'The moderator id does not exist in our records.',
            'disputer_id.exists'            => 'The disputer id does not exist in our records.',
            'disputed_against_id.exists'    => 'The disputer against id does not exist in our records.'
        ];
    }
}
