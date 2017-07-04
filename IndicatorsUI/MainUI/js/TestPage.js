
function documentReady() {
    $('#jsTestError').click(function () {
        logError();
    });


    function logError(xhr) {
        var jsTestMessage = 'Javascript Test Error';
        var jsTestStatus = '99999';
        var parameters = 'errorMessage=' + encodeURI(jsTestMessage) + '&errorStatus=' + encodeURI(jsTestStatus);

        var ajaxConfig = {
            type: 'POST',
            url: FT.url.bridge + 'log/exception?' + parameters,
            cache: false,
            contentType: 'application/json; charset=utf-8'
        };

        $.ajax(ajaxConfig);
    };
}

$(document).ready(documentReady);
