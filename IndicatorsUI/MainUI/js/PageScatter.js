function goToScatterPage() {
    lock();
    
    setPageMode(PAGE_MODES.SCATTER);
    
    ajaxMonitor.setCalls(5);
    
    ui.callbackIds['areaData'] = null;
    
    getPracticeAndParentLists();
    
    // Check for first menus
    var sid = PP.model.groupId;
    getIndicatorMetadata(sid);
    getNationalData(sid); 
    
    ajaxMonitor.monitor(getScatter2Data);
};

function displayScatterChart() {
    
    ui.callbackIds['metadata'] = null;
    ui.callbackIds['areaData'] = null;
    
    evaluateIndicator1Menu();
    evaluateSubgroup2Menu();
    evaluateIndicator2Menu();
    
    var model = PP.model,
    src = [FT.url.bridge, 'img/gp-scatter-chart?width=900&height=500&pyr=',
        model.year, '&off=', getYearOffset()];
    
    var isPractice = isDefined(model.practiceCode);
    if (isPractice) {
        src.push("&are=", model.practiceCode);
    }
    updatePracticeScatterLabel(isPractice);
    
    var isParent = isDefined(model.parentCode);
    if (isParent) {
        src.push("&par=", model.parentCode);
    }
    updatePctScatterLabel(isParent);
    
    var i = ui.getSelectedRootIndex();
    var root = ui.getData(NATIONAL_CODE)[i];
    
    src.push("&gid1=", model.groupId, 
        "&iid1=", root.IID, 
        "&sex1=", root.SexId,
        '&age1=', root.AgeId);
    
    var index2 = $('#indicator2Menu').val();
    var sid2 = model.groupId2;
    var root2 = loaded.data[sid2][NATIONAL_CODE][index2];
    src.push("&gid2=", sid2,
        "&iid2=", root2.IID,
        "&sex2=", root2.SexId,
        '&age2=', root2.AgeId);
    
    // TODO only change src if necessary
    setScatterSrc(src.join(''));
    
    showAndHidePageElements();
    
    unlock();
};

function group2Changed(gid) {
    
    /* User selected new option in menu */
    
    lock();
    
    PP.model.groupId2 = gid;
    ftHistory.setHistory();
    
    setScatterSrc('');
    getScatter2Data();
};

function indicator2Changed(index) {
    
    /* User selected new option in menu */
    
    lock();
    
    scatterState.setPreferredGroupRoot(index);
    ftHistory.setHistory();
    
    setScatterSrc('');
    
    displayScatterChart();
};

function evaluateIndicator2Menu() {
    
    var roots = loaded.data[PP.model.groupId2][NATIONAL_CODE];
    var jq = $('#indicator2Menu');
    
    if (scatterState.doIndicatorRepopulate()) { 
        
        scatterState.setIndicatorMenuSubgroupId();
        
        var metadata = loaded.indicatorMetadata[PP.model.groupId2];
        populateIndicatorMenu(jq, metadata, roots);
    }

    // Not ideal that indicator index set every time, should be only on hash restore
    var i = getIndicator2Index(roots);
    jq.val(i);
};

scatterState = {
    
    /* If second indicator menu required by another page then move preferred group root 
    information to ui object */
    
    indicatorMenuSubgroupId : null,
    rootIndexes:{},
    isSubgroup2MenuPopulated:false,
    parent:'',
    practice:'',
    indicator2DefaultIndex:1,
    
    doParentUpdate : function(code) {
        return !isDefined(PP.model.parentCode) || code !== this.parent;
    },
    
    doPracticeUpdate : function(code) {
        return !isDefined(PP.model.practiceCode) || code !== this.practice;
    },
    
    setParentCode: function(code) {
        this.parent = code;   
    },
    
    setPracticeCode: function(code) {
        this.practice = code;   
    },
    
    setPreferredGroupRoot : function(index) {
        var sid = PP.model.groupId2;
        this.rootIndexes[sid] = index;
    },
    
    getPreferredGroupRoot : function() {
        var sid = PP.model.groupId2;
        return ui._getValidRootIndex(this.rootIndexes[sid]);
    },
    
    doIndicatorRepopulate : function() {
        return this.indicatorMenuSubgroupId === null ||
        this.indicatorMenuSubgroupId != PP.model.groupId2;
    },
    
    setIndicatorMenuSubgroupId : function() {
        this.indicatorMenuSubgroupId = PP.model.groupId2;
    }
};

function evaluateSubgroup2Menu() {
    
    if (!scatterState.isSubgroup2MenuPopulated) {
        populateSubgroupMenu(loaded.subgroups, 'group2Menu');
        scatterState.isSubgroup2MenuPopulated = true;
    }
    
    // Must select if restoring hash state
    $('#group2Menu').val(PP.model.groupId2);
};

function setScatterSrc(src) {

    $('#scatter').html('<img src="' + src + '" alt="" />');
};

function updatePctScatterLabel(isParent) {
    
    if (scatterState.doParentUpdate(PP.model.parentCode)) {
        if(isParent) {
            var label = getPracticeParentName();
        } else {
            label = getPracticeParentLabel();
        }
        
        setScatterKeyImg('scatterPct', 'pct', label);
        scatterState.setParentCode(PP.model.parentCode);
    }
};

function updatePracticeScatterLabel(isPractice) {
    
    var code = PP.model.practiceCode;
    
    if (scatterState.doPracticeUpdate(code)) {
        if(isPractice) {
            var label = getPracticeLabel();
        } else {
            label = 'PRACTICE';
        }
        
        setScatterKeyImg('scatterPractice', 'practice', label);
        scatterState.setPracticeCode();
    }
};

function getScatter2Data() {
    
    // Set groupId2 if not yet defined
    var sid = PP.model.groupId2;
    if (!isDefined(sid)) {
        sid=PP.model.groupId;
        PP.model.groupId2 = sid;
    }
    
    ajaxMonitor.setCalls(2);
    
    ui.callbackIds['areaData'] = sid;
    
    getIndicatorMetadata(sid);
    getNationalData(sid);
    
    ajaxMonitor.monitor(displayScatterChart);
};

function setScatterKeyImg(id, item, label) {
    $('#' + id).html('<img src="' + FT.url.img + 'scatter-' + item + '.png" />' + label);
};

