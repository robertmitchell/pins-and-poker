<footer class="main-footer">
    <div class="footer-left">
        <a href="{{ route('admin.dashboard') }}">
            {{ 'Developed By ' . config('app.name', 'APP_NAME') }}
        </a>
    </div>
    <div class="footer-right">
        {{ __('Copyright © ') }} <span class="year"></span>
        <a href="{{ route('admin.dashboard') }}" class="web-name">{{ config('app.name', 'APP_NAME') }}.</a>
        {{ __(' All Rights Reserved') }}
    </div>
</footer>
