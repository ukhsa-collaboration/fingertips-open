/**
 * This file is created to integrate fingertips FT model datta available to Angular2 App. Variable declared in Angular app to access this data must be named "FTWrapper"
 */
var FTWrapper = {
    config: function () {
        return FT.config;
    },
    model: function () {
        return FT.model;
    },
    url: function () {
        return FT.url;
    },
    logEvent : function(category, action, label) {
        logEvent(category, action, label);
    },
    formatCount: function (dataInfo) {
        return formatCount(dataInfo);
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
    newComparisonConfig: function (groupRoot, indicatorMetadata) {
        return new ComparisonConfig(groupRoot, indicatorMetadata);
    },
    newTooltipManager: function () {
        return tooltipManager;
    },
    getNationalComparatorGrouping: function (groupRoot) {
        return getNationalComparatorGrouping(groupRoot);
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
    goToMetadataPage: function (rootIndex) {
        goToMetadataPage(rootIndex);
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
    redirectToPopulationPage: function () {
        var typeCheck = typeof (goToPopulationPage);
        if (typeCheck === 'undefined')
            goToAreaProfilePage();
        else
            goToPopulationPage();
    },
    search: {
        getIndicatorListId: function() {
            return indicatorListId;
        },
        isIndicatorList: function () {
            return isDefined(indicatorListId);
        },
        isInSearchMode: function () {
            return isInSearchMode();
        },
        getIndicatorIdList: function () {
            return indicatorIdList;
        },
        getProfileIdsForSearch: function () {
            // All searchable IDs will be used on server
            return [];
        }
    },
    display: {
        getBenchmarkAreaName: function () {
            return getCurrentComparator().Name;
        },
        getAreaName: function (areaCode) {
            var area = new AreaCollection(loaded.areaLists[FT.model.areaTypeId]).find(areaCode);
            if (area)
                return area.Name;
            else
                return '';
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
        getParentTypeName: function () {
            return new ParentTypes(model).getCurrent().Name;
        },
        getAreaTypeName: function () {
            return FT.menus.areaType.getName();
        },
        getIndicatorName: function () {
            return getIndicatorName(indicatorId);
        },
        getGroupName: function () {
            return getCurrentDomainName();
        },
        getCurrentTabId: function () {
            return pages.getCurrent();
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
    bridgeDataHelper: {
        getGroopRoot: function () {
            return getGroupRoot();
        },
        getAllGroupRoots: function () {
            return groupRoots;
        },
        getComparatorId: function () {
            return getComparatorId();
        },
        getCurrentComparator: function () {
            return getComparatorById(getComparatorId());
        }
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