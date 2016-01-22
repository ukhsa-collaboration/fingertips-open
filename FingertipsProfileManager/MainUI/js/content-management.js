
function documentReady() {

    initTinyMce();

    initTableSorter();

    $('#selectedProfile').change(function () {
        loading();
        $('form#IndicatorManagementForm').submit();
    });

    $('.select-all-check-box').click(function () {
        $('.indicator-check-box').attr('checked', $(this).is(':checked'));
    });

    $('#PlainTextContent').click(function () {
        toggleEditorWarning();
    });

    $('#audit_history').click(function () {

        var contentIds = [];
        $('.indicator-check-box:checked').each(function () {
            contentIds.push($(this).val());
        });

        if (contentIds.length) {
            loading();

            $.ajax({
                cache: false,
                type: 'Get',
                url: '/GetContentAuditList',
                data: { contentIds: contentIds },
                traditional: true,
                dataType: 'html',
                success: function (data) {
                    lightbox.show(data, 20, 300, 700);
                    loadingFinished();
                },
                error: function () {
                    loadingFinished();
                }
            });
        } else {
            showSimpleMessagePopUp('Select a content item to view its change history');
        }
    });

    $('#delete').click(function() {
        $.ajax({
            type: 'post',
            url: '/content/delete-content-item',
            data: {
                contentItemId: $('#Id').val()
            },
            success: function (data) {
                var url = $('#ReturnUrl').val();
                setUrl(url);
            },
            error: function (xhr, error) {
                showSimpleMessagePopUp('Sorry, that did not work');
            }
        });
    });
}

function toggleEditorWarning() {
    if ($('#PlainTextContent').is(':checked')) {
        $('.mce-toolbar').hide();
        $('#ContentWarning').show();
    } else {
        $('.mce-toolbar').show();
        $('#ContentWarning').hide();
    }
}

function initTinyMce() {
    if (typeof(tinymce) !== 'undefined') {
        tinymce.init({
            verify_html: false,
            height: 300,
            width: '100%',
            menubar: false,
            selector: 'textArea',
            theme: 'modern',
            theme_advanced_buttons1: 'formatselect',
            style_formats: [
                { title: 'Header 1', block: 'h1' },
                { title: 'Header 2', block: 'h2' },
                { title: 'Header 3', block: 'h3' },
                { title: 'Header 4', block: 'h4' }
            ],
            relative_urls: false,
            plugins: [
                'advlist autolink lists link image charmap hr anchor pagebreak',
                'searchreplace wordcount visualblocks visualchars fullscreen',
                'insertdatetime nonbreaking save table directionality',
                'template paste code preview'
            ],
            theme_advanced_buttons1_add_before: 'h1,h2,h3,h4,h5,h6,separator',
            toolbar1: 'insertfile undo redo | styleselect | bold italic | bullist numlist | link | code preview',
            image_advtab: true,

            setup: function(ed) {
                ed.on("init", function () {
                    if ($('#PlainTextContent').is(':checked') || $('#disabledCheckBox').is(':checked')) {
                        $('#ContentWarning').show();
                        $('.mce-toolbar').hide();
                    }
                });
            }
        });
    }
}

$(document).ready(documentReady);
