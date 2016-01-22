
$(document).ready(function () {

    initTableSorter();
    initLookupAgeFilter();
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
