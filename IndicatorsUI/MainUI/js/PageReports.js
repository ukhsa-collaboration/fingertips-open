function goToReportsPage() {    
    setPageMode(PAGE_MODES.REPORTS);
    ajaxMonitor.setCalls(1);
    getReportsData();
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
        reports: reports
    }

    templates.add('ssrsReportList', '<h2>Reports</h2><table class="bordered-table table-hover" >' +
        '<tbody>' +
        '{{#reports}}' +
        '<tr>' +
        '<td>{{Name}}</td>' +
        '<td class="format"><a class="pLink" href="#" onclick="openReport({{Id}}); return false;"  target="_blank">HTML</a></td>' +
        '<td class="format"><a class="pLink" href="#" onclick="openReport({{Id}}, 1); return false;" >PDF</a></td>' +
        '<td class="format"><a class="pLink" href="#" onclick="openReport({{Id}}, 2); return false;" >Word</a></td>' +
        '</tr>' +
        '{{/reports}}' +
        '</tbody>' +
        '</table>');

    pages.getContainerJq().html(templates.render('ssrsReportList', obj));

    showAndHidePageElements();
    loadReports();
    // Select the first report
    renderReport(getReportUrl(reports[0]));
    // Add Change Event to report list
    $('#reports').on('change', function () {
        var index = $("#reports option:selected").val();
        renderReport(getReportUrl(reports[index].Name));
    });
    unlock();
}


function getReportUrl(report,format) {    
    var parameters = report.Parameters.split(',');    
    var ssrsParameters='';
    for (var i = 0; i < parameters.length; i ++) {
        if (parameters[i] === 'areaCode') {
            ssrsParameters +=  '&areaCode=' + FT.model.areaCode;
        } else if (parameters[i] === 'areaTypeId') {
            ssrsParameters += '&areaTypeId=' + FT.model.areaTypeId;
        } else if (parameters[i] === 'parentCode') {
            ssrsParameters += '&parentCode=' + FT.model.parentCode;
        } else if (parameters[i] === 'parentTypeId') {
            ssrsParameters += '&parentTypeId=' + FT.model.parentTypeId;
        } else if (parameters[i] === 'groupId') {
            ssrsParameters += '&groupId=' + FT.model.groupId;
        }
    }

    var baseUrl = 'http://sqlpor01.phe.gov.uk/ReportServer/Pages/ReportViewer.aspx?%2f';
    var finalUrl = baseUrl + report.Name + '&rs:Command=Render' + ssrsParameters + '&rc:Toolbar=false';
    if (format === FORMAT_PDF) {
        finalUrl = baseUrl + report.Name + '&rs:Command=Render' + ssrsParameters + '&rc:Toolbar=false' + '&rs:Format=PDF';
    } else if (format === FORMAT_WORD) {
        finalUrl = baseUrl + report.Name + '&rs:Command=Render' + ssrsParameters + '&rc:Toolbar=false' + '&rs:Format=WORD';
    }
    return finalUrl;
}

function  renderReport(url) {
    $('#report-frame').attr("src",url);
}

function loadReports() {
    for (var i = 0; i < _.size(reports); i++) {
        $('#reports').append('<option value='+ i  +'>' + reports[i].Name + '</option>');
    }
}

function openReport(x, format) {
    console.log(format);
    window.open(getReportUrl(reports[0],format),'_blank');
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
