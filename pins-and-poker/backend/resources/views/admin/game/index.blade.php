@extends('admin.layouts.app')

@push('stylesheet')
    <style>
        .league-image { width: 40px; height: 40px; background-repeat: no-repeat; background-position: center; background-size: cover; border-radius: 100%; }
    </style>
@endpush

@section('content')
    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-header">
                    <h4>All League List</h4>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <table class="table table-striped" id="table-2">
                            <thead>
                                <tr>
                                    <th>S No.</th>
                                    <th>Game Name</th>
                                    <th>League Name</th>
                                    <th>Lane</th>
                                    <th>Participants</th>
                                    <th>Start Time</th>
                                    <th>Status</th>
                                    <th>Created By</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                @forelse ($games as $game)
                                    <tr>
                                        <td>{{ $loop->iteration }}</td>
                                        <td>{{ $game->name }}</td>
                                        <td>{{ $game->league->name }}</td>
                                        <td>{{ $game->lane }}</td>
                                        <td>{{ $game->participants }}</td>
                                        <td>{{ formattedTime($game->start_time) }}</td>
                                        <td>
                                            @if ($game->status == 'started')
                                                <div class="badge badge-success badge-shadow text-capitalize">{{ $game->status }}</div>
                                            @elseif ($game->status == 'ended')
                                                <div class="badge badge-danger badge-shadow text-capitalize">{{ $game->status }}</div>
                                            @else
                                                <div class="badge badge-dark badge-shadow text-capitalize">{{ $game->status }}</div>
                                            @endif
                                        </td>
                                        <td>{{ $game->user->username }}</td>
                                        <td>
                                            <a href="{{ route('admin.game.edit', ['id' => $game->id]) }}" class="btn mb-3 btn-success mx-1">
                                                <i class="fas fa-edit"></i> Edit
                                            </a>
                                            
                                            <button data-id="{{ $game->id }}" class="btn mb-3 delete-game btn-danger">
                                                <i class="fas fa-trash-alt"></i> Delete
                                            </button>
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

@push('scripts')
    <script>
        // DELETE REQUEST
        $(document).on('click', '.delete-game', function(e) {
            var id = $(this).data('id');

            $.ajax({
                url: "{{ route('admin.game.destroy') }}",
                type: 'DELETE',
                data: { game_id: id },
                success: function(response) {
                    if (response.success) {
                        location.reload();
                        notify('success', response.message);
                    }
                },
                error: function(xhr, status, error) {
                    if (xhr.status === 422 || xhr.status === 404) {
                        notify('error', 'Oops! Something went wrong.');
                    }
                }
            });
        });
    </script>
@endpush