<!-- General JS Scripts -->
<script src="{{ asset('assets/admin/js/app.min.js') }}"></script>
<!-- JS Libraies -->
<script src="{{ asset('assets/admin/bundles/apexcharts/apexcharts.min.js') }}"></script>
<script src="{{ asset('assets/admin/bundles/izitoast/js/iziToast.min.js') }}"></script>
<!-- Page Specific JS File -->
<script src="{{ asset('assets/admin/js/page/index.js') }}"></script>
<!-- Template JS File -->
<script src="{{ asset('assets/admin/js/scripts.js') }}"></script>
<!-- Custom JS File -->
<script src="{{ asset('assets/admin/js/custom.js') }}"></script>
<!-- Page Specific JS File -->
<script src="{{ asset('assets/admin/bundles/datatables/datatables.min.js') }}"></script>
<script src="{{ asset('assets/admin/bundles/datatables/DataTables-1.10.16/js/dataTables.bootstrap4.min.js') }}"></script>
<script src="{{ asset('assets/admin/bundles/jquery-ui/jquery-ui.min.js') }}"></script>
<script src="{{ asset('assets/admin/js/page/datatables.js') }}"></script>

<script type="text/javascript">
    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });

    // Only Numeric Digits Allowed
    $(".numeric").on('keypress', function(event) {
        var keyCode = event.which || event.keyCode;
        return (keyCode >= 48 && keyCode <= 57) ? true : (event.preventDefault(), false);
    });

    // Disable pasting except numeric digits
    $(".numeric").on('paste', function (e) {
        e.preventDefault();
        var pastedData = (e.originalEvent || e).clipboardData.getData('text');
        var numericData = pastedData.replace(/\D/g, '');
        document.execCommand('insertText', false, numericData);
    });

    // Footer Year
    let yearElement = $('.year');
    yearElement.text(new Date().getFullYear());

    // Enable Button
    function startLoading(id, value) {
        $(`#${id}`).prop('disabled', true).text(value).css('opacity', '0.5');
    }

    // Disable Button
    function stopLoading(id, value) {
        $(`#${id}`).prop('disabled', false).text(value).css('opacity', '1');
    }

    // End Button
    function endLoading(id, value) {
        $(`#${id}`).text(value).css('opacity', '0.5');
    }

    // Hide Error Messages
    function hideInputErrors() { $('.invalid-feedback').remove(); }
</script>
