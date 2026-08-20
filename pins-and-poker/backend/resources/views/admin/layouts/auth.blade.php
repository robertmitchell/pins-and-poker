<!DOCTYPE html>
<html lang="en">

<head>
    <!-- META Tags-->
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no,  shrink-to-fit=no">

  <!-- CSRF Token -->
  <meta name="csrf-token" content="{{ csrf_token() }}">
  <meta http-equiv="X-UA-Compatible" content="IE=edge">
  <meta name="robots" content="noindex" />

  <title>{{ !empty($pageTitle) ? "{$pageTitle} – " : '' }}{{ config('app.name', 'APP_NAME') }}</title>
  
  <!-- General CSS Files -->
  <link rel="stylesheet" href="{{ asset('assets/admin/css/app.min.css') }}">
  <link rel="stylesheet" href="{{ asset('assets/admin/bundles/bootstrap-social/bootstrap-social.css') }}">
  <!-- Template CSS -->
  <link rel="stylesheet" href="{{ asset('assets/admin/css/style.css') }}">
  <link rel="stylesheet" href="{{ asset('assets/admin/css/components.css') }}">
  <!-- Custom style CSS -->
  <link rel="stylesheet" href="{{ asset('assets/admin/css/custom.css') }}">
  @stack('stylesheets')

  <link rel='shortcut icon' type='image/x-icon' href="{{ asset('assets/admin/img/logo.png') }}"/>
</head>

<body>
    <!-- Loader -->
    <div class="loader"></div>

    <div id="app">
        <!-- Authentication Page -->
        @yield('content')
    </div>

    <!-- General JS Scripts -->
    <script src="{{ asset('assets/admin/js/app.min.js') }}"></script>
    <script src="{{ asset('assets/admin/js/scripts.js') }}"></script>
    <script src="{{ asset('assets/admin/js/custom.js') }}"></script>

    @include('admin.components.notify')

    <script type="text/javascript">
        $.ajaxSetup({
            headers: {
                'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
            }
        });

        // Enable Button
        function startLoading(id, value) {
            $(`#${id}`).prop('disabled', true).text(value).css('opacity', '0.5');
        }

        // Disable Button
        function stopLoading(id, value) {
            $(`#${id}`).prop('disabled', false).text(value).css('opacity', '1');
        }

        function endLoading(id, value) {
            $(`#${id}`).text(value).css('opacity', '0.5');
        }

        // Hide Error Messages
        function hideInputErrors() { $('.invalid-feedback').remove(); }
    </script>
    @stack('scripts')
</body>

</html>