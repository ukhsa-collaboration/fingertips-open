function goToIndicatorsPage() {
    lock();
    
    setPageMode(PAGE_MODES.SPINE);
    
    getSpineChartData(PP.model.year, getYearOffset());
};

function displaySpineCharts() {

    var model = PP.model;
    var sid = model.groupId,
    subgroupData = loaded.data[sid],
    formatItems = null,
    nationalSubgroupData = subgroupData[NATIONAL_CODE],
    metadata = loaded.indicatorMetadata[sid];
    
    if (!spineChartState.isSubgroupDisplayed(sid)) {
        
        var provider = new PPTooltipProvider(stems);
        tooltipManager.setTooltipProvider(provider);
        spineChartState.tooltipProvider = provider;
        
        spineChartState.hideDisplayedTable();
        
        if (!spineChartState.doesSubgroupTableExist(sid)) {
            
            // Is required data available
            if (!isRequiredDataAvailable(sid)) {
                // Get previous year's data
                getSpineChartData(PP.model.year - 1, getYearOffset() + 1);
                return;
            }
            
            // Create table
            if (isDefined(subgroupData)) {
                formatItems = createSpineChartTable(sid, nationalSubgroupData,metadata);     
            }  
            
        } else {
            // Display existing table
            spineChartState.showTable(sid);
        }
        
        spineChartState.setDisplayedSubgroupId(sid);
    }
    
    updatePracticeLabel();
    
    var isPractice = model.isPractice(),
    benchmarkCode = getBenchmarkCode(),
    isBenchmark = benchmarkCode !== null,
    bid = getBenchmarkId();
    
    var updateBenchmark = !spineChartState.isBenchmarkDisplayed(sid, benchmarkCode) ||
        bid == BENCHMARKS.CCG; 
    if (updateBenchmark) {
        spineChartState.setDisplayedBenchmark(sid, benchmarkCode);    
        var benchmarkImage = null;
        if (bid == BENCHMARKS.CCG) {
            if (isPractice) {
                // White if practice is also displayed
                benchmarkImage = getParentMarkerImage(0);
            }
        } else if (isPractice) {
            // Shape or deprivation decile
            benchmarkImage = getBenchmarkImage();
        }       
    }
    
    // Get spine chart rows
    var tableId = spineChartState.getTableId(sid);
    var $rows = $('#' + tableId + ' tbody tr');
    
    var practiceSubgroupData = subgroupData[model.practiceCode];
    var benchmarkSubgroupData = subgroupData[benchmarkCode];
    
    var yearOffset = getYearOffset();
    
    for(var rootIndex in nationalSubgroupData) { 
        
        var nationalItem = nationalSubgroupData[rootIndex];
        var iid = nationalItem.IID;
        var indicatorMetadata = metadata[iid];
        var $row = $rows.filter(':eq(' + rootIndex + ')');

        // Practice
        getAreaInfo(isPractice, 3/*column index*/, indicatorMetadata,
            practiceSubgroupData, $row, rootIndex, yearOffset, iid,
            nationalItem, tableId, true, null);
        
        // Comparator
        if (updateBenchmark) {
            getAreaInfo(isBenchmark, 4/*column index*/, indicatorMetadata,
                benchmarkSubgroupData, $row, rootIndex, yearOffset, iid,
                nationalItem, tableId, false, benchmarkImage);
        }
    }
    
    showAndHidePageElements();
    unlock();
    
    // Spine chart tooltips
    if (formatItems !== null) {
        for(rootIndex in nationalSubgroupData) {
            var item = formatItems[rootIndex];
            if (item !== null)    {
                addTableSpineChartTooltip(rootIndex + '_' + tableId, 
                    new IndicatorFormatter(formatItems[rootIndex]));
            }
        }
    }
    
    changeManager.initChangeTooltips();
    
    // Init marker IDs
    var ids = spineChartState.markerIds;
    for (var i in ids) {
        tooltipManager.initElement(ids[i]);
    }
    spineChartState.markerIds = [];
};

function addSpineChartHeader(h) {
    
    h.push('<thead><tr>');
    
    addTh(h, 'Indicator');
    addTh(h, 'Period', 'period');
    addTh(h, 'Practice<br/>Count', CSS_VAL);
    addTh(h, 'Practice<br/>Value', CSS_VAL);
    addTh(h, getBenchmarkHeader(), CSS_VAL + ' benchmark');
    addTh(h, 'England<br>Value', CSS_VAL);
    addTh(h, 'England<br>Lowest', CSS_VAL);
    addTh(h, '<div class="fl w100" style="position:relative;height:20px;">' + 
'<div id="info-spinechart" class="infoTooltip" onclick="showSpineInfo()" title="More information about these charts"></div></div><div style="float:left;width:100%;clear:both;">England Range</div>', 'range');
    addTh(h, 'England<br>Highest', CSS_VAL);

    h.push('<tr></thead>');
};

function getTrendTriangle(item, indicatorId, dataIndex, practiceValue, practiceValF) {
    
    if (PP.model.year > earliestYear && dataIndex > 0) {
        
        var previousIndex = dataIndex - 1;
        
        var previousYearData = item.Data[previousIndex];
        
        if (isDefined(previousYearData)) {
            var previousYearValue = previousYearData.Val; 
            
            if (previousYearValue != null) {

                var image = getTrendTriangleImage(practiceValue, previousYearValue);      
                var imgId = changeManager.newImage(indicatorId, previousYearData.ValF);
                
                return '<img id="' + imgId + '" class="change" alt="" src="' + 
                    FT.url.img + image + '">' + practiceValF;
            }
        }
    }
    return practiceValF;
};

function getTrendTriangleImage(currentValue, previousValue) {

    if (currentValue > previousValue) {
        var image = 'up';
    } else if (currentValue < previousValue) {
        image = 'down';
    } else {
        // The rare occurrence where the significance has changed
        // between years but the values are the same
        image = 'same';
    }

    return 'change-' + image + '.png';
}

function addIndicatorName(h, indicatorMetadata, root, rootIndex) {
    
    if (isDefined(indicatorMetadata)) {
        var name = '<a href="javascript:selectRoot(' + rootIndex + ')">' + 
            indicatorMetadata.Descriptive.Name + new SexAndAge().getLabel(root) +
        '</a>';
    } else {
        name = '';
    }
    addTd(h, name, 'rowTitle');
};

function getAreaData(groupId, areaCode, includeTimePeriods) {

    if (isDefined(areaCode)) {
        var data = loaded.data;
        if (!data.hasOwnProperty(groupId) ||
            !data[groupId].hasOwnProperty(areaCode)) {
            
            var comparators = [];
            if (areaCode !== NATIONAL_CODE) {
                comparators.push(NATIONAL_CODE);

                var parentCode = PP.model.parentCode;
                if (parentCode !== null && 
                    parentCode !== areaCode) {
                    comparators.push(parentCode);
                }
            }
            
            var a = ['gid=', groupId,
                '&are=', areaCode,
                '&ati=', PRACTICE,
                '&com=', comparators.join(',')];
            
            if (isDefined(includeTimePeriods) && includeTimePeriods) {
                a.push('&tim=yes');
            }
            
            getData(getAreaDataCallback, 'ad', a.join(''));
            
            return;
        }
    }
    
    ajaxMonitor.callCompleted();  
};

function getAreaDataCallback(obj) {
    
    var key = 'areaData',
    callData = ui.callbackIds;
    
    // Get Subgroup ID the callback data refers to
    var sid = callData[key];
    if (!isDefined(sid)) {
        sid = PP.model.groupId;
    }
     
    if (isDefined(obj)) {
        
        checkHash(loaded.data,sid);
        
        for (var areaCode in obj) {
            loaded.data[sid][areaCode] = obj[areaCode];
        }
    }
    
    // Reset call data
    callData[key] = null;
    
    ajaxMonitor.callCompleted();
};

function checkHash(hash,key) {
    if (!hash.hasOwnProperty(key)) {
        hash[key] = {};
    }
};

function getBenchmarkData() {
    
    var model = PP.model,
    callOnFinish = displaySpineCharts;
    
    if (model.isPractice()) {
        
        var population = currentPracticePopulation;
        
        var code = getBenchmarkId() === BENCHMARKS.PEER_GROUP ?
            getPeerGroupCode(population.Shape) :
            getDeprivationCode(population.GpDeprivationDecile);
        
        if (code !== null) {
            var gid = model.groupId;
            var data = loaded.data
            if (!data.hasOwnProperty(gid) ||
                !data[gid].hasOwnProperty(code)) {
                
                ajaxMonitor.setCalls(1);
                
                getData(getAreaDataCallback, 'ad', 'gid=' + gid +
                        '&ati=' + PRACTICE +
                        '&are=' + code +
                        '&com=' + NATIONAL_CODE +
                        '&pyr=' + ajaxMonitor.state.year
                );
                
                ajaxMonitor.monitor(callOnFinish);
                return;
            }
        }
    }
    callOnFinish();
};

function benchmarkChanged() {
    
    setBenchmarkImage();
    updateBenchmarkHeaders();
    
    goToIndicatorsPage();
};

function getBenchmarkImage() {
    
    var id = getBenchmarkId();
    
    switch(id) {
        
        case 0:
            // CCG
            return getParentMarkerImage(0);
            
        case 1:
            // Deprivation
            return 'square_white_18.png';
            
        case 2:
            // Peer group
            return 'diamond_white_18.png';
    }
    
    return '';
};

function setBenchmarkImage() {
    $('#benchmarkImg').attr('src', FT.url.img + getBenchmarkImage());
};

spineChartState = {
    
    displayedSubgroupId : null,
    
    _displayedPractice : '-',  
    
    displayedBenchmarks : {},
    
    createdSubgroups : [],
    
    tooltipProvider : null,
    
    // Marker Ids that have not had tooltip events registered
    markerIds : [],
    
    // key is marker Id, value is ValF
    markerValues : {},
    
    refreshPracticeLabel : function() {
        return PP.model.practiceCode !== this._displayPractice;
    },
    
    setDisplayedPractice : function() {
        this._displayPractice = PP.model.practiceCode;
    },
    
    isSubgroupDisplayed : function(sid) {
        return sid !== null && 
            this.getId(sid) === this.displayedSubgroupId;	
    },
    
    doesSubgroupTableExist : function(sid) {
        return $.inArray(this.getId(sid), this.createdSubgroups) > -1;
    },
    
    getTableId : function(sid) {
        return 'st' + this.getId(sid);
    },
    
    getJq : function(sid) {
        return $('#' + this.getTableId(sid));  
    },
    
    hideDisplayedTable : function() {
        var id = this.displayedSubgroupId; 
        if (id !== null) {
            $('#st' + id).hide(); 
            this.displayedSubgroupId = null;
        }
    },
    
    setDisplayedSubgroupId : function(sid) {
        this.displayedSubgroupId = this.getId(sid);
    },
    
    addSubgroup : function(sid) {
        this.createdSubgroups.push(this.getId(sid));
    },
    
    showTable : function(sid) {
        this.getJq(sid).show();
    },
    
    getId : function(sid) {
        if (sid !== null) {
            return getYearOffset().toString() + sid.toString();
        }
        return '';
    },
    
    isBenchmarkDisplayed : function(sid, code) {
        var displayedCode = this.displayedBenchmarks[this.getId(sid)];
        if (isDefined(displayedCode)) {
            return displayedCode === code;
        } 
        return false;
    },
    
    setDisplayedBenchmark : function(sid, code) {
        var id = this.getId(sid);
        this.displayedBenchmarks[id] = code;
    }
};

function createSpineChartTable(sid, nationalSubgroupData,metadata) {
    
    var year = PP.model.year;
    var statsForCurrentYear = loaded.indicatorStats[sid][year];
    var offset = getYearOffset();
    
    var formatItems = [];
    
    var tableId = spineChartState.getTableId(sid);
    
    var h = [];
    h.push('<table id="', tableId,'" class="spineTable borderedTable" cellspacing="0">');
    addSpineChartHeader(h);
    h.push('<tbody>');
    
    for(var rootIndex in nationalSubgroupData) { 
        
        var stats = statsForCurrentYear[rootIndex];
        var isNationalData = isDefined(stats.Stats);
        
        var nationalRoot = nationalSubgroupData[rootIndex];
        var indicatorMetadata = metadata[nationalRoot.IID];
        var nationalDataArray =  nationalRoot.Data;
        
        var timePeriod = new TimePeriod(nationalRoot.Periods, nationalDataArray.length, offset);
        
        if (isNationalData) {
            var data = nationalDataArray[timePeriod.yearIndex];
        } else if (offset === 0) {
            stats = loaded.indicatorStats[sid][year - 1][rootIndex];
            data = nationalDataArray[timePeriod.yearIndex];
        } else {
            data = null;
        }
        showSpine = isDefined(data) && isDefined(stats.Stats);
        
        h.push('<tr>');
        
        var unit = getUnitLabel(indicatorMetadata);
        addIndicatorName(h, indicatorMetadata, nationalRoot, rootIndex);
        
        addTd(h, timePeriod.label, CSS_CENTER);
        
        // Placeholders for practice and benchmark data
        addTd(h, '', CSS_NUMERIC);
        addTd(h, '', CSS_NUMERIC);
        addTd(h, '', CSS_NUMERIC);
        
        // National
        var nationalValF = getValF(data, unit);
        label = nationalValF;            
        addTd(h, label, CSS_NUMERIC);
        
        if (showSpine) 
        {
            var indicatorStats = stats.StatsF;
            var min = indicatorStats.Min + unit;
            var max = indicatorStats.Max + unit;
        } else {
            indicatorStats = null;
            min = NO_DATA;
            max = NO_DATA;
        }
        addTd(h, min, CSS_NUMERIC);
        
        if (showSpine) {
            // Show spine
            formatItems.push({
                    'metadata':indicatorMetadata, 
                    'stats':indicatorStats,
                    'average':nationalValF
            });
            
            var key = 'proportions';
            // Check if spine chart proportions are already calculated
            if (!data.hasOwnProperty(key)) {           
                data[key] = getSpineProportions(
                    data.Val, 
                    stats.Stats, 
                    1/*polarity*/);
                
                data.dimensions = spineChart.getDimensions(data[key]);
            }
            
            var html = spineChart.getHtml(data.dimensions, rootIndex + '_' + tableId, '');
            
        } else {
            formatItems.push(null);
            html = '<div class="noSpine">-</div>';   
        }
        
        addTd(h, html);
        addTd(h, max, CSS_NUMERIC);
        
        h.push('</tr>');
    }
    
    h.push('</tbody></table>');
    
    $('#spineTableBox').append(h.join(''));
    spineChartState.addSubgroup(sid);
    
    return formatItems;
};

function addTableSpineChartTooltip(spineChartElementIdStem, formatter) {
    
    spineChartState.tooltipProvider.add(spineChartElementIdStem, formatter);
    var stemKeys = stems.getKeys();
    for (var i in stemKeys) {
        tooltipManager.initElement(
            stemKeys[i] + '_' + spineChartElementIdStem);
    }
};

function getAreaInfo(isArea, cellIndex, indicatorMetadata, subgroupData, row, rootIndex, yearOffset, 
    iid,  nationalItem, tableId, isPractice, img) {
    
    var val = null,  valHtml = NO_DATA;
    
    // Update value cell
    if (isArea) {      
        var item = subgroupData[rootIndex],
        dataArray = item.Data,
        dataIndex = dataArray.length - yearOffset - 1,
        data = dataArray[dataIndex]; 
        
        var dataInfo = new CoreDataSetInfo(data);

        // Practice count
        if (isPractice) {
            var count = formatCount(dataInfo);
            row.children(':eq(2)').html(count);
        }

        if (dataInfo.isValue()) {          
            var unit = getUnitLabel(indicatorMetadata),
            valF =  getValF(data, unit);
            
            val = data.Val;
            valHtml = valF;
            
            if (isPractice && item.Sig) {
                var significances = item.Sig[NATIONAL_CODE],
                sig = significances[dataIndex];
                if (significances.length) {
                    var sigPreviousYear = significances[dataIndex - 1];
                    if (sig !== sigPreviousYear) {
                        // Only show change if significance different to last year
                        valHtml = getTrendTriangle(item, iid, dataIndex, val, valF);
                    }
                }
                
                // Standardise significant difference
                sig = sig == 3 ? 1 : sig; 
            } else {
                sig = 0;
            }
        }
    }

    // Update value cell
    row.children(':eq(' + cellIndex + ')').html(valHtml);
    
    // Update spine chart marker
    var markerId = getMarkerId(rootIndex, tableId, isPractice ? 'p' : 'b'),
    $marker = $('#' + markerId);
    
    if (val === null) {
        valF = null;
        if ($marker.length) {
            // Marker exists
            $marker.hide();
        }
    } else {
        
        if (nationalItem.Data[dataIndex] &&
                nationalItem.Data[dataIndex].dimensions) {
            
            var left = spineChart.getMarkerOffset(val, nationalItem.Data[dataIndex].dimensions, 
                nationalItem.Data[dataIndex].proportions, 0/*polarity*/) + nationalItem.Data[dataIndex].dimensions.q1Offset;
            
            if (img === null) {
                img = isPractice ?
                    getPracticeMarkerImage(sig):
                    getParentMarkerImage(sig); 
            }
            
            if ($marker.length) {
                // Marker exists
                $marker.attr('src', FT.url.img + img).css('left', left).show();
            } else {
                // Create marker
                row.children(':eq(7)').children(':eq(0)').append(
                    getMarkerHtml(markerId, left, img, 2/*top*/, 
                        isPractice ? '' : ' onTop'
                ));
                spineChartState.markerIds.push(markerId);
            }
        }
        
        spineChartState.markerValues[markerId] = valF;
    }
};

changeManager = {
    
    changeValues : [],
    changeIds : [],
    changeIndex : 0,
    
    getValF : function(changeIndex) {
        return this.changeValues[changeIndex];
    },
    
    newImage : function(indicatorId, valF) {
        var id = 'ch_' + this.changeIndex + '_' + indicatorId;
        this.changeValues.push(valF);
        this.changeIds.push(id);
        this.changeIndex++;
        return id;
    },
    
    initChangeTooltips : function() {
        for(var i in this.changeIds) {
            tooltipManager.initElement(this.changeIds[i]);
        } 
        this.changeIds = [];
    },
    
    // The annual change for a practice
    getChangeText : function(id, bits) {
        // e.g. id "ch_12_639"
        
        var sid = PP.model.groupId,
        changeIndex = bits[1],
        iid = bits[2],
        metadata = loaded.indicatorMetadata[sid][iid];
        
        // Annual change in value
        return ['<i>Significantly different to previous year (', 
            this.changeValues[changeIndex], getUnitLabel(metadata),
            ')</i><br><b>', metadata.Descriptive.Name, '</b>'].join('');
    }
};

function getMarkerHtml(id,left,img, top, extraClass) {
    
    return ['<img id="', id, '" src="', FT.url.img, img,
        '" class="marker', extraClass,'" style="left:', left,
        'px;top:', top, 'px" alt=""/>'].join('');
};

function getMarkerId(rootIndex, tableId, markerType) {
    return ['m', rootIndex, tableId, markerType].join('_')
};

function getBenchmarkId() {
	return parseInt($('#benchmarkMenu').val(),10);
};

function getBenchmarkCode() {
    
    var id = getBenchmarkId(),
    model = PP.model,
    population = currentPracticePopulation,
    practiceCode = model.practiceCode;
    
    switch(id) {
        
        case BENCHMARKS.CCG:
            return model.parentCode;
            
        case BENCHMARKS.DEPRIVATION:
            if (practiceCode !== null) {
                return getDeprivationCode(
                    population.GpDeprivationDecile);
            }
            break;
            
        case BENCHMARKS.PEER_GROUP:
            if (practiceCode !== null) {
                return getPeerGroupCode(
                    population.Shape);
            }
            break;
    }  
    
    return null;
};

function updateBenchmarkHeaders() {

    $('TH.benchmark').html(getBenchmarkHeader());
};

function getNationalData(sid) {
    getAreaData(sid, NATIONAL_CODE, true);
};

function getPracticeMarkerImage(sig) {
    if (sig == 1) {
        return 'circle_darkblue.png'; 
    }
    if (sig == 0) {
        return 'circle_white.png'; 
    }
    return 'circle_orange.png';
};

function updatePracticeLabel() {
    if (spineChartState.refreshPracticeLabel()) {
        spineChartState.setDisplayedPractice();
    }
};

function showSpineInfo() {
    getHelpText('spine-chart', 300, 370, 410);
};

function showBenchmarkInfo() {
    
    var h = "<h1>Benchmarks</h1>" + 
            "<p></p>";
    
    lightbox.show(h, 300, 360, 300);
};

function isRequiredDataAvailable(sid) {
    
    var year = PP.model.year;
    var statsForCurrentYear = loaded.indicatorStats[sid][year];
    var offset = getYearOffset();
    
    // Is latest year?
    if (offset == 0) {
        
        // Are missing stats for any indicator?
        if (!isDefined(statsForCurrentYear.isAllData)) {
            var isAllData = true;
            for (var rootIndex in statsForCurrentYear) {
                if (!isDefined(statsForCurrentYear[rootIndex].Stats)) {
                    isAllData = false;
                    break;
                }
            }
            statsForCurrentYear.isAllData = isAllData;
        }
        
        if (!statsForCurrentYear.isAllData) {
            // Is previous year data not loaded
            if (!isDefined(loaded.indicatorStats[sid][year -1])) {
                return false;
            }
        }
    }
    
    
    return true;
};

function getSpineChartData(year, offset) {

    var groupId = PP.model.groupId;

    ajaxMonitor.setCalls(9);
    ajaxMonitor.setState({year:year, offset:offset});
    
    getPracticeAndParentLists();
    
    getIndicatorMetadata(groupId);
    getIndicatorStats(groupId, year, offset);
    
    getLabelSeries(LABELS_DEPRIVATION);
    
    if (getBenchmarkId() == BENCHMARKS.CCG) {
        var callOnFinish = displaySpineCharts;
        getAreaData(groupId, PP.model.parentCode, false);
    } else {
        // Must find out deprivation decile
        // before benchmark data can be retrieved
        getPracticePopulation(year, offset);
        callOnFinish = getBenchmarkData;
    }
    
    getAreaData(groupId, PP.model.practiceCode);
    getNationalData(groupId);   
    
    ajaxMonitor.monitor(callOnFinish);
};

function getBenchmarkHeader() {
    var id = getBenchmarkId();
    
    switch(id) {
        
        case BENCHMARKS.CCG:
            var txt = getPracticeParentLabel();
            break;
            
        case BENCHMARKS.DEPRIVATION:
            txt = "Decile";
            break;
            
        case BENCHMARKS.PEER_GROUP:
            txt = 'Peer';
            break;
    } 
    
    return txt + '<br>Value';
}

function getParentMarkerImage(sig) {
    
    var shape = 'triangle-down-';
    
    var colour;
    switch(sig) {    
        case 0:
            colour = 'white';    
            break;
        case 2:
            colour = 'orange';   
            break;
        default:
            colour = 'darkblue';          
    }
    
    
    return 'markers/' + shape + colour + '.png'; 
}

PPTooltipProvider = function(stems) {
    
    map = {};
    
    this.add = function (key, formatter) {
        map[key.toString()] = formatter;
    };
    
    this.getHtml = function (id) {
        if (id !== '') {
            var bits = id.split('_');
            
            return bits[0] === 'ch' ?
                changeManager.getChangeText(id, bits) :
                this.getChartText(id, bits);
        }
        return '';
        
    }
    
    this.getChartText = function (id, bits) {
        
        // e.g. bits = 'q1_3_tableId' or m_2_tableId_b
        var stem = bits[0],
        rowNumber = bits[1],
        isMarker = stem === 'm';
        
        try {
            var formatter = map[rowNumber + '_' + bits[2]];
        } catch (e) {}
        
        var html = [];
        if (isDefined(formatter)) {
            html.push('<span id="tooltipData">');
            
            html.push(isMarker ?
                    spineChartState.markerValues[id] :
                    stems.getStemText(stem, formatter)
            );
            
            html.push(formatter.getSuffixIfNoShort(),
                '</span>');
            
            if (!isMarker) {
                html.push(stems.getStemQualifier(stem));
            }
            
            html.push('<span id="tooltipArea">');
            
            // Area name
            if (isMarker) {
                if (bits[3] == 'p') {
                    var areaName = getPracticeLabel();
                } else {
                    var bid = getBenchmarkId();
                    if (bid == BENCHMARKS.DEPRIVATION) {
                        var areaCode = currentPracticePopulation.GpDeprivationDecile;
                        areaName = labels[LABELS_DEPRIVATION][areaCode];
                    } else if (bid == BENCHMARKS.CCG) {
                        areaName = getPracticeParentName();    
                    } else {
                        areaName = $('#benchmarkMenu option:selected').text();
                    }
                }
            }
            else {
                areaName = 'England';
            }
            
            html.push(areaName, '</span><span id="tooltipIndicator">', 
                formatter.getIndicatorName(), '</span>');
        }
        return html.join(''); 
    }
}

