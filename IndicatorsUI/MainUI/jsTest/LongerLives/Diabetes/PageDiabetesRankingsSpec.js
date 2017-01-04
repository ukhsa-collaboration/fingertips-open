
describe('getIndexOfGroupRootThatContainsIndicator', function () {

    it('correct index if found', function () {

        var roots = [
        { IID: 2, Sex: {Id:1} },
        { IID: 20, Sex: { Id: 2 } },
        { IID: 200, Sex: { Id: 4 } }
        ];

        var model = { indicatorId: 2, sexId: 1 };
        var index = getIndexOfGroupRootThatContainsIndicator(model, roots);
        expect(index).toBe(0);

        model = { indicatorId: 20, sexId: 2 };
        index = getIndexOfGroupRootThatContainsIndicator(model, roots);
        expect(index).toBe(1);

        model = { indicatorId: 200, sexId: 4 };
        index = getIndexOfGroupRootThatContainsIndicator(model, roots);
        expect(index).toBe(2);
    });
});


describe('ColumnHeader', function () {

    var metadata = {
        Descriptive:
        {
            Name: 'zz'
        },
        Unit: {
            Id: 1,
            Label: 'yy'
        }
    };

    it('unit is included if unit is not proportion', function () {
        var text = new ColumnHeader(metadata).text;
        expect(text).toContain('zz');
        expect(text).toContain('yy');
    });

    it('unit is included if unit is proportion', function () {

        metadata.Unit.Id = 5;
        var text = new ColumnHeader(metadata).text;
        expect(text).toContain('zz');
        expect(text).not.toContain('yy');
    });
});