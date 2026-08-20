@extends('admin.layouts.app')

@push('stylesheet')
    <style>
        .profile-image { width: 40px; height: 40px; background-repeat: no-repeat; background-position: center; background-size: cover; border-radius: 100%; }
    </style>
@endpush

@section('content')
    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-header">
                    <h4>All Users List</h4>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <table class="table table-striped" id="table-2">
                            <thead>
                                <tr>
                                    <th>S No.</th>
                                    <th>User Profile</th>
                                    <th>Player ID</th>
                                    <th>Username</th>
                                    <th>Email Address</th>
                                    <th>Profile Type</th>
                                    <th>Platform</th>
                                    <th>Status</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                @forelse ($users as $user)
                                    <tr>
                                        <td>{{ $loop->iteration }}</td>
                                        <td class="d-flex justify-content-center">
                                            <div style="background-image: url('{{ asset($user->avatar_image ?? 'uploads/images/user/default.png') }}');" 
                                                class="profile-image"></div>
                                        </td>
                                        <td>{{ $user->player_id }}</td>
                                        <td>{{ $user->username }}</td>
                                        <td>{{ $user->email }}</td>
                                        <td>{{ $user->user_type }}</td>
                                        <td>{{ $user->platform }}</td>
                                        <td>
                                            @if ($user->is_blocked == '0')
                                                <div class="badge badge-success badge-shadow text-capitalize">Enable</div>
                                            @else
                                                <div class="badge badge-danger badge-shadow text-capitalize">Disable</div>
                                            @endif
                                        </td>
                                        <td>
                                            <a href="{{ route('admin.user.edit', ['id' => $user->id]) }}" class="btn btn-success">
                                                <i class="fas fa-edit"></i> Edit
                                            </a>
                                            
                                            <a href="{{ route('admin.user.show', ['id' => $user->id]) }}" class="btn btn-primary">
                                                <i class="fa fa-info-circle"></i> Detail
                                            </a>
                                        </td>
                                    </tr>
                                @empty
                                    <tr>
                                        <td class="text-center" colspan="7">
                                            <h4>No Record Found.</h4>
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
@endsection
