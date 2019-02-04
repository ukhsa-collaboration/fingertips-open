/**
 * This file is created to integrate fingertips FT model data available to Angular2 App.
 * Variable declared in Angular app to access this data must be named "FTWrapper"
 */
var FTWrapper = {
    config: function() {
        return FT.config;
    },
    model: function() {
        return FT.model;
    },
    url: function() {
        return FT.url;
    },
    getAreaName: function (areaCode) {
        var area = new AreaCollection(loaded.areaLists[FT.model.areaTypeId]).find(areaCode);
        if (area)
            return area.Name;
        else
            return '';
    },
    getCurrentDomainName: function () {
        return getCurrentDomainName();
    },
    getAreaList: function () {
        return loaded.areaLists[FT.model.areaTypeId];
    },
    getComparatorId: function () {
        return comparatorId;
    },
    getValueNotes: function () {
        return loaded.valueNotes;
    },
    getValueNoteById: function (id) {
        return loaded.valueNotes[id];
    },
    getParentAreaName: function () {
        return getParentArea().Name;
    },
    getParentTypeId: function() {
        return FT.model.parentTypeId;
    },
    getParentTypeName: function () {
        return FT.menus.parentType.getName();
    },
    getAreaTypeId: function() {
        return FT.model.areaTypeId;
    },
    getAreaTypeName: function () {
        return FT.menus.areaType.getName();
    },
    exportTableAsImage : function(containerId, fileNamePrefix, legends) {
        exportTableAsImage(containerId, fileNamePrefix, legends);
    },
    getGroupingSubheadings: function() {
        return ui.getGroupingSubheadings();
    },
    getAreaNameToDisplay: function(area) {
        return getAreaNameToDisplay(area);
    },
    logEvent : function(category, action, label) {
        logEvent(category, action, label);
    },
    hasDataChanged: function(groupRoot) {
        return hasDataChanged(groupRoot);
    },
    getIndicatorNameTooltip: function(rootIndex, area) {
        return getIndicatorNameTooltip(rootIndex, area);
    },
    initBootstrapTooltips: function() {
        // Enable tooltips
        setTimeout(
            function() {
                $('[data-toggle="tooltip"]').tooltip();
            }, 0);

    },
    formatCount: function (dataInfo) {
        return formatCount(dataInfo);
    },
    isSubnationalColumn: function() {
        return isSubnationalColumn();
    },
    newCoreDataSetInfo: function (data) {
        return new CoreDataSetInfo(data);
    },
    newValueDisplayer: function (unit) {
        return new ValueDisplayer(unit);
    },
    newCommaNumber: function (n) {
        return new CommaNumber(n);
    },
    newValueNoteTooltipProvider: function () {
        return new ValueNoteTooltipProvider();
    },
    newRecentTrendsTooltip: function () {
        return new RecentTrendsTooltip();
    },
    newIndicatorFormatter: function (groupRoot, metadata, coreDataSet, indicatorStatsF) {
        return new IndicatorFormatter(groupRoot, metadata, coreDataSet, indicatorStatsF);
    },
    newComparisonConfig: function (groupRoot, indicatorMetadata) {
        return new ComparisonConfig(groupRoot, indicatorMetadata);
    },
    newTooltipManager: function () {
        return tooltipManager;
    },
    getMarkerImageFromSignificance: function (significance, useRag, suffix, useQuintileColouring, indicatorId, sexId, ageId) {
        return getMarkerImageFromSignificance(significance, useRag, suffix, useQuintileColouring, indicatorId, sexId, ageId);
    },
    getArea: function (areaCode) {
        return areaHash[areaCode];
    },
    getComparatorById: function (comparatorId) {
        return getComparatorById(comparatorId);
    },
    getRegionalComparatorGrouping: function (groupRoot) {
        return getRegionalComparatorGrouping(groupRoot);
    },
    getNationalComparatorGrouping: function (groupRoot) {
        return getNationalComparatorGrouping(groupRoot);
    },
    getProfileUrlKey: function() {
        return profileUrlKey;
    },
    getSexAndAgeLabel: function (groupRoot) {
        return new SexAndAge().getLabel(groupRoot);
    },
    getTrendMarkerImage: function (trendMarker, polarityId) {
        return getTrendMarkerImage(trendMarker, polarityId);
    },
    getIndicatorDataQualityHtml: function (text) {
        return getIndicatorDataQualityHtml(text);
    },
    getIndicatorDataQualityTooltipText: function (dataQualityCount) {
        return getIndicatorDataQualityTooltipText(dataQualityCount);
    },
    getParentArea: function () {
        return getParentArea();
    },
    getNationalComparator: function() {
        return getNationalComparator();
    },
    goToBarChartPage: function (rootIndex) {
        goToBarChartPage(rootIndex);
    },
    goToMetadataPage: function (rootIndex) {
        goToMetadataPage(rootIndex);
    },
    goToAreaTrendsPage: function (rootIndex) {
        goToAreaTrendsPage(rootIndex);
    },
    recentTrendSelected: function () {
        return recentTrendSelected;
    },
    showIndicatorMetadataInLightbox: function (elementRef) {
        showIndicatorMetadataInLightbox(elementRef);
    },
    showAndHidePageElements: function () {
        showAndHidePageElements();
    },
    showTargetBenchmarkOption: function (roots) {
        showTargetBenchmarkOption(roots);
    },
    getTargetLegendHtml: function (comparisonConfig, metadata) {
        return getTargetLegendHtml(comparisonConfig, metadata);
    },
    lightboxShow : function(html, top, left, popupWidth) {
        lightbox.show(html, top, left, popupWidth);
    },
    lock: function () {
        lock();
    },
    unlock: function () {
        unlock();
    },
    version: function () {
        return FT.version;
    },
    saveElementAsImage: function (element, outputFilename) {
        return saveElementAsImage(element, outputFilename);
    },
    setAreaCode: function (areaCode) {
        FT.menus.parent.setCode(new AreaMappings(FT.model).getParentAreaCode(areaCode));
        FT.menus.area.setCode(areaCode);
        setAreas();
        ftHistory.setHistory();
    },
    setComparatorId: function(id) {
        FT.menus.benchmark.setComparatorId(id);
        benchmarkChanged();
    },
    redirectToPopulationPage: function () {
        var typeCheck = typeof (goToPopulationPage);
        if (typeCheck === 'undefined')
            goToAreaProfilePage();
        else
            goToPopulationPage();
    },
    getGroopRoot: function () {
        return getGroupRoot();
    },
    getCurrentComparator: function () {
        return getComparatorById(getComparatorId());
    },
    showDataQualityLegend: function() {
        showDataQualityLegend();
    },
    showTrendInfo: function() {
        showTrendInfo();
    },
    search: {
        getIndicatorListId: function() {
            return indicatorListId;
        },
        isIndicatorList: function () {
            return isDefined(indicatorListId) && indicatorListId !== '';
        },
        isInSearchMode: function () {
            return isInSearchMode();
        },
        getIndicatorIdList: function () {
            return indicatorIdList;
        },
        getIndicatorIdsParameter: function() {
            return getIndicatorIdsParameter();
        },
        getProfileIdsForSearch: function () {
            // All searchable IDs will be used on server
            return [];
        }
    },
    coreDataHelper: {
        addOrderandPercentilesToData: function (coreDataSet) {
            return addOrderandPercentilesToData(coreDataSet);
        },
        valueWithUnit: function (unit) {
            return new ValueWithUnit(unit);
        }
    },
    indicatorHelper: {
        getMetadataHash: function () {
            return loaded.indicatorMetadata[FT.model.groupId];
        },
        getIndicatorIndex: function () {
            return getIndicatorIndex();
        }
    },
    getAreaMappingsForParentCode: function(key) {
        return loaded.areaMappings[key][FT.model.parentCode];
    }
}



/**
 * Dispatches an event from JavaScript to a HostListener in Angular
 */
function callAngularEvent(eventName, eventDetail) {

    // Define polyfill CustomEvent for IE11 compatability
    var PolyfillCustomEvent = function (event, params) {
        var evt = document.createEvent('CustomEvent');
        //evt.initCustomEvent(event, null, null, params.detail);
        evt.initCustomEvent(event, null, null, params);
        return evt;
    };
    eventDetail = eventDetail || { 'detail': true };
    var event = new PolyfillCustomEvent(eventName, eventDetail);
    window.dispatchEvent(event);
}