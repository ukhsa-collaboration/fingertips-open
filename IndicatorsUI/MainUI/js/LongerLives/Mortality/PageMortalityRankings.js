function initPage() {

    updateModelFromHash();
    ftHistory.setHistory();

    if (!String.isNullOrEmpty(onsClusterCode)) {
        MT.model.parentCode = onsClusterCode;
    }

    var model = MT.model,
    isSimilar = isSimilarAreas();

    if (model.areaTypeId === AreaTypeIds.CountyUA) {
        ajaxMonitor.setCalls(isSimilar ? 7 : 6);
    } else {
        ajaxMonitor.setCalls(isSimilar ? 8 : 6);
    }

    loaded.areaDetails.fetchDataByAjax({ areaCode: NATIONAL_CODE });
    getDecileData(model);
    getAllAreas(model);
    getIndicatorMetadata(model.groupId);
    getGroupRoots(model);
    getAreaTypes();

    if (isSimilar) {

        loaded.areaDetails.fetchDataByAjax({ areaCode: model.parentCode });
        if (model.areaTypeId !== AreaTypeIds.CountyUA) {
            loaded.areaDetails.fetchDataByAjax();
        }
    }

    ajaxMonitor.monitor(getSecondaryData);

}

function displayPage() {
    
    initView();
    setValueTypeHeading();
    
    var model = MT.model,
    similarAreaCode = model.areaCode,
    groupRoot = groupRoots[selectedRootIndex],
    rows = rankingsState.rows,
    areaCodeToRowHash = rankingsState.areaCodeToRowHash,
    isNational = !isSimilarAreas(),
    metadataHash = loaded.indicatorMetadata[model.groupId];
    metadata = metadataHash[groupRoot.IID];
    
    displayAreaRangeElements(isNational);
    
    // Which decile to display - Decile is only used for Upper Tier (102)
    if (model.areaTypeId === AreaTypeIds.CountyUA) {
        decile = getDecileLookUp()[similarAreaCode];
    }

    if (!isNational) {
        // Update area name
        var name = areaCodeToRowHash[similarAreaCode].Name;
        $('#area-name').html(name);
    }

    initSupportingData(areaCodeToRowHash, rows);
    
    // Cause name
    $('#indicator-name').html('- ' + metadata.Descriptive.Name);
    
    var comparatorAreaCode = getCurrentComparator().Code;
    
    // Cause data
        dataList = loaded.areaValues[getIndicatorKey(groupRoot, model,comparatorAreaCode)];
        setRowData(dataList, areaCodeToRowHash, rows, selectedRootIndex, 
            function(data) {return data.ValF;});
    
    // Deprivation 
    var deprivationIndex = ROOT_INDEXES.DEPRIVATION
    if (rankingsState.showDeprivation && !rows[0][deprivationIndex]) {
        dataList = loaded.areaValues[getIndicatorKey(groupRoots[deprivationIndex], model,NATIONAL_CODE)];
        setRowData(dataList, areaCodeToRowHash, rows, deprivationIndex, function() {return null;});
    }

    var parentValue;
    // Mortality
    var isSimilar = isSimilarAreas();
    if (isSimilar) {
        if (model.areaTypeId === AreaTypeIds.CountyUA) {
            parentValue = getParentDetails(MT.model.parentCode, NATIONAL_CODE, selectedRootIndex).Val;
            $('#comparator-name').html('the same Socioeconomic deprivation bracket');
        } else {
            var onsClusterName = getOnsClusterName(MT.model.parentCode);
            $('#comparator-name').html(onsClusterName);
            parentValue = loaded.areaDetails.getData().Benchmarks[model.parentCode][selectedRootIndex].Val;
        }

        $('#similar-areas-tooltip').html(getSimilarAreaTooltipText(model.areaTypeId));
    } else {
        parentValue = getParentDetails(MT.model.parentCode, NATIONAL_CODE, selectedRootIndex).Val;
    }

    gradeFunction = getGradeFunction(parentValue);

    // Assign data for display & choose rows to display
    var rowsToDisplay = [];
    var useRate = rankingsState.valueType === 0;
    for (var i in rows) {
        
        var row = rows[i];
        
        var cause = row[selectedRootIndex];
        
        // Should row be included in table
        var isShown = cause && cause.Val > -1;
        if (!isNational && model.areaTypeId === AreaTypeIds.CountyUA) {
            isShown = isShown && decile === row.dep.decile;
        }
        
        // Whether or not the row is selected
        row.selected = '';
        if (!isNational && row.Code === similarAreaCode) {
            row.selected = 'chosen';
        }
        
        if (isShown) {
            
            // Mortality rate or total
            row.col2 = useRate ?
                cause.ValF :
                cause.CountF;
            
            // RAG
            row.grade = gradeFunction(cause.Sig[comparatorId], cause.Val);
            
            // Whether all areas are displayed
            row.isNat = isNational;
            
            rowsToDisplay.push(row);
        }
    }
    
    // Sort data by before rank can be assigned
    rowsToDisplay.sort(function (a, b) {
            return a[selectedRootIndex].Val - b[selectedRootIndex].Val;
    });
    
    assignRank(rowsToDisplay);
    sortDisplayedRows(rowsToDisplay);
    
    // Render data
    $('#mortality-rankings-table tbody').html(
        templates.render('rows', { rows: rowsToDisplay })
    );
    
    $('#la_count').html(rowsToDisplay.length);
    
    unlock();
}

function getSecondaryData() {
    var model = MT.model;

    if (rootIndexesToGet.length) {
        populateAreaTypes(model);

        ajaxMonitor.setCalls(1);
        
        var index = rootIndexesToGet[0];
        
        var code = (index === ROOT_INDEXES.POPULATION || index === ROOT_INDEXES.DEPRIVATION) ?
            NATIONAL_CODE : 
            getCurrentComparator().Code;
        
        // One after the other so know which index in callback
        getAreaValues(groupRoots[index], MT.model, code);
        
        ajaxMonitor.monitor(getSecondaryData);
    } else {
        displayPage();
    }
}

function populateAreaTypes(model) {

    if (model.areaTypeId !== PRACTICE) {

        var templateName = 'areaFilter';

        templates.add(templateName, '<h5>Select Area</h5><ul class="areaFilters">\
        {{#types}}<li id="{{Id}}" class="areaFilter {{class}}"><a href="javascript:switchAreas({{Id}});">{{Short}}</a></li>{{/types}}' + '<div class="hr"><hr /></div>');

        setAreaTypeOptionHtml(templateName);

    } else {
        $('#area-filter').hide();
    }
}

function switchAreas(parentAreaTypeId) {
    setUrl('/topic/' + profileUrlKey + '/comparisons#par/' + NATIONAL_CODE + '/ati/' + parentAreaTypeId + '/pat/' + parentAreaTypeId);
    window.location.reload();
}

function populateCauseList() {
    $('#mortality_list').html(templates.render('causes'));
}

function selectIndicator(rootIndex, name) {

    if (!ajaxLock) {
        lock();

        setActiveOption('mortality_list', rootIndex - 1);

        selectedRootIndex = rootIndex;

        rootIndexesToGet = [rootIndex];

        getSecondaryData();

        logEvent(AnalyticsCategories.RANKINGS, 'IndicatorSelected', name);
    }
}

//
// Page state
//
var rankingsState = {
    // Row objects that are fed into a template for generating table row HTML
    rows: [],
    // Key(area code)/value(a row) pair list to enable easy look up of a row
    areaCodeToRowHash: {},
    // Whether the view has been initialised
    isInitialised: false,
    // Whether rates or totals area displayed
    valueType: 0,
    // Whether population or deprivation deciles are displayed
    showDeprivation: false,
    sortAscending: true,
    sortedColumn: 0,
    // Whether or not the URL parameters have already been parsed,
    isHashRestored: false
}

function selectArea(code) {
    // Log to google analytics
    var areas = loaded.areaLists[MT.model.areaTypeId];
    logEvent(AnalyticsCategories.RANKINGS, 'AreaSelected', areas[code].Name);

    var model = MT.model;
    model.areaCode = code;
    MT.nav.areaDetails(model);
}

function setRowData(dataList, areaCodeToRowHash, rows, property, valueFormatFunction) {

    for (var i in dataList) {

        var data = dataList[i];

        var row = areaCodeToRowHash[data.AreaCode];

        if (row) {

            var count = parseInt(data.Count);

            row[property] = {
                Val: data.Val,
                ValF: valueFormatFunction(data),
                CountF: new CommaNumber(count).rounded(),
                Count: count,
                Sig: data.Sig
            };
        }
    }
}

function selectValueType(type) {
    rankingsState.valueType = type;
    setActiveOption('value_types', type);
    displayPage();
    logEvent(AnalyticsCategories.RANKINGS, 'ValueTypeSelected',
        type === 0 ? 'Rate' : 'Count');
}

function initTableRows() {

    var rows = rankingsState.rows;
    var areaCodeToRowHash = rankingsState.areaCodeToRowHash;

    var areas = loaded.areaLists[MT.model.areaTypeId];

    for (var areaCode in areas) {

        var row = {
            Name: areas[areaCode].Name,
            Code: areaCode
        };

        rows.push(row);

        areaCodeToRowHash[areaCode] = row;
    }
}

function initView() {

    var state = rankingsState,
    parentCode, val, period;

    var isSimilar = isSimilarAreas();

    if (!state.isInitialised) {

        state.isInitialised = 1;

        populateCauseList();
        initTableRows();

        // National overall mortality
        val = getParentDetails(NATIONAL_CODE, NATIONAL_CODE, ROOT_INDEXES.OVERALL_MORTALITY).Count;
        period = getParentPeriod(NATIONAL_CODE, NATIONAL_CODE, ROOT_INDEXES.OVERALL_MORTALITY);

        $('#national-overall').html(new CommaNumber(val).rounded());
        $('.national_overall_period').html(period);

        // National population
        val = getParentDetails(NATIONAL_CODE, NATIONAL_CODE, ROOT_INDEXES.POPULATION).Val;
        $('#national-population').html(new CommaNumber(val).rounded());
    }
}

function setValueTypeHeading() {
    var valueTypeHeading = rankingsState.valueType === 0 ?
        'Premature deaths <small>per 100,000</small>' :
        'Total premature deaths<small>' + getParentPeriod(NATIONAL_CODE, NATIONAL_CODE, ROOT_INDEXES.OVERALL_MORTALITY) + '</small>';
    $('#value_type_heading').html(valueTypeHeading + '<i></i>');
}

/*
* Set which type of supporting data is displayed. Either population or deprivation.
*/
function selectSupportingData(optionIndex) {

    if (!ajaxLock) {
        lock();

        var showDeprivation = optionIndex === 1;

        if (showDeprivation) {
            rootIndexesToGet = [ROOT_INDEXES.DEPRIVATION];
        }

        rankingsState.showDeprivation = showDeprivation;

        setActiveOption('supporting_data', optionIndex);

        initPage();

        logEvent(AnalyticsCategories.RANKINGS, 'SupportingDataSelected',
            optionIndex === 0 ? 'Population' : 'Deprivation');
    }
}

function initSupportingData(areaCodeToRowHash, rows) {

    // Init deprivation 
    if (MT.model.areaCode || rankingsState.showDeprivation) {

        var deprivationKey = 'dep';

        // Init deprivation decile for all areas
        if (!rows[0][deprivationKey]) {
            var dataList = getDecileLookUp();

            for (var areaCode in dataList) {

                var row = areaCodeToRowHash[areaCode];

                if (row) {
                    row[deprivationKey] = getDecileInfo(dataList[areaCode]);
                }
            }
        }
    }

    if (rankingsState.showDeprivation) {
        // Deprivation displayed
        var key = deprivationKey,
        col1Property = 'text',
        columnHeader = 'Deprivation';

    } else {
        //Population displayed
        key = '0';
        col1Property = 'ValF';
        columnHeader = 'Population';

        // Init population for all areas once
        if (!rows[0][key]) {

            var indicatorKey = getIndicatorKey(groupRoots[ROOT_INDEXES.POPULATION],MT.model, NATIONAL_CODE),
            dataList = loaded.areaValues[indicatorKey];
            setRowData(dataList, areaCodeToRowHash, rows, key, 
                function(data) {
                    // Format population
                    return new CommaNumber(data.Val).rounded();
                });
        }
    }

    // Set values to display in supporting data column
    for (var i in rows) {
        row = rows[i];
        if (row[key] && row[key][col1Property]) {
        	row.col1 = row[key][col1Property];
        }
    }

    // Set column heading
    $('#supporting_data_heading').html('<a href="javascript:sortRankings(2);">' + 
            columnHeader + ' <i></i></a>');
}

function sortRankings(columnIndex) {


    var headers = $('#mortality-rankings-table TH');

    // Remove all sorted classes
    headers.removeClass('sorted sorted-desc');

    // Change sort order
    var currentColumn = rankingsState.sortedColumn;
    if (currentColumn !== columnIndex) {
        // Change sorted column   
        rankingsState.sortedColumn = columnIndex;
        rankingsState.sortAscending = true;
    } else {
        // Change sort order
        rankingsState.sortAscending = !rankingsState.sortAscending;
    }

    // Apply style to selected header
    var columnJq = $(headers[columnIndex]);
    columnJq.addClass('sorted');
    if (!rankingsState.sortAscending) {
        columnJq.addClass('sorted-desc');
    }

    displayPage();
}

function sortDisplayedRows(rowsToDisplay) {

    var sortAscending = rankingsState.sortAscending;
    var sortedColumn = rankingsState.sortedColumn;
    if (sortedColumn === 1) {
        // Sort on area name
        rowsToDisplay.sort(sortAreasByName);
    } else if (sortedColumn === 2) {
        // Sort on supporting data

        if (rankingsState.showDeprivation) {

            rowsToDisplay.sort(function (a, b) {
                return a[6].Val - b[6].Val;
            });
        } else {
            // Sort on population
            rowsToDisplay.sort(function (a, b) {
                return a[0].Val - b[0].Val;
            });
        }

    } else {
        // Sort on cause
        if (rankingsState.valueType === 1 && sortedColumn === 3) {
            // Sort on Totals if last column is selected
            rowsToDisplay.sort(function (a, b) {
                return a[selectedRootIndex].Count - b[selectedRootIndex].Count;
            });
        }
    }

    if (!sortAscending) {
        rowsToDisplay.reverse();
    }
}

function assignRank(rowsToDisplay) {
    var rank = 1;
    for (var i in rowsToDisplay) {
        var row = rowsToDisplay[i];
        row.rank = rank++;
    }
}

function selectSimilarAreas(areaCode) {
    if (!ajaxLock) {
        lock();
        

        comparatorId = DEPRIVATION_DECILE_COMPARATOR_ID;
        MT.model.areaCode = areaCode;
        MT.model.parentCode = getDecileCode(getDecileLookUp()[areaCode]);

        ajaxMonitor.setCalls(MT.model.areaTypeId !== AreaTypeIds.CountyUA ? 1 : 0);

        if (MT.model.areaTypeId !== AreaTypeIds.CountyUA) {
            getOnsClusterCode(MT.model);
            rankingsState.isInitialised = false;
            rankingsState.rows = [];
        }

        // Ensure top of table can be seen
        if ($(window).scrollTop() > 600) {
            $(window).scrollTop(260);
        }

        rootIndexesToGet = [selectedRootIndex];

        if (MT.model.areaTypeId !== AreaTypeIds.CountyUA) {
            ajaxMonitor.monitor(waitForOnsCode);
        } else {
            initPage();
        }

        logEvent(AnalyticsCategories.RANKINGS, AnalyticsAction.SIMILAR_AREAS);
    }
}

function waitForOnsCode() {
    MT.model.parentCode = onsClusterCode;
    initPage();
}

function getDecileLookUp() {
    return loaded.categories[AreaTypeIds.DeprivationDecile];
}

function showAllAreas() {
    if (!ajaxLock) {
        lock();

        comparatorId = NATIONAL_COMPARATOR_ID;
        MT.model.areaCode = null;
        MT.model.parentCode = NATIONAL_CODE;

        rootIndexesToGet = [selectedRootIndex];
        onsClusterCode = '';

        initPage();

        logEvent(AnalyticsCategories.RANKINGS, AnalyticsAction.ALL_AREAS);
    }
}
// List of group root indexes for which area values are to be fetched by AJAX
rootIndexesToGet = [
    ROOT_INDEXES.POPULATION,
    ROOT_INDEXES.OVERALL_MORTALITY];

templates.add('causes',
    '<li class="active"><a id="Overall-Premature" href="javascript:selectIndicator(1, \'Overall premature deaths\')">Overall premature deaths</a></li>\
    <li class=""><a id="Overall-Cancer" href="javascript:selectIndicator(2, \'Cancer\')">Cancer</a></li>\
    <li class="sub"><a id="Overall-Lung-Cancer" href="javascript:selectIndicator(3, \'Lung cancer\')">Lung cancer (all ages)</a></li>\
    <li class="sub"><a id="Overall-Breast-Cancer" href="javascript:selectIndicator(4, \'Breast cancer\')">Breast cancer</a></li>\
    <li class="sub"><a id="Overall-Colorectal-Cancer" href="javascript:selectIndicator(5, \'Colorectal cancer\')">Colorectal cancer</a></li>\
    <li class=""><a id="Overall-Heart-Disease-And-Stroke" href="javascript:selectIndicator(6, \'Heart disease and stroke\')">Heart disease and stroke</a></li>\
    <li class="sub"><a id="Overall-Heart-Disease" href="javascript:selectIndicator(7, \'Heart disease\')">Heart disease</a></li>\
    <li class="sub"><a id="Overall-Stroke" href="javascript:selectIndicator(8, \'Stroke\')">Stroke</a></li>\
    <li class=""><a id="Overall-Lung-Disease" href="javascript:selectIndicator(9, \'Lung disease\')">Lung disease</a></li>\
    <li class=""><a id="Overall-Liver-Disease" href="javascript:selectIndicator(10, \'Liver disease\')">Liver disease</a></li>\
    <li class=""><a id="Overall-Injury" href="javascript:selectIndicator(11, \'Injury\')">Injury</a></li>');

templates.add('rows',
'{{#rows}}<tr class="odd {{selected}}"><td>{{rank}}</td><td><span class="grade grade-{{grade}}"><img src="' + FT.url.img + 'Mortality/grade-{{grade}}.png" />\
<a href="javascript:selectArea(\'{{Code}}\')">{{Name}}</a></td>\
<td>{{col1}}</td><td class="last-child"><span>{{col2}}</span>{{#isNat}}<a href="javascript:selectSimilarAreas(\'{{Code}}\');">Compare similar</a>{{/isNat}}</td></tr>{{/rows}}');

