@extends('admin.layouts.app')

@section('content')
    <section class="section">
        <div class="row ">
        <div class="col-xl-3 col-lg-6 col-md-6 col-sm-6 col-xs-12">
            <div class="card">
            <div class="card-statistic-4">
                <div class="align-items-center justify-content-between">
                <div class="row ">
                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 pr-0 pt-3">
                    <div class="card-content">
                        <h5 class="font-18">Users</h5>
                        <h2 class="mb-3 font-18">{{ $users->count() }}</h2>
                    </div>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 pl-0">
                    <div class="banner-img">
                        <img src="{{ asset('assets/admin/img/banner/1.png') }}" alt="">
                    </div>
                    </div>
                </div>
                </div>
            </div>
            </div>
        </div>
        <div class="col-xl-3 col-lg-6 col-md-6 col-sm-6 col-xs-12">
            <div class="card">
            <div class="card-statistic-4">
                <div class="align-items-center justify-content-between">
                <div class="row ">
                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 pr-0 pt-3">
                    <div class="card-content">
                        <h5 class="font-18">Leagues</h5>
                        <h2 class="mb-3 font-18">{{ $leagueCount }}</h2>
                    </div>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 pl-0">
                    <div class="banner-img">
                        <img src="{{ asset('assets/admin/img/banner/2.png') }}" alt="">
                    </div>
                    </div>
                </div>
                </div>
            </div>
            </div>
        </div>
        <div class="col-xl-3 col-lg-6 col-md-6 col-sm-6 col-xs-12">
            <div class="card">
            <div class="card-statistic-4">
                <div class="align-items-center justify-content-between">
                <div class="row ">
                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 pr-0 pt-3">
                    <div class="card-content">
                        <h5 class="font-18">Rules</h5>
                        <h2 class="mb-3 font-18">{{ $rulesCount }}</h2>
                    </div>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 pl-0">
                    <div class="banner-img">
                        <img src="{{ asset('assets/admin/img/banner/3.png') }}" alt="">
                    </div>
                    </div>
                </div>
                </div>
            </div>
            </div>
        </div>
        <div class="col-xl-3 col-lg-6 col-md-6 col-sm-6 col-xs-12">
            <div class="card">
            <div class="card-statistic-4">
                <div class="align-items-center justify-content-between">
                <div class="row ">
                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 pr-0 pt-3">
                    <div class="card-content">
                        <h5 class="font-18">Games</h5>
                        <h2 class="mb-3 font-18">{{ $gameCount }}</h2>
                    </div>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6 pl-0">
                    <div class="banner-img">
                        <img src="{{ asset('assets/admin/img/banner/4.png') }}" alt="">
                    </div>
                    </div>
                </div>
                </div>
            </div>
            </div>
        </div>
        </div>
        <div class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-header">
                        <h4>Users Listing</h4>
                    </div>

                    <div class="card-body">
                        <div class="table-responsive">
                            <table class="table table-striped" id="table-2">
                                <thead>
                                    <tr>
                                        <th>S No.</th>
                                        <th>Name</th>
                                        <th>Email Address</th>
                                        <th>Status</th>
                                        <th>Created At</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    @forelse ($users as $user)
                                        <tr>
                                            <td>{{ $loop->iteration }}</td>
                                            <td>{{ $user->username }}</td>
                                            <td>{{ $user->email }}</td>
                                            <td>
                                                @if ($user->is_blocked == '0')
                                                    <div class="badge badge-success badge-shadow text-capitalize">Enable</div>
                                                @else
                                                    <div class="badge badge-danger badge-shadow text-capitalize">Disabled</div>
                                                @endif
                                            </td>
                                            <td>{{ format_date($user->created_at) }}</td>
                                        </tr>
                                    @empty
                                        <tr>
                                            <td colspan="5">
                                                <h4 class="text-center">No Record Found.</h4>
                                            </td>
                                        </tr>
                                    @endforelse
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
@endsection
