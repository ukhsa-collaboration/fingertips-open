'use strict';

function goToEnglandPage() {

    setPageMode(PAGE_MODES.ENGLAND);

    if (!areIndicatorsInDomain()) {
        displayNoData();
    } else {
        callAngularEvent('EnglandSelected');
    }
}

pages.add(PAGE_MODES.ENGLAND, {
    id: 'england',
    title: 'England',
    icon: 'england',
    goto: goToEnglandPage,
    gotoName: 'goToEnglandPage',
    needsContainer: false,
    jqIds: ['england-container', 'value-note-legend', 'trend-marker-legend', 'filter-indicator-wrapper']
});
