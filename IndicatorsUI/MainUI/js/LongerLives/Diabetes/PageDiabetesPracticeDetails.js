function initPage() {
    lock();
    showLoadingSpinner();
    updateModelFromHash();
    setSearchText(MT.model.parentAreaType);
}

function updatePage() {

    selectedRootIndex = 0;
    MT.model.groupId = groupIds[0];

    resetState();

    var model = MT.model,
      areaCode = model.areaCode,
        groupIdsString = groupIds.join(',');

    ajaxMonitor.setCalls(7);

    getNhsId(model);
    getIndicatorMetadata(groupIdsString);
    getAreaAddress(model.areaCode);

    getPracticeData(groupIdsString + ',' + SupportingGroupId, areaCode, AreaTypeIds.Practice);
    getCCGData(groupIdsString, model.parentCode, [NATIONAL_CODE], model.areaTypeId);
    getEnglandPrimaryData(groupIdsString, NATIONAL_CODE, AreaTypeIds.Practice);

    getAllAreas(model);

    ajaxMonitor.monitor(displayPage);
}

function getSecondaryData() {
    var model = MT.model;

    ajaxMonitor.setCalls(groupIds.length);

    for (var i = 0; i < groupIds.length; i++) {
        loaded.areaDetails.fetchDataByAjax({ 
            areaCode: model.parentCode, 
            parentCode: NATIONAL_CODE, 
            groupId: groupIds[i] 
        });
    }

    ajaxMonitor.monitor(displayPage);
}

function displayPage() {

    initTableRows();
    displayValuesAtTopOfPage();

    var rows = areaDetailsState.rows,
        areaCode = MT.model.areaCode,
        areaAddress = loaded.addresses[areaCode],
        parentCode = MT.model.parentCode,
        practiceArea = loaded.areaLists[MT.model.areaTypeId][parentCode];

    // Area name
    $('.area_name').html(areaAddress.Name);
    $('.practice_is_in').html('This practice is in:');
    $('.practice_in_area').html(practiceArea.Name);

    $('#area-header').html(getSimpleAreaTypeName() + '<br>value');
    $('#outcomes-header').html(getSimpleAreaTypeName());
    $('#comparison-header').html('Compared to the other practices in the ' + getSimpleAreaTypeName());

    $('#area-address').html(getAddressText(areaAddress));

    // Display data table
    var rankingsHtml = isDefined(rows)
        ? templates.render('rows', { rows: rows })
        : 'No data is available for this practice';

    $('#diabetes-rankings-table tbody').html(rankingsHtml);
    removeLoadingSpinner();
    unlock();
}

function setRowData(dataList, areaCodeToRowHash, rows, property, valueFormatFunction) {

    for (var i in dataList) {
        var data = dataList[i],
            row = areaCodeToRowHash[data.AreaCode];

        if (row) {
            var count = parseInt(row.DiabetesCount);
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

function displayValuesAtTopOfPage() {

    var supportingData = loaded.areaData.getData(SupportingGroupId, MT.model.areaCode, AreaTypeIds.Practice);
    var dataCount = supportingData.length;
    var noDataMessage = NO_DATA;

    // Info box 1
    var count = noDataMessage;
    if (dataCount > 1) {
        // Number with condition
        var numberWithCondition = _.last(supportingData[1].Data);

        if (new CoreDataSetInfo(numberWithCondition).isCount()) {
            count = new CommaNumber(numberWithCondition.Count).rounded();
        }
    }
    var html = templates.renderOnce(
        '<h2>Adults with {{condition}} in this practice</h2><p><span>{{count}}</span></p>',
        {
            condition: getConditionWord(),
            count: count
        });
    $('#info_box_2').html(html);

    // Info box 2
    count = noDataMessage;
    if (dataCount > 2) {
        // Practice population
        var listSize = _.last(supportingData[2].Data);
        if (new CoreDataSetInfo(listSize).isValue()) {
            count = new CommaNumber(listSize.Val).rounded();
        }
    }
    var html = templates.renderOnce(
        '<h2>Practice list size</h2><p><span>{{count}}</span></p>',
        {
            count: count
        });
    $('#info_box_1').html(html);
}

function initTableRows() {

    var model = MT.model;

    var rows = areaDetailsState.rows,
        indicatorMetadataHash = _.values(loaded.indicatorMetadata)[0];

    var areaData = loaded.areaData,
        parentCode = model.parentCode;

    for (var j in groupIds) {
        var groupId = groupIds[j];
        var dataList = areaData.getData(groupId, model.areaCode, AreaTypeIds.Practice),
            parentDataList = areaData.getData(groupId, parentCode, model.areaTypeId),
            nationalDataList = areaData.getData(groupId, NATIONAL_CODE, AreaTypeIds.Practice);

        for (var i in nationalDataList) {
            var data = dataList[i];
            var nationalData = nationalDataList[i];
            var iid = nationalData.IID;
            var metadata = indicatorMetadataHash[iid];

            // Practice data
            var practiceDataIndex = data.Data.length - 1;
            var practiceData = data.Data[practiceDataIndex];
            var practiceFormattedValue;

            // Parent data
            if (parentDataList[i]) {
                var parentDataIndex = parentDataList[i].Data.length - 1;
                var parentData = parentDataList[i].Data[parentDataIndex];
            } else {
                parentDataIndex = parentData = null;
            }
            var parentAreaFormattedValue;

            var gradeFunction = getGradeFunction(nationalData.ComparatorMethodId);

            // Only include rows where area has data
            if (practiceData) {

                var isValidPracticeData = new CoreDataSetInfo(practiceData).isValue();
                var significance = data.Sig[parentCode][practiceDataIndex];

                if (iid === IndicatorIds.Deprivation) {
                    // Practice deprivation
                    practiceFormattedValue = getDeprivationLabel(deprivationDefinitions[significance], 
                        model.parentAreaType);

                    /******** Always ensure that Deprivation is the first indicator 
                    in the prev & risk group for CCG COUNTY/UA *********/
                    var deprivationRootIndex = 0; 

                    // CCG deprivation
                    var parentSignificance = parentDataList[deprivationRootIndex].Sig[NATIONAL_CODE];

                    // Assume always have parent data for deprivation
                    parentAreaFormattedValue = getDeprivationLabel(
                        deprivationDefinitions[parentSignificance[parentDataIndex]]);

                    var nationalValF = 'N/A';
                } else {
                    practiceFormattedValue = isValidPracticeData
                        ? practiceData.ValF
                        : 'No Data';
                    parentAreaFormattedValue = parentData !== null
                        ? parentData.ValF
                        : null;
                    nationalValF = nationalData.Data[0].ValF;
                }

                var row = {
                    Name: replacePercentageWithArialFont(metadata.Descriptive.Name),
                    IID: iid,
                    PracticeValue: practiceFormattedValue,
                    ParentValue: parentAreaFormattedValue,
                    NationalValue: nationalValF,
                    Unit: isValidPracticeData ? new UnitFormat(metadata).getLabel() : '',
                    Grade: gradeFunction(significance)
                };

                rows.push(row);
            }
        }
    }
}

function resetState() {
    areaDetailsState = {
        // Row objects that are fed into a template for generating table row HTML
        rows: [],
        // Key(area code)/value(a row) pair list to enable easy look up of a row
        areaCodeToRowHash: {}
    };
}

function updatePopulation(rank) {

    var html = rank.AreaRank ?
        new CommaNumber(rank.AreaRank.Val).rounded() :
        NO_DATA;

    $('.population_val').html(html);
}

function selectArea(code) {

    if (!FT.ajaxLock) {
        MT.model.areaCode = code;

        initPage();
    }
}

function removeAllGradeClasses(jq) {
    for (var i = 1; i <= 4; i++) {
        jq.removeClass('grade-' + i);
    }
}

function getPracticeData(groupId, areaCode, areaTypeId) {
    getAreaData(groupId, areaCode, [MT.model.parentCode], areaTypeId);
};

function getCCGData(groupId, areaCode, comparators, areaTypeId) {
    getAreaData(groupId, areaCode, comparators, areaTypeId);
};

function getEnglandPrimaryData(groupId, areaCode, areaTypeId) {

    var parameters = new ParameterBuilder(
        ).add('group_ids', groupId
        ).add('area_type_id', areaTypeId
        ).add('area_codes', areaCode
        ).add('include_time_periods', 'yes'
        ).add('latest_data_only', 'yes');

    ajaxGet('api/latest_data/all_indicators_in_multiple_profile_groups_for_multiple_areas', parameters.build(),
        function (obj) {
            for (var groupId in obj) {
                for (areaCode in obj[groupId]) {
                    loaded.areaData.addData(groupId, areaCode, areaTypeId, obj[groupId][areaCode]);
                }
            }
            ajaxMonitor.callCompleted();
        });
};

function getAreaData(groupId, areaCode, comparators, areaTypeId) {

    var parameters = new ParameterBuilder(
    ).add('group_ids', groupId
    ).add('area_type_id', areaTypeId
    ).add('area_codes', areaCode
    ).add('latest_data_only', 'yes'
    ).add('comparator_area_codes', comparators.join(','));

    ajaxGet('api/latest_data/all_indicators_in_multiple_profile_groups_for_multiple_areas', parameters.build(),
        function (obj) {
            for (var groupId in obj) {
                for (areaCode in obj[groupId]) {
                    loaded.areaData.addData(groupId, areaCode, areaTypeId, obj[groupId][areaCode]);
                }
            }
            ajaxMonitor.callCompleted();
        });
}

function getAreaDataCallback(obj) {

    // {}  --groupId--> {} --areaCode--> data
    for (var groupId in obj) {
        for (areaCode in obj[groupId]) {
            loaded.areaData.addData(groupId, areaCode, obj[groupId][areaCode]);
        }
    }
    ajaxMonitor.callCompleted();
}

function AreaDataCollection() {

    var data = {};

    this.addData = function (groupId, areaCode, areaTypeId, coreDataSet) {
        data[groupId + '-' + areaCode + '-' + areaTypeId] = coreDataSet;
    };

    this.getData = function (groupId, areaCode, areaTypeId) {
        return data[groupId + '-' + areaCode + '-' + areaTypeId];
    };
}

MT.nav.rankings = function () {
    setUrl('/topic/' + profileUrlKey + '/comparisons#par/' + MT.model.parentCode +
        '/ati/' + AreaTypeIds.Practice + '/pat/' + MT.model.parentAreaType);
}

function goToPracticeProfiles() {
    var url = 'http://fingertips.phe.org.uk/profile/general-practice/data#mod,2,pat,19,par,' +
        MT.model.parentCode + ',are,' + MT.model.areaCode + ',sid1,2000005,ind1,-,sid2,-,ind2,-';
    window.open(url);
}

function goToNhsChoices() {
    if (loaded.nhsId === '') {
        alert('Sorry, this practice is not on NHS Choices');
    } else {
        var urlNhsChoices = 'http://www.nhs.uk/Services/GP/Overview/DefaultView.aspx?id=' +
            loaded.nhsId;
        window.open(urlNhsChoices);
    }
}

loaded.areaData = new AreaDataCollection();

templates.add('rows',
'{{#rows}}<tr class="odd {{selected}}"><td><span class="grade"><img src="' +
FT.url.img + 'Mortality/{{Grade}}.png" />{{{Name}}}</td>\
<td>{{PracticeValue}}{{{Unit}}}</td>\
<td>{{#ParentValue}}<span>{{ParentValue}}</span>{{{Unit}}}{{/ParentValue}}{{^ParentValue}}<span>-</span>{{/ParentValue}}</td>\
<td><span>{{NationalValue}}</span>{{{Unit}}}</td></tr>{{/rows}}');

NO_DATA = "Data is not available";

