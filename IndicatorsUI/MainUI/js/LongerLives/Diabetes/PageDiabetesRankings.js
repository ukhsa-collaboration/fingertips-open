rankingsState = null;

function initPage() {
    showLoadingSpinner();
    tooltipManager.init();
    updateModelFromHash();
    setSearchText(MT.model.parentAreaType);
}

function updatePage() {

    resetRankingsState();
    setGlobalGroupIds();

    var model = MT.model;
    ajaxMonitor.setCalls(9);

    getAreaAddress(model.parentCode);
    getIndicatorMetadata(model.groupId, GET_METADATA_DEFINITION);
    getIndicatorMetadata(selectedSupportingGroupId);
    getGroupRoots(model);
    getSupportingGroupRoots(model);
    getSupportingAreaDetails(model.parentCode, model.parentAreaType);
    getAreaTypes();
    getEnglandPrimaryData(model);
    getEnglandSupportingData(model);

    ajaxMonitor.monitor(getSecondaryData);
}

function getSecondaryData() {

    changeNumberOrder(); // Reorder table_options pretext

    var model = MT.model;
    setGlobalGroupRootIndexesAndIndicatorId();

    ajaxMonitor.setCalls(rankingsState.comparisonIsAvailable ? 6 : 5);

    if (rankingsState.comparisonIsAvailable) {
        getComparisonValues(model);
    }

    loaded.primaryDataValues.fetchDataByAjax(groupRoots[selectedRootIndex]);
    loaded.practiceValues.fetchDataByAjax(groupRoots[ROOT_INDEXES.POPULATION]);
    getPracticeList();
    getSupportingDataValues(selectedSupportingGroupRootIndex);
    getValueNotes();

    ajaxMonitor.monitor(displayPage);

    populateAreaTypes(model);
    populateCauseList();
    populateSupportingIndicatorList();
    removeLoadingSpinner();
}

function setGlobalGroupIds() {
    var model = MT.model;
    var startingGroupId = model.groupId;

    var indexOfGroupId = _.indexOf(groupIds, startingGroupId);
    isSupportingIndicatorSelected = indexOfGroupId >= domainsToDisplay;

    // Set supporting group id
    selectedSupportingGroupId = isSupportingIndicatorSelected
        ? groupIds[indexOfGroupId]
        : groupIds[domainsToDisplay];

    // Set primary group id
    model.groupId = isSupportingIndicatorSelected
        ? groupIds[0] // group id on model is of a supporting domain
        : model.groupId;
}

function getSupportingDataValues(prevAndRiskRootIndex) {
    var root = supportingGroupRoots[prevAndRiskRootIndex];
    var groupId = selectedSupportingGroupId;
    var model = MT.model;

    var parameters = new ParameterBuilder(
    ).add('group_id', groupId
        ).add('area_type_id', model.areaTypeId
        ).add('parent_area_code', model.parentCode
        ).add('profile_id', model.profileId
        ).add('comparator_id', -1
        ).add('indicator_id', root.IID
        ).add('sex_id', root.Sex.Id
        ).add('age_id', root.Age.Id);

    ajaxGet('api/latest_data/single_indicator_for_all_areas', parameters.build(),
        function (obj) {
            loaded.supportingDataValues[model.areaTypeId] = obj;
            ajaxMonitor.callCompleted();
        });
};

function setGlobalGroupRootIndexesAndIndicatorId() {

    var model = MT.model;

    if (!isDefined(model.indicatorId)) {
        // No indicator specified in hash
        model.indicatorId = getDefaultIndicator();
        selectedRootIndex = 0;
        selectedSupportingGroupRootIndex = 0;
    } else if (isSupportingIndicatorSelected) {
        // Indicator in hash is in a supporting domain
        selectedSupportingGroupRootIndex = getIndexOfGroupRootThatContainsIndicator(
            model, supportingGroupRoots);
        model.indicatorId = getDefaultIndicator();
        selectedRootIndex = 0;
    } else {
        // model.indicatorId already is set to an indicator in a primary domain
        selectedSupportingGroupRootIndex = 0;
        selectedRootIndex = getIndexOfGroupRootThatContainsIndicator(
            model, groupRoots);
    }
}

function getComparisonValues(model) {
    if (model.areaTypeId === AreaTypeIds.Practice) {
        ajaxMonitor.callCompleted();
    } else if (doesAreaTypeCompareToOnsCluster()) {
        getOnsClusterCode(model);
    } else {
        getDecileData(model);
    }
}

function resetRankingsState() {

    var oldState = rankingsState;

    // Global variable containing state specific to this page
    rankingsState = {
        // Row objects that are fed into a template for generating table row HTML
        rows: [],
        // Key(area code)/value(a row) pair list to enable easy look up of a row
        areaCodeToRowHash: {},
        // Whether the view has been initialised
        isInitialised: false,
        // Is the comparison option available to offer
        comparisonIsAvailable: isComparisonAvailable(MT.model),
        sortAscending: true,
        sortedColumn: 2
    }

    // Restore table ordering
    if (oldState) {
        rankingsState.sortAscending = oldState.sortAscending;
        rankingsState.sortedColumn = rankingsState.sortedColumn;
    }
}

function displayRankingLegend() {

    var template;
    var viewModel = {};

    if (MT.model.profileId === ProfileIds.Suicide) {
        viewModel.comparator = 'national';
        template = 'bobLegend';
    } else if (MT.model.areaTypeId === AreaTypeIds.Practice) {
        viewModel.comparator = 'area';
        template = 'bobLegend';
    } else {
        template = 'ragAndBobLegend';
    }

    var html = templates.render(template, viewModel);
    $('#data_legend').html(html);
}

function displayPage() {

    initView();

    displayColumnHeadersAndAddDataToRows();

    var rows = rankingsState.rows;
    displayTable(rows);
    displayRankingLegend();
    toggleComparisonHeader();
    setBreadcrumb();
    displayParentAreaName();
    displayPracticeProfileLink();
    displayAreaDetailsLink();
    displayRankingHeader();
    displayTableHeader(rows);
    $('#area-type-name').html(getAreaTypeNamePlural(MT.model.areaTypeId));
    $('#similar-areas-tooltip').html(getSimilarAreaTooltipText());
    displayLegendHeader();
    displayCompareSimilarHelpText();

    // Similar areas mode may be indicated by area code being passed as a parameter
    if (MT.model.similarAreaCode) {
        if (isNearestNeighbour()) {
            displayNearestNeighbours();
        } else {
            displaySimilarAreas(MT.model.areaCode);
        }
    } else {
        unlock();
    }
}

function displayAreaDetailsLink() {
    if (!hasPracticeData && MT.model.areaCode) {
        // Only display for profiles with area details page
        var $link = $('#area-details-link');
        $link.prop('areaCode', MT.model.areaCode);
        var areaName = getArea().Name;
        $link.html('Details for ' + areaName).show();
    }
}

function goToAreaDetails() {
    var $link = $('#area-details-link');
    MT.model.areaCode = $link.prop('areaCode');
    MT.nav.areaDetails();
}

function displayPracticeProfileLink() {
    if (MT.model.areaTypeId === AreaTypeIds.CCGPreApr2017) {
        $('#practice-profile-link').show();
    }
}

function displayTableHeader(rows) {
    var areaTypeId = MT.model.areaTypeId;
    if (rows.length === 1) {
        var html = '1 ' + getAreaTypeNameSingular(areaTypeId);
    } else {
        html = 'Comparing ' + rows.length + ' ' + getAreaTypeNamePlural(areaTypeId);
    }
    $('#table-header').html(html);
}

function displayCompareSimilarHelpText() {
    var $hoverOrTap = $('#hover-or-tap');
    if (rankingsState.comparisonIsAvailable) {
        var areaTypeId = MT.model.areaTypeId;
        $('#area-type-singular').html(getSimpleAreaTypeNameSingular(areaTypeId));
        $('#area-type-plural').html(getSimpleAreaTypeNamePlural(areaTypeId));
        $hoverOrTap.show();
    } else {
        $hoverOrTap.hide();
    }
}

function displayLegendHeader() {
    // Set comparison text
    var comparisonText;
    if (MT.model.parentCode === NATIONAL_CODE) {
        comparisonText = "Compared to other areas";
    } else {
        comparisonText = "Compared to the other practices in the " + getSimpleAreaTypeName();
    }
    $('#legend-header').html(comparisonText);
}

function displayParentAreaName() {
    var area = loaded.addresses[MT.model.parentCode];

    var areaName = area.AreaTypeId === AreaTypeIds.Country
        ? area.Name
        : 'this ' + getSimpleAreaTypeNameSingular(MT.model.parentAreaType);

    $('.area_name').html(areaName);
}

function displayTable(rows) {

    rows.sort(function (a, b) {
        return a.ValToSortOn - b.ValToSortOn;
    });

    var root = groupRoots[selectedRootIndex];
    sortByPolarity(rows, root.PolarityId);
    sortDisplayedRows(rows);

    var column1Val = "",
        column2Val = "",
        column1Unit = "",
        column2Unit = "",
        showEnglandVal = false;

    var areaTypeId = MT.model.areaTypeId;
    if (areaTypeId !== AreaTypeIds.Practice) {
        var model = {};
        model.areaCode = MT.model.parentCode;
        model.areaTypeId = MT.model.areaTypeId;
        model.profileId = MT.model.profileId;
        model.groupId = MT.model.groupId;

        var primaryMetadataHash = loaded.indicatorMetadata[model.groupId];
        var supportingMetadataHash = loaded.indicatorMetadata[selectedSupportingGroupId];

        // get primary grouping data
        var primaryGroupData = loaded.groupDataAtDataPoint.getData(model);
        column2Val = primaryGroupData[selectedRootIndex].ValF;
        column2Unit = new UnitFormat(primaryMetadataHash[MT.model.indicatorId], column2Val).getLabel();

        // get supporting grouping data
        var selectedSupportingIndicatorId = getSelectedSupportingIndicatorId();
        var supportingIndicatorMetadata = supportingMetadataHash[selectedSupportingIndicatorId];
        model.groupId = selectedSupportingGroupId;
        var supportingGroupData = loaded.groupDataAtDataPoint.getData(model);
        column1Val = supportingIndicatorMetadata.IID === IndicatorIds.Deprivation
            ? 'N/A'
            : supportingGroupData[selectedSupportingGroupRootIndex].ValF;
        column1Unit = new UnitFormat(supportingIndicatorMetadata, column1Val).getLabel();

        showEnglandVal = true;
    }

    // primary indicator sig property to rows object
    // this is used for the tooltip
    _.forEach(rows, function (row) {
        if (row.PrimaryData) {
            row.primaryIndicatorSig = row.PrimaryData !== null ? row.PrimaryData.Sig[-1] : 0;
        }
    });

    // Render data
    $('#diabetes-rankings-table tbody').html(
        templates.render('rows',
            {
                rows: rows,
                englandValCol1: column1Val,
                englandValCol2: column2Val,
                col1Unit: column1Unit,
                col2Unit: column2Unit,
                showEnglandVal: showEnglandVal
            })
    );
}

function setBreadcrumb() {
    var links = ['<li><a href="javascript:MT.nav.home();">Home</a></li>'];

    if (MT.model.areaTypeId === AreaTypeIds.Practice) {
        links.push('<li id="national-comparisons"><a href="javascript:selectAreaType(' +
            MT.model.parentAreaType + ')">National comparisons</a></li>',
            '<li id="practice-comparisons" class="last"><a>Practice comparisons</a></li>');
    } else {
        links.push('<li id="national-comparisons" class="last"><a>National comparisons</a></li>');
    }

    $('#breadcrumbs').html(links);
}

function displayRankingHeader() {
    var areaName = loaded.addresses[MT.model.parentCode].Name;

    var $header = $('#ranking-header');

    if (MT.model.parentCode === NATIONAL_CODE) {
        var header = 'National comparisons';
    } else {
        header = areaName;
        $header.addClass('smaller-header');
    }

    $header.html(header);
}

function getDefaultIndicator() {
    return MT.model.indicatorId = groupRoots[0].IID;
}

function populateAreaTypes(model) {
    var $filter = $('#area-filter');

    if (model.areaTypeId !== AreaTypeIds.Practice) {
        if (_.size(loaded.areaTypes) < 2) {

            $filter.hide();
        } else {
            var templateName = 'areaFilter';

            templates.add(templateName, '<h5><span class="pretext">1.</span><span class="posttext">Select Area</span></h5><ul class="areaFilters">\
            {{#types}}<li id="{{Id}}" class="areaFilter {{class}}"><a href="javascript:selectAreaType({{Id}});">{{Short}}</a></li>{{/types}}' + '<div class="hr"><hr /></div>');

            setAreaTypeOptionHtml(templateName);

            $filter.show();
        }
    } else {
        $filter.hide();
    }
}

function selectAreaType(parentAreaTypeId) {

    if (!FT.ajaxLock) {
        lock();

        var model = MT.model;

        setUrl('/topic/' + profileUrlKey + '/comparisons#par/' + NATIONAL_CODE + '/ati/' + parentAreaTypeId +
            '/iid/' + model.indicatorId + '/gid/' + model.groupId + '/pat/' + parentAreaTypeId);
        window.location.reload();
    }
}

function getIndexOfGroupRootThatContainsIndicator(model, groupRoots) {

    var index = 0;

    var selectedIndex = 0;

    _.each(groupRoots, function (root) {
        if (root.IID === model.indicatorId && root.Sex.Id === model.sexId) {
            selectedIndex = index;
        }
        index++;
    });

    return selectedIndex;
}

function getPracticeList() {

    var model = MT.model;

    var parameters = new ParameterBuilder(
    ).add('profile_id', model.profileId
        ).add('parent_area_code', model.parentCode
        ).add('area_type_id', model.areaTypeId);

    ajaxGet('api/areas/by_parent_area_code', parameters.build(),
        getPracticeListCallback);
};

function getPracticeListCallback(obj) {

    var areasHash = {},
        areaTypeId = MT.model.areaTypeId;

    _.each(obj, function (area) {
        areasHash[area.Code] = area;
    });

    var areaLists = loaded.areaLists;
    if (!areaLists[areaTypeId]) {
        areaLists[areaTypeId] = {};
    }

    areaLists[areaTypeId][MT.model.parentCode] = areasHash;

    ajaxMonitor.callCompleted();
};

function repopulatePrimaryIndicatorList() {
    MT.model.indicatorId = groupRoots[0].IID;
    selectedRootIndex = 0;
    populateCauseList();
    unlock();

    ajaxMonitor.setCalls(1);
    loaded.practiceValues.fetchDataByAjax(groupRoots[ROOT_INDEXES.POPULATION]);
    ajaxMonitor.monitor(selectPrimaryIndicator);
}

function getSupportingIndicators(groupId) {
    var metadataHash = loaded.indicatorMetadata[groupId],
        causes = [], i, metadata;

    var selectedIndicatorId = getSelectedSupportingIndicatorId();

    for (i = 0; i < supportingGroupRoots.length; i++) {

        var indicatorId = supportingGroupRoots[i].IID;
        metadata = metadataHash[indicatorId];
        var stateSex = supportingGroupRoots[i].StateSex;
        var indicatorName = metadata.Descriptive.Name;
        if (stateSex) {
            indicatorName = indicatorName + ' (' + supportingGroupRoots[i].Sex.Name + ')';
        }

        causes.push({
            index: i,
            name: replacePercentageWithArialFont(indicatorName),
            id: indicatorId,
            cssClass: indicatorId === selectedIndicatorId
                ? 'sub prev_risk_active'
                : 'sub'
        });
    }

    return causes;
}

function populateSupportingIndicatorList() {
    var html = templates.render('prevandriskcauses',
        { causes: getSupportingIndicators(selectedSupportingGroupId) });

    $('#diabetes_prev_and_risk_list-' + selectedSupportingGroupId).html(html);
    setDefaultPrevAndRiskIndicator();
}

function setDefaultPrevAndRiskIndicator() {
    $('#prev-' + getSelectedSupportingIndicatorId()).addClass('prev_risk_active');
    $('#domain-' + selectedSupportingGroupId).addClass('active');
}

function selectDomain(groupId) {

    var model = MT.model;

    if (!FT.ajaxLock && groupId !== model.groupId) {
        lock();

        model.groupId = groupId;

        setSelectedPrimaryDomain(groupId);

        ajaxMonitor.setCalls(3);
        getIndicatorMetadata(groupId);
        getGroupRoots(model);
        getEnglandPrimaryData(model);
        ajaxMonitor.monitor(repopulatePrimaryIndicatorList);
    }
}

function selectSupportingDomain(groupId) {

    if (!FT.ajaxLock && selectedSupportingGroupId !== groupId) {
        lock();

        selectedSupportingGroupId = groupId;

        // Set first indicator to be selected
        selectedSupportingGroupRootIndex = 0;

        var model = MT.model;

        ajaxMonitor.setCalls(3);

        getSupportingGroupRoots(model);
        getIndicatorMetadata(selectedSupportingGroupId);

        ajaxMonitor.monitor(repopulateSupportingIndicatorList);

        displaySupportingIndicatorSelection();
        getEnglandSupportingData(model);
        unlock();
    }
}

function displaySupportingIndicatorSelection() {
    // Display new active options
    var cssActiveClass = 'active';

    var groupId = selectedSupportingGroupId;
    $('.prev_and_risk_filters').hide();
    $('#diabetes_prev_and_risk_list-' + groupId).show();
    $('.prev_and_risk_indicator_group').removeClass(cssActiveClass);
    $('#domain-' + groupId).addClass(cssActiveClass);
}

function repopulateSupportingIndicatorList() {
    populateSupportingIndicatorList();
    selectSupportingIndicator(0);
}

// Reload group roots each time to ensure clicking on a grouping reloads the relevant indicators
function getSupportingGroupRoots(model) {

    var parameters = new ParameterBuilder(
    ).add('group_id', selectedSupportingGroupId
        ).add('area_type_id', model.areaTypeId);

    ajaxGet('api/profile_group_roots', parameters.build(),
        function (obj) {
            supportingGroupRoots = obj;
            ajaxMonitor.callCompleted();
        });
}

function getEnglandPrimaryData(model) {
    var modelLocal = {}
    modelLocal.areaCode = model.parentCode;
    modelLocal.areaTypeId = model.areaTypeId;
    modelLocal.profileId = model.profileId;
    modelLocal.groupId = model.groupId;
    loaded.groupDataAtDataPoint.fetchDataByAjax(modelLocal);
}

function getEnglandSupportingData(model) {
    var modelLocal = {}
    modelLocal.areaCode = model.parentCode;
    modelLocal.areaTypeId = model.areaTypeId;
    modelLocal.profileId = model.profileId;
    modelLocal.groupId = selectedSupportingGroupId;
    loaded.groupDataAtDataPoint.fetchDataByAjax(modelLocal);
}


function selectPrimaryIndicator(rootIndex) {

    if (!FT.ajaxLock) {
        lock();

        var model = MT.model;

        rootIndex = isDefined(rootIndex)
            ? rootIndex
            : 0;
        selectedRootIndex = rootIndex;

        var groupRoot = groupRoots[rootIndex];
        model.indicatorId = groupRoot.IID;
        model.sexId = groupRoot.Sex.Id;
        isSupportingIndicatorSelected = false;

        ftHistory.setHistory();

        ajaxMonitor.setCalls(1);
        loaded.primaryDataValues.fetchDataByAjax(groupRoots[selectedRootIndex]);
        ajaxMonitor.monitor(reloadData);

        setSelectedPrimaryIndicator(rootIndex);
    }
}

function setSelectedPrimaryIndicator(index) {
    selectedCause = $('#' + index + '-iid-' + MT.model.indicatorId);
    var cssClass = 'active';
    $('.causes li').removeClass(cssClass);
    selectedCause.addClass(cssClass);
}

function selectSupportingIndicator(rootIndex) {
    if (!FT.ajaxLock) {
        lock();

        var indicatorId = supportingGroupRoots[rootIndex].IID;
        selectedSupportingGroupRootIndex = rootIndex;
        isSupportingIndicatorSelected = true;

        ajaxMonitor.setCalls(1);
        getSupportingDataValues(selectedSupportingGroupRootIndex);
        ajaxMonitor.monitor(reloadData);

        setSelectedSupportingIndicator(indicatorId);
    }
}

function setSelectedSupportingIndicator(indicatorId) {
    selectedCause = $('#prev-' + indicatorId);
    var cssClass = 'prev_risk_active';
    $('.' + cssClass).removeClass(cssClass);
    selectedCause.addClass(cssClass);
}

/**
 * Practice or CCG clicked
 */
function selectRankingsArea(code) {

    var model = MT.model;
    if (hasPracticeData) {
        if (!FT.ajaxLock) {
            // CCG or practice clicked

            lock();
            showLoadingSpinner();

            model.similarAreaCode = null;

            if (model.areaTypeId === AreaTypeIds.Practice) {
                // Practice clicked: Show practice details
                model.areaCode = code;
                MT.nav.practiceDetails(model);
            } else {
                // CCG clicked: Show practices in a CCG
                MT.nav.practiceRankings(code);
            }

            scrollToTop();
        }
    } else {
        // Area clicked: Show area details
        model.areaCode = code;

        if (isNearestNeighbour()) {
            // Need to change NN code to selected area
            model.similarAreaCode = model.parentCode = getNearestNeighbourCode();
        }

        MT.nav.areaDetails(model);
    }
}

function showAllAreas() {

    if (!FT.ajaxLock) {
        lock();

        MT.model.similarAreaCode = null;
        MT.model.areaCode = null;
        ftHistory.setHistory();

        toggleComparisonHeader();
        reloadData();

        logEvent(AnalyticsCategories.RANKINGS, AnalyticsAction.ALL_AREAS);
    }
}

function getNewRowsWithCoreData() {

    var areaCodeToRowHash = createRows();

    // Add data to rows
    var root = groupRoots[selectedRootIndex];
    var primaryDataList = loaded.primaryDataValues.getData(root);
    var supportingDataList = loaded.supportingDataValues[MT.model.areaTypeId];

    var isPrimaryData = primaryDataList.length > 0;
    var isSupportingData = supportingDataList.length > 0;

    var dataList = isPrimaryData
        ? primaryDataList
        : supportingDataList;

    for (var i in dataList) {

        var areaCode = dataList[i].AreaCode;
        row = areaCodeToRowHash[areaCode];

        if (row) {
            row.PrimaryData = isPrimaryData
                ? primaryDataList[i]
                : null;

            // Assign supporting data
            var supportingData = null;
            if (isSupportingData) {
                var coreData = _.find(supportingDataList, function (data) { return data.AreaCode === areaCode });
                supportingData = coreData;
            }
            row.SupportingData = supportingData;
        }
    }
}

function initView() {
    var state = rankingsState;

    if (!state.isInitialised) {
        state.isInitialised = 1;
        displayInfoBoxes();
    }
}

function displayInfoBoxes() {

    var supportingAreaData = loaded.supportingAreaData.getData(
        {
            profileId: SupportingProfileId,
            groupId: SupportingGroupId,
            areaCode: MT.model.parentCode
        });
    var ranks = supportingAreaData.Ranks[NATIONAL_CODE];

    switch (MT.model.profileId) {
        case ProfileIds.Suicide:
            displayInfoBox1(ranks[1]);
            displayInfoBox2(ranks[2]);
            break;
        case ProfileIds.HealthChecks:
            displayInfoBox1(ranks[1]);
            break;
        case ProfileIds.DrugsAndAlcohol:
            // Content defined in FPM content manager
            break;
        default:
            displayInfoBox1(ranks[1]);
            displayInfoBox2(ranks[0]);
    }
}

function displayInfoBox1(rank) {

    var html = templates.renderOnce(
        '<h2>Population of <span class="area_name"></span></h2><p><span>{{count}}</span></p>',
        {
            count: new CommaNumber(rank.AreaRank.Count).rounded()
        });

    $('#info_box_1').html(html);
}

function displayInfoBox2(rank) {

    var count, template;
    var templateModel = {};

    count = isDefined(rank.AreaRank)
        ? new CommaNumber(rank.AreaRank.Count).rounded()
        : 'Data unavailable';
    template = '<h2>Adults in <span class="area_name"></span> with {{condition}}';
    templateModel.condition = '???';

    templateModel.period = rank.Period;
    templateModel.count = count;

    $('#info_box_2').html(templates.renderOnce(template + '<p><span>{{count}}</span> {{period}}</p>',
        templateModel)).show();
}

function setPrimaryDataHeader(metadata) {

    if (metadata.IID === IndicatorIds.Deprivation) {
        var columnHeader = getDeprivationColumnHeader();
    } else {
        var root = groupRoots[selectedRootIndex];
        columnHeader = new ColumnHeader(metadata, root).text;
    }

    $('#value_type_heading').html(columnHeader +
        '<i style="right: -1.3em;"></i>'/*sorted by triangle*/);
}

function assignDataLabelsToRows(primaryMetadata, supportingMetadata) {

    getNewRowsWithCoreData();
    var rows = rankingsState.rows;

    var isSupportingDataUnit = isSupportingDataUnitRequired();
    var isDeprivation = isDeprivationSelected();

    var sortedColumn = rankingsState.sortedColumn;
    var sortOnPrimaryData = sortedColumn === 2;
    var sortOnSecondaryData = sortedColumn === 1;

    var root = groupRoots[selectedRootIndex];
    var gradeFunction = getGradeFunctionFromGroupRoot(root);

    for (var i in rows) {
        var row = rows[i];
        var valToSortOn = -1;

        // Primary data
        if (row.PrimaryData) {
            var data = row.PrimaryData;
            var dataInfo = new CoreDataSetInfo(data);
            if (dataInfo.isValue()) {
                row.primaryDataText = getDataText(data, root.IID === IndicatorIds.Deprivation);
                row.unitLabel = new UnitFormat(primaryMetadata, data.Val).getLabel();
                if (sortOnPrimaryData) {
                    valToSortOn = data.Val;
                }
                row.grade = gradeFunction(data.Sig[-1], root);
            } else {
                row.primaryDataText = 'No data';
                row.unitLabel = '';
                row.grade = gradeFunction(0, root);
            }

            // Add value note
            if (!dataInfo.isNote() || !dataInfo.isValue()/*temporarily hide value note because line is too long*/) {
                row.primaryValueNote = '';
            } else {
                row.primaryValueNote = '<span class="tooltip primary-value-note-tooltip"><i>' + getValueNoteText(data.NoteId) + '</i></span>';
                if (data.NoteId === ValueNoteIds.DataQualityIssue) {
                    // Don't display colour
                    row.grade = 'grade-99';
                }
            }
        } else {
            row.primaryValueNote = '';
            row.primaryDataText = 'No data';
            row.unitLabel = '';
            row.grade = gradeFunction(0, root);
        }

        // Supporting data
        if (row.SupportingData) {
            var supportingData = row.SupportingData;

            var dataInfo = new CoreDataSetInfo(supportingData);
            if (dataInfo.isValue()) {
                row.supportingDataText = getDataText(supportingData, isDeprivation);
                row.supportingGrade = supportingData.Sig[-1];
                row.supportingDataUnitLabel = isSupportingDataUnit
                    ? new UnitFormat(supportingMetadata, supportingData.Val).getLabel()
                    : '';
                if (sortOnSecondaryData) {
                    valToSortOn = supportingData.Val;
                }
            } else {
                row.supportingDataText = 'No data';
                row.supportingDataUnitLabel = '';
                row.supportingGrade = gradeFunction(0, root);
            }

            if (!dataInfo.isNote()) {
                row.supportingValueNote = '';
            } else {
                row.supportingValueNote = '<span id="supporting-value-note-tooltip" class="tooltip"><i>' +
                    getValueNoteText(supportingData.NoteId) + '</i></span>';
            }

        } else {
            row.supportingValueNote = '';
            row.supportingDataText = 'No data';
            row.supportingGrade = 0;
            row.supportingDataUnitLabel = '';
        }
        row.supportingSignificanceImage = getSupportingGrade(row.supportingGrade);

        row.ValToSortOn = valToSortOn;
    }
}

function getDataText(data, isDeprivation) {
    if (isDeprivation) {
        return deprivationDefinitions[data.Sig[-1]];
    }

    return data.ValF;
}

function getSupportingGrade(grade) {

    // Middle column
    var groupRoot = supportingGroupRoots[selectedSupportingGroupRootIndex];
    var gradeFunction = getGradeFunctionFromGroupRoot(groupRoot);
    var gradeName = gradeFunction(grade, groupRoot);

    return '<img class="tooltip" id="sigTooltip-' + Math.floor(Math.random() * 100) +
        '" src="' + FT.url.img + 'Mortality/' + gradeName + '.png"  onmouseover="showSigTooltip(event,' + grade + ',0)" onmouseout="hideSigTooltip()"/>';
}

function getCompareLinkHtml(areaCode) {
    if (rankingsState.comparisonIsAvailable) {
        return '<div class="compare-link"><a href="#' + areaCode + '" onclick="selectSimilarAreas(\''
            + areaCode + '\', this);">Compare similar</a></div>';
    }

    return '';
}

function setSupportingDataHeader(metadata) {
    if (metadata.IID === IndicatorIds.Deprivation) {
        var columnHeader = getDeprivationColumnHeader();
    } else {
        columnHeader = new ColumnHeader(metadata).text;
    }

    // Set column heading
    $('#supporting_data_heading').html('<a href="javascript:sortRankings(1);">' +
        columnHeader + '<i></i></a>');
}

function isDeprivationSelected() {
    return getSelectedSupportingIndicatorId() === IndicatorIds.Deprivation;
}

function isSupportingDataUnitRequired() {
    return !isDeprivationSelected(); // no unit for deprivation labels
}

function reloadData() {

    displayColumnHeadersAndAddDataToRows();

    var rows = rankingsState.rows;
    if (MT.model.similarAreaCode) {
        rows = getSimilarAreaRows(rows);
    }

    displayTable(rows);
    displayParentAreaName();

    unlock();
}

function getCategoryFromCategoryArea(code) {
    return parseInt(code.split('-')[2]);
}

function getArea() {
    var model = MT.model;
    var areaList = loaded.areaLists[model.areaTypeId][model.parentCode];
    return areaList[model.areaCode];
}

function getSimilarAreaRows(rows) {
    var model = MT.model;
    var areaTypeId = model.areaTypeId;
    var areaCode = model.areaCode;
    var areaName = getArea().Name;

    $('#comparing_practice_name').html(areaName);
    toggleComparisonHeader();

    var similarRows = [];

    // Set decile or neighbours if needed
    var decileToShow, areaCodesToKeep;
    var useNeighbours = isNearestNeighbour();
    if (model.similarAreaCode) {
        if (useNeighbours) {
            // Copy array of neighbour codes
            areaCodesToKeep = loaded.neighbours[areaCode].slice();

            // Add selected area so included in table
            areaCodesToKeep.push(areaCode);
        } else if (doesAreaTypeCompareToOnsCluster()) {
            areaCodesToKeep = loaded.onsClusterCodes[model.similarAreaCode];
        } else {
            decileToShow = getCategoryFromCategoryArea(model.similarAreaCode);
        }
    }

    _.each(rows, function (row) {

        switch (areaTypeId) {
            case AreaTypeIds.CountyUA:
                if (useNeighbours) {
                    // Get nearest neighbours
                    if (_.contains(areaCodesToKeep, row.Code)) {
                        similarRows.push(row);
                    }
                } else {
                    // Get LAs in same decile
                    var decile = getDecileInfo(loaded.categories[AreaTypeIds.DeprivationDecile][row.Code]);
                    if (decileToShow === decile.decile) {
                        similarRows.push(row);
                    }
                }
                break;
            case AreaTypeIds.DistrictUA:
                if (_.contains(areaCodesToKeep, row.Code)) {
                    similarRows.push(row);
                }
                break;
            case AreaTypeIds.CCGPreApr2017:
                // Get CCGS in same decile
                var decileNumber = loaded.categories[AreaTypeIds.DeprivationDecile][row.Code];
                if (decileToShow === decileNumber) {
                    similarRows.push(row);
                }
                break;
            default:
                throw new Error('Similar areas not supported for current area type');
        }
    });

    return similarRows;
}

function toggleComparisonHeader() {
    var $similar = $('.similar');
    var $nonSimilar = $('.non-similar');
    if (MT.model.similarAreaCode) {
        $nonSimilar.hide();
        $similar.show();
    } else {
        $nonSimilar.show();
        $similar.hide();
    }
}

function sortRankings(columnIndex) {

    changeSortState(columnIndex);
    displaySortArrowOnTableHeader(columnIndex);

    if (MT.model.similarAreaCode) {
        reloadData();
        hideCompareSimilarLink();
    } else {
        displayPage();
    }
}

function changeSortState(columnIndex) {

    var state = rankingsState;
    var currentColumn = state.sortedColumn;

    if (currentColumn !== columnIndex) {
        // Change sorted column   
        state.sortedColumn = columnIndex;
        state.sortAscending = true;
    } else {
        // Change sort order
        state.sortAscending = !state.sortAscending;
    }
}

function displaySortArrowOnTableHeader(columnIndex) {
    var $headers = $('#diabetes-rankings-table TH');

    // Remove all sorted classes
    $headers.removeClass('sorted sorted-desc');

    // Apply style to selected header
    var $column = $($headers[columnIndex]);
    $column.addClass('sorted');
    if (!rankingsState.sortAscending) {
        $column.addClass('sorted-desc');
    }
}

function sortDisplayedRows(rows) {

    var sortAscending = rankingsState.sortAscending;
    var sortedColumn = rankingsState.sortedColumn;

    if (sortedColumn === 0) {
        // Sort on area name
        rows.sort(sortAreasByName);
    } else if (sortedColumn === 2) {
        // Sort on supporting data
        rows.sort(function (a, b) {
            return a.ValToSortOn - b.ValToSortOn;
        });
    } else if (sortedColumn === 1) {
        // Sort on Totals if last column is selected
        rows.sort(function (a, b) {
            return a.ValToSortOn - b.ValToSortOn;
        });
    }

    if (!sortAscending) {
        rows.reverse();
    }
}

function sortCategory(a, b) {
    return sortString(a.supportingDataText, b.supportingDataText);
};

function sortByPolarity(rowsToDisplay, polarity) {
    if (polarity !== 0) {
        rowsToDisplay.reverse();
    }
}

function displayNearestNeighbours() {

    getNearestNeighbours(function () {
        // Update text header of rankings table
        $('#comparison_category').html('(CIPFA nearest neighbours)');
        $('#comparing_area_type').html('to similar areas');
        $('#comparison_type').html('Nearest Neighbours');

        scrollToTop();
        reloadData();
        hideCompareSimilarLink();
    });
}

function getDecileFromAreaCode(areaCode) {
    var decileLookup = loaded.categories[AreaTypeIds.DeprivationDecile];
    return decileLookup[areaCode];
}

function displaySimilarAreas(areaCode) {

    if (doesAreaTypeCompareToOnsCluster()) {
        // Set model
        MT.model.similarAreaCode = getOnsCodeForArea(areaCode);
        MT.model.areaCode = areaCode;

        $('#comparison_category').html('(in same ONS cluster group)');
        $('#comparing_area_type').html('to similar areas');
        $('#comparison_type').html('Similar areas');
    } else {
        var decile = getDecileFromAreaCode(areaCode);

        // Set model
        MT.model.similarAreaCode = getCategoryAreaCode(decile);
        MT.model.areaCode = areaCode;

        // Update text header of rankings table
        $('#comparison_category').html('(in Socioeconomic decile ' + decile + ' - ' + getDecileInfo(decile).text + ')');
        $('#comparing_area_type').html('to similar areas');
        $('#comparison_type').html('Deprivation group');
    }

    scrollToTop();
    reloadData();
    hideCompareSimilarLink();
    displayAreaDetailsLink();
}

function selectNearestNeighbours(areaCode) {

    if (!FT.ajaxLock) {
        lock();

        lightbox.hide();

        MT.model.areaCode = areaCode;
        MT.model.similarAreaCode = getNearestNeighbourCode();

        ftHistory.setHistory();

        updatePage();

        logEvent(AnalyticsCategories.RANKINGS, AnalyticsAction.SIMILAR_AREAS);
    }
}

function selectSimilarAreas(areaCode, element) {

    if (!FT.ajaxLock) {
        lock();

        if (element && doesAreaTypeHaveNearestNeighbours()) {
            // More than one compare similar option so need to show lightbox for user to choose
            showCompareSimilarOptions(areaCode, element);
            return;
        }

        // Hide lightbox if user has had to choose a particular compare option
        lightbox.hide();

        // Set model
        if (doesAreaTypeCompareToOnsCluster) {
            MT.model.similarAreaCode = getOnsCodeForArea(areaCode);
        } else {
            var decile = getDecileFromAreaCode(areaCode);
            MT.model.similarAreaCode = getCategoryAreaCode(decile);
        }
        MT.model.areaCode = areaCode;

        ftHistory.setHistory();

        displaySimilarAreas(areaCode);

        logEvent(AnalyticsCategories.RANKINGS, AnalyticsAction.SIMILAR_AREAS);
    }
}

function showCompareSimilarOptions(areaCode, element) {
    tooltipManager.init();
    var name = loaded.areaLists[MT.model.areaTypeId][MT.model.parentCode][areaCode].Name;
    var position = $(element).offset();
    templates.add('compare', '<div class="compare-popup"><h3>Compare {{name}} with:</h3>\
<a href="javascript:selectNearestNeighbours(\'{{areaCode}}\')">Similar areas</a>\
<a href="javascript:selectSimilarAreas(\'{{areaCode}}\')">Deprivation group</a></div>');
    var html = templates.render('compare', { areaCode: areaCode, name: name });
    lightbox.show(html, position.top - 50, position.left - 220, 300/*width*/);
}

function hideCompareSimilarLink() {
    $('.compare-link').removeClass('show-compare').addClass('hide-compare');
}

function scrollToTop() {
    // Ensure top of table can be seen
    var $window = $(window);
    if ($window.scrollTop() > 600) {
        $window.scrollTop(260);
    }
}

function changeNumberOrder() {

    var titleOrder = 1;
    var riskOrder = 2;

    if (MT.model.areaTypeId !== AreaTypeIds.Practice && _.size(loaded.areaTypes) > 1) {
        titleOrder++;
        riskOrder++;
    }

    $('#indicator-title-order').text(titleOrder + '.');
    $('#prevalence-risk-order').text(riskOrder + '.');
}

function goToPracticeProfiles() {
    var url = 'http://fingertips.phe.org.uk/profile/general-practice/data#mod,2,pat,19,par,' +
        MT.model.parentCode + ',are,-,sid1,2000005,ind1,-,sid2,-,ind2,-';
    window.open(url);
}

function setSelectedPrimaryDomain(groupId) {
    // Display new active options
    var cssActiveClass = 'active';

    // Hide current indicator list
    var indicatorGroups = $('.filters .causes');
    indicatorGroups.hide();

    // Remove selected from current indicator
    $('.filters li').removeClass(cssActiveClass);

    // Select new domain
    $('#domain-' + groupId).addClass(cssActiveClass);

    // Show list of indicators in selected domain
    var $currentList = $('#diabetes_list-' + groupId);
    $currentList.show();
    $currentList.addClass(cssActiveClass);
}

function getSelectedSupportingIndicatorId() {
    return supportingGroupRoots[selectedSupportingGroupRootIndex].IID;
}

function createRows() {

    // Define rows
    var rows = rankingsState.rows = [];
    var areaCodeToRowHash = {};
    var areas = loaded.areaLists[MT.model.areaTypeId][MT.model.parentCode];
    for (var i in areas) {

        var areaCode = areas[i].Code;

        var row = {
            Name: areas[areaCode].Name,
            Code: areas[areaCode].Code,
            compareSimilar: getCompareLinkHtml(areaCode)
        };

        areaCodeToRowHash[areaCode] = row;
        rows.push(row);
    }

    return areaCodeToRowHash;
}

function displayColumnHeadersAndAddDataToRows() {

    var model = MT.model;

    var primaryMetadataHash = loaded.indicatorMetadata[model.groupId];
    var supportingMetadataHash = loaded.indicatorMetadata[selectedSupportingGroupId];

    var primaryMetadata = primaryMetadataHash[model.indicatorId];
    var supportingMetadata = supportingMetadataHash[getSelectedSupportingIndicatorId()];

    setPrimaryDataHeader(primaryMetadata);
    setSupportingDataHeader(supportingMetadata);

    assignDataLabelsToRows(primaryMetadata, supportingMetadata);
}

function ColumnHeader(metadata, root) {

    var unit = metadata.Unit;
    var unitLabel = unit.Id === 5
        ? '' :
        unit.Label;

    var sexLabel = '';
    if (root && root.StateSex) {
        sexLabel = ' (' + root.Sex.Name + ')';
    }

    this.text = replacePercentageWithArialFont(metadata.Descriptive.Name) + sexLabel +
        '<small>' + unitLabel + '</small>';
}

function getDeprivationColumnHeader() {

    var model = MT.model;
    var parentAreaType = model.areaTypeId === AreaTypeIds.Practice
        ? model.parentAreaType
        : AreaTypeIds.Country;

    return getDeprivationLabel('Deprivation', parentAreaType);
}

function showSigTooltip(e, grade, col) {

    var groupRoot;

    if (col == 0) {
        groupRoot = supportingGroupRoots[selectedSupportingGroupRootIndex];
    } else {
        groupRoot = groupRoots[selectedRootIndex];
    }

    if (grade > 0) {
        var text = getSignificanceText(grade, groupRoot);
        var html = '<div class="tooltip-two" ><div>' + text + '</div></div>';
        tooltipManager.show(e, html);
    }
}

function hideSigTooltip() {
    $('.tooltip-two').hide();
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
                ).add('profile_id', modelCopy.profileId
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


templates.add('causes',
    '{{#causes}}<li id={{index}}-iid-{{id}} class="{{cssClass}}"><a href="javascript:selectPrimaryIndicator({{index}})">{{{name}}}</a></li>{{/causes}}');

templates.add('prevandriskcauses',
    '{{#causes}}<li id=prev-{{id}} class="{{cssClass}}"><a href="javascript:selectSupportingIndicator({{index}})">{{{name}}}</a></li>{{/causes}}');

templates.add('rows',
    '{{#showEnglandVal}}<tr class="england-row"><td><span class="england-label">England</span></td><td><span class="england-val">{{englandValCol1}}{{{col1Unit}}}</span></td><td><span class="england-val">{{englandValCol2}}{{{col2Unit}}}</span></td></td>{{/showEnglandVal}}\
    {{#rows}}<tr class="odd {{selected}}"><td>\
    <a href="javascript:selectRankingsArea(\'{{Code}}\')">{{Name}}</a></td>\
    <td><span class="grade grade-quintile-{{supportingGrade}}">{{{supportingSignificanceImage}}}</span>{{supportingDataText}}<span>{{{supportingDataUnitLabel}}}</span>{{{supportingValueNote}}}</td>\
    <td class="last-child"><span class="grade {{grade}}" onmouseover="showSigTooltip(event, {{primaryIndicatorSig}})" onmouseout="hideSigTooltip()"><img src="' + FT.url.img + 'Mortality/{{grade}}.png" /></span><span style="max-width:170px;">{{primaryDataText}}</span><span>{{{unitLabel}}}</span>{{{primaryValueNote}}}{{{compareSimilar}}}</td></tr>{{/rows}}');

templates.add('bobLegend',
    '<p class="legend">Comparison with {{comparator}} average\
    <span class="grade">\
        <img src="' + FT.url.img + 'Mortality/bobLower.png" alt="worse" />lower\
    </span>\
    <span class="grade">\
        <img src="' + FT.url.img + 'Mortality/grade-2.png" alt="consistent" />consistent\
    </span>\
    <span class="grade">\
        <img src="' + FT.url.img + 'Mortality/bobHigher.png" alt="better" />higher\
    </span>\
</p>');

templates.add('ragAndBobLegend',
    '<p class="legend">Comparison with national average\
    <span class="grade">\
        <img src="' + FT.url.img + 'Mortality/grade-3.png" alt="worse" />worse\
    </span>\
    <span class="grade">\
        <img src="' + FT.url.img + 'Mortality/grade-2.png" alt="consistent" />consistent\
    </span>\
    <span class="grade">\
        <img src="' + FT.url.img + 'Mortality/grade-0.png" alt="better" />better\
    </span>\
</p>' +
    '<p class="legend">Comparison with national average\
    <span class="grade">\
        <img src="' + FT.url.img + 'Mortality/bobLower.png" alt="worse" />lower&nbsp;\
    </span>\
    <span class="grade">\
        <img src="' + FT.url.img + 'Mortality/grade-2.png" alt="consistent" />consistent\
    </span>\
    <span class="grade">\
        <img src="' + FT.url.img + 'Mortality/bobHigher.png" alt="better" />higher\
    </span>\
</p>'

);

// Global state of supporting indicator selection
isSupportingIndicatorSelected = false;
selectedSupportingGroupRootIndex = -1;
selectedSupportingGroupId = -1;

loaded.supportingDataValues = {};
loaded.primaryDataValues = new AreaValuesDataManager();
loaded.practiceValues = new AreaValuesDataManager();
loaded.specificDomainAreaDetails = new AreaDetailsDataManager();
loaded.groupDataAtDataPoint = new GroupDataAtDataPointOfSpecificAreasDataManager();