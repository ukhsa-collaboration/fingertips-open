function initPage() {

    tooltipManager.init();

    updateModelFromHash();

    var model = MT.model;
    var isSimilar = isSimilarAreas();
    ajaxMonitor.setCalls(isSimilar ? 9 : 6);

    // Get the data by AJAX
    getBoundaries(model.areaTypeId);
    getAllAreas(model);
    getIndicatorMetadata(model.groupId);
    getGroupRoots(model);
    getAreaTypes();
    loaded.areaDetails.fetchDataByAjax({ areaCode: NATIONAL_CODE });

    if (isSimilar){
        getDecileData(model);
        loaded.areaDetails.fetchDataByAjax();
        loaded.areaDetails.fetchDataByAjax({ areaCode: model.parentCode });
    }

    ajaxMonitor.monitor(getSecondaryData);

    initMapOptions();
}

function displayMapLegend() {

    var prefix = '#map-legend-';

    new MutuallyExclusiveDisplay({
        a: $(prefix + 'quintiles'),
        b: $(prefix + 'deaths')
    }).showA(selectedRootIndex === 'dep');
}

function displayPage() {

    var model = MT.model;

    initMap();

    displayMapLegend();
    var comparatorAreaCode = getCurrentComparator().Code;
    var isSimilarView = isSimilarAreas();
    var areaDetails = isSimilarView ?
        loaded.areaDetails.getData() :
        null;

    if (selectedRootIndex === 'dep') {
        colorPolygonsByDeprivation();
    } else {

        // Get area values
        var root = groupRoots[selectedRootIndex];
        var key = getIndicatorKey(root, model, comparatorAreaCode);

        var areaValues;
            areaValues = getAreaCodeToCoreDataHash(
                loaded.areaValues[key]);

        var parentData;
        if (isSimilarView) {
            parentData = areaDetails ?
                areaDetails.Benchmarks[model.parentCode][selectedRootIndex] :
                getParentDetails(model.parentCode, NATIONAL_CODE, selectedRootIndex);
        } else {
            parentData = areaDetails ?
                areaDetails.Benchmarks[model.parentCode][selectedRootIndex] :
                getParentDetails(NATIONAL_CODE, model.parentCode, selectedRootIndex);
        }

        colorPolygons(areaValues, root, parentData.Val);
    }

    if (isSimilarView) {
        setNonSimilarPolygons();
    }

    // Display similar area overlay
    updateCompareBox();

    $('#gmap').show();

    if (mapState.isPopup) {
        showInfoBox(model.areaCode);
    }

    unlock();
}

function getSecondaryData() {

    ajaxMonitor.setCalls(1);

    getAreaValues(groupRoots[ROOT_INDEXES.OVERALL_MORTALITY], MT.model,
        getCurrentComparator().Code);

    ajaxMonitor.monitor(displayPage);

    initHomeElements();
}

function getPolygonColourFunction(root, parentValue) {

    var c = colours,
        noComparison = c.noComparison,
        sameWorse = c.sameWorse,
        sameBetter = c.sameBetter,
        polarityId = root.PolarityId;

    if (polarityId === -1) {
        return function () { return noComparison; };
    }

    var ragColors = [c.worse, c.better];

    if (polarityId === PolarityIds.RAGLowIsGood) {
        ragColors.reverse();
    }

    return function (sig, areaValue) {
        switch (sig) {
            case 1:
                return ragColors[1];
            case 2:
                return areaValue < parentValue ?
                    sameBetter :
                    sameWorse;
            case 3:
                return ragColors[0];
        }
        return noComparison;
    };
}

function showInfoBox() {

    var model = MT.model;
    ajaxMonitor.setCalls(2);
    loaded.areaDetails.fetchDataByAjax();
    getOnsClusterCode(model);

    ajaxMonitor.monitor(displayPopup);
}

function setSelectedDiseaseFilter(selectedDisease) {

    var newOption;

    switch (selectedDisease) {
        case 'Overall-Mortality':
            newOption = ROOT_INDEXES.OVERALL_MORTALITY;
            break;
        case 'Overall-Cancer':
            newOption = ROOT_INDEXES.OVERALL_CANCER;
            break;
        case 'Overall-CardioVascular':
            newOption = ROOT_INDEXES.OVERALL_CARDIOVASCULAR;
            break;
        case 'Overall-Lung':
            newOption = ROOT_INDEXES.OVERALL_LUNG;
            break;
        case 'Overall-Liver':
            newOption = ROOT_INDEXES.OVERALL_LIVER;
            break;
        case 'Overall-Injury':
            newOption = ROOT_INDEXES.OVERALL_INJURY;
            break;
        case 'Overall-Lung-Cancer':
            newOption = ROOT_INDEXES.OVERALL_LUNG_CANCER;
            break;
        case 'Overall-Colorectal-Cancer':
            newOption = ROOT_INDEXES.OVERALL_COLORECTAL_CANCER;
            break;
        case 'Overall-Breast-Cancer':
            newOption = ROOT_INDEXES.OVERALL_BREAST_CANCER;
            break;
        case 'Overall-Heart-Disease':
            newOption = ROOT_INDEXES.OVERALL_HEART_DISEASE;
            break;
        case 'Overall-Stroke':
            newOption = ROOT_INDEXES.OVERALL_STROKE;
            break;
        case 'Deprivation':
            newOption = 'dep';
            break;
    }

    selectedRootIndex = newOption;
}


function getPopUpHtml() {
  
    var model = MT.model;
    var areaDetails = loaded.areaDetails.getData();
    var ranks = areaDetails.Ranks[model.parentCode];
    var index = ROOT_INDEXES.POPULATION;

    var isDeprivation = selectedRootIndex === 'dep';

    var showData = isDeprivation ?
        true :
        isDefined(ranks[selectedRootIndex].AreaRank);

    if (isDefined(ranks[index].AreaRank) && showData) {

        var areaPopulation = ranks[index].AreaRank.Val;

        var content = isDeprivation ?
            getDeprivationPopUpHtml(areaDetails, areaPopulation) :
            getCausePopUpHtml(ranks, areaDetails, areaPopulation);

    } else {
        content = templates.render('nodatafound', {
            nameofplace: areaDetails.Area.Name
        });
    }

    return content;
}

function getCausePopUpHtml(ranks, areaDetails, areaPopulation) {

    var index = selectedRootIndex;

    switch (index) {
        case ROOT_INDEXES.OVERALL_MORTALITY:
            var header = 'Overall deaths';
            var imageClass = 'stat_overall';
            break;
        case ROOT_INDEXES.OVERALL_CANCER:
            header = 'Cancer deaths';
            imageClass = 'stat_cancer';
            break;
        case ROOT_INDEXES.OVERALL_CARDIOVASCULAR:
            header = 'Heart Disease deaths';
            imageClass = 'stat_heart';
            break;
        case ROOT_INDEXES.OVERALL_LUNG:
            header = 'Lung deaths';
            imageClass = 'stat_lung';
            break;
        case ROOT_INDEXES.OVERALL_LIVER:
            header = 'Liver deaths';
            imageClass = 'stat_liver';
            break;
        case ROOT_INDEXES.OVERALL_INJURY:
            header = 'Injury deaths';
            imageClass = 'stat_injury';
            break;
        case ROOT_INDEXES.OVERALL_LUNG_CANCER:
            header = 'Lung Cancer deaths';
            imageClass = 'stat_cancer';
            break;
        case ROOT_INDEXES.OVERALL_COLORECTAL_CANCER:
            header = 'Colorectal Cancer deaths';
            imageClass = 'stat_cancer';
            break;
        case ROOT_INDEXES.OVERALL_BREAST_CANCER:
            header = 'Breast Cancer deaths';
            imageClass = 'stat_cancer';
            break;
        case ROOT_INDEXES.OVERALL_HEART_DISEASE:
            header = 'Heart Disease deaths';
            imageClass = 'stat_heart';
            break;
        case ROOT_INDEXES.OVERALL_STROKE:
            header = 'Stroke deaths';
            imageClass = 'stat_heart';
            break;
    }

    // Grade class
    var areaRank = ranks[index].AreaRank;
    var parentCode = MT.model.parentCode;
    var parentValue = areaDetails.Benchmarks[parentCode][index].Val;
    var getGrade = getGradeFunction(parentValue);
    var sig = areaDetails.Significances[parentCode][index];
    var gradeClass = 'grade-' + getGrade(sig, areaRank.Val);

    var rank = areaRank.Rank;

    var topLink = getTopLink('Compare with ');

    return templates.render('areaoverlay', {
        nameofplace: areaDetails.Area.Name,
        population: new CommaNumber(areaPopulation).rounded(),
        prematuredeaths: new CommaNumber(areaRank.Count).rounded(),
        ranked: rank + getCardinal(rank),
        rankoutof: ranks[index].Max.Rank,
        areaCode: MT.model.areaCode,
        imageclass: imageClass,
        filterheader: header,
        rankClass: gradeClass,
        topLinkText: topLink.text,
        topLinkFunction: topLink.func,
        period: ranks[index].Period.replace(/ /g, '')
    });
}

function initMapOptions() {

    $('.filters li').click(function () {

        if (!FT.ajaxLock) {

            lock();
            setSelectedDiseaseFilter($(this).attr('id'));
            redrawMap();

            // Display new active options
            var cssClass = 'active';
            $('.filters.causes li').removeClass(cssClass);
            $(this).addClass(cssClass);

            updateCompareBox();

            logEvent(AnalyticsCategories.MAP,
                'IndicatorSelected', $(this).children('a').text());
        }
    });
}

function switchAreas(areaTypeId) {

    var model = MT.model;
    setUrl('#ati/' + areaTypeId + '/gid/' + model.groupId + '/pat/' + areaTypeId);
    window.location.reload();
}

popUpFooter = '</div><div class="map-info-footer clearfix"><a href="javascript:MT.nav.areaDetails();">View local authority details</a></div>\
<div class="map-info-tail" onclick="pointerClicked()"><i></i></div>\</div>';

popUpHeader = '<div class="map-template map-info"><div class="map-info-header clearfix"><span class="map-info-close" onclick="closeInfo()">&times;</span>\
<a href="javascript:{{topLinkFunction}}()">{{topLinkText}} areas on map ></a></div><div class="map-info-body map-info-stats clearfix">';

population = '<dl class="stat population_stat"><dt>Population</dt><dd><strong>{{population}}</strong></dd></dl>';
totalMortality = '<dl class="stat premature_death_stat"><dt>Total premature deaths</dt><dd><strong>{{prematuredeaths}}</strong> for {{period}}</dd></dl>';

templates.add('areaHover',
    '<div class="{{hoverTemplateClass}} map-info"><div class="map-info-header clearfix">{{#showSimilarLink}}<a href="javascript:viewSimilar()">{{similarText}} similar areas on map ></a>{{/showSimilarLink}}</div><div class="hover-map-info-body map-info-stats clearfix">'
        + '<h4 class="hover-place-name">{{nameofplace}}</h4>' + '</div><div class="map-info-footer clearfix"></div><div class="{{hoverTemplateTailClass}}" onclick="pointerClicked()"><i></i></div></div>');

templates.add('areaoverlay',
    popUpHeader + '<div class="map-info-right"><dl class="stat ranked_stat {{rankClass}} {{imageclass}}"><dt id="filterHeader">{{filterheader}}</dt><dd><i></i>Ranked <strong>{{ranked}}</strong>out of <span class="rank-max">{{rankoutof}}</span></dd></dl></div>' +
       '<h4 class="place-name">{{nameofplace}}</h4><ul class="totals"><li class="left">' + population + '</li><li>' + totalMortality + '</li></ul>' + popUpFooter);

templates.add('deprivationOverlay', popUpHeader + '<h4 class="place-name">{{nameofplace}}</h4>' +
        '<ul class="totals"><li class="left">' + population + '</li><li>' +
        '<dl class="stat deprivation_stat level{{quintile}}"><dt>Socioeconomic deprivation</dt><dd><strong>{{text}}</strong></dd></dl></li></ul>' + popUpFooter);

