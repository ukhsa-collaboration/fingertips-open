/**
 * This file is created to integrate fingertips FT model datta available to Angular2 App. Variable declared in Angular app to access this data must be named "FTWrapper"
 */
var FTWrapper = {
    model: function() {
        return FT.model;
    },
    url: function() {
        return FT.url;
    },
    display: {
        getBenchmarkAreaName: function() {
            return getCurrentComparator().Name;
        },
        getAreaName: function (areaCode) {
            //return areaHash[areaCode].Name;
            return new AreaCollection(loaded.areaLists[FT.model.areaTypeId]).find(areaCode).Name;
        },
        getparentAreaName: function() {
            return getParentArea().Name;
        },
        getParentTypeName: function() {
            return new ParentTypes(model).getCurrent().Name;
        },
        getAreaTypeName: function() {
            return FT.menus.areaType.getName();
        },
        getIndicatorName: function() {
            return getIndicatorName(indicatorId);
        },
        
        getGroupName: function() {
            return getCurrentDomainName();
        },
        gettCurrentTabId: function() {
            return pages.getCurrent();
        }
    },
    coreDataHelper: {
        addOrderandPercentilesToData : function(coreDataSet) {
            return addOrderandPercentilesToData(coreDataSet);
        },
        
        valueWithUnit: function (unit) {
            return new ValueWithUnit(unit);
        }
    },
    indicatorHelper: {
        getMetadataHash: function () {
            return loaded.indicatorMetadata[FT.model.groupId];
        }
    },
    bridgeDataHelper: {
        getGroopRoot: function () {
            return getGroupRoot();
        },
        getComparatorId: function () {
            return getComparatorId();
        },
        getCurrentComparator: function () {
            return getComparatorById(getComparatorId());
        }
    }
}

