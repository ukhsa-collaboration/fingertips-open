function goToClusterPage() {
    
    lock();
    
    setPageMode(PAGE_MODES.CLUSTER);
  
    ajaxMonitor.setCalls(4);
    
    getPracticeAndParentLists();
    getClusters();
    
    ajaxMonitor.monitor(displayCluster);
}

function displayCluster() {
    
    if (!clusterState.isInitialised) {
        initCluster();
    }
    
    // Set general page HTML
    $('#clusterBox').html(templates.render(
            clusterState.getTemplate()));
    
    cluster.setUpClusterIcon();
    
    // Set elements of page
    displayClusterList();
    
    
    displayTabsByClusterMode();
    
    showAndHidePageElements();
    
    unlock();
}

function Cluster() {
    
    this.areas = [];
    
    this.clear = function() {
        this.areas = [];
    },
    
    this.getName = function() {
        return $('#clusterName').val();
    },
    
    this.getId = function() {
        // ID not used yet
        return '';
    },
    
    this.icon = null;
    
    this.iconOver = false;
    
    this.togglePractice = function(code) {
        
        var area = loaded.addresses[code];
        
        var reject = _.reject(this.areas, function(a) {return a.Code === code});
        
        if (reject.length === this.areas.length) {
            area.Address = getAddressText(area);
            this.areas.push(area);
        } else {
            this.areas = reject;   
        }
        
        this.refreshClusterIcon();
    }
    
    this.setUpClusterIcon = function() {
        
        if (this.icon === null) { 
            this.icon = $('#mainClusterIcon');
        }
        
        if (PP.model.isPractice() && clusterState.isEditMode) {
            this.icon.show();
        } else {
            this.icon.hide();   
        }
        
        this.refreshClusterIcon();
    }
    
    this.refreshClusterIcon = function() {
        var clas = 'clusterRemove';
        if (_.any(this.areas, function(a) {return a.Code === PP.model.practiceCode})) {
            // Practice is already in cluster
            this.icon.addClass(clas);
            var label = this.iconOver ?
                'remove practice from cluster' :
                'practice in cluster';
        } else {
            // Practice is not in cluster
            this.icon.removeClass(clas);
            label = 'add practice to cluster';
        }
        this.icon.html(label);
    }
}

function toggleClusterArea(areaCode) {
    
    if (areaCode === null) {
        areaCode = PP.model.practiceCode;
    }
    
    cluster.togglePractice(areaCode); 
    displayClusterList();
}

clusterState = {
    
    // Modes 
    EDIT : 0,
    NEW : 1,
    LOGIN_REQUIRED : 2,
    USER_CLUSTERS : 3,
    
    isInitialised : false,
    isEditMode : false,
    clusterList : null,
    editOn : function() {
        this.isEditMode = true;
    },
    
    editOff : function() {
        this.isEditMode = false;
    },
    
    getTemplate : function() {
        
        switch (this.mode) {
            case this.NEW:
            case this.EDIT:
                return 'editCluster';
            case this.USER_CLUSTERS:
                return 'userClusters';
            default:
                return 'loginRequired';
        }
    }
}

function displayClusterList() {
    
    if (clusterState.mode === clusterState.USER_CLUSTERS) {
        
        var clusters = clusterState.clusterList;
        for(var i in clusters) {
            clusters[i].count = clusters[i].AreaCodes.length;   
        }
        
        $('#clusterBox').html(templates.render('userClusters',
                {clusters:clusters}));
    }
    
    if (clusterState.mode === clusterState.EDIT) {
        
        var areas = cluster.areas;
        $('#clusterList').html(templates.render('practiceList',
                {areas:areas.length?areas:null}));
    }
    
    
}

function displayTabsByClusterMode() {
    
    var jqs1 = $('#downloadPdf,#exportExcel,#tabMetadata,#tabScatter,#tabIndicators,#tabBar,#tabTrends,#yearBox');
    if (clusterState.isEditMode) {
        // Turn off
        jqs1.hide();  
    } else {           
        // Turn on
        jqs1.show();
    }
}

function cancelClusterEdit() {

    cluster.clear();
    setClusterMode(clusterState.USER_CLUSTERS);
}

function initCluster() {
    
    cluster = new Cluster();
    clusterState.mode = clusterState.LOGIN_REQUIRED;
    
    // Add main toggle practice icon
    $('<div id="mainClusterIcon" class="fl clusterIcon" onmouseover="clusterIconOver()" onmouseout="clusterIconOut()" onclick="toggleClusterArea(null)"></div>').insertAfter(
        '#practiceMenu');
    cluster.setUpClusterIcon();
    clusterState.isInitialised = true;
}

function setClusterMode(newMode) {
    
    
    clusterState.mode = newMode;
    
    if (newMode == clusterState.NEW || 
            newMode == clusterState.EDIT) {   
        clusterState.editOn();
    } else {
        clusterState.editOff();
    }
    
    displayCluster();
}

function saveCluster() {
    
    var name = cluster.getName(); 
    
    if (String.isNullOrEmpty(name)) {
        alert("no name");
        return;
    }
    //TODO check cluster name is unique
    
    if (!cluster.areas.length) {
        alert("no practices");
        return;
    }
    
    
    var codes = _.pluck(cluster.areas,'Code');
    $.post('SaveCluster.aspx', 
        {
            s: 'sc',
            acs: codes.join(','),
            nam: name,
            cid: cluster.getId()
        }, 
        saveClusterCallback,
        'json');
    
}

function clusterIconOver() {
    cluster.iconOver = true;
    cluster.refreshClusterIcon();
}

function clusterIconOut() {
    cluster.iconOver = false;
    cluster.refreshClusterIcon();
}

function saveClusterCallback(obj) {
    
    if (obj.result === 1) {
        cancelClusterEdit();
    } else {
        // Save failed
        alert(obj.message);
    }
}

function getClusters() {
    ajaxGetPage('GetClusters.aspx','',getClustersCallback);
}

function getClustersCallback(obj) {
    
    clusterState.clusterList = obj;
    ajaxMonitor.callCompleted();
}

function ajaxGetPage(service, data, successFunction, errorFunction) {

    var parameters = {
        type: 'GET',
        url: service,
        data: data,
        cache: false,
        contentType: 'application/json',
        success: successFunction,
        error: isDefined(errorFunction) ?
            errorFunction : ajaxError
    };
    
    parameters['dataType'] = useJsonp ? 'jsonp': 'json';
    if (useJsonp) {
        parameters['jsonpCallback'] = 'jp';
    }
    
    $.ajax(parameters);
}

function login() {
    setClusterMode(clusterState.USER_CLUSTERS);
}

function viewCluster(id) {

    
    
    
    
    
    
    
    
}

templates.add('editCluster','<h3>New cluster</h3>Create groups of practice, etc.<br>Name:<input id="clusterName" type="text" /><table id="clusterList"></table>\
<input type="button" value="Save" class="link-button" onclick="saveCluster()"/><input class="link-button" type="button" value="Cancel" onclick="cancelClusterEdit()"/>');

templates.add('loginRequired','<h3 style="margin-left:10%;">Create your own practice clusters</h3><p style="margin-left:10%;width:80%;">Group practices together to view data for a limited number of practices at a time. Cluster indicator averages are calculated. You must be logged in to use.</p>\
<br><input id="loginCluster" class="link-button" onclick="login();" value="Login">');

templates.add('practiceList','<tbody>{{#areas}}<tr><td style="vertical-align:top;padding: 0 6px 0 6px;"><div class="removeClusterItem" onclick="toggleClusterArea(\'{{Code}}\')"></div></td>\
<td class="resultCode">{{Code}}</td><td class="result">{{Name}}, {{Address}}</td></tr>{{/areas}}{{^areas}}<tr><td>No assigned practices</td></tr>{{/areas}}</tbody>');

templates.add('userClusters','<h3>Your practice clusters</h3>\
<ul id="clusterList">{{#clusters}}<li><a href="javascript:viewCluster(\'{{Id}}\');">{{Name}} ({{count}})</li>{{/clusters}}{{^clusters}}<li>You have no clusters</li>{{/clusters}}</ul>\
<input style="float:left;clear:both;" type="button" class="link-button" onclick="setClusterMode(clusterState.NEW);" value="New">');

