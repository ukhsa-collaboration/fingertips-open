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

        displayIndicatorSelected();
    }
}

function displayIndicatorSelected() {
    var $selectedCause = $('#iid-' + MT.model.indicatorId);
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

    if (useBlueOrangeBlue(root.PolarityId)) {
        $(prefix + 'bob').show();
        $(prefix + 'quintiles').hide();
        $(prefix + 'deaths').hide();
    } else {
        $(prefix + 'bob').hide();
        new MutuallyExclusiveDisplay({
            a: $(prefix + 'quintiles'),
            b: $(prefix + 'deaths')
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

    var ragColors = [];
    if (polarityId == PolarityIds.BlueOrangeBlue) {
        ragColors = [c.bobLower, c.bobSimilar, c.bobHigher];
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
        
        var areaPopulation = !!rank
            ? rank.Count
            : 'Data unavailable';
        
        // For NHS Health Checks we use Denom of the indicator #91111 as population
        if (model.profileId === ProfileIds.HealthChecks) {
            
            var denom = loaded.groupDataAtDataPoint.getData(healthChecksAlternativeModel(model))[0].Denom;
            
            areaPopulation = !!denom ? denom : 'Data unavailable';
        }

        var content = CallOutBox.getCausePopUpHtml(ranks, areaDetails, areaPopulation);
    } else {
        content = templates.render('nodatafound', {
            nameofplace: areaDetails.Area.Name
        });
    }

    return content;
}

CallOutBox.getCausePopUpHtml = function (ranks, areaDetails, areaPopulation) {
    var index = selectedRootIndex;
    var imageClass = 'stat_overall';
    var root = groupRoots[index];
    var indicatorId = root.IID;

    // Grade class
    var areaRank = ranks[index].AreaRank;
    var parentCode = MT.model.parentCode;
    var getGrade = getGradeFunctionFromGroupRoot(root);
    var sig = areaDetails.Significances[parentCode][index];
    var gradeClass = getGrade(sig, root);
    var rank = areaRank.Rank;
    var topLink = getTopLink('Compare with ');

    var footerLink = hasPracticeData
        ? '<span style="float: left; padding-bottom: 10px;"><a href=javascript:MT.nav.rankings();><strong style="font-size: 1em;">See local GP practice comparison table</strong></a></span>'
        : '';

    var metadataHash = loaded.indicatorMetadata[MT.model.groupId];
    var metadata = metadataHash[indicatorId];
    var textMetadata = metadata.Descriptive;

    var comparatorAreaCode = getCurrentComparator().Code;
    var key = getIndicatorKey(root, MT.model, comparatorAreaCode);
    var areaValues = getAreaCodeToCoreDataHash(loaded.areaValues[key]);
    var valF = areaValues[MT.model.areaCode].ValF;
    if (metadata.ValueType.Id === 2/*indirectly standardised ratio*/) {
        // e.g. Diabetes complications
        var indicatorName = textMetadata.Name;
        var indicatorDescription = '';
        var rankingValF = indicatorName + ' is <span class="complication_premature_death_stat"><strong>' + valF +
            '</strong></span> times as likely in people with diabetes in this area than the general population';
        var populationText = 'Adults with this diabetic complication in this area';
        var isDomainNormal = false;
    } else {

        indicatorName = replacePercentageWithArialFont(textMetadata.Definition);
        indicatorName = trimName(indicatorName, 350);

        indicatorDescription = trimName(isDefined(textMetadata.IndicatorContent) ? replacePercentageWithArialFont(textMetadata.IndicatorContent) : replacePercentageWithArialFont(textMetadata.NameLong), 140);

        // For D&A, we use definition of numerator
        if (MT.model.groupId === GroupIds.DrugsAndAlcohol.PrevalenceAndRisks ||
            MT.model.groupId === GroupIds.DrugsAndAlcohol.TreatmentAndRecovery) {
            populationText = metadata.Descriptive.CountDefinition;
        } else {
            populationText = loaded.contentText;
        }


        if (indicatorId === IndicatorIds.Deprivation) {
            rankingValF = deprivationDefinitions[sig];
        } else {
            rankingValF = valF;
        }

        isDomainNormal = true;
    }

    var data = areaValues[MT.model.areaCode];

    var valueNoteText = new CoreDataSetInfo(data).isNote()
        ? getValueNoteText(data.NoteId)
        : '';


    var unitFormat = new UnitFormat(metadata, data.Val);

    var templateModel = {
        nameofplace: areaDetails.Area.Name,
        indirectlyStandardisedRate: new CommaNumber(data.Count).rounded(),
        ranking: rankingValF,
        period: ranks[index].Period,
        unit: unitFormat.getLongLabel(),
        unitClass: unitFormat.getClass(),
        ranked: rank + getCardinal(rank),
        rankoutof: ranks[index].Max.Rank,
        areatypename: getAreaTypeNamePlural(MT.model.areaTypeId),
        areaCode: MT.model.areaCode,
        imageclass: imageClass,
        filterheader: metadata.ValueType.Id === 2 ? textMetadata.Definition : indicatorName,
        rankClass: gradeClass,
        valueNote: valueNoteText,
        topLinkText: topLink.text,
        topLinkFunction: topLink.func,
        footerLinkText: footerLink,
        indicatordescription: indicatorDescription,
        isdomainnormal: isDomainNormal,
        populationText: populationText,
        dataSource: textMetadata.DataSource,
        isDefinitionOpen: mapState.isDefinitionOpen,
        area: getAreaTypeNameSingular(MT.model.areaTypeId),
        hasPracticeData: hasPracticeData
    };

    var extendedModel = CallOutBox.getExtendedModel(areaPopulation, getGrade, areaValues);
    jQuery.extend(templateModel, extendedModel);

    return templates.render('areaoverlay', templateModel);
}

CallOutBox.getExtendedModel = function () {
    alert('You need to include a callout js file');
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
    '{{#causes}}<li id=iid-{{id}} class="{{cssClass}}"><a href="javascript:selectIndicator({{index}}, {{id}})">{{{name}}}</a></li>{{/causes}}');

