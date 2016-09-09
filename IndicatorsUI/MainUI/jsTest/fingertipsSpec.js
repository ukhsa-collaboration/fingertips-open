
describe('SexAndAge', function () {

    var sex = { Name: 'Female' };
    var age = { Name: '60+' };

    it('label as expected for only sex', function () {

        var groupRoot = {
            StateSex: true,
            Sex: sex
        };

        expect(new SexAndAge().getLabel(groupRoot)).toBe(' (Female)');
    });

    it('no label if StateSex is false and AgeLabel not defined', function () {

        var groupRoot = {
            StateSex: false,
            StateAge: false,
            Sex: sex
        };

        expect(new SexAndAge().getLabel(groupRoot)).toBe('');
    });

    it('label as expected for only age', function () {

        var groupRoot = {
            StateAge: true,
            Age:age
        };

        expect(new SexAndAge().getLabel(groupRoot)).toBe(' (60+)');
    });

    it('label for both age and sex', function () {

        var groupRoot = {
            StateAge: true,
            Age:age,
            StateSex: true,
            Sex: sex
        };

        expect(new SexAndAge().getLabel(groupRoot)).toBe(' (Female, 60+)');
    });
});

describe('MutuallyExclusiveDisplay', function () {

    var a = $('<div></div>'),
        b = $('<div></div>');

    var assertShown = function (jq) {
        expect(jq.css('display')).toBe('block');
    };

    var assertHidden = function (jq) {
        expect(jq.css('display')).toBe('none');
    };

    it('show a and hide b', function () {

        new MutuallyExclusiveDisplay({
            a: a,
            b: b
        }).showA(true);

        assertShown(a);
        assertHidden(b);
    });

    it('show b and hide a', function () {

        new MutuallyExclusiveDisplay({
            a: a,
            b: b
        }).showA(false);

        assertShown(b);
        assertHidden(a);
    });

    it('showB always shows b and hides a', function () {

        new MutuallyExclusiveDisplay({
            a: a,
            b: b
        }).showB();

        assertShown(b);
        assertHidden(a);
    });
});

describe('ValuePrefix', function () {

    var prefixText = 'a';

    var unitHtml = function (unit) {
        return '<span class="unit">' + unit + '</span>';
    };

    it('no prefix if ShowLeft not defined', function () {
        var unit = { Label: prefixText };
        expect(new ValuePrefix(unit).getLabel()).toBe('');
    });

    it('prefix displayed if ShowLeft true', function () {
        var unit = { Label: prefixText, ShowLeft: true };
        expect(new ValuePrefix(unit).getLabel()).toBe(
            unitHtml(prefixText)
            );
    });

    it('getLabel tolerates null unit', function () {
        expect(new ValuePrefix(null).getLabel()).toBe('');
    });
});


describe('ValueSuffix', function () {

    var suffixText = 'a';

    var fullLabel = function(unit) {
        return new ValueSuffix(unit).getFullLabel();
    };

    var shortLabel = function (unit) {
        return new ValueSuffix(unit).getShortLabel();
    };

    var unitHtml = function (unit) {
        return '<span class="unit">' + unit + '</span>';
    };

    it('getShortLabel returns no suffix if ShowLeft defined', function () {
        var unit = { Label: suffixText, ShowLeft: true };
        expect(shortLabel(unit)).toBe('');
    });

    it('getShortLabel tolerates null unit', function () {
        expect(shortLabel(null)).toBe('');
    });

    it('getShortLabel returns % if unit is percentage', function () {
        var unit = { Label: '%' };
        expect(shortLabel(unit)).toBe(unitHtml('%'));
    });

    it('getShortLabel returns empty string if unit not short', function () {
        var unit = { Label: suffixText };
        expect(shortLabel(unit)).toBe('');
    });

    it('getFullLabel tolerates null unit', function () {
        expect(fullLabel(null)).toBe('');
    });

    it('getFullLabel returns no suffix if ShowLeft defined', function () {
        var unit = { Label: suffixText, ShowLeft: true };
        expect(fullLabel(unit)).toBe('');
    });

    it('getFullLabel for percentage', function () {
        var unit = { Label: '%' };
        expect(fullLabel(unit)).toBe(unitHtml('%'));
    });

    it('getFullLabel for other units', function () {
        var unit = { Label: suffixText };
        expect(fullLabel(unit)).toBe(' ' + unitHtml(suffixText));
    });

    it('getFullLabelIfNoShort for percentage returns empty string', function () {
        var unit = { Label: '%' };
        expect(new ValueSuffix(unit).getFullLabelIfNoShort()).toBe('');
    });

    it('getFullLabelIfNoShort for other units', function () {
        var unit = { Label: suffixText };
        expect(new ValueSuffix(unit).getFullLabelIfNoShort()).toBe(' ' + unitHtml(suffixText));
    });

    it('getFullLabelIfNoShort tolerates null unit', function () {
        expect(new ValueSuffix(null).getFullLabelIfNoShort()).toBe('');
    });
});

describe('ValueWithUnit', function () {

    var label = 'a';

    var unitHtml = function (unit) {
        return '<span class="unit">' + unit + '</span>';
    };

    it('numbers formatted with commas by default', function () {
        expect(new ValueWithUnit(null).getShortLabel(1000)).toBe('1,000');
    });

    it('noCommas option generates number without commas', function () {
        expect(new ValueWithUnit(null).getShortLabel(1000, {noCommas:'y'})).toBe('1000');
    });

    it('getFullLabel prefix if ShowLeft defined', function () {
        var unit = { Label: label, ShowLeft : true };
        expect(new ValueWithUnit(unit).getFullLabel(1)).toBe(unitHtml(label) + '1');
    });

    it('getFullLabel space then suffix by default', function () {
        var unit = { Label: label};
        expect(new ValueWithUnit(unit).getFullLabel(1)).toBe('1 ' + unitHtml(label));
    });

    it('getFullLabel space then suffix by default', function () {
        var unit = { Label: '%' };
        expect(new ValueWithUnit(unit).getFullLabel(1)).toBe('1' + unitHtml('%'));
    });

    it('getFullLabel just value if unit is null', function () {
        expect(new ValueWithUnit(null).getFullLabel(1)).toBe('1');
    });

    it('getShortLabel prefix if ShowLeft defined', function () {
        var unit = { Label: label, ShowLeft: true };
        expect(new ValueWithUnit(unit).getShortLabel(1)).toBe(unitHtml(label) + '1');
    });

    it('getShortLabel does not display suffixes other than %', function () {

        var unit = { Label: label };
        expect(new ValueWithUnit(unit).getShortLabel(1)).toBe('1');

        unit = { Label: '%' };
        expect(new ValueWithUnit(unit).getShortLabel(1)).toBe('1' + unitHtml('%'));
    });

    it('getShortLabel just value if unit is null', function () {
        expect(new ValueWithUnit(null).getShortLabel(1)).toBe('1');
    });
});

describe('ElementManager', function () {

    it('add', function () {
        var manager = new ElementManager();
        manager.add(['a','b']);
        expect(manager.getAll().length).toBe(2);
    });

    it('addNotShown', function () {
        var manager = new ElementManager();
        manager.add(['a']);
        manager.addNotShown(['b','c']);
        expect(manager.getAll().length).toBe(3);
    });

    it('add ignores ids that have already been added', function () {
        var manager = new ElementManager();
        manager.add(['a', 'b']);
        manager.add(['a', 'c']);
        expect(manager.getAll().length).toBe(3);
    });

    it('displayElements does not throw exception', function () {
        var manager = new ElementManager();
        manager.add(['a', 'b']);
        manager.displayElements([$('#a')]);
    });
});


describe('sortAreasByShort', function () {

    it('sort is case insensitive', function() {
        var areas = [{ Short: 'c' }, { Short: 'B' }];
        areas.sort(sortAreasByShort);
        expect(areas[0].Short).toBe('B');
        expect(areas[1].Short).toBe('c');
    });
});

describe('sortAreasByName', function () {

    it('sort is case insensitive', function () {
        var areas = [{ Name: 'c' }, { Name: 'B' }];
        areas.sort(sortAreasByName);
        expect(areas[0].Name).toBe('B');
        expect(areas[1].Name).toBe('c');
    });
});

describe('AreaSearchResults', function () {

    it('place name and polygon name are not repeated if only differ by case ', function () {

        var data = [
            { PlaceName: 'Kingston upon Thames', PolygonAreaCode: 'code1', PolygonAreaName: 'Kingston Upon Thames' }
        ];

        expect(new AreaSearchResults(data).suggestions[0].label).toEqual('Kingston upon Thames');
    });

    it('case is ignored for place name', function () {

        var data = [
            { PlaceName: 'Hull on Sea', PolygonAreaCode: 'code1', PolygonAreaName: 'c' },
            { PlaceName: 'Hull On Sea', PolygonAreaCode: 'code2', PolygonAreaName: 'c' }
        ];

        expect(new AreaSearchResults(data).suggestions.length).toEqual(1);
    });

    it('are no duplicate suggestions', function () {

        var data = [
            { PlaceName: 'b', PolygonAreaCode: 'code1', PolygonAreaName: 'c' },
            { PlaceName: 'b', PolygonAreaCode: 'code2', PolygonAreaName: 'c' }
        ];

        expect(new AreaSearchResults(data).suggestions.length).toEqual(1);
    });

    it('county and polygonareaname are combined to make the label', function () {

        var data = [
            { PlaceName: 'b', PolygonAreaCode: 'code1', PolygonAreaName: 'a' }
        ];

        expect(new AreaSearchResults(data).suggestions[0].label).toEqual('b, a');
    });

    it('county and placename are not repeated when the same', function () {

        var data = [
            { PlaceName: 'a', PolygonAreaCode: 'code1', PolygonAreaName: 'a' }
        ];

        expect(new AreaSearchResults(data).suggestions[0].label).toEqual('a');
    });

    it('placename is followed by CCG name when CCG is area type', function () {

        var data = [
            { PlaceName: 'a', PolygonAreaCode: 'code1', PolygonAreaName: 'ccg' }
        ];

        expect(new AreaSearchResults(data).suggestions[0].label).toEqual('a, ccg');
    });

    it('placename is followed by area team name when area team is area type', function () {

        var data = [
            { PlaceName: 'a', PolygonAreaCode: 'code1', PolygonAreaName: 'areateam' }
        ];

        expect(new AreaSearchResults(data).suggestions[0].label).toEqual('a, areateam');
    });

    it('placename is followed by district name when area type is district & UA', function () {

        var data = [
            { PlaceName: 'a', PolygonAreaCode: 'code1', PolygonAreaName: 'district' }
        ];

        expect(new AreaSearchResults(data).suggestions[0].label).toEqual('a, district');
    });
});

describe('AreaTypes', function () {

    it('getAllIds', function () {

        var areaTypes = new AreaTypes([{ Id: 1 }, { Id: 2 }], {});
        var allIds = areaTypes.getAllIds();

        expect(allIds[0]).toBe(1);
        expect(allIds[1]).toBe(2);
    });


    it('getAllIds if loaded.areaTypes is already defined', function () {

        loaded.areaTypes = {
            1: { Id: 1 },
            2: { Id: 2 }
        };

        var areaTypes = new AreaTypes();
        var allIds = areaTypes.getAllIds();

        expect(allIds[0]).toBe(1);
        expect(allIds[1]).toBe(2);
    });

    it('getAreaTypes if loaded.areaTypes is already defined', function () {

        loaded.areaTypes = {
            1: { Id: 1 },
            2: { Id: 2 }
        };

        var areaTypes = new AreaTypes();
        var allIds = areaTypes.getAreaTypes();

        expect(allIds[0].Id).toBe(1);
        expect(allIds[1].Id).toBe(2);
    });
});

describe('CoreDataSetInfo', function () {

    var isNote = function (data) {
        return new CoreDataSetInfo(data).isNote();
    };

    var isValue = function (data) {
        return new CoreDataSetInfo(data).isValue();
    };

    var isCount = function (data) {
        return new CoreDataSetInfo(data).isCount();
    };

    it('areCIs', function () {
        expect(new CoreDataSetInfo({ Val: 2, LoCI:1,UpCI:3 }).areCIs()).toBe(true);
    });

    it('areCIs true if LoCI is zero', function () {
        expect(new CoreDataSetInfo({ Val: 2, LoCI: 0, UpCI: 3 }).areCIs()).toBe(true);
    });

    it('areCIs false if null CoreDataSet', function () {
        expect(new CoreDataSetInfo(null).areCIs()).toBe(false);
    });

    it('areCIs false if CIs not defined', function () {
        expect(new CoreDataSetInfo({ Val: 2 }).areCIs()).toBe(false);
    });

    it('areCIs false if everything is -1', function() {
        expect(new CoreDataSetInfo({ Val:-1, LoCI:-1,UpCI:-1}).areCIs()).toBe(false);
    });

    it('getValF returns ValF', function () {
        expect(new CoreDataSetInfo({ ValF: 2 }).getValF()).toBe(2);
    });

    it('isValue false if data null', function () {
        expect(isValue(null)).toBe(false);
    });

    it('isValue false if ValF "-"', function () {
        expect(isValue({ ValF: '-' })).toBe(false);
    });

    it('isValue true if ValF number string', function () {
        expect(isValue({ ValF: '2' })).toBe(true);
    });

    it('isCount false if data null', function () {
        expect(isCount(null)).toBe(false);
    });

    it('isValue true if Val is zero', function () {
        expect(isValue({ Val: 0 })).toBe(true);
    });

    it('isCount false if Count -1', function () {
        expect(isCount({ Count: -1 })).toBe(false);
    });

    it('isCount false if Count null', function () {
        expect(isCount({ Count: null })).toBe(false);
    });

    it('isCount true if Count defined', function () {
        expect(isCount({ Count: 2 })).toBe(true);
    });

    it('isNote false if data null', function () {
        expect(isNote(null)).toBe(false);
    });

    it('isNote false if NoteId property not defined', function () {
        expect(isNote({})).toBe(false);
    });

    it('isNote false if NoteId property zero', function () {
        expect(isNote({ NoteId: 0 })).toBe(false);
    });

    it('isNote true if valid NoteId property', function () {
        expect(isNote({ NoteId: 100 })).toBe(true);
    });

    it('getNoteId returns null if not defined', function () {
        expect(new CoreDataSetInfo({}).getNoteId()).toBe(null);
    });

    it('getNoteId returns NoteId if defined', function () {
        expect(new CoreDataSetInfo({ NoteId: 2 }).getNoteId()).toBe(2);
    });

    // areValueAndCIsZero
    it('areValueAndCIsZero true if everything zero', function () {
        var data = { Val: 0, LoCI: 0, UpCI: 0, ValF: "0.0", LoCIF: "0.0", UpCIF: "0.0" };
        expect(new CoreDataSetInfo(data).areValueAndCIsZero()).toBe(true);
    });

    it('areValueAndCIsZero false if UpCI not zero', function () {
        var data = { Val: 0, LoCI: 0, UpCI: 0.1, ValF: "0.0", LoCIF: "0.0", UpCIF: "0.1" };
        expect(new CoreDataSetInfo(data).areValueAndCIsZero()).toBe(false);
    });

    it('areValueAndCIsZero false if LoCI not zero', function () {
        var data = { Val: 0, LoCI: -0.1, UpCI: 0, ValF: "0.0", LoCIF: "-0.1", UpCIF: "0.0" };
        expect(new CoreDataSetInfo(data).areValueAndCIsZero()).toBe(false);
    });
});

describe('FT.model', function() {

    // Globals required by FT.model
    groupIds = [1];
    defaultAreaType = 2;

    var model = FT.model;

    it('toString', function () {
        model.update();
        pages.setCurrent(PAGE_MODES.TARTAN);

        expect(model.toString()).toBe('page/0/gid/1/ati/2');
    });

    it('toString nearestNeighbour', function () {
        model.update();
        model.nearestNeighbour = 'a';
        pages.setCurrent(PAGE_MODES.TARTAN);

        expect(model.toString()).toBe('page/0/gid/1/ati/2/nn/a');
    });

});