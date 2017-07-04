'use strict';

/**
* England namespace
* @module england
*/
var england = {
    state : {
        viewModel : null
    }
};

function goToEnglandPage() {

    setPageMode(PAGE_MODES.ENGLAND);

    if (!areIndicatorsInDomain()) {
        displayNoData();
    } else {
        ajaxMonitor.setCalls(2);

        getGroupingData();
        getIndicatorMetadata(FT.model.groupId);

        ajaxMonitor.monitor(england.displayEnglandView);
    }
}

england.displayEnglandView = function () {

    var viewModel = england.getViewModel();
    england.state.viewModel = viewModel;

    // Render view
    templates.add('england', '<table class="bordered-table table-hover"><thead><tr><th>Indicator</th><th>Period</th><th class="center">England<br/>count</th><th class="center">England<br/>value</th>' +
        '{{#hasRecentTrends}}<th class="center">Recent<br/>trend</th>{{/hasRecentTrends}}{{#isChangeFromPreviousPeriodShown}}<th class="center">Change from<br/>previous time period</th>{{/isChangeFromPreviousPeriodShown}}</tr></thead>' +
        '<tbody>{{#rows}}<tr>' +
        '<td><a class="pLink" href="javascript:goToMetadataPage({{id}});">{{indicatorName}}</a></td>' +
        '<td>{{period}}</td><td class="numeric">{{{count}}}</td>' +
        '<td class="numeric{{#hasValueNote}} valueNote" id="england-value-{{id}}" vn="{{noteId}}{{/hasValueNote}}">{{{value}}}</td>' +
        '{{#hasRecentTrends}}<td id="england-trend-{{id}}" class="center pointer" onclick="recentTrendSelected.byGroupRoot({{id}})">{{{recentTrend}}}</td>{{/hasRecentTrends}}' +
        '{{#isChangeFromPreviousPeriodShown}}<td id="england-change-{{id}}" class="center">{{{changeFromPrevious}}}</td>{{/isChangeFromPreviousPeriodShown}}' +
        '</tr>{{/rows}}</tbody></table>');
    var englandhtml = templates.render('england', viewModel);
    pages.getContainerJq().html(englandhtml);

    showAndHidePageElements();

    unlock();

    england.initChangeTooltips(viewModel);
}

function EnglandTooltipProvider() {
}

EnglandTooltipProvider.prototype = {
    getHtml: function (id) {

        var bits = id.split('-');
        var identifier = bits[1];

        if (identifier === 'value') {
            // Value cell with a value note asterisk
            var $td = $('#' + id);
            var noteId = $td.attr('vn');
            var html = new ValueNoteTooltipProvider().getHtmlFromNoteId(noteId);
            return html;
        } else if (identifier === 'trend') {
            var rows = england.state.viewModel.rows;
            var rowId = bits[2];
            var row = _.find(rows, function (row) { return row.id === rowId });
            return new RecentTrendsTooltip().getTooltipByData(row.recentTrendObj);
        }
    }
};

england.initChangeTooltips = function(viewModel) {
    var provider = new EnglandTooltipProvider();
    tooltipManager.setTooltipProvider(provider);
    var rows = viewModel.rows;
    for (var i in rows) {
        var id = rows[i].id;
        tooltipManager.initElement('england-value-' + id);
        tooltipManager.initElement('england-trend-' + id);
    }

    loadValueNoteToolTips();
}

england.getViewModel = function () {
    var metadataHash = ui.getMetadataHash();

    // Create view model
    var config = FT.config;
    var viewModel = {
        rows: [],
        hasRecentTrends: config.hasRecentTrends,
        isChangeFromPreviousPeriodShown: config.isChangeFromPreviousPeriodShown
    };
    for (var i in groupRoots) {

        // Define data
        var root = groupRoots[i];
        var metadata = metadataHash[root.IID];
        var unit = !!metadata ? metadata.Unit : null;

        // Create view model row
        var row = {};
        viewModel.rows.push(row);
        row.id = i;
        row.period = root.Grouping[0].Period;
        row.indicatorName = metadata.Descriptive.Name + new SexAndAge().getLabel(root);

        // Set data
        var englandData = getNationalComparatorGrouping(root).ComparatorData;
        var dataInfo = new CoreDataSetInfo(englandData);
        row.value = new ValueDisplayer(unit).byDataInfo(dataInfo);
        row.count = formatCount(dataInfo);
        row.hasValueNote = dataInfo.isNote();
        row.noteId = englandData.NoteId;

        // Recent trend
        row.isTrend = !!root.RecentTrends;
        if (row.isTrend) {
            if (dataInfo.isValue()) {
                var recentTrends = root.RecentTrends[englandData.AreaCode];
                row.recentTrendObj = recentTrends;
                var polarityId = root.PolarityId;
                row.recentTrend = getTrendMarkerImage(recentTrends.Marker, polarityId);
                row.changeFromPrevious = getTrendMarkerImage(recentTrends.MarkerForMostRecentValueComparedWithPreviousValue, polarityId);
            } else {
                row.recentTrend = getTrendMarkerImage(TrendMarkerValue.CannotCalculate, 0);
                row.changeFromPrevious = row.recentTrend;
            }
        }
    }

    return viewModel;
}


pages.add(PAGE_MODES.ENGLAND, {
    id: 'england',
    title: 'England',
    icon: 'england',
    goto: goToEnglandPage,
    gotoName: 'goToEnglandPage',
    needsContainer: true,
    jqIds: ['england', 'value-note-legend', 'trend-marker-legend']
});
