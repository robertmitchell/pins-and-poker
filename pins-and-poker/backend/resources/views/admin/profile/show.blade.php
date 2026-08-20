@extends('admin.layouts.app')

@section('content')
    <section class="section">
        <div class="section-body">
            <div class="row justify-content-center">
                <div class="col-6 col-md-12 col-lg-5">
                    <div class="card author-box" style="height: auto;">
                        <div class="card-header">
                            <h4>Personal Details</h4>
                        </div>
                        <div class="card-body">
                            <div class="author-box-center pb-4">
                                <img alt="image" src="{{ asset('uploads/images/user/default.png') }}"
                                    class="rounded-circle" style="width: 150px; height: 150px; object-fit: cover;">
                                <div class="clearfix"></div>
                                <div class="author-box-name pt-3">
                                    <a href="javascript:void()">{{ $admin->name }}</a>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="py-2">
                                    <p class="clearfix">
                                        <span class="float-left">Email</span>
                                        <span class="float-right text-muted">{{ $admin->email }}</span>
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
    </section>
@endsection
