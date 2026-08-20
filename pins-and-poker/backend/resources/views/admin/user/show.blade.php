@extends('admin.layouts.app')

@section('content')
    <section class="section">
        <div class="section-body">
            <div class="row justify-content-center">
                <div class="col-6 col-md-12 col-lg-5">
                    <div class="card author-box" style="height: auto;">
                        <div class="card-header">
                            <h4>Personal Details</h4>
                        </div>
                        <div class="card-body">
                            <div class="author-box-center pb-4">
                                <img alt="image" src="{{ asset($user->avatar_image ?? 'uploads/images/user/default.png') }}"
                                    class="rounded-circle" style="width: 150px; height: 150px; object-fit: cover;">
                                <div class="clearfix"></div>
                                <div class="author-box-name pt-3">
                                    <a href="javascript:void()">{{ $user->username }}</a>
                                </div>
                            </div>
                            <div class="card-body">
                                <p class="clearfix">
                                    <span class="float-left">Player ID</span>
                                    <span class="float-right text-muted">{{ $user->player_id }}</span>
                                </p>
                                <div class="py-2">
                                    <p class="clearfix">
                                        <span class="float-left">Email</span>
                                        <span class="float-right text-muted">{{ $user->email }}</span>
                                    </p>
                                </div>
                                @if ($user->user_type == \App\Constants\RoleType::MODERATOR)
                                    <p class="clearfix">
                                        <span class="float-left">Phone</span>
                                        <span class="float-right text-muted">{{ $user->phone }}</span>
                                    </p>
                                @endif
                                <p class="clearfix">
                                    <span class="float-left">Profile Type</span>
                                    <span class="float-right text-muted">{{ ucfirst($user->user_type) }}</span>
                                </p>
                                <p class="clearfix">
                                    <span class="float-left">Platform</span>
                                    <span class="float-right text-muted">{{ ucfirst($user->platform) }}</span>
                                </p>
                                <p class="clearfix">
                                    <span class="float-left">Social Connection</span>
                                    <span class="float-right text-muted">
                                        {{ ucfirst($user->auth_provider) }}
                                    </span>
                                </p>
                                <p class="clearfix">
                                    <span class="float-left">Account Status</span>
                                    <span class="float-right text-muted text-capitalize">
                                        @if ($user->is_blocked == '0')
                                            <span class="badge badge-success badge-shadow text-capitalize">Enable</span>
                                        @else
                                            <span class="badge badge-danger badge-shadow text-capitalize">Disable</span>
                                        @endif
                                    </span>
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
    </section>
@endsection
