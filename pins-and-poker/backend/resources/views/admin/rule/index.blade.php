@extends('admin.layouts.app')

@section('content')
    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-header">
                    <h4>All Rules List</h4>
                </div>
                <div class="card-body">
                    <div class="table-responsive">
                        <table class="table table-striped" id="table-2">
                            <thead>
                                <tr>
                                    <th>S No.</th>
                                    <th>Type</th>
                                    <th>Description</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                @forelse ($rules as $rule)
                                    <tr>
                                        <td>{{ $loop->iteration }}</td>
                                        <td>{{ $rule->type }}</td>
                                        <td>{{ text_limit($rule->description, 60) }}</td>
                                        <td>
                                            <a href="{{ route('admin.rule.edit', ['id' => $rule->id]) }}" class="btn mb-3 btn-success mx-1">
                                                <i class="fas fa-edit"></i> Edit
                                            </a>

                                            <a href="{{ route('admin.rule.show', ['id' => $rule->id]) }}" class="btn btn-primary mb-3 mx-1">
                                                <i class="fas fa-eye"></i> Show
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
                                <!--@if($rule->type !== \App\Constants\Rule::GENERAL)-->
                                <!--    <button data-id="{{ $rule->id }}" class="btn mb-3 delete-rule btn-danger">-->
                                <!--        <i class="fas fa-trash-alt"></i> Delete-->
                                <!--    </button>-->
                                <!--@endif-->
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
        // $(document).on('click', '.delete-rule', function(e) {
        //     var id = $(this).data('id');

        //     $.ajax({
        //         url: "{{ route('admin.rule.destroy') }}",
        //         type: 'DELETE',
        //         data: { rule_id: id },
        //         success: function(response) {
        //             if (response.success) {
        //                 location.reload();
        //                 notify('success', response.message);
        //             }
        //         },
        //         error: function(xhr, status, error) {
        //             if (xhr.status === 422 || xhr.status === 404) {
        //                 notify('error', 'Oops! Something went wrong.');
        //             }
        //         }
        //     });
        // });
    </script>
@endpush