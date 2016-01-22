describe('AreaDataCollection', function () {

    it('addData then getData', function () {

        var areaTypeId = 5;

        var obj1 = 11,
            obj2 = 22,
            obj3 = 33,
            obj4 = 44;

        var collection = new AreaDataCollection();
        collection.addData(1, 'a', areaTypeId, obj1);
        collection.addData(1, 'b', areaTypeId, obj2);
        collection.addData(2, 'c', areaTypeId, obj3);
        collection.addData(2, 'd', areaTypeId, obj4);

        expect(collection.getData(1, 'a', areaTypeId)).toEqual(obj1);
        expect(collection.getData(1, 'b', areaTypeId)).toEqual(obj2);
        expect(collection.getData(2, 'c', areaTypeId)).toEqual(obj3);
        expect(collection.getData(2, 'd', areaTypeId)).toEqual(obj4);
    });

});
