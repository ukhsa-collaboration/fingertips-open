
function documentReady() {
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
}

$(document).ready(documentReady);
