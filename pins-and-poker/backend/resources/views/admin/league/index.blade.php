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
                                    <th>Image</th>
                                    <th>Name</th>
                                    <th>Prize Pool</th>
                                    <th>Participants</th>
                                    <th>Start Time</th>
                                    <th>Created By</th>
                                    <th>Created At</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                @forelse ($leagues as $league)
                                    <tr>
                                        <td>{{ $loop->iteration }}</td>
                                        <td class="d-flex justify-content-center">
                                            <div style="background-image: url('{{ asset($league->image) }}');" class="league-image"></div>
                                        </td>
                                        <td>{{ $league->name }}</td>
                                        <td>{{ $league->prize_pool }}</td>
                                        <td>{{ $league->participants }}</td>
                                        <td>{{ formattedTime($league->start_time) }}</td>
                                        <td>{{ $league->user->username }}</td>
                                        <td>{{ format_date($league->created_at) }}</td>
                                        <td>
                                            <a href="{{ route('admin.league.edit', ['id' => $league->id]) }}" class="btn mb-3 btn-success mx-1">
                                                <i class="fas fa-edit"></i> Edit
                                            </a>
                                            
                                            <button data-id="{{ $league->id }}" class="btn mb-3 delete-league btn-danger">
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
        $(document).on('click', '.delete-league', function(e) {
            var id = $(this).data('id');

            $.ajax({
                url: "{{ route('admin.league.destroy') }}",
                type: 'DELETE',
                data: { league_id: id },
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