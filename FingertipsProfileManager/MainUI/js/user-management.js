
function documentReady() {
    initTableSorter();

    $('.select-all-check-box').live('click', function () {
        $('.indicator-check-box').attr('checked', $(this).is(':checked'));
    });

    $('#audit_history').click(function () {
        var selectedUsers = [] ;

        $('.indicator-check-box:checked').each(function () {
            selectedUsers.push($(this).val());
        });

        if (selectedUsers.length) {
            loading();

            $.ajax({
                cache: false,
                type: 'Get',
                url: '/GetUserAudit',
                data: { jdata: selectedUsers },
                traditional: true,
                dataType: 'html',
                success: function (data) {
                    var h = data;
                    lightbox.show(h, 20, 300, 700);
                    $('#deleteIndicators').show();
                    loadingFinished();
                },
                error: function (xhr, error) {
                    loadingFinished();
                }
            });
        }
    });
}

$(document).ready(documentReady);
