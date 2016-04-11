// Base namespace for practice profiles specific code
PP = {};

function documentReady() {

    if (!isInitRequired) return;

    lock();

    PP.model.reset();
    ftHistory.init(PP.model);

    spineChart.init(200, 18);
    stems = new SpineChartStems({
        min: 'Lowest',
        max: 'Highest'
    });

    // Init area lists
    loaded.areaLists[PRACTICE] = {};
    loaded.areaLists[AreaTypeIds.CCG] = {};

    addYearHashes(loaded.population);
    loaded.indicatorMetadata = {};
    loaded.indicatorStats = {};
    loaded.addresses = {};

    var subgroupSelection = $('#subgroupSelection');

    spineChartJQs = [subgroupSelection];
    metadataJQs = [$('.indicatorSelection'), subgroupSelection];
    scatterJQs = metadataJQs;
    trendsJQs = [subgroupSelection];
    barJQs = metadataJQs;

    allJQs = [spineChartJQs, metadataJQs];

    PP.menuManager = new MenuManager();

    ajaxMonitor.setCalls(1);

    getGroupingTree();

    ajaxMonitor.monitor(PP.model.restore);

    tooltipManager.init();

    initYears();

    initAreaSearch('#searchText', FT.model, true);
};

function handleAjaxFailure(e) {

    var href = window.location.href;

    // Only reset URL if it contains a hash state string
    if (href.indexOf('#') > 0) {
        alert("The data could not be loaded. Please try again.")
        PP.model.reset();
    }
};

function getPracticeList() {

    var parentCode = PP.model.parentCode;

    if (isDefined(parentCode)) {
        var obj = loaded.areaLists[PRACTICE][parentCode];

        if (isDefined(obj)) {
            getPracticeListCallback(obj);
        } else {

            var areaList = loaded.areaLists[PRACTICE];
            var areas = areaList[parentCode];
            if (isDefined(areas)) {

                getPracticeListCallback(areas);

            } else {

                ajaxGet('data/areas',
                    'parent_area_code=' + parentCode +
                        '&area_type_id=' + PRACTICE,
                    getPracticeListCallback);
            }
        }
    }
    else {
        // No practice displayed
        $('#all_ccg_practices').hide();
        $('#practiceMenuBox').hide();
        ajaxMonitor.callCompleted();
    }
};

function getPracticeListCallback(obj) {

    loaded.areaLists[PRACTICE][PP.model.parentCode] = obj;

    displayPracticeMenu(obj);

    ajaxMonitor.callCompleted();
};

function practiceChanged(practiceCode) {

    lock();

    setPracticeCode(practiceCode);

    setPopulationModeIfDataPageNotDisplayed();

    ftHistory.setHistory();

    goToCurrentPage();
};

function getLabelSeries(arg) {

    if (!isDefined(labels[arg])) {
        ajaxGet('GetLabelSeries.ashx', 'id=' + arg,
            getLabelSeriesCallback);
    } else {

        ajaxMonitor.callCompleted();
    }
};

function getLabelSeriesCallback(obj) {
    if (isDefined(obj)) {
        labels[obj.Id] = obj.Labels;
    }

    ajaxMonitor.callCompleted();
};

function showBox(id) {

    id = id.toLowerCase();
    var pageSelected = 'pageSelected';
    $('.pageBox').hide();
    $('.' + 'pageSelected').removeClass(pageSelected);
    $('#page-' + id).addClass(pageSelected);
    $('#' + id + 'Box').show();
};

function goToCurrentPage(arg) {

    lock();

    switch (PP.model.mode) {
        case PAGE_MODES.SELECT:
            goToPracticeSearchPage(arg);
            break;
        case PAGE_MODES.SPINE:
            goToIndicatorsPage(arg);
            break;
        case PAGE_MODES.POPULATION:
            goToPopulationPage(arg);
            break;
        case PAGE_MODES.METADATA:
            goToMetadataPage(arg);
            break;
        case PAGE_MODES.SCATTER:
            goToScatterPage(arg);
            break;
        case PAGE_MODES.BAR:
            goToBarChartPage(arg);
            break;
        case PAGE_MODES.CLUSTER:
            goToClusterPage(arg);
            break;
        case PAGE_MODES.TRENDS:
            goToTrendsPage(arg);
            break;
    }
};

PAGE_MODES = {
    SPINE: 1,
    POPULATION: 2,
    METADATA: 3,
    SCATTER: 4,
    BAR: 5,
    SELECT: 6,
    CLUSTER: 7,
    TRENDS: 8
};

function makeValuesNegative(values) {
    for (i in values) {
        values[i] = -Math.abs(values[i]);
    }
};

colours = {
    better: '#92d050',
    same: '#ffc000',
    worse: '#c00000',
    none: '#ffffff',
    noComparison: '#c9c9c9',
    limit99: '#a8a8cc',
    limit95: '#444444',
    border: '#666666',
    comparator: '#000000',
    bobLower: '#5555E6',
    bobHigher: '#C2CCFF'
};

function getPracticeParentName() {

    var option = $(ID_PARENT_AREA_MENU + " option:selected");

    return option.val() != NULL_OPTION
        ? option.text()
        : getPracticeParentLabel();
};

function getPracticeParentLabel() {
    return "CCG";
};

function yearClicked(year) {

    if (!ajaxLock) {

        lock();

        setPopulationModeIfDataPageNotDisplayed();
        setYear(year);

        ftHistory.setHistory();

        goToCurrentPage();
    }
};

function getYearOffset() {
    return latestYear - PP.model.year;
};

function setPracticeCode(code) {

    PP.model.practiceCode = code && code !== NULL_OPTION ?
        code :
        null;

    ui.setPreferredPractice();

    selectCodeInMenu(ID_PRACTICE_MENU, PP.model.practiceCode);
};

ui = {

    // Array of all the SELECT elements in the dom
    _selects: null,

    practice: null, /* Area object*/
    rootIndexes: {},
    indicatorMenuSubgroupId: null,
    callbackIds: {},
    displayedAddressCode: null,

    // Stores the practice that was last selected by the user for a given CCG
    preferredPractices: {},
    _getParentKey: function () {
        return getKey(PP.model.parentAreaType);
    },

    setPreferredPractice: function () {
        var parent = PP.model.parentCode;
        if (parent !== null) {
            this.preferredPractices[parent] = PP.model.practiceCode;
        }
    },

    getPreferredPractice: function () {
        var code = this.preferredPractices[PP.model.parentCode];
        if (isDefined(code)) {
            return code;
        }
        return null;
    },

    setPreferredGroupRoot: function (index) {
        var sid = PP.model.groupId;
        this.rootIndexes[sid] = index;
    },

    getPreferredGroupRoot: function () {
        var sid = PP.model.groupId;
        return this._getValidRootIndex(this.rootIndexes[sid]);
    },

    getMetadataHash: function () {
        var sid = PP.model.groupId;
        return loaded.indicatorMetadata[sid];
    },

    getMetadataHashBySid: function (sid) {
        return loaded.indicatorMetadata[sid];
    },

    getSelectedRootIndex: function () {
        return this._getValidRootIndex($('#indicatorMenu').find('option:selected').val());
    },

    _getValidRootIndex: function (i) {
        if (!isDefined(i)) {
            i = 0;
        }
        return i;
    },

    getPractices: function () {
        return loaded.areaLists[PRACTICE][PP.model.parentCode];
    },

    /* The area object */
    getCurrentPractice: function () {

        var targetCode = PP.model.practiceCode;

        if (!isDefined(this.practice) || this.practice.Code !== targetCode) {

            if (PP.model.parentCode === null) {
                // No parents
                this.practice = loaded.addresses[targetCode];
            } else {
                // Practice belongs to current CCG
                var practices = this.getPractices();
                for (var i in practices) {
                    if (practices[i].Code === targetCode) {
                        this.practice = practices[i];
                        break;
                    }
                }
            }
        }

        return this.practice;
    },

    getData: function (areaCode) {
        var groupId = PP.model.groupId;
        var data = loaded.data;
        if (data.hasOwnProperty(groupId)) {
            return data[groupId][areaCode];
        }
        return null;
    },

    doIndicatorRepopulate: function () {
        return this.indicatorMenuSubgroupId === null ||
            this.indicatorMenuSubgroupId != PP.model.groupId;
    },

    setIndicatorMenuSubgroupId: function () {
        // Set the subgroupId so the menu only needs to be repopulated when required
        this.indicatorMenuSubgroupId = PP.model.groupId;
    }
}

function getGroupingTree() {
    getData(getGroupingTreeCallback, 'sg', 'gid=' + groupIds);
};

function getGroupingTreeCallback(obj) {

    if (isDefined(obj)) {
        loaded.subgroups = obj;
        PP.model.groupId = obj[0].Id
        populateSubgroupMenu(obj, 'subgroupMenu');
    }

    ajaxMonitor.callCompleted();
};

function populateSubgroupMenu(items, id) {

    var selectElement = $('#' + id)[0];
    selectElement.options.length = 0;

    var j = 0;
    for (var i in items) {
        var item = items[i];
        selectElement.options[j++] = new Option(item.Name, item.Id);
    }
};

function getSelectedSubgroup() {
    return $('#subgroupMenu').val();
};

function subgroupChanged(val) {

    lock();

    PP.model.groupId = val;
    ftHistory.setHistory();

    goToCurrentPage();
};

function getIndicatorStats(groupId, year, offset) {

    var stats = loaded.indicatorStats[groupId];

    if (isDefined(stats) && isDefined(stats[year])) {

        ajaxMonitor.callCompleted();
    } else {
        getData(getIndicatorStatsCallback, 'is',
            'gid=' + groupId +
                '&ati=' + PRACTICE +
                '&off=' + offset +
                '&par=' + NATIONAL_CODE
        );
    }
}

function getIndicatorStatsCallback(obj) {

    if (isDefined(obj)) {

        var sid = PP.model.groupId;
        var stats = loaded.indicatorStats;
        checkHash(stats, sid);
        var year = ajaxMonitor.state.year;
        stats[sid][year] = obj;
    }

    ajaxMonitor.callCompleted();
};

function getValF(data, unitLabel) {

    if (isDefined(data)) {
        return data.ValF + unitLabel;
    }

    return NO_DATA;
};

function getUnitLabel(metadata) {

    return (isDefined(metadata) && metadata.Unit.Id === 5)
        ? '<span class="unit">%</span>'
        : '';
};

function lock() {

    if (!ajaxLock) {
        ajaxLock = 1;
        PP.menuManager.disableAll();
    }
};

function addYearHashes(hash) {
    for (var i = earliestYear; i <= latestYear; i++) {
        hash[i] = {};
    }
};

function IndicatorFormatter(items) {

    // Private variables
    var stats = items.stats,
    metadata = items.metadata,
    unit = metadata.Unit,
    valueAndUnit = new ValueWithUnit(unit),
    isInverted = false,
    average = items.average;

    this.getIndicatorName = function () {
        return metadata.Descriptive.Name;
    };

    this.getIndicatorNameLong = function () {
        return metadata.Descriptive.NameLong;
    };

    this._getVal = function (data, key) {
        if (!data || data[key] === '-') {
            return NO_DATA;
        }
        return valueAndUnit.getShortLabel(data[key]);
    }

    this.getSuffixIfNoShort = function () {
        return new ValueSuffix(unit).getFullLabelIfNoShort();
    }

    this.getArea = function (id) {
        return this.getMarker(id).name;
    }

    this.getAverage = function () {
        return average;
    };

    this.getMin = function () {
        return this._getVal(stats,
            (isInverted ? 'Max' : 'Min')
        );
    };

    this.getMax = function () {
        return this._getVal(stats,
            (isInverted ? 'Min' : 'Max')
        );
    };

    this.get25 = function () {
        return this._getVal(stats,
            (isInverted ? 'P75' : 'P25')
        );
    };

    this.get75 = function () {
        return this._getVal(stats,
            (isInverted ? 'P25' : 'P75')
        );
    };
};

function unlock() {

    if (ajaxLock === 1) {
        ajaxLock = null;

        PP.menuManager.enableAll();

        hideSpinner();
        $(MAIN).show();
    }
};

function getDeprivationCode(decile) {
    if (isDefined(decile)) {
        return 'cat-1-' + decile;
    }
    return null;
};

function getPeerGroupCode(number) {
    if (isDefined(number)) {
        return 'S_' + number;
    }
    return null;
};

BENCHMARKS = {
    CCG: 0,
    DEPRIVATION: 1,
    PEER_GROUP: 2
};

/* Overrides function in PageMetadata.js */
function goToMetadataPage() {
    lock();

    setPageMode(PAGE_MODES.METADATA);

    ajaxMonitor.setCalls(6);

    getPracticeAndParentLists();

    var sid = PP.model.groupId;
    getIndicatorMetadata(sid);
    getNationalData(sid);
    getMetadataProperties();

    ajaxMonitor.monitor(displayPracticeProfilesMetadata);
};

function displayPracticeProfilesMetadata() {

    evaluateIndicator1Menu();

    var rootIndex = ui.getSelectedRootIndex();
    var sid = PP.model.groupId;

    if (metadataState.redisplay(rootIndex, sid)) {

        metadataState.setDisplayedKey(rootIndex, sid);

        var metadata = ui.getMetadataHash();
        var root = ui.getData(NATIONAL_CODE)[rootIndex];
        var iid = root.IID;

        displayMetadata(metadata[iid], root);
    }

    showAndHidePageElements();

    unlock();
};

function populateIndicatorMenu(jq, metadata, roots, preferredIndex) {

    var selectElement = jq[0];
    selectElement.options.length = 0;

    for (var i in roots) {
        var root = roots[i];
        var id = root.IID;
        selectElement.options[i] = new Option(metadata[id].Descriptive.Name +
                new SexAndAge().getLabel(root), i);
    }

    if (isDefined(preferredIndex)) {
        for (var j in roots) {
            if (preferredIndex == j) {
                var s = 'selected';
                $(selectElement.options[j]).attr(s, s);
                break;
            }
        }
    }
};

function indicatorChanged(index) {

    // User selected menu option

    ui.setPreferredGroupRoot(index);
    ftHistory.setHistory();

    goToCurrentPage();
};

function showAndHidePageElements() {

    if (tooltipManager.isVisible) {
        tooltipManager.hide();
    }

    switch (PP.model.mode) {
        case PAGE_MODES.SELECT:
            showAllJQs([]);
            showBox('Map');
            break;
        case PAGE_MODES.SPINE:
            showAllJQs(spineChartJQs);
            showBox('Indicators');
            break;
        case PAGE_MODES.POPULATION:
            showAllJQs([]);
            showBox('Population');
            break;
        case PAGE_MODES.METADATA:
            showAllJQs(metadataJQs);
            showBox('Metadata');
            break;
        case PAGE_MODES.SCATTER:
            showAllJQs(scatterJQs);
            showBox('Scatter');
            break;
        case PAGE_MODES.BAR:
            showAllJQs(barJQs);
            showBox('Bar');
            break;
        case PAGE_MODES.CLUSTER:
            showAllJQs([]);
            showBox('Cluster');
            break;
        case PAGE_MODES.TRENDS:
            showAllJQs(trendsJQs);
            showBox('Trends');
            break;
    }
};

function evaluateIndicator1Menu() {

    var roots = ui.getData(NATIONAL_CODE, PP.model.year);
    var jq = $('#indicatorMenu');

    if (ui.doIndicatorRepopulate()) {

        ui.setIndicatorMenuSubgroupId();

        populateIndicatorMenu(jq, ui.getMetadataHash(), roots);
    }

    // Not ideal that indicator index set every time, should be only on hash restore
    var i = getIndicator1Index(roots);
    jq.val(i.toString());
};

function selectRoot(index) {

    /* User clicked link in spine chart */

    if (!ajaxLock) {
        lock();

        ui.setPreferredGroupRoot(index);
        PP.model.mode = PAGE_MODES.BAR;
        ftHistory.setHistory();

        goToBarChartPage();
    }
};

function selectMenuOption(id, index) {

    $('#' + id + ' option:eq(' + index + ')').attr('selected', 'selected');
};

function getPracticeFromCode(code) {
    var practices = ui.getPractices();
    for (i in practices) {
        if (practices[i].Code === code) {
            return practices[i];
        }
    }
    return null;
};

function getPracticeLabel() {
    if (PP.model.practiceCode === null) {
        return '';
    }
    var p = ui.getCurrentPractice();

    return p.Code + ' - ' + p.Name;
};

function displayPracticeMenu(areas) {

    var practiceMenu = $(ID_PRACTICE_MENU),
    currentAreaCode = PP.model.practiceCode,
    selected = 'selected';

    // Current practice is in list of areas
    var options = practiceMenu[0].options;
    options.length = 0;

    var j = 0;
    options[j++] = new Option('SELECT A PRACTICE >>', '-');

    for (var i in areas) {
        var area = areas[i],
        code = area.Code,
        option = new Option(code + ' - ' + area.Name, code);
        options[j++] = option;

        // Select option
        if (code === currentAreaCode) {
            $(option).prop(selected, selected);
        }
    }

    // Menu must be visible before chosen is called
    $('#practiceMenuBox').show();

    if (!$(ID_PRACTICE_MENU + '_chosen').length) {
        practiceMenu.chosen({ search_contains: true });
    }
};

function getPracticeAddress() {

    displayPracticeHeading();

    var practiceTitle = $('#practiceTitle');
    var code = PP.model.practiceCode;
    if (code) {
        // A practice is selected

        if (ui.displayedAddressCode !== code) {
            // Practice address not already displayed
            var address = loaded.addresses[code];
            ui.displayedAddressCode = code;
            if (isDefined(address)) {
                // Practice address previously retrieved from server
                updatePracticeAddress(address);
            } else {
                // Practice address not retrieved from server yet
                practiceTitle.html('');
                ajaxGet('GetData.ashx', "s=aa&are=" + code, getPracticeAddressCallback)
                return;
            }
        }
    } else {
        // No practice selected
        ui.displayedAddressCode = null;
        practiceTitle.html('');

        //TODO temp try catch for optional inclusion of cluster code
        try {
            if (clusterState.isEditMode) {
                cluster.setUpClusterIcon();
            }
        } catch (e) { }
    }

    ajaxMonitor.callCompleted();
};

function getPracticeAddressCallback(obj) {

    if (isDefined(obj)) {

        var code = obj.Code;
        loaded.addresses[code] = obj;

        if (PP.model.practiceCode === code) {
            updatePracticeAddress(obj);
        }
    }

    ajaxMonitor.callCompleted();
};

function updatePracticeAddress(address) {

    $('#practiceTitle').html(getAddressText(address));

    //TODO temp try catch for optional inclusion of cluster code
    try {
        if (clusterState.isEditMode) {
            cluster.setUpClusterIcon();
        }
    } catch (e) { }
};

function clearPracticeAddress() {
    $('#address').html('');
};

function setPageMode(newMode) {
    if (PP.model.mode !== newMode) {
        PP.model.mode = newMode;
        ftHistory.setHistory();
    }
};

PP.model = {

    mode: null,
    year: null,
    parentAreaType: null,
    parentCode: null,
    practiceCode: null,

    groupId: null,
    indicator1: null,

    groupId2: null,
    indicator2: null,

    // Static function hence this not used
    reset: function () {
        var m = PP.model;
        m.mode = PAGE_MODES.SELECT;
        m.year = latestYear;
        m.parentAreaType = AreaTypeIds.CCG;
        m.parentCode = null;
        m.practiceCode = null;
        /* PP.model.groupId is set when subgroup menu is populated
        to avoid having to hard code an ID*/
    },

    restore: function () {

        var m = PP.model;

        m.reset();
        m._update();

        m.setupUI();

        ui.setPreferredPractice();
        goToCurrentPage();
    },

    setupUI: function () {

        setYear(PP.model.year);

        $('#subgroupMenu').val(PP.model.groupId);

        setAreaType(PP.model.parentAreaType);
    },

    _update: function () {

        var v = ftHistory.getKeyValuePairsFromHash();

        if (_.size(v)) {

            var s = this;
            s.mode = parseInt(v['mod'], 10);
            s.year = v['pyr'];
            s.parentAreaType = parseInt(v['pat'], 10);
            s.parentCode = s._parseVal(v['par']);
            s.practiceCode = s._parseVal(v['are']);

            s.groupId = s._parseVal(v['sid1']);
            s.indicator1 = s._parseIndicator(v['ind1']);

            s.groupId2 = s._parseVal(v['sid2']);
            s.indicator2 = s._parseIndicator(v['ind2']);
        }
    },

    _parseIndicator: function (indicator) {

        if (indicator.length < 2/* not '-' or empty */) {
            return '-';
        }
        var bits = indicator.split('-');
        return { IID: bits[0], SexId: bits[1] };
    },

    _parseVal: function (val) {
        return val === '-' ? null : val;
    },

    _valToString: function (val) {
        return val === null ? '-' : val;
    },

    toString: function () {
        var sid1 = this._valToString(this.groupId);
        var sid2 = this._valToString(this.groupId2);

        /* Cannot use root index because order may change in grouping tables, but 
        * information regarding grouping may not yet have been retrieved from server */
        indicator1 = this._getIndicatorKey(ui.getPreferredGroupRoot(), this.groupId);
        indicator2 = this._getIndicatorKey(scatterState.getPreferredGroupRoot(), this.groupId2);

        var parentCode = this._valToString(PP.model.parentCode);
        var practiceCode = this._valToString(PP.model.practiceCode);

        var s = [
            'mod', PP.model.mode,
            'pyr', this.year,
            'pat', this.parentAreaType,
            'par', parentCode,
            'are', practiceCode,
            'sid1', sid1,
            'ind1', indicator1,
            'sid2', sid2,
            'ind2', indicator2
        ];

        return s.join(',');
    },

    _getIndicatorKey: function (i, sid) {
        if (isDefined(i) && isDefined(sid)) {

            checkHash(loaded.data, sid);

            var subgroupData = loaded.data[sid];
            if (subgroupData.hasOwnProperty(NATIONAL_CODE)) {
                var root = subgroupData[NATIONAL_CODE][i];
                return root.IID + '-' + root.SexId;
            }
        }
        return '-';
    },

    getRootIndex: function (indicator, roots) {
        var found = false;
        if (indicator !== '-') {

            // Translate indicator data into root index
            for (var i in roots) {
                var root = roots[i];
                if (root.IID == indicator.IID &&
                        root.SexId == indicator.SexId) {
                    found = true;
                    break;
                }
            }
        }

        // Use first indicator if indicator no longer in grouping
        return found ? i : 0;
    },

    isPractice: function () {
        return this.practiceCode !== null
    }
};

function setYear(year) {

    PP.model.year = year;
    displayCurrentYear();
};

function setAreaType(id) {

    PP.model.parentAreaType = id;

    var label = 'CCG';
    $('#areaTypeSelected').html(label);
    $('#areaTypeUnselected').html(label);
    $('#parentBoxLabel').html(label + ':');
    $('#benchmarkParentOption').html(label);

    setBenchmarkImage();

    updateBenchmarkHeaders();
};

function selectCodeInMenu(id, code) {
    if (isDefined(code)) {
        $(id).val(code);
    } else {
        $(id).val(NULL_OPTION);
    }
};

function getIndicator1Index(roots) {

    var indicator = PP.model.indicator1;
    if (indicator !== null) {

        // Restore model from hash - index can be derived now data is loaded
        var i = PP.model.getRootIndex(indicator, roots);
        PP.model.indicator1 = null;
        ui.setPreferredGroupRoot(i);
    }

    return ui.getPreferredGroupRoot();
};

function getIndicator2Index(roots) {

    var indicator = PP.model.indicator2;

    if (indicator !== null) {

        // Restore model from hash - index can be derived now data is loaded
        var i = PP.model.getRootIndex(indicator, roots);
        PP.model.indicator2 = null;
        scatterState.setPreferredGroupRoot(i);
    } else {

        // Apply default index so user does not see diagonal line on first sight
        i = scatterState.indicator2DefaultIndex;
        if (i !== null) {

            // Check the index is valid
            if (i > roots.length - 1) {
                i = 0;
            }

            // Reset as only want to perform check once
            scatterState.indicator2DefaultIndex = null;
        }
    }

    if (i !== null) {
        scatterState.setPreferredGroupRoot(i);
    }

    return scatterState.getPreferredGroupRoot();
};

function ifNoLock(func) {
    if (!ajaxLock) {
        func();
    }
};

function showExport() {

    if (!ajaxLock) {

        // Parent
        var isPractice = PP.model.isPractice();
        var practice = isPractice ?
            getPracticeLabel() :
            '';

        // Practice parent
        var isParent = PP.model.parentCode !== null;
        var parent = isParent ?
            getPracticeParentName() :
            getPracticeParentLabel();

        var menuId = "exportSubgroupMenu";

        var html = templates.render('export', {
            practice: practice,
            parent: parent,
            isPractice: isPractice,
            isParent: isParent,
            menuId: menuId
        });
        lightbox.show(html, 150, 410, 400);

        populateSubgroupMenu(loaded.subgroups, menuId);
        var menu = $('#' + menuId);
        menu.prepend(
            '<option value="pop" selected="selected">Population Age Distribution</option>');
        menu[0].selectedIndex = 0;
    }
};

function exportData() {

    // Determine area code
    var val = parseInt($('input[name=g1]:checked').val(), 10);
    switch (val) {
        case 0:
            var code = PP.model.practiceCode;
            break;
        case 1:
            code = PP.model.parentCode;
            break;
        default:
            code = NATIONAL_CODE;
    }

    var sid = $('#exportSubgroupMenu').val();
    var args = sid === 'pop' ?
        '&pro=qp&gid=' + populationGroupId :
        '&pro=pp&gid=' + sid;

    lightbox.hide();

    setUrl(FT.url.corews + 'GetData.ashx?s=db&are=' + code + '&ati=' + PP.model.parentAreaType + args);

    logEvent('Download', 'Excel', 'AreaCode=' + code + args);
};

function downloadPdf() {

    try {
        // This section can fail for some IE7 users, reason unknown
        if (!ajaxLock) {
            var practiceCode = PP.model.practiceCode;
            if (practiceCode) {
                var url = FT.url.pdf + '?' + 
                    'CCG=' + PP.model.parentCode +
                    '&PracCode=' + practiceCode;

                setUrl(url);

                logEvent('Download', 'PDF', practiceCode);
            } else {
                lightbox.show('Please select a practice', 300, 700, 300);
            }
        }
    } catch (e) {
        try {
            // NEPHO practice summary page
            lightbox.show('<div>Please <a href="' + url +
                    '">click here</a> to <br>download a practice<br>summary.</div>',
                300, 500, 160);
        } catch (e) {
            // Lightbox not work
            setUrl(url);
        }
    }
};

function showMetadata(groupId, indicatorId, e) {

    var a = getMetadataHtml(ui.getMetadataHashBySid(groupId)[indicatorId],
        null);

    var html = '<div style="padding:15px;">' + a.html + '</div>';

    var popupWidth = 800;
    var left = ($(window).width() - popupWidth) / 2;

    var top = 200;
    if (isDefined(e)) {
        top = $(e).parent().position().top - top;
    }

    lightbox.show(html, top, left, popupWidth);

    labelAsync.populate(a.labelArgs);
}

function getCurrentSubgroup() {
    return PP.model.groupId;
}

// These are different on IE 7-8 because bar label top offset does not work
chartColours = {
    label: '#333',
    bar: '#76B3B3',
    pink: '#FF66FC'
}

CHART_TEXT_STYLE = {
    color: chartColours.label,
    fontWeight: 'normal',
    fontFamily: 'Verdana'
};

function getParentList() {

    var areaTypeId = PP.model.parentAreaType;
    if (!loaded.areaLists[areaTypeId].length) {

        var parameters = new ParameterBuilder();
        parameters.add('area_type_id', areaTypeId);
        parameters.add('profile_id', ProfileIds.PracticeProfiles);

        ajaxGet('data/areas',
            parameters.build(),
            function (obj) {

                if (isDefined(obj)) {
                    loaded.areaLists[areaTypeId] = obj;
                    populateParentMenu(obj);
                }

                ajaxMonitor.callCompleted();
            });
    } else {
        ajaxMonitor.callCompleted();
    }
}

function getPracticeAndParentLists() {
    getParentList();
    getPracticeList();
    getPracticeAddress();
}

function initYears() {

    templates.add('years', '<table cellspacing="0">\
<tr><td onclick="yearClicked({{first}})" class="firstYearLine"></td>{{#middleYears}}<td class="yearLine" onclick="yearClicked({{year}})"></td>{{/middleYears}}<td class="lastYearLine" onclick="yearClicked({{last}})"></td></tr>\
<tr><td id="timeline" colspan="{{yearCount}}">TIMELINE<div id="info-timeline" class="infoTooltip" onclick="getHelpText(\'timeline\',200,lightbox.leftForMiddle(410),410)" title="More information about this timeline"></div></td></tr></table>');

    var middleYears = [];
    for (var year = earliestYear + 1; year < latestYear; year++) {
        middleYears.push({ year: year });
    }

    $('#yearBox').html(
        templates.render('years', {
            middleYears: middleYears,
            first: earliestYear,
            last: latestYear,
            yearCount: latestYear - earliestYear + 1
        })
    );

    displayCurrentYear();
}

function displayCurrentYear() {

    var year = PP.model.year,
    tdIndex = year - earliestYear,
    selectedYearClass = 'selectedYear',
    stem = selectedYearClass + 'Line',
    last = 'Last';

    var lineClass = stem;
    if (year == earliestYear) {
        lineClass += 'First';
    } else if (year == latestYear) {
        lineClass += last;
    }

    var jq = $('#yearBox TR:eq(0) TD');
    jq.removeClass(stem + ' ' + stem + 'First ' + stem + last);
    $(jq[tdIndex]).addClass(lineClass);
}

function displayPracticeHeading() {

    $('#practiceHeading').css('color',
        PP.model.parentCode ?
            '#333' :
            '#fff'
    );
}

function setPopulationModeIfDataPageNotDisplayed() {
    // Display population page if on a page without practice data
    if (PP.model.mode === PAGE_MODES.SELECT ||
            PP.model.mode === PAGE_MODES.METADATA) {
        PP.model.mode = PAGE_MODES.POPULATION;
    }
}

function showSpinnerIfNoPopulation() {
    var practiceCode = PP.model.practiceCode;
    if (isDefined(practiceCode)) {
        var pop = loaded.population[PP.model.year][practiceCode];
        if (!isDefined(pop)) {
            showSpinner();
        }
    }
}

function populateParentMenu(areas) {

    var id = ID_PARENT_AREA_MENU,
    $menu = $(id),
    model = PP.model;

    populateAreaMenu(areas, $menu, 'SELECT A CCG >>');
    selectCodeInMenu(id, model.parentCode);
    $menu.show();

    $menu.chosen({ search_contains: true });

    $menu.change(function () {

        lock();

        // Set parent code
        var parentCode = $menu.find('option:selected').val();
        model.parentCode = parentCode === NULL_OPTION ?
            null :
            parentCode;

        model.practiceCode = ui.getPreferredPractice();
        ftHistory.setHistory();

        ajaxMonitor.setCalls(1);

        getPracticeList();

        ajaxMonitor.monitor(goToCurrentPage);

        // Display show all practices link
        var $showAllPractices = $('#all_ccg_practices');
        parentCode != '-' ? $showAllPractices.show() : $showAllPractices.hide();
    });
}

function getHelpText(key, top, left, width) {
    if (!ajaxLock) {
        var help = 'help';
        if (!loaded.hasOwnProperty(help)) {
            loaded[help] = {};
        }
        if (loaded[help].hasOwnProperty(key)) {
            lightbox.show(loaded[help][key], top, left, width);
        } else {
            lightbox.show('<div class="ajax-loading"><img src="' + FT.url.img + 'spinner.gif" alt=""/></div>', top, left, width);
            getData(getHelpTextCallback, 'ht', 'key=' + key);
        }
    }
}

function getHelpTextCallback(obj) {
    var txt = obj.Content;
    lightbox.setHtml(txt);
    loaded.help[obj.Key] = txt;
}

function showAllJQs(toShow) {

    for (var i in allJQs) {
        if (allJQs[i] !== toShow) {
            hideJQs(allJQs[i]);
        }
    }

    showJQs(toShow);
};

function TimePeriod(timePeriods, nationalDataPointCount, yearOffset) {

    var isNationalData = nationalDataPointCount > 0,
    timePeriods = timePeriods;

    if (isNationalData) {
        // [] --yearIndex--> {Val,LoCI,UpCI,etc}
        var yearIndex = nationalDataPointCount - yearOffset - 1;
    } else if (yearOffset === 0) {
        // Use most recent year's data
        yearIndex = nationalDataPointCount - 2;
        if (yearIndex < 0) {
            yearIndex = 0;
        }
    } else {
        timePeriods = null;
    }

    this.isData = timePeriods && timePeriods[yearIndex];

    this.label = this.isData ?
        timePeriods[yearIndex] :
        NO_DATA;

    this.yearIndex = yearIndex;
}

/**
* Responsible for the enabled status of all the drop down menus.
*/
function MenuManager() {

    var chosenSelect = '.chosen-select',
    disabled = 'disabled',
    $menus = $('select'),
    setDisabled = function (b) {

        // Ensure chosen drops are set
        if (!$chosenDrops) {
            $chosenDrops = $('.chosen-drop');
        }

        $menus.not(chosenSelect).attr(disabled, b);
        $menus.filter(chosenSelect).attr(disabled, b).trigger('chosen:updated');
    },
    $chosenDrops;

    this.enableAll = function () {

        setDisabled(false);

        // Workaround for infinite page height IE11 bug (height of '.chosen-results' becomes -0.001)
        $chosenDrops.show();
    };

    this.disableAll = function () {

        setDisabled(true);

        // Workaround for infinite page height IE11 bug (height of '.chosen-results' becomes -0.001)
        $chosenDrops.hide();
    };
}

templates.add('export', "<div id='popupTitle'><h2>Export Data</h2></div> \
<table class='export greyBoxed'><tr><td><b>Topic:</b></td><td> \
<select id='{{menuId}}'></select></td></tr></table> \
<table class='export greyBoxed'>\
<tr><td colspan='2'><b>Practices:</b></td></tr>\
<tr><td style='width:15px;'><input type='radio' name='g1' value='2' {{^isParent}}checked='checked'{{/isParent}}></td><td>All practices in England </td></tr>\
<tr><td>\
{{#isParent}}<input type='radio' name='g1' value='1' {{^isPractice}}checked='checked'{{/isPractice}}></td><td>All practices in {{parent}}{{/isParent}}\
{{^isParent}}</td><td class='noDataText'>No {{parent}} selected{{/isParent}}\
</td></tr><tr><td>\
{{#isPractice}}<input type='radio' name='g1' value='0' checked></td><td>{{practice}}{{/isPractice}} \
{{^isPractice}}</td><td class='noDataText'>No single practice selected{{/isPractice}} \
</td></tr></table>\
<div class='fl w100' style='padding:15px;padding-left:130px;clear:left;'>\
<input type='button' class='exportBtn' onclick='exportData()' value='Export' />\
<input type='button' class='exportBtn' onclick='lightbox.hide()' value='Cancel' /></div>");

loaded.population = {};
loaded.data = {};
loaded.valueLimits = {};

MAIN = '#main';
ID_PARENT_AREA_MENU = '#parentAreaMenu';
ID_PRACTICE_MENU = '#practiceMenu';
PRACTICE = 7;
NULL_OPTION_TEXT = '...';
NULL_OPTION = '-';
SEX_MALE = 1;
SEX_FEMALE = 2;
SEX_PERSON = 4;

LABELS_QUINARY = 'qpop';
LABELS_DEPRIVATION = 'dep';

SEARCH_NO_RESULT_TOP_OFFSET = 27;
NATIONAL_CODE = 'E92000001';

labels = {};

changeIndex = 0;

lastDecile = null;

CSS_NUMERIC = 'numeric';
CSS_CENTER = 'center';
CSS_VAL = 'value';

nationalPopulation = {};

currentPracticePopulation = null;
correctForPolarity = false;

chart = null;
barChart = null;
ajaxLock = 1;

$(document).ready(documentReady);

