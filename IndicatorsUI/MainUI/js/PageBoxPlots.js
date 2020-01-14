'use strict';

/**
* BoxPlot namespace
* @module boxplot
*/

function goToBoxPlotPage() {

    setPageMode(PAGE_MODES.BOX_PLOT);

    if (!areIndicatorsInDomain()) {
        displayNoData();
        callAngularEvent('NoDataDisplayed', { 'isEnglandAreaType': isEnglandAreaType() });
    } else {

        showSpinner();
        callAngularEvent('BoxplotSelected', { 'isEnglandAreaType': isEnglandAreaType() });
    }
}

pages.add(PAGE_MODES.BOX_PLOT, {
    id: 'boxplots',
    title: 'Box<br/>plots',
    goto: goToBoxPlotPage,
    gotoName: 'goToBoxPlotPage',
    needsContainer: false,
    jqIds: ['boxplot-container', 'indicator-menu-div', 'spine-range-key', 'spineKey', '.geo-menu', 'export-chart-box',
        'spineRangeKeyContainer', 'nearest-neighbour-link', 'area-list-wrapper', 'filter-indicator-wrapper']
});