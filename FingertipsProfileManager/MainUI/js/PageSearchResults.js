
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

    $('.view-indicator-data-btn').click(function () {
        var indicatorId = this.id.replace('view-data-', '');
        var indicatorName = $(this).attr('indicatorname');
        viewIndicatorData(indicatorId, indicatorName);
    });
});
