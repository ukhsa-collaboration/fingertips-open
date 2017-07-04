'use strict';

/**
* Population namespace
* @module population
*/

/**
* Entry point to displaying population page
* @class goToPopulationPage
*/
function goToPopulationPage() {
    var model = FT.model;

    setPageMode(PAGE_MODES.POPULATION);

    var noOfApiCalls = model.areaTypeId === AreaTypeIds.Practice ? 5 : 3;

    ajaxMonitor.setCalls(noOfApiCalls);

    // AJAX call to api/quinary_population
    getPopulation(model.areaCode);
    getPopulation(model.parentCode);
    getPopulation(NATIONAL_CODE);

    if (FT.model.areaTypeId === AreaTypeIds.Practice) {
        getLabelSeries();
        getPopulationSummary(model.areaCode);
    }
    ajaxMonitor.monitor(displayPopulation);
}
/**
* Creates and displays the HTML for the population page.
* @class displayPopulation
*/
function displayPopulation() {

    var $container = pages.getContainerJq();
    var label;
    var model = FT.model;
    var population = loaded.population;
    if (!isDefined(population[model.areaCode]) &&
        !isDefined(population[model.parentCode]) &&
        !isDefined(population[NATIONAL_CODE])) {
        label = '<div class="no-population-data fl">&nbsp;</div><div class="no-population-data fl">No population data available for current area.</div>';
        $container.html(label);
    }
    else {
        label = '<div style="float: left;width: 600px">' +
                '<div class="export-chart-box"><a class="export-link" href="javascript:exportPopulationChart()">Export chart as image</a></div>' +
                '<div id="population-chart"></div>' +
                '<div id="population-source"></div>' +
                '</div>';

        if (model.areaTypeId === AreaTypeIds.Practice) {
            $container.html(label + displayPopulationInfo());
        } else {
            $container.html(label);
        }

        displayPopulationChart();
    }

    showAndHidePageElements();
    unlock();
}

function getPopulation(areaCode) {

    ajaxGet('api/quinary_population',
        'area_code=' + areaCode +
        '&area_type_id=' + FT.model.areaTypeId,
        function (data) {

            if (isDefined(data.Values[SexIds.Male]) || isDefined(data.Values[SexIds.Female])) {
                makeValuesNegative(data.Values[SexIds.Male]);
                loaded.population[areaCode] = data;
            }
            ajaxMonitor.callCompleted();
        });
};

function getPopulationSummary(areaCode) {

    ajaxGet('api/quinary_population_summary',
        'area_code=' + areaCode +
        '&area_type_id=' + FT.model.areaTypeId,
        function (data) {
            loaded.PopulationSummary[areaCode] = data;
            ajaxMonitor.callCompleted();
        });
};

function makeValuesNegative(values) {
    for (var i in values) {
        values[i] = -Math.abs(values[i]);
    }
};

var chartColours = {
    label: '#333',
    bar: '#76B3B3',
    pink: '#FF66FC'
}

var CHART_TEXT_STYLE = {
    color: chartColours.label,
    fontWeight: 'normal',
    fontFamily: 'Verdana'
};

function getPopulationMax(populations) {
    var max = 5;
    var min = -max;
    for (var i in populations) {
        if (populations[i] != null) {
            min = _.min([min, _.min(populations[i].male)]);
            max = _.max([max, _.max(populations[i].female)]);
        }
    }
    return Math.ceil(_.max([max, Math.abs(min)]));
}

function displayPopulationChart() {

    var model = FT.model;

    // Define populations
    var population = loaded.population;
    var profilePopulation = new Population(population[model.areaCode]),
        parentPopulation = new Population(population[model.parentCode]),
        nationalPop = new Population(population[NATIONAL_CODE]);

    var max = getPopulationMax([profilePopulation, parentPopulation, nationalPop]);

    // Labels
    var subtitle = population[NATIONAL_CODE].IndicatorName + " " + population[NATIONAL_CODE].Period;
    var maleString = ' (Male)',
        femaleString = ' (Female)',
        parentColour = chartColours.pink,
        chartTitle = '<div style="text-align:center;">Age Profile<br><span style="font-size:12px;">' + subtitle + '</span></div>';
    populationState.chartTitle = chartTitle;

    var areaName = areaHash[model.areaCode].Name;
    var isParentNotEngland = model.parentCode.toUpperCase() !== NATIONAL_CODE;
    // Area
    var series = [{
        name: areaName + maleString,
        data: profilePopulation.male,
        type: 'bar',
        color: colours.bobLower
    }, {
        name: areaName + femaleString,
        data: profilePopulation.female,
        type: 'bar',
        color: colours.bobHigher
    }];

    // Parent
    if (isParentNotEngland) {
        series.push({
            name: getParentArea().Name,
            data: parentPopulation.male,
            color: parentColour,
            showInLegend: false
        },
            {
                name: getParentArea().Name,
                data: parentPopulation.female,
                color: parentColour
            });
    }

    // England
    series.push({
        name: 'England',
        data: nationalPop.male,
        color: colours.comparator,
        showInLegend: false
    },
        {
            name: 'England',
            data: nationalPop.female,
            color: colours.comparator
        });

    try {
        populationState.populationChart = new Highcharts.Chart({
            chart: {
                renderTo: 'population-chart',
                defaultSeriesType: 'line',
                margin: [60, 55, 150, 55],
                /* margins must be set explicitly to avoid labels being positioned outside visible chart area */
                width: 600,
                height: 550
            },
            title: {
                text: chartTitle,
                style: CHART_TEXT_STYLE,
                useHTML: true
            },
            xAxis: [{
                categories: nationalPop.Labels,
                reversed: false
            }, { // mirror axis on right side

                opposite: true,
                reversed: false,
                categories: nationalPop.Labels,
                linkedTo: 0
            }
            ],
            yAxis: {
                min: -max,
                max: max,
                title: {
                    text: '% of total population',
                    style: CHART_TEXT_STYLE
                },
                labels: {
                    formatter: function () {
                        return Math.abs(this.value);
                    }
                }
            },
            plotOptions: {
                series: {
                    events: {
                        legendItemClick: function () {
                            return false;
                        }
                    }
                },
                bar: {
                    borderWidth: 0,
                    pointPadding: 0,
                    stacking: 'normal',
                    animation: false
                },
                line: {
                    // The symbol is a non-valid option here to work round a bug
                    // in highcharts where only half of the markers appear on hover
                    marker: HC.noLineMarker,
                    animation: false,
                    states: {
                        hover: {
                            marker: HC.noLineMarker
                        }
                    }
                }
            },
            legend: {
                enabled: true,
                layout: 'vertical',
                align: 'center',
                verticalAlign: 'bottom',
                itemStyle: {
                    fontWeight: 'normal'
                }
            },
            tooltip: {
                formatter: function () {
                    return '<b>' + this.series.name.replace(/2$/, '') + '<br>Age: ' +
                        this.point.category + '</b><br/>' +
                        'Population: ' + Math.abs(this.point.y) + '%';
                }
            },
            credits: HC.credits,
            exporting: {
                enabled: false
            },
            series: series
        });
    } catch (e) {
    }
}

function Population(population) {
    if (!population) {
        var male = [], female = [];
    } else {
        male = population.Values[SexIds.Male];
        female = population.Values[SexIds.Female];
        this.Labels = population.Labels;
    }
    this.male = male;
    this.female = female;
}

function exportPopulationChart() {
    populationState.populationChart.exportChart({ type: 'image/png' }, {
        chart: {
            spacingTop: 70,
            events: {
                load: function () {
                    this.renderer.text(populationState.chartTitle, 300, 15)
                        .attr({
                            align: 'center'
                        })
                        .css({
                            fontSize: '14px',
                            width: '600px'
                        })
                        .add();
                }
            }
        },
        title: {
            text: '',
            style: CHART_TEXT_STYLE
        }
    });
}

function displayPopulationInfo() {

    var populationSummary = loaded.PopulationSummary[FT.model.areaCode];
    var viewModel = {};
    if (isDefined(loaded.PopulationSummary)) {
        viewModel.practiceLabel = FT.model.areaCode + ' - ' + areaHash[FT.model.areaCode].Name;
        viewModel.ethnicity = updateEthnicity(populationSummary);
        viewModel.deprivation = updateDeprivationTable(populationSummary);
        viewModel.furtherInfo = updateFurtherInfo(populationSummary.AdHocValues);
        viewModel.registeredPersons = updateRegisteredPersons();
        viewModel.noPopulation = false;
    } else {
        viewModel.noPopulation = true;
    }

    templates.add('summary',
        '<div style="float: right; width: 360px;">' +
        '<div id="population-table-box">' +
        '<div style="float: left; text-align: center; width: 100%; font-weight: bold; position: relative;">' +
        '<div class="right-tooltip-icon info-tooltip" onclick="showMetadata(114)">' +
        '</div>' +
        '<div>Registered Persons</div>' +
        '</div>' +
        '{{{registeredPersons}}}' +
        '</div>' +
        '<div id="population-info">' +
        '<div id="practice-pop-info">' +
        '<div id="practice-label" class="popLabel">{{practiceLabel}}</div>{{{furtherInfo}}}{{{deprivation}}}{{{ethnicity}}}' +
        '</div></div>' +
        '{{#noPopulation}}<div id="noPractice" class="selectLabel">No population data<br>available for current practice</div>{{/noPopulation}}' +
        '</div>');

    return templates.render('summary', viewModel);
};

function getLabelSeries() {

    if (!isDefined(labels[1])) {

        var parameters = new ParameterBuilder(
        ).add('category_type_id', CategoryTypeIds.DeprivationDecileGp2015);
        ajaxGet('api/categories',
            parameters.build(),
            function (categories) {

                labels = _.object(_.map(categories,
                    function (category) {
                        return [category.Id, category.Name];
                    }));

                ajaxMonitor.callCompleted();
            });

    } else {

        ajaxMonitor.callCompleted();
    }
};

function updateDeprivationTable(populationSummary) {
    var decileNo = populationSummary.GpDeprivationDecile;
    var viewModel = {};
    if (isDefined(decileNo)) {
        viewModel.decileName = labels[decileNo];

        // Define decile list
        viewModel.decile = [];
        for (var i = 1; i <= 10; i++) {
            viewModel.decile.push({ text: '', num: i });
        }

        // Selected decile
        viewModel.decile[decileNo - 1].text = '<div class="decileSpacer"></div><div class="decile decile' + decileNo + '" >' + decileNo + '</div>';
    } else {
        viewModel.decileName = '<i>Data not available for current practice</i>';
    }
    templates.add("deprivationInfo",
        '<table id="deprivation-table" class="bordered-table" cellspacing="0">' +
        '<thead>' +
        '<tr>' +
        '<th>' +
        '<div class="w100" style="position: relative;">' +
        'Deprivation' +
        '<div class="right-tooltip-icon info-tooltip" onclick="showMetadata(91872)"></div>' +
        '</div>' +
        '</th>' +
        '</tr>' +
        '</thead>' +
        '<tbody>' +
        '<tr><td id="deprivation" style="border-bottom: none;">{{decileName}}</td></tr>' +
        '<tr>' +
        '<td style="border-top: none;">' +
        '<table id="decileKey" cellspacing="0">' +
        '<tr>{{#decile}}<td id="d{{num}}">{{{text}}}</td>{{/decile}}</tr>' +
        '<tr>{{#decile}}<td class="decile{{num}}"></td>{{/decile}}</tr>' +
        '</table>' +
        '<div class="fl deprivationLabel">More deprived</div>' +
        '<div class="fr deprivationLabel">Less deprived</div>' +
        '</td>' +
        '</tr>' +
        '</tbody>' +
        '</table>');
    return templates.render('deprivationInfo', viewModel);
};

function updateEthnicity(data) {

    var viewModel = {};
    var ethnicity = data.Ethnicity;

    viewModel.ethnicity = isDefined(ethnicity) ?
        ethnicity :
        '<i>Insufficient data to provide accurate summary</i>';

    templates.add('ethnicityInfo',
        '<table id="ethnicity-table" class="bordered-table" cellspacing="0">' +
        '<thead>' +
        '<tr>' +
        '<th>' +
        '<div class="w100" style="position: relative;">' +
        'Ethnicity Estimate' +
        '<div class="right-tooltip-icon info-tooltip" onclick="showMetadata(1679)">' +
        '</div>' +
        '</div>' +
        '</th>' +
        '</tr>' +
        '</thead>' +
        '<tbody>' +
        '<tr>' +
        '<td id="ethnicity">{{ethnicity}}</td>' +
        '</tr>' +
        '</tbody>' +
        '</table>');
    return templates.render('ethnicityInfo', viewModel);
};

templates.add('furtherInfo', '{{#rows}}<tr><td class="header information">{{#iid}}<div class="fl info-tooltip" onclick="showMetadata({{iid}})"></div>{{/iid}}{{name}}</td><td>{{val}}{{^val}}' +
        NO_DATA + '{{/val}}{{#average}} <span class="averageLabel">(average)</span>{{/average}}</td></tr>{{/rows}}');

function updateFurtherInfo(adhocValues) {

    var rows = [];

    //TODO all these IIDs could come from corews

    // QOF achievement
    var qof = adhocValues.Qof;
    var value = isDefined(qof) ?
        (roundNumber(qof.Count, 1) + ' (out of ' + qof.Denom + ')') : null;
    rows.push({ name: 'QOF achievement', val: value, iid: 295 });

    // Life expectancy
    var text = 'ale life expectancy';

    var life = adhocValues.LifeExpectancyMale;
    var isLife = isDefined(life);
    value = isLife ? life.ValF + ' years' : null;
    rows.push({ name: 'M' + text, val: value, iid: 650 });

    life = adhocValues.LifeExpectancyFemale;
    value = isLife ? life.ValF + ' years' : null;
    rows.push({ name: 'Fem' + text, val: value, iid: 650 });

    // Patients that recommend practice
    var recommend = adhocValues.Recommend;
    value = isDefined(recommend) ? recommend.ValF + '%' : null;
    rows.push({ name: '% of patients that would recommend their practice', val: value, iid: 347 });

    var viewModel = {}
    viewModel.tablebody = templates.render('furtherInfo', { rows: rows });
    templates.add('further-info-table',
        '<table id="further-info-table" class="bordered-table" cellspacing="0">{{{tablebody}}}</table>');
    return templates.render('further-info-table', viewModel);
};

function PopulationNumber(coreDataSet) {
    this.data = coreDataSet;
}

PopulationNumber.prototype = {

    isData: function () {
        return this.data && this.data.Val > 0;
    },

    val: function () {
        return this.isData() ?
            new CommaNumber(this.data.Val).rounded() :
            null;
    }
}

function updateRegisteredPersons() {

    var rows = [], populationNumber;
    var practiceParentName = getParentArea().Name.toUpperCase();
    var model = FT.model;

    var practicePop = loaded.population[model.areaCode];;
    var practiceParentPop = loaded.population[model.parentCode];
    var nationalPop = loaded.population[NATIONAL_CODE];
    // Practice
    if (isDefined(practicePop)) {
        rows.push({
            name: areaHash[model.areaCode].Name,
            val: practicePop.hasOwnProperty('ListSize') ? new PopulationNumber(practicePop.ListSize).val() : ''
        });

    }
    // Practice parent
    if (practiceParentPop) {
        populationNumber = new PopulationNumber(practiceParentPop.ListSize);
        rows.push({
            name: practiceParentName,
            val: practiceParentPop.hasOwnProperty('ListSize') ? new PopulationNumber(practiceParentPop.ListSize).val() : '',
            average: populationNumber.isData()
        });
    }

    // National
    if (nationalPop) {
        rows.push({
            name: 'ENGLAND',
            val: nationalPop.hasOwnProperty('ListSize') ? new PopulationNumber(nationalPop.ListSize).val() : '',
            average: true
        });
    }

    var viewModel = {}
    viewModel.tablebody = templates.render('furtherInfo', { rows: rows });

    templates.add("pop-table", '<table id="pop-table" class="bordered-table" cellspacing="0">{{{tablebody}}}</table>');
    return templates.render('pop-table', viewModel);
};

function showMetadata(indicatorId) {

    ajaxMonitor.setCalls(2);

    getPopulationIndicatorMetadata(indicatorId);
    getMetadataProperties();

    ajaxMonitor.monitor(displayIndicatorMetadata);
}

function getPopulationIndicatorMetadata(indicatorId) {

    var parameters = new ParameterBuilder(
   ).add('indicator_ids', indicatorId
   ).add('include_definition', 'yes');

    ajaxGet('api/indicator_metadata/by_indicator_id',
        parameters.build(),
        function (data) {
            populationIndicatorMetadata = data[indicatorId];
            ajaxMonitor.callCompleted();
        });
}

function displayIndicatorMetadata() {
    if (isDefined(populationIndicatorMetadata)) {
        var a = getMetadataHtml(populationIndicatorMetadata, null);
        var html = '<div style="padding:15px;">' + a.html + '</div>';
        var popupWidth = 800;
        var left = ($(window).width() - popupWidth) / 2;
        var top = 200;
        lightbox.show(html, top, left, popupWidth);
        labelAsync.populate(a.labelArgs);
    }
}

var populationIndicatorMetadata = {}
var labels = {};

loaded.population = {};
loaded.PopulationSummary = {}

var populationState = {
    populationChart: null,
    chartTitle : null
};

pages.add(PAGE_MODES.POPULATION, {
    id: 'population',
    title: 'Population',
    goto: goToPopulationPage,
    gotoName: 'goToPopulationPage',
    needsContainer: true,
    jqIds: ['areaMenuBox', 'parentTypeBox', 'areaTypeBox', 'region-menu-box', 'nearest-neighbour-link']
});

