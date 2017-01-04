function initPage() {
    tooltipManager.init();
    updateModelFromHash();
    setSearchText(MT.model.areaTypeId);
}

function updatePage() {

    var model = MT.model;
    var isSimilar = isSimilarAreas();

    ajaxMonitor.setCalls(isSimilar ? 10 : 7);

    // Get the data by AJAX
    getBoundaries(model.areaTypeId);
    getAllAreas(model);
    loaded.areaDetails.fetchDataByAjax({ areaCode: NATIONAL_CODE });
    getIndicatorMetadata(model.groupId, GET_METADATA_DEFINITION, GET_METADATA_SYSTEM_CONTENT);
    getGroupRoots(model);
    getAreaTypes();
    getContentText('population-text');

    if (isSimilar) {
        getDecileData(model);
        loaded.areaDetails.fetchDataByAjax();
        loaded.areaDetails.fetchDataByAjax({ areaCode: model.parentCode });
    }

    ajaxMonitor.monitor(getSecondaryData);
}

function getSecondaryData() {

    ajaxMonitor.setCalls(1);

    getAreaValues(groupRoots[ROOT_INDEXES.POPULATION], MT.model, getCurrentComparator().Code);

    ajaxMonitor.monitor(displayPage);

    initHomeElements();
    populateCauseList();
}

function selectIndicator(rootIndex, indicatorId) {
    
    if (!FT.ajaxLock) {
        lock();

        var model = MT.model;

        selectedRootIndex = rootIndex;   
        model.indicatorId = indicatorId;
        model.sexId = groupRoots[selectedRootIndex].Sex.Id;

        // Only render calloutbox if we have ranking available for clicked area
        if (isAnyIndicatorData()) {
            redrawMap();
        } else {
            colorAllPolygonsGrey();

            var name = loaded.indicatorMetadata[model.groupId][indicatorId].Descriptive.Name;
            alert('Data not available for "' + name + '"');

            CallOutBox.hide();
            unlock();
        }

        displayIndicatorSelected(rootIndex);
    }
}

function displayIndicatorSelected(index) {
    var $selectedCause = $('#'+ index + '-iid-' + MT.model.indicatorId);
    var cssClass = 'active';
    $('.causes li').removeClass(cssClass);
    $selectedCause.addClass(cssClass);
}

function isAnyIndicatorData() {
   
    var parentCode = MT.model.parentCode;

    var modelExtensions = (!MT.model.areaCode)
        ? { areaCode: parentCode } // No area selected 
        : {};

    var data = loaded.areaDetails.getData(modelExtensions);

    var ranks = data.Ranks[parentCode];
    return !!ranks[selectedRootIndex];

   
   
}

function selectDomain(groupId) {

    var model = MT.model;

    if (groupId !== model.groupId) {

        lock();

        selectedRootIndex = ROOT_INDEXES.POPULATION;

        model.groupId = groupId;

        ajaxMonitor.setCalls(!model.areaCode ? 3 : 4);

        getGroupRoots(model);
        getIndicatorMetadata(groupId, GET_METADATA_DEFINITION, GET_METADATA_SYSTEM_CONTENT);
        loaded.areaDetails.fetchDataByAjax({areaCode: model.parentCode});

        if (model.areaCode) {
            loaded.areaDetails.fetchDataByAjax({ areaCode: model.areaCode });
        }

        ajaxMonitor.monitor(repopulateCausesAndMap);
    }
}

function displayDomainSelected() {
    var selectedDomain = $('#domain-' + MT.model.groupId);
    $('.indicator-group').removeClass('active');
    selectedDomain.addClass('active');
}

function repopulateCausesAndMap() {
    populateCauseList();
    selectIndicator(0, groupRoots[ROOT_INDEXES.POPULATION].IID);
}

function displayMapLegend(root) {

    var prefix = '#map-legend-';

    var $bobLegend = $(prefix + 'bob');
    var $quintilesLegend = $(prefix + 'quintiles');
    var $ragLegend = $(prefix + 'rag');

    // Hide all
    $bobLegend.hide();
    $quintilesLegend.hide();
    $ragLegend.hide();

    // Show specific legend
    if (root.IID === IndicatorIds.SuicidePlan) {
        // No legend
        return;
    } else if (useBlueOrangeBlue(root.PolarityId)) {
        $bobLegend.show();
    } else {
        new MutuallyExclusiveDisplay({
            a: $quintilesLegend,
            b: $ragLegend
        }).showA(useQuintiles(root.ComparatorMethodId));
    }
}

function displayPage() {
   
    var model = MT.model;

    initMap();

    // Only render calloutbox if we have ranking available for clicked area
    if (isAnyIndicatorData()) {

        var comparatorAreaCode = getCurrentComparator().Code;

        // Get area values
        var root = groupRoots[selectedRootIndex];
        MT.model.indicatorId = root.IID;
        MT.model.sexId = root.Sex.Id;
        var key = getIndicatorKey(root, model, comparatorAreaCode);
        var areaValues = getAreaCodeToCoreDataHash(loaded.areaValues[key]);

        colorPolygons(areaValues, root);
        displayMapLegend(root);

        // Display similar area overlay
        updateCompareBox();

        $('#gmap').show();

        if (mapState.isPopup) {
            showInfoBox();
        }

    } else {
        colorAllPolygonsGrey();
        CallOutBox.hide();
    }

    displayDomainSelected();

    unlock();
}

function getPolygonColourFunction(root) {

    var c = colours,
        noComparison = c.noComparison,
        polarityId = root.PolarityId,
        getColourFromSignificance = function (colourList, sig) {
            return sig
               ? colourList[sig - 1]
               : noComparison;
        };

    if (useQuintiles(root.ComparatorMethodId)) {
        return function (sig) {
            return getColourFromSignificance(quintileColors, sig);
        };
    }

    if (polarityId === -1) {
        return function () { return noComparison; };
    }

    var ragColors;
    if (polarityId === PolarityIds.BlueOrangeBlue) {
        ragColors = [c.bobLower, c.bobSimilar, c.bobHigher];
    } else if (polarityId === PolarityIds.RAGLowIsGood) {
        ragColors = [c.better, c.sameWorse, c.worse];
    } else {
        ragColors = [c.worse, c.sameWorse, c.better];
    }

    return function (sig) {
        return getColourFromSignificance(ragColors, sig);
    };
}

function showInfoBox() {

    var model = MT.model;
    
    if (model.profileId === ProfileIds.HealthChecks) {
        ajaxMonitor.setCalls(4);

        loaded.groupDataAtDataPoint.fetchDataByAjax(healthChecksAlternativeModel(model));
    } else {
        ajaxMonitor.setCalls(3);
    }

    loaded.areaDetails.fetchDataByAjax();
    getSupportingAreaDetails(model.areaCode, model.areaTypeId);
    getValueNotes();

    ajaxMonitor.monitor(displayPopup);
}

CallOutBox.toggleDefinition = function () {
    mapState.isDefinitionOpen = !mapState.isDefinitionOpen;
    $('#def').toggle();
    $('#defBtn').toggleClass('plus minus');
    CallOutBox.show();
}

CallOutBox.getPopUpHtml = function () {
    var model = MT.model;
    var areaDetails = loaded.areaDetails.getData();
    var ranks = areaDetails.Ranks[model.parentCode];
    var isDeprivation = selectedRootIndex === 'dep';

    var showData = isDeprivation ?
        true :
        isDefined(ranks[selectedRootIndex].AreaRank);

    if (showData) {
        var supportingAreaData = loaded.supportingAreaData.getData(
            {
                profileId: SupportingProfileId,
                groupId: SupportingGroupId
            });
        
        var rank = supportingAreaData.Ranks[NATIONAL_CODE][0].AreaRank;
        
        var content = CallOutBox.getCausePopUpHtml(ranks, areaDetails, rank);
    } else {
        content = templates.render('nodatafound', {
            nameofplace: areaDetails.Area.Name
        });
    }

    return content;
}

CallOutBox.getCausePopUpHtml = function (ranks, areaDetails, supportingDataRank) {

    var model = MT.model;
    var areaPopulation;
    var profileId = model.profileId;

    // Top indicator
    if (profileId === ProfileIds.HealthChecks) {
        // For NHS Health Checks we use Denom of the indicator #91111 as population
        var denom = loaded.groupDataAtDataPoint.getData(healthChecksAlternativeModel(model))[0].Denom;
        areaPopulation = !!denom ? denom : 'Data unavailable';
    } else if (profileId === ProfileIds.Suicide) {
        areaPopulation = supportingDataRank;
    } else {
        areaPopulation = !!supportingDataRank
            ? supportingDataRank.Count
            : 'Data unavailable';
    }

    var groupId = model.groupId;
    var index = selectedRootIndex;
    var imageClass = 'stat_overall';
    var root = groupRoots[index];
    var indicatorId = root.IID;

    // Grade class
    var parentCode = model.parentCode;
    var getGrade = getGradeFunctionFromGroupRoot(root);
    var sig = areaDetails.Significances[parentCode][index];
    var gradeClass = getGrade(sig, root);
    var topLink = getTopLink('Compare with ');

    var footerLink = hasPracticeData
        ? '<span style="float: left; padding-bottom: 10px;"><a href=javascript:MT.nav.rankings();><strong style="font-size: 1em;">See local GP practice comparison table</strong></a></span>'
        : '';

    var metadataHash = loaded.indicatorMetadata[groupId];
    var metadata = metadataHash[indicatorId];
    var textMetadata = metadata.Descriptive;

    var comparatorAreaCode = getCurrentComparator().Code;
    var key = getIndicatorKey(root, model, comparatorAreaCode);
    var areaValues = getAreaCodeToCoreDataHash(loaded.areaValues[key]);
    var valF = areaValues[model.areaCode].ValF;

    if (metadata.ValueType.Id === 2/*indirectly standardised ratio*/) {
        // e.g. Diabetes complications
        var indicatorName = textMetadata.Name;
        var indicatorDescription = '';
        var rankingValF = indicatorName + ' is <span class="complication_premature_death_stat"><strong>' + valF +
            '</strong></span> times as likely in people with diabetes in this area than the general population';
        var topIndicatorText = 'Adults with this diabetic complication in this area';
        var isDomainNormal = false;
    } else {

        // Indicator name
        indicatorName = replacePercentageWithArialFont(textMetadata.Definition);
        indicatorName = trimName(indicatorName, 350);

        // Indicator desecription
        indicatorDescription = trimName(isDefined(textMetadata.IndicatorContent)
            ? replacePercentageWithArialFont(textMetadata.IndicatorContent)
            : replacePercentageWithArialFont(textMetadata.NameLong), 140);

        // Top indicator text
        if (groupId === GroupIds.DrugsAndAlcohol.PrevalenceAndRisks ||
            groupId === GroupIds.DrugsAndAlcohol.TreatmentAndRecovery) {
            // For D&A, we use definition of numerator
            topIndicatorText = metadata.Descriptive.CountDefinition;
        } else {
            topIndicatorText = loaded.contentText;
        }

        if (indicatorId === IndicatorIds.Deprivation) {
            rankingValF = deprivationDefinitions[sig];
        } else {
            rankingValF = valF;
        }

        isDomainNormal = true;
    }

    var data = areaValues[model.areaCode];

    var valueNoteText = new CoreDataSetInfo(data).isNote()
        ? getValueNoteText(data.NoteId)
        : '';

    var unitFormat = new UnitFormat(metadata, data.Val);

    var hideSupportingData = indicatorId === IndicatorIds.SuicidePlan;

    var showMan = profileId !== ProfileIds.Suicide;

    var viewModel = {
        showMan : showMan,
        hideSupportingData : hideSupportingData,
        rankingHtml: CallOutBox.getRankingHtml(ranks[index], indicatorId),
        nameofplace: areaDetails.Area.Name,
        indirectlyStandardisedRate: new CommaNumber(data.Count).rounded(),
        ranking: rankingValF,
        period: ranks[index].Period,
        unit: unitFormat.getLongLabel(),
        unitClass: unitFormat.getClass(),
        areaCode: model.areaCode,
        imageclass: imageClass,
        filterheader: metadata.ValueType.Id === 2 ? textMetadata.Definition : indicatorName,
        rankClass: gradeClass,
        valueNote: valueNoteText,
        topLinkText: topLink.text,
        topLinkFunction: topLink.func,
        footerLinkText: footerLink,
        indicatordescription: indicatorDescription,
        isdomainnormal: isDomainNormal,
        topIndicatorText: topIndicatorText,
        dataSource: textMetadata.DataSource,
        isDefinitionOpen: mapState.isDefinitionOpen,
        area: getAreaTypeNameSingular(model.areaTypeId),
        hasPracticeData: hasPracticeData
    };

    var extendedViewModel = CallOutBox.getExtendedModel(areaPopulation, getGrade, areaValues);
    jQuery.extend(viewModel, extendedViewModel);

    return templates.render('areaoverlay', viewModel);
}

CallOutBox.getExtendedModel = function () {
    alert('You need to include a callout js file');
}

CallOutBox.getRankingHtml = function (rankInfo, indicatorId) {

    // Values should not be ranked for all indicators
    if (indicatorId === IndicatorIds.SuicidePlan) {
        return '';
    }

    var areaRank = rankInfo.AreaRank.Rank;

    var viewModel = {
        ranked: areaRank + getCardinal(areaRank),
        areaCount: rankInfo.Max.Rank,
        areatypename: getAreaTypeNamePlural(MT.model.areaTypeId)
    };

    templates.add('rankings',
        'Ranked <strong style="font-size: 1em; padding-left: 0px;">{{ranked}}</strong> out of <span class="rank-max">{{areaCount}} {{{areatypename}}}.');
    return templates.render('rankings', viewModel);
}

function healthChecksAlternativeModel(model) {
    var modelForHealthCheck = {};
    modelForHealthCheck.areaCode = model.areaCode;
    modelForHealthCheck.areaTypeId = model.areaTypeId;
    modelForHealthCheck.profileId = ProfileIds.HealthChecks;
    modelForHealthCheck.groupId = GroupIds.HealthChecks.HealthCheck;
    return modelForHealthCheck;
}


templates.add('areaHover',
    '<div class="{{hoverTemplateClass}} map-info"><div class="map-info-header clearfix">{{#showSimilarLink}}<a href="javascript:viewSimilar()">{{similarText}} similar areas on map ></a>{{/showSimilarLink}}</div><div class="hover-map-info-body map-info-stats clearfix">'
    + '<h4 class="hover-place-name">{{nameofplace}}</h4>' + '</div><div class="map-info-footer clearfix"></div><div class="{{hoverTemplateTailClass}}" onclick="pointerClicked()"><i></i></div></div>');

templates.add('causes',
    '{{#causes}}<li id={{index}}-iid-{{id}} class="{{cssClass}}"><a href="javascript:selectIndicator({{index}}, {{id}})">{{{name}}}</a></li>{{/causes}}');

