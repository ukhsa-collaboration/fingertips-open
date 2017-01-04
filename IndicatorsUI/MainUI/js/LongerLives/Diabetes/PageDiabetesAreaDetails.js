function initPage() {

    showLoadingSpinner();
    lock();

    updateModelFromHash();

    var model = MT.model,
        isSimilar = isSimilarAreas();

    var callCount = model.profileId === ProfileIds.HealthChecks ? 7 : 6;
    if (isSimilar) {
        callCount += 2;
    }
    ajaxMonitor.setCalls(callCount);
    var groupId = getGroupId().groupId;

    loaded.areaDetails.fetchDataByAjax(getGroupId());

    if (model.profileId === ProfileIds.HealthChecks) {
        loaded.areaDetailsForDiseaseAndDeath.fetchDataByAjax({ groupId: GroupIds.HealthChecks.DiseaseAndDeath });
    }
    getIndicatorMetadata(groupId);
    getSpecificGroupRoots(groupId, model.areaTypeId);
    getDecileData(model);
    getAllAreas(model);
    getOnsClusterCode(model);

    if (isSimilar) {
        getChildAreas(model);
        loaded.areaDetails.fetchDataByAjax({ areaCode: model.parentCode });
    }

    ajaxMonitor.monitor(displayPage);
}

function displayPage() {
    var isSimilar = isSimilarAreas(),
        model = MT.model;

    if (isSimilar) {
        populateSimilarAreasList();
        selectAreaOption('#show_similar_areas');
    } else {
        selectAreaOption('#show_all_areas');
    }

    var comparisonText = "Compared to other areas";
    $('#comparison-header').html(comparisonText);


    displayAreaRangeElements(!isSimilar);
    var areaDetails = loaded.areaDetails.getData(getGroupId());

    // Area name
    var area = areaDetails.Area;
    var areaName = area.Name;

    // Used to set the areaname in breadcrumb
    $('.area_name').html(areaName);

    // Area grouping
    if (areaDetails.Decile) {
        var decileNumber = areaDetails.Decile.Number,
            decileInfo = getDecileInfo(decileNumber);
        $('.decile_text').html(decileInfo.text);
        $('#decile_number').html(decileNumber);

        // Purple deprivation man
        $('#socioeconomic_ib').addClass('level' + decileInfo.quintile);
    } else {
        // No decile so hide section
        $('.similar_authorities').html('');
    }

    var parentCode = model.parentCode;
    var metadataHash = loaded.indicatorMetadata[getGroupId().groupId];

    if (model.profileId === ProfileIds.HealthChecks) {
        headlineIndicator = new HeadlineIndicator();
    }

    // Render Reset of the page content
    renderHeader(areaDetails);
    renderLegend(areaName);
    renderHeadliningBarChart();
    renderRanks();
    renderRelatedLinks(areaDetails.Url);

    var ranks = areaDetails.Ranks[parentCode];
    var significances = areaDetails.Significances[parentCode];

    var html = [];
    var noOfIndicators = _.size(areaDetails.Benchmarks[NATIONAL_CODE]);
    var overallMax = getOverallMax(ranks, noOfIndicators);
    var causeOptions = getCauseOptions();
    for (var i = 0; i < noOfIndicators; i++) {
        var causeInfo = causeOptions[i];
        var root = groupRoots[i],
            rank = ranks[i];

        // Do not display suicide plan as bars
        if (root.ComparatorMethodId === ComparatorMethodIds.SuicidePlan) continue;

        // Sex label if required
        var sexForCurrectIndicator = root.StateSex
            ? sexForCurrectIndicator = ' (' + root.Sex.Name + ')'
            : '';

        var causeClass = (rank && rank.AreaRank)
            ? getCauseClass(significances[i], root)
            : 'none';

        var causeBar = getCauseBars(area, causeInfo, rank, overallMax, causeClass,
            metadataHash[root.IID], root.ComparatorMethodId, sexForCurrectIndicator);
        html.push(causeBar);
    }

    // Show table element before HTML rendered to prevent IE8 difficulties
    if (model.areaTypeId === AreaTypeIds.CountyUA) {
        $('.aboutONSGroup').hide();
        $('.aboutDecileGroup').show();
        $('.area-bracket').html('Socioeconomic deprivation bracket');
    } else {
        $('.aboutDecileGroup').hide();
        $('.area-bracket').html('ONS cluster');
        if (isSimilar) {
            $('#ons_name').html(getOnsClusterName(model.parentCode));
            $('.aboutONSGroup').show();
        } else {
            $('.aboutONSGroup').hide();
        }
    }

    $('#data_page_table, .more_info, .similar_authorities').show();
    $('#cause_bars').html(html);

    toggleDataHeaders(true, areaName);

    removeLoadingSpinner();
    unlock();
}

// First indicator with bars for Health Checks
function HeadlineIndicator() {
    var overallIndex = CVD_INDICATOR_INDEX;

    var areaDetails = loaded.areaDetailsForDiseaseAndDeath.getData(
        { groupId: GroupIds.HealthChecks.DiseaseAndDeath });

    var parentCode = MT.model.parentCode;
    var ranks = areaDetails.Ranks[parentCode];
    var significances = areaDetails.Significances[parentCode];
    var areaRank = ranks[overallIndex];

    this.ranks = areaDetails.Ranks;
    this.areaRank = areaRank;
    this.overallMax = ranks[overallIndex].Max.Val;
    this.decileNumber = areaDetails.Decile.Number;
    this.decileInfo = getDecileInfo(this.decileNumber);
    this.dataTimeStamp = areaDetails.Ranks[NATIONAL_CODE][overallIndex].Period;
    this.causeClass = function () {
        var cssClass = "no-data";
        if (areaRank.AreaRank) {
            var root = {
                PolarityId: PolarityIds.RAGLowIsGood,
                ComparatorMethodId: -1
            };
            cssClass = getCauseClass(significances[overallIndex], root);
            applySigClass(cssClass);
        }
        return cssClass;
    }
    this.overallIndex = overallIndex;
    this.area = areaDetails.Area;
}

function renderHeader(areaDetails) {
    var pageHeaderModel;
    if (MT.model.profileId === ProfileIds.HealthChecks) {
        // People eligible for an NHS Health Check in England
        var eligibleInEngland = areaDetails.Benchmarks[NATIONAL_CODE][0].Denom;
        var eligibleInEnglandFormated = new CommaNumber(eligibleInEngland).rounded();

        // People eligible for an NHS Health Check in this area
        var eligibleInThisArea = areaDetails.Ranks[NATIONAL_CODE][0].AreaRank.Denom;
        var eligibleInThisAreaFormated = new CommaNumber(eligibleInThisArea).rounded();

        // Preventable cardio-vascular disease mortality rate in this area
        var overallIndex = headlineIndicator.overallIndex;
        var preventableCvdMortality = headlineIndicator.ranks[NATIONAL_CODE][overallIndex].AreaRank ?
            headlineIndicator.ranks[NATIONAL_CODE][overallIndex].AreaRank.Count : 0;

        var preventableCvdMortalityFormatted = new CommaNumber(preventableCvdMortality).rounded();

        pageHeaderModel = {
            areaName: headlineIndicator.areaName,
            eligibleEnglang: eligibleInEnglandFormated,
            eligibleThisArea: eligibleInThisAreaFormated,
            preventableCvdMortality: preventableCvdMortality === 0 ? 'No data for ' : '<strong>' + preventableCvdMortalityFormatted + '</strong>'
        };
    } else {
        pageHeaderModel = {
            areaName: areaDetails.areaName
        };
    }
    pageHeader(pageHeaderModel);
}

function renderLegend(areaName) {
    var pageLegendModel = {
        contentHeading: 'Similar local authorities',
        isSimilarViewMode: MT.model.parentCode !== NATIONAL_CODE,
        areaName: areaName
    };
    pageLegend(pageLegendModel);
}

function renderHeadliningBarChart() {
    if (MT.model.profileId === ProfileIds.HealthChecks) {
        var metadataHash = loaded.indicatorMetadata[MT.model.groupId];

        var barsHtml = getBars(headlineIndicator.areaRank, headlineIndicator.overallMax,
            headlineIndicator.area, true, false, metadataHash[headlineIndicator.overallIndex]);

        var areaRank = headlineIndicator.areaRank;
        var comparisonStatement = areaRank.AreaRank
            ? getComparisonStatement(areaRank.AreaRank.Rank, areaRank.Max.Rank)
            : '';

        var pageHeadliningBarChartModel = {
            topIndicatorSig: headlineIndicator.causeClass,
            topIndicatorHeading: 'CVD premature deaths ',
            topIndicatorRanking: comparisonStatement,
            topIndicatorDate: headlineIndicator.dataTimeStamp,
            topIndicatorToolTip: 'CVD premature death rate per 100,000 is adjusted for various factors, including age of the population.',
            topIndicatorBarChart: barsHtml
        };
        pageHeadliningBarChart(pageHeadliningBarChartModel);
    }
}

function renderRanks() {
    var areaDetails = loaded.areaDetails.getData(getGroupId());
    var pageRanksModel = {
        rankingTableName: getRankingTableName(),
        rankingTableDate: areaDetails.Ranks[NATIONAL_CODE][0].Period
    };
    pageRanks(pageRanksModel);
}

function toggleDataHeaders(dataExists, areaName) {
    if (!dataExists) {
        //There is no data to display
        $('.data_page_content').hide();
        $('.info_box').hide();
        $('.verdict_box').hide();
        var areaText = ' - No data available';
    } else {
        //There is no data to display
        $('.data_page_content').show();
        $('.info_box').show();
        $('.verdict_box').show();
        areaText = '';
    }

    $('h1.area_name').html(areaName + areaText);
}

function groupColour(imgUrl, grade) {
    // Group colour   
    $('#similar_colour').html(
        '<img src="' + imgUrl + 'Mortality/' +
        grade + '.png" />'
    );
}

function getCauseBars(area, causeInfo, rankInfo, overallMax, causeClass,
    metadata, comparatorMethodId, indicatorSex) {

    var comparisonStatement = '';
    if (rankInfo && rankInfo.AreaRank) {
        var areaRank = rankInfo.AreaRank;
        comparisonStatement = getComparisonStatement(areaRank.Rank, rankInfo.Max.Rank);
    }

    // Whether to flip lowest and highest
    var flip = MT.model.profileId !== ProfileIds.Suicide && !useQuintiles(comparatorMethodId);

    var viewModel = {
        parentCode: MT.model.parentCode,
        areaCode: area.Code,
        areaRank: areaRank ? areaRank.Rank : 0,
        areaCardinal: areaRank ? getCardinal(areaRank.Rank) : '',
        causeKey: causeInfo.key,
        causeSig: causeClass,
        indicatorName: metadata.Descriptive.Name,
        barsHtml: getBars(rankInfo, overallMax, area, false, flip, metadata),
        rankHtml: comparisonStatement,
        isProfileDrugsAndAlcohol: isProfileDrugsAndAlcohol(),
        indicatorSex: indicatorSex
    };

    return templates.render('causeBars', viewModel);
}

function getCauseClass(sig, groupRoot) {
    var getGrade = getGradeFunction(groupRoot.ComparatorMethodId);
    var grade = getGrade(sig, groupRoot);

    return grade === '' ?
        'none' : grade;
}

function getJudgment(sig) {
    var suffix = ' than avg';
    switch (sig) {
        case 0:
            return 'Best';
        case 1:
            return 'Better' + suffix;
        case 2:
            return 'Worse' + suffix;
        case 3:
            return 'Worst';
    }
    return '';
}

function getBars(rankInfo, overallMax, area, isOverall, flip, indicatorMetaData) {

    if (!rankInfo) {
        // No data for any areas
        return templates.render('barItem', {
            level: '',
            barImage: "area-bar",
            val: '',
            unit: '',
            barWidth: 0,
            areaName: area.Name,
            label: '',
            message: 'NO DATA',
            labelClass: ''
        });
    }

    var min = rankInfo.Min,
        max = rankInfo.Max,
        template = 'barItem';
    var pixelsPerUnit = 470 /*available width in px*/ / overallMax;
    var lowestLabel = 'LOWEST';
    var highestLabel = 'HIGHEST';

    if (flip) {
        min = rankInfo.Max;
        max = rankInfo.Min;
    }

    // Is data defined for the current area?
    var areaRank = rankInfo.AreaRank;
    if (areaRank) {
        var areaVal = areaRank.Val;
        var areaValF = areaRank.ValF;
        var isMax = areaRank.Rank === max.Rank;
        var isMin = areaRank.Rank === min.Rank;
    } else {
        isMax = false;
        isMin = false;
        areaVal = 0;
        areaValF = "0";
    }

    var html = [];
    var indicatorUnit = new UnitFormat(indicatorMetaData).getLabel();

    //Change to charcoal overall bar if Yellow - ('Better than average')
    var mainRanking = $('#main_ranking');
    var overallBarGrade = '';

    if (mainRanking.hasClass('grade-1')) {
        overallBarGrade = 'overall-bar1';
    } else {
        overallBarGrade = 'overall-bar';
    }

    // Min bar
    html.push(
        templates.render(template, {
            level: 'low',
            barImage: isOverall ? overallBarGrade : "low-bar",
            val: min.ValF,
            unit: indicatorUnit,
            barWidth: min.Val * pixelsPerUnit,
            areaName: min.Area.Name,
            label: lowestLabel + ': ',
            message: isMin ? lowestLabel : null,
            labelClass: min.Val === 0 ? 'zero_val' : ''
        }));

    // Area label if required
    if (isMin) {
        var label = lowestLabel + ': ';
    } else if (isMax) {
        label = highestLabel + ': ';
    } else {
        label = '';
    }

    // Area bar
    html.push(
        templates.render(template, {
            level: '',
            barImage: isOverall ? overallBarGrade : "area-bar",
            val: areaValF,
            unit: indicatorUnit,
            barWidth: areaVal * pixelsPerUnit,
            areaName: area.Name,
            label: label,
            message: areaRank ? null : 'NO DATA',
            labelClass: areaVal === 0 ? 'zero_val' : ''
        }));

    // Max bar
    html.push(
        templates.render(template, {
            level: 'high',
            barImage: isOverall ? overallBarGrade : "high-bar",
            val: max.ValF,
            unit: indicatorUnit,
            barWidth: max.Val * pixelsPerUnit,
            areaName: max.Area.Name,
            label: highestLabel + ': ',
            message: isMax ? highestLabel : null,
            labelClass: max.Val === 0 ? 'zero_val' : ''

        }));

    return html.join('');
}

function showSimilarAreas() {
    if (!FT.ajaxLock) {
        lock();
        var model = MT.model;
        if (model.areaTypeId === AreaTypeIds.CountyUA) {
            model.parentCode = loaded.areaDetails.getData(getGroupId()).Decile.Code;
        } else {
            model.parentCode = onsClusterCode;
        }
        initPage();

        logEvent(AnalyticsCategories.DETAILS, AnalyticsAction.SIMILAR_AREAS);
    }
}

function showAllAreas() {
    if (!FT.ajaxLock) {
        lock();
        MT.model.parentCode = NATIONAL_CODE;
        initPage();

        logEvent(AnalyticsCategories.DETAILS, AnalyticsAction.ALL_AREAS);
    }

}

function selectAreaOption(id) {
    var className = 'selected';
    $('#area_display_options li').removeClass(className);
    $(id).addClass(className);
}

/*
* Applies a significance class after removing any that are already assigned
*/
function applySigClass(sigClass) {
    var jq = $('#main_ranking');
    removeAllGradeClasses(jq);
    jq.addClass(sigClass);
}

function getComparisonStatement(rank, maxRank) {

    var subtitle = isSimilarAreas() ?
        ' out of ' + maxRank + '<br>similar local<br>authorities' :
        ' out of ' + maxRank + '<br>local authorities';

    return [
        '<b>', rank,
        '<span>', getCardinal(rank),
        '</span></b> ', subtitle
    ].join('');
}

function getPopulation(rank) {
    var html = rank.AreaRank ?
        new CommaNumber(rank.AreaRank.Val).rounded() :
        NO_DATA;
    return html;
}

function populateSimilarAreasList() {

    var areaList = loaded.areaLists[MT.model.areaTypeId][MT.model.parentCode];

    $('#similar_areas_list').html(
        templates.render('similarAreas', {
            areas: areaList
        })
    );
}

function selectArea(code) {

    if (!FT.ajaxLock) {
        MT.model.areaCode = code;

        initPage();
    }
}

function setOverallCauseColour(id, className) {

    var header = $(id);

    if (!header.hasClass(className)) {
        removeAllGradeClasses(header);
        header.addClass(className);
    }
}

function getVerdictAndRank(grade, rank, ranks, overallIndex) {
    return templates.render('verdict', {
        judgement: getJudgment(grade),
        rank: rank + getCardinal(rank),
        total: ranks[overallIndex].Max.Rank
    });
}

function removeAllGradeClasses(jq) {
    for (var i = 1; i <= 4; i++) {
        jq.removeClass('grade-' + i);
    }
}

function getRankingTableName() {
    var tableName = '';
    if (MT.model.profileId === ProfileIds.HealthChecks) {
        tableName = "NHS Health Checks";
    } else if (MT.model.profileId === ProfileIds.DrugsAndAlcohol) {
        tableName = "Drugs and Alcohol";
    }
    return tableName;
}

function getGroupId() {
    var groupId = {};
    switch (MT.model.profileId) {
        case ProfileIds.DrugsAndAlcohol:
            groupId = { groupId: GroupIds.DrugsAndAlcohol.TreatmentAndRecovery };
            break;
        case ProfileIds.HealthChecks:
            groupId = { groupId: GroupIds.HealthChecks.HealthCheck };
            break;
        case ProfileIds.Cancer:
            groupId = { groupId: GroupIds.Cancer.IncidenceAndMortality };
            break;
        case ProfileIds.Suicide:
            groupId = { groupId: GroupIds.Suicide.SuicideData };
            break;
    }
    return groupId;
}

function getOverallMax(ranks, noOfIndicators) {
    var max = 0, min = 0;

    for (var i = 0; i < noOfIndicators; i++) {
        var rank = ranks[i];
        if (rank) {
            if (rank.Min.Val > min) {
                min = rank.Min.Val;
            }

            if (rank.Max.Val > max) {
                max = rank.Max.Val;
            }
        }
    }

    return max > min ? max : min;
}

function updatePage() {
    // Don't remove. required for SiteDiabetes.js
}

NO_DATA = 'n/a';

templates.add('barItem', '{{#message}}<li class="high_low_message">{{message}}</li>{{/message}}\
{{^message}}<li class="{{level}}"><div class="bar_max"><div class="bar" style="width:{{barWidth}}px;"><img src="' + FT.url.img + 'Mortality/{{barImage}}.png" style="width:{{barWidth}}px; height:27px;" /><span class="value {{labelClass}}">{{val}}{{{unit}}}</span></div></div><p>{{label}}{{areaName}}</p></li>{{/message}}'
);

templates.add('causeBars', '<tr id="{{causeKey}}_row" class="{{causeSig}}">\
<td class="col1">{{#isProfileDrugsAndAlcohol}}<div class="drugs-n-alcohol">{{{rankHtml}}}</div>{{/isProfileDrugsAndAlcohol}}{{^isProfileDrugsAndAlcohol}}<div><img src="' + FT.url.img + 'health-checks/icons-large.png" alt="" height="149px;"/></div>{{{rankHtml}}}{{/isProfileDrugsAndAlcohol}}</td>\
<td class="col2" style="width:540px;"><h3>{{indicatorName}}{{indicatorSex}}</h3><ul class="bar_chart">{{{barsHtml}}}</ul>\
</td></tr>');

templates.add('similarAreas', '{{#areas}}<li><a href="javascript:selectArea(\'{{Code}}\')">{{Name}}</a></li>{{/areas}}');

templates.add('verdict', '<span>{{judgement}}</span> | {{rank}} out of {{total}}');


function getOnsClusterName(onsClusterCode) {

    return loaded.areaDetails.getData({ areaCode: onsClusterCode }).Area.Name;
}

function pageHeader(model) {

    // Define template and view model
    var pageHeaderTemplate, viewModel;
    if (MT.model.profileId === ProfileIds.HealthChecks) {
        pageHeaderTemplate = '<h1 class="area_name">{{areaName}}</h1>' +
                '<div id="c1" class="info_box_3">' +
                '<h2><strong>{{eligibleEnglang}}</strong> people eligible for an NHS Health Check in England</h2>' +
                '</div>' +
                '<div id="c2" class="info_box_3">' +
                '<h2><strong>{{eligibleThisArea}}</strong> people eligible for an NHS Health Check in this area</h2>' +
                '</div>' +
                '<div id="c3" class="info_box_3">' +
                '<h2>{{{preventableCvdMortality}}} preventable cardio-vascular disease deaths in this area</h2>' +
                '</div>' +
                '</div>';

        viewModel = {
            areaName: model.areaName,
            eligibleEnglang: model.eligibleEnglang,
            eligibleThisArea: model.eligibleThisArea,
            preventableCvdMortality: model.preventableCvdMortality
        };
    } else if (MT.model.profileId === ProfileIds.Suicide) {
        pageHeaderTemplate = '<h1 class="area_name">{{areaName}}</h1><div class="hr"></div><h2 style="margin:0;float:left;">{{plan}}</div><h2 style="float:right;margin:0;">{{suicides}}</h2>';

        var areaDetails = loaded.areaDetails.getData(getGroupId());

        var ranks = areaDetails.Ranks[NATIONAL_CODE];

        // Find index of suicide plan indicator
        var planIndex = 0;
        for (var i in groupRoots) {
            if (groupRoots[i].IID === IndicatorIds.SuicidePlan) {
                planIndex = i;
                break;
            }
        }

        // Suicide plan in place
        var plan = (ranks[planIndex].AreaRank)
            ? ranks[planIndex].AreaRank.ValF
            : NO_SUICIDE_PLAN;

        // Suicides for area
        var indexOfPersonSuicides = 0;
        if (ranks[indexOfPersonSuicides].AreaRank) {
            var suicides = ranks[indexOfPersonSuicides].AreaRank.Count + ' suicides (' + ranks[indexOfPersonSuicides].Period + ')';
        } else {
            suicides = '';
        }

        viewModel = {
            areaName: model.areaName,
            plan: plan,
            suicides: suicides
        };
    } else {
        pageHeaderTemplate = '<h1 class="area_name">{{areaName}}</h1><div class="hr">';
        viewModel = {
            areaName: model.areaName
        };
    }

    // Render HTML
    templates.add('page-header', pageHeaderTemplate);
    var html = templates.render('page-header', viewModel);
    $('#data_page_header').html(html);
}

function isProfileDrugsAndAlcohol() {
    return MT.model.profileId === ProfileIds.DrugsAndAlcohol;
}

function pageHeadliningBarChart(model) {

    var viewModel = {
        topIndicatorSig: model.topIndicatorSig,
        topIndicatorHeading: model.topIndicatorHeading,
        topIndicatorRanking: model.topIndicatorRanking,
        topIndicatorDate: model.topIndicatorDate,
        topIndicatorToolTip: model.topIndicatorToolTip,
        topIndicatorBarChart: model.topIndicatorBarChart
    };

    var template = '<div id="main_ranking" class="clearfix {{topIndicatorSig}}">' +
        '<h3>{{topIndicatorHeading}} <span>per 100,000 for {{{topIndicatorDate}}}</span><span class="tooltip tooltip-inverse"><i>{{topIndicatorToolTip}}</i></span></h3>' +
        '<div class="ranking">{{{topIndicatorRanking}}}</div>' +
        '<ul class="bar_chart">' +
        '{{{topIndicatorBarChart}}}' +
        '</ul>' +
        '</div>';

    templates.add('page-headling-indicator-bar-chart', template);
    var html = templates.render('page-headling-indicator-bar-chart', viewModel);
    $('#headline_bar_chart').html(html);
}

function pageLegend(model) {

    var viewModel = {
        contentHeading: model.contentHeading,
        isSimilarViewMode: model.isSimilarViewMode,
        areaName: model.areaName,
        imgUrl: FT.url.img,
        useBob: MT.model.profileId === ProfileIds.Suicide
    };

    var pageLegendTemplate = '{{#isSimilarViewMode}}<h2>{{contentHeading}}</h2>{{/isSimilarViewMode}}' +
        '{{^isSimilarViewMode}}<h2>All local authorities</h2>{{/isSimilarViewMode}}' +
        '{{#isSimilarViewMode}}<p class="ranking_note"><b>Similar view:</b> <span>{{areaName}}</span>&apos;s rank within the local authorities in the same <span class="area-bracket" id="comparisondd"></span>.</p>{{/isSimilarViewMode}}' +
        '{{^isSimilarViewMode}}<p class="ranking_note"><b>National view:</b> <span>{{areaName}}</span>&apos;s rank within local authorities in England.</p>{{/isSimilarViewMode}}' +
        '<p class="legend"> Comparison with the average' +
        '<span class="grade"><img src="{{imgUrl}}Mortality/{{#useBob}}bobLower.png" alt="lower" />lower{{/useBob}}{{^useBob}}grade-3.png" alt="worse" />worse{{/useBob}}</span>' +
        '<span class="grade"><img src="{{imgUrl}}Mortality/grade-2.png" alt="consistent" />consistent</span>' +
        '<span class="grade"><img src="{{imgUrl}}Mortality/{{#useBob}}bobHigher.png" alt="higher" />higher{{/useBob}}{{^useBob}}grade-0.png" alt="better" />better{{/useBob}}</span>' +
        '</p>';

    templates.add('page-legend', pageLegendTemplate);
    var html = templates.render('page-legend', viewModel);
    $('#legend').html(html);
}

function pageRanks(model) {
    var pageRanksTempl =
        '<table id="data_page_table" style="display:none;">' +
        '<thead><tr>' +
        '<th class="col1"><div><span>Rank</span></div></th>' +
        '<th class="col2" style="width:540px;"><div><span>{{rankingTableName}}</span> &nbsp;&nbsp; {{rankingTableDate}}</div></th>' +
        '</tr></thead>' +
        '<tbody id="cause_bars"></tbody>' +
        '</table>' +
        '<ul class="more_info clearfix no-print">' +
        '<li style="width:100%" ><a href="javascript:MT.nav.nationalRankings();">View full rankings</a></li>' +
        '</ul>';
    templates.add('page-ranks', pageRanksTempl);
    var htmlOut = templates.render('page-ranks', {
        rankingTableName: model.rankingTableName,
        rankingTableDate: model.rankingTableDate
    });
    $('#ranking_bar_charts').html(htmlOut);
}

function renderRelatedLinks(url) {
    if (MT.model.profileId === ProfileIds.HealthChecks) {
        $('#la_link').attr('href', url);
    }
}

function getCauseOptions() {
    if (MT.model.profileId === ProfileIds.HealthChecks) {
        // Keys to provide hooks for specific styling of each row
        var options = [
            { key: 'people_invited' },
            { key: 'people_receiving' },
            { key: 'people_taking_up' }
        ];
    } else {
        // Do not need to associate styling with individual rows
        options = [
        { key: 'a' },
        { key: 'b' },
        { key: 'c' },
        { key: 'd' },
        { key: 'e' },
        { key: 'f' },
        { key: 'g' }
        ];
    }
    return options;
}

causeDetails = {
    people_invited: {},
    people_receiving: {},
    people_taking_up: {},
    a: {}, b: {}, c: {}, d: {}, e: {}, f: {}, g: {}
};

CVD_INDICATOR_INDEX = 4;
