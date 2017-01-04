

describe('IndicatorFormatter', function () {

    NO_DATA = "x";
    ui = null;
    correctForPolarity = null;

    it('getVal', function () {

        var formatter = new IndicatorFormatter(null, null, null, null);
        expect(formatter.getVal({ ValF: 1 }, 'ValF')).toBe('1');
    });

    it('getVal for null CoreDataSet object', function () {

        var formatter = new IndicatorFormatter(null, null, null, null);
        expect(formatter.getVal(null, 'ValF')).toBe(NO_DATA);
    });

    it('getVal for "-" ValF', function () {

        var formatter = new IndicatorFormatter(null, null, null, null);
        expect(formatter.getVal({ ValF: '-' }, 'ValF')).toBe(NO_DATA);
    });
});


describe('CoreDataSetList', function () {

    it('getValidValues', function () {

        var array = [
            { Val: 5 },
            { Val: 7 },
            { Val: 1 },
            { Val: 2 }
        ];

        var values = new CoreDataSetList(array).getValidValues('Val');

        expect(values.length).toBe(4);
    });

    it('getValidValues for property that is not "Val"', function () {

        var array = [
            { LoCI: 7 },
            { LoCI: 15 },
            { LoCI: 3 },
            { Val: 1 }
        ];

        var values = new CoreDataSetList(array).getValidValues('LoCI');

        expect(values.length).toBe(3);
    });

    it('getValidValues ignores null elements', function () {

        var array = [null, null];

        var values = new CoreDataSetList(array).getValidValues('Val');

        expect(values.length).toBe(0);
    });

    it('getValidValues removes -1', function () {

        var array = [{ Val: -1 }, { Val: -1 }];

        var values = new CoreDataSetList(array).getValidValues('Val');

        expect(values.length).toBe(0);
    });

    it('getValidValues ignores undefined elements', function () {

        var undef;
        var array = [{ Val: 2 }, undef, { Val: 3 }];

        var values = new CoreDataSetList(array).getValidValues('Val');

        expect(values.length).toBe(2);
    });

    it('areAnyValidTrendValues false if list null', function () {
        expect(new CoreDataSetList(null).areAnyValidTrendValues()).toEqual(false);
    });

    it('areAnyValidTrendValues false if list not defined', function () {
        expect(new CoreDataSetList().areAnyValidTrendValues()).toEqual(false);
    });

    it('areAnyValidTrendValues false if list empty', function () {
        expect(new CoreDataSetList([]).areAnyValidTrendValues()).toEqual(false);
    });

    it('areAnyValidTrendValues false if only -1 values', function () {
        expect(new CoreDataSetList([{ V: -1 }]).areAnyValidTrendValues()).toEqual(false);
    });

    it('areAnyValidTrendValues true if valid values', function () {
        expect(new CoreDataSetList([{ V: 1 }]).areAnyValidTrendValues()).toEqual(true);
    });

    it('sortByValue', function () {

        var values = [
            { Val: 1 },
            { Val: 2 },
            { Val: 0 }
        ];

        new CoreDataSetList(values).sortByValue();

        expect(values[0].Val).toBe(0);
        expect(values[1].Val).toBe(1);
        expect(values[2].Val).toBe(2);
    });

    it('sortByCount', function () {

        var values = [
            { Count: 1 },
            { Count: 2 },
            { Count: 0 }
        ];

        new CoreDataSetList(values).sortByCount();

        expect(values[0].Count).toBe(0);
        expect(values[1].Count).toBe(1);
        expect(values[2].Count).toBe(2);
    });
});

describe('TrendDataInfo', function () {

    var isValue = function (data) {
        return new TrendDataInfo(data).isValue();
    };

    it('getValF returns V', function () {
        expect(new TrendDataInfo({ V: 2 }).getValF()).toBe(2);
    });

    it('isValue false if data null', function () {
        expect(isValue(null)).toBe(false);
    });

    it('isValue false if formatted value is "-"', function () {
        expect(isValue({ V: '-' })).toBe(false);
    });

    it('isValue true if formatted value is a number string', function () {
        expect(isValue({ V: '2' })).toBe(true);
    });

    it('getNoteId function is defined', function () {
        expect(!!TrendDataInfo.prototype.getNoteId).toBe(true);
    });
});

describe('MinMaxFinder', function () {

    it('finds min and max', function () {

        var numbers = [5, 7, 1, 2];

        var maxMin = new MinMaxFinder(numbers);

        expect(maxMin.min).toBe(1);
        expect(maxMin.max).toBe(7);
        expect(maxMin.isValid).toBe(true);
    });

    it('does not use text sort', function () {

        var numbers = [7, 15, 3, 1];

        var maxMin = new MinMaxFinder(numbers);

        expect(maxMin.min).toBe(1);
        expect(maxMin.max).toBe(15);
    });
});

describe('BarScale', function () {

    var assertPixelsPerUnit = function (data, min, max) {
        var barScale = new BarScale(data);

        max = max * barScale.buffer;

        min = min > 0 ?
            0 :
            min;

        var expectedPixelsPerUnit = barScale.width / (max - min);

        expect(barScale.pixelsPerUnit).toBe(expectedPixelsPerUnit);
    };

    it('pixels per unit for all positive numbers', function () {

        var max = 5,
            min = 4;

        var array = [
            { Val: max, LoCI: 5, UpCI: 5 },
            { Val: min, LoCI: 4, UpCI: 4 }
        ];

        assertPixelsPerUnit(array, min, max);
    });

    it('pixels per unit with a negative number', function () {

        var max = 5,
            min = -4;

        var array = [
            { Val: max, LoCI: 5, UpCI: 5 },
            { Val: min, LoCI: 4, UpCI: 4 }
        ];

        assertPixelsPerUnit(array, min, max);
    });

    it('pixels per unit for all negative numbers', function () {

        var max = -4,
            min = -5;

        var array = [
            { Val: max, LoCI: max, UpCI: max },
            { Val: min, LoCI: min, UpCI: min }
        ];

        var barScale = new BarScale(array);
        expect(barScale.range).toBe(5/* bars start from zero */);
    });

    it('pixels per unit considers LoCI and UpCI', function () {

        var max = 5,
            min = 2;

        var array = [
            { Val: 3, LoCI: min, UpCI: 3 },
            { Val: 3, LoCI: 3, UpCI: max }
        ];

        assertPixelsPerUnit(array, min, max);
    });

    it('when negative pixels is zero', function () {

        var max = 5,
            min = 2;

        var array = [
            { Val: min, LoCI: 3, UpCI: 3 },
            { Val: max, LoCI: 3, UpCI: 3 }
        ];

        var barScale = new BarScale(array);
        expect(barScale.negativePixels).toBe(0);
    });

    it('when negative pixels is not zero', function () {

        var max = 5,
            min = -2;

        var array = [
            { Val: min, LoCI: 3, UpCI: 3 },
            { Val: max, LoCI: 3, UpCI: 3 }
        ];

        var barScale = new BarScale(array);
        expect(barScale.negativePixels).toBe(66);
    });
});

describe('AreaCollection', function () {

    var areas = [
        { Name: 'a-name', Code: 'a-code' },
        { Name: 'b-name', Code: 'b-code' },
        { Name: 'c-name', Code: 'c-code' }
    ];

    it('find', function () {
        expect(new AreaCollection(areas).find('b-code').Code).toBe('b-code');
    });

    it('containsAreaCode', function () {
        expect(new AreaCollection(areas).containsAreaCode('c-code')).toBeTruthy();
    });

    it('findAll', function () {
        expect(new AreaCollection(areas).findAll(['a-code', 'c-code']).length).toBe(2);
    });
});

describe('ParentMenu', function () {

    it('count', function () {

        var model = { parentCode: 'a', parentTypeId: 1 };
        var loadedData = { 1: [{ Name: 'A', Code: 'a' }, { Name: 'B', Code: 'b' }] };
        var menu = new ParentMenu('<select><option/></select>', model, loadedData);

        menu.setOptions();

        expect(menu.count()).toBe(2);
    });

});

describe('ValueDisplayer', function () {

    var valF = '2',
        validNoteId = 100;

    var getHtml = function (data) {
        return new ValueDisplayer().byDataInfo(new CoreDataSetInfo(data));
    };

    var unitHtml = function (unit) {
        return '<span class="unit">' + unit + '</span>';
    };

    it('validate generated HTML', function () {
        new HtmlChecker(getHtml({ ValF: valF })).validate();
    });

    it('Value is displayed if defined', function () {
        expect(getHtml({ ValF: valF })).toContain(valF);
    });

    it('Note symbol and value are displayed if NoteId defined and Val defined', function () {
        var html = getHtml({ ValF: valF, NoteId: validNoteId });
        expect(html).toContain(VALUE_NOTE);
        expect(html).toContain(valF);
    });

    it('Only note symbol is displayed if NoteId defined and Val not defined', function () {
        var html = getHtml({ ValF: '-', NoteId: validNoteId });
        expect(html).toContain(VALUE_NOTE);
    });

    it('suffix is displayed after value', function () {
        var unit = { Label: '%' },
            dataInfo = new CoreDataSetInfo({ ValF: valF }),
            html = new ValueDisplayer(unit).byDataInfo(dataInfo);

        expect(html).toContain(valF + unitHtml('%'));
    });

    it('byNumberString', function () {
        var unit = { Label: '%' },
            html = new ValueDisplayer(unit).byNumberString(valF);
        expect(html).toContain(valF + unitHtml('%'));
    });

    it('noCommas generates numbers without commas', function () {
        var html = new ValueDisplayer(null).byNumberString(1000, { noCommas: 'y' });
        expect(html).toContain('1000');
    });

    it('number formatted with commas by default', function () {
        var html = new ValueDisplayer(null).byNumberString(1000);
        expect(html).toContain('1,000');
    });
});

describe('SearchTextValidator', function () {

    it('valid text available unchanged', function () {
        expect(new SearchTextValidator('a').text).toEqual('a');
    });

    it('hyphen replaced with space', function () {
        expect(new SearchTextValidator('a-b').text).toEqual('a b');
    });

    it('+ replaced with and', function () {
        expect(new SearchTextValidator('a+b').text).toEqual('a and b');
    });

    it('& replaced with and', function () {
        expect(new SearchTextValidator('a&b').text).toEqual('a and b');
    });

    it('non alphanumerics removed', function () {
        expect(new SearchTextValidator('a"b').text).toEqual('ab');
    });

    it('isTextOk false if matches place holder text', function () {
        expect(new SearchTextValidator('Indicator keywords').isOk).toEqual(false);
    });

    it('isTextOk true', function () {
        expect(new SearchTextValidator('a').isOk).toEqual(true);
    });
});

describe('PreferredAreaTypeId', function () {

    it('containsProfileId', function () {
        var model = { profileId: 1 };
        expect(new PreferredAreaTypeId(null, model).containsProfileId()).toBe(false);
        expect(new PreferredAreaTypeId('', model).containsProfileId()).toBe(false);
        expect(new PreferredAreaTypeId('1:2', model).containsProfileId()).toBe(true);
    });

    it('getAreaTypeId', function () {
        var model = { profileId: 1 };
        var areaTypeId = new PreferredAreaTypeId('1:3', model).getAreaTypeId();

        expect(areaTypeId).toBe(3);
        expect(_.isNumber(areaTypeId)).toBe(true);
    });

    it('deserialise on new', function () {
        var model = { profileId: 1 };
        expect(new PreferredAreaTypeId('1:5', model).getAreaTypeId()).toEqual(5);
    });

    it('serialise', function () {
        var model = {};
        expect(new PreferredAreaTypeId('1:5,2:6', model).serialise()).toEqual('1:5,2:6');
    });
});

describe('PreferredAreas', function () {

    it('get area null if no state', function () {
        var model = { areaTypeId: 1 };
        expect(new PreferredAreas(null, model).getAreaCode()).toEqual(null);
        expect(new PreferredAreas('', model).getAreaCode()).toEqual(null);
    });

    it('updateAreaCode', function () {
        var model = { areaTypeId: 1, areaCode: 'b' };
        var preferredAreas = new PreferredAreas(null, model);
        expect(preferredAreas.getAreaCode()).toEqual(null);
        preferredAreas.updateAreaCode();
        expect(preferredAreas.getAreaCode()).toEqual('b');
    });

    it('doesAreaCodeNeedUpdating', function () {
        var model = { areaTypeId: 1, areaCode: 'b' };
        var preferredAreas = new PreferredAreas(null, model);

        // Area code undefined so update required
        expect(preferredAreas.doesAreaCodeNeedUpdating()).toEqual(true);

        // Area code updated so no update required
        preferredAreas.updateAreaCode();
        expect(preferredAreas.doesAreaCodeNeedUpdating()).toEqual(false);

        // Changing model means update required
        model.areaCode = 'c';
        expect(preferredAreas.doesAreaCodeNeedUpdating()).toEqual(true);
    });

    it('deserialise on new', function () {
        var model = { areaTypeId: 1 };
        expect(new PreferredAreas('1:a', model).getAreaCode()).toEqual('a');
    });

    it('serialise', function () {
        var model = {};
        expect(new PreferredAreas('1:a,2:b', model).serialise()).toEqual('1:a,2:b');
    });
});


describe('HashSerialiser', function () {

    var hashString = 'a:1,b:2';
    var hash = { a: 1, b: 2 };

    it('serialise', function () {
        expect(new HashSerialiser().serialise(hash)).toEqual(hashString);
    });

    it('deserialise', function () {
        var hash = new HashSerialiser().deserialise(hashString);
        expect(hash['b']).toEqual('2');
    });
});


describe('getTargetLegendHtml', function () {

    var compareConfig = {
        useTarget: true,
    };

    var metaData = {
        Target: {
            BespokeKey: 'test',
            LowerLimit: 10,
            UpperLimit: 50,
            PolarityId: -1
        },
        Unit: 5
    };

    var nthPercentile = 'nth-percentile-range';
    var lastYearEngland = 'last-year-england';

    var expectedLowerRed = '<span class="target worse">&lt;10';
    var expectedHigherGreen = '<span class="target better">&ge;50';

    var expectedHigherRed = '<span class="target worse">&gt;50';
    var expectedLowerGreen = '<span class="target better">&le;10';


    it('should return empty for invalid BespokeKey', function () {
        var result = getTargetLegendHtml(compareConfig, metaData);
        expect(result).toEqual('');
    });

    it('should return LowerRed for nth-percentile-range BeSpokeKey and Polarity 1', function () {
        metaData.Target.PolarityId = 1;
        metaData.Target.BespokeKey = nthPercentile;
        var result = getTargetLegendHtml(compareConfig, metaData);
        expect(result).toContain(expectedLowerRed);
    });

    it('should return HigherGreen for nth-percentile-range BeSpokeKey and Polarity 1', function () {
        metaData.Target.PolarityId = 1;
        metaData.Target.BespokeKey = nthPercentile;
        var result = getTargetLegendHtml(compareConfig, metaData);
        expect(result).toContain(expectedHigherGreen);
    });


    it('should return expectedLowerGreen for nth-percentile-range BeSpokeKey and Polarity 0', function () {
        metaData.Target.PolarityId = 0;
        metaData.Target.BespokeKey = nthPercentile;
        var result = getTargetLegendHtml(compareConfig, metaData);
        expect(result).toContain(expectedLowerGreen);
    });

    it('should return HigherGreen for nth-percentile-range BeSpokeKey and Polarity 0', function () {
        metaData.Target.PolarityId = 0;
        metaData.Target.BespokeKey = nthPercentile;
        var result = getTargetLegendHtml(compareConfig, metaData);
        expect(result).toContain(expectedHigherRed);
    });

    it('should return ValidValue for null BespokeKey', function () {
        metaData.Target.PolarityId = 0;
        metaData.Target.BespokeKey = lastYearEngland;
        var result = getTargetLegendHtml(compareConfig, metaData);
        expect(result).toContain('previous year\'s England value');
    });

    it('should result contain better for last-year-england BespokeKey', function () {
        metaData.Target.PolarityId = 0;
        metaData.Target.BespokeKey = lastYearEngland;
        var result = getTargetLegendHtml(compareConfig, metaData);
        expect(result).toContain('target better');
    });

    it('should result contain bobHigher for last-year-england BespokeKey and Polarity 99', function () {
        metaData.Target.PolarityId = PolarityIds.BlueOrangeBlue;
        metaData.Target.BespokeKey = lastYearEngland;
        var result = getTargetLegendHtml(compareConfig, metaData);
        expect(result).toContain('target bobHigher');
    });

    it('should return empty for no target', function () {
        compareConfig.useTarget = false;
        var result = getTargetLegendHtml(compareConfig, metaData);
        expect(result).toEqual('');
    });

    it('should return LowerGreen for no BespokeKey and Polarity 0', function () {
        metaData.Target.BespokeKey = null;
        compareConfig.useTarget = true;
        metaData.Target.PolarityId = 0;
        var result = getTargetLegendHtml(compareConfig, metaData);
        expect(result).toContain('<span class="target better">&lt;10</span');
    });

    it('should return LowerRed for no BespokeKey and Polarity 1', function () {
        metaData.Target.BespokeKey = null;
        compareConfig.useTarget = true;
        metaData.Target.PolarityId = 1;
        var result = getTargetLegendHtml(compareConfig, metaData);
        expect(result).toContain(expectedLowerRed);
    });
});

describe('isSubnationalColumn', function () {

    var init = function () {
        // Set up conditions that need subnational column
        enumParentDisplay = PARENT_DISPLAY.NATIONAL_AND_REGIONAL;
        FT = {
            model: {
                parentTypeId: AreaTypeIds.CountyUA,
                isNearestNeighbours: function () { return false; }
            }
        };
    }

    it('yes', function () {
        init();
        expect(isSubnationalColumn()).toBe(true);
    });

    it('no where only national parent is displayed', function () {
        init();
        enumParentDisplay = PARENT_DISPLAY.NATIONAL_ONLY;
        expect(isSubnationalColumn()).toBe(false);
    });

    it('no if nearest neighbours', function () {
        init();
        FT.model.isNearestNeighbours = function () { return true; };
        expect(isSubnationalColumn()).toBe(false);
    });

    it('no if country parent area', function () {
        init();
        FT.model.parentTypeId = AreaTypeIds.Country;
        expect(isSubnationalColumn()).toBe(false);
    });

    it('no if all negative conditions met', function () {
        init();
        enumParentDisplay = PARENT_DISPLAY.NATIONAL_ONLY;
        FT.model.parentTypeId = AreaTypeIds.Country;
        FT.model.isNearestNeighbours = function () { return true; };
        expect(isSubnationalColumn()).toBe(false);
    });
});

describe('AreaAndDataSorter',
    function () {

        var areas, areaHash, data;

        // Initialise test data
        var init = function() {
            areas = [{ Code: 'a' }, { Code: 'b' }, { Code: 'c' }];
            areaHash = _.object(_.map(areas,
                function(area) {
                    return [area.Code, area];
                }));
            data = [
                { Val: 3, Count: 33, AreaCode: 'c' },
                { Val: 1, Count: 11, AreaCode: 'a' },
                { Val: 2, Count: 22, AreaCode: 'b' }
            ];
        }

        it('sort areas by value', function () {
            init();
            var sorter = new AreaAndDataSorter(0, data, areas, areaHash);
            var sortedAreas = sorter.byValue();

            // Assert
            expect(sortedAreas.length).toBe(3);
            expect(sortedAreas[0].Code).toBe('a');
            expect(sortedAreas[2].Code).toBe('c');
        });

        it('sort areas by value where no matching data', function () {
            init();
            areas.push({ Val: 4, Count: 44, AreaCode: 'e' });
            var sorter = new AreaAndDataSorter(0, data, areas, areaHash);
            var sortedAreas = sorter.byValue();

            // Assert
            expect(sortedAreas.length).toBe(3);
            expect(sortedAreas[0].Code).toBe('a');
            expect(sortedAreas[2].Code).toBe('c');
        });

        it('sort areas by value where no matching area', function () {
            init();
            data.push({ Code: 'd' });
            var sorter = new AreaAndDataSorter(0, data, areas, areaHash);
            var sortedAreas = sorter.byValue();

            // Assert
            expect(sortedAreas.length).toBe(3);
            expect(sortedAreas[0].Code).toBe('a');
            expect(sortedAreas[2].Code).toBe('c');
        });

        it('sort areas by count', function () {
            init();
            var sorter = new AreaAndDataSorter(0, data, areas, areaHash);
            var sortedAreas = sorter.byCount();

            // Assert
            expect(sortedAreas.length).toBe(3);
            expect(sortedAreas[0].Code).toBe('a');
            expect(sortedAreas[2].Code).toBe('c');
        });

        it('sort areas by value when no data', function () {
            init();
            var sorter = new AreaAndDataSorter(0, [], [], {});
            var sortedAreas = sorter.byValue();
            expect(sortedAreas.length).toBe(0);
        });

        it('sort areas by count when no data', function () {
            init();
            var sorter = new AreaAndDataSorter(0, [], [], {});
            var sortedAreas = sorter.byCount();
            expect(sortedAreas.length).toBe(0);
        });

    });