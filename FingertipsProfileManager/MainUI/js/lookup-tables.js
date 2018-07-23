
$(document).ready(function () {

    initTableSorter();
    initLookupAgeFilter();

    initTinyMce();
});

function initLookupAgeFilter() {
    var ageFilter = $('#age-filter'),
    $table = $('#lookup-table');
    ageFilter.bind("keyup", function (event) {

        var userText = $.trim(ageFilter.val()).toLowerCase();

        if (userText.length === 0) {
            // Show all
            $table.find('tr').show();
        } else {
            var tds = $table.find('tr td:last-child');
            _.each(tds, function (td) {
                var $td = $(td);
                $td.parent().toggle($td.html().toLowerCase().indexOf(userText) > -1);
            });
        }
    });
}

function initTinyMce() {
    if (typeof (tinymce) !== 'undefined') {
        tinymce.init({
            verify_html: false,
            height: 160,
            width: '99.9%',
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
            plugins: [
                'advlist autolink lists link image charmap hr anchor pagebreak',
                'searchreplace wordcount visualblocks visualchars fullscreen',
                'insertdatetime nonbreaking save table directionality',
                'template paste code preview'
            ],
            theme_advanced_buttons1_add_before: 'h1,h2,h3,h4,h5,h6,separator',
            toolbar1: 'insertfile undo redo | styleselect | bold italic | bullist numlist | link | code preview',
            image_advtab: true,
            forced_root_block: "" // do not automatically insert p tags
        });
    }
}
