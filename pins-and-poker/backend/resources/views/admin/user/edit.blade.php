@extends('admin.layouts.app')

@section('content')
    <section class="section">
        <div class="section-body">
            <div class="row justify-content-center">
                <div class="col-12 col-md-8 col-lg-7">
                    <div class="card">
                        <form action="{{ route('admin.user.update') }}" id="mod-form" method="POST" novalidate="">
                            @csrf
                            @method('POST')
                            
                            <div class="card-header">
                                <h4>Add Moderator</h4>
                            </div>
                            <div class="card-body">
                                
                                @if (auth()->user()->user_type == '')
                                    <div class="form-group mb-3">
                                        <label>Password</label>
                                        <input type="password" class="form-control" name="password" placeholder="Password" required>
                                    </div>
                                    
                                    <div class="form-group mb-4">
                                        <label>Status</label>
                                        
                                        <select name="status" class="form-control selectric mb3" required>
                                            <option disabled>Select Status</option>
                                            <option class="text-dark" value="0" @if ($user->is_blocked === '0') selected @endif>Enable</option>
                                            <option class="text-dark" value="1" @if ($user->is_blocked === '1') selected @endif>Disable</option>
                                        </select>
                                    </div>
                                @else
                                    <div class="form-group mb-4">
                                        <label>Status</label>
                                        
                                        <select name="status" class="form-control selectric mb3" required>
                                            <option disabled>Select Status</option>
                                            <option class="text-dark" value="0" @if ($user->is_blocked === '0') selected @endif>Enable</option>
                                            <option class="text-dark" value="1" @if ($user->is_blocked === '1') selected @endif>Disable</option>
                                        </select>
                                    </div>
                                @endif
                                
                                
                            </div>
                            <div class="card-footer">
                                <button class="btn btn-primary w-100" id="add-mod">Update</button>
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
                startLoading('add-mod', 'Updating...');
                
                const form = document.getElementById('mod-form');

                $.ajax({
                    url: $(this).action(),
                    type: 'POST',
                    data: $(this).serialize(),
                    dataType: 'json',
                    success: function(response) {
                        if (response.success) {
                            form.reset(); // Reset Form
                            endLoading('add-mod', 'Updated');
                            notify('success', response.message);
                            window.location.href = "{{ route('admin.user.index') }}";
                        }
                    },
                    error: function(xhr, status, error) {
                        if (xhr.status === 422 || xhr.status === 404) {
                            stopLoading('add-mod', 'Update');
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