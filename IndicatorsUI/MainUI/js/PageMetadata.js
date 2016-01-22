function displayMetadata(indicatorMetadata, root) {
    
    var a = getMetadataHtml(indicatorMetadata, root);

    var e = $('#metadataBox');
    if (e.length) {
        // Practice profiles
        e.html(a.html);
    } else {
        // Fingertips
        pages.getContainerJq().html(a.html);
    }

    labelAsync.populate(a.labelArgs);
};

metadataState = {

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

labelAsync = (function () {

    // Key so that most recent AJAX response can be identified
    var key = 0;

    return {

        getCurrentKey: function () {
            return key;
        },

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
            if (_.size(labelArgs) > 0) {
                key++;

                var a = ['key=', key];
                for (var i in labelArgs) {
                    a.push('&', i, '=', labelArgs[i]);
                }

                ajaxGet('GetLabel.ashx', a.join(''), getLabelCallback);
            }
        }
    }
})();

function getLabelCallback(obj) {
    if (isDefined(obj) &&
            parseInt(obj.key, 10) === labelAsync.getCurrentKey()) {

        displayLabel(obj, 'age', 'ages');
        displayLabel(obj, 'yti', 'yearTypes');
        displayLabel(obj, 'cm', 'methods');

        var key = 'cim';
        var ciMethod = obj[key];
        if (isDefined(ciMethod)) {

            loaded['ciMethods'][ciMethod.Id] = ciMethod;

            $('#' + key + 'Label').html(ciMethod.Name);

            var des = ciMethod.Description;
            var jq = $('#cimdLabel');
            if (isDefined(des)) {
                $(jq).html(des);
            } else {
                jq.parent().parent().hide();
            }
        }
    }
};

function addMetadataRow(rows, textMetadata, property) {

    var columnName = property.ColumnName;

    if (textMetadata !== null && textMetadata.hasOwnProperty(columnName)) {
        var text = textMetadata[columnName];

        if (isDefined(text) && !String.isNullOrEmpty(text)) {

            if ((columnName === 'DataQuality') && showDataQuality) {
                // Add data quality flags instead of number
                var dataQualityCount = parseInt(text);
                rows.push([property.DisplayName, getIndicatorDataQualityHtml(text) + ' ' + getIndicatorDataQualityTooltipText(dataQualityCount)]);
            } else {
                rows.push([property.DisplayName, text]);
            }
        }
    }
}

function getMetadataProperties() {

    if (isDefined(loaded.indicatorProperties)) {
        ajaxMonitor.callCompleted();
    } else {

        ajaxGet('data/indicator_metadata_text_properties', '',
            function (obj) {

                loaded.indicatorProperties = obj;

                ajaxMonitor.callCompleted();
            });
    }
}

function displayLabel(obj, key, cacheKey) {

    var label = obj[key];

    if (isDefined(label)) {
        loaded[cacheKey][label.Id] = label.Text;
        $('#' + key + 'Label').html(label.Text);
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

    var grouping = isDefined(root) && root.hasOwnProperty('Grouping') ?
        root.Grouping[0] :
        root; // Practice profiles

    if (isDefined(grouping)) {
        var ageId = root.AgeId,
        benchmarkingMethodId = grouping.MethodId,
        benchmarkingSigLevel = grouping.SigLevel,
        sexId = root.SexId;
    } else {
        ageId = benchmarkingMethodId = benchmarkingSigLevel = sexId = null;
    }

    var labelArgs = {},
    rows = [],
    des = indicatorMetadata.Descriptive,
    text;

    // Initial indicator text properties
    var properties = loaded.indicatorProperties;
    for (var propertyIndex = 0; propertyIndex < properties.length; propertyIndex++) {
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
    rows.push(['Value type', indicatorMetadata.ValueType.Label]);

    // Text - Methodology
    addMetadataRow(rows, des, properties[propertyIndex++]);

    // Unit
    var unit = indicatorMetadata.Unit.Label;
    if (!String.isNullOrEmpty(unit)) {
        rows.push(['Unit', indicatorMetadata.Unit.Label]);
    }

    // Text - Standard population
    addMetadataRow(rows, des, properties[propertyIndex++]);

    // Age
    if (isDefined(ageId)) {
        text = labelAsync.getLabel(ageId, loaded.ages, 'age', labelArgs);
        rows.push(['Age', text]);
    }

    // Sex
    if (isDefined(sexId)) {
        rows.push(['Sex', new SexAndAge().getSexLabel(sexId)]);
    }

    // Year type
    var yearTypeId = indicatorMetadata.YearTypeId;
    metadataState.yearTypeId = yearTypeId;
    text = labelAsync.getLabel(yearTypeId, loaded.yearTypes, 'yti', labelArgs);
    rows.push(['Year type', text]);

    // Text - Frequency
    addMetadataRow(rows, des, properties[propertyIndex++]);

    // Benchmarking method
    if (isDefined(benchmarkingMethodId)) {
        var text = labelAsync.getLabel(benchmarkingMethodId, loaded.methods, 'cm', labelArgs);
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
    var ciMethodId = indicatorMetadata.CIMethodId;
    if (isDefined(ciMethodId) && ciMethodId > -1) {
        var ciMethod = labelAsync.getLabel(ciMethodId, loaded.ciMethods, 'cim', labelArgs);

        var isMethod = ciMethod.hasOwnProperty('Name');
        rows.push(['Confidence interval method', isMethod ? ciMethod.Name : ciMethod]);

        if (ciMethodId <= 0) {
            var description = null;
        }
        else if (isMethod) {
            var cimDescription = ciMethod.Description;
            description = isDefined(cimDescription) ? cimDescription : null;
        }
        else {
            description = getLabelSpan('cimd');
        }

        if (description !== null) {
            rows.push(['Confidence interval methodology', description]);
        }
    }

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
    return '<span id="' + id + 'Label"></span>';
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

