'use strict';

var headlineIndicator;

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

    // Useful variables
    var model = MT.model;
    var areaDetails = loaded.areaDetails.getData(getGroupId());
    var area = areaDetails.Area;
    var areaName = area.Name;

    displayListOfSimilarAreas();
    displaySelectedCompareOption();
    $('#legend-header').html("Compared to other areas");
    displayAreaRangeElements(!isSimilarAreas());

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

        var isData = rank && rank.AreaRank;

        // Should show significance
        var metadata = metadataHash[root.IID];
        var isSummaryRank = isSummaryIndicator(metadata);
        var showSignificance = model.profileId === ProfileIds.PublicHealthDashboard
            ? isData && isSummaryRank
            : isData;

        // Significance colouring
        var causeClass = showSignificance
            ? getCauseClass(significances[i], root)
            : 'none';

        // Significance text
        var significanceText = showSignificance
            ? getSignificanceText(significances[i], root)
            : '';

        if (isFeatureEnabled('enablePhdLinkToFingertips')) {
            if (MT.model.profileId === ProfileIds.PublicHealthDashboard && !isSummaryRank) {
                if (i > 0) {
                    significanceText = getViewTrendLink(MT.model, root);
                }
            }
        }
        var causeBar = getCauseBars(area, causeInfo, rank, causeClass,
            metadata, root.ComparatorMethodId, sexForCurrectIndicator, significanceText);

        html.push(causeBar);
    }

    // Show table element before HTML rendered to prevent IE8 difficulties
    var $onsGroup = $('.aboutONSGroup');
    var similarAreaListText = '';
    var similarAreaSupportingText = '';
    if (isNearestNeighbour()) {
        $onsGroup.hide();
        $('.aboutDecileGroup').show();
        similarAreaSupportingText = areaName + '&apos;s rank within its CIPFA nearest neighbours (most similar local authorities)';
        similarAreaListText = 'Local authorities that are CIPFA nearest neighbours of ' + areaName;
    } else {
        if (model.areaTypeId === AreaTypeIds.CountyUA) {
            $onsGroup.hide();
            $('.aboutDecileGroup').show();
            similarAreaSupportingText = areaName + '&apos;s rank within its IMD(2015) decile group';
            similarAreaListText = 'Local authorities in this Deprivation group';
        } else {
            $('.aboutDecileGroup').hide();
            if (isSimilar) {
                $('#ons_name').html(getOnsClusterName(model.parentCode));
                $onsGroup.show();
            } else {
                $onsGroup.hide();
            }
            similarAreaSupportingText = areaName + '&apos;s rank within the local authorities in the same ONS cluster';
            similarAreaListText = 'Local authorities in the same ONS cluster group';
        }
    }
    $('#similar-area-list-text').html(similarAreaListText);
    $('#similar-area-supporting-text').html(similarAreaSupportingText);

    $('#data_page_table, .more_info, .similar_authorities').show();
    $('#cause_bars').html(html);

    toggleDataHeaders(true, areaName);

    removeLoadingSpinner();
    if (MT.model.profileId === ProfileIds.PublicHealthDashboard) {
        selectDomainOption();
    }
    unlock();
}

function displaySelectedCompareOption() {

    // Highlight compare option
    var areaOptionId;
    if (isSimilarAreas()) {
        if (isNearestNeighbour()) {
            areaOptionId = 'show_nearest_neighbours';
        } else {
            areaOptionId = 'show_similar_areas';
        }
    } else {
        areaOptionId = 'show_all_areas';
    }
    selectAreaOption('#' + areaOptionId);
}

function displayListOfSimilarAreas() {
    if (isSimilarAreas()) {

        var template;
        var areaList = loaded.areaLists[MT.model.areaTypeId][MT.model.parentCode];
        if (isNearestNeighbour()) {
            // Reorder area list by neighbour rank
            var altModel = getGroupId();
            altModel.areaCode = MT.model.parentCode;
            var areaDetailsParent = loaded.areaDetails.getData(altModel);
            var neighbourAreaCodes = areaDetailsParent.Area.NeighbourAreaCodes;
            areaList = sortAreaListByNeighbourCodes(neighbourAreaCodes, areaList);
            template = 'similarAreasOrdered';
        } else {
            template = 'similarAreasUnordered';
        }

        populateSimilarAreasList(areaList, template);
    }
}

function sortAreaListByNeighbourCodes(neighbourCodes, areaList) {

    var areaHash = {};
    for (var i in areaList) {
        areaHash[areaList[i].Code] = areaList[i];
    }

    var sortedList = [];
    for (var j in neighbourCodes) {
        sortedList.push(areaHash[neighbourCodes[j]]);
    }

    return sortedList;
}

function isSummaryIndicator(metadata) {
    return metadata.Descriptive.Name.indexOf('summary rank') > -1 ||
        metadata.IID === IndicatorIds.LivingInAqmas;
}

// First indicator with bars for Health Checks
function HeadlineIndicator() {

    var overallIndex = 4;/* CVD indicator index*/

    var areaDetails = loaded.areaDetailsForDiseaseAndDeath.getData(
        { groupId: GroupIds.HealthChecks.DiseaseAndDeath });

    var parentCode = MT.model.parentCode;
    var ranks = areaDetails.Ranks[parentCode];
    var significances = areaDetails.Significances[parentCode];
    var areaRank = ranks[overallIndex];

    this.ranks = areaDetails.Ranks;
    this.areaRank = areaRank;
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

        var areaRank = headlineIndicator.ranks[NATIONAL_CODE][overallIndex].AreaRank;
        var preventableCvdMortality = areaRank && areaRank.Val !== -1 ?
            headlineIndicator.ranks[NATIONAL_CODE][overallIndex].AreaRank.Count : 0;

        var preventableCvdMortalityFormatted = new CommaNumber(preventableCvdMortality).rounded();

        pageHeaderModel = {
            areaName: headlineIndicator.areaName,
            eligibleEnglang: eligibleInEnglandFormated,
            eligibleThisArea: eligibleInThisAreaFormated,
            preventableCvdMortality: preventableCvdMortality === 0
                ? 'No data for '
                : '<strong>' + preventableCvdMortalityFormatted + '</strong>'
        };
    } else {
        pageHeaderModel = {
            areaName: areaDetails.areaName
        };
    }
    pageHeader(pageHeaderModel);
}

function renderLegend(areaName) {

    var isSimilarView = MT.model.parentCode !== NATIONAL_CODE;

    var contentHeading = '';
    if (isSimilarView) {
        contentHeading = isNearestNeighbour() ? 'Similar local authorities' : 'Deprivation group';
    }

    var pageLegendModel = {
        contentHeading: contentHeading,
        isSimilarViewMode: isSimilarView,
        areaName: areaName
    };
    pageLegend(pageLegendModel);
}

function renderHeadliningBarChart() {
    if (MT.model.profileId === ProfileIds.HealthChecks) {
        var metadataHash = loaded.indicatorMetadata[MT.model.groupId];

        var barsHtml = getBars(headlineIndicator.areaRank,
            headlineIndicator.area, true, false, metadataHash[headlineIndicator.overallIndex]);

        var areaRank = headlineIndicator.areaRank;
        var comparisonStatement = areaRank.AreaRank !== null && areaRank.AreaRank.Rank !== null
            ? getComparisonStatement(areaRank.AreaRank.Rank, areaRank.Max.Rank, true)
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

function getCauseBars(area, causeInfo, rankInfo, causeClass,
    metadata, comparatorMethodId, indicatorSex, significanceText) {

    // Whether or not to flip the data
    var displayMaxFirst = false;

    // Statement to left of bars, e.g. 41st of 150 local authorities
    var comparisonStatement = '';
    if (rankInfo) {
        if (rankInfo.AreaRank && rankInfo.AreaRank.Rank) {
            comparisonStatement = getComparisonStatement(rankInfo.AreaRank.Rank,
                rankInfo.Max.Rank,
                true);
        }

        // Display best value first for PHD
        if (MT.model.profileId === ProfileIds.PublicHealthDashboard) {
            if (isFeatureEnabled('useLongerLivesBestWorstLabels')) {
                // Larc indicator is BOB but high is good
                displayMaxFirst = rankInfo.IID === IndicatorIds.LarcPrescribed;
            } else {
                // Display minimum first and reverse the polarity switch made in WS
                displayMaxFirst = rankInfo.PolarityId === PolarityIds.RAGHighIsGood;
            }
        }

        var period = rankInfo.Period;
    } else {
        period = '';
    }

    var viewModel = {
        sigLabel: significanceText,
        showSigLabel: MT.model.profileId !== ProfileIds.HealthChecks,
        causeKey: causeInfo.key,
        causeSig: causeClass,
        indicatorName: replacePercentageWithArialFont(metadata.Descriptive.Name),
        barsHtml: getBars(rankInfo, area, false, displayMaxFirst, metadata),
        rankHtml: comparisonStatement,
        indicatorSex: indicatorSex,
        period: period
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

function getBars(rankInfo, area, isOverall, displayMaxFirst, metadata) {

    var template = 'barItem';

    if (!rankInfo) {
        // No data for any areas
        return templates.render(template, {
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

    // Set min and max
    var min, max;
    if (displayMaxFirst) {
        min = rankInfo.Max;
        max = rankInfo.Min;
    } else {
        min = rankInfo.Min;
        max = rankInfo.Max;
    }

    // Bar labels
    var lowestLabel, highestLabel;
    if (MT.model.profileId === ProfileIds.PublicHealthDashboard &&
        isFeatureEnabled('useLongerLivesBestWorstLabels')) {
        lowestLabel = 'BEST';
        highestLabel = 'WORST';
    } else {
        if (min.Val < max.Val) {
            lowestLabel = 'LOWEST';
            highestLabel = 'HIGHEST';
        } else {
            lowestLabel = 'HIGHEST';
            highestLabel = 'LOWEST';
        }
    }

    var valueNotes = getValueNotes(rankInfo);
    var viewModel = {
        isMax: false,
        isMin: false,
        areaVal: 0,
        areaValF: "0",
        minVal: min.Val,
        minValF: min.ValF,
        maxVal: max.Val,
        maxValF: max.ValF,
        areaValueNote: valueNotes.areaValueNote,
        minValueNote: valueNotes.minValueNote,
        maxValueNote: valueNotes.maxValueNote
    };

    var shouldDisplayRank = MT.model.profileId === ProfileIds.PublicHealthDashboard &&
        metadata.ValueType.Id === ValueTypeIds.Score;

    if (shouldDisplayRank) {
        // Overwrite min/max with ranks instead of values
        viewModel.minVal = min.Rank;
        viewModel.minValF = min.Rank.toString();
        viewModel.maxVal = max.Rank;
        viewModel.maxValF = max.Rank.toString();
    }

    // Is data defined for the current area?
    var areaRank = rankInfo.AreaRank;
    if (areaRank) {

        // Value to display
        if (areaRank.Val === -1) {
            // No value
            viewModel.areaVal = 0;
            viewModel.areaValF = '';
        } else {
            if (shouldDisplayRank) {
                // Ues ranks instead of values
                viewModel.areaVal = areaRank.Rank;
                viewModel.areaValF = areaRank.Rank.toString();
            } else {
                viewModel.areaVal = areaRank.Val;
                viewModel.areaValF = areaRank.ValF;
            }
        }

        viewModel.isMax = areaRank.Rank === max.Rank;
        viewModel.isMin = areaRank.Rank === min.Rank;
    }

    var html = [];
    var indicatorUnit = new UnitFormat(metadata).getLabel();

    //Change to charcoal overall bar if Yellow - ('Better than average')
    var mainRanking = $('#main_ranking');
    var overallBarGrade = '';

    if (mainRanking.hasClass('grade-1')) {
        overallBarGrade = 'overall-bar1';
    } else {
        overallBarGrade = 'overall-bar';
    }

    // Calculate pixels per unit 
    var availableWidthInPixels = 400;
    var maxVal = new MinMaxFinder([viewModel.minVal, viewModel.maxVal]).max;
    var pixelsPerUnit = availableWidthInPixels / maxVal;

    // Min bar
    html.push(
        templates.render(template, {
            level: 'low',
            barImage: isOverall ? overallBarGrade : "low-bar",
            val: viewModel.minValF,
            unit: indicatorUnit,
            barWidth: viewModel.minVal * pixelsPerUnit,
            areaName: min.Area.Name,
            label: lowestLabel + ': ',
            message: viewModel.isMin ? lowestLabel : null,
            labelClass: viewModel.minVal < 1 ? 'zero_val' : '',
            valueNote: viewModel.minValueNote
        }));

    // Area label if required
    if (viewModel.isMin) {
        var areaLabel = lowestLabel + ': ';
    } else if (viewModel.isMax) {
        areaLabel = highestLabel + ': ';
    } else {
        areaLabel = '';
    }

    // Area bar
    html.push(
        templates.render(template, {
            level: '',
            barImage: isOverall ? overallBarGrade : "area-bar",
            val: viewModel.areaValF,
            unit: indicatorUnit,
            barWidth: viewModel.areaVal * pixelsPerUnit,
            areaName: area.Name,
            label: areaLabel,
            message: areaRank && areaRank.Val !== -1 ? null : 'NO DATA',
            labelClass: viewModel.areaVal < 1 ? 'zero_val' : '',
            valueNote: viewModel.areaValueNote
        }));

    // Max bar
    html.push(
        templates.render(template, {
            level: 'high',
            barImage: isOverall ? overallBarGrade : "high-bar",
            val: viewModel.maxValF,
            unit: indicatorUnit,
            barWidth: viewModel.maxVal * pixelsPerUnit,
            areaName: max.Area.Name,
            label: highestLabel + ': ',
            message: viewModel.isMax ? highestLabel : null,
            labelClass: viewModel.maxVal < 1 ? 'zero_val' : '',
            valueNote: viewModel.maxValueNote

        }));

    return html.join('');
}

function showNearestNeighbours() {
    if (!FT.ajaxLock) {
        lock();

        // Set model
        var model = MT.model;
        model.parentCode = getNearestNeighbourCode();
        model.similarAreaCode = model.parentCode;

        initPage();

        logEvent(AnalyticsCategories.DETAILS, AnalyticsAction.SIMILAR_AREAS);
    }
}

function showSimilarAreas() {
    if (!FT.ajaxLock) {
        lock();

        // Set model
        var model = MT.model;
        if (model.areaTypeId === AreaTypeIds.DistrictUA) {
            model.parentCode = getOnsCodeForArea(model.areaCode);
        } else {
            model.parentCode = loaded.areaDetails.getData(getGroupId()).Decile.Code;
        }
        model.similarAreaCode = model.parentCode;

        initPage();

        logEvent(AnalyticsCategories.DETAILS, AnalyticsAction.SIMILAR_AREAS);
    }
}

function showAllAreas() {
    if (!FT.ajaxLock) {
        lock();
        MT.model.parentCode = NATIONAL_CODE;
        MT.model.similarAreaCode = null;

        initPage();

        logEvent(AnalyticsCategories.DETAILS, AnalyticsAction.ALL_AREAS);
    }

}

function selectAreaOption(id) {
    var className = 'selected';
    $('#area_display_options li').removeClass(className);
    $(id).addClass(className);
}

function selectDomainOption() {
    var className = 'selected';
    $('#domain_display_options li').removeClass(className);
    $('#domain-' + MT.model.groupId).addClass(className);
}


/*
* Applies a significance class after removing any that are already assigned
*/
function applySigClass(sigClass) {
    var jq = $('#main_ranking');
    removeAllGradeClasses(jq);
    jq.addClass(sigClass);
}

function getComparisonStatement(rank, maxRank, includeCardinal) {

    var subtitle = isSimilarAreas() ?
        ' out of ' + maxRank + '<br>similar local<br>authorities' :
        ' out of ' + maxRank + '<br>local authorities';

    return [
        '<b>', rank,
        '<span>', includeCardinal ? getCardinal(rank) : '',
        '</span></b> ', subtitle
    ].join('');
}

function getPopulation(rank) {
    var html = rank.AreaRank ?
        new CommaNumber(rank.AreaRank.Val).rounded() :
        NO_DATA;
    return html;
}

function populateSimilarAreasList(areaList, template) {

    $('#similar_areas_list').html(
        templates.render(template, {
            areas: areaList
        })
    );
}

function selectArea(code) {

    if (!FT.ajaxLock) {
        MT.model.areaCode = code;

        if (isNearestNeighbour()) {
            MT.model.parentCode = getNearestNeighbourCode();
        }

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
    if (MT.model.profileId === ProfileIds.HealthChecks) {
        var tableName = "NHS Health Checks";
    } else if (MT.model.profileId === ProfileIds.DrugsAndAlcohol) {
        tableName = "Drugs and Alcohol";
    } else {
        tableName = "Indicator data";
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
        case ProfileIds.PublicHealthDashboard:
            groupId = { groupId: MT.model.groupId };
            break;
    }
    return groupId;
}

function updatePage() {
    // Don't remove. required for SiteDiabetes.js
}

var NO_DATA = 'n/a';

templates.add('barItem', '{{#message}}<li class="high_low_message">{{message}}<span>{{{valueNote}}}</span></li>{{/message}}\
{{^message}}<li class="{{level}}"><div class="bar_max"><div class="bar" style="width:{{barWidth}}px;"><img src="' + FT.url.img + 'Mortality/{{barImage}}.png" style="width:{{barWidth}}px; height:27px;" /><span class="value {{labelClass}}">{{val}}{{{unit}}}<span>{{{valueNote}}}</span></span></div></div><p>{{label}}{{areaName}}</p></li>{{/message}}'
);

templates.add('causeBars', '<tr id="{{causeKey}}_row" class="{{causeSig}}"><td class="col1">\
<div style="margin-top: 25px;">{{{rankHtml}}}</div>\
{{^showSigLabel}}<div class="image"><img src="' + FT.url.img + 'health-checks/icons-large.png" alt="" height="149px;"/></div>{{/showSigLabel}}\
{{#showSigLabel}}<div class="sig-label"> {{{sigLabel}}}</div>{{/showSigLabel}}</td>\
<td class="col2" style="width:540px;">' +
    '<h3>{{{indicatorName}}}{{indicatorSex}}{{#period}} ({{period}}){{/period}}</h3>' +
    '<ul class="bar_chart">{{{barsHtml}}}</ul>\
</td></tr>');

templates.add('similarAreasOrdered', '<ol>{{#areas}}<li><a href="javascript:selectArea(\'{{Code}}\')">{{Name}}</a></li>{{/areas}}</ol>');
templates.add('similarAreasUnordered', '<ul>{{#areas}}<li><a href="javascript:selectArea(\'{{Code}}\')">{{Name}}</a></li>{{/areas}}</ul>');

templates.add('verdict', '<span>{{judgement}}</span> | {{rank}} out of {{total}}');

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
            var suicides = ranks[indexOfPersonSuicides].AreaRank.Count +
                ' suicides (' + ranks[indexOfPersonSuicides].Period + ')';
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
        useBob: MT.model.profileId === ProfileIds.Suicide,
        showLegend: MT.model.profileId === ProfileIds.HealthChecks
    };

    var pageLegendTemplate = '{{#isSimilarViewMode}}<h2>{{contentHeading}}</h2>{{/isSimilarViewMode}}' +
        '{{^isSimilarViewMode}}<h2>All local authorities</h2>{{/isSimilarViewMode}}' +
        '{{#isSimilarViewMode}}<p class="ranking_note"><b>Similar view:</b> <span id="similar-area-supporting-text"></span></p>{{/isSimilarViewMode}}' +
        '{{^isSimilarViewMode}}<p class="ranking_note"><b>National view:</b> <span>{{areaName}}</span>&apos;s rank within local authorities in England.</p>{{/isSimilarViewMode}}' +
        '{{#showLegend}}<p class="legend"> Comparison with the average' +
        '<span class="grade"><img src="{{imgUrl}}Mortality/{{#useBob}}bobLower.png" alt="lower" />lower{{/useBob}}{{^useBob}}grade-3.png" alt="worse" />worse{{/useBob}}</span>' +
        '<span class="grade"><img src="{{imgUrl}}Mortality/grade-2.png" alt="consistent" />consistent</span>' +
        '<span class="grade"><img src="{{imgUrl}}Mortality/{{#useBob}}bobHigher.png" alt="higher" />higher{{/useBob}}{{^useBob}}grade-0.png" alt="better" />better{{/useBob}}</span>' +
        '</p>{{/showLegend}}';

    templates.add('page-legend', pageLegendTemplate);
    var html = templates.render('page-legend', viewModel);
    $('#legend').html(html);
}

function renderRanks() {

    var pageRanksTempl =
        '<table id="data_page_table" style="display:none;">' +
        '<thead><tr>' +
        '<th class="col1"><div><span>Rank</span></div></th>' +
        '<th class="col2"><div><span>{{rankingTableName}}</span></div></th>' +
        '</tr></thead>' +
        '<tbody id="cause_bars"></tbody>' +
        '</table>' +
        '<ul class="more_info clearfix no-print">' +
        '<li style="width:100%" ><a href="javascript:MT.nav.nationalRankings();">View full rankings</a></li>' +
        '</ul>';
    templates.add('page-ranks', pageRanksTempl);

    var model = {
        rankingTableName: getRankingTableName()
    };

    var html = templates.render('page-ranks', {
        rankingTableName: model.rankingTableName
    });

    $('#ranking_bar_charts').html(html);
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
        { key: 'g' },
        { key: 'h' },
        { key: 'i' }
        ];
    }
    return options;
}

function selectDomain(gid) {
    MT.model.groupId = gid;
    initPage();
}

function getValueNotes(data) {

    var getText = function (areaRank) {
        if (!areaRank) {
            return '';
        }

        var valueNote = areaRank.ValueNote;
        return valueNote && valueNote.Id > 0
            ? '<span class="tooltip"><i>' + valueNote.Text + '</i></span>'
            : '';
    }

    var areaValueNote = data.AreaRank ? getText(data.AreaRank) : '';

    return {
        areaValueNote: areaValueNote,
        minValueNote: data.Max.ValueNote,
        maxValueNote: data.Min.ValueNote
    };
}

function getViewTrendLink(model, root) {
    var baseUrl = 'https://fingertips.phe.org.uk/',
    link = baseUrl + profileUrlKey + "-ft" + "#page/4";
    link += "/gid/" + model.groupId;
    link += "/pat/" + model.parentTypeId;
    link += "/par/" + model.parentCode;
    link += "/ati/" + model.areaTypeId;
    link += "/are/" + model.areaCode;
    link += "/iid/" + root.IID;
    link += "/age/" + root.Age.Id;
    link += "/sex/" + root.Sex.Id;
    
    return '<a style="text-transform:none;color:#2e3191;"  href="' + link + '" target="_blank">View trend</a>';
}


var causeDetails = {
    people_invited: {},
    people_receiving: {},
    people_taking_up: {},
    a: {}, b: {}, c: {}, d: {}, e: {}, f: {}, g: {}
};
