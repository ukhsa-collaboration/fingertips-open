'use strict';

function goToBarChartPage(rootIndex, triggeredExternally) {
    setPageMode(PAGE_MODES.INDICATOR_DETAILS);

    if (!areIndicatorsInDomain()) {
        displayNoData();
        callAngularEvent('NoDataDisplayed', { 'isEnglandAreaType': isEnglandAreaType() });
    } else {
        showSpinner();
        callAngularEvent('CompareAreaSelected', {
            'isEnglandAreaType': isEnglandAreaType(),
            'rootIndex': rootIndex,
            'triggeredExternally': triggeredExternally
        });
        adjustScrollTop();
    }
}

pages.add(PAGE_MODES.INDICATOR_DETAILS, {
    id: 'indicators',
    title: 'Compare areas',
    goto: goToBarChartPage,
    gotoName: 'goToBarChartPage',
    needsContainer: true,
    jqIds: ['compare-area-container', 'indicator-menu-div', '.geo-menu', 'value-note-legend',
        'nearest-neighbour-link', 'trend-marker-legend', 'area-list-wrapper', 'filter-indicator-wrapper'],
    jqIdsNotInitiallyShown: ['data-quality-key', 'key-ad-hoc', 'key-bar-chart', 'key-spine-chart', 'target-benchmark-box']
});
