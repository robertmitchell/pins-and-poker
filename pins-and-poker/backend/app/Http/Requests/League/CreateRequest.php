<?php

namespace App\Http\Requests\League;

use Illuminate\Foundation\Http\FormRequest;

class CreateRequest extends FormRequest
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
            'name'            => 'required|string|min:3|max:255',
            'prize_pool'      => 'required|numeric|digits_between:1,20',
            'image'           => 'nullable|image|mimes:jpeg,png,jpg|max:2048',
            'start_time'      => 'required|string|min:4|max:255',
            // 'start_time'      => 'required|regex:/^(0[0-9]|1[0-2]):[0-5][0-9] (AM|PM)$/|max:255',
            'general_rules'   => 'required|string',
            'special_rules'   => 'required',
            'special_rules.*' => 'required|integer'
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
            // 'start_time.regex' => 'Please enter a valid time format (e.g., 09:30 AM or 12:45 PM).'
        ];
    }
}
