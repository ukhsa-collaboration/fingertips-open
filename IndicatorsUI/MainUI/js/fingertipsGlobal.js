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
    data: function() {
        return FT.data;
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
    formatTrendDataCount: function (dataInfo) {
        return formatCount(dataInfo);
    },
    isSubnationalColumn: function() {
        return isSubnationalColumn();
    },
    newCoreDataSetInfo: function (data) {
        return new CoreDataSetInfo(data);
    },
    newTrendDataInfo: function (trendDataPoint) {
        return new TrendDataInfo(trendDataPoint);
    },
    newValueDisplayer: function (unit) {
        return new ValueDisplayer(unit);
    },
    newTrendValueDisplayer: function (unit) {
        return new ValueDisplayer(unit);
    },
    newCommaNumber: function (n) {
        return new CommaNumber(n);
    },
    newValueNoteTooltipProvider: function () {
        return new ValueNoteTooltipProvider();
    },
    newValueSuffix: function(unit) {
        return new ValueSuffix(unit);
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
    newComparisonConfigForTrendRoot: function (trendRoot, indicatorMetadata) {
        return new ComparisonConfig(trendRoot, indicatorMetadata);
    },
    newTooltipManager: function () {
        return tooltipManager;
    },
    getMarkerImageFromSignificance: function (significance, useRag, suffix, useQuintileColouring, indicatorId, sexId, ageId) {
        return getMarkerImageFromSignificance(significance, useRag, suffix, useQuintileColouring, indicatorId, sexId, ageId);
    },
    getMiniMarkerImageFromSignificance: function (significance, useRag, useQuintileColouring, indicatorId, sexId, ageId) {
        return getMiniMarkerImageFromSignificance(significance, useRag, useQuintileColouring, indicatorId, sexId, ageId);
    },
    getArea: function (areaCode) {
        return areaHash[areaCode];
    },
    getAreaHash: function() {
        return areaHash;
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
    getValueNoteSymbol: function(){
        return getValueNoteSymbol();
    },
    goToBarChartPage: function (rootIndex, triggeredExternally) {
        goToBarChartPage(rootIndex, triggeredExternally);
    },
    goToMetadataPage: function (rootIndex) {
        goToMetadataPage(rootIndex);
    },
    goToAreaTrendsPage: function (rootIndex) {
        goToAreaTrendsPage(rootIndex);
    },
    goToAreaProfilePage: function() {
        goToAreaProfilePage();
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
    isLocked: function () {
        return isLocked();
    },
    lock: function () {
        lock();
    },
    unlock: function () {
        unlock();
    },
    isFeatureEnabled: function (feature) {
        return isFeatureEnabled(feature);
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
        },
        setIndicatorIndex: function(indicatorIndex) {
            setIndicatorIndex(indicatorIndex);
            var root = groupRoots[indicatorIndex];
            FT.menus.area.setAdditionalParameters(root.IID, root.Age.Id, root.Sex.Id);
            ftHistory.setHistory();
        }
    },
    getAreaMappingsForParentCode: function(key) {
        if (FT.model.isNearestNeighbours()) {
            key = key + FT.model.nearestNeighbour;
            return loaded.areaMappings[key][FT.model.areaCode];
        }
        return loaded.areaMappings[key][FT.model.parentCode];
    },
    getIndicatorKey: function(root, model, comparatorCode) {
        return getIndicatorKey(root, model, comparatorCode);
    },
    getAreaValues: function() {
        return loaded.areaValues;
    },
    setAreaValues: function(key, data) {
        loaded.areaValues[key] = addOrderandPercentilesToData(data);
    },
    setTrendMarkers: function(key, data) {
        loaded.trendMarkers[key] = data;
    },
    getAreaNamesAndCodes: function(index) {
        return loaded.areaNamesAndCodes[index];
    },
    getYAxisSelectedKey: function() {
        return FT.model.yAxisSelectedKey;
    },
    setYAxisSelectedKey: function(key) {
        FT.model.yAxisSelectedKey = key;
    },
    hideBenchmarkBox: function() {
        $('#benchmark-box').hide();
    },
    getAreasCodeDisplayed: function() {
        return getAreasCodeDisplayed();
    },
    getLegendDisplayStatus: function() {
        return FT.config.displayLegend;
    },
    setLegendDisplayStatus: function(state) {
        FT.config.displayLegend = state;
    },
    setAreaMappings: function (mappings) {
        new AreaMappings(FT.model).set(mappings);
    },
    setAreaMenuCode: function(areaCode) {
        FT.menus.area.setCode(areaCode);
    }
}



/**
 * Dispatches an event from JavaScript to a HostListener in Angular
 */
function callAngularEvent(eventName, eventDetail) {

    // Define polyfill CustomEvent for IE11 compatibility
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