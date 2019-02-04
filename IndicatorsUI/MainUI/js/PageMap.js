/**
* Map namespace
* @module map
*/
// ---------------- PageMap functions below --------------------

function goToMapPage() {

    setPageMode(PAGE_MODES.MAP);

    if (!areIndicatorsInDomain() && FT.model.areaTypeId !== AreaTypeIds.Practice) {
        // Search results empty
        displayNoData();
    } else {
        callAngularEvent('MapSelected');
        unlock();
    }
}

function hideAndShowMapMenus() {

    if (enumParentDisplay !== PARENT_DISPLAY.NATIONAL_AND_REGIONAL) {
        // Hide area and menus that aren't relevant
        var menus = FT.menus;
        menus.area.hide();
        menus.parentType.hide();
    }
}

pages.add(PAGE_MODES.MAP, {
    id: 'map',
    title: 'Map',
    goto: goToMapPage,
    gotoName: 'goToMapPage',
    needsContainer: false,
    jqIds: ['map-container', 'indicator-menu-div', '.geo-menu', 'benchmark-box', 'nearest-neighbour-link', 'region-menu-box', 'area-list-wrapper', 'filter-indicator-wrapper'],
    jqIdsNotInitiallyShown: ['target-benchmark-box'],
    showHide: hideAndShowMapMenus
});
