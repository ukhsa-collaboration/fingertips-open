function goToBarChartPage() {
    lock();

    setPageMode(PAGE_MODES.BAR);

    ajaxMonitor.setCalls(5);

    getPracticeAndParentLists();
    getIndicatorMetadata(PP.model.groupId);
    getNationalData(PP.model.groupId);

    ajaxMonitor.monitor(getAreaValues);
}

function displayBarChart() {

    initBarChart();

    var rootIndex = ui.getSelectedRootIndex();

    var recreate = barChartState.recreateChart(rootIndex);
    var resort = barChartState.doResort(),
            parentCode = PP.model.parentCode;

    if (recreate || resort) {

        var isChartDisplayed = false;

        if (parentCode) {

            var areaValues = barChartState.getAreaValues(parentCode, rootIndex);

            if (isDefined(areaValues) && !_.isEmpty(areaValues)) {

                var practices = ui.getPractices();

                if (recreate) {
                    // Need new practice list
                    barChartState.practiceList = getPracticeDataForBarChart(areaValues,
                            practices);
                }

                var sortedPractices = barChartState.practiceList;

                sortPractices(sortedPractices);

                var practiceLabels = [],
                        yLabels = [],
                        dataAllPractices = [],
                        dataPractice = [],
                        currentPracticeCode = PP.model.practiceCode;

                for (var i in sortedPractices) {

                    var practice = sortedPractices[i],
                            code = practice.Code;

                    practiceLabels.push(
                            trimName(practice.Name, 23) + ' ' + code);

                    var sig = sortedPractices[i].Sig[-1];
                    var sigColor = getPracticeSigColour(sig);

                    var val = practice.Val;
                    if (val !== -1) {
                        if (code !== currentPracticeCode) {
                            dataAllPractices.push({ y: val, color: sigColor });
                            dataPractice.push(0);
                        } else {
                            // Selected practice
                            dataAllPractices.push(0);
                            dataPractice.push({ y: val, color: '#000' });
                        }

                        yLabels[val] = getYValLabel(practice);
                    }
                }
                barChartState.displayedAreaValues = yLabels;

                isChartDisplayed = true;

                barChartState.setSortKey();

                if (recreate) {
                    createBarChart(dataAllPractices, dataPractice, practiceLabels, rootIndex);
                } else {
                    resortBarChart(dataAllPractices, dataPractice, practiceLabels);
                }

            } else {
                var noDataMessage = 'No data is available';
            }
        } else {
            noDataMessage = 'Select a CCG';

        }

        barChartState.setDisplayedKey(rootIndex);

        showOrHideBarChart(isChartDisplayed, noDataMessage);
    }

    var scrollTop = barChartState.scrollTop;
    if (scrollTop) {
        // Prevents vertical scrollbar being reset to top in Chrome
        $(window).scrollTop(scrollTop);
        barChartState.scrollTop = null;
    }

    showAndHidePageElements();

    unlock();
}

function getPracticeSigColour(sig) {

    if (sig === 0) {
        var sigColor = colours.noComparison;

    } else {
        sigColor = getColourFromSignificance(sig, false, colours, false);

        // Make sure high and low are same blue
        if (sigColor === colours.bobHigher) {
            sigColor = colours.bobLower;
        }
    }

    return sigColor;
}

function getAreaValues() {

    evaluateIndicator1Menu();

    if (PP.model.parentCode !== null) {

        var index = ui.getSelectedRootIndex(),
                parentCode = PP.model.parentCode,
                values = barChartState.getAreaValues(parentCode, index);

        if (!isDefined(values)) {

            ajaxMonitor.setCalls(2);

            getAreaData(PP.model.groupId, parentCode, false);

            var roots = ui.getData(NATIONAL_CODE),
                    root = roots[index];

            getData(getAreaValuesCallback, 'av', ['par=', NATIONAL_CODE,
                '&gid=', PP.model.groupId,
                '&ati=', PRACTICE,
                '&off=', getYearOffset(),
                '&iid=', root.IID,
                '&age=', root.AgeId,
                '&sex=', root.SexId].join(''));

            ajaxMonitor.monitor(displayBarChart);
            return;
        }
    }

    displayBarChart();
}
;

function getAreaValuesCallback(obj) {

    barChartState.setAreaValues(PP.model.parentCode, obj);

    ajaxMonitor.callCompleted();
}
;

barChartState = {
    //TODO - move sort code into own class

    isInitialised: false,
    displayedAreaValues: null,
    displayedUnit: null,
    scrollTop: null,
    _displayedKey: '',
    _lastSortKey: '',
    sortType: 2,
    sortDirection: 1,
    practiceList: null,
    sortOptions: [
        { id: 2, label: 'Indicator Value', extraClass: 'pageSelected', defaultOrder: 1 },
        { id: 0, label: 'Practice Name', defaultOrder: 0 },
        { id: 1, label: 'Practice Code', defaultOrder: 0 }
    ],
    resetSort: function () {
        this.sortType = 0;
        this.sortDirection = 0;
    },
    setSortType: function (type) {

        this.sortDirection = this.sortType == type ?
                // Toggle sort direction
                Math.abs(this.sortDirection - 1) :
                // Set direction to default
                _.find(this.sortOptions, function (option) {
                    return option.id === type
                }).defaultOrder;

        this.sortType = type;
    },
    _getSortKey: function () {
        return this.sortType + '-' + this.sortDirection;
    },
    setSortKey: function () {
        this._lastSortKey = this._getSortKey();
    },
    doResort: function () {
        return this._lastSortKey != this._getSortKey();
    },
    getAreaValueKey: function (rootIndex, areaCode) {
        return [areaCode, rootIndex, PP.model.groupId, PP.model.year].join('-');
    },
    setAreaValues: function (areaCode, obj) {
        var key = this.getAreaValueKey(ui.getSelectedRootIndex(), areaCode);
        loaded.areaValues[key] = obj;
    },
    getAreaValues: function (areaCode, rootIndex) {
        var key = this.getAreaValueKey(rootIndex, areaCode);
        return loaded.areaValues[key];
    },
    getKey: function (rootIndex) {
        var codeString = (PP.model.parentCode === null ? '-' : PP.model.parentCode);
        var key = this.getAreaValueKey(rootIndex, codeString);
        return key + (PP.model.practiceCode === null ? '-' : PP.model.practiceCode);
    },
    recreateChart: function (rootIndex) {
        var key = this.getKey(rootIndex);
        return key !== this._displayedKey;
    },
    setDisplayedKey: function (rootIndex) {
        this._displayedKey = this.getKey(rootIndex);
    }

};

function getYValLabel(data) {
    var y = data.Val;
    if (y < 10) {
        return roundNumber(y, 2);
    } else {
        return data.ValF;
    }
}
;

function getCategoryFromLabel(label) {
    var bits = label.split(' ');
    return $.trim(bits[bits.length - 1]);
}
;

function barClicked(category) {

    lock();

    barChartState.scrollTop = $(window).scrollTop();

    var code = getCategoryFromLabel(category);

    setPracticeCode(code);
    ftHistory.setHistory();

    goToBarChartPage();
}
;

function showOrHideBarChart(isChartDisplayed, noDataMessage) {

    var noChart = $('#noBarChart');
    var chart = $('#barChart,#sortPracticeBox');
    var exportLink = $("#barBox .export-chart-box");
    var legends = $("#barBox .legends");

    if (isChartDisplayed) {
        chart.show();
        noChart.hide();
        exportLink.show();
        legends.show();

    } else {
        noChart.html(noDataMessage);
        chart.hide();
        noChart.show();

        // hide export button
        // hide legends
        exportLink.hide();
        legends.hide();

    }
}
;


function createBarChart(dataAllPractices, dataPractice, practiceLabels, rootIndex) {

    var nationalRoot = ui.getData(NATIONAL_CODE)[rootIndex],
            iid = nationalRoot.IID,
            nationalDataValues = nationalRoot.Data;

    var timePeriod = new TimePeriod(nationalRoot.Periods, nationalDataValues.length, getYearOffset()),
            yearIndex = timePeriod.yearIndex;

    // England average
    var dataNationalArray = [];
    var dataNational = nationalDataValues[yearIndex];
    if (dataNational) {
        var dataNationalVal = dataNational.Val;
        for (var i in dataPractice) {
            dataNationalArray.push(dataNationalVal);
        }
    }

    // Parent average
    var parentData = ui.getData(PP.model.parentCode);
    var parentDataValues = parentData[rootIndex].Data;
    var dataParent = parentDataValues[yearIndex];
    if (dataParent) {
        var dataParentVal = dataParent.Val,
                dataParentArray = [];
        for (var i in dataPractice) {
            dataParentArray.push(dataParentVal);
        }
    }

    var metadata = ui.getMetadataHash()[iid],
            indicatorName = metadata.Descriptive.Name,
            unit = metadata.Unit,
            yLabel = getBarChartYLabel(indicatorName, unit.Label);

    var parentName = getPracticeParentName();

    var series = [
        {
            // All practices
            name: parentName + ' (individual practices)',
            data: dataAllPractices,
            events: {
                click: function (event) {
                    barClicked(event.point.category);
                }
            },
            showInLegend: false
        }
    ];

    if (PP.model.isPractice()) {

        var practice = ui.getCurrentPractice();

        series.push(
                {
                    // Selected practice (this series must be added last as is optional)
                    name: practice.Code + ' - ' + practice.Name,
                    data: dataPractice,
                    color: '#000',
                    events: {
                        click: function (event) {
                            barClicked(event.point.category);
                        }
                    },
                    legendIndex: 3
                });
    }

    var country = 'England',
            parentColour = chartColours.pink,
            average = ' (average)';
    // Add lines last so that they are displayed over the bars
    series.push(
        {
            // National
            name: country + average,
            type: 'line',
            data: dataNationalArray,
            color: colours.comparator,
            legendIndex: 0
        },
        {
            // Parent
            name: parentName + average,
            type: 'line',
            data: dataParentArray,
            color: parentColour,
            legendIndex: 1
        }
    );

    var itemStyle = { color: '#333', fontWeight: 'normal', textShadow: '0' };
    var title = indicatorName + new SexAndAge().getLabel(nationalRoot);

    try {
        chart = new Highcharts.Chart({
            chart: {
                renderTo: 'barChart',
                animation: false,
                defaultSeriesType: 'bar',
                width: 850,
                height: getBarChartHeight(dataAllPractices)
            },
            title: {
                text: '<div class="barChartTitle"><b>' +
                    title + '</b> - <b>' + timePeriod.label + '</div>',
                style: CHART_TEXT_STYLE,
                useHTML: true
            },
            subtitle: {
                style: {
                    direction: 'ltr',
                    align: 'center',
                    color: chartColours.label,
                    fontWeight: 'normal',
                    fontFamily: 'Verdana'
                },
                text: parentName,
                useHTML: true,
                align: 'center'
            },
            xAxis: {
                categories: practiceLabels,
                labels: {
                    step: 1
                }
            },
            yAxis: {
                title: {
                    text: title + yLabel,
                    style: CHART_TEXT_STYLE
                }
            },
            plotOptions: {
                series: {
                    animation: false,
                    events: {
                        legendItemClick: function () {
                            return false;
                        }
                    }
                },
                bar: {
                    borderWidth: 0,
                    pointPadding: 0,
                    pointWidth: 11,
                    stacking: 'normal',
                    shadow: false,
                    dataLabels: {
                        allowOverlap: true,
                        enabled: true,
                        style: itemStyle,
                        align: 'right',
                        y: 0, // Ignored IE 6-8
                        x: 28,
                        formatter: function () {
                            return this.y === 0 ?
                                    '' :
                                    barChartState.displayedAreaValues[this.y];
                        }
                    }
                },
                line: {
                    // The symbol is a non-valid option here to work round a bug
                    // in highcharts where only half of the markers appear on hover
                    marker: HC.noLineMarker,
                    states: {
                        hover: {
                            marker: HC.noLineMarker
                        }
                    }
                }
            },
            legend: {
                enabled: true,
                borderWidth: 0,
                layout: 'vertical',
                itemStyle: itemStyle
            },
            tooltip: {
                formatter: function () {

                    var series = this.series;

                    if (series.name.indexOf(country) === 0) {
                        // National
                        var category = series.name,
                                val = dataNational.ValF;
                    } else if (series.color === parentColour) {
                        // Practice parent
                        category = series.name;
                        val = dataParent.ValF;
                    } else {
                        // Practice
                        var y = this.point.y;
                        val = barChartState.displayedAreaValues[y];
                        category = this.point.category;

                        var code = getCategoryFromLabel(category),
                                practice = getPracticeFromCode(code);

                        if (practice) {
                            category = code + ' - ' + practice.Name;
                        }
                    }
                    return '<b>' + category + '</b><br/>' +
                            new ValueWithUnit(unit).getFullLabel(val);
                }
            },
            credits: HC.credits,
            series: series,
            exporting: {
                allowHTML: true,
                enabled: false
            }
        });
    } catch (e) {
        // HighChart reports errors via console.log which is not available <=IE8
    }
};

function getPracticeDataForBarChart(areaValues, practices) {

    var dataList = [],
            areaCodeToData = {},
            i, areaCode, data;

    _.each(areaValues, function (data) {
        areaCodeToData[data.AreaCode] = data;
    });

    for (i in practices) {

        areaCode = practices[i].Code;

        // Not all practices will have data
        if (areaCodeToData[areaCode]) {
            data = areaCodeToData[areaCode];
            dataList.push({
                'Code': areaCode,
                'Val': data.Val,
                'ValF': data.ValF,
                'Name': practices[i].Name,
                'Sig': data.Sig
            });
        }
    }

    return dataList;
}
;

function sortPractices(practices) {

    if (barChartState.sortType === 0) {
        var sorter = sortArea;
    } else if (barChartState.sortType === 1) {
        sorter = sortByCode;
    } else {
        sorter = sortData;
    }
    practices.sort(sorter);

    if (barChartState.sortDirection === 1) {
        practices.reverse();
    }
}
;

function sortPracticesClicked(type) {

    lock();
    var prefix = '#sortPractice';
    setPageSelected(prefix + 'Box', prefix + type);
    barChartState.setSortType(type);
    displayBarChart();
}
;

function getBarChartHeight(data) {

    var height = (data.length * 15/*height of single bar, see pointWidth in options*/) +
            205/*space of main title, x-axis title, legend*/;

    if (PP.model.isPractice()) {
        // Extra space as practice only included in legend when selected
        height += 20;
    }
    return height;
}

function initBarChart() {

    if (!barChartState.isInitialised) {
        $('#barBox').html(templates.render('barChart',
                { options: barChartState.sortOptions }));
        barChartState.isInitialised = true;
    }
}

function getBarChartYLabel(indicatorName, unitLabel) {

    if (unitLabel !== '') {
        unitLabel = ' (' + unitLabel + ')';
        if (indicatorName.endsWith(unitLabel)) {
            unitLabel = '';
        }
    }
    return unitLabel;
}

function resortBarChart(dataAllPractices, dataPractice, practiceLabels) {

    //NOTE: Series are accessed by index... brittle method

    // Modify practice data
    barChart.series[0].setData(dataAllPractices, false);
    if (PP.model.isPractice()) {
        barChart.series[1].setData(dataPractice, false);
    }
    barChart.xAxis[0].setCategories(practiceLabels, false);

    barChart.redraw();
}

function sortByCode(a, b) {
    var codeA = a.Code.toLowerCase(),
            codeB = b.Code.toLowerCase();
    if (codeA < codeB)
        return -1;
    if (codeA > codeB)
        return 1;
    return 0;
}

var practiceProfilesLegend = '<div class="legends" style="position: relative; top: 10px;">\
    <table id="barChartKeyTable">\
    <tr><td style="background-color: #682EE3;" class="key">&nbsp;</td><td>Significantly different from England average</td></tr><tr><td style="background-color: #F5C720;" class="key">&nbsp;</td>\
    <td>No significant difference from England average</td></tr><tr><td style="background-color: #c9c9c9;" class="key">&nbsp;</td><td>Significance not calculated</td></tr>\
    </table></div>';

templates.add('barChart', showExportChartLink() + practiceProfilesLegend + '<div id="sortPracticeBox" class="greyBoxed">Sort by:{{#options}}\
<a href="javascript:sortPracticesClicked({{id}});" id="sortPractice{{id}}" class="{{extraClass}} practiceSort pageSelector" >{{label}}</a>\
{{/options}}</div><div id="barChart"></div><div id="noBarChart" class="selectLabel"></div>');

