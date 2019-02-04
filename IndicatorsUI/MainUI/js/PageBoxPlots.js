'use strict';

/**
* BoxPlot namespace
* @module boxplot
*/

function goToBoxPlotPage() {

    setPageMode(PAGE_MODES.BOX_PLOT);

    if (!areIndicatorsInDomain()) {
        displayNoData();
    } else {

        showSpinner();
        callAngularEvent('BoxplotSelected');
    }
}

pages.add(PAGE_MODES.BOX_PLOT, {
    id: 'boxplots',
    title: 'Box<br/>Plots',
    goto: goToBoxPlotPage,
    gotoName: 'goToBoxPlotPage',
    needsContainer: false,
    jqIds: ['boxplot-container', 'indicator-menu-div', 'spine-range-key', 'spineKey', '.geo-menu', 'export-chart-box',
        'spineRangeKeyContainer', 'nearest-neighbour-link', 'area-list-wrapper', 'filter-indicator-wrapper']
});