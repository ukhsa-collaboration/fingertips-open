function initPage() {
    lock();
    updateModelFromHash();

    var model = MT.model;
    var isSimilar = isSimilarAreas();

    var callCount = 6;
    if (isSimilar) {
        callCount += 2;
    }
    ajaxMonitor.setCalls(callCount);

    loaded.areaDetails.fetchDataByAjax();
    getIndicatorMetadata(model.groupId);
    getGroupRoots(model);
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
        model = MT.model,
        nationalAreaCode = NATIONAL_CODE,
        imgUrl = FT.url.img;

    if (isSimilar) {
        populateSimilarAreasList();
        selectAreaOption('#show_similar_areas');
    } else {
        selectAreaOption('#show_all_areas');
    }

    displayAreaRangeElements(!isSimilar);

    var areaDetails = loaded.areaDetails.getData();
    var significances = areaDetails.Significances[nationalAreaCode];
    var overallIndex = ROOT_INDEXES.OVERALL_MORTALITY;
    var ranks = areaDetails.Ranks[nationalAreaCode];
    var similarVerdict, similarVerdictColor;

    // Area name
    var area = areaDetails.Area;
    var areaName = area.Name;
    // Used to set the areaname in breadcrumb
    $('.area_name').html(areaName);
    // LA link
    showLocalAuthorityWebsiteLink(areaDetails.Url);

    // National overall mortality
    areaRank = ranks[overallIndex].AreaRank;
    if (areaRank) {
        var totalDeathCount = new CommaNumber(areaRank.Count).rounded();

        // National rank and judgement
        var nationalVal = areaDetails.Benchmarks[nationalAreaCode][overallIndex].Val;
        var getGrade = getGradeFunction(nationalVal);


        var grade = getGrade(significances[overallIndex], areaRank.Val);
       
        var rank = areaRank.Rank;
        var nationalVerdict = getVerdictAndRank(grade, rank, ranks, overallIndex);
        var nationalVerdictColor = '<img src="' + imgUrl + 'Mortality/grade-' + grade + '.png" />';

        // Area grouping
        var decileNumber = areaDetails.Decile.Number,
            decileInfo = getDecileInfo(decileNumber);
        $('.decile_text').html(decileInfo.text);
        $('#decile_number').html(decileNumber);
        // Purple deprivation man
        $('#socioeconomic_ib').addClass('level' + decileInfo.quintile);


        if (model.areaTypeId == AreaTypeIds.CountyUA) {
            // Decile judgement and rank
            var decileCode = areaDetails.Decile.Code,
                decileRanks = areaDetails.Ranks[decileCode],
                areaDecileRank = decileRanks[overallIndex].AreaRank;
            significances = areaDetails.Significances[decileCode];
            var decileRank = areaDecileRank.Rank;

            //Judgements and ranks
            var decileVal = areaDetails.Benchmarks[decileCode][overallIndex].Val;
            getGrade = getGradeFunction(decileVal, true);

            grade = getGrade(significances[overallIndex], areaRank.Val);

            similarVerdict = getVerdictAndRank(grade, decileRank, decileRanks, overallIndex);
            similarVerdictColor = '<img src="' + imgUrl + 'Mortality/grade-' + grade + '.png" />';
        } else {
            var onsClusterCode = getOnsCodeForArea(area.Code);

            // ONS judgement and rank
            var onsRanks = areaDetails.Ranks[onsClusterCode];
            var areaOnsRank = onsRanks[overallIndex].AreaRank;
            significances = areaDetails.Significances[onsClusterCode];
            var onsRank = areaOnsRank.Rank;

            //Judgements and ranks
            var onsValue = areaDetails.Benchmarks[onsClusterCode][overallIndex].Val;
            getGrade = getGradeFunction(onsValue, true);
            grade = getGrade(significances[overallIndex], areaRank.Val);
            similarVerdict = getVerdictAndRank(grade, onsRank, onsRanks, overallIndex);
            similarVerdictColor = '<img src="' + imgUrl + 'Mortality/grade-' + grade + '.png" />';
        }

        // START OF BAR CHART SECTIONS    
        var overallMax = ranks[overallIndex].Max.Val;

        // Parent specific
        var parentCode = model.parentCode;
        significances = areaDetails.Significances[parentCode];

        // Overall mortality
        ranks = areaDetails.Ranks[parentCode];
        areaRank = ranks[overallIndex];
        var causeClass = getCauseClass(significances[overallIndex],
            areaRank.AreaRank.Val,
            areaDetails.Benchmarks[parentCode][overallIndex].Val);

        applySigClass(causeClass);
        // Render Header
        var pageHeaderModel = {
            areaName: areaName,
            population: getPopulation(ranks[ROOT_INDEXES.POPULATION]),
            totalPrematureDeathsCount: totalDeathCount,
            nationalVerdict: nationalVerdict,
            nationalVerdictColor: nationalVerdictColor,
            similarVerdict: similarVerdict,
            similarVerdictColor: similarVerdictColor,
            period: areaRank.Period.replace(/ /g, '')
        };
        pageHeader(pageHeaderModel);

        var pageContentModel = {
            contentHeading: 'Similar local authorities',
            isSimilarViewMode: MT.model.parentCode == nationalAreaCode ? false : true,
            comparisonRank: areaRank.Max.Rank,
            areaName: areaName,
            topIndicatorSig: causeClass,
            topIndicatorHeading: 'Overall premature deaths ',
            topIndicatorRanking: getComparisonStatement(areaRank.AreaRank.Rank, areaRank.Max.Rank),
            topIndicatorDate: areaRank.Period.replace(/ /g, ''),
            topIndicatorToolTip: 'Premature death rate per 100,000 is adjusted for various factors, including age of the population.',
            topIndicatorBarChart: getBars(areaRank, overallMax, area, true),
            rankingTableName: 'Deaths',
            rankingTableDate: areaRank.Period.replace(/ /g, ''),
            rankingTableToolTip: 'Standardised rate of premature deaths (deaths before age 75) per 100,000 population',
            rankingTableShowCauses: true
        };
        pageContent(pageContentModel);

        var metadataHash = loaded.indicatorMetadata[model.groupId];
        var benchmarks = areaDetails.Benchmarks;
        var html = [];
        var imageSize;
        for (var i = overallIndex + 1; i <= ROOT_INDEXES.OVERALL_INJURY; i++) {

            var causeInfo = causeOptions[i - overallIndex - 1],
                root = groupRoots[i];

            rank = ranks[i];

            if (rank.AreaRank) {

                causeClass = getCauseClass(significances[i],
                    rank.AreaRank.Val,
                    benchmarks[parentCode][i].Val);
            } else {
                causeClass = 'none';
            }

            if (i == ROOT_INDEXES.OVERALL_LUNG_CANCER ||
                i == ROOT_INDEXES.OVERALL_BREAST_CANCER ||
                i == ROOT_INDEXES.OVERALL_COLORECTAL_CANCER ||
                i == ROOT_INDEXES.OVERALL_HEART_DISEASE ||
                i == ROOT_INDEXES.OVERALL_STROKE) {
                imageSize = '75px;';
            } else {
                // Liver / injuries
                imageSize = '125px;';
            }

            html.push(
                getCauseBars(area, causeInfo, rank, overallMax, causeClass, metadataHash[root.IID], imageSize));
        }

        // Show table element before HTML rendered to prevent IE8 difficulties
        var $aboutOnsGroup = $('.aboutONSGroup');
        if (model.areaTypeId === AreaTypeIds.CountyUA) {
            $aboutOnsGroup.hide();
            $('.aboutDecileGroup').show();
            $('.area-bracket').html('Socioeconomic deprivation bracket');
        } else {
            $('.aboutDecileGroup').hide();
            $('.area-bracket').html('ONS cluster');
            if (isSimilar) {
                $('#ons_name').html(getOnsClusterName(model.parentCode));
                $aboutOnsGroup.show();
            } else {
                $aboutOnsGroup.hide();
            }
        }

        $('#data_page_table, .more_info, .similar_authorities').show();
        $('#cause_bars').html(html);

        toggleDataHeaders(true, areaName);
    } else {
        // Render Header
        var pageHeaderModel = {
            areaName: areaName
        };
        pageHeader(pageHeaderModel);

        toggleDataHeaders(false, areaName);
    }
    unlock();
}

function toggleDataHeaders(dataExists, areaName) {
    if (!dataExists) {
        //There is no data to display
        $('.data_page_content').hide();
        $('.info_box').hide();
        $('.verdict_box').hide();
        $('h1.area_name').html(areaName + ' - No data available');
    } else {
        //There is no data to display
        $('.data_page_content').show();
        $('.info_box').show();
        $('.verdict_box').show();
        $('h1.area_name').html(areaName);
    }
}

function groupColour(imgUrl, grade) {

    // Group colour   
    $('#similar_colour').html(
        '<img src="' + imgUrl + 'Mortality/grade-' +
            grade + '.png" />'
    );
}

function getCauseBars(area, causeInfo, rankInfo, overallMax, causeClass, metadata, imageSize) {

    var areaRank = rankInfo.AreaRank;

    var cause = causeDetails[causeInfo.key];

    return templates.render('causeBars', {
        parentCode: MT.model.parentCode,
        areaCode: area.Code,
        areaRank: areaRank ? areaRank.Rank : 0,
        areaCardinal: areaRank ? getCardinal(areaRank.Rank) : '',
        causeKey: causeInfo.key,
        causes: cause.causes,
        causeName: cause.name,
        reduceUrl: cause.reduceUrl,
        showLinks: cause.showLinks,
        showBlankRow: cause.showBlankRow,
        causeSig: causeClass,
        indicatorName: metadata.Descriptive.Name,
        barsHtml: getBars(rankInfo, overallMax, area),
        rankHtml: areaRank ? getComparisonStatement(areaRank.Rank, rankInfo.Max.Rank) : '',
        imageSize: imageSize
    });
}

function getCauseClass(sig, areaValue, parentValue) {
    var getGrade = getGradeFunction(parentValue);
    var grade = getGrade(sig, areaValue);

    return grade === '' ?
        'none' :
        'grade-' + grade;
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

function getBars(rankInfo, overallMax, area, isOverall) {

    var min = rankInfo.Min,
    max = rankInfo.Max,
    template = 'barItem',
    pixelsPerUnit = 470/*available width in px*/ / overallMax,
    lowestLabel = 'LOWEST: ',
    highestLabel = 'HIGHEST: ';

    // Is data defined for the current area?
    var areaRank = rankInfo.AreaRank;
    if (areaRank) {
        var areaVal = areaRank.Val;
        var isMax = areaRank.Rank === max.Rank;
        var isMin = areaRank.Rank === min.Rank;
    } else {
        isMax = false;
        isMin = false;
        areaVal = 0;
    }

    var html = [];

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
            val: roundNumber(min.Val, 0),
            barWidth: min.Val * pixelsPerUnit,
            areaName: min.Area.Name,
            label: lowestLabel,
            message: isMin ? 'LOWEST DEATH RATE' : null
        }));

    // Area label if required
    if (isMin) {
        var label = lowestLabel;
    } else if (isMax) {
        label = highestLabel;
    } else {
        label = '';
    }

    // Area bar
    html.push(
        templates.render(template, {
            level: '',
            barImage: isOverall ? overallBarGrade : "area-bar",
            val: roundNumber(areaVal, 0),
            barWidth: areaVal * pixelsPerUnit,
            areaName: area.Name,
            label: label,
            message: areaRank ? null : 'NO DATA'
        }));

    // Max bar
    html.push(
        templates.render(template, {
            level: 'high',
            barImage: isOverall ? overallBarGrade : "high-bar",
            val: roundNumber(max.Val, 0),
            barWidth: max.Val * pixelsPerUnit,
            areaName: max.Area.Name,
            label: highestLabel,
            message: isMax ? 'HIGHEST DEATH RATE' : null
        }));

    return html.join('');
}

function showSimilarAreas() {
    if (!FT.ajaxLock) {
        lock();
        var model = MT.model;

        if (model.areaTypeId === AreaTypeIds.CountyUA) {
            model.parentCode = loaded.areaDetails.getData().Decile.Code;
            initPage();
        } else {
            // District & UA
            ajaxMonitor.setCalls(1);
            getOnsClusterCode(model);
            ajaxMonitor.monitor(function() {
                model.parentCode = getOnsCodeForArea(MT.model.areaCode);
                initPage();
            });
        }

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
        scrollToTop();
    }
}

function scrollToTop() {
    // Ensure top of area-details can be seen
    var $window = $(window);
    if ($window.scrollTop() > 600) {
        $window.scrollTop(160);
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

function showLocalAuthorityWebsiteLink(url) {
    var model = MT.model;
    if (model.areaTypeId != AreaTypeIds.DistrictUA) {
        var $laLink = $('#la_link').html();
        if(_.isUndefined($laLink)){
            $('.related_links')
                .append('<li><a id="la_link" class="external_link" target="_blank" href="' +
                    url +
                    '">Visit the local authority website</a></li>');
        }
    }
}

NO_DATA = 'n/a';

templates.add('barItem', '{{#message}}<li class="high_low_message">{{message}}</li>{{/message}}\
{{^message}}<li class="{{level}}"><div class="bar_max"><div class="bar" style="width:{{barWidth}}px;"><img src="' + FT.url.img + 'Mortality/{{barImage}}.png" style="width:{{barWidth}}px; height:27px;" /><span class="value">{{val}}</span></div></div><p>{{label}}{{areaName}}</p></li>{{/message}}'
);

templates.add('causeBars', '<tr id="{{causeKey}}_row" class="{{causeSig}}">\
<td class="col1"><div id="mortality-image" style="width: {{imageSize}};"><img src="' + FT.url.img + 'Mortality/killers.png" alt="" height="149px;"/></div>{{{rankHtml}}}</td>\
<td class="col2"><h3>{{indicatorName}}</h3><ul class="bar_chart">{{{barsHtml}}}</ul>\
</td><td class="col3"><ul>{{#showLinks}}{{#causes}}<li>{{.}}</li>{{/causes}}{{/showLinks}}\
{{#showLinks}}<li><a class="health-intervention" href="/topic/mortality/health-intervention/{{causeKey}}#are/{{areaCode}}/par/{{parentCode}}">How to reduce {{causeName}} rates</a></li>{{/showLinks}}\
{{#showLinks}}<li><a href="{{reduceUrl}}" class="external_link" target="_blank">Reduce your risk of {{causeName}}</a></li>{{/showLinks}}\
</ul></td></tr>{{#showBlankRow}}<tr id="{{causeKey}}_blank_row" class="blank_row"></tr>{{/showBlankRow}}');

templates.add('similarAreas', '{{#areas}}<li><a href="javascript:selectArea(\'{{Code}}\')">{{Name}}</a></li>{{/areas}}');

templates.add('verdict', '<span>{{judgement}}</span> | {{rank}} out of {{total}}');

causeDetails = {
    cancer: {
        causes: ['Smoking', 'Alcohol', 'Poor diet'],
        name: 'cancer',
        reduceUrl: 'http://www.nhs.uk/be-clear-on-cancer/Pages/beclearoncancer.aspx',
        showLinks: true,
        showBlankRow: false,
        largeImage: true
    },
    lung_cancer: {
        causes: ['Smoking', 'Alcohol', 'Poor diet'],
        name: 'lung cancer',
        reduceUrl: 'http://www.nhs.uk/be-clear-on-cancer/Pages/beclearoncancer.aspx',
        showLinks: false,
        showBlankRow: false,
        largeImage: false
    },
    breast_cancer: {
        causes: ['Smoking', 'Alcohol', 'Poor diet'],
        name: 'breast cancer',
        reduceUrl: 'http://www.nhs.uk/be-clear-on-cancer/Pages/beclearoncancer.aspx',
        showLinks: false,
        showBlankRow: false,
        largeImage: false
    },
    colorectal_cancer: {
        causes: ['Smoking', 'Alcohol', 'Poor diet'],
        name: 'colorectal cancer',
        reduceUrl: 'http://www.nhs.uk/be-clear-on-cancer/Pages/beclearoncancer.aspx',
        showLinks: false,
        showBlankRow: true,
        largeImage: false
    },
    heart_disease_and_stroke: {
        causes: ['High blood pressure', 'Smoking', 'Poor diet'],
        name: 'heart disease and stroke',
        reduceUrl: 'http://www.nhs.uk/Change4Life/Pages/change-for-life.aspx',
        showLinks: true,
        showBlankRow: false,
        largeImage: true
    },
    heart_disease: {
        causes: ['High blood pressure', 'Smoking', 'Poor diet'],
        name: 'heart disease',
        reduceUrl: 'http://www.nhs.uk/Change4Life/Pages/change-for-life.aspx',
        showLinks: false,
        showBlankRow: false,
        largeImage: false
    },
    stroke: {
        causes: ['High blood pressure', 'Smoking', 'Poor diet'],
        name: 'stroke',
        reduceUrl: 'http://www.nhs.uk/Change4Life/Pages/change-for-life.aspx',
        showLinks: false,
        showBlankRow: true,
        largeImage: false
    },
    lung_disease: {
        causes: ['Smoking', 'Air pollution'],
        name: 'lung disease',
        reduceUrl: 'http://smokefree.nhs.uk',
        showLinks: true,
        showBlankRow: false,
        largeImage: true
    },
    liver_disease: {
        causes: ['Alcohol', 'Hepatitis', 'Obesity'],
        name: 'liver disease',
        reduceUrl: 'http://www.nhs.uk/Change4Life/Pages/drink-less-alcohol.aspx',
        showLinks: true,
        showBlankRow: false,
        largeImage: true
    },
    injuries: {
        causes: ['Smoking', 'Air pollution'],
        name: 'injuries',
        reduceUrl: 'http://smokefree.nhs.uk',
        showLinks: false,
        showBlankRow: false,
        largeImage: true
    }
};

function pageHeader(model) {
    var pageHeaderTempl =
        '<h1 class="area_name">{{areaName}}</h1>' +
        '<div id="similar_colour" class="verdict_box">' +
        '{{{similarVerdictColor}}}</div>' +
        '<div  class="mortality_ib info_box">' +
        '<h2>Similar local authorities</h2> ' +
        '<p id="similar_verdict">{{{similarVerdict}}}</p>' +
        '</div>' +
        '<div id="national_colour" class="verdict_box">{{{nationalVerdictColor}}}</div>' +
        '<div class="mortality_ib info_box">' +
        '<h2>National</h2>' +
        '<p id="national_verdict">{{{nationalVerdict}}}</p>' +
        '</div>' +
        '<div id="population_ib" class="info_box">' +
        '<h2>Population</h2>' +
        '<p><span class="population_val">{{population}}</span></p>' +
        '</div>' +
        '<div id="premature_ib" class="info_box">' +
        '<h2>Total premature deaths</h2>' +
        '<p>' +
        '<span class="premature_val">{{totalPrematureDeathsCount}}</span>&nbsp;{{period}}' +
        '<span class="tooltip-right right"><i>Total premature deaths (deaths before age 75) in your chosen area</i></span>' +
        '</p>' +
        '</div>' +
        '</div>';
    templates.add('page-header', pageHeaderTempl);
    var html = templates.render('page-header', {
        areaName: model.areaName,
        population: model.population,
        totalPrematureDeathsCount: model.totalPrematureDeathsCount,
        similarVerdict: model.similarVerdict,
        similarVerdictColor: model.similarVerdictColor,
        nationalVerdict: model.nationalVerdict,
        nationalVerdictColor: model.nationalVerdictColor,
        period: model.period.replace(/ /g, '')
    });
    $('#data_page_header').html(html);
}

function pageContent(model) {

    var pageContentTempl = '{{#isSimilarViewMode}}<h2>{{contentHeading}}</h2>{{/isSimilarViewMode}}' +
        '{{^isSimilarViewMode}}<h2>All local authorities</h2>{{/isSimilarViewMode}}' +
        '{{#isSimilarViewMode}}<p class="ranking_note"><b>Similar view:</b> <span>{{areaName}}</span>&apos;s rank within the <span>{{comparisonRank}}</span> local authorities in the same <span class="area-bracket" id="comparisondd"></span>.</p>{{/isSimilarViewMode}}' +
        '{{^isSimilarViewMode}}<p class="ranking_note"><b>National view:</b> <span>{{areaName}}</span>&apos;s rank within the <span>{{comparisonRank}}</span> local authorities in England.</p>{{/isSimilarViewMode}}' +
        '<p class="legend"> Premature mortality outcomes' +
        '<span class="grade"><img src="{{imgUrl}}Mortality/grade-3.png" alt="worst" />worst</span>' +
        '<span class="grade"><img src="{{imgUrl}}Mortality/grade-2.png" alt="below average" />worse than average</span>' +
        '<span class="grade"><img src="{{imgUrl}}Mortality/grade-1.png" alt="above average" />better than average</span>' +
        '<span class="grade"><img src="{{imgUrl}}Mortality/grade-0.png" alt="best" />best</span>' +
        '</p>' +
        '<div id="main_ranking" class="clearfix {{topIndicatorSig}}">' +
            '<h3>{{topIndicatorHeading}} <span>per 100,000 for {{{topIndicatorDate}}}</span><span class="tooltip tooltip-inverse"><i>{{topIndicatorToolTip}}</i></span></h3>' +
            '<div class="ranking">{{{topIndicatorRanking}}}</div>' +
            '<ul class="bar_chart">' +
                '{{{topIndicatorBarChart}}}' +
            '</ul>' +
        '</div>' +
        '<table id="data_page_table" style="display:none;">' +
         '<thead><tr>' +
                    '<th class="col1"><div><span>Rank</span></div></th>' +
                    '<th class="col2"><div><span>{{rankingTableName}}</span> per 100,000 for {{rankingTableDate}}<span class="tooltip"><i>{{rankingTableToolTip}}</i></span></div></th>' +
                    '<th class="col3"><div><span>Common causes</span></div></th>' +
                '</tr></thead>' +
            '<tbody id="cause_bars"></tbody>' +
        '</table>' +
        '<ul class="more_info clearfix no-print" >' +
        '<li><a href="javascript:MT.nav.rankings();">View full rankings</a></li>' +
        '<li class="last"><a href="https://fingertips.phe.org.uk/profile/public-health-outcomes-framework/data#gid/1000044" class="external_link" target="_blank">View more data at phoutcomes.info</a></li>' +
        '</ul>';

    templates.add('page-content', pageContentTempl);

    var html = templates.render('page-content', {
        contentHeading: model.contentHeading,
        isSimilarViewMode: model.isSimilarViewMode,
        areaName: model.areaName,
        comparisonRank: model.comparisonRank,
        imgUrl: FT.url.img,
        topIndicatorSig: model.topIndicatorSig,
        topIndicatorHeading: model.topIndicatorHeading,
        topIndicatorRanking: model.topIndicatorRanking,
        topIndicatorDate: model.topIndicatorDate,
        topIndicatorToolTip: model.topIndicatorToolTip,
        topIndicatorBarChart: model.topIndicatorBarChart,
        rankingTableName: model.rankingTableName,
        rankingTableDate: model.rankingTableDate,
        rankingTableToolTip: model.rankingTableToolTip
    });
    $('#main_content').html(html);
}