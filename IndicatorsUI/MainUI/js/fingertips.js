/**
* fingertips.js
* Code that is shared between Longer Lives and Fingertips.
*
* @module fingertips
*/

FT = { menus: {} };

/**
* Global hash containing data that has been fetched by AJAX.
* @class loaded
* @static
*/
loaded = {
    areaLists: {},
    areaTypes: {},
    indicatorMetadata: {},
    yearTypes: {},
    methods: {},
    ciMethods: {},
    ages: {},
    areaValues: {},
    areaMappings: {},
    parentAreaGroups: {},
    parentAreas: {},
    boundaries: {}, // Boundary coordinates for maps: Key -> areaTypeId, Value -> [] of area coordinates
    valueNotes: {},
    groupDataAtDataPoint: {}
};

/**
* Enum of the tabs that are available in Fingertips.
* @class PAGE_MODES
*/
PAGE_MODES = {
    TARTAN: 0,
    AREA_SPINE: 1,
    INDICATOR_DETAILS: 3,
    AREA_TRENDS: 4,
    INEQUALITIES_80_20: 5,
    METADATA: 6,
    CONTENT: 7,
    MAP: 8,
    DOWNLOAD: 9,
    SCATTER_PLOT: 10
};

/**
* Sort function for sorting areas alphabetically by name.
* @class sortAreasByName
*/
function sortAreasByName(a, b) {
    return sortString(a.Name, b.Name);
};

/**
* Sort function for sorting areas alphabetically by short name.
* @class sortAreasByShort
*/
function sortAreasByShort(a, b) {
    return sortString(a.Short, b.Short);
};

/**
* Sort function for sorting areas alphabetically by name. The order may be A-Z or Z-A depending on the order parameter.
* @class sortAreasAToZ
* @param {Object} order Determines whether the areas are sorted A-Z or Z-A
* @param {Object} areas The array of areas to be sorted
*/
function sortAreasAToZ(order, areas) {
    areas.sort(sortAreasByName);
    if (order > 0) {
        areas.reverse();
    }
};

/**
* Sort function for sorting strings.
* @class sortString
*/
function sortString(a, b) {
    var nameA = a.toLowerCase(),
        nameB = b.toLowerCase();
    if (nameA < nameB) return -1;
    if (nameA > nameB) return 1;
    return 0;
}

/**
* Sort function for sorting data object by their 'Val' property.
* @class sortData
*/
function sortData(a, b) {
    return a.Val - b.Val;
};

function populateAreaMenu(areas, $menu, firstOption) {

    // Check the menu exists for the current profile
    if ($menu.length) {
        var options = $menu[0].options;
        options.length = 0;

        var j = 0;
        if (isDefined(firstOption)) {
            options[j++] = new Option(firstOption, '-');
        }

        for (var i in areas) {
            var area = areas[i];
            if (area.AreaTypeId == AreaTypeIds.Practice) {
                options[j++] = new Option(area.Code + ' - ' + area.Name, area.Code);
            } else {
                options[j++] = new Option(area.Name, area.Code);
            }
            
        }
    }
};

function AreaTypes(areaTypesArray, areaTypesHash) {

    if (areaTypesHash) {
        // Initialise
        _.each(areaTypesArray, function (areaType) {
            areaTypesHash[areaType.Id] = areaType;
        });
    } else {
        // Already initialised
        areaTypesHash = loaded.areaTypes;
    }

    this.getAllIds = function () {
        return _.map(_(areaTypesHash).keys(), function (strAreaTypeId) {
            return parseInt(strAreaTypeId);
        });
    };

    this.getAreaTypes = function () {
        if (areaTypesArray) {
            return areaTypesArray;
        }

        return _(areaTypesHash).toArray();
    };
}

function addTd(html, text, cssClass, tooltip) {

    html.push('<td');

    if (cssClass) {
        html.push(' class="', cssClass, '"');
    }

    if (tooltip) {
        html.push(' title="', tooltip, '"');
    }

    html.push('>', text, '</td>');
}

function addTh(html, text, cssClass, tooltip) {

    html.push('<th');

    if (cssClass) {
        html.push(' class="', cssClass, '"');
    }
    if (tooltip) {
        html.push(' title="', tooltip, '"');
    }

    html.push('>', text, '</th>');
};

function setPageSelected(selectionBoxId, selectedId) {
    var p = 'pageSelected';
    $(selectionBoxId).find('.pageSelector').removeClass(p);
    $(selectedId).addClass(p);
};

/**
* Show the Fingertips spinner and hides the main panel.
* @class showSpinner
*/
function showSpinner() {
    if (!isSpinnerDisplayed) {
        $(MAIN).hide();
        $('#spinner').show();
        isSpinnerDisplayed = true;
    }
};

/**
* Hides the Fingertips spinner and shows the main panel.
* @class hideSpinner
*/
function hideSpinner() {
    if (isSpinnerDisplayed) {
        $('#spinner').hide();
        isSpinnerDisplayed = false;
    }
};

/**
* Trims a string to the specified character limit and appends "...".
* @class trimName
*/
function trimName(name, limit) {

    if (name.length > limit) {

        name = $.trim(name.substr(0, limit)) + '...';
    }
    return name;
};

/**
* Returns true if a data value is valid.
* @class isValidValue
*/
function isValidValue(val) {
    return isDefined(val) && val !== -1;
};

function getKey() {
    return _.toArray(arguments).join('-');
};

function showDataQualityLegend() {
    var $dataQuality = $('#data-quality-key');
    if (showDataQuality) {
        $dataQuality.show();
    } else {
        $dataQuality.hide();
    }
}

/*
* Displays or hides the target benchmark checkbox
*/
function showTargetBenchmarkOption(roots) {

    var areAnyTargets = false,
        metadataHash = ui.getMetadataHash(),
        i;

    for (i in roots) {
        var indicatorId = roots[i].IID;
        if (metadataHash[indicatorId].Target) {
            areAnyTargets = true;
            break;
        }
    }
    var $targetBenchmarkBox = $('#target-benchmark-box');
    if (areAnyTargets) {
        $targetBenchmarkBox.show();
    } else {
        $targetBenchmarkBox.hide();
    }
}

/*
* Get AreaName from selected area Object
*/
function getAreaNameToDisplay(area) {
    if (area.AreaTypeId == AreaTypeIds.Practice) {
        return area.Code + ' - ' + area.Name;
    } else {
        return area.Name;
    }
}

function getShortAreaNameToDisplay(area) {
    if (area.AreaTypeId == AreaTypeIds.Practice) {
        return area.Code + ' - ' + area.Short;
    } else {
        return area.Short;
    }
}


/**
* Controls which tab is displayed in Fingertips and provides access to the main panel
* for each page for setting the HTML.
* @class pages
* @static
*/
pages = (function () {

    // Global (for now)
    mode = null;

    var _pages = [],
    _pageProperties = [],
    ordered = [],
    tabsSelector = '#tabs',
    jqIdsNotInitiallyShown = 'jqIdsNotInitiallyShown',
    elementManager = new ElementManager(),
    tabTemplate = '{{#pages}}<div id="page-{{id}}" onclick="executeWithLock({{gotoName}});" class="pageSelector page">\
<div class="pageIcon page-{{#icon}}{{icon}}{{/icon}}{{^icon}}{{id}}{{/icon}}">&nbsp;</div>\
<span>{{{title}}}</span></div>{{/pages}}',
    onResize = function () {
        // Enables a page to resize if required
        var resizeFunction = _pages[mode].resize;
        if (resizeFunction) {
            resizeFunction();
        }
    },
    getJqs = function () {
        var page = _pages[mode];
        if (!page['jqs']) {

            page.jqs = elementManager.add(page.jqIds);

            // Add elements that are not initially shown
            var jqs = page[jqIdsNotInitiallyShown];
            if (jqs) {
                elementManager.addNotShown(jqs);
            }
        }
        return page.jqs;
    };

    return {

        /** 
        * Adds a page.
        * @method add
        * @param {String} pageNumber unique ID of the page
        * @param {String} goToFunction function called to change view to that page
        * @param {String} pageId suffix that forms the ID of the tab element
        * @param {Object} properties global UI configuration e.g. showAllAvailable - whether or not show all checkbox is visible
        */
        add: function (pageNumber, goToFunction, pageId, properties) {

            // New way
            var obj = goToFunction;
            obj.number = pageNumber;

            // Pages should be display in the same order as they are added
            ordered.push(obj);
            _pages[pageNumber] = obj;

            _pageProperties[pageNumber] = isDefined(properties) ?
                properties :
                    {};
        },

        /**
        * Returns true if JQuery selector is not shown initially but may be shown in certain contexts.
        * @method isInJqIdsNotInitiallyShown
        * @param {String} id Element ID
        */
        isInJqIdsNotInitiallyShown: function (id) {
            var jqs = _pages[mode][jqIdsNotInitiallyShown];
            return jqs
                ? _.indexOf(jqs, id) > -1
                : false;
        },

        /**
        * Whether or not a page property is true.
        * @method isTrue
        * @param {String} propertyName Property name
        */
        isTrue: function (propertyName) {
            var properties = _pageProperties[mode];
            return isDefined(properties[propertyName]) ?
                properties[propertyName] === true :
                false;
        },

        /**
        * Goes to the specified page.
        * @method goTo
        * @param {Integer} pageNumber Page number
        */
        goTo: function (pageNumber) {
            // Select the first page if none is requested
            var page = pageNumber ?
                _pages[pageNumber] :
                ordered[0];
            page.goto(null);
        },

        /**
        * Get the default page number.
        * @method getDefault
        */
        getDefault: function () {
            return mode ? _pages[mode].number :
                ordered[0].number;
        },

        /**
        * Display the elements on the current page.
        * @method displayElements
        */
        displayElements: function () {

            elementManager.displayElements(getJqs());

            // Call page specific display modifier
            var s = _pages[mode].showHide;
            if (s) {
                s();
            }
        },

        /**
        * Display the current page.
        * @method goToCurrent
        */
        goToCurrent: function () {
            this.goTo(mode);
        },

        /**
        * Set the current page.
        * @method setCurrent
        * @param {Integer} pageNumber Page number
        */
        setCurrent: function (pageNumber) {
            mode = pageNumber;

            setPageSelected(tabsSelector,
                '#page-' + _pages[mode].id);
        },

        /**
        * Initialise the tabs and page objects
        * @method init
        */
        init: function () {

            // Create tabs
            var name = 'tabs';
            templates.add(name, tabTemplate);
            $(tabsSelector).append(templates.render(name, { pages: ordered }));

            var main = $('#main');
            _.each(ordered, function (page) {

                // Create container
                if (page.needsContainer) {
                    var containerId = page.id + '-container';
                    page.containerId = containerId;
                    page.jqIds.push(containerId);

                    main.prepend('<div id="' + containerId +
                            '" class="standardWidth clearfix" style="display:none;"></div>');
                } else {
                    // Use ID already specified
                    page.containerId = page.id;
                }
            });

            // Hide all keys by default to deal with ad hoc display (e.g. on maps)
            allJQs.push([$('.keyContainer')]);

            // Set default page
            this.setCurrent(this.getDefault());

            // Resizing
            var resizeTimer;
            $(window).resize(function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(onResize, 100);
            });
        },

        /**
        * Get container JQuery element for the current page
        * @method getContainerJq
        */
        getContainerJq: function () {
            return $('#' + _pages[mode].containerId);
        },

        /**
        * Get the current page ID
        * @method getCurrent
        */
        getCurrent: function () {
            return mode;
        },

        /**
        * Get the current page object
        * @method getCurrentPage
        */
        getCurrentPage: function () {
            return _pages[mode];
        }
    };
})();

/**
* AJAX call to fetch indicator metadata for a domain.
* @class getIndicatorMetadata
*/
function getIndicatorMetadata(groupId, getSystemContent) {

    // Establish whether metadata is already loaded
    var metadata = loaded.indicatorMetadata;
    if (isDefined(metadata[groupId])) {
        ajaxMonitor.callCompleted();
    } else {
        if (!isDefined(getSystemContent)) {
            getSystemContent = 'no';
        }

        getData(function (obj) {
            loaded.indicatorMetadata[groupId] = obj;
            ajaxMonitor.callCompleted();
        }, 'im',
        'def=yes' + '&include_system_content=' + getSystemContent +
                getIndicatorMetadataArgument(groupId));
    }
};

/**
* Generic AJAX call to the GetData.ashx service.
* @class getData
*/
function getData(callback, service, args) {

    args = isDefined(args) && !String.isNullOrEmpty(args) ?
        '&' + args :
        '';

    ajaxGet('GetData.ashx', 's=' + service + args, callback);
};

/**
* Gets the argument for retrieving the appropriate indicator metadata. 
* In this context it is for all the indicators in a domain.
* @class getIndicatorMetadataArgument
*/
function getIndicatorMetadataArgument(groupId) {
    return '&gid=' + groupId;
}

/**
* Gets an element ID from the argument of a JQuery event.
* @class getElementIdFromJQueryEvent
*/
function getElementIdFromJQueryEvent(e) {

    // Target may be child of element that event is registered to
    var target = e.target,
        targetId = target.id;
    while (targetId === '') {
        target = $(target).parent()[0];
        targetId = target.id;
    }
    return targetId;
}

/**
* Defines the state of the Fingertips data page.
* @class FT.model
* @static
*/
FT.model = {
    areaTypeId: null,
    groupId: null,
    parentCode: null,
    profileId: null,
    parentTypeId: null,
    areaCode: null,
    iid: null,
    ageId: null,
    sexId: null,
    nearestNeighbour: null,

    /**
    * Resets the model.
    * @method _reset
    */
    _reset: function () {
        var m = FT.model;
        m.groupId = groupIds[0];
        m.areaTypeId = defaultAreaType;
        // Cannot define any more parameters until after web services calls have been made
    },

    /**
    * Restores the UI from the hash state
    * @method restore
    */
    restore: function () {
        this.update();

        // Must set up area options before page can be refreshed
        initAreaData2();
        updateDomains();
    },

    /**
    * Updates the model from the current hash state
    * @method update
    */
    update: function () {
        this._reset();
        var m = this;
        var pairs = ftHistory.getKeyValuePairsFromHash();

        if (_.size(pairs)) {

            // Group ID
            var prop = pairs['gid'];
            if (prop) m.groupId = parseInt(prop);

            // Parent area code
            prop = pairs['par'];
            if (prop) m.parentCode = prop;

            // Area type ID
            prop = pairs['ati'];
            if (prop) m.areaTypeId = parseInt(prop);

            // Parent area type ID
            prop = pairs['pat'];
            if (prop) m.parentTypeId = parseInt(prop);

            // Area code
            prop = pairs['are'];
            if (prop) m.areaCode = prop;

            // Indicator Id
            prop = pairs['iid'];
            if (prop) m.iid = parseInt(prop);

            // Age Id
            prop = pairs['age'];
            if (prop) m.ageId = parseInt(prop);

            // Sex Id
            prop = pairs['sex'];
            if (prop) m.sexId = parseInt(prop);

            // Nearest neighbours
            prop = pairs['nn'];
            m.nearestNeighbour = !!prop ? prop : null;

            // Page 
            prop = pairs['page'];
            var pageId = prop ?
                parseInt(prop) :
                pages.getDefault();
            pages.setCurrent(pageId);
        }
    },

    /**
    * Adds a model property to the parameters hash if it is defined
    * @method _addPropertyToParametersIfDefined
    */
    _addPropertyToParametersIfDefined: function (modelParameter, urlParameter, parameters) {
        if (this[modelParameter] && !String.isNullOrEmpty(this[modelParameter])) {
            parameters.push(urlParameter, this[modelParameter]);
        }
    },

    /**
    * Generates a string representation of the model to be used as the URL hash
    * @method toString
    */
    toString: function () {
        var m = this;

        // These parameters always included in serialisation
        var parameters = [
            'page', pages.getCurrent()
        ];

        // IMPORTANT: Only add parameters if they are defined (some browsers will not parse hash otherwise)
        m._addPropertyToParametersIfDefined('groupId', 'gid', parameters);
        m._addPropertyToParametersIfDefined('parentTypeId', 'pat', parameters);
        m._addPropertyToParametersIfDefined('parentCode', 'par', parameters);
        m._addPropertyToParametersIfDefined('areaTypeId', 'ati', parameters);
        m._addPropertyToParametersIfDefined('areaCode', 'are', parameters);
        m._addPropertyToParametersIfDefined('iid', 'iid', parameters);
        m._addPropertyToParametersIfDefined('ageId', 'age', parameters);
        m._addPropertyToParametersIfDefined('sexId', 'sex', parameters);
        m._addPropertyToParametersIfDefined('nearestNeighbour', 'nn', parameters);

        return parameters.join('/');
    }
};

function getAreaValues(root, model, code) {

    var parentCode = !!code ?
        code :
        getCurrentComparator().Code,
    key = getIndicatorKey(root, model, parentCode);

    ajaxMonitor.state.indicatorKey = key;

    var areaValues = loaded.areaValues;
    if (areaValues.hasOwnProperty(key)) {
        // Data already loaded
        getAreaValuesCallback(areaValues[key]);
    } else {
        var parameters = 'par=' + parentCode +
            '&gid=' + model.groupId +
            '&off=0&iid=' + root.IID +
            '&sex=' + root.SexId +
            '&age=' + root.AgeId +
            '&ati=' + model.areaTypeId +
            '&com=' + comparatorId +
            getRestrictByProfileParameter();

        getData(getAreaValuesCallback, 'av', parameters);
    }
}

function getIndicatorKey(root, model, comparatorCode) {
    return getKey(
        model.groupId,
        root.IID,
        root.SexId,
        root.AgeId,
        model.areaTypeId,
        comparatorCode
    );
}

/**
* Creates a key/value pair list of area code to coredataset
* where: Key -> area code, Value -> coredataset
* @class getAreaCodeToCoreDataHash
*/
function getAreaCodeToCoreDataHash(dataList) {

    var hash = {};
    _.each(dataList, function (data) {
        hash[data.AreaCode] = data;
    });
    return hash;
}

/**
* AJAX call to fetch the value notes.
* @class getValueNotesCall
*/
function getValueNotesCall() {
    ajaxGet('data/value_notes', '', getValueNotesCallback);
}

/**
* AJAX call to fetch the area types for a profile.
* @class getAreaTypesCall
*/
function getAreaTypesCall(profileId) {
    var parameters = new ParameterBuilder().add('profile_ids', getProfileIds(profileId));

    ajaxGet('data/area_types',
        parameters.build(),
        getAreaTypesCallback);
}

/**
* Returns an array of all the relevant profile ids.
* @class getProfileIds
*/
function getProfileIds(profileId) {

    if (profileCollectionIdList.length) {
        return profileCollectionIdList;
    }

    return [restrictSearchProfileId
        ? restrictSearchProfileId
        : profileId];
}

//
// note: Use getProfileIds instead
//
function getRestrictByProfileParameter() {

    var parameter = '&res=';

    if (profileCollectionIdList.length) {
        return parameter + profileCollectionIdList;
    }

    return isDefined(restrictSearchProfileId) ?
        parameter + restrictSearchProfileId :
        '';
}

function SexAndAge() { }

SexAndAge.prototype = {

    getSexLabel: function (sexId) {
        return sexId === 1 ?
            'Male' : sexId === 2 ?
            'Female' :
            'Persons';
    },

    getLabel: function (groupRoot) {

        var label = [];
        if (groupRoot.StateSex || groupRoot.AgeLabel) {

            var areBothLabelsRequired = groupRoot.StateSex && groupRoot.AgeLabel;

            label.push(' (');

            if (groupRoot.StateSex) {
                label.push(this.getSexLabel(groupRoot.SexId));
            }

            if (areBothLabelsRequired) {
                label.push(', ');
            }

            if (groupRoot.AgeLabel) {
                label.push(groupRoot.AgeLabel);
            }

            label.push(')');
        }

        return label.join('');
    }
}

function MutuallyExclusiveDisplay(jquerySelectors) {

    this.A = jquerySelectors.a;
    this.B = jquerySelectors.b;
}

MutuallyExclusiveDisplay.prototype = {

    showA: function (showAHideB) {

        var a = this.A,
        b = this.B;

        (showAHideB ? a : b).show();
        (showAHideB ? b : a).hide();
    },

    showB: function () {
        this.showA(false);
    }

}

function toggleQuintileLegend(element, useQuintileColouring) {
    if (!useQuintileColouring) {
        element.hide();
    } else {
        element.show();
    }
}

/**
* Compiles a label based on the unit to be shown after a data value.
* @class ValueSuffix
*/
function ValueSuffix(unit) {

    this.unit = unit;
}

ValueSuffix.prototype = {

    /**
    * Get HTML unit label.
    * @method _getUnitHtml
    */
    _getUnitHtml: function () {
        return '<span class="unit">' + this.unit.Label + '</span>';
    },

    /**
    * Returns true if the unit has a short suffix, e.g. "%".
    * @method _isShortSuffix
    */
    _isShortSuffix: function () {
        return !!this.unit && this.unit.Label === '%';
    },

    /**
    * Gets a short unit label if appropriate.
    * @method getShortLabel
    */
    getShortLabel: function () {
        return this._isShortSuffix() ?
            this._getUnitHtml() :
            '';
    },

    /**
     * Gets the full unit label if no defined short label.
     * @method getFullLabelIfNoShort
     */
    getFullLabelIfNoShort: function () {

        return this._isShortSuffix() ?
            '' :
            this.getFullLabel();
    },

    /**
     * Gets the full unit label.
     * @method getFullLabel
     */
    getFullLabel: function () {

        var _this = this,
        unit = _this.unit;
        if (!unit || unit.ShowLeft) {
            return '';
        }

        return _this._isShortSuffix() ?
            _this.getShortLabel() :
            ' ' + _this._getUnitHtml();
    }
}

/**
* Determines the prefix of a value according to its unit.
* @class ValuePrefix
*/
function ValuePrefix(unit) {

    /**
    * Gets a label to be shown before a value.
    * @method getLabel
    */
    this.getLabel = function () {
        return unit && unit.ShowLeft ?
            '<span class="unit">' + unit.Label + '</span>' :
            '';
    };
}

/**
* Generates a formatted number string with the unit positioned appropriately. 
* If the unit is undefined then a string is returned without a unit.
* @class ValueWithUnit
*/
function ValueWithUnit(unit) {

    this.prefix = new ValuePrefix(unit).getLabel();
    this.suffix = new ValueSuffix(unit);
}

ValueWithUnit.prototype = {

    /**
    * Returns a string of the number and it's prefix
    * @method _getNumber
    */
    _getNumber: function (value, options) {

        var numberToDisplay = options && options.noCommas ?
            value :
            new CommaNumber(value).unrounded();

        return this.prefix + numberToDisplay;
    },

    /**
    * Returns a string of the number and it's full unit label.
    * @method getFullLabel
    */
    getFullLabel: function (value, options) {
        return this._getNumber(value, options) +
            this.suffix.getFullLabel();
    },

    /**
    * Returns a string of the number and it's short unit label.
    * @method getShortLabel
    */
    getShortLabel: function (value, options) {
        return this._getNumber(value, options) +
            this.suffix.getShortLabel();
    }
}

/**
* Manages which elements are displayed on a page.
* @class ElementManager
*/
function ElementManager() {

    var _this = this,
        allSelectors = [],
        $allElements = [];

    // Converts a list of JQuery selectors to JQuery elements
    var selectorListToJqs = function (selectorArray) {
        return _.map(selectorArray, function (selector) {

            if (selector.charAt(0) !== '.') {
                selector = '#' + selector;
            }

            var $element = $(selector);

            // Add to array of all if required
            if (_.indexOf(allSelectors, selector) < 0) {
                $allElements.push($element);
                allSelectors.push(selector);
            }

            return $element;
        });
    };

    /**
    * Adds the elements that should be displayed on a page.
    * @method add
    */
    _this.add = function (arrayOfJQuerySelectors) {
        return selectorListToJqs(arrayOfJQuerySelectors);
    };

    /**
    * Adds the elements that do not need to be displayed on a page.
    * @method addNotShown
    */
    _this.addNotShown = function (arrayOfJQuerySelectors) {
        selectorListToJqs(arrayOfJQuerySelectors);
    };

    /**
    * Gets all the elements known to the ElementManager.
    * @method getAll
    */
    _this.getAll = function () {
        return $allElements;
    };

    /**
    * Shows the specified elements and hides all the others.
    * @method displayElements
    */
    _this.displayElements = function ($elementsToShow) {

        for (var i in $allElements) {
            var $element = $allElements[i];
            if (_.indexOf($elementsToShow, $element)) {
                $element.hide();
            }
        }

        _.each($elementsToShow, function ($e) { $e.show(); });
    };
}

/**
* Get HTML image and label for a particular data quality value.
* @class getIndicatorDataQualityHtml
*/
function getIndicatorDataQualityHtml(dataQualityCount) {

    if (showDataQuality && dataQualityCount) {
        var count = parseInt(dataQualityCount);
        if (count > 0) {

            var colour =
                count === 1 ? 'red' :
                count === 2 ? 'orange' :
                 'green';

            return ['&nbsp;<span class="indicator-data-quality-container" title="',
                getIndicatorDataQualityTooltipText(count),
                '"><img src="', FT.url.img,
                'markers/square-', colour, '.png" /></span>'].join('');
        }
    }
    return '';
}


/**
* Get text describing a specific data quality value.
* @class getIndicatorDataQualityTooltipText
*/
function getIndicatorDataQualityTooltipText(dataQualityCount) {
    switch (dataQualityCount) {
        case 1:
            return 'There are significant concerns regarding the quality of this data';
        case 2:
            return 'There are some concerns regarding the quality of this data';
        case 3:
            return 'The quality of this data is robust';
        default:
            return ' ';
    }
}

/**
* Returns an address string.
* @class getAddressText
*/
function getAddressText(address) {

    var a = [],
        keys = ['A1', 'A2', 'A3', 'A4', 'Postcode'],
        add, i;

    for (i in keys) {
        add = address[keys[i]];
        if (isDefined(add)) {
            a.push(add);
        }
    }

    return a.join(', ');
}

function getColourFromSignificance(significance, useRag, colours, useQuintileColouring) {
    if (useQuintileColouring) {
        switch (true) {
            case (significance > 0 && significance < 6):
                var quintile = 'quintile' + significance;
                return colours[quintile];
        }
    } else {
        if (useRag) {
            switch (significance) {
                case 1:
                    return colours.worse;
                case 2:
                    return colours.same;
                case 3:
                    return colours.better;
            }
        } else {
            switch (significance) {
                case 1:
                    return colours.bobLower;
                case 2:
                    return colours.same;
                case 3:
                    return colours.bobHigher;
            }
        }
    }

    return colours.none;
}

function initAreaSearch(jquerySelector, model, excludeCCGs) {

    var $searchBox = $(jquerySelector),
        $noMatches = $('#no-matches-found'),
        cssSearchWatermark = 'search-watermark',
        title = 'title';

    // Initialize auto-hint fields 
    $searchBox.focus(function () {
        $searchBox.val('').removeClass(cssSearchWatermark);
    }).blur(function () {
        if ($searchBox.val() === '' && $searchBox.attr(title) !== '') {
            // Display watermark
            $searchBox.val($searchBox.attr(title)).addClass(cssSearchWatermark);
        }
        $noMatches.hide();
    }).keydown(function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if ($noMatches.is(':visible') && code == 13) {
            e.preventDefault();
        }
    }).keyup(function () {
        // Hide no matches if search box empty
        if (!$searchBox.val().length) {
            $noMatches.hide();
        }
    });

    // No results match
    var showSearchNotFound = function() {
        var position = $searchBox.position();
        $noMatches.width($searchBox.width()).css(
            {
                top: position.top + SEARCH_NO_RESULT_TOP_OFFSET,
                left: position.left + SEARCH_NO_RESULT_LEFT_OFFSET
            }).show();

        // Hide suggestions
        $('#ui-id-1').hide();
    }

    $searchBox.autocomplete({
        source: function (request, response) {
            ajaxAreaSearch($searchBox.val(),
                function (data) {
                    if (data.length > 0) {
                        $noMatches.hide();
                        response(new AreaSearchResults(data).suggestions);
                    } else {
                        showSearchNotFound();
                    }
                },
                excludeCCGs
            );
        },
        delay: 0,
        autoFocus: true,
        minLength: 3,
        select: function (event, ui) {
            event.preventDefault();
            areaSearchResultSelected($noMatches, ui.item.result);

            logEvent('Search', 'AreaSearch', ui.item.result.PlaceName);
        },
        open: function () {
            $searchBox.removeClass('ui-corner-all').addClass('ui-corner-top');
        },
        close: function () {
            $searchBox.removeClass('ui-corner-top').addClass('ui-corner-all');
        }
    });
}

/**
* AJAX call to fetch results of an area search
* @class ajaxAreaSearch
*/
function ajaxAreaSearch(text, successFunction, excludeCcgs) {
    getAreaSearchResults(text, successFunction, AreaTypeIds.CCG, true, excludeCcgs);
}

/**
* AJAX call to fetch results of an area search
* @class getAreaSearchResults
*/
function getAreaSearchResults(text, successFunction, areaTypeId, shouldSearchRetreiveCoordinates, excludeCcgs) {

    $.ajax({
        type: 'GET',
        url: FT.url.corews + 'AreaLookup.ashx?s=aa&text=' + text +
            '&polygon_area_type_id=' + areaTypeId +
            '&include_coordinates=' + shouldSearchRetreiveCoordinates +
            '&exclude_ccgs=' + excludeCcgs,
        data: {},
        cache: false,
        contentType: 'application/json',
        dataType: 'jsonp',
        success: successFunction,
        error: function () { }
    });
}

/**
* Results of an area search
* @class AreaSearchResults
*/
function AreaSearchResults(data) {
    var labels = [], parentAreaName, label, placeName;

    var caseInsensitiveAreEqual = function (s1, s2) {
        return s1.toLowerCase() === s2.toLowerCase();
    }

    /**
    * List of suggested area names.
    * @property suggestions
    */
    this.suggestions = $.map(data, function (searchResult) {

        placeName = searchResult.PlaceName;
        parentAreaName = searchResult.PolygonAreaName;

        // Do not want to repeated name
        label = parentAreaName === '' || caseInsensitiveAreEqual(placeName, parentAreaName) ?
            placeName :
            placeName + ', ' + parentAreaName;

        // Will this option already be displayed?
        if (!_.any(labels,
            function (option) {
                return caseInsensitiveAreEqual(option, label);
        })) {

            // Store that option will be displayed
            labels.push(label);

            // This result will be suggested to the user
            return {
                label: label,
                areaName: parentAreaName,
                result: searchResult
            };
        }
        return null;
    });
}

/**
* Fits a Google Map to specified LatLngBounds.
* @class fitMapToPracticeResults
*/
function fitMapToPracticeResults(map, bounds) {

    map.fitBounds(bounds);

    var googleMapsEvent = google.maps.event;

    // Add bounds changed listener
    var zoomChangeBoundsListener =
        googleMapsEvent.addListenerOnce(map, 'bounds_changed', function () {

            var maximumZoom = 13;

            // Zoom out if to close
            if (map.getZoom() > maximumZoom) {
                map.setZoom(maximumZoom);
            }
            googleMapsEvent.removeListener(zoomChangeBoundsListener);
        });
}

/**
* Gets a basic Google Map object with controls displayed as appropriate.
* @class getGoogleMap
*/
function getGoogleMap() {

    var gm = google.maps;
    return new gm.Map($('#map-canvas')[0], {
        zoom: 6,
        mapTypeId: gm.MapTypeId.ROADMAP,
        panControl: false,
        zoomControl: true,
        scaleControl: false,
        streetViewControl: false,
        mapTypeControl: false
    });
}

/**
* Predefined configuration objects for HighCharts
* @class HC
*/
HC = {
    credits: { enabled: false },
    noLineMarker: { enabled: false, symbol: 'x' }
}

/**
* Provides information about a CoreDataSet object
* @class CoreDataSetInfo
*/
function CoreDataSetInfo(coreDataSet) {
    this.data = coreDataSet;
}

CoreDataSetInfo.prototype = {

    /**
    * Is the CoreDataSet object defined
    * @method isDefined
    */
    isDefined: function () {
        return !!this.data;
    },

    /**
    * Is there are value note associated with the CoreDataSet object
    * @method isNote
    */
    isNote: function () {
        var data = this.data;
        return data ?
            !!data.NoteId :
            false;
    },

    /**
    * Gets the ID of value note
    * @method getNoteId
    */
    getNoteId: function () {
        return this.isNote() ?
            this.data.NoteId :
            null;
    },

    /**
    * Is the value defined
    * @method isValue
    */
    isValue: function () {
        var data = this.data;
        return data ?
            data.ValF !== '-' :
            false;
    },

    /**
    * Gets the formatted value
    * @method getValF
    */
    getValF: function () {
        return this.data.ValF;
    },

    /**
    * Is the count defined
    * @method isCount
    */
    isCount: function () {
        var data = this.data;
        if (!data) return false;
        return data.Count !== null && data.Count !== -1;
    },

    /**
    * Are confidence intervals defined
    * @method areCIs
    */
    areCIs: function () {
        var data = this.data;
        if (!this.isValue()) return false;
        return data.hasOwnProperty('LoCI') && data.LoCI !== -1;
    },

    /**
    * Are the value and both the confidence intervals equal to zero
    * @method areValueAndCIsZero
    */
    areValueAndCIsZero: function () {

        if (!this.areCIs()) return false;

        var rep = function (valF) {

            return parseInt(valF.replace('.', '').replace('-', ''));
        }

        var data = this.data;
        var up = rep(data.UpCIF);
        var lo = rep(data.LoCIF);
        var val = rep(data.ValF);

        return up === 0 && lo === 0 && val === 0;
    }
};

/**
* Returns a rounded and commarised count from a CoreDataSetInfo object.
* @class formatCount
*/
function formatCount(dataInfo) {
    return dataInfo.isCount() ?
        new CommaNumber(dataInfo.data.Count).rounded() : // Rounded for DSR with decimal denominators
        NO_DATA;
}

/**
* Get the HTML of a trend marker image.
* @class getTrendMarkerImage
*/
function getTrendMarkerImage(trendMarker, polarity) {

    var lowIsGood = PolarityIds.RAGLowIsGood;
    var highIsGood = PolarityIds.RAGHighIsGood;
    var blueOrangeBlue = PolarityIds.BlueOrangeBlue;

    var imageName;
    switch (trendMarker) {
        case TrendMarkerValue.Up:
            imageName = 'up_' +
                (polarity === blueOrangeBlue ? 'blue' :
                polarity === lowIsGood ? 'red' :
                polarity === highIsGood ? 'green' :
                'grey');
            break;
        case TrendMarkerValue.Down:
            imageName = 'down_' +
                (polarity === blueOrangeBlue ? 'blue' :
                polarity === lowIsGood ? 'green' :
                polarity === highIsGood ? 'red' :
                'grey');
            break;
        case TrendMarkerValue.NoChange:
            imageName = 'no_change';
            break;
        default:
            imageName = 'no_calc';
            break;
    }
    return "<img src='/images/trends/" + imageName + ".png" + "'/>";
}


// Constants
REGIONAL_COMPARATOR_ID = 1;
NATIONAL_COMPARATOR_ID = 4;
TARGET_COMPARATOR_ID = 2;
NOT_APPLICABLE = 'n/a';
NATIONAL_CODE = 'E92000001';
SEARCH_NO_RESULT_TOP_OFFSET = 0;
SEARCH_NO_RESULT_LEFT_OFFSET = 0;

/**
* Enum of PHOLIO area type IDs.
* @class AreaTypeIds
*/
AreaTypeIds = {
    District: 2,
    Region: 6,
    Practice: 7,
    County: 9,
    Country: 15,
    UnitaryAuthority : 16,
    GpShape: 18,
    CCG: 19,
    DeprivationDecile: 23,
    Subregion: 46,
    DistrictUA: 101,
    CountyUA: 102,
    PheCentres2013: 103,
    PheCentres2015: 104,
    OnsClusterGroup: 110
};

/**
* Enum of PHOLIO category type IDs.
* @class CategoryTypeIds
*/
CategoryTypeIds = {
    CountyUA: 2,
    DistrictUA: 3,
    HealthProfilesSSILimit: 5,
    CCG: 11
};

/**
* Enum of PHOLIO value type IDs.
* @class ValueTypeIds
*/
ValueTypeIds = {
    Count: 7
};

/**
* Enum of PHOLIO polarity IDs.
* @class PolarityIds
*/
PolarityIds = {
    RAGLowIsGood: 0,
    RAGHighIsGood: 1,
    Quintiles: 15,
    BlueOrangeBlue: 99
}

/**
* Enum of PHOLIO sex IDs.
* @class SexIds
*/
SexIds = {
    Person: 4
};

/**
* Enum of PHOLIO profile IDs.
* @class ProfileIds
*/
ProfileIds = {
    SearchResults: 13,
    PracticeProfiles: 20, 
    Mortality: 22,
    HealthProfiles: 26,
    CommunityMentalHealth: 50,
    Diabetes: 51,
    Liver: 55,
    Hypertension: 67,
    Cancer: 71,
    DrugsAndAlcohol: 75,
    HealthChecks: 77,
    ChildHealth: 105
};

TrendMarkerValue = {
    Up: 1,
    Down: 2,
    NoChange: 3,
    CannotCalculate: 4
};

// Application state
isSpinnerDisplayed = true;
ajaxLock = null;
