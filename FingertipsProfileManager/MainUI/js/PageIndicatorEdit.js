function loading() {
    $('body').addClass('loading');
}

// Also see LookUps.Frequencies in C#
FREQUENCIES = {
    Annual: '1',
    Quarterly: '2',
    Monthly: '3'
};

function setInputNotSelected($menu) {
    $menu.removeClass('dropdown-selected').addClass('dropdown-not-selected');
}

function setInputSelected($menu) {
    $menu.removeClass('dropdown-not-selected').addClass('dropdown-selected');
}

function showHideComparatorConfidence() {

    var comparatorConfidenceDiv = $('#comparatorConfidenceDiv'),
        comparatorConfidence = $('#ComparatorConfidence');

    if ($('#ComparatorMethodId').val() === '-1'/*No comparison*/) {
        setInputSelected(comparatorConfidence);
        comparatorConfidenceDiv.hide();
        comparatorConfidence[0].selectedIndex = 0;
    } else {
        comparatorConfidenceDiv.show();
        if (comparatorConfidence.val() === '-1') {
            setInputNotSelected(comparatorConfidence);
        }
    }
};

function checkMandatoryFields() {
    var v1 = $('#v1').val(),
        v2 = $('#v2').val(),
        v3 = $('#v3').val(),
        v6 = $('#v6').val();
    if (v1 === '' || v2 === '' || v3 === '' || v6 === '') {
        return false;
    } else {
        return true;
    }
}

$(document).ready(function () {
    initShowEmptyMetadataFields();
    loadDefaultTextMetadata();
    registerReloadPopUpDomains();

    var $selectedProfile = $('#selectedProfile');
    $selectedProfile.live('change', function () {
        reloadDomains($selectedProfile);
    });

    var $saveButton = $('#save'),
        $saveNewButton = $('.save-new-indicator');

    $saveButton.click(function () {
        updateShowSpineChartValue();
        if (checkFieldLength()) {
            if (checkMandatoryFields()) {
                loading();
                $('.create-indicator-dropdown').removeAttr('disabled');
                $('.indicator-text').removeAttr('disabled');
                $('form#IndicatorEditForm').submit();
                configurePermissions();
            } else {
                showSimpleMessagePopUp('Please complete all the required fields');
            }
        }
    });

    $saveNewButton.removeAttr('href');

    $saveNewButton.click(function () {
        updateShowSpineChartValue();
        if (checkFieldLength()) {
            if ($(this).hasClass('save-required')) {
                var selectedProfile = $selectedProfile.find(':selected')[0].text;
                $('#popupDiv #popup-profile-confirm-label').text(selectedProfile);
                var selectedDomain = $('#popup-domain-confirm-label').text($('#selectedDomain').find(':selected').text())[0].innerText;
                $('#popupDiv #popup-domain-confirm-label').text(selectedDomain);
                lightbox.show($('#popupDiv').html(), 250, 300, 600);
            } else {
                showSimpleMessagePopUp('Please complete all the required fields');
            }
        }
    });

    $('#saveAs').live('click', function () {
        lightbox.show($('#confirmNewFromOld').html(), 300, 300, 700);
    });

    // Called when any drop down is changed
    $('.create-indicator-dropdown, .save-as-indicator-dropdown').change(function () {

        var $menu = $(this);

        if (!$menu.hasClass('ignore-validation')) {
            if (isPleaseSelectSelected($menu)) {
                setInputNotSelected($menu);
            } else {
                setInputSelected($menu);
            }
        }

        configureSaveButton();
        checkIsReadOnly();
    });

    $('.return-url').click(function () {
        $('body').addClass('loading');
    });

    $('.year-Range').keydown(function (e) {

        var keyCode = e.keyCode;

        if (keyCode == 8 || keyCode == 9 || keyCode == 46) {
            return true;
        }

        if ((keyCode > 95 && keyCode < 106) || (keyCode > 46 && keyCode < 59)) {
            if ($(this).val().length > 3) {
                return false;
            } else {
                return true;
            }
        }

        return false;
    });

    $('.year-Range').blur(function () {
        if ($(this).val().length < 4) {
            $(this).addClass('dropdown-not-selected');
        }
        checkAllMandatoryFields();
    });

    $('#selectedFrequency').change(function () {
        showCorrectFrequency($(this).val());
        checkAllMandatoryFields();
    });

    $('#ComparatorMethodId').change(function () {
        showHideComparatorConfidence();
        checkAllMandatoryFields();
    });

    var showCorrectFrequency = function (frequency) {

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
    };

    var checkAllMandatoryFields = function () {
        configureSaveButton();
        checkIsReadOnly();
    };

    var checkIsReadOnly = function () {
        var isReadOnly = $('#isReadOnly');
        if (isReadOnly.val() == 'True') {
            $saveButton.hide();
        }
    };

    var validateGenericDropdowns = function () {
        var dropdownsToValidate = $('.validate-required');
        for (var i in dropdownsToValidate) {
            var $menu = $(dropdownsToValidate[i]);
            if (isPleaseSelectSelected($menu)) {

                setInputNotSelected($menu);

                $saveButton.addClass('save-new-indicator');
                $saveButton.removeClass('save-required');
                $('.save-new-indicator').removeAttr('href');
            }
        }

        showHideComparatorConfidence();

        configurePermissions();
    };

    var configurePermissions = function () {
        if ($('#doesUserHaveWritePermission').val() == 'False') {
            //Disable all three tab elements - The user doesn't have write permissions to this profile
            $('.create-indicator-dropdown').attr('disabled', 'True');
            $('.indicator-text').attr('disabled', 'True');
            $saveButton.hide();
        } else if ($('#doesProfileOwnIndicator').val() == 'False' && $('#doesUserHaveWritePermission').val() == 'True') {
            // The user has write permission to this profile but the selected profile 
            // doesn't own the indicator. Only allow text value override and access to the grouping tab elements
            $('#tabs-1 .indicator-text').attr('disabled', 'True');
            $('#tabs-1 .overridden').removeAttr('disabled');
            $('#tabs-2 .create-indicator-dropdown').attr('disabled', 'True');
            $('#tabs-2 .indicator-text').attr('disabled', 'True');
        }
    };

    var startQuarter, endQuarter, startMonth, endMonth;

    var setPeriodRanges = function (frequency) {

        startQuarter = '-1';
        endQuarter = '-1';
        startMonth = '-1';
        endMonth = '-1';

        if (frequency == FREQUENCIES.Quarterly) {
            startQuarter = $('#startQuarterRange').val();
            endQuarter = $('#endQuarterRange').val();
        }

        if (frequency == FREQUENCIES.Monthly) {
            startMonth = $('#startMonthRange').val();
            endMonth = $('#endMonthRange').val();
        }
    };

    function initialiseMtvValues() {
        var mtvValues = '';
        var mtvFields = $('.mtvArea');

        for (var i = 0; i < mtvFields.length; i++) {
            var mtvfield = mtvFields[i];

            mtvValues += mtvfield.id.replace('v', '') + '¬' + encodeURI(mtvfield.value) + '¬';
        }

        $('#userMTVChanges').val(mtvValues.slice(0, -1));
    }

    $('#confirm-ok').live('click', function () {
        updateShowSpineChartValue();
        // Save new indicator
        var frequency = $('#selectedFrequency').val();
        setPeriodRanges(frequency);

        $.ajax({
            type: 'post',
            url: '/SaveNewIndicator',
            data: {
                //Tab 1
                userMTVChanges: $('#userMTVChanges').val(),
                selectedProfileId: $('#selectedProfile').val(),
                selectedDomain: $('#selectedDomain').val(),

                //Tab 2                    
                selectedValueType: $('#selectedValueType').val(),
                selectedCIMethodType: $('#selectedCIMethodType').val(),
                selectedCIConfidenceLevel: $('#selectedCIConfidenceLevel').val(),
                selectedPolarityType: $('#selectedPolarityType').val(),
                selectedUnitType: $('#selectedUnitType').val(),
                selectedDenominatorType: $('#selectedDenominatorType').val(),
                selectedYearType: $('#selectedYearType').val(),
                selectedDecimalPlaces: $('#selectedDecimalPlaces').val(),
                selectedTargetId: $('#selectedTargetId').val(),

                //Tab 3
                selectedAreaType: $('#selectedAreaType').val(),
                selectedSex: $('#selectedSex').val(),
                selectedAge: $('#selectedAge').val(),
                selectedComparator: $('#selectedComparator').val(),
                selectedComparatorMethod: $('#selectedComparatorMethod').val(),
                selectedComparatorConfidence: $('#selectedComparatorConfidence').val(),
                selectedYearRange: $('#selectedYearRange').val(),
                timeFrequency: frequency,
                startYear: $('#startYear').val(),
                endYear: $('#endYear').val(),
                startQuarterRange: startQuarter,
                endQuarterRange: endQuarter,
                startMonthRange: startMonth,
                endMonthRange: endMonth
            },
            success: function (data) {
                $('#confirm-new-indicator').hide();
                $('#indicator-created').show();
                $('#popup-domain-new-indicator-label').text(data);
            },
            error: function (xhr, error) {
                alert('An error has occurred - Please contact the site Administrator.');
            }
        });
    });

    $('#confirmOldFromNew').live('click', function () {
        updateShowSpineChartValue();
        // Save as
        var frequency = $('#selectedFrequency').val();
        setPeriodRanges(frequency);

        initialiseMtvValues();
        $.ajax({
            type: 'post',
            dataType: 'json',
            url: '/CreateNewFromOld',
            data: {
                //Tab 1
                userMTVChanges: $('#userMTVChanges').val(),
                selectedProfileId: $('#selectedProfileId').val(),
                selectedDomain: $('#selectedDomainId').val(),

                //Tab 2                    
                selectedValueType: $('#valueTypeId').val(),
                selectedCIMethodType: $('#CIMethodID').val(),
                selectedCIConfidenceLevel: $('#CIComparatorConfidence').val(),
                selectedPolarityType: $('#PolarityId').val(),
                selectedUnitType: $('#UnitId').val(),
                selectedDenominatorType: $('#DenominatorTypeID').val(),
                selectedYearType: $('#YearTypeId').val(),
                selectedDecimalPlaces: $('#DecimalPlaces').val(),
                selectedTargetId: $('#TargetId').val(),

                //Tab 3
                selectedAreaType: $('#AreaTypeId').val(),
                selectedSex: $('#SexId').val(),
                selectedAge: $('#AgeId').val(),
                selectedComparator: $('#ComparatorId').val(),
                selectedComparatorMethod: $('#ComparatorMethodId').val(),
                selectedComparatorConfidence: $('#ComparatorConfidence').val(),
                selectedYearRange: $('#YearRange').val(),
                timeFrequency: $('#selectedFrequency').val(),
                startYear: $('#BaselineYear').val(),
                endYear: $('#DatapointYear').val(),
                startQuarterRange: startQuarter,
                endQuarterRange: endQuarter,
                startMonthRange: startMonth,
                endMonthRange: endMonth
            },
            success: function (data) {
                $('#newIndicatorId').html(data[2]);
                $('#redirectUrl').val(data[0]);
                $('#areaType').val(data[1]);
                $('#domainSequence').val(data[3]);
                lightbox.show($('#newFromOldSuccess').html(), 300, 300, 700);
            },
            error: function (xhr, error) {
                alert('An error has occurred - Please contact the site Administrator.');
            }
        });
    });

    $('#created-ok').click(function () {
        $.ajax({
            url: 'profiles'
        });
    });

    showCorrectFrequency($('#selectedFrequency').val());
    validateGenericDropdowns();

    $('#AgeId,#selectedAge').chosen({ search_contains: true });
});

function isPleaseSelectSelected($menu) {
    return $menu.find('option:selected').text().indexOf('Please ') === 0;
}

function toggleShowEmpty() {
    areEmptyShown = !areEmptyShown;
    $.cookie('areEmptyShown', areEmptyShown.toString(), { expires: 1 });
    setUpEmptyFields();
}

function setUpEmptyFields() {

    if (areEmptyShown) {
        showEmpty.html('Hide empty fields');
        $('.mtvArea').parent().parent().show();
    } else {
        showEmpty.html('Show empty fields');
        var textAreas = $('.mtvArea');
        for (var i in textAreas) {
            var jq = $(textAreas[i]);
            var val = jq.attr('value');
            if (val === '') {
                jq.parent().parent().hide();
            }
        }
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

function setuserMTVChanges() {

    var val = [];
    for (var i in changes) {

        var id = i;
        if (isPropertyOverridden(i)) {
            id += 'o';
        }

        val.push(id);
        val.push(changes[i]);
    }

    $('#userMTVChanges').val(val.join('¬'));
}

function dropdownFocus(e) {
    var jq = $(e);

    var startdropdownValue = jq.attr('start-dropdown-value');
    if (typeof (startdropdownValue) === 'undefined') {
        jq.attr('start-dropdown-value', jq.val());
    }

    jq.addClass(ENTERED);
}

function dropdownChanged(e) {

    var $menu = $(e);
    var startDropdownValue = $menu.attr('start-dropdown-value');

    var dropdownValue = $menu.val();
    var key = $menu.attr('id').replace('v', ''); // Why do this???
    var profileName = $('.profile-title').text();
    var domainName = $('#domainName').val();
    var indicatorName = $('#indicatorName').val();

    if (dropdownValue !== startDropdownValue) {
        // Changed
        otherChanges[key] = key + ' changed from ' + encodeURI(startDropdownValue) +
            ' to ' + encodeURI(dropdownValue) + ' in ' + profileName + ' - (' + domainName + ' / ' + indicatorName + ')';
        $menu.addClass(CHANGED);
    } else {
        // Text unaltered
        $menu.removeClass(CHANGED);
        delete otherChanges[key];
    }

    setuserOtherChanges();
}

function setuserOtherChanges() {

    var val = [];
    for (var i in otherChanges) {
        val.push(otherChanges[i]);
    }

    $('#userOtherChanges').val(val.join('¬'));
}

function checkIsReadOnly() {
    var saveButton = $('#save'),
        isReadOnly = $('#isReadOnly');
    if (isReadOnly.val() == 'True') {
        saveButton.hide();
    }
};

function textKeyDown(e) {

    if (!isSaveRed) {
        saveRequired();
    }

    if (!wasKeyPressForCurrentProperty) {
        wasKeyPressForCurrentProperty = true;
        var jq = $(e);
        if (!jq.hasClass(CHANGED)) {
            jq.addClass(CHANGED);
        }
    }

    checkIsReadOnly();
}

function checkIfSaveRequired() {
    var saveButton = $('#save');
    if (!_.size(changes)) {
        saveButton.removeClass('save-required').hide();
        isSaveRed = false;
    }
}

function saveRequired() {
    isSaveRed = true;
    var saveButton = $('#save');

    if ($('.mandatory-input').length == 0) {
        saveButton.show().addClass('save-required');
    }
}

function isPropertyOverridden(id) {
    return _.any(overridden, function (overriddenId) { return id == overriddenId });
}

function override(propertyId) {
    if (!isPropertyOverridden(propertyId)) {
        overridden.push(propertyId);
        var jq = $('#override' + propertyId);
        // change the css for text field
        $('#v' + propertyId).addClass('overridden').removeAttr('disabled').select().focus();

        var parent = jq.parent();
        // remove calling DOM
        $('#override' + propertyId).remove();

        // add clear override 
        var clearOverride = ' <span id="override-v' + propertyId +
            '" class="overridden-label" title="The original metadata has been overridden" onclick="clearOverride(this)"><strong>Clear Override</strong></span>';
        parent.prepend(clearOverride);
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

        configureSaveButton();
        checkIfSaveRequired();
        setuserMTVChanges();
        checkIsReadOnly();
    }
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

function reloadDomains($selectedProfile) {
    var $selectedDomain = $('#selectedDomain');
    $.ajax({
        type: 'post',
        url: '/reloadDomains',
        data: { selectedProfile: $selectedProfile.val() },
        success: function (data) {
            $selectedDomain.empty();
            setInputSelected($selectedProfile);
            setInputSelected($selectedDomain);
            $.each(data, function (key, value) {
                $selectedDomain.append($('<option value="' + value.Text + '"></option>').val(value.Value).html(value.Text));
            });
        },
        error: function () { }
    });
}

function checkFieldLength() {

    var properties = [
        [1, 'Indicator name', 255],
        [2, 'Indicator full name', 500],
        [25, 'Data Quality', 1]
    ];

    for (var i in properties) {
        var property = properties[i];
        var id = property[0];
        var maxLength = property[2];
        if ($('#v' + id).val().length > maxLength) {
            var propertyName = property[1];
            showSimpleMessagePopUp(propertyName + ' cannot be more than ' + maxLength + ' characters');
            return false;
        }
    }

    return true;
}

function configureSaveButton() {

    var $saveButton = $('#save');
    var $saveNewButton = $('.save-new-indicator');
    var $notSelected = $('.dropdown-not-selected');

    if (($('.mandatory-input').length == $('.mandatory-success').length) &&
        $notSelected.length === 0) {
        $saveButton.show();
        $saveButton.addClass('save-required');
        $saveNewButton.attr('href', '#');
    } else {
        $saveButton.removeClass('save-required');
        $saveNewButton.removeAttr('href');
    }
}

function clearOverride(e) {
    var jq = $(e);
    var propertyId = jq.attr('id').split('-').pop();
    var propertyIdNumber = propertyId.substring(1, propertyId.length);
    var $property = $('#' + propertyId);

    var fieldText = indicatorDefaultMetadata[1 + parseInt(propertyIdNumber)];
    // add default property text 
    $property.val(fieldText);
    // remove the css class
    $property.removeClass('overridden');

    // make the save button red
    if (!isSaveRed) {
        saveRequired();
    }

    // add override link
    var parent = jq.parent();

    var overrideLink = '<a id="override' + propertyIdNumber +
        '" title="Override this metadata" href="javascript:override('
        + propertyIdNumber + ')">Override</a>';
    parent.prepend(overrideLink);

    // add changes to changes array
    changes[propertyIdNumber] = encodeURI($property.val());
    setuserMTVChanges();

    // check and remove from overridden array, if exists
    if (isPropertyOverridden(propertyIdNumber)) {
        var indexOfOverride = overridden.indexOf(propertyIdNumber);
        overridden.splice(indexOfOverride, 1);
    }

    // remove calling DOM object
    $('#' + jq.attr('id')).remove();
}

function loadDefaultTextMetadata() {
    var selectedIndicatorId = $('#indicatorId').val();

    // Indicator ID will not be defined if new indicator is being created
    if (!_.isUndefined(selectedIndicatorId)) {
        $.get('/indicator/metadata/' + selectedIndicatorId, function (data) {
            indicatorDefaultMetadata = data;
        });
    }
}

function updateShowSpineChartValue() {
    var $showSpineChart = $('#ShouldAlwaysShowSpineChart');
    var isChecked = $showSpineChart.is(':checked');
    if (isChecked) {
        $('#AlwaysShowSpineChart').val(true);
    } else {
        $('#AlwaysShowSpineChart').val(false);
    }
}

indicatorDefaultMetadata = [];
wasKeyPressForCurrentProperty = false;
isSaveRed = false;
overridden = [];
CHANGED = 'text-changed';
ENTERED = 'text-entered';
startText = null;
changes = {};
otherChanges = {};
NAME_LENGTH = 255;
LONG_NAME_LENGTH = 500;
DATA_QUALITY_LENGTH = 1;