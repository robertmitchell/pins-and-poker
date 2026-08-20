<link rel="stylesheet" href="{{ asset('assets/admin/bundles/sweetalert2/css/sweetalert2.min.css') }}">
<script src="{{ asset('assets/admin/bundles/sweetalert2/js/sweetalert2.min.js') }}"></script>

<script type="text/javascript">
    const Toast = Swal.mixin({
        toast: true,
        width: "400px",
        position: "top-end",
        showConfirmButton: false,
        timer: 5000,
        timerProgressBar: true,
        didOpen: (toast) => {
        toast.onmouseenter = Swal.stopTimer;
        toast.onmouseleave = Swal.resumeTimer;
        }
    });
</script>

@if(session()->has('notify'))
    @foreach(session('notify') as $msg)
        <script>
            "use strict";
            const icon = @json($msg[0]);
            const title = @json(__($msg[1]));
            
            Toast.fire({ icon: icon, title: title});
        </script>
    @endforeach
@endif

@if (isset($errors) && $errors->any())
    @php
        $collection = collect($errors->all());
        $errors = $collection->unique();
    @endphp

    <script>
        "use strict";
            @foreach ($errors as $error)
                Toast.fire({
                    icon: 'error',
                    title: @json(__($error))
                });
            @endforeach
    </script>

@endif

<script>
    "use strict";
    function notify(status, message) {
        if (typeof message == 'string') {
            Toast.fire({
                icon: status,
                title: message
            });
        } else {
            $.each(message, function(i, val) {
                Toast.fire({
                    icon: status,
                    title: val
                });
            });
        }
    }
</script>