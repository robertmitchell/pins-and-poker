@extends('admin.layouts.app')

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
                                    <th>Winner ID</th>
                                    <th>Winner Name</th>
                                    <th>Game Name</th>
                                    <th>League Name</th>
                                    <th>Total Score</th>
                                    <th>Poker Hand</th>
                                </tr>
                            </thead>
                            <tbody>
                                @forelse ($scores as $score)
                                    <tr>
                                        <td>{{ $loop->iteration }}</td>
                                        <td>{{ $score->user->player_id }}</td>
                                        <td>{{ $score->user->username }}</td>
                                        <td>{{ $score->game->name }}</td>
                                        <td>{{ $score->game->league->name }}</td>
                                        <td>{{ $score->total_score }}</td>
                                        <td>{{ $score->poker_hands }}</td>
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
