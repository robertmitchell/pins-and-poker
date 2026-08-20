@extends('admin.layouts.app')

@section('content')
    <section class="section">
        <div class="section-body">
            <div class="row justify-content-center">
                <div class="col-12 col-md-8 col-lg-7">
                    <div class="card">
                        <form action="{{ route('admin.user.store') }}" id="mod-form" method="POST" novalidate="">
                            @csrf
                            @method('POST')
                            
                            <div class="card-header">
                                <h4>Add Moderator</h4>
                            </div>
                            <div class="card-body">
                                <div class="form-group mb-3">
                                    <label>Email</label>
                                    <input type="email" class="form-control @error('email') is-invalid @enderror" name="email" placeholder="Email" value="{{ old('email') }}" required>
                                    @error('email') <div class="text-danger">{{ $message }}</div> @enderror
                                </div>

                                <div class="form-group mb-3">
                                    <label>Password</label>
                                    <input type="password" class="form-control @error('password') is-invalid @enderror" name="password" placeholder="Password" required>
                                    @error('password') <div class="text-danger">{{ $message }}</div> @enderror
                                </div>
                            </div>
                            <div class="card-footer">
                                <button class="btn btn-primary w-100" id="add-mod">Create</button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </section>
@endsection

{{--
@push('scripts')
    <script>
        $(document).on('ready', function () {

            hideInputErrors();

            $('#mod-form').on('submit', function (e) {
                
                e.preventDefault();
                hideInputErrors();
                $('input').css('border-color', '#e4e6fc');
                startLoading('add-mod', 'Creating...');
                
                const form = document.getElementById('mod-form');

                $.ajax({
                    url: $(this).action(),
                    type: 'POST',
                    data: $(this).serialize(),
                    dataType: 'json',
                    success: function(response) {
                        if (response.success) {
                            form.reset(); // Reset Form
                            endLoading('add-mod', 'Created');
                            notify('success', response.message);
                            window.location.href = "{{ route('admin.user.index') }}";
                        }
                    },
                    error: function(xhr, status, error) {
                        if (xhr.status === 422 || xhr.status === 404) {
                            stopLoading('add-mod', 'Create');
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
--}}