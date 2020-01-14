"use strict";

function goToInequalitiesPage() {

    setPageMode(PAGE_MODES.INEQUALITIES);

    if (!areIndicatorsInDomain()) {
        displayNoData();
    } else {
        showSpinner();
        callAngularEvent('InequalitiesSelected');
    }
}

pages.add(PAGE_MODES.INEQUALITIES,
    {
        id: "inequalities",
        title: "Inequalities",
        icon: "inequalities",
        'goto': goToInequalitiesPage,
        gotoName: "goToInequalitiesPage",
        needsContainer: false,
        jqIds: ["inequalities-container", ".geo-menu", "indicator-menu-div", "nearest-neighbour-link", 'area-list-wrapper', 'filter-indicator-wrapper'],
        jqIdsNotInitiallyShown: ["key-ad-hoc", "key-bar-chart", "inequalities-trend-box", "benchmark-box"]
    });
