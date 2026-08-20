@extends('admin.layouts.app')

@section('content')
    <section class="section">
        <div class="section-body">
            <div class="row justify-content-center">
                <div class="col-12 col-md-8 col-lg-7">
                    <div class="card">
                        <form id="rule-form" method="POST" novalidate="">
                            <div class="card-header">
                                <h4>Add Special Rule</h4>
                            </div>
                            <div class="card-body">
                                <input type="hidden" name="rule_id" value="{{ $rule->id }}">
                                <div class="form-group mb-3">
                                    <label>Description</label>
                                    <textarea class="form-control" id="description" name="description" placeholder="Description" rows="2" required>{{ $rule->description }}</textarea>
                                </div>
                            </div>
                            <div class="card-footer">
                                <button class="btn btn-primary w-100" id="update-rule">Update</button>
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

            $('#rule-form').on('submit', function (e) {
                e.preventDefault();
                hideInputErrors();
                $('#description').css('border-color', '#e4e6fc');
                startLoading('update-rule', 'Updating...');
                
                const form = document.getElementById('rule-form');
                
                $.ajax({
                    url: "{{ route('admin.rule.update') }}",
                    type: 'POST',
                    data: $(this).serialize(),
                    dataType: 'json',
                    success: function(response) {
                        if (response.success) {
                            form.reset(); // Reset Form
                            endLoading('update-rule', 'Updated');
                            notify('success', response.message);
                            window.location.href = "{{ route('admin.rule.index') }}";
                        }
                    },
                    error: function(xhr, status, error) {
                        if (xhr.status === 422 || xhr.status === 404) {
                            stopLoading('update-rule', 'Update');
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