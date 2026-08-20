<?php

namespace App\Helpers;

use App\Constants\RoleType;

class FileHelper {

    public static $userType = [RoleType::ADMIN => RoleType::ADMIN, RoleType::MODERATOR => RoleType::MODERATOR, RoleType::PLAYER => RoleType::PLAYER];

    public static function handleImageUpload($image, $user_type = 'user', $key = '', $directory = '')
    {
        if (empty($image) && !$image->isValid()) {
            return null;
        }
   
        $type = self::$userType[$user_type] ?? 'temp';
        $basePath = "images/{$type}" . (!empty($directory) && $type !== 'temp' ? "/{$directory}/" : '/');

        return upload_image($image, $basePath, $key);
    }

    public static function getDefaultImage($user_type = 'user', $directory = '')
    {
        $type = self::$userType[$user_type] ?? 'temp';
        $path = "uploads/images/{$type}" . (!empty($directory) && $type !== 'temp' ? "/{$directory}" : '');

        return "{$path}/default.png";
    }

    public static function removeOldImage($removeImage = '')
    {
        $filePath = public_path($removeImage);
        
        if (file_exists($filePath) ) {
            return unlink($filePath);
        }

        return false;
    }
}