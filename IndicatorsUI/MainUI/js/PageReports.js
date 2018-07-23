function goToReportsPage() {    
    setPageMode(PAGE_MODES.REPORTS);
    ajaxMonitor.setCalls(2);
    getReportsData();
    getContentText('ssrs-report-extra-notes');
    ajaxMonitor.monitor(displayReport);
}

function getReportsData() {
    ajaxGet('api/ssrs_reports/' + FT.model.profileId ,'' ,function (data) {
        reports = data;
        ajaxMonitor.callCompleted();
    });
}


function displayReport() {
    
    var obj = {
        reports: reports,
        hasReports: reports.length > 0,
        noReports: reports.length < 1,
        word: 'word',
        pdf: 'pdf',
        extraNotes: loaded.contentText
    };

    templates.add('ssrsReportList', '<h2>Related reports</h2>' +
        '{{#hasReports}}' +
        '<table class="bordered-table table-hover" >' +
        '<thead>' +
        '<tr>' +
        '<th>Name</th>' +
        '<th style="width:400px;">Notes</th>' +
        '<th colspan="3" style="text-align:center;width:400px;">Download</th>' +
        '</tr>' +
        '</thead>' + 
        '<tbody>' +
        '{{#reports}}' +
        '<tr>' +
        '<td>{{Name}}</td>' +
        '<td>{{{Notes}}}</td>' +
        '<td class="format"><a class="pLink" href="#" onclick="openReport({{Id}}, \'pdf\'); return false;" >PDF</a></td>' +
        '<td class="format"><a class="pLink" href="#" onclick="openReport({{Id}}, \'word\'); return false;" >Word</a></td>' +
        '</tr>' +
        '{{/reports}}' +
        '</tbody>' +
        '</table>' +
        '<br><br><div>Please note reports open in a separate tab or window. Reports may take a minute or two to load.</div>' +
        '<br><div>{{{extraNotes}}}</div>' +
        '{{/hasReports}}' +
        '{{#noReports}}<div>There are no reports available for this profile</div>{{/noReports}}' 
        );

    pages.getContainerJq().html(templates.render('ssrsReportList', obj));

    showAndHidePageElements();
    loadReports();
    unlock();
}


function getReportUrl(report, format) {

    var parameters = report.Parameters;
   
    var baseUrl = '/reports/ssrs/';

    var model = FT.model;

    var reportUrl = baseUrl + "?reportName=" + encodeURIComponent(report.File);
    if (parameters.includes('areaCode')) {
        reportUrl = reportUrl + "&areaCode=" + model.areaCode;
    }
    if (parameters.includes('areaTypeId')) {
        reportUrl = reportUrl + "&areaTypeId=" + model.areaTypeId;
    }
    
    if (parameters.includes('parentCode')) {
        reportUrl = reportUrl + "&parentCode=" + model.parentCode;
    }
     
    if (parameters.includes('parentTypeId')) {
        reportUrl = reportUrl + "&parentTypeId=" + model.parentTypeId;
    }

    if (parameters.includes('groupId')) {
        reportUrl = reportUrl + "&groupId=" + model.groupId;
    }

    return  reportUrl + "&format=" +format;   
}

function  renderReport(url) {
    $('#report-frame').attr("src",url);
}

function loadReports() {
    for (var i = 0; i < _.size(reports); i++) {
        $('#reports').append('<option value='+ i  +'>' + reports[i].Name + '</option>');
    }
}

function openReport(id, format) {
    var selectedReport = _.findWhere(reports, { Id: id });
    window.open(getReportUrl(selectedReport, format), '_blank');
    logEvent('SsrsReportView', format, id);
}


var reports;
var FORMAT_PDF = 1;
var FORMAT_WORD = 2;
pages.add(PAGE_MODES.REPORTS,
{
    id: "reports",
    title: "Reports",
    goto: goToReportsPage,
    gotoName: 'goToReportsPage',
    needsContainer: true,
    jqIds: [ 'areaMenuBox', 'parentTypeBox', 'areaTypeBox', 'region-menu-box']
});
