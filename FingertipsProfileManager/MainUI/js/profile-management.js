
function popupwindow(url, title, w, h) {
    return window.open(url, title, 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
}

function documentReady() {
    $('.sortHeader').click(function () {
        loading();
    });

    $('.sortDesc').click(function () {
        loading();
    });

    $('.sortAsc').click(function () {
        loading();
    });

    $('.show-spinner').click(function () {
        loading();
    });

    $('.select-all-check-box').click(function () {
        $('.selectable-check-box').attr('checked', $(this).is(':checked'));
    });
    
    $("form").submit(function (e) {
        if ($(this).valid()) {

            // Get all selected area-types for PDF
            var selectedAreaTypes = [];
            $('#profile-area-types input:checked').each(function () {
                selectedAreaTypes.push({
                    Id: $(this).attr('id').replace('chk', ''),
                    Value: $(this).attr('value')
                });
            });
            $('#SelectedPdfAreaTypes').val(JSON.stringify(selectedAreaTypes));

            // Get all selected profile users
            var selectedUsers = [];
            $('.selectable-check-box:checked').each(function () {
                selectedUsers.push({
                    Id: $(this).attr('id').replace('chk', ''),
                    Value: $(this).attr('value')
                });
            });
            $('#ProfileUsers').val(JSON.stringify(selectedUsers));
        }
    });

}

$(document).ready(documentReady);
