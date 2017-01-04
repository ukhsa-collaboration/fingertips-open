
CallOutBox.getExtendedModel = function (areaPopulation, getGrade) {
   
    var model = MT.model;
    var indicatorId = model.indicatorId;
    var metadataHash = loaded.indicatorMetadata[model.groupId];

    var extraModel = {
        isHealthChecksDomain: model.groupId === GroupIds.HealthChecks.HealthCheck,
        population: new CommaNumber(areaPopulation).rounded()
    };

    if (extraModel.isHealthChecksDomain) {

        var areaData = loaded.groupDataAtDataPoint.getData();
        for (var i = 0; i <= 2; i++) {

            var data = areaData[i];
            var iid = groupRoots[i].IID;

            extraModel['indicatorGrade' + i] = getGrade(data.Sig[NATIONAL_COMPARATOR_ID], data.Val);
            extraModel['indicatorName' + i] = isDefined(metadataHash[iid].Descriptive.IndicatorContent) ? metadataHash[iid].Descriptive.IndicatorContent : '';
            extraModel['indicatorValF' + i] = data.ValF;
            extraModel['isindicatorselected' + i] = indicatorId === iid;
        }
    }
    return extraModel;
}

var header = '<div class="map-template map-info"><div class="map-info-header clearfix"><span class="map-info-close" onclick="closeInfo()">&times;</span></div><div class="map-info-body map-info-stats clearfix">';
var footer = '</div><div class="map-info-footer clearfix">{{^hasPracticeData}}<a href="javascript:MT.nav.areaDetails();">View local authority details</a>{{/hasPracticeData}}</div><div class="map-info-tail" onclick="pointerClicked()"><i></i></div>\</div>';
var indicatorText = '{{{indicatordescription}}} {{period}}. {{{rankingHtml}}}';

var selectedIndicatorContent = '<div id="selected-indicator-content">' +
    '<div class="{{rankClass}} man"></div>' +
    '<div class="selected-indicator-text"><span class="{{rankClass}}"><strong>{{ranking}}</strong><span class="{{unitClass}}">{{{unit}}}</span></span><p class="selected-indicator-ranking-text">' + indicatorText + '</p></div></div>' +
    '<div style="margin-left:30px; margin-bottom:30px;" onclick="CallOutBox.toggleDefinition()"><span><strong>Definition</strong></span><span id="defBtn" class="{{#isDefinitionOpen}}minus{{/isDefinitionOpen}}{{^isDefinitionOpen}}plus{{/isDefinitionOpen}}">&nbsp;</span>' +
    '<div id="def" style="display:{{#isDefinitionOpen}}block{{/isDefinitionOpen}}{{^isDefinitionOpen}}none{{/isDefinitionOpen}};">{{{filterheader}}}<div id="data-source"><span>Data source:</span> {{{dataSource}}}</div></div>' +
    '</div>';
var indicator1Content = '{{#isindicatorselected0}}' + selectedIndicatorContent + '{{/isindicatorselected0}}{{^isindicatorselected0}}<div style="margin-left:30px; padding-bottom:8px;"><span class="grade-{{indicatorGrade0}}" style="font-size:22px;">{{indicatorValF0}}<span class="unit arial">%</span></span> {{indicatorName0}} {{period}}</div>{{/isindicatorselected0}}';
var indicator2Content = '{{#isindicatorselected1}}' + selectedIndicatorContent + '{{/isindicatorselected1}}{{^isindicatorselected1}}<div style="margin-left:30px; padding-bottom:8px;" ><span class="grade-{{indicatorGrade1}}" style="font-size:22px;">{{indicatorValF1}}<span class="unit arial">%</span></span> {{indicatorName1}} {{period}}</div>{{/isindicatorselected1}}';
var indicator3Content = '{{#isindicatorselected2}}' + selectedIndicatorContent + '{{/isindicatorselected2}}{{^isindicatorselected2}}<div style="margin-left:30px; padding-bottom:8px;"><span class="grade-{{indicatorGrade2}}" style="font-size:22px;">{{indicatorValF2}}<span class="unit arial">%</span></span> {{indicatorName2}} {{period}}</div>{{/isindicatorselected2}}';
var line = '<hr style="border: 0;height: 2px;background: #333; margin-left:30px;"/>';
var content = '<h4 class="diabetes-place-name">{{nameofplace}}</h4>' +
        '<div id="cbox_population">' +
        '<div class="t-cell width-30 grey-man">&nbsp;</div><div class="t-cell"><div><strong>{{population}}</strong></div><div id="population-text">{{{topIndicatorText}}}</div></div>' +
        '</div>' +
        line +
        '{{#isHealthChecksDomain}}' + indicator1Content + indicator2Content + indicator3Content + '{{/isHealthChecksDomain}}{{^isHealthChecksDomain}}' + selectedIndicatorContent + '{{/isHealthChecksDomain}}' +
        line + (hasPracticeData ? '<div class="gp-link">{{{footerLinkText}}}</div>' : '') +
        '<div class="ccg-link"><a href=javascript:MT.nav.nationalRankings();><strong style="font-size: 1em;">See national {{area}} comparison table</strong></a></div>' +
        '<br style="clear:both;" />';

templates.add('areaoverlay', header + content + footer);