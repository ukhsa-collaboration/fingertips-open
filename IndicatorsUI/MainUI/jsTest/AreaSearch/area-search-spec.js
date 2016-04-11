describe('areaSearchResults.AreaListProcessor', function () {

    var getAreaList = function() {
        return [
            { AreaTypeId: AreaTypeIds.County },
            { AreaTypeId: AreaTypeIds.UnitaryAuthority },
            { AreaTypeId: AreaTypeIds.District }
        ];
    };

    it('assignCompositeAreaTypeId', function () {

        var areaList = new areaSearchResults.AreaListProcessor(getAreaList())
            .assignCompositeAreaTypeId()
            .getAreaList();

        expect(areaList[0].CompositeAreaTypeId).toEqual(AreaTypeIds.CountyUA);
        expect(areaList[1].CompositeAreaTypeId).toEqual(AreaTypeIds.CountyUA);
        expect(areaList[2].CompositeAreaTypeId).toEqual(AreaTypeIds.DistrictUA);
    });

    it('removeUAs', function () {

        var areaList = new areaSearchResults.AreaListProcessor(getAreaList())
            .removeUAs()
            .getAreaList();

        expect(areaList.length).toEqual(2);
        expect(areaList[0].AreaTypeId).toEqual(AreaTypeIds.County);
        expect(areaList[1].AreaTypeId).toEqual(AreaTypeIds.District);
    });
});

describe('areaSearchResults.findAreaIncludedInSearchResults', function () {

    it('findAreaIncludedInSearchResults finds correct area', function () {

        var resultsList = [
            { PolygonAreaCode: 'd' },
            { PolygonAreaCode: 'a' }
        ];

        var areaList = [
           { Code: 'a' },
           { Code: 'b' }
        ];

        areaList = areaSearchResults.findAreaIncludedInSearchResults(areaList, resultsList);

        expect(areaList.length).toEqual(1);
        expect(areaList[0].Code).toEqual('a');
    });

});
