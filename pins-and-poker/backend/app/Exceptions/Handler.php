<?php

namespace App\Exceptions;

use Illuminate\Database\QueryException;
use Illuminate\Foundation\Exceptions\Handler as ExceptionHandler;
use Illuminate\Auth\AuthenticationException;
use Illuminate\Database\Eloquent\ModelNotFoundException;
use Illuminate\Validation\ValidationException;
use Symfony\Component\HttpKernel\Exception\MethodNotAllowedHttpException;
use Symfony\Component\HttpKernel\Exception\NotFoundHttpException;
use Throwable;

class Handler extends ExceptionHandler
{
    /**
     * A list of the exception types that are not reported.
     *
     * @var array<int, class-string<Throwable>>
     */
    protected $dontReport = [
        //
    ];

    /**
     * A list of the inputs that are never flashed for validation exceptions.
     *
     * @var array<int, string>
     */
    protected $dontFlash = [
        'current_password',
        'password',
        'password_confirmation',
    ];

    /**
     * Register the exception handling callbacks for the application.
     *
     * @return void
     */
    public function register()
    {
        $this->reportable(function (Throwable $e) {
            //
        });
    }

    public function render($request, Throwable $exception)
    {
        if ($request->is("api/*")) {
            if ($exception instanceof ModelNotFoundException) {
                return $this->modelNotFoundException($exception->getMessage());

            } else if ($exception instanceof ValidationException) {
                return $this->validationException($exception->validator->errors()->first());

            } else if ($exception instanceof MethodNotAllowedHttpException) {
                return $this->methodNotAllowedHttpException();

            } else if ($exception instanceof NotFoundHttpException) {
                return $this->notFoundHttpException();

            } else if ($exception instanceof AuthenticationException) {
                return $this->authenticationException($exception->getMessage());
                
            } else if ($exception instanceof QueryException) {
                return $this->queryException($exception->getMessage());

            } else {
                return response()->json([
                    'success' => false,
                    'message' => env('APP_DEBUG') 
                                 ? $exception->getMessage() . ' on line no ' . $exception->getLine() 
                                 : "Something went wrong",
                ], 500);
            }
        }

        return parent::render($request, $exception);
    }

    public function modelNotFoundException($message = 'model not found')
    {
        return response()->json([
            'success' => false,
            'message' => $message,
        ], 404);
    }

    public function validationException($message = 'Validation Error')
    {
        return response()->json([
            'success' => false,
            'message' => $message,
        ], 422);
    }

    public function methodNotAllowedHttpException()
    {
        return response()->json([
            'success' => false,
            'message' => 'Method Not Allowed: The specified HTTP method is not supported by this resource',
        ], 405);
    }

    public function notFoundHttpException()
    {
        return response()->json([
            'success' => false,
            'message' => 'The requested URL could not be found on this server.',
        ], 404);
    }

    public function authenticationException($message = 'Unauthorized')
    {
        return response()->json([
            'success' => false,
            'message' => $message,
        ], 401);
    }

    public function queryException($message = 'Database Error')
    {
        return response()->json([
            'success' => false,
            'message' => "Database Error: {$message}",
        ], 500);
    }

    // InvalidArgumentException
}
