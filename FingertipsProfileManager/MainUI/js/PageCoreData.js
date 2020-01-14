
function viewIndicatorData(indicatorId, indicatorName) {
    var ajaxAction = '/profiles-and-indicators/browse-data/' + indicatorId.trim();
    var popupId = "#viewIndicatorData";

    loading();

    $.ajax({
        cache: false,
        type: 'Get',
        url: ajaxAction,
        traditional: true,
        dataType: 'html',
        success: function (data) {
            var h = data.replace('{indicator-name}', indicatorName);
            lightbox.show(h, 20, 800, 1000);
            $(popupId).show();
            loadingFinished();
            initDataBrowse();
        },
        error: function (xhr, error) {
            loadingFinished();
        }
    });
}

function initDataBrowse() {

    $('.filter-item').unbind().change(function () {
        enableDeleteButton(false);
        enableGetButton(true);
    });

    // Area code filter
    $('input.filter-item').keydown(function (event) {
        // Do not want enter press submit form
        if (event.keyCode === 13) {
            event.preventDefault();
            return false;
        }
        enableDeleteButton(false);
        enableGetButton(true);
    });

    $('#btn-get-filteredData').unbind().click(function () {
        applyFilterAndRetrieveData();
    });

    $('#btn-delete-filteredData').unbind().click(function () {
        deleteDataSet();
    });

    function applyFilterAndRetrieveData() {
        var form = $('form#CoreDataFilterForm');
        $.ajax({
            cache: false,
            async: true,
            type: "POST",
            url: form.attr('action'),
            data: form.serialize(),
            success: function (data) {
                $('#coreDataView').html(data);
                enableDeleteButton(true);
                enableGetButton(false);
                initDataBrowse();
            },
            error: function (data) {
                showSimpleMessagePopUp('An error occurred while retrieving the data');
            }
        });
    }

    function deleteDataSet() {
        loading();

        var form = $('form#CoreDataFilterForm');
        $.ajax({
            cache: false,
            async: true,
            type: 'POST',
            url: '/profiles-and-indicators/delete-data',
            data: form.serialize(),
            success: function (data) {
                $('#coreDataView').html(data);
                enableDeleteButton(false);
                loadingFinished();
                initDataBrowse();
            },
            error: function (data) {
                showSimpleMessagePopUp('An error occurred while deleting the data');
                enableDeleteButton(true);
            }
        });
    }

    function enableGetButton(isEnabled) {
        enableButton(isEnabled, '#btn-get-filteredData');
    }

    function enableDeleteButton(isEnabled) {
        enableButton(isEnabled, '#btn-delete-filteredData');
    }

    function enableButton(isEnabled, elementId) {
        var disabled = 'disabled';
        var $element = $(elementId);
        $element.prop(disabled, !isEnabled);
        if (isEnabled) {
            $element.removeClass(disabled);
        } else {
            $element.addClass(disabled);
        }
    }
}

$(document).ready(function() {

    $('.view-indicator-data-btn').click(function() {
        var indicatorId = this.id.replace('view-data-', '');
        var indicatorName = $(this).attr('indicatorname');
        viewIndicatorData(indicatorId, indicatorName);
    });
});
