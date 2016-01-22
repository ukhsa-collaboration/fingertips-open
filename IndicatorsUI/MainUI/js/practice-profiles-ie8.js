lightbox.setFadeIn(false);

lightbox.preShow = function() {
    $('.highcharts-container').hide();
};

lightbox.preHide = function() {
    $('.highcharts-container').show();
};

