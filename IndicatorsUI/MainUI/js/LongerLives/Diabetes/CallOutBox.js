CallOutBox.getExtendedModel = function (areaPopulation, getGrade, areaValues) {
    var population;
    var model = MT.model;

    if (model.groupId === GroupIds.Diabetes.Complications ||
        model.profileId === ProfileIds.DrugsAndAlcohol) {
        population = areaValues[model.areaCode].Count;
    } else {
        population = areaPopulation;
    }

    // Round population if a number
    if (_.isNumber(population)) {
        population = new CommaNumber(population).rounded();
    }

    return {
        population: population
    };
}

var header = '<div class="map-template map-info"><div class="map-info-header clearfix"><span class="map-info-close" onclick="closeInfo()">&times;</span><a href="javascript:{{topLinkFunction}}()">{{topLinkText}} areas on map ></a></div><div class="map-info-body map-info-stats clearfix">';
var footer = '</div><div class="map-info-footer clearfix">{{^hasPracticeData}}<a href="javascript:MT.nav.areaDetails();">View details for this area</a>{{/hasPracticeData}}</div><div class="map-info-tail" onclick="pointerClicked()"><i></i></div>\</div>';
var rankings = 'Ranked <strong style="font-size: 1em; padding-left: 0px;">{{ranked}}</strong> out of <span class="rank-max">{{rankoutof}} {{{areatypename}}}.';
var indicatorText = '{{{indicatordescription}}} for {{period}}. ' + rankings;

var complications = '{{{ranking}}}. ' + rankings;

var content = '<h4 class="diabetes-place-name">{{nameofplace}}</h4>' +
        '<div id="cbox_population">' +
        '<div class="t-cell width-30 grey-man">&nbsp;</div><div class="t-cell"><div><strong>{{population}}</strong></div><div id="population-text">{{{populationText}}}</div></div>' +
        '</div>' +
        '<div id="selected-indicator-content">' +
        '<div class="{{rankClass}} man"></div>' +
        '<div class="selected-indicator-text">{{#isdomainnormal}}<strong class="{{rankClass}}">{{ranking}}</strong><span class="{{unitClass}}">{{{unit}}}</span> <span class="value-note">{{valueNote}}</span>{{/isdomainnormal}}<p class="selected-indicator-ranking-text">{{#isdomainnormal}}' +
        indicatorText + '{{/isdomainnormal}}{{^isdomainnormal}}' + complications + '{{/isdomainnormal}}</p></div></div>' +
        '<div style="margin-left:30px; margin-bottom:30px;" onclick="CallOutBox.toggleDefinition()"><span><strong>Definition</strong></span><span id="defBtn" class="{{#isDefinitionOpen}}minus{{/isDefinitionOpen}}{{^isDefinitionOpen}}plus{{/isDefinitionOpen}}">&nbsp;</span>' +
        '<div id="def" style="display:{{#isDefinitionOpen}}block{{/isDefinitionOpen}}{{^isDefinitionOpen}}none{{/isDefinitionOpen}};">{{{filterheader}}}<div id="data-source"><span>Data source:</span> {{{dataSource}}}</div></div>' +
        '</div>' +
        (hasPracticeData ? '<div class="gp-link">{{{footerLinkText}}}</div>' : '') +
        '<div class="ccg-link"><a href=javascript:MT.nav.nationalRankings();><strong style="font-size: 1em;">See national {{area}} comparison table</strong></a></div>' +
        '<br style="clear:both;" />';

templates.add('areaoverlay', header + content + footer);