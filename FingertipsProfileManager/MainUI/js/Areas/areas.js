
function documentReady() {

    initTableSorter();

    $('#Confirm').live('click', function () {
        loading();

        var originalAreaCode = $('#InitialAreaCode').val();
        var newAreaCode = $('#AreaDetails_AreaCode').val();

        if (originalAreaCode !== newAreaCode) {
            $('#dialog-confirm').dialog({
                resizable: false,
                height: 300,
                width: 400,
                modal: true,
                buttons: {
                    'Continue': function () {
                        $(this).dialog('close');
                        $('form#EditArea').submit();
                    },
                    Cancel: function () {
                        $(this).dialog('close');
                        loadingFinished();
                    }
                }
            });
        } else {
            $('form#EditArea').submit();
        }
    });

    $('.mandatory').live('keyup', function () {

        var allMandatoryCompleted = true;
        $('.mandatory').each(function () {
            if ($(this).val() == '') {
                $(this).addClass('input-validation-error');
                allMandatoryCompleted = false;
            } else {
                $(this).removeClass('input-validation-error');
            }
        });

        var confirm = $('#Confirm');
        if (!allMandatoryCompleted) {
            confirm.hide();
        } else {
            confirm.show();
        }
    });

    $('#goSearch').live('click', function () {
        var areaTypeId = $('#areaTypeId').val();
        var searchText = $('#searchText').val();

        if (areaTypeId !== -1 && searchText.length > 0) {
            loading();
            $('form#searchAreas').submit();
        };
    });

    $('.area-detail-link').on('click', function () {
        loading();
        var areaCode = $(this).text(),
            searchAreaTypeId = $('#areaTypeId').val(),
            searchText = $('#searchText').val();

        // Turn off the click event to avoid multiple requests
        $('.area-detail-link').off('click');

        $.ajax({
            cache: false,
            type: 'Get',
            url: '/areas/area-details',
            data: {
                areaCode: areaCode,
                areaTypeId: searchAreaTypeId,
                searchText: searchText
            },
            traditional: true,
            dataType: 'html',
            success: function (data) {
                lightbox.show(data, 20, 300, 700);
                loadingFinished();
            },
            error: function (xhr, error) {
                loadingFinished();

                // Failure. Turn on the click event, so the click event can be triggered again
                $('.area-detail-link').on('click');
            }
        });
    });
}

$(document).ready(documentReady);
