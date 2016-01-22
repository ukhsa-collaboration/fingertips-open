function initPage() {
    
    updateModelFromHash();
    
    ajaxMonitor.setCalls(1);
    
    var model = MT.model;
    
    /*
    NOTE: use national code to bring back total for england, if page displays 
    area specific content in the future then two calls will be needed
    
    ALSO: in future, must be able to handle case where no area code is specified, 
    e.g. for a link from the front page
    */

    loaded.areaDetails.fetchDataByAjax({ areaCode: NATIONAL_CODE });

    ajaxMonitor.monitor(displayPage);
}

function displayPage() {
    
    var areaCode = NATIONAL_CODE;
    var areaDetails = loaded.areaDetails.getData({ areaCode: NATIONAL_CODE });
    var ranks = areaDetails.Ranks[areaCode];
    var rank = ranks[getGroupRootIndex()];
    var max = rank.Max,
    min = rank.Min;
    
    $('#worst_national').html(max.Area.Name + ' ' + new CommaNumber(max.Count).rounded() + ' deaths');
    $('#best_national').html(min.Area.Name + ' ' + new CommaNumber(min.Count).rounded() + ' deaths');
    $('#total_national').html(new CommaNumber(rank.AreaRank.Count).rounded());
}

function getGroupRootIndex() {
    
    for (var i = 0; i < causeOptions.length; i++) {
        if (causeOptions[i].key === causeKey) {
            break;   
        }
    }
    
    return i + ROOT_INDEXES.OVERALL_MORTALITY + 1;
    
}

