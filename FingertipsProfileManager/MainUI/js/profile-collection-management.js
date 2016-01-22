
function documentReady() {
    initTableSorter();

    $('.sortHeader').click(function () {
        loading();
    });

    $('.sortDesc').click(function () {
        loading();
    });

    $('.sortAsc').click(function () {
        loading();
    });

    $('.show-spinner').click(function () {
        loading();
    });

    $('.select-all-check-box').click(function () {
        $('.selectable-check-box').attr('checked', $(this).is(':checked'));
    });
    
    $('#update_profile_collection').click(function () {
        var jdata = { 'selectedProfiles': [] };
        var displayDomains;
        $('.selectable-check-box:checked').each(function () {
            displayDomains = $('#' + $(this).val() + '_display_domains').attr('checked') == 'checked';
            jdata['selectedProfiles'].push($(this).val() + '~' + displayDomains);
        });

        $('#assignedProfiles').val(jdata['selectedProfiles']);
        $("form#UpdateProfileCollection").submit();
    });

    $('#create_profile_collection').click(function () {
        var jdata = { 'selectedProfiles': [] };
        $('.selectable-check-box:checked').each(function () {
            jdata['selectedProfiles'].push($(this).val());
        });

        $('#assignedProfiles').val(jdata['selectedProfiles']);
        $("form#CreateProfileCollection").submit();
    });
}

$(document).ready(documentReady);
