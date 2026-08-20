@extends('admin.layouts.app')

@section('content')
    <section class="section">
        <div class="section-body">
            <div class="row justify-content-center">
                <div class="col-12 col-md-12 col-lg-8">
                    <div class="card">
                        <div class="card-header pt-3">
                            <h4>Rule Details</h4>
                        </div>
                        <div class="px-5 pt-4 pb-5">
                            <div class="row ">
                                <div class="col-md-3 col-6 b-r">
                                    <strong class="text-dark">Rule Type</strong> <br>
                                    <p>{{ $rule->type }}</p>
                                </div>
                            </div>

                            <div class="section-title my-2">Description</div>
                            <p>{{ $rule->description }}</p>
                        </div>
                    </div>
                </div>
            </div>
    </section>
@endsection
