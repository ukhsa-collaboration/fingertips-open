
describe('IndicatorIdList', function () {

    var areaTypeId = 1;
    var areaTypeId2 = 2;
    var indicatorId = 10;
    var indicatorId2 = 11;
    var indicatorId3 = 12;

    it('any is false', function () {

        var hash = {};
        hash[areaTypeId] = [];

        expect(new IndicatorIdList(hash).any()).toBe(false);
    });

    it('any is true', function () {

        var hash = {};
        hash[areaTypeId] = [indicatorId];

        expect(new IndicatorIdList(hash).any()).toBe(true);
    });

    it('getIndicatorCount', function () {

        var hash = {};
        hash[areaTypeId] = [indicatorId];
        hash[areaTypeId2] = [indicatorId, indicatorId2];

        expect(new IndicatorIdList(hash).getIndicatorCount(areaTypeId)).toBe(1);
        expect(new IndicatorIdList(hash).getIndicatorCount(areaTypeId2)).toBe(2);
    });

    it('areaTypeIdWithMostIndicators', function () {

        var hash = {};
        hash[areaTypeId] = [indicatorId];
        hash[areaTypeId2] = [indicatorId, indicatorId2];

        expect(new IndicatorIdList(hash).areaTypeIdWithMostIndicators()).toBe(areaTypeId2);
    });

    it('anyForAreaType', function () {

        var hash = {};
        hash[areaTypeId] = [];
        hash[areaTypeId2] = [indicatorId, indicatorId2];

        expect(new IndicatorIdList(hash).anyForAreaType(areaTypeId)).toBe(false);
        expect(new IndicatorIdList(hash).anyForAreaType(areaTypeId2)).toBe(true);
    });

    it('anyForAreaType where area type id is not in list', function () {

        var hash = {};
        hash[areaTypeId] = [indicatorId];

        expect(new IndicatorIdList(hash).anyForAreaType(areaTypeId2)).toBe(false);
    });

    it('getIds', function () {

        var hash = {};
        hash[areaTypeId2] = [indicatorId, indicatorId2];

        expect(new IndicatorIdList(hash).getIds(areaTypeId2).length).toBe(2);
    });

    it('getAllIds', function () {

        var hash = {};
        hash[areaTypeId] = [indicatorId, indicatorId3];
        hash[areaTypeId2] = [indicatorId, indicatorId2];

        expect(new IndicatorIdList(hash).getAllIds().length).toBe(3);
    });
});

describe('indicatorSearch.filterResults', function () {

    var areaTypeToIndicatorIdsMap = { 1: [], 2: [1, 2], 3: [4, 5] };

    it('filterResults removes areas types with no indicator results',
        function () {

            var filtered = indicatorSearch.filterResults(areaTypeToIndicatorIdsMap);
            expect(_.size(filtered)).toBe(2);
        });
});


describe('indicatorSearch.filterAreaTypes', function () {

    var areaTypes = [{ Id: 1 }, { Id: 2 }, { Id: 3 }, { Id: 4 }];

    var areaTypeIds = ['2', '4'];

    it('filterAreaTypes only keeps those with specified IDs',
        function () {

            var filtered = indicatorSearch.filterAreaTypes(areaTypes, areaTypeIds);
            expect(filtered.length).toBe(2);
        });
});

