
describe('getIndexOfGroupRootThatContainsIndicator', function () {

    it('correct index if found', function () {

        var roots = [
        { IID: 2 },
        { IID: 20 },
        { IID: 200 }
        ];

        var index = getIndexOfGroupRootThatContainsIndicator(2, roots);
        expect(index).toBe(0);

        index = getIndexOfGroupRootThatContainsIndicator(20, roots);
        expect(index).toBe(1);

        index = getIndexOfGroupRootThatContainsIndicator(200, roots);
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