<?php

namespace App\Traits;

trait ApiResponseTrait
{
    /**
     * success response method with no data.
     * CODE 200
     * @return \Illuminate\Http\Response
     */
    public function successResponse($message = 'success', $code = 200)
    {
        return response()->json([
            'success' => true,
            'message' => $message,
        ], $code);
    }

    /**
     * success response method.
     * CODE 200
     * @return \Illuminate\Http\Response
     */
    public function successDataResponse($data = [], $message = 'success', $code = 200)
    {
        return response()->json([
            'success' => true,
            'message' => $message,
            'data'    => $data
        ], $code);
    }

    /**
     * error response method.
     * CODE 400
     * @return \Illuminate\Http\Response
     */
    public function errorResponse($message = 'error', $code = 400)
    {
        return response()->json([
            'success' => false,
            'message' => $message
        ], $code);
    }

    /**
     * data not found response method.
     * CODE 404
     * @return \Illuminate\Http\Response
     */
    protected function notFound($message = 'not found', $code = 404)
    {
        return response()->json([
            'success'   => false,
            'message'   => $message,
        ], $code);
    }
}