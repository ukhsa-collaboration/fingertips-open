function initPage() {
    tooltipManager.init();
    updateModelFromHash();
    setSearchText(MT.model.areaTypeId);
}

function updatePage() {

    var model = MT.model;

    if (isMapWithNoData()) {
        // Only need to display boundaries on map
        ajaxMonitor.setCalls(2);

        // Get the data by AJAX
        getBoundaries(model.areaTypeId);
        getAllAreas(model);

        ajaxMonitor.monitor(displayPage);

    } else {
        // Map with data
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
            if (isNearestNeighbour()) {
                getNearestNeighbours();
            } else {
                getDecileData(model);
            }
            loaded.areaDetails.fetchDataByAjax();
            loaded.areaDetails.fetchDataByAjax({ areaCode: model.parentCode });
        }

        ajaxMonitor.monitor(getSecondaryData);
    }
}

function getSecondaryData() {

    checkGroupRootsDefined();
    setSelectedRootIndexFromModel();

    ajaxMonitor.setCalls(1);

    getAreaValues(groupRoots[selectedRootIndex], MT.model, getMapBenchmarkCode());

    ajaxMonitor.monitor(displayPage);

    initHomeElements();
    populateCauseList();
}

function displayPage() {

    var model = MT.model;

    initMap();

    if (isMapWithNoData()) {
        // Map only for user to choose an area
        $('#map').css('border-style', 'none');
        colorAllPolygonsBlue();
    } else {
        if (isAnyIndicatorData()) {

            var root = groupRoots[selectedRootIndex];

            // Set model
            model.indicatorId = root.IID;
            model.sexId = root.Sex.Id;

            // Get area values
            var parentCode = getMapBenchmarkCode();
            var key = getIndicatorKey(root, model, parentCode);
            var areaValues = getAreaCodeToCoreDataHash(loaded.areaValues[key]);
            colorPolygons(areaValues, root);
            displayMapLegend(root);

            // Display similar area overlay
            updateCompareBox();

            $('#gmap').show();

            // Only render calloutbox if we have ranking available for clicked area
            if (mapState.isPopup) {
                showInfoBox();
            }

        } else {
            colorAllPolygonsGrey();
            CallOutBox.hide();
        }

        displayDomainSelected();
    }

    unlock();
}

/**
 * Set selectedRootIndex from model if indicator has been bookmarked
 */
function setSelectedRootIndexFromModel() {
    var model = MT.model;
    if (model.indicatorId) {
        for (var i in groupRoots) {
            if (groupRoots[i].IID === model.indicatorId && groupRoots[i].Sex.Id === model.sexId) {
                selectedRootIndex = i;
                return;
            }
        }
    }
    selectedRootIndex = 0;
}

function selectIndicator(rootIndex, indicatorId) {

    if (!FT.ajaxLock) {
        lock();

        var model = MT.model;

        selectedRootIndex = rootIndex;
        model.indicatorId = indicatorId;
        model.sexId = groupRoots[selectedRootIndex].Sex.Id;

        ftHistory.setHistory();

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

function checkGroupRootsDefined() {
    if (groupRoots.length === 0) {
        throw new Error(
            'No group roots defined. Every domain for an area type must contain indicators.');
    }
}

function displayIndicatorSelected(index) {
    var $selectedCause = $('#' + index + '-iid-' + MT.model.indicatorId);
    var cssClass = 'active';
    $('.causes li').removeClass(cssClass);
    $selectedCause.addClass(cssClass);
}

function isAnyIndicatorData() {
    var parentCode = getMapBenchmarkCode();

    // Get data (internal working of getData need areaCode defined on model)
    var modelExtensions = (!MT.model.areaCode)
        ? { areaCode: parentCode } // No area selected 
        : {};
    var data = loaded.areaDetails.getData(modelExtensions);
    if (!isDefined(data)) {
        data = loaded.areaDetails.getData({ areaCode: parentCode });
    }

    var ranks = data.Ranks[parentCode];
    return !!ranks[selectedRootIndex];
}

function selectDomain(groupId) {

    var model = MT.model;

    if (groupId !== model.groupId) {

        lock();

        // Select first indicator by default
        selectedRootIndex = 0;

        model.groupId = groupId;

        ajaxMonitor.setCalls(!model.areaCode ? 3 : 4);

        getGroupRoots(model);
        getIndicatorMetadata(groupId, GET_METADATA_DEFINITION, GET_METADATA_SYSTEM_CONTENT);
        loaded.areaDetails.fetchDataByAjax({ areaCode: model.parentCode });

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

    // Hide all
    $('.map-legend').hide();

    // Show specific legend
    var toShow;
    var comparatorMethodId = root.ComparatorMethodId;
    if (root.IID === IndicatorIds.SuicidePlan) {
        // No legend
        return;
    } else if (useBlueOrangeBlue(root.PolarityId)) {
        toShow = 'bob';
    } else if (useQuintiles(comparatorMethodId)) {
        toShow = 'quintiles';
    } else if (comparatorMethodId === ComparatorMethodIds.Quartiles) {
        toShow = 'quartiles';
    } else {
        toShow = 'rag';
    }

    $('#map-legend-' + toShow).show();
}

function getPolygonColourFunction(root) {

    var c = colours;
    var noComparison = c.noComparison;
    var polarityId = root.PolarityId;
    var getColourFromSignificance = function (colourList, sig) {
        return sig
           ? colourList[sig - 1]
           : noComparison;
    };

    // Quintiles
    if (useQuintiles(root.ComparatorMethodId)) {
        return function (sig) {
            return getColourFromSignificance(quintileColors, sig);
        };
    }

    // Quartiles
    if (root.ComparatorMethodId === ComparatorMethodIds.Quartiles) {

        var quartileColors = [c.better, c.sameBetter, c.sameWorse, c.worse];

        return function (sig) {
            return getColourFromSignificance(quartileColors, sig);
        };
    }

    // No comparison
    if (polarityId === -1) {
        return function () { return noComparison; };
    }

    var ragColors;
    if (polarityId === PolarityIds.BlueOrangeBlue) {
        ragColors = [c.bobLower, c.bobSimilar, c.bobHigher];
    } else {
        // Do no need to consider RAG polarity because it is handled in the web services
        ragColors = [c.worse, c.sameWorse, c.better];
    }

    return function (sig) {
        return getColourFromSignificance(ragColors, sig);
    };
}

function selectAreaType(areaTypeId) {
    var model = MT.model;
    setUrl('/topic/' + profileUrlKey +
        '#par/' + NATIONAL_CODE +
        '/ati/' + areaTypeId +
        '/gid/' + model.groupId +
        '/pat/' + areaTypeId);
    // we only reload the page if we have a topic
    // otherwise we just set the url with clicked areatype
    var currentUrl = window.location.href;
    if (currentUrl.indexOf("topic") > -1) {
        window.location.reload();
    }
}

function getMapBenchmarkCode() {
    return isNearestNeighbour() ? NATIONAL_CODE : MT.model.parentCode;
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
    var areaDetails = loaded.areaDetails.getData();
    var ranks = areaDetails.Ranks[NATIONAL_CODE];
    var showData = ranks[selectedRootIndex].AreaRank && ranks[selectedRootIndex].AreaRank.Rank;

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
    var groupRootIndex = selectedRootIndex;
    var imageClass = 'stat_overall';
    var root = groupRoots[groupRootIndex];
    var indicatorId = root.IID;

    // Grade class
    var parentCode = getMapBenchmarkCode();
    var getGrade = getGradeFunctionFromGroupRoot(root);
    var sig = areaDetails.Significances[parentCode][groupRootIndex];
    var gradeClass = getGrade(sig, root);

    // Get metadata
    var metadataHash = loaded.indicatorMetadata[groupId];
    var metadata = metadataHash[indicatorId];
    var textMetadata = metadata.Descriptive;

    // Get area values
    var key = getIndicatorKey(root, model, parentCode);
    var areaValues = getAreaCodeToCoreDataHash(loaded.areaValues[key]);

    // Set area value
    var valF = areaValues[model.areaCode].ValF;

    if (metadata.ValueType.Id === ValueTypeIds.IndirectlyStandardisedRatio &&
        MT.model.profileId === ProfileIds.Diabetes) {
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
            : replacePercentageWithArialFont(textMetadata.Name), 140);

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

    var hideSupportingData = !MT.config.showCallOutBoxPopulation;

    // Top compare links
    var areComparingSimilarAreas = isSimilarAreas();
    var similarAreaLabel = '';
    var similarAreaOptions = [];
    if (areComparingSimilarAreas) {
        var similarAreaLabel;
        if (isNearestNeighbour()) {
            similarAreaLabel = 'CIPFA nearest neighbours';
        } else if (doesAreaTypeCompareToOnsCluster()) {
            similarAreaLabel = 'ONS cluster group';
        } else {
            similarAreaLabel = 'Deprivation group';
        }
    } else {
        // Nearest neighbours
        if (model.areaTypeId === AreaTypeIds.CountyUA) {
            similarAreaOptions.push({ func: 'viewNearestNeighbours', text: 'Similar areas' });
        }

        // Other option
        var text = doesAreaTypeCompareToOnsCluster()
            ? 'Similar areas'
            : 'Deprivation group';
        similarAreaOptions.push({ func: 'viewSimilar', text: text });
    }

    // Whether or not to show the man next to the top indicators
    var showMan = profileId !== ProfileIds.Suicide;
    var extendedViewModel = CallOutBox.getExtendedModel(areaPopulation, getGrade, areaValues);
    var viewModel = {
        areComparingSimilarAreas: areComparingSimilarAreas,
        similarAreaLabel: similarAreaLabel,
        similarAreaOptions: similarAreaOptions,
        showMan: showMan,
        hideSupportingData: hideSupportingData,
        rankingHtml: CallOutBox.getRankingHtml(ranks[groupRootIndex], indicatorId),
        nameofplace: areaDetails.Area.Name,
        areaCode: model.areaCode,
        areaTypeName: getSimpleAreaTypeName(),
        ranking: rankingValF,
        period: ranks[groupRootIndex].Period,
        unit: unitFormat.getLongLabel(),
        unitClass: unitFormat.getClass(),
        imageclass: imageClass,
        filterheader: metadata.ValueType.Id === ValueTypeIds.IndirectlyStandardisedRatio ? textMetadata.Definition : indicatorName,
        rankClass: gradeClass,
        valueNote: valueNoteText,
        indicatordescription: indicatorDescription,
        isdomainnormal: isDomainNormal,
        topIndicatorText: topIndicatorText,
        dataSource: textMetadata.DataSource,
        isDefinitionOpen: mapState.isDefinitionOpen,
        hasPracticeData: hasPracticeData
    };
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
    var altModel = {};
    altModel.areaCode = model.areaCode;
    altModel.areaTypeId = model.areaTypeId;
    altModel.profileId = ProfileIds.HealthChecks;
    altModel.groupId = GroupIds.HealthChecks.HealthCheck;
    return altModel;
}

/**
 * Template of indicator links under a domain
 */
templates.add('causes',
    '{{#causes}}<li id={{index}}-iid-{{id}} class="{{cssClass}}"><a href="javascript:selectIndicator({{index}}, {{id}})">{{{name}}}</a></li>{{/causes}}');
