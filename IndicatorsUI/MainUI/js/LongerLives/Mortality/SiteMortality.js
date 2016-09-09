/*
* SiteMortality.js inherits from SiteBaseLongerLives.js
*/

function initSite() {
    searchPolygonAreaTypeId = MT.model.areaTypeId;
}

MT.model = {
    area: null,
    areaCode: null,
    parentCode: null,
    isHashRestored: false,

    //
    // Resets the model
    //
    _reset: function () {
        var m = MT.model;
        m.areaCode = null;
        m.parentCode = NATIONAL_CODE;
        comparatorId = NATIONAL_COMPARATOR_ID;
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

            // Group ID
            var prop = pairs['are'];
            if (prop) m.areaCode = prop;

            // Area type ID
            prop = pairs['ati'];
            if (prop) m.areaTypeId = parseInt(prop);

            // Parent area code
            prop = pairs['par'];
            if (prop) {
                m.parentCode = prop;
                // Comparator ID is dependent on parent
                if (prop !== NATIONAL_CODE) {
                    comparatorId = DEPRIVATION_DECILE_COMPARATOR_ID;
                }
            }

            // Parent area type ID
            prop = pairs['pat'];
            if (prop) m.parentAreaType = parseInt(prop);
        }
    },

    //
    // Generates a string representation of the model to be used as the URL hash
    //
    toString: function () {
        var m = this;
        return [
            'are', m.areaCode,
            'par', m.parentCode,
            'ati', m.areaTypeId,
            'pat', m.parentAreaType
        ].join('/');
    }

};

function getGroupRoots(model) {
    if (!groupRoots) {
        var parameters = new ParameterBuilder(
            ).add('group_id', model.groupId
            ).add('area_type_id', model.areaTypeId);
        ajaxGet('api/profile_group_roots', parameters.build(), getGroupRootsCallback);

    } else {
        // Data already loaded
        ajaxMonitor.callCompleted();
    }
}

function getGroupRootsCallback(obj) {
    groupRoots = obj;
    ajaxMonitor.callCompleted();
}

/*
* Defines the indexes for specific indicator in the list of group roots
*/
ROOT_INDEXES = {
    POPULATION: 0,
    OVERALL_MORTALITY: 1,
    OVERALL_CANCER: 2,
    OVERALL_LUNG_CANCER: 3,
    OVERALL_BREAST_CANCER: 4,
    OVERALL_COLORECTAL_CANCER: 5,
    OVERALL_CARDIOVASCULAR: 6,
    OVERALL_HEART_DISEASE: 7,
    OVERALL_STROKE: 8,
    OVERALL_LUNG: 9,
    OVERALL_LIVER: 10,
    OVERALL_INJURY: 11,
    DEPRIVATION: 12
};

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
        ).add('parent_area_code', parentCode
        ).add('area_type_id', areaTypeId);

        ajaxGet('api/areas/by_parent_area_code', parameters.build(), getChildAreasCallback);
    }
}

MT.nav = {

    //
    // Go to the mortality rankings page
    //
    rankings: function (model) {
        if (!isDefined(model)) {
            model = MT.model;
        }
        if (model.profileId === ProfileIds.Mortality) {
            setUrl('/topic/mortality/comparisons#' + model.toString());
        } else {
            model.parentCode = NATIONAL_CODE;
            model.areaCode = null;
            model.areaTypeId = model.parentAreaType;

            setUrl('/topic/' + profileUrlKey + '/comparisons#' + model.toString());
        }
    },

    //
    // Go to the area details page
    //
    areaDetails: function (model) {
        if (!isDefined(model)) {
            model = MT.model;
        }

        var initialUrl = window.location.href,
        path = '/topic/mortality/area-details';
        setUrl(path + '#' + model.toString());

        // Is user already on area details page?
        if (initialUrl.indexOf(path) > -1) {
            // Clear search box
            $('#search_text').val('').blur();

            initPage();
        }
    },

    //
    // Go to the home page
    //
    home: function (model) {

        if ($('.home_intro').length) {
            // Already on home page
            return;
        }

        if (!isDefined(model)) {
            model = MT.model;
        }
        setUrl('/topic/' + profileUrlKey + '/#' + model.toString());
    },

    gohome: function () {
        setUrl('/topic/' + profileUrlKey);
    }
}

function getDecileCode(decile) {
    return 'cat-2-' + decile;
}

function getGradeFunction(parentValue) {

    return function (sig, areaValue) {
        switch (sig) {
            case 1:
                return 3 // Red
            case 2:
                return areaValue > parentValue ?
                    2 : // Orange
                    1; // Yellow
            case 3:
                return 0; // Green
        }
        return '';
    };
}

function goToAreaDetails() {
    MT.nav.areaDetails();
}

function areaSearchResultSelected(noMatches, searchResult) {

    //TODO log search keyword to google analytics
    if (searchResult) {
        var selectedText;
        if (searchResult.County) {
            selectedText = searchResult.PlaceName + ', ' + searchResult.County;
        } else {
            selectedText = searchResult.PlaceName;
        }
        selectedText = searchResult.PlaceName;
    }

    if (!noMatches.is(':visible')) {

        var model = MT.model;

        var areaCode = searchResult.PolygonAreaCode;
        model.areaCode = areaCode;

        if (isSimilarAreas()) {
            var deciles = loaded.categories[AreaTypeIds.DeprivationDecile];
            model.parentCode = getDecileCode(deciles[areaCode]);
        }

        // Reset search box
        $('#search_text').val('').blur();

        goToAreaDetails();
    }
}

MT.model.parentCode = NATIONAL_CODE;

// National, Deprivation decile
DEPRIVATION_DECILE_COMPARATOR_ID = 1;
comparatorIds = [NATIONAL_COMPARATOR_ID, DEPRIVATION_DECILE_COMPARATOR_ID];
comparatorId = NATIONAL_COMPARATOR_ID;
groupRoots = null;
selectedRootIndex = ROOT_INDEXES.OVERALL_MORTALITY;
onsClusterCode = '';

causeOptions = [
    { key: 'cancer' },
    { key: 'lung_cancer' },
    { key: 'breast_cancer' },
    { key: 'colorectal_cancer' },
    { key: 'heart_disease_and_stroke' },
    { key: 'heart_disease' },
    { key: 'stroke' },
    { key: 'lung_disease' },
    { key: 'liver_disease' },
    { key: 'injuries' }
];

shouldSearchRetreiveCoordinates = false;
