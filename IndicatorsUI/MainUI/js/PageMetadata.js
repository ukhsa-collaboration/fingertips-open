'use strict';

/**
* Defined in PageMetadata.js
* @module metadata
*/

var metadata = {};

function displayMetadata(indicatorMetadata, root) {

    var metadata = getMetadataHtml(indicatorMetadata, root);

    var html = metadata.html;
    var $box = $('#metadataBox');
    if ($box.length) {
        // Practice profiles
        $box.html(html);
    } else {
        // Fingertips
        pages.getContainerJq().html(html);
    }

    labelAsync.populate(metadata.labelArgs);
};

var metadataState = {

    // Stored to facilitate label callback 
    yearTypeId: null,

    _displayedKey: '',

    redisplay: function (rootIndex, sid) {
        var key = getKey(rootIndex, sid);
        return key !== this._displayedKey;
    },

    setDisplayedKey: function (rootIndex, sid) {
        this._displayedKey = getKey(rootIndex, sid);
    }
};

var labelAsync = (function () {

    var _getLabel = function (labelArgs, key, cacheKey) {
        if (labelArgs[key]) {
            ajaxGet('api/' + key.replace('-', '_') + '?', 'id=' + labelArgs[key],
                function (obj) {
                    if (isDefined(obj)) {
                        loaded[cacheKey][obj.Id] = obj.Name;
                        $('#' + key + '-label').html(obj.Name);
                    }
                });
        }
    }

    return {

        getLabel: function (id, loaded, arg, labelArgs) {

            if (id <= 0) {
                return NOT_APPLICABLE;
            }

            // Label has already been retrieved by AJAX and is cached
            if (loaded.hasOwnProperty(id)) {
                return loaded[id];
            }

            labelArgs[arg] = id;
            return getLabelSpan(arg);
        },

        populate: function (labelArgs) {
            _getLabel(labelArgs, 'comparator-method', 'comparatorMethods');
        }
    }
})();


function addMetadataRow(rows, textMetadata, property) {

    var columnName = property.ColumnName;

    if (textMetadata !== null && textMetadata.hasOwnProperty(columnName)) {
        var text = textMetadata[columnName];

        if (isDefined(text) && !String.isNullOrEmpty(text)) {

            if ((columnName === 'DataQuality') && showDataQuality) {
                // Add data quality flags instead of number
                var dataQualityCount = parseInt(text);
                var displayText = getIndicatorDataQualityHtml(text) + ' ' + getIndicatorDataQualityTooltipText(dataQualityCount);
            } else {
                displayText = text;
            }
            rows.push([property.DisplayName, displayText]);
        }
    }
}

function getMetadataProperties() {

    if (isDefined(loaded.indicatorProperties)) {
        ajaxMonitor.callCompleted();
    } else {

        ajaxGet('api/indicator_metadata_text_properties', '',
            function (obj) {

                loaded.indicatorProperties = obj;

                ajaxMonitor.callCompleted();
            });
    }
}

function goToMetadataPage(rootIndex) {
    if (!groupRoots.length) {
        // Search results empty
        noDataForAreaType();
    } else {

        lock();

        setPageMode(PAGE_MODES.METADATA);

        // Establish indicator
        if (!isDefined(rootIndex)) {
            rootIndex = getIndicatorIndex();
        }
        setIndicatorIndex(rootIndex);

        ajaxMonitor.setCalls(2);

        getIndicatorMetadata(FT.model.groupId);
        getMetadataProperties();

        ajaxMonitor.monitor(displayFingertipsMetadata);
    }
};

function getMetadataHtml(indicatorMetadata, root) {

    var propertyIndex, text, ageId, sexId, benchmarkingMethodId, benchmarkingSigLevel;

    var grouping = isDefined(root) && root.hasOwnProperty('Grouping') ?
        root.Grouping[0] :
        root; // Practice profiles

    if (isDefined(grouping)) {
        ageId = root.Age.Id,
        benchmarkingMethodId = grouping.MethodId,
        benchmarkingSigLevel = grouping.SigLevel,
        sexId = root.Sex.Id;
    } else {
        ageId = benchmarkingMethodId = benchmarkingSigLevel = sexId = null;
    }

    var labelArgs = {},
        rows = [],
        des = indicatorMetadata.Descriptive;

    // Initial indicator text properties
    var properties = loaded.indicatorProperties;
    for (propertyIndex = 0; propertyIndex < properties.length; propertyIndex++) {
        var property = properties[propertyIndex];
        if (property.Order > 59) {
            break;
        }

        // Do not dislay name as full name is displayed
        if ((property.ColumnName !== 'Name')) {
            addMetadataRow(rows, des, property);
        }
    }

    // Value type
    rows.push(['Value type', indicatorMetadata.ValueType.Name]);

    // Text - Methodology
    addMetadataRow(rows, des, properties[propertyIndex++]);

    // Unit
    var unit = indicatorMetadata.Unit.Label;
    if (!String.isNullOrEmpty(unit)) {
        rows.push(['Unit', indicatorMetadata.Unit.Label]);
    }

    // Text - Standard population
    addMetadataRow(rows, des, properties[propertyIndex++]);

    if (root) {
        // Age
        rows.push(['Age', root.Age.Name]);

        // Sex
        rows.push(['Sex', root.Sex.Name]);
    }

    // Year type
    rows.push(['Year type', indicatorMetadata.YearType.Name]);

    // Frequency
    addMetadataRow(rows, des, properties[propertyIndex++]);

    // Benchmarking method
    if (isDefined(benchmarkingMethodId)) {
        text = labelAsync.getLabel(benchmarkingMethodId, loaded.comparatorMethods, 'comparator-method', labelArgs);
        rows.push(['Benchmarking method', text]);
    }

    // Benchmarking significance level
    if (isDefined(benchmarkingSigLevel)) {
        text = (benchmarkingSigLevel <= 0) ?
            NOT_APPLICABLE :
            benchmarkingSigLevel + '%';
        rows.push(['Benchmarking significance level', text]);
    }

    // Confidence interval method
    var ciMethod = indicatorMetadata.ConfidenceIntervalMethod;
    if (ciMethod) {

        rows.push(['Confidence interval method', ciMethod.Name]);

        // Display CI method description
        var cimDescription = ciMethod.Description;
        if (cimDescription) {
            rows.push(['Confidence interval methodology', cimDescription]);
        }
    }

    // Confidence level
    var confidenceLevel = indicatorMetadata.ConfidenceLevel;
    if (confidenceLevel > -1) {
        rows.push(['Confidence level', confidenceLevel + '%']);
    }

    // Display all remaining properties
    while (propertyIndex < properties.length) {
        addMetadataRow(rows, des, properties[propertyIndex]);
        propertyIndex++;
    }

    return {
        html: templates.render('metadata', { rows: rows }),
        labelArgs: labelArgs
    };
}

function getLabelSpan(id) {
    return '<span id="' + id + '-label"></span>';
}

templates.add('metadata',
    '<div id="definitionTable"><h2>Indicator Definitions and Supporting Information</h2><table class="borderedTable" cellspacing="0">{{#rows}}<tr><td class="header">{{0}}</td><td>{{{1}}}</td></tr>{{/rows}}</table></div>');

pages.add(PAGE_MODES.METADATA, {
    id: 'metadata',
    title: 'Definitions',
    goto: goToMetadataPage,
    gotoName: 'goToMetadataPage',
    needsContainer: true,
    jqIds: ['indicatorMenuDiv', 'areaTypeBox']
});
