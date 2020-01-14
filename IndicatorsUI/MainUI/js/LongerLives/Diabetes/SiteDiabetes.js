/**
* SiteDiabetes namespace
* SiteDiabetes.js inherits from SiteBaseLongerLives.js
* @module SiteDiabetes
*/

function initSite() {
    searchPolygonAreaTypeId = MT.model.areaTypeId;
}

MT.model = {
    //area: null,
    areaCode: null,
    parentCode: null,
    areaTypeId: null,
    groupId: null,
    indicatorId: null,
    parentAreaType: null,
    sexId: null,
    similarAreaCode: null,
    isHashRestored: false,

    //
    // Resets the model
    //
    _reset: function () {
        var m = MT.model;
        m.areaCode = null;
        m.parentCode = NATIONAL_CODE;
    },

    //
    // Restores the UI from the hash state
    //
    restore: function () {
        this.update();
    },

    //
    // Updates the model from the current hash state
    //
    update: function () {
        this._reset();

        var pairs = ftHistory.getKeyValuePairsFromHash();

        if (_.size(pairs)) {
            var m = this;

            // IndicatorIID
            var prop = pairs['iid'];
            if (prop) m.indicatorId = parseInt(prop);

            // Sex ID
            var prop = pairs['sexId'];
            if (prop) m.sexId = parseInt(prop);

            // Group Id
            prop = pairs['gid'];
            if (prop) m.groupId = parseInt(prop);

            // Area Code
            prop = pairs['are'];
            if (prop) m.areaCode = prop;

            // Area type ID
            prop = pairs['ati'];
            if (prop) m.areaTypeId = parseInt(prop);

            // Parent area code
            prop = pairs['par'];
            if (prop) m.parentCode = prop;

            // Parent area type ID
            prop = pairs['pat'];
            if (prop) m.parentAreaType = parseInt(prop);

            // Similar area code
            prop = pairs['sim'];
            if (prop) m.similarAreaCode = prop;
        }

        updatePage();
    },

    //
    // Generates a string representation of the model to be used as the URL hash
    //

    _serialiseIdDefined: function (parameters, modelProperty, hashProperty) {
        var m = this;
        if (m.hasOwnProperty(modelProperty) && m[modelProperty]) {
            parameters.push(hashProperty, m[modelProperty]);
        }
    },

    toString: function () {
        var m = this;
        var parameters = [
            'par', m.parentCode,
            'ati', m.areaTypeId,
            'iid', m.indicatorId,
            'sexId', m.sexId,
            'gid', m.groupId,
            'pat', m.parentAreaType
        ];

        this._serialiseIdDefined(parameters, 'areaCode', 'are');
        this._serialiseIdDefined(parameters, 'similarAreaCode', 'sim');

        return parameters.join('/');
    }

};

function setSearchText(areaTypeId) {

    var $searchBox = $('#search_text');

    switch (areaTypeId) {
        case AreaTypeIds.CCGPreApr2017:
            var areaType = 'CCG';
            var areaType2 = 'GP practice or CCG';
            break;

        default:
            areaType = areaType2 = 'local authority';
            break;
    }

    $searchBox.val('enter postcode, town or ' + areaType);
    $searchBox.attr('title', 'enter postcode, town or ' + areaType);
    $('.home_search h2').html('See how your ' + areaType2 + ' compares');
}

function getGroupRoots(model) {
    //Reload group roots each time to ensure clicking on a grouping reloads the relevant indicators
    var parameters = new ParameterBuilder(
        ).add('group_id', model.groupId
        ).add('area_type_id', model.areaTypeId);
    ajaxGet('api/profile_group_roots', parameters.build(), getGroupRootsCallback);
}

function getSpecificGroupRoots(groupId, areaTypeId) {
    var parameters = new ParameterBuilder(
        ).add('group_id', groupId
        ).add('area_type_id', areaTypeId);
    ajaxGet('api/profile_group_roots', parameters.build(), getGroupRootsCallback);
}

function getGroupRootsCallback(obj) {
    groupRoots = obj;
    ajaxMonitor.callCompleted();
}

function getSupportingAreaDetails(areaCode, areaTypeId) {
    loaded.supportingAreaData.fetchDataByAjax({
        'profileId': SupportingProfileId,
        'groupId': SupportingGroupId,
        'areaCode': areaCode,
        'areaTypeId': areaTypeId
    });
}

/*
* Defines the indexes for specific indicator in the list of group roots
*/

ROOT_INDEXES = {
    POPULATION: 0,
    DIABETES: 1
};

function populateCauseList() {

    var model = MT.model;
    var groupId = model.groupId;
    var metadataHash = loaded.indicatorMetadata[groupId];
    var indicators = [];
    var root = groupRoots[selectedRootIndex];
    var selectedIndicatorId = root.IID;

    if (!isDefined(model.indicatorId)) {
        model.indicatorId = selectedIndicatorId;
        model.sexId = root.Sex.Id;
    }

    for (var i = 0; i < groupRoots.length; i++) {

        var indicatorId = groupRoots[i].IID;
        var indicatorSex = groupRoots[i].Sex.Id;
        var metadata = metadataHash[indicatorId];

        // Consider sex and indicator ID to determine whether it should be selected
        var isIndicatorSelected, lastIndicatorSex, lastIndicator;
        if (indicatorId !== lastIndicator && lastIndicatorSex !== indicatorSex) {
            lastIndicator = indicatorId;
            isIndicatorSelected = indicatorId === selectedIndicatorId;
        } else {
            lastIndicatorSex = indicatorSex;
            isIndicatorSelected = false;
        }

        var stateSex = groupRoots[i].StateSex;
        var sex = groupRoots[i].Sex.Name;
        var indicatorName = stateSex ? metadata.Descriptive.Name + ' (' + sex + ')' : metadata.Descriptive.Name;

        indicators.push({
            index: i,
            name: replacePercentageWithArialFont(indicatorName),
            id: indicatorId,
            cssClass: isIndicatorSelected ? 'sub active' : 'sub',
            noteText: '<span class=asterisk>*</span>'
        });

        if (isIndicatorSelected) {
            $('#' + groupId).addClass('active');
            selectedRootIndex = i;
        }
    }

    $('.causes.filters').html('');
    var html = templates.render('causes', { causes: indicators });
    $('#diabetes_list-' + groupId).html(html);
    $('#domain-' + groupId).addClass('active');

    unlock();
}

/**
* Retrieves and manages the group data for a specific area.
* @class GroupDataAtDataPointOfSpecificAreasDataManager
*/
function GroupDataAtDataPointOfSpecificAreasDataManager() {

    var data = {};
    var _this = this;

    /**
    * Data is only publically accessible for debugging.
    * @property data
    */
    _this.data = data;

    var getDataKey = function (modelForKey) {
        return getKey(modelForKey.groupId, modelForKey.areaCode, modelForKey.areaTypeId);
    }

    /**
    * Adds data object that was retrieved by AJAX to the manager
    * @method setData
    */
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

            var parameters = new ParameterBuilder()
                .add('area_code', modelCopy.areaCode)
                .add('area_type_id', modelCopy.areaTypeId)
                .add('profile_id', modelCopy.profileId)
                .add('group_id', modelCopy.groupId);

            ajaxGet('api/latest_data/all_indicators_in_profile_group_for_single_area',
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

function getValueNotes() {
    if (isDefined(loaded.valueNotes[0])) {
        // Data already loaded
        ajaxMonitor.callCompleted();
    } else {
        getValueNotesCall();
    }
}

function getValueNotesCallback(obj) {

    if (isDefined(obj)) {
        // Set up loaded value notes hash
        _.each(obj, function (valueNote) {
            loaded.valueNotes[valueNote.Id] = valueNote;
        });
    }

    ajaxMonitor.callCompleted();
}

function getValueNoteText(valueNoteId) {
    return loaded.valueNotes[valueNoteId].Text;
}

function getAreaAddress(areaCode) {

    // Get practice address details
    ajaxGet('api/area_address', 'area_code=' + areaCode, getAreaAddressCallback);
}

function getAreaAddressCallback(obj) {

    var code = obj.Code;
    loaded.addresses[code] = obj;

    ajaxMonitor.callCompleted();
}

function getChildAreas(model) {

    var areaTypeId = model.areaTypeId,
    parentCode = model.parentCode,
    areaLists = loaded.areaLists;

    if (!areaLists[areaTypeId]) {
        // Initialise hash for loaded child areas
        areaLists[areaTypeId] = {};
    }

    if (areaLists[areaTypeId][parentCode]) {
        // Child areas are already loaded
        ajaxMonitor.callCompleted();
    } else {
        // Get child areas
        var parameters = new ParameterBuilder(
            ).add('profile_id', model.profileId
            ).add('parent_area_code', model.parentCode
            ).add('area_type_id', model.areaTypeId);

        ajaxGet('api/areas/by_parent_area_code', parameters.build(),
            getChildAreasCallback);
    }
}

function getContentText(contentKey) {
    var model = MT.model;
    var parameters = new ParameterBuilder(
      ).add('profile_id', model.profileId
      ).add('key', contentKey);

    ajaxGet('api/content',
        parameters.build(),
        function (obj) {
            loaded.contentText = obj;
            ajaxMonitor.callCompleted();
        });
}

function isMapPage() {
    return $('#map').length > 0;
}

MT.nav = {

    _addIndicator: function (model) {

        // Use when navigating from map page to maintain selected indicator
        if (isMapPage() && groupRoots && groupRoots.length) {
            var root = groupRoots[selectedRootIndex];
            model.indicatorId = root.IID;
            model.sexId = root.Sex.Id;
        }
    },

    //
    // Go to the rankings page preserving the similar areas comparison if appropriate
    //
    rankings: function (model) {
        if (!model) {
            model = MT.model;
        }

        model.parentCode = NATIONAL_CODE;
        model.areaTypeId = model.parentAreaType;

        this._addIndicator(model);

        setUrl('/topic/' + profileUrlKey + '/comparisons#' + model.toString());
    },

    //
    // List practices in CCG
    //
    practiceRankings: function (parentCode) {
        var model = MT.model;

        model.areaTypeId = AreaTypeIds.Practice;
        model.parentCode = parentCode;
        this._addIndicator(model);

        // Similar area not relevant
        model.similarAreaCode = null;
        model.areaCode = null;

        setUrl('/topic/' + profileUrlKey + '/comparisons#' + model.toString());
    },

    nationalRankings: function (model) {
        if (!model) {
            model = MT.model;
        }

        model.parentCode = NATIONAL_CODE;
        model.similarAreaCode = null;
        model.areaTypeId = model.parentAreaType;

        this._addIndicator(model);

        setUrl('/topic/' + profileUrlKey + '/comparisons#' + model.toString());
    },

    //
    // Go to the practice details page
    //
    practiceDetails: function (model) {
        if (!isDefined(model)) {
            model = MT.model;
        }

        // Is user already on area details page?
        if (typeof (areaDetailsState) !== 'undefined') {
            ftHistory.setHistory();
            updatePage();
        } else {
            setUrl('/topic/' + profileUrlKey + '/practice-details#are/' + model.areaCode +
                '/par/' + model.parentCode + '/pat/' + model.parentAreaType + '/ati/' + model.parentAreaType);
        }
    },

    _addSimilarAreaParameters: function (url, similarAreaCode) {
        return url + '/par/' + similarAreaCode + '/sim/' + similarAreaCode;
    },

    //
    // Go to the area details page
    //
    areaDetails: function (model) {
        if (!isDefined(model)) {
            model = MT.model;
        }

        var addSimilarAreaParameters = this._addSimilarAreaParameters;
        var similarAreaCode;
        var url = '/topic/' + profileUrlKey + '/area-details#are/' + model.areaCode;

        if (isMapWithNoData()) {
            // Ensure area details will be comparing similar areas
            if (doesAreaTypeHaveNearestNeighbours()) {
                similarAreaCode = getNearestNeighbourCode();
                url = addSimilarAreaParameters(url, similarAreaCode);
                setUrl(url);
            } else {
                // Compare drivation group
                ajaxMonitor.setCalls(1);
                getDecileData(model);
                ajaxMonitor.monitor(function() {
                    var decileNumber = loaded.categories[AreaTypeIds.DeprivationDecile][model.areaCode];
                    similarAreaCode = getCategoryAreaCode(decileNumber);
                    url = addSimilarAreaParameters(url, similarAreaCode);
                    setUrl(url);
                });
            }
            return;
        }

        if (MT.model.profileId === ProfileIds.PublicHealthDashboard) {
            // Ensure same domain as selected on map is displayed on area details
            url += '/ati/' + model.areaTypeId + '/gid/' + model.groupId;
        }
        
        // Navigate keeping similar area selection
        if (model.similarAreaCode) {
            url = addSimilarAreaParameters(url, model.similarAreaCode);
        } else {
            url += '/par/' + model.parentCode;
        }

        setUrl(url);
    },

    _getParametersForMap: function() {

        // Navigate keeping similar area selection
        var data = '';
        var model = MT.model;
        if (model.similarAreaCode) {
            var similarAreaCode = model.similarAreaCode;
            data += '#par/' + similarAreaCode + '/sim/' + similarAreaCode +
                '/are/' + model.areaCode + '/ati/' + model.areaTypeId;
        }
        return data;
    },

    //
    // Go to the home page
    //
    home: function () {
        setUrl('/topic/' + profileUrlKey + this._getParametersForMap());
    },

    //
    // Go to map page that shows data
    //
    mapWithData: function () {
        setUrl('/topic/' + profileUrlKey + '/map-with-data' + this._getParametersForMap());
    }
}

function doesAreaTypeHaveNearestNeighbours() {
    return MT.model.areaTypeId === AreaTypeIds.CountyUA;
}

function getCategoryAreaCode(decile) {
    return 'cat-' + getDecileCategoryTypeId(MT.model.areaTypeId) + '-' + decile;
}

function useQuintiles(comparatorMethodId) {
    return comparatorMethodId === ComparatorMethodIds.Quintiles;
}

function useBlueOrangeBlue(comparatorMethodId) {
    return comparatorMethodId === PolarityIds.BlueOrangeBlue;
}

function getGradeFunctionFromGroupRoot(groupRoot) {
    return getGradeFunction(groupRoot.ComparatorMethodId);
}

function getGradeFunction(comparatorMethodId) {
    var noValue = 'no-data';

    if (useQuintiles(comparatorMethodId)) {
        return function (sig) {
            return sig > 0
                ? 'grade-quintile-' + sig
                : noValue;
        };
    }

    if (comparatorMethodId === ComparatorMethodIds.Quartiles) {
        return function (sig) {
            return sig > 0
                ? 'grade-' + (sig - 1)
                : noValue;
        };
    }

    function getSignificanceColour(sig, polarity) {
        switch (polarity) {
            case PolarityIds.BlueOrangeBlue:
                switch (sig) {
                    case 1:
                        return 'bobLower';
                    case 2:
                        return 'grade-' + 2; // Amber;
                    case 3:
                        return 'bobHigher';
                }
            default:
                // Polarity handled in web services re: HighIsGood/LowIsGood
                switch (sig) {
                    case 1:
                        return 'grade-' + 3; // Red
                    case 2:
                        return 'grade-' + 2; // Amber
                    case 3:
                        return 'grade-' + 0; // Green
                }
        }

        return noValue;
    };

    return function (sig, groupRoot) {
        if (isDefined(groupRoot)) {
            return getSignificanceColour(sig, groupRoot.PolarityId);
        } else {
            return getSignificanceColour(sig, PolarityIds.BlueOrangeBlue);
        }
    };
}

function UnitFormat(metadata, value) {

    var showUnit = value !== -1 && isDefined(metadata);
    var isPercentage = showUnit && metadata.Unit.Id === 5;
    var percentageLabel = '<span class="unit arial">%</span>';

    this.getLongLabel = function () {

        if (showUnit) {
            return isPercentage
                ? percentageLabel
                : ' ' + metadata.Unit.Label;
        }
        return '';
    };

    this.getLabel = function () {
        return isPercentage
            ? percentageLabel
            : '';
    };

    this.getClass = function () {
        var cssClass = '';
        if (showUnit) {
            var suffix = isPercentage
                ? '-percent'
                : '-long';

            cssClass = 'unit' + suffix;
        }
        return cssClass;
    };
}

function goToAreaDetails() {
    MT.nav.practiceDetails();
}

function areaSearchResultSelected(noMatches, searchResult) {

    if (!noMatches.is(':visible')) {        
        // Reset search box
        $('#search_text').val('').blur();
        var model = MT.model;

        var polygonAreaCode = searchResult.PolygonAreaCode;

        if (hasPracticeData) {
            if (searchResult.Easting) {
                // Go to search results page
                setUrl('/topic/' + profileUrlKey + '/area-search-results?' +
                    getSearchResultParameters(searchResult) +
                    '#par/' + polygonAreaCode +
                    '/pat/' + model.parentAreaType +
                    '/ati/' + AreaTypeIds.Practice);
            } else {
                // Parent polygon areas do not have easting/northing
                model.parentCode = polygonAreaCode;
                MT.nav.rankings();
            }
        } else {   // if we don't have practice redirect to area
            model.areaCode = polygonAreaCode;
            MT.nav.areaDetails(model);
            // if user is on the areaDetails page and perform search we need 
            // to reload the page.
            if (FT.ajaxLock) {
                location.reload();
            }
        }
    }
}

function getSearchResultParameters(searchResult) {
    var builder = new ParameterBuilder();
    builder.add('easting', searchResult.Easting
        ).add('northing', searchResult.Northing
        ).add('place_name', searchResult.PlaceName);
    return builder.build();
}

/**
 * Can current area type be compared to similar areas 
 */
function isComparisonAvailable(model) {
    return model.areaTypeId !== AreaTypeIds.Practice;
}

function getNearestNeighbours(postCallBackFunction) {
    var areaCode = MT.model.areaCode;
    ajaxGet('api/parent_to_child_areas',
        'nearest_neighbour_code=' + MT.model.similarAreaCode,
    function (data) {
        loaded.neighbours[areaCode] = data[areaCode];
        if (postCallBackFunction) postCallBackFunction();
        ajaxMonitor.callCompleted();
    });
}

function getNearestNeighbourCode() {
    return 'nn-1-' + MT.model.areaCode;
}

function getAreaCodeOfAreaWithNeighbours() {
    // e.g. 'E10000023' from 'nn-1-E10000023'
    return MT.model.similarAreaCode.split('-')[2];
}

function getDeprivationLabel(label, parentAreaType) {

    var areaTypeLabel = parentAreaType === AreaTypeIds.CountyUA ? 'County/UA'
        : parentAreaType === AreaTypeIds.CCGPreApr2017 ? 'CCG'
        : 'England';

    return label + ' within ' + areaTypeLabel;
}

/**
 * Function is overriden for map page
 */
function isMapWithNoData() {
    return false;
}

deprivationDefinitions = [
    'No data',
    'Least deprived',
    'Less deprived',
    'Average',
    'More deprived',
    'Most deprived'
];

loaded.selectedTreatmentTargetIndicatorValues = {};
loaded.addresses = {};
loaded.supportingAreaData = new AreaDetailsDataManager();
loaded.supportingPracticeData = {};
loaded.nhsId = {};
loaded.nearbyPractices = [];
loaded.practiceCategories = {};
loaded.onsClusterCodes = {};
loaded.areaDetailsForDiseaseAndDeath = new AreaDetailsDataManager();
loaded.contentText = '';
loaded.groupDataAtDataPoint = new GroupDataAtDataPointOfSpecificAreasDataManager(MT.model);


groupRoots = null;
selectedRootIndex = ROOT_INDEXES.POPULATION;
treatmentTargetSelected = null;
comparatorId = NATIONAL_COMPARATOR_ID;
shouldSearchRetreiveCoordinates = true;
NO_SUICIDE_PLAN = 'No information on current suicide plan status';

var callOutBoxFooter = '</div><div class="map-info-footer clearfix">\
<ul><li>View:</li>\
<li><a href=javascript:MT.nav.rankings();>National comparisons table</a></li>\
{{#hasPracticeData}}<li><a href="javascript:MT.nav.practiceRankings(\'{{areaCode}}\');">Local GP practices</a></li>{{/hasPracticeData}}\
{{^hasPracticeData}}<li><a href="javascript:MT.nav.areaDetails();">Details for this area</a></li>{{/hasPracticeData}}\
</ul></div><div class="map-info-tail" onclick="pointerClicked()"><i></i></div></div>';

