'use strict';

$(document).ready(function() {
    initTableSorter();
});

function updateLive($td, url, data) {
    displayPleaseWaitForUpdate($td);

    $.ajax({
        cache: false,
        type: 'post',
        url: url,
        data: data,
        success: function (response) {
            handleUpdateResponse(response, $td);
        },
        error: function (xhr, error) {
            handleUpdateErrorResponse(error, $td);
        }
    });
}

function replaceIndicatorMetadataTextValuesLive(indicatorId) {

    var $td = $('#td-replace-indicator-metadata-textvalues-live-' + indicatorId);

    updateLive($td,
        '/live-updates/replace-metadata',
        {
            indicator_id: indicatorId
        });
}

function replaceGroupingsLive(profileId, indicatorId) {
    var $td = $('#td-replace-groupings-live-' + indicatorId);

    updateLive($td,
        '/live-updates/replace-groupings',
        {
            profile_id: profileId,
            indicator_id: indicatorId
        });
}

function replaceCoreDataSetLive(indicatorId) {
    var $td = $('#td-replace-indicator-coredataset-live-' + indicatorId);

    updateLive($td,
        '/live-updates/replace-coredata',
        {
            indicator_id: indicatorId
        });
}

function appendUpdateError(error) {
    var $error = $('#error-message');
    var message = error + '<br><br>' + $error.html();
    $error.html(message).addClass('live-update-error-message');
}

function handleUpdateResponse(response, $td) {
    if (response.Success) {
        $td.html("<span class='update-success'>Replaced</span>");
    }
    else {
        handleUpdateErrorResponse(response.Message, $td);
    }
}

function handleUpdateErrorResponse(error, $td) {
    if ($td) {
        $td.html('<span class="update-failed">Error</span>');
    }
    appendUpdateError(error);
}

function displayPleaseWaitForUpdate($td) {
    $td.html('<span class="update-wait">Please wait</span>');
}

function replaceAllIndicatorMetadataAndGroupings(profileId) {

    // Check bulk replace can go ahead
    if ($('#source-url').html() === $('#target-url').html()) {
        showSimpleMessagePopUp('Bulk replace will not work when source and target URLs are the same because the grouping will be deleted from the target.');
        return;
    }

    // Prevent repeated clicks of overall replace button
    var $replaceButton = $('#replace-all-indicator-metadata-and-groupings-live');
    $replaceButton.enabled = false;

    $.ajax({
        cache: false,
        type: 'post',
        url: '/live-updates/delete-all-groupings-for-profile',
        data: { profile_id: profileId },
        success: function (response) {
            if (response.Success) {
                $("[id^='replace-groupings-live-']").each(function () {
                    var indicatorId = $(this).attr("indicatorId");

                    replaceIndicatorMetadataTextValuesLive(indicatorId);
                    replaceGroupingsLive(profileId, indicatorId);
                });
            }
            else {
                appendUpdateError(response.Message);
            }
            $replaceButton.enabled = true;
        },
        error: function (xhr, error) {
            handleUpdateErrorResponse(error);
            $replaceButton.enabled = true;
        }
    });
}
