describe('getSearchResultParameters', function () {

    var childAreaTypeId = 3;
    var searchResult = {
        Easting: 1,
        Northing: 2,
        PlaceName: 'a',
        PolygonAreaCode: 'b'
    };

    it('all required parameters are included', function () {
        var parameters = getSearchResultParameters(searchResult, childAreaTypeId);
        expect(parameters).toEqual(
            'easting=1&northing=2&place_name=a');
    });

});

describe('useQuintiles', function () {

    it('true', function () {
        expect(useQuintiles(15)).toEqual(true);
    });

    it('false', function () {
        expect(useQuintiles(1)).toEqual(false);
    });
});

describe('useBlueOrangeBlue', function () {

    it('true', function () {
        expect(useBlueOrangeBlue(99)).toEqual(true);
    });

    it('false', function () {
        expect(useBlueOrangeBlue(1)).toEqual(false);
    });
});

describe('getGradeFunction', function () {

    var quintilesComparatorMethodId = 15;
    hasPracticeData = false;
    var otherComparatorMethodId = 1;
    var noData = 'no-data';

    var ragGroupRoot = {
        PolarityId: 1
    };

    var bobGroupRoot = {
        PolarityId: 99
    };

    it('quintiles', function () {
        var gradeFunction = getGradeFunction(quintilesComparatorMethodId);
        expect(gradeFunction(1)).toEqual('grade-quintile-1');
    });

    it('quintiles no data', function () {
        var gradeFunction = getGradeFunction(quintilesComparatorMethodId);
        expect(gradeFunction(0)).toEqual(noData);
        expect(gradeFunction(-1)).toEqual(noData);
    });

    it('bob', function () {
        hasPracticeData = true;
        var gradeFunction = getGradeFunction(otherComparatorMethodId);
        expect(gradeFunction(1, bobGroupRoot)).toEqual('bobLower');
        hasPracticeData = false;
    });

    it('rag', function () {
        MT.model.parentCode = NATIONAL_CODE;

        var gradeFunction = getGradeFunction(otherComparatorMethodId);
        expect(gradeFunction(1, ragGroupRoot)).toEqual('grade-3');
    });

    it('rag no data', function () {
        var gradeFunction = getGradeFunction(otherComparatorMethodId);
        expect(gradeFunction(0)).toEqual(noData);
        expect(gradeFunction(-1)).toEqual(noData);
    });
});

describe('UnitFormat', function () {

    var percentageMetadata = { Unit: { Id: 5 } };
    var otherUnitMetadata = { Unit: { Id: 1, Label: 'per 1000' } };

    it('getLabel where no value', function () {
        expect(new UnitFormat(percentageMetadata, -1).getLabel()).toBe('');
    });

    it('getLabel for percentage unit', function () {
        expect(new UnitFormat(percentageMetadata, 0).getLabel()).toContain('%');
    });

    it('getLabel for unit that is not a percentage', function () {
        expect(new UnitFormat(otherUnitMetadata, 0).getLabel()).not.toContain('%');
    });

    it('getLongLabel where no value', function () {
        expect(new UnitFormat(percentageMetadata, -1).getLongLabel()).toBe('');
    });

    it('getLongLabel for percentage unit', function () {
        expect(new UnitFormat(percentageMetadata, 0).getLongLabel()).toContain('%');
    });

    it('getLongLabel for other unit', function () {
        expect(new UnitFormat(otherUnitMetadata, 0).getLongLabel()).toContain('per 1000');
    });

    it('getClass', function () {
        // No class
        expect(new UnitFormat(otherUnitMetadata, -1).getClass()).toBe('');
        expect(new UnitFormat(null, 0).getClass()).toBe('');

        // Class is returned
        expect(new UnitFormat(otherUnitMetadata, 0).getClass()).toBe('unit-long');
        expect(new UnitFormat(percentageMetadata, 0).getClass()).toBe('unit-percent');
    });
});

