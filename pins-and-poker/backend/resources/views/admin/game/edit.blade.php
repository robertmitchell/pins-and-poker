@extends('admin.layouts.app')

@section('content')
    <section class="section">
        <div class="section-body">
            <div class="row justify-content-center">
                <div class="col-12 col-md-8 col-lg-7">
                    <div class="card">
                        <form id="game-form" method="POST" novalidate="">
                            <div class="card-header">
                                <h4>Edit Game</h4>
                            </div>
                            <div class="card-body">
                                <input type="hidden" name="game_id" value="{{ $game->id }}">

                                <div class="form-group mb-3">
                                    <label>Name</label>
                                    <input type="text" class="form-control" name="name" placeholder="Name" value="{{ $game->name }}" required autofocus>
                                </div>

                                <div class="form-group mb-3">
                                    <label>Lane</label>
                                    <input type="text" class="form-control numeric" name="lane" placeholder="Lane" 
                                        value="{{ $game->lane }}" required>
                                </div>

                                <div class="form-group mb-3">
                                    <label>Start Time</label>
                                    <input type="text" class="form-control" name="start_time" placeholder="Start Time" 
                                        value="{{ formattedTime($game->start_time) }}" required>
                                </div>

                                <div class="form-group mb-4">
                                    <label>Status</label>
                                    
                                    <select name="status" class="form-control selectric mb3" required>
                                        <option disabled>Select Status</option>
                                        <option class="text-dark" value="pending" @if ($game->status === 'pending') selected @endif>Pending</option>
                                        <option class="text-dark" value="started" @if ($game->status === 'started') selected @endif>Started</option>
                                        <option class="text-dark" value="ended" @if ($game->status === 'ended') selected @endif>Ended</option>
                                    </select>
                                </div>
                            </div>
                            <div class="card-footer">
                                <button class="btn btn-primary w-100" id="update-game">Update</button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </section>
@endsection

@push('scripts')
    <script>
        $(document).ready(function() {

            hideInputErrors();

            $('#game-form').on('submit', function (e) {
                e.preventDefault();
                hideInputErrors();
                $('input').css('border-color', '#e4e6fc');
                startLoading('update-game', 'Updating...');
                
                const form = document.getElementById('game-form');
                
                $.ajax({
                    url: "{{ route('admin.game.update') }}",
                    type: 'POST',
                    data: $(this).serialize(),
                    dataType: 'json',
                    success: function(response) {
                        if (response.success) {
                            form.reset(); // Reset Form
                            endLoading('update-game', 'Updated');
                            notify('success', response.message);
                            window.location.href = "{{ route('admin.game.index') }}";
                        }
                    },
                    error: function(xhr, status, error) {
                        if (xhr.status === 422 || xhr.status === 404) {
                            stopLoading('update-game', 'Update');
                            const errors = xhr.responseJSON.errors;
                            if (Object.keys(errors).length > 0) {
                                $.each(errors, function (key, value) {
                                    $("[name='" + key + "']")
                                        .after(`<div class="invalid-feedback d-block">${value}</div>`)
                                        .css('border-color', '#dc3545');
                                });
                            } else {
                                notify('error', 'Oops! Something went wrong.');
                            }
                        }
                    }
                });
            });
        });
    </script>
@endpush