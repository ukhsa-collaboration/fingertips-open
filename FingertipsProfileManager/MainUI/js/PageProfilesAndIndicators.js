
function submitMainForm() {
    loading();
    $('form#IndicatorManagementForm').submit();
}

function getIndicatorKeys() {
    var indicators = [];
    $.each($('.indicator-id'), function (index, value) {

        var sexTd = $(this).next('td').next('td');
        var sexId = sexTd.find(':selected').val();
        var ageId = sexTd.next('td').find(':selected').val();
        indicators.push(value.innerHTML + '~' + sexId + '~' + ageId);
    });
    return indicators;
}

function getIndicatorSpecifyingData(selectedIndicators) {

    var $profile = $('#selectedProfile'),
        $domain = $('#selectedDomain');

    return {
        jdata: selectedIndicators,
        selectedProfileUrlkey: $profile.val(),
        selectedProfileName: $profile.find(':selected')[0].text,
        selectedDomainId: $domain.val(),
        selectedDomainName: $domain.find(':selected')[0].text,
        selectedAreaTypeId: $('#SelectedAreaTypeId').val()
    };
}

$(document).ready(function () {

    registerReloadPopUpDomains();
    initSearchElements();
    initTableSorter();
    initLaterDataLinks();

    // When user clicks Confirm in the move indicators dialogue
    $(document).on('click', '#ConfirmMove', function () {
        loading();
        $('#indicatorTransferDetails').val(getIndicatorKeys());
        $('form#MoveIndicatorForm').submit();
    });

    // When user clicks Confirm in the copy indicators dialogue
    $(document).on('click', '#ConfirmCopy', function () {
        loading();
        $('#indicatorTransferDetails').val(getIndicatorKeys());
        $('form#CopyIndicatorForm').submit();
    });

    $(document).on('click', '#ConfirmDelete', function () {
        loading();

        var jdata = [];
        $.each($('.indicator-id'), function (index, value) {
            jdata.push(value.value);
        });

        $('#indicatorDeleteDetails').val(jdata);
        $('form#DeleteIndicatorForm').submit();
    });

    $('#resetArea').val('False');

    $('#selectedProfile').change(function () {
        if ($('#selectedDomain').length > 0) {
            $('#selectedDomain')[0].selectedIndex = 0;
        }
        $('#sortBy').val('Sequence');
        $('#ascending').val('True');
        $('#resetArea').val('True');
        submitMainForm();
    });

    $('#selectedDomain').change(function () {
        submitMainForm();
    });

    $('#SelectedAreaTypeId').change(function () {
        submitMainForm();
    });

    $('.select-all-check-box').click(function () {
        $('.indicator-check-box').attr('checked', $(this).is(':checked'));
    });

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

    $('#edit_Domains').click(function () {
        lightbox.show($('#editDomains').html(), 250, 0, 700);

        $('.unselected-domain').hover(function () {
            if (!$(this).hasClass('selected-domain')) {
                $(this).removeClass('unselected-domain').addClass('hover-domain');
            }
        }, function () { // mouseleave 
            if (!$(this).hasClass('selected-domain')) {
                $(this).removeClass('hover-domain').addClass('unselected-domain');
            }
        });

        $('.unselected-domain').click(function () {
            var $domain = $('.domain-to-edit');
            $domain.removeClass('unselected-domain').addClass('unselected-domain');
            $domain.removeClass('hover-domain');
            $domain.removeClass('selected-domain');
            $(this).addClass('selected-domain');
        });

        $('#reorderUp').click(function () {
            if ($('.selected-domain').length > 0) {
                var $parent = $('.selected-domain').closest('.domain-list');
                $('#save').addClass('save-required');
                $parent.insertBefore($parent.prev());
            }
        });

        $('#reorderDown').click(function () {
            if ($('.selected-domain').length > 0) {
                var $parent = $('.selected-domain').closest('.domain-list');
                $('#save').addClass('save-required');
                $parent.insertAfter($parent.next());
            }
        });

        $('#DeleteDomain').click(function () {

            var $selectedDomains = $('.selected-domain');

            if ($selectedDomains.length === 1) {
                var $parent = $selectedDomains.siblings('.domain-id');
                var $id = $parent.children(0).attr('value');
                $('#Domainform').attr('action', '/DeleteDomain');
                $('#Domainform').append('<input type="hidden" id="domainDeleteId" name="domainDeleteId" value="myvalue" />');
                $('#domainDeleteId').val($id);
                $('#save').addClass('save-required');
                $selectedDomains.hide();
            }
        });

        $('#addDomain').click(function () {
            $('#reorderUp').hide();
            $('#reorderDown').hide();
            $('#addDomain').hide();
            $('#DeleteDomain').hide();
            $('#newDomain').show().focus();
        });

        $('#save').click(function () {
            reorderDomains();
        });

        function reorderDomains() {
            for (var i = 0; i < $('#editDomains .domain-to-edit').length; i++) {
                var sequence = i + 1;
                $('.inputSequence')[i].value = sequence;
            }
        }
    });

    $('.edit-link').click(function () {
        loading();
    });

    $('#reorder-indicators').click(function() {
        var profileId = $('#profile-id').val(),
            profileUrlKey = $('#UrlKey').val(),
            domainSequence = $('#selectedDomain').val(),
            areaTypeId = $('#SelectedAreaTypeId').val(),
            groupId = $('#selected-group-id').val();

        window.location.href = "/ReorderIndicators?profileId=" +
            profileId +
            "&profileUrlKey=" +
            profileUrlKey +
            "&domainSequence=" +
            domainSequence +
            "&areaTypeId=" +
            areaTypeId +
            "&groupId=" +
            groupId;
    });

    $('#audit_history').click(function () {
        indicatorAction('/GetAudit', '#deleteIndicators', ' to see the history');
    });

    $('#delete-indicators-button').click(function () {
        indicatorAction('/DeleteIndicators', '#deleteIndicators', 'remove');
    });

    $('#move-indicators-button').click(function () {
        indicatorAction('/MoveIndicators', '#moveIndicators', 'move');
    });

    $('#copy-indicators-button').click(function () {
        indicatorAction('/CopyIndicators', '#copyIndicators', 'copy');
    });

    $('.view-indicator-data-btn').click(function () {
        var indicatorId = this.id.replace('view-data-', '');
        var indicatorName = $(this).attr('indicatorname');
        viewIndicatorData(indicatorId, indicatorName);
    });

    registerCopyDomains();
});

function registerCopyDomains() {
    var profileMenuId = '#TargetProfileUrlKey';

    $(document).on('change', profileMenuId, function () {
        var $domainMenu = $('#TargetGroupId');

        $.ajax({
            type: "post",
            url: "/reloadDomains",
            data: { selectedProfile: $(profileMenuId).val() },
            success: function (data) {
                repopulateDomainMenu($domainMenu, data);
            },
            error: function (xhr, error) {
            }
        });
    });
}

function initLaterDataLinks() {
    // Highlight when later data is available
    $('.later-period').each(function () {
        var $e = $(this),
            laterString = $e.attr('laterstring');

        $e.append('<i>Later data is available for this area type:<br><br><b>' +
            laterString + '</b><br><br>Click to use latest data</i>');

        $e.click(function () {
            $.ajax({
                type: 'post',
                dataType: 'json',
                url: '/SetDataPoint',
                data: {
                    profileId: FT.model.profileId,
                    areaTypeId: FT.model.areaTypeId,
                    indicatorId: $e.attr('indicatorid'),
                    sexId: $e.attr('sexid'),
                    ageId: $e.attr('ageid'),
                    year: $e.attr('year'),
                    quarter: $e.attr('quarter'),
                    month: $e.attr('month'),
                    yearRange: $e.attr('yearrange'),
                },
                success: function (data) {
                    $e.parent().html(laterString);
                },
                error: function (xhr, error) {
                    showSimpleMessagePopUp("<h3>Save failed</h3><p>The time period was not changed. You probably do not have permission to change data in this profile.</p>");
                }
            });
        });
    });
}

function indicatorAction(ajaxAction, popupId, action) {
    var selectedIndicators = [];

    $('.indicator-check-box:checked').each(function () {
        selectedIndicators.push($(this).val());
    });

    if (selectedIndicators.length > 0) {
        loading();

        $.ajax({
            cache: false,
            type: 'Get',
            url: ajaxAction,
            data: getIndicatorSpecifyingData(selectedIndicators),
            traditional: true,
            dataType: 'html',
            success: function (data) {
                var h = data;
                lightbox.show(h, 20, 300, 700);
                $(popupId).show();
                loadingFinished();
            },
            error: function (xhr, error) {
                loadingFinished();
            }
        });
    } else {
        showSimpleMessagePopUp('Please select some indicators to ' + action);
    }
}

