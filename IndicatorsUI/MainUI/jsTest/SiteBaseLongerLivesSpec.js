
describe('getCardinal', function() {

    var expectCardinal = function(num, suffix)
    {
        expect(getCardinal(num)).toEqual(suffix);
    }
    var th = function(num) {
        expectCardinal(num,'th');
    }
    var st = function (num) {
        expectCardinal(num,'st');
    }
    var nd = function (num) {
        expectCardinal(num,'nd');
    }
    var rd = function (num) {
        expectCardinal(num,'rd');
    }

    it('1 to 5', function() {
        st(1);
        nd(2);
        rd(3);
        th(4);
        th(5);
    });

    it('10 to 15', function () {
        th(10);
        th(11);
        th(12);
        th(13);
        th(14);
        th(15);
    });

    it('20 to 25', function () {
        th(20);
        st(21);
        nd(22);
        rd(23);
        th(24);
        th(25);
    });

    it('101', function () { st(101); });
    it('102', function () { nd(102); });
    it('103', function () { rd(103); });
    it('104', function () { th(104); });
    it('105', function () { th(105); });

    it('110', function () { th(110); });
    it('111', function () { th(111); });
    it('112', function () { th(112); });
    it('113', function () { th(113); });
    it('114', function () { th(114); });
    it('115', function () { th(115); });
});

describe('AreaFilterModelBuilder', function() {

    var TestData = function () {

        var areaType1 = { Short: '1', Id: 1 };

        this.one = [areaType1];

        this.two = [
            areaType1,
            { Short: '2', Id: 2 }
        ];
    };

    it('isOneType is true', function () {
        var model = new AreaFilterModelBuilder(new TestData().one, 1);
        expect(model.getModel().isOneType).toEqual(true);
    });

    it('isOneType is false', function () {
        var model = new AreaFilterModelBuilder(new TestData().two, 1);
        expect(model.getModel().isOneType).toEqual(false);
    });

    it('selected area type class is assigned correctly', function () {
        var model = new AreaFilterModelBuilder(new TestData().two, 2).getModel();

        expect(model.types[0].hasOwnProperty('class')).toBe(false);
        expect(model.types[1]['class']).toEqual('active');
    });

});
