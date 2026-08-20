<?php

use Illuminate\Support\Str;
use Illuminate\Http\UploadedFile;
use Illuminate\Support\Facades\Log;

if (!function_exists('format_date')) {
    function format_date($date, $format = 'd-m-Y') {
        return \Carbon\Carbon::parse($date)->format($format);
    }
}

if (!function_exists('format_time')) {
    function format_time($time, $format = 'H:i:s') {
        return \Carbon\Carbon::parse($time)->format($format);
    }
}

if (!function_exists('formattedTime')) {
    function formattedTime($time) {
        $hour = substr($time, 0, 2);
        $minute = substr($time, 2, 2);
        $period = substr($time, 4, 2);

        return "{$hour}:{$minute} {$period}";
    }
}

if (!function_exists('timeIntoString')) {
    function timeIntoString($time) {
        $hour = substr($time, 0, 2);
        $minute = substr($time, 3, 2);
        $period = substr($time, 6, 2); // AM/PM

        return $hour . $minute . $period;
    }
}

if (!function_exists('generate_player_id')) {
    function generate_player_id($min, $max) {
        //
    }
}

if (!function_exists('text_limit')) {
    function text_limit($description, $limit = '150', $end = '....') {
        return \Illuminate\Support\Str::limit($description, $limit, $end);
    }
}

if (!function_exists('upload_image')) {
    function upload_image(UploadedFile $file, $directory = 'temp', $uniqid = null)
    {
        $allowedExtensions = ['jpeg', 'jpg', 'png'];

        // Validate File Extension
        if (!in_array($file->extension(), $allowedExtensions)) {
            Log::error("File Upload Error: Unsupported file format ({$file->extension()}).");
            return "Unsupported Format: Only jpg, jpeg, and png files are allowed.";
        }

        $uploadDirectory = 'uploads';
        $basePath = $uploadDirectory . '/' . trim($directory, '/');
        $defaultPath = $directory ? $basePath . '/default.png' : null;

        // Generate unique filename
        $filename = ($uniqid ? $uniqid . '_' : '') . Str::random(40) . '.' . $file->extension();

        try {
            $file->move(public_path($basePath), $filename);
            $path = $basePath . '/' . $filename;

            // Check if the file exists and if default path should be used
            if (!empty($directory) && $directory !== 'temp') {
                if (!file_exists(public_path($path))) {
                    return $defaultPath;
                }
            }
            return $path;
        } catch (\Exception $e) {
            Log::error('File Upload Error: ' . $e->getMessage());
            return "File Upload Error: Failed to upload the image.";
        }
    }
}