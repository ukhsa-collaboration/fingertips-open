
$(document).ready(function() {

    registerReloadPopUpDomains();
    initSearchElements();
    initTableSorter();

    $('#copy_multiple_indicators').click(function() {
        var selectedIndicators = [];

        $('.indicator-check-box:checked').each(function() {
            selectedIndicators.push($(this).attr('id'));
        });

        $('#selectedIndicators').val(selectedIndicators.toString());
    });

    $('.select-all-check-box').click(function () {
        $('.indicator-check-box').prop('checked', $(this).is(':checked'));
    });

    $('.indicator-check-box').change(function () {
        var selectAll = true;
        $($('.indicator-check-box')).each(function () {
            if (!$(this).prop('checked')) {
                selectAll = false;
            }
        });

        $('.select-all-check-box').prop('checked', selectAll);
    });

    $('.view-indicator-data-btn').click(function () {
        var indicatorId = this.id.replace('view-data-', '');
        var indicatorName = $(this).attr('indicatorname');
        viewIndicatorData(indicatorId, indicatorName);
    });

    $('#remove-indicators-button').click(function () {
        var selectedIndicators = [];

        $('.indicator-check-box:checked').each(function () {
            selectedIndicators.push($(this).attr('id'));
        });

        var uniqueSelectedIndicators = jQuery.unique(selectedIndicators);

        if (uniqueSelectedIndicators.length > 0) {
            loading();

            $.ajax({
                cache: false,
                type: 'Get',
                url: '/search/remove-multiple-indicators',
                data: 'selectedIndicators=' + uniqueSelectedIndicators.toString(),
                traditional: true,
                dataType: 'html',
                success: function (data) {
                    var h = data;
                    lightbox.show(h, 20, 300, 700);
                    $('#removeIndicatorsConfirmation').show();
                    loadingFinished();
                },
                error: function (xhr, error) {
                    loadingFinished();
                }
            });
        } else {
            showSimpleMessagePopUp('Please select some indicators to remove');
        }
    });

    $('#remove-multiple-indicators-confirm-button').click(function() {
        loading();

        var jdata = [];
        $.each($('.indicator-id'), function (index, value) {
            jdata.push(value.value);
        });

        $('#removeMultipleIndicatorDetails').val(jdata);
        $('form#RemoveMultipleIndicatorsForm').submit();
    });
});
