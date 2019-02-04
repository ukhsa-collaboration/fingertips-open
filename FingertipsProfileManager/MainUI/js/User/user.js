
function documentReady() {
    initTableSorter();

    $('.select-all-check-box').live('click', function () {
        $('.indicator-check-box').attr('checked', $(this).is(':checked'));
    });

    toggleInactiveUsers();

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
                url: '/user/user-audit',
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

function toggleInactiveUsers() {
    var inactiveUsersShown = false;

    var $button = $('#toggle-current-users');
    var $inactiveUsers = $('.not-current');

    $('#toggle-current-users').click(function () {

        if (inactiveUsersShown) {
            $button.val('Show inactive users');
            $inactiveUsers.hide();
        } else {
            $button.val('Hide inactive users');
            $inactiveUsers.show();
        }

        inactiveUsersShown = !inactiveUsersShown;
    });
}

$(document).ready(documentReady);
