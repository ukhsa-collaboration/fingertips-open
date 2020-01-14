function documentReady() {
    // Remove validation class from the server dropdown
    $('#ServerList').removeClass('input-validation-error');

    $('.sortable').tablesorter({
        theme: 'blue',
        widthFixed: true,
        widgets: ['zebra'],
        headers: {
            1: {
                sorter: false
            },
            3: {
                // disable it by setting the property sorter to false 
                sorter: false
            },
            4: {
                // disable it by setting the property sorter to false 
                sorter: false
            },
            6: {
                // disable it by setting the property sorter to false 
                sorter: false
            },
            7: {
                // disable it by setting the property sorter to false 
                sorter: false
            }
        }
    });
    
    $('#exceptionDays, #exceptionServer').change(function () {
        loading();
        $("form").submit();
    });

    $('.show-spinner').click(function () {
        loading();
    });
    
    $('.exception-detail-link').click(function () {
        loading();
        var exceptionId = $(this).attr('id');
        
        $.ajax({
            cache: false,
            type: "Get",
            url: "/exceptions/exception-partial/" + exceptionId,
            traditional: true,
            dataType: 'html',
            success: function (data) {
                var h = data;
                lightbox.show(h, 20, 300, 1000);
                $('#exception-detail').show();
                loadingFinished();
            },
            error: function (xhr, error) {
                loadingFinished();
                //show the error somewhere
            }
        });
    });

    $('#live-servers').click(function() {
        if ($('#LiveServersFilterApplied').val() === 'False') {
            $('#LiveServersFilterApplied').val('True');
            $('#NumberOfDays').val('5');
        } else {
            $('#LiveServersFilterApplied').val('False');
            $('#NumberOfDays').val('0');
        }

        $('#ServerList').val('ALL SERVERS');
        $('#Server').val('ALL SERVERS');

        submitForm();
    });

    $('#api-errors').click(function() {
        if ($('#ApiErrorsFilterApplied').val() === 'False') {
            $('#ApiErrorsFilterApplied').val('True');
            $('#NumberOfDays').val('5');
        } else {
            $('#ApiErrorsFilterApplied').val('False');
            if ($('#LiveServersFilterApplied').val() === 'False') {
                $('#NumberOfDays').val('0');
                $('#ServerList').val('ALL SERVERS');
                $('#Server').val('ALL SERVERS');
            }
        }

        submitForm();
    });
}

function triggerNumberOfDaysChangedEvent(e) {
    submitForm();
}

function triggerServerListChangedEvent(e) {
    $('#Server').val(e.value);
    submitForm();
}

function submitForm() {
    $('#reload-exceptions-form').submit();
}

$(document).ready(documentReady);
