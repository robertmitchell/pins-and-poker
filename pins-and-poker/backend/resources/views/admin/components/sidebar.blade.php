<div class="main-sidebar sidebar-style-2">
    <aside id="sidebar-wrapper">
        <div class="sidebar-brand">
            <a href="{{ route('admin.dashboard') }}">
                <img alt="image" src="{{ asset('assets/admin/img/logo.png') }}" class="header-logo" />
                {{-- <span class="logo-name"></span> --}}
            </a>
        </div>
        <ul class="sidebar-menu">
            <li class="menu-header">Dashboard</li>
            <li class="dropdown {{ (request()->routeIs('admin.dashboard')) ? 'active' : '' }}">
                <a href="{{ route('admin.dashboard') }}" class="nav-link">
                    <i data-feather="monitor"></i><span>Dashboard</span>
                </a>
            </li>

            <li class="menu-header">Users</li>
            <li class="dropdown {{ 
                (request()->routeIs('admin.user.index')) ||
                (request()->routeIs('admin.user.create')) ||
                (request()->routeIs('admin.user.show')) ? 'active' : '' }}">

                <a href="javascript:void(0)" class="menu-toggle nav-link has-dropdown">
                    <i data-feather="users"></i><span>Users</span>
                </a>
                <ul class="dropdown-menu">
                    <li><a class="nav-link" href="{{ route('admin.user.index') }}">List Users</a></li>
                    <li><a class="nav-link" href="{{ route('admin.user.create') }}">Add Moderator</a></li>
                </ul>
            </li>

            <li class="menu-header">Game Play</li>
            <li class="dropdown {{
                (request()->routeIs('admin.rule.index')) || 
                (request()->routeIs('admin.rule.create')) ||
                (request()->routeIs('admin.rule.edit')) ? 'active' : '' }}">

                <a href="javascript:void(0)" class="menu-toggle nav-link has-dropdown">
                    <i data-feather="map-pin"></i><span>Rules</span>
                </a>
                <ul class="dropdown-menu">
                    <li><a class="nav-link" href="{{ route('admin.rule.index') }}">List Rules</a></li>
                    <li><a class="nav-link" href="{{ route('admin.rule.create') }}">Add Rule</a></li>
                </ul>
            </li>

            <li class="dropdown {{ 
                (request()->routeIs('admin.league.index')) ||
                (request()->routeIs('admin.league.edit')) ? 'active' : '' }}">

                <a href="{{ route('admin.league.index') }}" class="nav-link">
                    <i data-feather="users"></i><span>Leagues</span>
                </a>
            </li>

            <li class="dropdown {{ 
                (request()->routeIs('admin.game.index')) ||
                (request()->routeIs('admin.game.edit')) ? 'active' : '' }}">

                <a href="{{ route('admin.game.index') }}" class="nav-link">
                    <i data-feather="users"></i><span>Games</span>
                </a>
            </li>

            <li class="dropdown {{ (request()->routeIs('admin.game.winner.index')) ? 'active' : '' }}">

                <a href="{{ route('admin.game.winner.index') }}" class="nav-link">
                    <i data-feather="users"></i><span>Winners</span>
                </a>
            </li>
        </ul>
    </aside>
</div>