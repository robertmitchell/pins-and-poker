@extends('admin.layouts.app')

@section('content')
    <section class="section">
        <div class="section-body">
            <div class="row justify-content-center">
                <div class="col-12 col-md-8 col-lg-7">
                    <div class="card">
                        <form id="league-form" method="POST" novalidate="">
                            <div class="card-header">
                                <h4>Edit League</h4>
                            </div>
                            <div class="card-body">
                                <input type="hidden" name="league_id" value="{{ $league->id }}">

                                <div class="form-group mb-3">
                                    <label>Name</label>
                                    <input type="text" class="form-control" name="name" placeholder="Name" value="{{ $league->name }}" required autofocus>
                                </div>

                                <div class="form-group mb-3">
                                    <label>Prize Pool</label>
                                    <input type="text" class="form-control numeric" name="prize_pool" placeholder="Prize Pool" 
                                        value="{{ $league->prize_pool }}" required>
                                </div>

                                <div class="form-group mb-3">
                                    <label>Start Time</label>
                                    <input type="text" class="form-control" name="start_time" placeholder="Start Time" 
                                        value="{{ formattedTime($league->start_time) }}" required>
                                </div>

                                <div class="form-group pb-1 mb-0"><label>Image</label></div>
                                <div class="custom-file">
                                    <input type="file" name="image" class="custom-file-input mb-1" accept="image/jpeg, image/png, image/jpg" 
                                        max-size="5120" id="customFile">
                                    <label class="custom-file-label" for="customFile">Choose file</label>
                                </div>
                                <div class="form-group mb-0">
                                    <img class="pt-4 w-25" id="show-image" src="{{ asset($league->image) }}" alt="">
                                </div>
                            </div>
                            <div class="card-footer">
                                <button class="btn btn-primary w-100" id="update-league">Update</button>
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

            $('#customFile').on('change', function() {
                var fileName = $(this).val().split('\\').pop();
                $('.custom-file-label').text(fileName);
            });

            $('#customFile').on('change', function() {
                var fileInput = this;
                var file = fileInput.files[0];

                if (file) {
                    var reader = new FileReader();
                    reader.onload = function(e) {
                        $('#show-image').attr('src', e.target.result);
                    };
                    reader.readAsDataURL(file);
                }
            });

            $('#league-form').on('submit', function (e) {
                e.preventDefault();
                hideInputErrors();
                $('input').css('border-color', '#e4e6fc');
                startLoading('update-league', 'Updating...');
                
                const form = document.getElementById('league-form');
                const formData = new FormData(form);

                $.ajax({
                    url: "{{ route('admin.league.update') }}",
                    type: 'POST',
                    data: formData,
                    processData: false,
                    contentType: false,
                    dataType: 'json',
                    success: function(response) {
                        if (response.success) {
                            form.reset(); // Reset Form
                            endLoading('update-league', 'Updated');
                            notify('success', response.message);
                            window.location.href = "{{ route('admin.league.index') }}";
                        }
                    },
                    error: function(xhr, status, error) {
                        if (xhr.status === 422 || xhr.status === 404) {
                            stopLoading('update-league', 'Update');
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