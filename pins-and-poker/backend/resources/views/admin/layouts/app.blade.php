<!DOCTYPE html>
<html lang="{{ str_replace('_', '-', app()->getLocale()) }}">

<head>
    <!-- META Tags-->
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no,  shrink-to-fit=no">

    <!-- CSRF Token -->
    <meta name="csrf-token" content="{{ csrf_token() }}">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="robots" content="noindex" />

    <title>{{ !empty($pageTitle) ? "{$pageTitle} – " : '' }}{{ config('app.name', 'APP_NAME') }}</title>

    <!-- Stylesheets CSS -->
    @include('admin.components.css')
    @stack('stylesheet')

    <!-- Icon image -->
    <link rel="shortcut icon" type='image/x-icon' href="{{ asset('assets/admin/img/logo.png') }}" />

</head>

<body>
    <!-- Loader -->
    <div class="loader"></div>
    <div id="app">
        <div class="main-wrapper main-wrapper-1">
            <div class="navbar-bg"></div>

            <!-- Header -->
            @include('admin.components.header')

            <!-- Left Sidebar Navigation -->
            @include('admin.components.sidebar')

            <!-- Main Content -->
            <div class="main-content">

                <!-- Page Dynamic Content -->
                @yield('content')
            </div>

            <!-- Footer -->
            @include('admin.components.footer')

            <!-- Scripts -->
            @include('admin.components.script')
            @include('admin.components.notify')
            @stack('scripts')
        </div>
    </div>
</body>

</html>