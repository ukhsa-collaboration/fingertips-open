/**
* SiteBaseLongerLives.js
* @module SiteBaseLongerLives
*/

MT = {};

function updateModelFromHash() {
    var model = MT.model;

    if (!model.isHashRestored) {
        model.isHashRestored = true;
        ftHistory.init(model);
        model.update();
    } else {
        ftHistory.setHistory();
    }
}

function handleAjaxFailure(e) {

    //var href = window.location.href;

    // Only reset URL if it contains a hash state string
    //if (href.indexOf('#') > 0) {
    //  alert("The data could not be loaded. Please try again.");
    //  PP.model.reset();
    //}
}

function getAreaTypeNamePlural(areaTypeId) {
    return areaTypeId === AreaTypeIds.CCG ? 'CCGs' :
        areaTypeId === AreaTypeIds.CountyUA ? 'Counties & Unitary Authorities' :
        areaTypeId === AreaTypeIds.DistrictUA ? 'Districts & Unitary Authorities' :
        'General Practices';
}

function getAreaTypeNameSingular(areaTypeId) {
    return areaTypeId === AreaTypeIds.CCG ? 'CCG' :
        areaTypeId === AreaTypeIds.CountyUA ? 'County & Unitary Authority' :
        areaTypeId === AreaTypeIds.DistrictUA ? 'District & Unitary Authority' :
        'General Practice';
}

function getSimpleAreaTypeName() {
    return MT.model.parentAreaType === AreaTypeIds.CountyUA ? 'County/UA' : 'CCG';
}

function getSimpleAreaTypeNamePlural(areaTypeId) {

    if (areaTypeId === AreaTypeIds.CCG) {
        return 'CCG';
    }

    return areaTypeId === AreaTypeIds.Practice
        ? 'practices'
        : 'areas';
}

function getSimpleAreaTypeNameSingular(areaTypeId) {

    if (areaTypeId === AreaTypeIds.CCG) {
        return 'CCG';
    }

    return areaTypeId === AreaTypeIds.Practice
        ? 'practice'
        : 'area';
}

function getSimilarAreaTooltipText() {

    var areaTypeId = MT.model.areaTypeId;
    var text;

    switch (areaTypeId) {
        case AreaTypeIds.CountyUA:
            text = 'This view shows your chosen local authority in comparison to the other local authorities with similar levels of socioeconomic deprivation';
            break;
        case AreaTypeIds.DistrictUA:
            text = 'This view shows your chosen local authority in comparison to those in the same ONS cluster';
            break;
        case AreaTypeIds.CCG:
            text = 'This view shows your chosen CCG in comparison to similar CCGs';
            break;
        case AreaTypeIds.Practice:
            text = 'This view shows practices that are similar to your selected practice';
            break;
        default:
    }

    return text;
}

function getCardinal(rank) {

    var rankString = rank.toString();
    var rankPenultimateChar = (rank < 10) ?
        '' :
        rankString.charAt(rankString.length - 2);

    if (rankPenultimateChar !== '1') {
        var lastChar = rankString.charAt(rankString.length - 1);
        switch (lastChar) {
            case '1':
                return 'st';
            case '2':
                return 'nd';
            case '3':
                return 'rd';
        }
    }

    return 'th';
}

function getAllAreas(model) {

    if (!loaded.areaLists[model.areaTypeId]) {

        var parameters = new ParameterBuilder();

        parameters.add('area_type_id', model.areaTypeId);
        parameters.add('profile_id', model.profileId);
        parameters.add('retrieve_ignored_areas', 'yes');

        ajaxGet('api/areas/by_area_type',
            parameters.build(), function (obj) {

                // Flatten hash
                var nationalAreaHash = {};

                _.each(obj, function (a) { nationalAreaHash[a.Code] = a; });

                loaded.areaLists[model.areaTypeId] = nationalAreaHash;

                ajaxMonitor.callCompleted();
            });

    } else {
        ajaxMonitor.callCompleted();
    }
}

/**
* AJAX call to fetch the area types for a profile.
* @class getAreaTypes
*/
function getAreaTypes() {

    var parameters = new ParameterBuilder().add('profile_ids',
        getProfileIds(MT.model.profileId));

    ajaxGet('api/area_types',
        parameters.build(),
        function (areaTypes) {
            loaded.areaTypes = areaTypes;
            ajaxMonitor.callCompleted();
        });
}

function AreaFilterModelBuilder(areaTypes, selectedAreaTypeId) {

    _.each(areaTypes, function (a) {
        if (a.Id === selectedAreaTypeId) {
            a['class'] = 'active';
        }

        a.Short = getAreaTypeNamePlural(a.Id);
    });

    //Ensure GP area types are never shown in the Longer Lives area filter.
    //This is need becaause the web service has been altered to now include GP area types as it is supported in Fingertips.
    areaTypes = areaTypes.filter(function (a) {
        return a.Id !== AreaTypeIds.Practice;
    });

    areaTypes.sort(sortAreasByShort);
    this.getModel = function () {
        return {
            types: areaTypes,
            isOneType: areaTypes.length === 1
        };
    }
}

function setAreaTypeOptionHtml(templateName) {

    var areaFilterModel = new AreaFilterModelBuilder(loaded.areaTypes, MT.model.areaTypeId).getModel();

    $('#area-filter').html(templates.render(templateName, areaFilterModel));
}

function getIndicatorMetadata(groupId, getDefinition, getSystemContent) {

    if (!isDefined(getDefinition)) {
        getDefinition = 'no';
    }

    if (!isDefined(getSystemContent)) {
        getSystemContent = 'no';
    }

    if (loaded.indicatorMetadata[groupId]) {
        // Data already loaded
        ajaxMonitor.callCompleted();
    } else {

        var parameters = new ParameterBuilder(
            ).add('include_system_content', getSystemContent
            ).add('group_ids', groupId
            ).add('include_definition', getDefinition);

        ajaxGet('api/indicator_metadata/by_group_id', parameters.build(),
            function (obj) {
                loaded.indicatorMetadata[groupId] = obj;
                ajaxMonitor.callCompleted();
            });
    }
};

function getIndicatorIdArgument() {
    return '';
}

function getAreaValuesCallback(obj) {

    loaded.areaValues[ajaxMonitor.state.indicatorKey] = obj;

    if (typeof (rootIndexesToGet) !== 'undefined' &&
            rootIndexesToGet.length) {
        // Remove the index that has been fetched
        rootIndexesToGet = _.without(rootIndexesToGet, rootIndexesToGet[0]);
    }

    ajaxMonitor.callCompleted();
}

/*
* Provide national area
*/
function getCurrentComparator() {

    var area = {};
    area.Code = MT.model.parentCode;
    return area;
}

function highlightSelectedTopic() {

    var id = null;

    switch (MT.model.profileId) {
        case ProfileIds.Diabetes:
            id = 'diabetes';
            break;
        case ProfileIds.Suicide:
            id = 'suicide';
            break;
        case ProfileIds.Mortality:
            id = 'mortality';
            break;
        case ProfileIds.HealthChecks:
            id = 'health-checks';
            break;
        case ProfileIds.Hypertension:
            id = 'blood-pressure';
            break;
        case ProfileIds.DrugsAndAlcohol:
            id = 'drugs-and-alcohol';
            break;
        case ProfileIds.Cancer:
            id = 'cancer';
            break;
    }

    if (id) {
        $('#' + id + '-topic').addClass('selected');
    }
}

function highlightSelectedTab() {
    var words = document.title.split(' '),
        lastWord = words[words.length - 1].toLowerCase();

    var lookUp = {
        'home': 'home-nav',
        'project': 'about-project-nav',
        'data': 'about-data-nav',
        'connect': 'connect-nav'
    }

    var id = lookUp[lastWord] ?
        lookUp[lastWord] :
        'rankings-nav';

    $('#' + id).addClass('selected');
}

function getDecileCategoryTypeId(areaTypeId) {
    switch (areaTypeId) {
        case AreaTypeIds.CountyUA:
            return CategoryTypeIds.DeprivationDecileCountyUA2015;
        case AreaTypeIds.DistrictUA:
            return CategoryTypeIds.DeprivationDecileDistrictUA2015;
        case AreaTypeIds.CCG:
            return CategoryTypeIds.DeprivationDecileCCG2010;
        default:
            throw 'Unsupported area type';
    }
}

/*
* Get deprivation decile of each area. Response is key/value pair list of 
* area code -> decile number
*/
function getDecileData(model) {
    if (!loaded.categories[AreaTypeIds.DeprivationDecile]) {

        var categoryTypeId = getDecileCategoryTypeId(model.areaTypeId);

        var parameters = new ParameterBuilder(
            ).add('profile_id', model.profileId
            ).add('category_type_id', categoryTypeId
            ).add('child_area_type_id', model.areaTypeId);

        ajaxGet('api/area_categories', parameters.build(),
            function (obj) {
                loaded.categories[AreaTypeIds.DeprivationDecile] = obj;
                ajaxMonitor.callCompleted();
            });

    } else {
        ajaxMonitor.callCompleted();
    }
}

function getDecileInfo(decile) {

    var suffix = ' deprived';

    if (decile > 8) {
        return {
            text: 'Least' + suffix,
            quintile: 1,
            decile: decile,
            color: quintileColors[0]
        };
    }

    if (decile > 6) {
        return {
            text: 'Less' + suffix,
            quintile: 2,
            decile: decile,
            color: quintileColors[1]
        };
    }

    if (decile > 4) {
        return {
            text: 'Average',
            quintile: 3,
            decile: decile,
            color: quintileColors[2]
        };
    }

    if (decile > 2) {
        return {
            text: 'More' + suffix,
            quintile: 4,
            decile: decile,
            color: quintileColors[3]
        };
    }

    return {
        text: 'Most' + suffix,
        quintile: 5,
        decile: decile,
        color: quintileColors[4]
    };
}

function getChildAreasCallback(obj) {

    var areaTypeId = MT.model.areaTypeId;

    loaded.areaLists[areaTypeId][MT.model.parentCode] = obj;

    ajaxMonitor.callCompleted();
}

function displayAreaRangeElements(isNational) {

    new MutuallyExclusiveDisplay({
        a: $('.national'),
        b: $('.similar')
    }).showA(isNational);
}

function getSimilarAreaCode() {
    return loaded.areaDetails.getData().Decile.Code;
}

function isSimilarAreas() {
    var model = MT.model;
    return model.parentCode !== NATIONAL_CODE &&
        model.areaTypeId !== AreaTypeIds.Practice;
}

function getUrlParameterString() {
    var href = window.location.href;
    var bits = href.split('?');
    return bits.length > 1 ?
        bits[1] :
        '';
}

function getParentDetails(parentCode, areaCode, index) {
    if (!isDefined(areaCode)) {
        areaCode = NATIONAL_CODE;
    }
    var ranks = loaded.areaDetails.getData({ areaCode: parentCode }).Ranks[areaCode];
    return ranks[index].AreaRank;
}

function getParentPeriod(parentCode, areaCode, index) {
    if (!isDefined(areaCode)) {
        areaCode = NATIONAL_CODE;
    }
    var ranks = loaded.areaDetails.getData({ areaCode: parentCode }).Ranks[areaCode];
    return ranks[index].Period.replace(/ /g, '');
}

function lock() {
    FT.ajaxLock = 1;
}

function showLoadingSpinner() {
    var over;
    if (SPINNER_STATE) {
        over = '<div class="loading"></div>';
        $(over).appendTo('body');
    } else {
        SPINNER_STATE = true;
        over = '<div id="overlay"><div class="loading"></div></div>';
        $(over).appendTo('body');
    }
}

function removeLoadingSpinner() {
    $('#overlay').remove();
    $('.loading').remove();
}

function bindOverlayMenuEvents() {
    $(".header_summary_tag").hover(
        function () {
            var pos = $(this).position();
            $("#overlay-hover-menu").css({ top: pos.top + "px", left: (pos.left) + "px" }).show();

            $("#overlay-hover-menu").mouseleave(function () {
                $("#overlay-hover-menu").hide();
            });
        }
    );
}

function ajaxAreaSearch(text, successFunction, excludeParentAreasFromSearchResults) {

    var model = MT.model;
    var areaTypeId = model.areaTypeId === AreaTypeIds.Practice
        ? model.parentAreaType
        : model.areaTypeId;

    var parentAreas = [];
    if (!excludeParentAreasFromSearchResults) {
        parentAreas.push(areaTypeId);
    }

    getAreaSearchResults(text, successFunction, areaTypeId, shouldSearchRetreiveCoordinates, parentAreas);
}

function replacePercentageWithArialFont(text) {
    text = text.replace(/%/g, '<span class="arial">%</span>');
    return text;
}

function getGpPracticesNearbyCallback(obj) {
    loaded.nearbyPractices = obj;
    ajaxMonitor.callCompleted();
}

/**
* Retrieves and manages area details data.
* @class AreaDetailsDataManager
*/
function AreaDetailsDataManager() {

    var data = {};
    var _this = this;

    /**
    * Data is only publically accessible for debugging.
    * @property data
    */
    _this.data = data;

    var getDataKey = function (modelForKey) {
        return getKey(modelForKey.groupId, modelForKey.areaCode);
    }

    var setData = function (modelForKey, newData) {
        var key = getDataKey(modelForKey);
        data[key] = newData;
    };

    var getDataFromModel = function (modelForKey) {
        var key = getDataKey(modelForKey);
        return data[key];
    }

    var getModel = function (alternativeModel) {
        var modelCopy = _.clone(MT.model);
        $.extend(modelCopy, alternativeModel);
        return modelCopy;
    }

    /**
    * Gets complex data object that was retrieved by AJAX
    * @method getData
    */
    _this.getData = function (alternativeModel) {
        var modelForKey = getModel(alternativeModel);
        return getDataFromModel(modelForKey);
    };

    /**
	* Fetches data by AJAX.
	* @method fetchDataByAjax
	*/
    _this.fetchDataByAjax = function (alternativeModel) {

        var modelCopy = getModel(alternativeModel);

        if (!getDataFromModel(modelCopy)) {

            var parameters = new ParameterBuilder(
            ).add('profile_id', modelCopy.profileId
            ).add('group_id', modelCopy.groupId
            ).add('area_code', modelCopy.areaCode
            ).add('child_area_type_id', modelCopy.areaTypeId);

            ajaxGet('api/area_details',
                parameters.build(),
                function (obj) {
                    setData(modelCopy, obj);
                    ajaxMonitor.callCompleted();
                });
        } else {
            // Do not need to load data
            ajaxMonitor.callCompleted();
        }
    }
}

/**
* Retrieves and manages area values data.
* @class AreaValuesDataManager
*/
function AreaValuesDataManager() {

    var data = {};
    var _this = this;

    /**
    * Data is only publically accessible for debugging.
    * @property data
    */
    _this.data = data;

    var getDataKey = function (modelForKey, root) {
        return getKey(modelForKey.parentCode, root.IID, root.Sex.Id, root.Age.Id);
    }

    var setData = function (modelForKey, root, newData) {
        var key = getDataKey(modelForKey, root);
        data[key] = newData;
    };

    var getDataFromModel = function (modelForKey, root) {
        var key = getDataKey(modelForKey, root);
        return data[key];
    }

    var getModel = function (alternativeModel) {
        var modelCopy = _.clone(MT.model);
        $.extend(modelCopy, alternativeModel);
        return modelCopy;
    }

    /**
    * Gets complex data object that was retrieved by AJAX
    * @method getData
    */
    _this.getData = function (root, alternativeModel) {
        var modelForKey = getModel(alternativeModel);
        return getDataFromModel(modelForKey, root);
    };

    /**
	* Fetches data by AJAX.
	* @method fetchDataByAjax
	*/
    _this.fetchDataByAjax = function (root, alternativeModel) {

        var modelCopy = getModel(alternativeModel);

        if (!getDataFromModel(modelCopy, root)) {

            var parameters = new ParameterBuilder(
                ).add('group_id', modelCopy.groupId
                ).add('area_type_id', modelCopy.areaTypeId
                ).add('parent_area_code', modelCopy.parentCode
                ).add('comparator_id', -1
                ).add('indicator_id', root.IID
                ).add('sex_id', root.Sex.Id
                ).add('age_id', root.Age.Id);

            ajaxGet('api/latest_data/single_indicator_for_all_areas', parameters.build(), function (obj) {
                setData(modelCopy, root, obj);
                ajaxMonitor.callCompleted();
            });

        } else {
            // Do not need to load data
            ajaxMonitor.callCompleted();
        }
    }
}

function getOnsClusterCode(model) {
    var parameters = new ParameterBuilder();
    parameters.add('profile_id', model.profileId);
    parameters.add('child_area_type_id', model.areaTypeId);
    parameters.add('parent_area_type_id', AreaTypeIds.OnsClusterGroup);

    // Get parent areas
    ajaxGet('api/parent_to_child_areas', parameters.build(), getOnsClusterCodeCallback);
}

function getOnsClusterCodeCallback(obj) {

    var model = MT.model;
    var onsGroupCode;
    for (var onsCode in obj) {
        _.each(obj[onsCode], function (area) {
            if (area === model.areaCode) {
                onsGroupCode = onsCode;
            }
        });
    };

    onsClusterCode = onsGroupCode;
    ajaxMonitor.callCompleted();
}

quintileColors = ['#DED3EC', '#BEA7DA', '#9E7CC8', '#7E50B6', '#5E25A4'];

$(document).ready(function () {

    // Do not proceed if test page
    if (!MT.model) {
        return;
    }

    // Intercept search text parameter for devices where autocomplete does not work
    var parameter = getUrlParameterString();
    if (parameter.indexOf('search_text') === 0) {
        MT.nav.home();
    }

    bindOverlayMenuEvents();
    highlightSelectedTopic();
    highlightSelectedTab();

    initAreaSearch('#search_text', false);
    initSite();

    // Page specific initialisation
    if (typeof initPage !== 'undefined') {
        initPage();
    }
});

function unlock() {
    FT.ajaxLock = null;
}

GET_METADATA_DEFINITION = GET_METADATA_SYSTEM_CONTENT = 'yes';
SEARCH_NO_RESULT_TOP_OFFSET = 38;

SPINNER_STATE = false;

loaded.areaDetails = new AreaDetailsDataManager();
loaded.categories = {};
loaded.areaLists = {};

/**
* Call out box for map.
* @class CallOutBox
*/
CallOutBox = {};

AnalyticsCategories = {
    DETAILS: 'AreaDetailsPage',
    RANKINGS: 'RankingsPage',
    MAP: 'MapPage'
}

AnalyticsAction = {
    ALL_AREAS: 'AllAreasSelected',
    SIMILAR_AREAS: 'SimilarAreasSelected'
}
