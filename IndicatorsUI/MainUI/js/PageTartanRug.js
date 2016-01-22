function TartanRugCellBuilder(coreDataSet, columnNumber, rowNumber, comparisonConfig, trendDisplay) {
    
    this.data = coreDataSet;
    this.dataInfo = new CoreDataSetInfo(coreDataSet);
    this.isValue = this.dataInfo.isValue();
    
    var hasTrends = (isDefined(groupRoots) && isDefined(groupRoots[rowNumber]) && isDefined(groupRoots[rowNumber].TrendMarkers) );

    var html = ['<td id="tc-', columnNumber, '-', rowNumber, '"'],
        useRag = comparisonConfig.useRagColours,
        useQuintileColouring = comparisonConfig.useQuintileColouring;
    
    this.html = html;
    
    if (coreDataSet) {
        if (trendDisplay != TrendDisplayOption.TrendsOnly)
        {
            this.sig = coreDataSet.Sig[comparisonConfig.comparatorId];
            var sigClass = this.getSigClass(useRag, useQuintileColouring);
            html.push(' class="', this.dataInfo.isNote() ? 
                    sigClass + ' valueNote' :
                    sigClass, '"');
        }

        if (this.dataInfo.isNote()) {
            html.push(' vn="', this.data.NoteId, '"');
        }

        html.push(' areacode="', coreDataSet.AreaCode, '"');
    }
    html.push('>');

    if (hasTrends && trendDisplay && trendDisplay != TrendDisplayOption.ValuesOnly) {
        var trendAreaCode = coreDataSet ? coreDataSet.AreaCode : null;
        if (trendDisplay == TrendDisplayOption.TrendsOnly) {
            this.addTrend(html, groupRoots[rowNumber], trendAreaCode);
        } else {
            this.addTrendWithText(html, groupRoots[rowNumber], trendAreaCode);
        }
    } else {
        html.push('<div class="tartanBox">', this.getTartanImage(useRag));
        this.addText(html);
        html.push('</div>');
    }

    html.push('</td>');
}

TartanRugCellBuilder.prototype = {
    
    getHtml : function() {
        return this.html.join('');
    },
    
    getTartanImage: function (useRag) {
        
        if (!this.isValue) return '';
        
        var img = getSignificanceImg(this.sig, useRag);
        
        return img === null ? 
            '' : 
            '<img class="tartanFill printOnly" src="' + FT.url.img + img + '"/>';
    },
    
    addText : function (html) {
        html.push('<div class="tartanText">',
            new ValueDisplayer().byDataInfo(this.dataInfo, {noCommas:'y'}),
            '</div>');
    },
    
    addTrend: function (html, root, areaCode) {
        var trendMarkerCode = areaCode ? root.TrendMarkers[areaCode] : TrendMarkerValue.CannotCalculate;
        var trendMarker = GetTrendMarkerImage(trendMarkerCode, root.PolarityId);
        html.push('<div class="tartanTrend">',
            trendMarker,
            '</div>');
    },

    addTrendWithText: function (html, root, areaCode) {
        var trendMarkerCode = areaCode ? root.TrendMarkers[areaCode] : TrendMarkerValue.CannotCalculate;
        var trendMarker = GetTrendMarkerImage(trendMarkerCode, root.PolarityId);
        html.push('<div class="tartanValueAndTrend">',
            trendMarker,
            '<br/>',
            new ValueDisplayer().byDataInfo(this.dataInfo, { noCommas: 'y' }),
            '</div>');
    },


    getSigClass: function (useRag, useQuintileColouring) {
        
        if (!this.isValue) return '';
            if (useQuintileColouring) {
                switch (true) {
                    case (this.sig > 0 && this.sig < 6):
                        return 'grade-quintile-' + this.sig;
                }
            } else {
                switch (this.sig) {

                    case 1:
                        return useRag ? 'worse' : 'bobLower';

                    case 2:
                        return 'same';

                    case 3:
                        return useRag ? 'better' : 'bobHigher';
                }
            }
            return 'none';
    }
}

function setTartanRugHtml(isDownloadable, trendDisplay) {

    var isRegionalDisplayed = isSubnationalColumn();

    if (!trendDisplay && isDefined(tartanRugState) && isDefined(tartanRugState.rug)) {
        trendDisplay = tartanRugState.rug.trendDisplaySelected;
    }

    if (!trendDisplay) {
        trendDisplay = TrendDisplayOption.ValuesOnly;
    }

    var rug = new TartanRug(sortedAreas.length, isRegionalDisplayed, isDownloadable, trendDisplay);
    tartanRugState.rug = rug;
    rug.init();
    
    if (!groupRoots.length) {
        var html = '<div class="tallCentralMessage">No indicators for current area type</div>';
    }
    else {
        tooltipManager.setTooltipProvider(
            new TartanRugTooltipProvider());
        
        // Benchmarks
        var isNationalDisplayed = enumParentDisplay != PARENT_DISPLAY.REGIONAL_ONLY;
        if (isNationalDisplayed) {
            var nationalArea = getNationalComparator();
            rug.addBenchmarkName(nationalArea.Name);
        }
        

        if (isRegionalDisplayed) {
            var regionalArea = getParentArea();
            rug.addBenchmarkName(trimName(regionalArea.Name,28));
        }
        
        // Areas
        for (var i in sortedAreas) {
            rug.addArea(sortedAreas[i]);
        }
        
        var indicatorMetadataHash =  ui.getMetadataHash();
        var useQuintileColouring=false;

        if (indicatorMetadataHash) {
            for (var groupRootIndex in groupRoots) {
                
                var groupRoot = groupRoots[groupRootIndex],
                id = groupRoot.IID,
                indicatorMetadata = indicatorMetadataHash[id],
                metadataText = indicatorMetadata.Descriptive,
                data = groupRoot.Data,
                regionalGrouping = getRegionalComparatorGrouping(groupRoot);
                var columnNumber = 1;
                
                // Choose comparator & Colouring: RAG, Quintile or not
                var comparisonConfig = new ComparisonConfig(groupRoot, indicatorMetadata);
                if (!useQuintileColouring) {
                    useQuintileColouring = comparisonConfig.useQuintileColouring;
                }

                rug.newRow(groupRootIndex);

                rug.setIndicator(
                    metadataText.Name + new SexAndAge().getLabel(groupRoot),
                    getIndicatorDataQualityHtml(metadataText.DataQuality) + '<br>' + 
                    getTargetLegendHtml(comparisonConfig, indicatorMetadata)
                );
                
                // National data
                if (isNationalDisplayed) {  
                    var nationalGrouping = getNationalComparatorGrouping(groupRoot);
                    html = new TartanRugCellBuilder(nationalGrouping.ComparatorData,
                        columnNumber++, groupRootIndex, comparisonConfig, trendDisplay).getHtml();
                    rug.addBenchmarkValue(html);
                }
                
                // Subnational data
                if (isRegionalDisplayed) {
                    html = new TartanRugCellBuilder(regionalGrouping.ComparatorData,
                        columnNumber++, groupRootIndex, comparisonConfig, trendDisplay).getHtml();
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
                    html = new TartanRugCellBuilder(getDataFromAreaCode(data,code),
                        columnNumber++, groupRootIndex, comparisonConfig, trendDisplay).getHtml();
                    rug.addRowValue(html);
                }
            }

            rug.hasTrendMarkers = isDefined(groupRoots[0]) && isDefined(groupRoots[0].TrendMarkers);

            html = templates.render('tartan', rug);
        }
    }
    
    pages.getContainerJq().html(html).width(
        rug.leftTableWidth + rug.viewport + rug.extraSpace /*required because tables are bigger in Chrome*/);

    toggleQuintileLegend($('.quintileKey'), useQuintileColouring);

    // Tooltips
    for (groupRootIndex in groupRoots) {

        // Value cells
        for (i = 0; i <= sortedAreas.length + 2; i+=1) {
            tooltipManager.initElement('tc-' + i + '-' + groupRootIndex);
        }

        // Indicator name
        tooltipManager.initElement('rug-indicator-' + groupRootIndex);
    }
};

function sortTartanRugAToZ() {    
    sortAreasAToZ(invertSortOrder(sortOrder.getOrder(0)), sortedAreas);
    goToTartanRugPage();
}

/**
* Sort tartan rug by nearest neighbours rank
* @class sortTartanRugByNearestNeighbours
*/
function sortTartanRugByNearestNeighbours() {
    sortedAreas = sortSortedAreasByRank();
    goToTartanRugPage();
}

function sortTartanRug(rowNumber) {

    var groupRoot = groupRoots[rowNumber - 1];
    sortedAreas = new AreaAndDataSorter(sortOrder.getOrder(rowNumber), groupRoot.Data, sortedAreas, areaHash).byValue();
    goToTartanRugPage();
};

function goToTartanRugPage(trendDisplay) {           
    // ajaxLock test cannot go here 
    setPageMode(PAGE_MODES.TARTAN);
    
    ui.storeScrollTop();
    
    setTartanRugHtml(false, trendDisplay);

    showAndHidePageElements();

    showDataQualityLegend();

    showTargetBenchmarkOption(groupRoots);

    ui.setScrollTop();

    if (!isIE()) {
        // Enable floating table header for area names
        $('#leftTartanTable,#rightTartanTable').floatThead();
    }

    unlock();
};

function unhighlightCell(td) {
    $(td).css({
            'border-color': '#eee',
            'cursor' : 'default'}
    );
};

var sortOrder = new function() {
    
    this._highToLow = false;
    this._lastRowNumber;
    
    this.getOrder = function(rowNumber) {
        
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

function TartanRug(areaCount,isSubnationalDisplayed, isDownloadable, trendDisplay) {
    
    /* Some of the this variables are used by a mustache template */
    this.areaCount = areaCount;
    
    var displayedAreas
    ,minimumAreaCount = 12
    ,row = null
    ,borderWidth = 2*2;
    
    var indicatorWidth = 224 + borderWidth // width 220 + padding*2 2
    ,sortWidth = 34/*width*/ + borderWidth
    ,periodWidth = 58/*width*/ + borderWidth;
    
    // (th.pLink, th.rotate, th.comparatorHeader)
    this.valueCellWidth = 41/*width*/ + borderWidth;
    
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
    this.isNotNN = !FT.model.isNearestNeighbours();

    this.isScrollbar = null;

    this.trendDisplaySelected = trendDisplay;

    this.displayValuesOnly = (trendDisplay == TrendDisplayOption.ValuesOnly);
    this.displayTrendsOnly = (trendDisplay == TrendDisplayOption.TrendsOnly);
    this.displayValuesAndTrends = (trendDisplay == TrendDisplayOption.ValuesAndTrends);


    var benchmarkColumnCount = isSubnationalDisplayed ? 2 : 1;
    this.leftTableWidth = indicatorWidth + sortWidth + periodWidth + 
        (this.valueCellWidth * benchmarkColumnCount) - 2/*no right border*/;
    
    this.allAreasWidth = areaCount * this.valueCellWidth;
    
    this.init = function() {

        displayedAreas = this.getBestVisibleAreaCount(isDownloadable);
        this.viewport = (this.valueCellWidth * displayedAreas) + 2/*border*/;
        
        this.isScrollbar = areaCount !== displayedAreas;
        this.scrollIndicatorWidth = this.isScrollbar ? 
            ((this.viewport / this.allAreasWidth) * this.viewport):
            0; 
    }
    
    this.isMostAppropriateNumberOfAreasDisplayed = function() {
        return this.getBestVisibleAreaCount(isDownloadable) !== displayedAreas; 
    }
    
    this.getDisplayedAreas = function() {
        return displayedAreas;
    }
    
    this.getBestVisibleAreaCount = function(downlaodable) {

        var w;
        var extraSpaceForImageExport = 60;
        if (downlaodable) {
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
            name:this.getVerticalText(trimName(areaName, FT.model.areaTypeId == AreaTypeIds.Practice ? 22 : 28)), 
                code:area.Code
        });
    };
    
    this.addBenchmarkName = function(txt) {
        this.benchmarks.push([this.getVerticalText(txt)]);   
    };
    
    this.newDivider = function(heading) {
        this.rows.push({
                divider:{heading:heading,colSpan:this.areaCount + 
                        3/*indicator + period + sort*/}
        });
    };
    
    this.newRow = function(rootIndex) {
        row = {
            number:++this.rowNumber,
            rootIndex:rootIndex,
            benchmarkValues:[],
            values:[]
        };
        this.rows.push(row);   
    };
    
    this.setIndicator = function(name, dataQuality) {
        row.indicator = name;
        row.indicatorDataQuality = dataQuality;
    };
    
    this.setTimePeriod = function(period) {
        row.period = period;
    };
    
    this.addRowValue = function(txt) {
        row.values.push({html:txt});
    };
    
    this.addBenchmarkValue = function(txt) {
        row.benchmarkValues.push({html:txt});
    };

    this.getVerticalText = function (txt) {
        // v parameter included as cache buster
        return '<img class="verticalText" src="' + FT.url.bridge
            + 'img/vertical-text?v=2&text=' + encodeURIComponent(txt)
            + '" alt=""/>';
    };
};

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
};

function highlightCell(td) {
    $(td).css({
            'border-color': colours.border,
            'cursor' : 'default'}
    );
};

function rugScrollClick(direction) {
    
    if (!ajaxLock) {
        
        lock();
        
        var scrollContent = $('.scrollContent');
        scrollContent.stop(true,true);
        var scrollPlace = $('.scroll-place');
        scrollPlace.stop(true,true);
        
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
                
                var scrollWidth = $( ".scroll-pane" ).width();
                ml += (scrollWidth - (rug.valueCellWidth * 2)/*overlap 2 areas from previous page*/
                        - borderWidth) * direction;
                
                var contentWidth = rug.allAreasWidth;
                var maxMargin = contentWidth - scrollWidth + borderWidth;
                
                if ( ml >  maxMargin) { 
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
            
            scrollContent.animate({'margin-left': -ml }, 400);
            scrollPlace.animate({'margin-left': margin }, 400); 
        }
        
        unlock();

        logEvent('TartanRug', 'RugScrolled');
    }
}

tartanRugState = {
    rug : null
}

function TartanRugTooltipProvider() {
    
    this.getHtml = function(id) {
        if (id !== '') {

            var bits = id.split('-');

            // Indicator name, e.g. 'rug-indicator_1'
            if (id.indexOf('rug-indicator') > -1) {
                return getIndicatorNameTooltip(bits[2]/*root index*/);
            }

            // Value cell
            return this.getTartanText(id, bits[2]);
        }
        return '';
    };
    
    this._val = function(unit, valF) {
        return new ValueWithUnit(unit).getFullLabel(valF);
    };
    
    this.getTartanText = function(id, groupRootIndex) {

        // Get value note
        var $tartanCell = $('#' + id);
        var areaCode = $tartanCell.attr('areacode');
        if (!areaCode) {
            // Area code is not defined because no data is available
            return '';
        }

        var valueNoteId = $tartanCell.attr('vn');
        var root = groupRoots[groupRootIndex],
        data = getDataFromAreaCode(root.Data, areaCode),
        dataInfo = new CoreDataSetInfo(data),
        area = areaHash[areaCode],
        isLocalArea = isDefined(area),
        message = 'No data',
        metadata = ui.getMetadataHash()[root.IID];
        
        if (isLocalArea) {
            if (dataInfo.isValue()) {
                message = this._val(metadata.Unit, data.ValF);
            }        
            var areaName = area.Name;
            
        } else {  
            // Try get comparator data
            var grouping = areaCode === FT.model.parentCode ?
                getRegionalComparatorGrouping(root) :
                getNationalComparatorGrouping(root);
            
            data = grouping.ComparatorData;
            
            if (data && data.Val !== -1) {  
                message = this._val(metadata.Unit, data.ValF);
            }
            
            areaName = getComparatorFromAreaCode(areaCode).Name;       
        }
        
        //TODO template this
        return ['<span id="tooltipArea">', areaName,'</span>',
            '<span id="tooltipData">', message,'</span>',
            new ValueNoteTooltipProvider().getHtmlFromNoteId(valueNoteId),
            '<span id="tooltipIndicator">',metadata.Descriptive.NameLong,
            '</span>'].join('');
    };
}

tartanRugSizeState = {
    Normal: 0, // Normal - when tartan rug rendered first time
    Expand: 1, // Expand - removes the overflow left-table + right-table
    Resize: 2  // Resize - put the width back to normal without cell count check
}

TrendDisplayOption = {
    ValuesOnly: 0,
    TrendsOnly: 1,
    ValuesAndTrends: 2
}


function saveAsImage() {

    if (isIE8()) {
        browserUpgradeMessage();
    } else {

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
}

function styleTartanRugForExport() {
    // hide download image link (after tartan has been resized)
    $('.export-chart-box').hide();

    $('#trendDisplayOptions').hide();
   
    // hide scrollbars
    $('.scrollbar').css('visibility', 'hidden');

    // change the div background colour to white as required by serverside code.
    $('#overview-container').css('background', 'white');

    // change the font
    $('#rightTartanTable,#leftTartanBox').css('font-family', 'Arial');
}

function showExportAsImageLink() {
    return '<div class="export-chart-box"><a class="export-link" onclick="saveAsImage();">Export table as image</a></div><div id="tartanTableBox" class="centered">';
}

function GetTrendDisplayOptions() {
    return '<div id="trendDisplayOptions">' +
            '<span>Display:</span>' +
            '<div id="options">' +
                '<button id="valuesOnly" value="0" type="button" class="optionButton {{#displayValuesOnly}}buttonSelected{{/displayValuesOnly}}" onclick="setDisplayFor(this)">Values</button>' +
                '<button id="trendsOnly" value="1" type="button" class="optionButton {{#displayTrendsOnly}}buttonSelected{{/displayTrendsOnly}}" onclick="setDisplayFor(this)">Trends</button> ' +
                '<button id="valuesAndTrends" value="2" type="button" class="optionButton {{#displayValuesAndTrends}}buttonSelected{{/displayValuesAndTrends}}" onclick="setDisplayFor(this)">Values & Trends</button> ' +
            '</div>' +
        '</div>';
}

function setDisplayFor(selectedId) {
    goToTartanRugPage($(selectedId).val());
}

var scrollPlaceHtml =  '<div class="scrollbar" style="width:{{viewport}}px;">' +
    '<table style="{{^isScrollbar}}display:none;{{/isScrollbar}}">' + 
    '<tr><td colspan="2" style="width:100%;height:3px;background:#eee;"><div class="scroll-place" style="width:{{scrollIndicatorWidth}}px;"></div><td/></tr>' +
    '<tr><td class="rug-scroll-left" onclick="rugScrollClick(-1)"></td><td class="rug-scroll-right" onclick="rugScrollClick(1)"></td></tr>' + 
    '</table></div>';

templates.add('tartan',
    //Static (indicator/period/sort/benchmarks)    
    showExportAsImageLink() +
    '<div id="leftTartanBox">' +  
    '{{#hasTrendMarkers}}' + GetTrendDisplayOptions() + '{{/hasTrendMarkers}}' +
    '<table id="leftTartanTable" class="borderedTable" cellspacing="0"><thead><tr>' +
        '<th class="rugIndicator white-background"><div>Indicator</div></th>' +
        '<th class="rugPeriod white-background">Period</th>' +
        '<th class="sortHeader white-background"><a class="rowSort" title="Sort area names alphabetically" href="javascript:{{#isNotNN}}sortTartanRugAToZ(){{/isNotNN}}{{^isNotNN}}sortTartanRugByNearestNeighbours(){{/isNotNN}};">&nbsp;</a></th>' +
        '{{#benchmarks}}<th class="comparatorHeader">{{{0}}}</th>{{/benchmarks}}' + 
        '</tr></thead><tbody>{{#rows}}<tr>' +
        '{{#divider}}<td class="tartanDivider" colspan="5"><div class="dividerBox">&nbsp;<div class="dividerText">{{heading}}</div></div></td>{{/divider}}' +
        '{{^divider}}<td id="rug-indicator-{{rootIndex}}" class="rugIndicator pLink" onclick="indicatorNameClicked(\'{{rootIndex}}\');" onmouseover="highlightRow(this, false);" onmouseout="unhighlightRow(false);"><div>{{indicator}}{{{indicatorDataQuality}}}</div></td>' +
        '<td class="rugPeriod center">{{period}}</td>' +
        '<td class="sort"><a class="rowSort" title="Sort on {{indicator}}" href="javascript:sortTartanRug({{number}});">&nbsp;</a></td>' +
        '{{#benchmarkValues}}{{{html}}}{{/benchmarkValues}}' + 
        '{{/divider}}</tr>{{/rows}}</tbody></table></div>' + 
    //Scrollable (areas)
    '<div class="scroll-pane" id="scrollable-pane" style="width:{{viewport}}px;">' + 
        scrollPlaceHtml +
        '<div id="tartanScrollBox"><table id="rightTartanTable" class="scrollContent borderedTable" cellspacing="0"><thead><tr>' +
        '{{#areas}}<th class="pLink white-background" {{#isNotNN}} onclick="goToAreaProfilePage(\'{{code}}\');" {{/isNotNN}} onmouseover="highlightCell(this);" onmouseout="unhighlightCell(this);" >{{{name}}}</th>{{/areas}}' +
        '<th class="rugIndicator white-background"><div>Indicator</div></th>' +
        '<th class="rugPeriod white-background">Period</th>' +
        '<th class="sortHeader white-background"><div class="rowSort"></div></th>' +
        '</tr></thead><tbody>{{#rows}}<tr>' +
        '{{#divider}}<td class="tartanDivider" colspan="{{colSpan}}">&nbsp;</td>{{/divider}}' +
        '{{^divider}}{{#values}}{{{html}}}{{/values}}' + 
        '<td class="rugIndicator pLink"><div>{{indicator}}{{{indicatorDataQuality}}}</div></td>' +
        '<td class="rugPeriod center">{{period}}</td>' +
        '<td class="sort"><div class="rowSort"></div></td>' +
        '{{/divider}}</tr>{{/rows}}</tbody></table>' + scrollPlaceHtml + '</div></div></div>'
);

pages.add(PAGE_MODES.TARTAN, {
    id: 'overview',
    title: 'Overview',
    goto: goToTartanRugPage,
    gotoName: 'goToTartanRugPage',
    needsContainer: true,
    jqIds: [
        'keyTartanRug', 'yearSelection', '.geo-menu', 'tartan-key-part', 'value-note-legend', 'nearest-neighbour-link', 'trend-marker-legend-tartan'],
    jqIdsNotInitiallyShown: ['data-quality-key', 'target-benchmark-box', 'key-spine-chart'],
    resize:tartanRugResize
});

