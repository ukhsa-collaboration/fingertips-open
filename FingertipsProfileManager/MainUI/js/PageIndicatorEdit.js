var areEmptyShown,
    showEmpty;

// Also see LookUps.Frequencies in C#
FREQUENCIES = {
    Annual: '1',
    Quarterly: '2',
    Monthly: '3'
};

$(document).ready(function () {
    var $addIndicatorButton = $('#add-indicator'),
        $confirmAddIndicatorButton = $('#confirm-create-indicator'),
        $updateIndicatorButton = $('#update-indicator'),
        $copyIndicatorButton = $('#copy-indicator'),
        $confirmCopyIndicatorButton = $('#confirm-copy-indicator'),
        $cancelCopyIndicatorButton = $('#cancel-copy-indicator'),
        $backButton = $('#back'),
        $withdrawIndicatorsButton = $('#withdraw-indicators-button'),
        $rejectIndicatorsButton = $('#reject-indicators-button');


    // Add indicator
    $addIndicatorButton.click(function () {

        var validationResult = validate();
        if (validationResult.trim().length === 0) {
            $('#profile-name').text($('#UrlKey option:selected').text());
            $('#DestinationProfileUrlKey').val($('#UrlKey').val());
            $('#domain-name').text($("#Grouping_GroupId option:selected").text());

            lightbox.show($('#save-indicator-popup').html(), 300, 300, 700);
        } else {
            showValidationMessagePopUp(validationResult);
        }
    });

    // Add indicator confirmation
    $confirmAddIndicatorButton.live('click', function () {
        $('#DomainSequence').val(1);
        $('#create-indicator-form').submit();
    });

    // Update indicator
    $updateIndicatorButton.click(function () {

        enableDropdownsAndInputs();

        var validationResult = validate();
        if (validationResult.trim().length === 0) {
            $('#edit-indicator-form').submit();
        } else {
            showValidationMessagePopUp(validationResult);
        }
    });

    // Copy indicator
    $copyIndicatorButton.live('click', function () {
        lightbox.show($('#copy-indicator-popup').html(), 300, 300, 700);
    });

    // Copy indicator confirmation
    $confirmCopyIndicatorButton.live('click', function () {
        $('#CopyToProfileUrlKey').val($('#ProfileSelectedToCopy').val());

        enableDropdownsAndInputs();

        $('#edit-indicator-form').submit();
    });

    // Cancel copy indicator confirmation
    $cancelCopyIndicatorButton.live('click', function () {
        $('#CopyToProfileUrlKey').val('');
        $('#CopyToDomainId').val(0);

        lightbox.hide();
    });

    // Go back to previous page
    $backButton.click(function () {
        history.back();
    });

    // Withdraw indicator button
    $withdrawIndicatorsButton.click(function() {
        lightbox.show($('#delete-indicator-popup').html(), 300, 300, 700);
    });

    // Reject indicator button
    $rejectIndicatorsButton.click(function() {
        lightbox.show($('#delete-indicator-popup').html(), 300, 300, 700);
    });

    // Decide whether empty fields can be shown
    var isEditAction = $('#IsEditAction').val();
    if (isEditAction === "True") {
        // Configure permissions
        configurePermissions();

        areEmptyShown = false;
    } else {
        areEmptyShown = true;
    }

    // Called when any drop down is changed
    $('.indicator-dropdown, .save-as-indicator-dropdown').change(function () {
        var $menu = $(this);

        if (!$menu.hasClass('ignore-validation')) {
            if (isPleaseSelectSelected($menu)) {
                setInputNotSelected($menu);
            } else {
                setInputSelected($menu);
            }
        }
    });

    $('#selectedFrequency').change(function () {
        showCorrectFrequency($(this).val());
    });

    $('#ComparatorMethodId').change(function () {
        showHideComparatorConfidence();
    });

    var validateGenericDropdowns = function () {
        var dropdownsToValidate = $('.validate-required');

        $.each(dropdownsToValidate, function() {
            if (isPleaseSelectSelected($(this))) {
                setInputNotSelected($(this));
            }
        });

        showHideComparatorConfidence();
    };

    // Set chosen style for age selection drop down
    $('#Grouping_AgeId').chosen({ search_contains: true });

    // JQuery UI datepicker
    $('#LatestChangeTimestampOverride').datepicker({
        dateFormat: 'dd-mm-yy',
        onSelect:function(dateText, inst) {
            $('#IndicatorMetadata_LatestChangeTimestampOverride').val(dateText);
        }
    }).val();
    $('#NextReviewTimestamp').datepicker({
        dateFormat: 'dd-mm-yy',
        onSelect: function(dateText, inst) {
            $('#IndicatorMetadata_NextReviewTimestamp').val(dateText);
        }
    }).val();

    if ($('#ComparatorMethodId').val() === ComparatorMethodIds.Quintiles.toString()) {
        $('#PolarityId').children('option[value="99"]').hide();
    }

    showCorrectFrequency($('#selectedFrequency').val());

    // Initialise
    Initialise();

    // Apply Select2 styles for partition age ids, partition
    // sex ids and partition area type ids drop downs
    ApplySelect2Styles();

    // Sort indicator metadata text properties
    sortIndicatorMetadataTextProperties();

    // Validate drop downs
    validateGenericDropdowns();
});

function enableDropdownsAndInputs() {
    $('.indicator-dropdown').removeAttr('disabled');
    $('.indicator-text').removeAttr('disabled');
    $('.indicator-internal-metadata-text').removeAttr('disabled');
}

function configurePermissions() {
    var userWritePermission = $('#user-write-permission').val().toLowerCase(),
        profileOwnIndicator = $('#profile-own-indicator').val().toLowerCase();


    if (userWritePermission === 'false') {
        //Disable all three tab elements - The user doesn't have write permissions to this profile
        $('.indicator-dropdown').attr('disabled', true);
        $('.indicator-text').attr('disabled', true);
        $('.indicator-internal-metadata-text').attr('disabled', true);
        $('#update-indicator').hide();
    } else if (profileOwnIndicator === 'false' && userWritePermission === 'true') {
        // The user has write permission to this profile but the selected profile 
        // doesn't own the indicator. Only allow text value override and access to the grouping tab elements
        $('#tabs-1 .indicator-text').attr('disabled', true);
        $('#tabs-1 .overridden').removeAttr('disabled');
        $('#tabs-2 .indicator-dropdown').attr('disabled', true);
        $('#tabs-2 .indicator-text').attr('disabled', true);
        $('#tabs-4 .indicator-text').attr('disabled', true);
        $('#tabs-4 .overridden').removeAttr('disabled');
    }
}

function Initialise() {
    displayComparatorConfidence();
    triggerTimeSeriesEvent($('#Grouping_TimeSeries'));
    initShowEmptyMetadataFields();
    setUpEmptyFields();
}

function displayComparatorConfidence() {
    var comparatorMethodId = $('#Grouping_ComparatorMethodId').val(),
        $comparatorConfidenceDiv = $('#comparator-confidence'),
        $comparatorConfidenceDropdown = $('#Grouping_ComparatorConfidence');

    if (comparatorMethodId === ComparatorMethodIds.SpcForDsr.toString() ||
        comparatorMethodId === ComparatorMethodIds.SpcForProportions.toString()) {
        $comparatorConfidenceDiv.show();
    } else {
        $comparatorConfidenceDiv.hide();
        $comparatorConfidenceDropdown.val('-1');
    }
}

function ApplySelect2Styles() {
    // Set partition age ids
    SetPartitionAgeIds();

    // Set partition sex ids
    SetPartitionSexIds();

    // Set partition areatype ids
    SetPartitionAreaTypeIds();

    // Select2 style
    ApplySelect2Style('partition-age-ids');
    ApplySelect2Style('partition-sex-ids');
    ApplySelect2Style('partition-areatype-ids');
}

function SetPartitionAgeIds() {
    var partitionAgeIds = $('#hdn-partition-age-ids').val();
    if (partitionAgeIds) {
        $.each(partitionAgeIds.split(","), function (i, e) {
            $("#PartitionAgeIds option[value='" + e + "']").prop("selected", true);
            $('#PartitionAgeIds').trigger('change.select2');
        });
    }
}

function SetPartitionSexIds() {
    var partitionSexIds = $('#hdn-partition-sex-ids').val();
    if (partitionSexIds) {
        $.each(partitionSexIds.split(","), function (i, e) {
            $("#PartitionSexIds option[value='" + e + "']").prop("selected", true);
            $('#PartitionSexIds').trigger('change.select2');
        });
    }
}

function SetPartitionAreaTypeIds() {
    var partitionAreaTypeIds = $('#hdn-partition-areatype-ids').val();
    if (partitionAreaTypeIds) {
        $.each(partitionAreaTypeIds.split(","), function (i, e) {
            $("#PartitionAreaTypeIds option[value='" + e + "']").prop("selected", true);
            $('#PartitionAreaTypeIds').trigger('change.select2');
        });
    }
}

function ApplySelect2Style(element) {
    if ($('.' + element).length) {
        $('.' + element).select2();
    }

    if ($('#' + element).length) {
        $('#' + element).select2();
    }
}

function isPleaseSelectSelected($menu) {
    return $menu.find('option:selected').text().indexOf('Please ') === 0;
}

function toggleShowEmpty() {
    areEmptyShown = !areEmptyShown;
    $.cookie('areEmptyShown', areEmptyShown.toString(), { expires: 1 });
    setUpEmptyFields();
}

function setUpEmptyFields() {
    var textAreas = $('.indicator-text');

    if (areEmptyShown) {
        showEmpty.html('Hide empty fields');
        textAreas.parent().parent().show();
    } else {
        showEmpty.html('Show empty fields');

        $.each(textAreas, function(){
            var $textArea = $(this);
            var val = $textArea.attr('value');
            if (val === '' &&
                $textArea.parent().parent().length === 1/*so that all rows are not hidden*/) {
                // Hide the row
                $textArea.parent().parent().hide();
            }
        });
    }
}

function initShowEmptyMetadataFields() {
    showEmpty = $('#show-empty');
    if (showEmpty.length) {
        areEmptyShown = $.cookie('areEmptyShown');
        if (typeof (areEmptyShown) === 'undefined') {
            areEmptyShown = true;
        } else {
            areEmptyShown = areEmptyShown === 'true';
        }
        setUpEmptyFields();
    }
}

function dropdownChanged(e) {

    var $menu = $(e);
    var startDropdownValue = $menu.attr('start-dropdown-value');

    var dropdownValue = $menu.val();
    var $polarity = $('#PolarityId');

    if (dropdownValue !== startDropdownValue) {
        $menu.addClass(CHANGED);
    } else {
        $menu.removeClass(CHANGED);
    }

    if ($('#ComparatorMethodId').val() === ComparatorMethodIds.Quintiles.toString()) {
        if ($polarity.val() === PolarityIds.UseBlues.toString()) {
            $polarity.val(PolarityIds.NotApplicable);
        }
        $polarity.children('option[value="99"]').hide();
        $polarity.children('option[value="-1"]').text("No judgement");
        $polarity.children('option[value="1"]').text("High is good");
        $polarity.children('option[value="0"]').text("Low is good");
    } else {
        $polarity.children('option[value="99"]').show();
        $polarity.children('option[value="-1"]').text("Not applicable");
        $polarity.children('option[value="1"]').text("RAG - High is good");
        $polarity.children('option[value="0"]').text("RAG - Low is good");
    }
}

function dropdownChanged_IndicatorNew(e) {

    var $menu = $(e),
        $polarity = $('#selectedPolarityType');

    if ($menu.val() === ComparatorMethodIds.Quintiles.toString()) {
        $polarity.children('option[value="99"]').hide();
        $polarity.children('option[value="-1"]').text("No judgement");
        $polarity.children('option[value="1"]').text("High is good");
        $polarity.children('option[value="0"]').text("Low is good");
    } else {
        $polarity.children('option[value="99"]').show();
        $polarity.children('option[value="-1"]').text("Not applicable");
        $polarity.children('option[value="1"]').text("RAG - High is good");
        $polarity.children('option[value="0"]').text("RAG - Low is good");
    }
}

function showEditor(object) {
    lightbox.show($('#editMetaData').html(), 250, 300, 600);
    var metaDataTextArea = $('#metaDataText');
    metaDataTextArea.val(object.val());
    metaDataTextArea.focus();

    $('#textValueId').val(object.attr('id'));

    tinymce.init({
        verify_html: false,
        height: 300,
        width: '100%',
        menubar: false,
        selector: '#metaDataText',
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

function triggerTimeSeriesEvent(e) {
    var timeSeriesValue = $(e).val();

    switch (timeSeriesValue) {
    case '1':
        $('#quarterly-range-selection').hide();
        $('#monthly-range-selection').hide();

        setInputSelected($(this));
        setInputSelected($('#startQuarterRange'));
        setInputSelected($('#endQuarterRange'));
        setInputSelected($('#startMonthRange'));
        setInputSelected($('#endMonthRange'));
        break;
    case '2':
        $('#monthly-range-selection').hide();
        $('#quarterly-range-selection').show();

        setInputSelected($('#startMonthRange'));
        setInputSelected($('#endMonthRange'));
        break;
    case '3':
        $('#quarterly-range-selection').hide();
        $('#monthly-range-selection').show();

        setInputSelected($('#startQuarterRange'));
        setInputSelected($('#endQuarterRange'));
        break;
    case '-1':
        $('#quarterly-range-selection').hide();
        $('#monthly-range-selection').hide();
        break;
    default:
    }
}

function triggerProfileChangeEvent(e) {
    var profileUrlKey = $(e).val(),
        domain;

    if (e.name === "UrlKey") {
        domain = $('#Grouping_GroupId');
    } else {
        domain = $('#DomainSelectedToCopy');
        $('#CopyToProfileUrlKey').val(profileUrlKey);
    }
    
    domain.empty();

    $.ajax({
        type: 'post',
        url: '/profiles-and-indicators/reload-domains',
        data: { selectedProfile: profileUrlKey },
        success: function(data) {
            setInputSelected($(e));
            setInputSelected(domain);
            $.each(data,
                function(key, value) {
                    domain.append($('<option value="' + value.Text + '"></option>').val(value.Value).html(value.Text));
                });

            triggerDomainSelectedToCopyEvent(domain);
        },
        error: function() {}
    });
}

function triggerDomainChangeEvent(e) {
    $('#DomainSequence').val($(e).prop('selectedIndex') + 1);
}

function triggerDomainSelectedToCopyEvent(e) {
    var domain = $(e).val();
    $('#CopyToDomainId').val(domain);
}

function triggerComparatorMethodChangedEvent(e) {
    var comparatorMethodId = $(e).val(),
        polarityIdElement = $("#Grouping_PolarityId"),
        polarityId = polarityIdElement.val();

    displayComparatorConfidence();

    if (comparatorMethodId === ComparatorMethodIds.Quintiles.toString()) {
        if (polarityId === PolarityIds.UseBlues.toString()) {
            polarityIdElement.val(PolarityIds.NotApplicable);
        }
        polarityIdElement.children('option[value="99"]').hide();
        polarityIdElement.children('option[value="-1"]').text("No judgement");
        polarityIdElement.children('option[value="1"]').text("High is good");
        polarityIdElement.children('option[value="0"]').text("Low is good");
    } else {
        polarityIdElement.children('option[value="99"]').show();
        polarityIdElement.children('option[value="-1"]').text("Not applicable");
        polarityIdElement.children('option[value="1"]').text("RAG - High is good");
        polarityIdElement.children('option[value="0"]').text("RAG - Low is good");
    }
}

function loading() {
    $('body').addClass('loading');
}

function setInputNotSelected($menu) {
    $menu.removeClass('dropdown-selected').addClass('dropdown-not-selected');
}

function setInputSelected($menu) {
    $menu.removeClass('dropdown-not-selected').addClass('dropdown-selected');
}

function showHideComparatorConfidence() {
    var comparatorMethodId = parseInt($('#ComparatorMethodId').val());

    // Display comparator confidence
    var $comparatorConfidenceDiv = $('#comparatorConfidenceDiv'),
        $comparatorConfidence = $('#ComparatorConfidence');

    var methodsThatDontNeedConfidenceLevel = [
        ComparatorMethodIds.Undefined, ComparatorMethodIds.SuicidePlanDefined,
        ComparatorMethodIds.Quartiles, ComparatorMethodIds.Quintiles,
        ComparatorMethodIds.SingleOverlappingCIsForTwoCILevels];

    if (_.contains(methodsThatDontNeedConfidenceLevel, comparatorMethodId)) {
        // User does not need to select comparator confidence
        setInputSelected($comparatorConfidence);
        $comparatorConfidenceDiv.hide();
        $comparatorConfidence[0].selectedIndex = 0;
    } else {
        $comparatorConfidenceDiv.show();
        if ($comparatorConfidence.val() === '-1') {
            setInputNotSelected($comparatorConfidence);
        }
    }

    // Display polarity
    var $polarity = $('#polarity-container');

    // Polarity might be needed for recent trends even if no comparison of significances
    var methodsThatDontNeedPolarity = [
        ComparatorMethodIds.SuicidePlanDefined,
        ComparatorMethodIds.Quartiles];

    if (_.contains(methodsThatDontNeedPolarity, comparatorMethodId)) {
        // User does not need to select polarity
        $polarity.hide();

        // Select "Not applicable"
        $('#PolarityId option[value="-1"]').prop('selected', true);
    } else {

        $polarity.show();
    }
}

function showCorrectFrequency(frequency) {
    switch (frequency) {
        case '1':
            $('#quarterly-range-selection').hide();
            $('#monthly-range-selection').hide();

            setInputSelected($(this));
            setInputSelected($('#startQuarterRange'));
            setInputSelected($('#endQuarterRange'));
            setInputSelected($('#startMonthRange'));
            setInputSelected($('#endMonthRange'));
            break;
        case '2':
            $('#monthly-range-selection').hide();
            $('#quarterly-range-selection').show();

            setInputSelected($('#startMonthRange'));
            setInputSelected($('#endMonthRange'));
            break;
        case '3':
            $('#quarterly-range-selection').hide();
            $('#monthly-range-selection').show();

            setInputSelected($('#startQuarterRange'));
            setInputSelected($('#endQuarterRange'));
            break;
        case '-1':
            $('#quarterly-range-selection').hide();
            $('#monthly-range-selection').hide();
            break;
        default:
    }
}

function validate() {
    var message = "";

    message = message + checkFieldLength();
    message = message + checkMandatoryFields();

    if (message.trim().length > 0) {
        message =
            "<div style='float: left; margin-left: 5px; font-size: 16px; font-weight: bold;'>Please address the validation error(s) below:</div><br><br><br>" +
            message;
    }

    return message;
}

function checkFieldLength() {
    var validationMessage = "",
        properties = [
            ['IndicatorMetadataTextValue_Name', 'Indicator name', 255],
            ['IndicatorMetadataTextValue_DataQuality', 'Data Quality', 1]
        ];

    for (var i in properties) {
        var property = properties[i];
        var id = property[0];
        var maxLength = property[2];
        if ($('#' + id).val().length > maxLength) {
            var propertyName = property[1];
            validationMessage = validationMessage + "<div style='float: left; margin-left: 5px; font-size: 14px;'>" +
                propertyName + ' cannot be more than ' + maxLength;

            if (propertyName === "Data Quality") {
                validationMessage = validationMessage + ' character.';
            } else {
                validationMessage = validationMessage + ' characters.';
            }

            validationMessage = validationMessage + '</div><br><br>';
        }
    }

    return validationMessage;
}

function checkMandatoryFields() {
    var validationMessage = "",
        isEditAction = $('#IsEditAction').val(),
        indicatorName = $('#IndicatorMetadataTextValue_Name').val().trim(),
        definition = $('#IndicatorMetadataTextValue_Definition').val().trim(),
        dataSource = $('#IndicatorMetadataTextValue_DataSource').val().trim(),
        valueType = $('#IndicatorMetadata_ValueTypeId').val(),
        ciMethod = $('#IndicatorMetadata_CIMethodId').val(),
        unit = $('#IndicatorMetadata_UnitId').val(),
        denominatorType = $('#IndicatorMetadata_DenominatorTypeId').val(),
        yearType = $('#IndicatorMetadata_YearTypeId').val(),
        profile = $('#UrlKey').val(),
        domain = $('#Grouping_GroupId').val(),
        areaType = $('#Grouping_AreaTypeId').val(),
        sex = $('#Grouping_SexId').val(),
        comparatorMethod = $('#Grouping_ComparatorMethodId').val(),
        comparatorConfidence = $('Grouping_ComparatorConfidence').val(),
        polarity = $('#Grouping_PolarityId').val(),
        yearRange = $('#Grouping_YearRange').val(),
        timeSeries = $('#Grouping_TimeSeries').val(),
        startYear = $('#Grouping_BaselineYear').val(),
        endYear = $('#Grouping_DataPointYear').val();

    // Tab1
    if (indicatorName.length === 0 ||
        definition.length === 0 ||
        dataSource.length === 0) {

        validationMessage = getValidationMessage(validationMessage,
            "Complete the mandatory fields in <b>Step 1 - Indicator Metadata Text</b> tab.");
    }

    // Tab2
    if (valueType === "-1" ||
        ciMethod === "-1" ||
        unit === "-1" ||
        denominatorType === "-1" ||
        yearType === "-1") {

        validationMessage = getValidationMessage(validationMessage,
            "Select valid option from the mandatory dropdowns in <b>Step 2 - Indicator Metadata Other</b> tab.");
    }

    // Tab 3
    if ((!isEditAction && (profile === "-1" || domain === "-1")) ||
        areaType === "-1" ||
        sex === "-99") {

        validationMessage = getValidationMessage(validationMessage,
            "Select valid option from the mandatory dropdowns in <b>Step 3 - Profile Data Selection</b> tab.>");
    }

    // Tab 3: Time
    if (yearRange === "-1" ||
        timeSeries === "-1" ||
        startYear === "-1" ||
        endYear === "-1" ||
        startYear > endYear ||
        startYear < 1950 || endYear < 1950) {

        validationMessage = getValidationMessage(validationMessage,
            "Invalid time fields in <b>Step 3 - Profile Data Selection</b> tab");
    }

    return validationMessage;
}

function getValidationMessage(validationMessage, message) {
    return validationMessage +
        "<div style='float: left; margin-left: 5px; font-size: 14px;'>" +
        message +
        "</div><br><br>";
}

function textEnter(e) {
    wasKeyPressForCurrentProperty = false;
    var jq = $(e);

    var startText = jq.attr('start-text');
    if (typeof (startText) === 'undefined') {
        jq.attr('start-text', jq.val());
    }

    jq.addClass(ENTERED);

    if (jq.hasClass('html-allowed')) {
        showEditor(jq);

        $('#editor-done').click(function () {
            var originalField = $('#' + $('#textValueId').val());
            originalField.val(tinyMCE.activeEditor.getContent());

            wasKeyPressForCurrentProperty = true;
            textOut(originalField);
            lightbox.hide();
        });
    }
}

function textOut(e) {
    var jq = $(e);
    jq.removeClass(ENTERED);

    if (wasKeyPressForCurrentProperty) {
        var startText = jq.attr('start-text');

        var text = jq.val();
        var key = jq.attr('id').replace('v', '');

        if (text !== startText) {
            // Changed
            changes[key] = encodeURI(text);
            jq.addClass(CHANGED);
        } else {
            // Text unaltered
            jq.removeClass(CHANGED);
            delete changes[key];
        }

        if (jq.hasClass('mandatory-input')) {

            var cssClass = 'mandatory-success';
            if (text.length > 0) {
                jq.addClass(cssClass);
            } else {
                jq.removeClass(cssClass);
            }
        }
    }
}

function dropdownFocus(e) {
    var jq = $(e);

    var startdropdownValue = jq.attr('start-dropdown-value');
    if (typeof (startdropdownValue) === 'undefined') {
        jq.attr('start-dropdown-value', jq.val());
    }

    jq.addClass(ENTERED);
}

function override(item) {
    var overrideItem = $('#override-' + item);

    if (overrideItem.text() === 'Override') {
        overrideItem.text('Clear override');
        overrideItem.attr('title', 'The original metadata has been overridden');

        if (item === 'ref-num') {
            $('#IndicatorMetadataTextValue_RefNum').addClass('overridden').removeAttr('disabled').select().focus();
        }

        if (item === 'specific-rationale') {
            $('#IndicatorMetadataTextValue_SpecificRationale').addClass('overridden').removeAttr('disabled').select().focus();
        }

    } else {
        overrideItem.text('Override');
        overrideItem.attr('title', 'Override this metadata');

        if (item === 'ref-num') {
            $('#IndicatorMetadataTextValue_RefNum').removeClass('overridden').attr('disabled', 'true');
        }

        if (item === 'specific-rationale') {
            $('#IndicatorMetadataTextValue_SpecificRationale').removeClass('overridden').attr('disabled', 'true');
        }
    }
}

function showValidationMessagePopUp(html) {

    lightbox.show('<div id="simple-message-popup">' + html + '<br><input class="medium-button" type="button" onclick="lightbox.hide()"  value="OK" /></div>',
        250/*top*/, 0/*left*/, 652/*width*/);
}

function sortIndicatorMetadataTextProperties() {
    var rows = $('#text-properties tbody  tr').get();

    rows.sort(function (a, b) {

        var A = $(a).attr("id");
        if (A !== undefined) {
            A = Number(A.replace("row-", ""));
        }

        var B = $(b).attr("id");
        if (B !== undefined) {
            B = Number(B.replace("row-", ""));
        }

        if (A < B) {
            return -1;
        }

        if (A > B) {
            return 1;
        }

        return 0;

    });

    $.each(rows, function (index, row) {
        $('#text-properties').children('tbody').append(row);
    });
}

indicatorDefaultMetadata = [];
wasKeyPressForCurrentProperty = false;
isSaveRed = false;
overridden = [];
CHANGED = 'text-changed';
ENTERED = 'text-entered';
startText = null;
changes = {};
NAME_LENGTH = 255;
LONG_NAME_LENGTH = 500;
DATA_QUALITY_LENGTH = 1;