/**
* SiteDiabetes namespace
* SiteDiabetes.js inherits from SiteBaseLongerLives.js
* @module SiteDiabetes
*/

function initSite() {
    searchPolygonAreaTypeId = MT.model.areaTypeId;
}

MT.model = {
    area: null,
    areaCode: null,
    parentCode: null,
    isHashRestored: false,
    areaTypeId: null,
    indicatorId: null,
    parentAreaType: null,

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
        }

        updatePage();
    },

    //
    // Generates a string representation of the model to be used as the URL hash
    //

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

        var areaCode = m.areaCode;
        if (areaCode) {
            parameters.push('are', m.areaCode);
        }

        return parameters.join('/');
    }

};

function setSearchText(areaTypeId) {

    var $searchBox = $('#search_text');

    switch (areaTypeId) {
        case AreaTypeIds.CCG:
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

function switchAreas(areaTypeId) {
    var model = MT.model;
    setUrl('/topic/' + profileUrlKey +
        '#par/' + model.parentCode +
        '/ati/' + areaTypeId +
        '/gid/' + model.groupId +
        '/pat/' + areaTypeId);
    // we only reload the page if we have a topic
    // otherwise we just set the url with clicked areatype
    var currentUrl = window.location.href;
    if (currentUrl.indexOf("topic") > -1) {
        window.location.reload();
    }
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

function getNhsId() {
    var areaCode = MT.model.areaCode;
    ajaxGet('api/area/nhs_choices_area_id', 'area_code=' + areaCode,
        function (obj) {
            loaded.nhsId = obj;
            ajaxMonitor.callCompleted();
        });
}

function getConditionWord() {

    switch (MT.model.profileId) {
        case ProfileIds.Diabetes:
            return 'diabetes';
        case ProfileIds.Hypertension:
            return 'recorded hypertension';
    }

    return '???';
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

MT.nav = {

    //
    // Go to the diabetes rankings page
    //
    rankings: function (model) {
        if (!model) {
            model = MT.model;
        }

        // Area details
        if (model.parentCode === NATIONAL_CODE) {
            model.parentCode = model.areaCode;
        }
        model.areaCode = null;
        model.areaTypeId = AreaTypeIds.Practice;

        //Force in the selected indicator Id
        if (groupRoots.length) {
            var root = groupRoots[selectedRootIndex];
            model.indicatorId = root.IID;
            model.sexId = root.Sex.Id;
        }
        setUrl('/topic/' + profileUrlKey + '/comparisons#' + model.toString());
    },

    nationalRankings: function (model) {
        if (!model) {
            model = MT.model;
        }

        model.parentCode = NATIONAL_CODE;
        model.areaCode = null;
        model.areaTypeId = model.parentAreaType;

        if (isDefined(groupRoots) && groupRoots.length) {
            var root = groupRoots[selectedRootIndex];
            model.indicatorId = root.IID;
            model.sexId = root.Sex.Id;
        }

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

    //
    // Go to the area details page
    //
    areaDetails: function (model) {
        if (!isDefined(model)) {
            model = MT.model;
        }
        setUrl('/topic/' + profileUrlKey + '/area-details#are/' + model.areaCode + '/par/' + model.parentCode);
    },


    //
    // Go to the home page
    //
    home: function () {
        // Check not already on home page
        if (!$('.home_intro').length) {
            setUrl('/topic/' + profileUrlKey);
        }
    },

    //
    // Go to home via bread crumbs
    // 
    gohome: function () {
        setUrl('/topic/' + profileUrlKey);
    }
}

function useQuintiles(comparatorMethodId) {
    return comparatorMethodId === 15;
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

function isComparisonAvailable(model) {

    return model.areaTypeId !== AreaTypeIds.Practice;
}

function getDeprivationLabel(label, parentAreaType) {

    var areaTypeLabel = parentAreaType === AreaTypeIds.CountyUA ? 'County/UA'
        : parentAreaType === AreaTypeIds.CCG ? 'CCG'
        : 'England';

    return label + ' within ' + areaTypeLabel;
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
loaded.areaDetailsForDiseaseAndDeath = new AreaDetailsDataManager();
loaded.contentText = '';
loaded.groupDataAtDataPoint = new GroupDataAtDataPointOfSpecificAreasDataManager(MT.model);

IndicatorIds = {
    Deprivation: 338,
    SuicidePlan : 92607
};

groupRoots = null;
selectedRootIndex = ROOT_INDEXES.POPULATION;
treatmentTargetSelected = null;
comparatorId = NATIONAL_COMPARATOR_ID;
shouldSearchRetreiveCoordinates = true;
NO_SUICIDE_PLAN = 'No information on current suicide plan status';

GroupIds = {
    HealthChecks: {
        HealthCheck: 1938132782,
        DiseaseAndDeath: 1938132785
    },
    Diabetes: {
        Complications: 1938132699,
        PrevalenceAndRisk: 1938132727
    },
    DrugsAndAlcohol: {
        PrevalenceAndRisks: 1938132771,
        TreatmentAndRecovery: 1938132772
    },
    Cancer: {
        IncidenceAndMortality: 1938132749
    },
    Suicide: {
        SuicideData: 1938132762,
        RelatedRiskFactors: 1938132763,
        RelatedServiceContacts: 1938133082
    },
    LAScorecard: {
        DrugTreatment: 1938133144,
        ChildObesity: 1938133145
    }
}

