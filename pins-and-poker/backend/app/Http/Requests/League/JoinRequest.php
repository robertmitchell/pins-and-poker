<?php

namespace App\Http\Requests\League;

use Illuminate\Foundation\Http\FormRequest;

class JoinRequest extends FormRequest
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
            'league_id' => 'required|numeric|digits_between:1,20|exists:leagues,id'
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
            'league_id.exists' => 'The league id does not exist in our records.'
        ];
    }

// 'player_id'     => 'required|numeric|digits_between:10,12'
// 'username'      => 'required|string|min:3|max:255'
// 'email'         => 'required|string|max:255|email|email:rfc,dns|unique:users,email|exists:users,email'
// 'password'      => 'required|string|min:5|max:30'
// 'image'         => 'required|image|mimes:jpeg,png,jpg|max:2048',
// 'auth_provider' => 'required|string|max:255|in:guest,google,apple'
// 'platform'     => 'required|string|max:255|in:android,ios'
// 'social_id'     => 'required|string|unique:users,social_id'
// 'social_token'  => 'required|string|unique:users,social_token'
// 'device_token'  => 'required|string|unique:users,device_token'
}
