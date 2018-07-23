'use strict';

/**
* Functions to display a user-defined indicator list.
* Functions mainly override equivalents in SiteSearch.js
*
* Global variables:
*   indicatorListId -> the ID of the user-defined indicator list
* 
* @module indicatorList
*/

function getSearchResults() {

    var parameters = new ParameterBuilder(
        ).add('indicator_list_id', indicatorListId
        ).setNotToCache();

    ajaxGet('/api/indicator-list/indicators-for-each-area-type', parameters.build(),
        getSearchResultsCallback, handleAjaxFailure);

    logEvent('IndicatorList', 'View', indicatorListId);
}

function getGroupingDataCall() {

    var parameters = new ParameterBuilder(
        ).add('indicator_list_id', indicatorListId
        ).setNotToCache();
    addGroupDataParameters(parameters);

    ajaxGet('api/latest_data/indicator_list_for_child_areas', parameters.build(),
        getGroupingDataCallback, handleAjaxFailure);
}



