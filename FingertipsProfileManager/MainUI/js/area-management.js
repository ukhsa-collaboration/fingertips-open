
function documentReady() {

    initTableSorter();

    $('#Confirm').live('click', function () {
        loading();

        var originalAreaCode = $('#originalAreaCode').val();
        var newAreaCode = $('#AreaDetails\\.AreaCode').val();
        if (originalAreaCode != newAreaCode) {
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

        if (areaTypeId != -1 && searchText.length > 0) {
            loading();
            $('form#searchAreas').submit();
        };
    });

    $('.area-detail-link').live('click', function () {
        loading();
        var areaCode = $(this).text(),
            searchAreaTypeId = $('#areaTypeId').val(),
            searchText = $('#searchText').val();

        $.ajax({
            cache: false,
            type: 'Get',
            url: '/ShowAreaDetails',
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
            }
        });
    });
}

$(document).ready(documentReady);
