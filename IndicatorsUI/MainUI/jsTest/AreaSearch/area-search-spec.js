describe('areaSearchResults.AreaListProcessor', function () {

    var getAreaList = function() {
        return [
            { AreaTypeId: AreaTypeIds.County },
            { AreaTypeId: AreaTypeIds.UnitaryAuthority },
            { AreaTypeId: AreaTypeIds.District }
        ];
    };

    it('assignCompositeAreaTypeId', function () {

        var areaTypeId = AreaTypeIds.CountyUA;

        var areaList = new areaSearchResults.AreaListProcessor(getAreaList(), areaTypeId)
            .assignCompositeAreaTypeId()
            .getAreaList();

        expect(areaList[0].CompositeAreaTypeId).toEqual(areaTypeId);
        expect(areaList[1].CompositeAreaTypeId).toEqual(areaTypeId);
        expect(areaList[2].CompositeAreaTypeId).toEqual(areaTypeId);
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
