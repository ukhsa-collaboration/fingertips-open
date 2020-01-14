'use strict';

/**
* Functions of the Overview tab.
* @module tartanRug
*/
var tartanRug = {};

function goToTartanRugPage() {
    // FT.ajaxLock test cannot go here 
    setPageMode(PAGE_MODES.TARTAN);

    if (!areIndicatorsInDomain()) {
        displayNoData();
    } else {

        ui.storeScrollTop();

        // Init view manager
        var ns = tartanRug;
        if (!ns.viewManager) {
            // Init view
            ns.viewManager = new ns.ViewManager(FT.config);
            ns.viewManager.init();
        }

        setTartanRugHtml(false);

        showAndHidePageElements();

        showDataQualityLegend();

        showTargetBenchmarkOption(groupRoots);

        ui.setScrollTop();

        $('#left-tartan-table,#rightTartanTable').floatThead({ position: 'absolute' });

        unlock();
    }

    overrideTartanRugLegend();

    toggleLegend(true);
};

function overrideTartanRugLegend() {
    if ($('#keyTartanRug').is(':visible')) {

        var $rag3 = $('#tartan-legend-rag-3'),
            $rag5 = $('#tartan-legend-rag-5'),
            $bob3 = $('#tartan-legend-bob-3'),
            $bob5 = $('#tartan-legend-bob-5'),
            $quintileRag = $('#tartan-quintile-rag'),
            $quintileBob = $('#tartan-quintile-bob'),
            $ragAndBobSection = $('#tartan-rag-and-bob-section'),
            $quintileSection = $('#tartan-quintile-key-table');

        $rag3.hide();
        $rag5.hide();
        $bob3.hide();
        $bob5.hide();
        $quintileRag.hide();
        $quintileBob.hide();
        $ragAndBobSection.hide();
        $quintileSection.hide();

        // Quintile BOB
        if (_.some(FT.model.groupRoots,
            function (x) {
                return x.ComparatorMethodId === ComparatorMethodIds.Quintiles && (
                    x.PolarityId === PolarityIds.NotApplicable ) })) {

            $quintileBob.show();
            $quintileSection.show();
        }

        // Quintile RAG
        if (_.some(FT.model.groupRoots,
            function (x) {
                return x.ComparatorMethodId === ComparatorMethodIds.Quintiles &&
                    (x.PolarityId === PolarityIds.RAGLowIsGood || x.PolarityId === PolarityIds.RAGHighIsGood || x.PolarityId === PolarityIds.BlueOrangeBlue);
            })) {

            $quintileRag.show();
            $quintileSection.show();
        }

        // RAG
        if (_.some(FT.model.groupRoots,
            function (x) {
                return x.ComparatorMethodId === ComparatorMethodIds.SingleOverlappingCIsForTwoCiLevels &&
                    (x.PolarityId === PolarityIds.RAGLowIsGood || x.PolarityId === PolarityIds.RAGHighIsGood);
            })) {
            // RAG5
            $rag5.show();
            $ragAndBobSection.show();
        } else if (_.some(FT.model.groupRoots,
            function (x) {
                return x.ComparatorMethodId !== ComparatorMethodIds.Quintiles &&
                    (x.PolarityId === PolarityIds.RAGLowIsGood || x.PolarityId === PolarityIds.RAGHighIsGood);
            })) {
            // 3 level
            $rag3.show();
            $ragAndBobSection.show();
        }

        // BOB
        if (_.some(FT.model.groupRoots,
            function (x) {
                return x.ComparatorMethodId === ComparatorMethodIds.SingleOverlappingCIsForTwoCiLevels &&
                    x.PolarityId === PolarityIds.BlueOrangeBlue;
            })) {
            // BOB5
            $bob5.show();
            $ragAndBobSection.show();

        } else if (_.some(FT.model.groupRoots, function (x) {
            return x.PolarityId === PolarityIds.BlueOrangeBlue &&
                x.ComparatorMethodId !== ComparatorMethodIds.Quintiles;
        })) {
            // BOB3
            $bob3.show();
            $ragAndBobSection.show();
        }
    }
}

/**
* Manages the views on the overview page
* @class tartanRug.ViewManager
*/
tartanRug.ViewManager = function (config) {

    var isInitialised = false;

    /**
    * The overview specific options.
    * @property tabSpecificOptions
    */
    this.tabSpecificOptions = null;

    /**
    * Initialises the view manager
    * @method init
    */
    this.init = function () {
        if (!isInitialised) {
            this.initTabSpecificOptions();

            isInitialised = true;
        }
    };

    /**
    * Initialises the tab specific options
    * @method initTabSpecificOptions
    */
    this.initTabSpecificOptions = function () {

        var optionsConfigImageExport = {
            exportImage: {
                label: 'Export table as image',
                clickHandler: function (ev) {
                    // Needed so does not load the repage
                    ev.preventDefault();
                    tartanRug.saveAsImage();
                    logEvent('ExportImage', getCurrentPageTitle());
                }
            }, exportCsvFile: {
                label: 'Export table as CSV file',
                clickHandler: function (ev) {
                    ev.preventDefault();
                    tartanRug.saveAsCsvFile();
                }
            }
        };

        // Buttons config
        if (config.hasRecentTrends) {
            var clickHandler = goToTartanRugPage;
            optionsConfigImageExport.eventHandlers = [clickHandler, clickHandler, clickHandler];
            optionsConfigImageExport.eventName = 'OverviewTrendOrValueSelected';
        }

        this.tabSpecificOptions = new TabSpecificOptions(optionsConfigImageExport);
    };

    /**
    * Displays the tab specific options
    * @method updateTabSpecificOptions
    */
    this.updateTabSpecificOptions = function () {
        if (config.hasRecentTrends) {
            var options = {
                label: 'Display',
                optionLabels: ['Values', 'Trends', 'Values & Trends']
            };
        } else {
            options = {};
        }

        if (this.tabSpecificOptions) {
            this.tabSpecificOptions.setHtml(options);
        }
    }

    /**
    * Gets the appropriate trend marker display option
    * @method getSelectedTrendMarkerOption
    */
    this.getSelectedTrendMarkerOption = function () {
        var option = TrendDisplayOption.ValuesOnly;
        if (this.tabSpecificOptions) {
            option = this.tabSpecificOptions.getOption();
        }
        return option;
    }
}

function TartanRugCellBuilder(coreDataSet, columnNumber, rowNumber, comparisonConfig,
    trendDisplay, hasTrends, areaCode, targetLegendHtml) {

    this.data = coreDataSet;
    this.dataInfo = new CoreDataSetInfo(coreDataSet);
    this.isValue = this.dataInfo.isValue();

    var html = ['<td  id="tc-' + columnNumber + '-' + rowNumber + '"'],
        useRag = comparisonConfig.useRagColours,
        useQuintileColouring = comparisonConfig.useQuintileColouring;

    this.html = html;

    if (coreDataSet) {
        if (trendDisplay !== TrendDisplayOption.TrendsOnly) {

            var sigClass = 'none';
            if (coreDataSet.AreaCode !== NATIONAL_CODE || (coreDataSet.AreaCode === NATIONAL_CODE && targetLegendHtml !== '')) {
                this.sig = coreDataSet.Sig[comparisonConfig.comparatorId];
                sigClass = this.getSigClass(useRag, useQuintileColouring, coreDataSet.IndicatorId, coreDataSet.SexId, coreDataSet.AgeId);
            }
            
            html.push(' class="', this.dataInfo.isNote() ? sigClass + ' valueNote' : sigClass, '"');
        }

        if (this.dataInfo.isNote()) {
            html.push(' vn="', this.data.NoteId, '"');
        }
        html.push(' areacode="', coreDataSet.AreaCode, '" categoryid="', coreDataSet.CategoryId, '"');

    } else {
        html.push(' areacode="' + areaCode + '"');
    }

    html.push(' style="cursor:pointer;" onclick="recentTrendSelected.byAreaAndRootIndex(\'' + areaCode + '\',' + rowNumber + ')"');

    html.push('>');

    if (hasTrends && trendDisplay && trendDisplay !== TrendDisplayOption.ValuesOnly) {
        var trendAreaCode = coreDataSet ? coreDataSet.AreaCode : null;
        if (trendDisplay === TrendDisplayOption.TrendsOnly) {
            this.addTrend(html, groupRoots[rowNumber], trendAreaCode);
        } else {
            this.addTrendWithText(html, groupRoots[rowNumber], trendAreaCode);
        }
    } else {
        if (coreDataSet) {
            html.push('<div class="tartan-box">', this.getTartanImage(useRag, useQuintileColouring, coreDataSet.IndicatorId, coreDataSet.SexId, coreDataSet.AgeId));
        } else {
            html.push('<div class="tartan-box">');
        }

        this.addText(html);
        html.push('</div>');
    }

    html.push('</td>');
}

TartanRugCellBuilder.prototype = {

    getHtml: function () {
        return this.html.join('');
    },

    getTartanImage: function (useRag, useQuintileColouring, indicatorId, sexId, ageId) {

        if (!this.isValue) return '';

        var img = getSignificanceImg(this.sig, useRag, useQuintileColouring, indicatorId, sexId, ageId);

        return img === null ?
            '' :
            '<img class="tartan-fill print-only" src="' + FT.url.img + img + '"/>';
    },

    addText: function (html) {
        html.push('<div class="tartan-text">',
            new ValueDisplayer().byDataInfo(this.dataInfo, { noCommas: 'y' }),
            '</div>');
    },

    addTrend: function (html, root, areaCode) {
        var trendMarker = this._getTrendMarker(root, areaCode);
        html.push('<div class="tartan-trend">',
            trendMarker,
            '</div>');
    },

    addTrendWithText: function (html, root, areaCode) {
        var trendMarker = this._getTrendMarker(root, areaCode);
        html.push('<div class="tartan-value-and-trend">',
            trendMarker,
            '<br/>',
            new ValueDisplayer().byDataInfo(this.dataInfo, { noCommas: 'y' }),
            '</div>');
    },

    _getTrendMarker: function (root, areaCode) {
        var trendMarkerCode;

        if (this.dataInfo.isValue()) {
            if (root.RecentTrends[areaCode] !== undefined) {
                trendMarkerCode = root.RecentTrends[areaCode].Marker;
            }
        } else {
            trendMarkerCode = TrendMarkerValue.CannotCalculate;
        }

        return getTrendMarkerImage(trendMarkerCode, root.PolarityId);
    },

    getSigClass: function (useRag, useQuintileColouring, indicatorId, sexId, ageId) {

        if(isEnglandAreaType() && pages.getDefault() === PAGE_MODES.TARTAN) {
            return '';
        }

        if (!this.isValue) return '';
        if (useQuintileColouring) {
            if (this.sig > 0 && this.sig < 6) {
                var groupRoot = getGroupRootByIndicatorSexAndAge(indicatorId, sexId, ageId);
                if (groupRoot.PolarityId === PolarityIds.NotApplicable) {
                    return 'grade-bob-quintile-' + this.sig;
                } else {
                    return 'grade-rag-quintile-' + this.sig;
                }
            }
        } else {
            switch (this.sig) {

                case 1:
                    return useRag ? 'worse' : 'bobLower';

                case 2:
                    return 'same';

                case 3:
                    return useRag ? 'better' : 'bobHigher';

                case 4:
                    return useRag ? 'worst' : 'bobLowest';

                case 5:
                    return useRag ? 'best' : 'bobHighest';
            }
        }
        return 'none';
    }
}

function setTartanRugHtml(isDownloadable) {

    var i,
        html,
        sortedAreas = FT.data.sortedAreas,
        isRegionalDisplayed = isSubnationalColumn();

    // Ensure trend marker mode is defined
    var trendMarkerOption = tartanRug.viewManager.getSelectedTrendMarkerOption();

    var rug = new TartanRug(FT.data.sortedAreas.length, isRegionalDisplayed, isDownloadable, trendMarkerOption);
    tartanRugState.rug = rug;
    rug.init();

    tooltipManager.setTooltipProvider(
        new TartanRugTooltipProvider());

    // National benchmark
    var isNationalDisplayed = enumParentDisplay !== PARENT_DISPLAY.REGIONAL_ONLY;
    if (isNationalDisplayed) {
        var nationalArea = getNationalComparator();
        rug.addBenchmarkName(nationalArea.Name);
    }

    // Subnational benchmark
    if (isRegionalDisplayed) {
        var regionalArea = '';
        var regionalAreaName = '';
        if (FT.model.isNearestNeighbours()) {
            regionalAreaName = 'Neighbours average';
        } else {
            regionalArea = getParentArea();
            regionalAreaName = regionalArea.Name;
        }
        rug.addBenchmarkName(trimName(regionalAreaName, TARTAN_RUG_MAX_CHARS));
    }

    // Areas
    for (i in sortedAreas) {
        if (sortedAreas.hasOwnProperty(i)) {
            var currentSortedAreaCode = sortedAreas[i].Code;
            if (currentSortedAreaCode !== NATIONAL_CODE) {
                rug.addArea(sortedAreas[i]);
            }
        }
    }

    var indicatorMetadataHash = ui.getMetadataHash();
    var useQuintileColouring = false;

    if (indicatorMetadataHash) {

        var groupingSubheadings = ui.getGroupingSubheadings();

        var isSubheadingRow = true,
            groupRootIndex = 0;
        while (groupRootIndex < groupRoots.length) {

            var groupRoot = groupRoots[groupRootIndex],
                id = groupRoot.IID,
                indicatorMetadata = indicatorMetadataHash[id],
                metadataText = indicatorMetadata.Descriptive,
                data = groupRoot.Data,
                regionalGrouping = getRegionalComparatorGrouping(groupRoot),
                indicatorDataQualityHtml = getIndicatorDataQualityHtml(metadataText.DataQuality),
                comparisonConfig = new ComparisonConfig(groupRoot, indicatorMetadata),
                targetLegendHtml = getTargetLegendHtml(comparisonConfig, indicatorMetadata);

            var columnNumber = 1,
                hasTrends = groupRoot.RecentTrends;

            // Choose comparator & Colouring: RAG, Quintile or not
            if (!useQuintileColouring) {
                useQuintileColouring = comparisonConfig.useQuintileColouring;
            }

            if (_.some(groupingSubheadings, function (x) { return x.Sequence === groupRoot.Sequence }) && isSubheadingRow) {
                var subheadings = _.filter(groupingSubheadings, function (x) { return x.Sequence === groupRoot.Sequence });
                for (i in subheadings) {
                    if (subheadings.hasOwnProperty(i)) {
                        addSubheadingRowToOverview(rug, subheadings[i].Subheading, groupRootIndex);
                    }
                }
                isSubheadingRow = false;
            } else {
                rug.newRow(groupRootIndex);
                rug.setSubheading(false);

                var newDataBadge = hasDataChanged(groupRoot) ? NEW_DATA_BADGE : '';

                rug.setIndicator(
                    metadataText.Name + new SexAndAge().getLabel(groupRoot),
                    indicatorDataQualityHtml + newDataBadge + '<br>' +
                    targetLegendHtml
                );

                // National data
                if (isNationalDisplayed) {
                    var nationalGrouping = getNationalComparatorGrouping(groupRoot);


                    html = new TartanRugCellBuilder(nationalGrouping.ComparatorData,
                        columnNumber++, groupRootIndex, comparisonConfig, trendMarkerOption,
                        hasTrends, nationalArea.Code, targetLegendHtml).getHtml();
                    rug.addBenchmarkValue(html);
                }

                // Subnational data
                if (isRegionalDisplayed) {
                    html = new TartanRugCellBuilder(regionalGrouping.ComparatorData,
                        columnNumber++, groupRootIndex, comparisonConfig, trendMarkerOption,
                        hasTrends, regionalArea.Code, targetLegendHtml).getHtml();
                    rug.addBenchmarkValue(html);
                }

                // Time period
                var periodGrouping = isNationalDisplayed && nationalGrouping.hasOwnProperty('Period')
                    ? nationalGrouping
                    : regionalGrouping;
                rug.setTimePeriod(periodGrouping.Period);

                // Area values
                for (var j in sortedAreas) {
                    var code = sortedAreas[j].Code;
                    if (code !== NATIONAL_CODE) {
                        html = new TartanRugCellBuilder(getDataFromAreaCode(data, code),
                            columnNumber++, groupRootIndex, comparisonConfig, trendMarkerOption,
                            hasTrends, code, targetLegendHtml).getHtml();
                        rug.addRowValue(html);
                    }
                }

                groupRootIndex++;
                isSubheadingRow = true;
            }
        }

        rug.hasRecentTrends = isDefined(groupRoots[0]) && isDefined(groupRoots[0].RecentTrends);

        html = templates.render('tartan', rug);

        tartanRug.viewManager.updateTabSpecificOptions();
    }

    pages.getContainerJq().html(html).width(
        rug.leftTableWidth + rug.viewport + rug.extraSpace /*required because tables are bigger in Chrome*/);

    // Tooltips
    addTooltips();

    // Enable tooltip
    $('[data-toggle="tooltip"]').tooltip();
};

function getTartanRugData() {

    var sortedAreas = FT.data.sortedAreas;
    var data = {};
    data.AreaType = FT.menus.areaType.getName();
    data.Row = [];

    // // National benchmark
    var isNationalDisplayed = enumParentDisplay !== PARENT_DISPLAY.REGIONAL_ONLY;

    var indicatorMetadataHash = ui.getMetadataHash();

    var groupRootIndex = 0;
    while (groupRootIndex < groupRoots.length) {

        data.Row[groupRootIndex] = {};

        var groupRoot = groupRoots[groupRootIndex],
            id = groupRoot.IID,
            indicatorMetadata = indicatorMetadataHash[id],
            metadataText = indicatorMetadata.Descriptive;

        data.Row[groupRootIndex].IndicatorID = id;
        data.Row[groupRootIndex].IndicatorName = metadataText.Name;
        data.Row[groupRootIndex].compDataInfoNational = {};
        data.Row[groupRootIndex].compDataInfoRegional = {};
        data.Row[groupRootIndex].compDataInfoAreas = {};

        // National data
        var nationalArea = getNationalComparator();
        var nationalGrouping = getNationalComparatorGrouping(groupRoot);

        data.Row[groupRootIndex].compDataInfoNational.Row = [];
        data.Row[groupRootIndex].compDataInfoNational.Row[0] = {};
        setDataComparatorForCsv(groupRoots[groupRootIndex], data.Row[groupRootIndex].compDataInfoNational.Row[0]);
        data.Row[groupRootIndex].compDataInfoNational.ParentCode = '';
        data.Row[groupRootIndex].compDataInfoNational.ParentName = '';

        // Subnational data
        var regionalArea = getParentArea();
        var regionalGrouping = getRegionalComparatorGrouping(groupRoot)

        data.Row[groupRootIndex].compDataInfoRegional.Row = [];
        data.Row[groupRootIndex].compDataInfoRegional.Row[0] = {};
        setDataComparatorForCsv(groupRoots[groupRootIndex], data.Row[groupRootIndex].compDataInfoRegional.Row[0]);
        data.Row[groupRootIndex].compDataInfoRegional.ParentCode = nationalArea.Code;
        data.Row[groupRootIndex].compDataInfoRegional.ParentName = nationalArea.Name;


        // Time period
        var periodGrouping = isNationalDisplayed && nationalGrouping.hasOwnProperty('Period')
            ? nationalGrouping
            : regionalGrouping;

        data.Row[groupRootIndex].TimePeriod = periodGrouping.Period;

        data.Row[groupRootIndex].compDataInfoAreas.Row = [];
        data.Row[groupRootIndex].compDataInfoAreas.ParentCode = regionalArea.Code;
        data.Row[groupRootIndex].compDataInfoAreas.ParentName = regionalArea.Name;

        // Area values
        for (var j in sortedAreas) {
            var area = sortedAreas[j];
            if (area.Code !== NATIONAL_CODE) {
                data.Row[groupRootIndex].compDataInfoAreas.Row[j] = {};
                setDataComparatorForCsv(groupRoots[groupRootIndex], data.Row[groupRootIndex].compDataInfoAreas.Row[j]);
            }
        }

        groupRootIndex++;

    }
    return data;
};

function setDataComparatorForCsv(data, obj) {

    obj.Sex = getSexName(data.Sex);
    obj.Age = getAgeName(data.Age);
    obj.AreaCode = data.AreaCode;
    obj.CategoryType = getCategoryTypeName(data);
    obj.Category = getCategoryName(data);
    obj.LoCI = data.LoCI;
    obj.UpCI = data.UpCI;
    obj.Count = data.Count;
    obj.Denom = data.Denom;
    obj.Val = data.Val;
};

function getSexName(sex) {
    return sex.Name;
}

function getAgeName(age) {
    return age.Name;
}

function getCategoryTypeName(data) {
    return data.Data[0].CategoryTypeId;
}

function getCategoryName(data) {
    return data.Data[0].CategoryId;
}

function addTooltips() {
    for (var groupRootIndex in groupRoots) {

        // Value cells
        for (var i = 0; i <= FT.data.sortedAreas.length + 2; i += 1) {
            tooltipManager.initElement('tc-' + i + '-' + groupRootIndex);
        }

        // Indicator name
        tooltipManager.initElement('rug-indicator-' + groupRootIndex);
    }
}

function addSubheadingRowToOverview(rug, subheading, groupRootIndex) {
    rug.newRow(groupRootIndex);
    rug.setIndicator(subheading, null);
    rug.setSubheading(true);

    rug.addRowValue('<td colspan="' + FT.data.sortedAreas.length + '"></td>');
}

function sortTartanRugAToZ() {
    sortAreasAToZ(invertSortOrder(sortOrder.getOrder(0)), FT.data.sortedAreas);
    goToTartanRugPage();
}

/**
* Sort tartan rug by nearest neighbours rank
* @class sortTartanRugByNearestNeighbours
*/
function sortTartanRugByNearestNeighbours() {
    FT.data.sortedAreas = sortAreasByRank();
    goToTartanRugPage();
}

function sortTartanRug(rowNumber) {
    var groupRoot = groupRoots[rowNumber];
    FT.data.sortedAreas = new AreaAndDataSorter(sortOrder.getOrder(rowNumber),
        groupRoot.Data, FT.data.sortedAreas, areaHash).byValue();
    goToTartanRugPage();
};

function unhighlightCell(td) {
    $(td).css({
        'border-color': '#eee',
        'cursor': 'default'
    }
    );
};

var sortOrder = new function () {

    this._highToLow = false;

    this.getOrder = function (rowNumber) {

        if (this._lastRowNumber == rowNumber) {
            this._highToLow = !this._highToLow;
        } else {
            this._highToLow = false;
        }

        this._lastRowNumber = rowNumber;

        if (this._highToLow) {
            return 0;
        }
        return 1;
    };
}

function TartanRug(areaCount, isSubnationalDisplayed, isDownloadable, trendDisplay) {
    /* Some of the this variables are used by a mustache template */
    this.areaCount = areaCount;

    var displayedAreas
        , minimumAreaCount = 12
        , row = null
        , borderWidth = 2 * 1;

    var indicatorWidth = 222 + borderWidth // width 220 + padding*2 2
        , sortWidth = 32/*width*/ + borderWidth
        , periodWidth = 56/*width*/ + borderWidth;

    // (th.pLink, th.rotate, th.comparator-header)
    this.valueCellWidth = 39/*width*/ + borderWidth;

    //to prevent browser horizontal scroll bar in some browsers
    this.extraSpace = 30;

    // Paging variables
    this.pageIndex = 0;
    this.pageMargins = [0];
    this.placeMargins = [0];
    this.lastIndex = 999;

    this.rowNumber = 0;
    this.benchmarks = [];
    this.areas = [];
    this.rows = [];
    this.groupingSubheadings = [];
    this.isNotNN = !FT.model.isNearestNeighbours();
    this.isNotAreaList = FT.model.parentTypeId !== AreaTypeIds.AreaList ? true : false;

    this.isScrollbar = null;

    this.trendDisplaySelected = trendDisplay;

    this.displayValuesOnly = (trendDisplay === TrendDisplayOption.ValuesOnly);
    this.displayTrendsOnly = (trendDisplay === TrendDisplayOption.TrendsOnly);
    this.displayValuesAndTrends = (trendDisplay === TrendDisplayOption.ValuesAndTrends);

    var benchmarkColumnCount = isSubnationalDisplayed ? 2 : 1;
    this.leftTableWidth = indicatorWidth + sortWidth + periodWidth +
        (this.valueCellWidth * benchmarkColumnCount) - 2/*no right border*/;

    this.allAreasWidth = (areaCount * this.valueCellWidth);

    this.init = function () {

        displayedAreas = this.getBestVisibleAreaCount(isDownloadable);
        this.viewport = (this.valueCellWidth * displayedAreas);

        this.isScrollbar = areaCount !== displayedAreas;
        this.scrollIndicatorWidth = this.isScrollbar ?
            ((this.viewport / this.allAreasWidth) * this.viewport) :
            0;
    }

    this.isMostAppropriateNumberOfAreasDisplayed = function () {
        return this.getBestVisibleAreaCount(isDownloadable) !== displayedAreas;
    }

    this.getDisplayedAreas = function () {
        return displayedAreas;
    }

    this.getBestVisibleAreaCount = function (downloadable) {

        var w;
        var extraSpaceForImageExport = 60;
        if (downloadable) {
            w = (this.leftTableWidth + this.allAreasWidth + this.extraSpace) + extraSpaceForImageExport;
        } else {
            w = $(window).width();
        }

        // Will all areas fit?
        if (w > this.leftTableWidth + this.allAreasWidth + this.extraSpace) {
            return this.areaCount;
        }

        var count = this.areaCount;
        while (count > minimumAreaCount) {
            count--;
            if (w > this.leftTableWidth + (count * this.valueCellWidth) +
                this.extraSpace) {
                break;
            }
        }
        return count;
    };

    this.addArea = function (area) {

        var areaName = getAreaNameToDisplay(area);

        if (FT.model.isNearestNeighbours() && area.Code !== FT.model.areaCode) {
            areaName = area.Rank + ' - ' + areaName;
        }

        this.areas.push({
            name: this.getVerticalText(trimRugHeading(areaName)),
            code: area.Code
        });
    };

    this.addBenchmarkName = function (txt) {
        this.benchmarks.push([this.getVerticalText(txt)]);
    };

    this.newDivider = function (heading) {
        this.rows.push({
            divider: {
                heading: heading, colSpan: this.areaCount +
                    3/*indicator + period + sort*/
            }
        });
    };

    this.newRow = function (rootIndex) {
        row = {
            number: ++this.rowNumber,
            rootIndex: rootIndex,
            benchmarkValues: [],
            values: []
        };
        this.rows.push(row);
    };

    this.setIndicator = function (name, dataQuality) {
        row.indicator = name;
        row.indicatorDataQuality = dataQuality;
    };

    this.setTimePeriod = function (period) {
        row.period = period;
    };

    this.setSubheading = function (subheading) {
        row.subheading = subheading;
    }

    this.addRowValue = function (txt) {
        row.values.push({ html: txt });
    };

    this.addBenchmarkValue = function (txt) {
        row.benchmarkValues.push({ html: txt });
    };

    this.getVerticalText = function (txt) {
        // v parameter included as cache buster
        return '<img class="verticalText" src="' + FT.url.bridge
            + 'img/vertical-text?v=2&text=' + encodeURIComponent(txt)
            + '" alt=""/>';
    };
};


function trimRugHeading(areaName) {
    // Practices are upper case so take up more space
    return trimName(areaName, FT.model.areaTypeId === AreaTypeIds.Practice ? 29 : TARTAN_RUG_MAX_CHARS);
}


function tartanRugResize(displayState) {
    var rug = tartanRugState.rug;

    switch (displayState) {
        case tartanRugSizeState.Expand:
            setTartanRugHtml(true);
            break;
        case tartanRugSizeState.Resize:
            setTartanRugHtml(false);
            break;
        default:
            if (rug !== null && rug.isMostAppropriateNumberOfAreasDisplayed()) {
                setTartanRugHtml(false);
            }
            break;
    }

    $('#left-tartan-table,#rightTartanTable').floatThead('destroy');

    //Redraw the 'sticky header'
    $('#left-tartan-table,#rightTartanTable').floatThead({ position: 'absolute' });
};

function highlightCell(td) {
    $(td).css({
        'border-color': colours.border,
        'cursor': 'default'
    }
    );
};

function rugScrollClick(direction) {

    if (!FT.ajaxLock) {

        lock();

        var scrollContent = $('.scrollContent');
        scrollContent.stop(true, true);
        var scrollPlace = $('.scroll-place');
        scrollPlace.stop(true, true);

        var rug = tartanRugState.rug;

        // Is page stored?
        var pageIndex = rug.pageIndex + direction;
        if (pageIndex < 0 || pageIndex > rug.lastIndex) {
            // Not allowed
        } else {

            var margins = rug.pageMargins;

            if (pageIndex >= margins.length) {

                // Calculate new index

                var ml = margins[margins.length - 1];
                var borderWidth = 2;

                var scrollWidth = $(".scroll-pane").width();
                ml += (scrollWidth - (rug.valueCellWidth * 2)/*overlap 2 areas from previous page*/
                    - borderWidth) * direction;

                var contentWidth = rug.allAreasWidth;
                var maxMargin = contentWidth - scrollWidth + borderWidth;

                if (ml > maxMargin) {
                    ml = maxMargin;
                    rug.lastIndex = pageIndex;
                }

                // push index
                margins.push(ml);

                // Place margin
                var scrollPlaceWidth = scrollPlace.width();
                var maxMarginPlace = scrollWidth - scrollPlaceWidth;
                var margin = (ml / maxMargin) * maxMarginPlace;
                rug.placeMargins.push(margin);

            } else {
                ml = margins[pageIndex];
                margin = rug.placeMargins[pageIndex];
            }

            rug.pageIndex = pageIndex;

            scrollContent.animate({ 'margin-left': -ml }, 400);
            scrollPlace.animate({ 'margin-left': margin }, 400);
        }

        unlock();

        logEvent('TartanRug', 'RugScrolled');
    }
}

var tartanRugState = {
    rug: null,
    trendsTooltip: new RecentTrendsTooltip()
}

function TartanRugTooltipProvider() {

    this.getHtml = function (id) {

        if (id !== '') {
            var bits = id.split('-'),
                rootIndex = bits[2];

            // Indicator name, e.g. 'rug-indicator_1'
            if (id.indexOf('rug-indicator') > -1) {
                return '';
            }

            return this.getTartanText(id, rootIndex);
        }
        return '';
    };

    this._val = function (unit, valF) {
        return new ValueWithUnit(unit).getFullLabel(valF);
    };

    this.getTartanText = function (id, groupRootIndex) {

        var $tartanCell = $('#' + id);

        var areaCode = $tartanCell.attr('areacode');

        if (!areaCode) {
            // Area code is not defined because no data is available
            return '';
        }

        var valueNoteId = $tartanCell.attr('vn');
        var root = groupRoots[groupRootIndex];
        var data = getDataFromAreaCode(root.Data, areaCode);
        var dataInfo = new CoreDataSetInfo(data);
        var area = areaHash[areaCode];
        var isLocalArea = isDefined(area);
        var message = 'No data';
        var metadata = ui.getMetadataHash()[root.IID];
        var areaName = '';
        var isNN = areaCode.indexOf('nn-') > -1;

        if (isLocalArea) {
            if (dataInfo.isValue()) {
                message = this._val(metadata.Unit, data.ValF);
            }
            areaName = area.Name;
        } else {
            // Try get comparator data
            var parentAreaCode = FT.model.parentCode;

            var isSubnational = false;
            if (isNN) {
                isSubnational = true;
            } else {
                isSubnational = areaCode === parentAreaCode;
            }

            var grouping = isSubnational
                ? getRegionalComparatorGrouping(root)
                : getNationalComparatorGrouping(root);

            data = grouping.ComparatorData;

            if (data && data.Val !== -1) {
                message = this._val(metadata.Unit, data.ValF);
            }

            if (isNN) {
                areaName = 'Neighbours average';
            } else {
                var codeForName = isSubnational ? parentAreaCode : areaCode;
                areaName = getComparatorFromAreaCode(codeForName).Name;
            }
        }

        var valueNote = new ValueNoteTooltipProvider().getHtmlFromNoteId(valueNoteId);
        return renderTooltip(id, areaName, message, valueNote, metadata.Descriptive.Name);
    };
}

function renderTooltip(id, areaName, message, valueNote, getIndicatorName) {
    templates.add('rugTooltip',
        '<span id="tooltipArea">{{areaName}}</span><span id="tooltipData">' +
        '{{#isDisplayValuesAndTrends}} {{{message}}} <br> {{{trendMessage}}}{{/isDisplayValuesAndTrends}}' +
        '{{^isDisplayValuesAndTrends}} {{{message}}}{{/isDisplayValuesAndTrends}}' +
        '</span>{{{valueNote}}}' +
        '<span id="tooltipIndicator">{{{getIndicatorName}}}</span>');


    if (tartanRugState.rug.displayTrendsOnly) {
        message = getTrendTooltipText(id);
    }

    var trendMessage = getTrendTooltipText(id);

    var html = templates.render('rugTooltip',
        {
            areaName: areaName,
            message: message,
            trendMessage: trendMessage,
            valueNote: valueNote,
            getIndicatorName: getIndicatorName,
            isDisplayValuesAndTrends: tartanRugState.rug.displayValuesAndTrends
        });
    return html;
}

var tartanRugSizeState = {
    Normal: 0, // Normal - when tartan rug rendered first time
    Expand: 1, // Expand - removes the overflow left-table + right-table
    Resize: 2  // Resize - put the width back to normal without cell count check
}

var TrendDisplayOption = {
    ValuesOnly: 0,
    TrendsOnly: 1,
    ValuesAndTrends: 2
}


tartanRug.saveAsImage = function () {

    // Reset so no scroll - any scroll can produce weird results
    $(window).scrollTop(0);

    // resize the tartan rug and show overflow
    tartanRugResize(tartanRugSizeState.Expand);
    $('#keyTartanRug').css('font-family', 'Arial');

    styleTartanRugForExport();

    // capture the tartan rug
    var overviewContainer = $('#overview-container');
    var keyContainer = $('#keyTartanRug').clone().attr('id', 'overviewKeys').after('#id');
    overviewContainer.prepend(keyContainer);
    saveElementAsImage(overviewContainer, 'OverviewTable');

    // put the tartan rug back to normal        
    goToTartanRugPage();
}

tartanRug.saveAsCsvFile = function () {

    var parameters = new ParameterBuilder()
        .add('parent_area_type_id', FT.model.parentTypeId)
        .add('child_area_type_id', FT.model.areaTypeId)
        .add('area_codes', getAreasCodeForCsvDownload())
        .add('parent_area_code', getParentAreaCode())
        .add('category_area_code', getCategoryAreaCode());

    if (isInSearchMode()) {
        parameters.add('profile_id', FT.model.profileId)
            .add('indicator_ids', indicatorIdList.getAllIds());
        downloadLatestNoInequalitiesDataCsvFileByIndicator(FT.url.corews, parameters);

    } else {
        parameters.add('group_id', FT.model.groupId);
        downloadLatestNoInequalitiesDataCsvFileByGroup(FT.url.corews, parameters);
    }
}

function styleTartanRugForExport() {
    // hide download image link (after tartan has been resized)
    $('.tartan-export-chart-box').hide();

    $('#trendDisplayOptions').hide();

    // hide scrollbars
    $('.scrollbar').css('visibility', 'hidden');

    // change the div background colour to white as required by serverside code.
    $('#overview-container').css('background', 'white');

    // change the font
    $('#rightTartanTable,#left-tartan-box').css('font-family', 'Arial');
}

/**
* Displays the details how trends are calculated 
* @method showTrendInfo
*/
function showTrendInfo() {
    var top = 300;
    var popupWidth = 600;
    var left = lightbox.getLeftForCenteredPopup(popupWidth);

    var parameters = new ParameterBuilder().add('profile_id', ProfileIds.Phof).add('key', 'trends-info');

    ajaxGet('api/content',
        parameters.build(),
        function (contentText) {
            var html = contentText;
            // remove floatThead
            $('#left-tartan-table,#rightTartanTable').floatThead('destroy');
            // enable floatThead before closing lightbox
            lightbox.preHide = function () {
                $('#left-tartan-table,#rightTartanTable').floatThead({ position: 'absolute' });
            };

            lightbox.show(html, top, left, popupWidth);
            ajaxMonitor.callCompleted();
        });
}

function getTrendTooltipText(id) {

    var bits = id.split('-');
    var rootIndex = bits[2];
    var recentTrends = groupRoots[rootIndex].RecentTrends;
    if (!recentTrends) {
        // No recent trends available
        return '';
    }

    var $tartanCell = $('#' + id);
    var areaCode = $tartanCell.attr('areacode');
    var trendDataForHighlightedArea = recentTrends[areaCode];
    return tartanRugState.trendsTooltip.getTooltipByData(trendDataForHighlightedArea);
}

function indicatorNameClicked(id) {
    goToBarChartPage(id, true);
}

function toggleLegend(initialLoad) {
    if (!initialLoad) {
        FT.config.displayLegend = !FT.config.displayLegend;
    }

    if (FT.config.displayLegend) {
        $('.overview-legend-link').text('Hide legend');
        $('#tartan-legend-container').show();
    } else {
        $('.overview-legend-link').text('Show legend');
        $('#tartan-legend-container').hide();
    }
}

function highlightRow(td, isBgChanged) {

    var cells = FT.data.highlightedRowCells = $(td).parent().children();

    var border = colours.border;

    var attrs = ({
        'border-top-color': border,
        'border-bottom-color': border,
        'cursor': 'pointer'
    });

    if (isDefined(isBgChanged) && isBgChanged) {
        attrs['background-color'] = '#FDFFDD';
    }

    cells.css(attrs);
    cells.first().css({ 'border-left-color': border });
    cells.last().css({ 'border-right-color': border });
}

function unhighlightRow(isBgReset) {
    var cells = FT.data.highlightedRowCells;

    if (cells) {
        var c = '#eee';

        var attrs = {
            'border-top-color': c,
            'border-bottom-color': c,
            'cursor': 'default'
        };

        if (isDefined(isBgReset) && isBgReset) {
            attrs['background-color'] = '#fff';
        }

        cells.css(attrs);
        cells.first().css({ 'border-left-color': c });
        cells.last().css({ 'border-right-color': c });
    }
}

var scrollPlaceHtml = '<div class="scrollbar" style="width:{{viewport}}px;">' +
    '<table id="tartan-rug-main-table" style="{{^isScrollbar}}display:none;{{/isScrollbar}}">' +
    '<tr><td colspan="2" style="width:100%;height:3px;background:#eee;position:absolute;"><div class="scroll-place" style="width:{{scrollIndicatorWidth}}px;position:absolute;"></div><td/></tr>' +
    '<tr><td colspan="2"></td></tr>' +
    '<tr><td class="rug-scroll-left" onclick="rugScrollClick(-1)"></td><td class="rug-scroll-right" onclick="rugScrollClick(1)"></td></tr>' +
    '</table></div>';

templates.add('tartan',
    // Static columns: indicator/period/sort/benchmarks  
    '<div id="tartan-table-box" class="centered">' +
    '<div id="left-tartan-box">' +
    '<table id="left-tartan-table" class="bordered-table" cellspacing="0">' +
    '<thead>' +
    '<tr>' +
    '<th class="rug-indicator white-background"><div>Indicator</div></th>' +
    '<th class="rug-period white-background">Period</th>' +
    '<th class="sortHeader white-background">' +
    '<a class="rowSort" title="Sort area names alphabetically" href="javascript:{{#isNotNN}}sortTartanRugAToZ(){{/isNotNN}}{{^isNotNN}}sortTartanRugByNearestNeighbours(){{/isNotNN}};">&nbsp;</a>' +
    '</th>' +
    '{{#benchmarks}}' +
    '<th class="comparator-header">{{{0}}}</th>' +
    '{{/benchmarks}}' +
    '</tr>' +
    '</thead>' +
    '<tbody>' +
    '{{#rows}}' +
    '<tr>' +
    '{{#divider}}' +
    '<td class="tartan-divider" colspan="5">' +
    '<div class="divider-box">&nbsp;<div class="divider-text">{{heading}}</div></div>' +
    '</td>' +
    '{{/divider}}' +
    '{{^divider}}' +
    '{{#subheading}}' +
    '<td colspan="5" title="{{indicator}}" data-toggle="tooltip" data-placement="top" class="rug-subheading ' + CSS_NUMERIC_WITH_TOOLTIP + '">' +
    '{{{indicator}}}' +
    '</td>' +
    '{{/subheading}}' +
    '{{^subheading}}' +
    '<td id="rug-indicator-{{rootIndex}}" class="rug-indicator pLink" onclick="indicatorNameClicked(\'{{rootIndex}}\');" onmouseover="highlightRow(this, false);" onmouseout="unhighlightRow(false);">' +
    '<div>{{{indicator}}}{{{indicatorDataQuality}}}</div>' +
    '</td>' +
    '<td class="rug-period center">' +
    '{{period}}' +
    '</td>' +
    '<td class="sort">' +
    '<a class="rowSort" title="Sort on {{indicator}}" href="javascript:sortTartanRug({{rootIndex}});">&nbsp;</a>' +
    '</td>' +
    '{{#benchmarkValues}}{{{html}}}{{/benchmarkValues}}' +
    '{{/subheading}}' +
    '{{/divider}}' +
    '</tr>' +
    '{{/rows}}' +
    '</tbody>' +
    '</table>' +
    '</div>' +
    //Scrollable (areas)
    '<div class="scroll-pane" id="scrollable-pane" style="width:{{viewport}}px;">' +
    scrollPlaceHtml +
    '<div id="tartan-scroll-box">' +
    '<table id="rightTartanTable" class="scrollContent bordered-table" cellspacing="0">' +
    '<thead>' +
    '<tr>' +
    '{{#areas}}' +
    '<th class="pLink white-background" {{#isNotNN}} onclick="goToAreaProfilePage(\'{{code}}\');" {{/isNotNN}} onmouseover="highlightCell(this);" onmouseout="unhighlightCell(this);" >' +
    '{{{name}}}' +
    '</th>' +
    '{{/areas}}' +
    '<th class="rug-indicator white-background">' +
    '<div>Indicator</div>' +
    '</th>' +
    '<th class="rug-period white-background">' +
    'Period' +
    '</th>' +
    '<th class="sortHeader white-background">' +
    '<div class="rowSort"></div>' +
    '</th>' +
    '</tr>' +
    '</thead>' +
    '<tbody>' +
    '{{#rows}}' +
    '<tr>' +
    '{{#divider}}' +
    '<td class="tartan-divider" colspan="{{colSpan}}">&nbsp;</td>' +
    '{{/divider}}' +
    '{{^divider}}' +
    '{{#values}}{{{html}}}{{/values}}' +
    '<td class="rug-indicator pLink">' +
    '{{^subheading}}' +
    '<div>{{indicator}}{{{indicatorDataQuality}}}</div>' +
    '{{/subheading}}' +
    '</td>' +
    '<td class="rug-period center">' +
    '{{period}}' +
    '</td>' +
    '<td class="sort">' +
    '<div class="rowSort"></div>' +
    '</td>' +
    '{{/divider}}' +
    '</tr>' +
    '{{/rows}}' +
    '</tbody>' +
    '</table>' +
    scrollPlaceHtml +
    '</div></div></div>'
);

var TARTAN_RUG_MAX_CHARS = 33;

pages.add(PAGE_MODES.TARTAN, {
    id: 'overview',
    title: 'Overview',
    goto: goToTartanRugPage,
    gotoName: 'goToTartanRugPage',
    needsContainer: true,
    jqIds: [
        'keyTartanRug', 'yearSelection', '.geo-menu', 'tartan-key-part',
        'value-note-legend', 'nearest-neighbour-link', 'overview-trend-marker-legend', 'tab-specific-options', 'area-list-wrapper', 'filter-indicator-wrapper'],
    jqIdsNotInitiallyShown: ['data-quality-key', 'target-benchmark-box', 'key-spine-chart'],
    resize: tartanRugResize
});

