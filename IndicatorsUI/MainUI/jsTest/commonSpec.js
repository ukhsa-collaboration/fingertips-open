describe('ParameterBuilder', function() {
        
        it('Empty string if no parameters', function() {            
                var builder = new ParameterBuilder();
                expect(builder.build()).toEqual('');
        });
        
        it('Builds with one parameter', function() {            
                var builder = new ParameterBuilder();
                builder.add('s',1);
                expect(builder.build()).toEqual('s=1');
        });
        
        it('Builds with two parameters', function() {            
                var builder = new ParameterBuilder();
                builder.add('s',1);
                builder.add('t',2);
                expect(builder.build()).toEqual('s=1&t=2');
        });

        it('Concaternates integer array values', function () {
            var builder = new ParameterBuilder();
            builder.add('s', [1,2]);
            expect(builder.build()).toEqual('s=1,2');
        });
});

describe('roundNumber', function() {
        
        it('Rounds down to 0 decimal places', function() {            
                expect(roundNumber(1.2,0)).toEqual(1);
        });
        
        it('Rounds up to 0 decimal places', function() {            
                expect(roundNumber(1.8,0)).toEqual(2);
        });
        
        it('Rounds down to 1 decimal place', function() {            
                expect(roundNumber(1.22,1)).toEqual(1.2);
        });
        
        it('Rounds up to 1 decimal place', function() {            
                expect(roundNumber(1.88,1)).toEqual(1.9);
        });
        
        it('Rounds down to 2 decimal places', function() {            
                expect(roundNumber(1.222,2)).toEqual(1.22);
        });
        
        it('Rounds up to 2 decimal places', function() {            
                expect(roundNumber(1.888,2)).toEqual(1.89);
        });
});

describe('sortNumericAsc', function () {

    it('lowest is first and highest is last', function () {

        var sorted = [2,5,1,26,3].sort(sortNumericAsc);

        var min = sorted[0];
        var max = _.last(sorted);

        expect(min).toEqual(1);
        expect(max).toEqual(26);
    });

});

describe('CommaNumber', function () {

    it('rounded with integer', function () {
        expect(new CommaNumber(1234).rounded()).toBe('1,234');
    });

    it('rounded rounds down', function () {
        expect(new CommaNumber(1234.493145).rounded()).toBe('1,234');
    });

    it('rounded rounds up', function () {
        expect(new CommaNumber(1234.562345).rounded()).toBe('1,235');
    });

    it('unrounded 4dp', function () {
        expect(new CommaNumber(1234.4444).unrounded()).toBe('1,234.4444');
    });

    it('unrounded 3dp', function () {
        expect(new CommaNumber(1234.444).unrounded()).toBe('1,234.444');
    });

    it('unrounded 2dp', function () {
        expect(new CommaNumber(1234.44).unrounded()).toBe('1,234.44');
    });

    it('unrounded 1dp', function () {
        expect(new CommaNumber(1234.4).unrounded()).toBe('1,234.4');
    });

    it('unrounded 0dp', function () {
        expect(new CommaNumber(1234).unrounded()).toBe('1,234');
    });
});

describe('isFeatureEnabled', function () {

    FT.features = ['ExportTartanRug', 'ExportHighCharts'];

    it('Feature flag is defined', function() {
        expect(isFeatureEnabled('ExportHighCharts')).toBe(true);
    });

    it('Feature flag is not defined', function () {
        expect(isFeatureEnabled('Undefined')).toBe(false);
    });
});

describe('isDefined', function () {

    it('object is defined', function () {
        expect(isDefined(1)).toBe(true);
    });

    it('null is not defined', function () {
        expect(isDefined(null)).toBe(false);
    });

    it('undefined variable is not defined', function () {
        var notDefined;
        expect(isDefined(notDefined)).toBe(false);
    });
});

describe('ftHistory.isParameterDefinedInHash', function() {

    it('where parameter is defined', function() {
        ftHistory.getHash = function() { return 'a/1'; }
        expect(ftHistory.isParameterDefinedInHash('a')).toBe(true);
    });

    it('where parameter is not defined', function () {
        ftHistory.getHash = function () { return 'a/1'; }
        expect(ftHistory.isParameterDefinedInHash('b')).toBe(false);
    });
});

describe('ftHistory.parseParameterString', function () {

    it('empty hash from empty string', function () {
        var hash = ftHistory.parseParameterString('');
        expect(_.size(hash)).toBe(0);
    });

    it('empty hash from null string', function () {
        var hash = ftHistory.parseParameterString(null);
        expect(_.size(hash)).toBe(0);
    });

    it('hash separated by / parsed correctly', function () {
        var hash = ftHistory.parseParameterString('a/1/b/2');
        expect(_.size(hash)).toBe(2);
        expect(hash.a).toBe('1');
        expect(hash.b).toBe('2');
    });

    it('hash separated by , parsed correctly', function () {
        var hash = ftHistory.parseParameterString('a,1,b,2');
        expect(_.size(hash)).toBe(2);
        expect(hash.a).toBe('1');
        expect(hash.b).toBe('2');
    });

    it('empty parameters are ignored', function () {
        var hash = ftHistory.parseParameterString('a//b/2');
        expect(_.size(hash)).toBe(1);
        expect(hash.b).toBe('2');
    });
});


describe('getTrendMarkerImage', function () {

    it('should return html that contains up-red', function () {
        var trendMarker = 1;
        var polarity = PolarityIds.RAGLowIsGood;
        var result = getTrendMarkerImage(trendMarker, polarity);
        expect(result).toContain('up_red');
    });

    it('should return html that contains up-green', function () {
        var trendMarker = 1;
        var polarity = PolarityIds.RAGHighIsGood;
        var result = getTrendMarkerImage(trendMarker, polarity);
        expect(result).toContain('up_green');
    });

    it('should return html that contains down-green', function () {
        var trendMarker = 2;
        var polarity = PolarityIds.RAGLowIsGood;
        var result = getTrendMarkerImage(trendMarker, polarity);
        expect(result).toContain('down_green');
    });

    it('should return html that contains down-red', function () {
        var trendMarker = 2;
        var polarity = PolarityIds.RAGHighIsGood;
        var result = getTrendMarkerImage(trendMarker, polarity);
        expect(result).toContain('down_red');
    });

    it('should return html that contains no-change', function () {
        var trendMarker = 3;
        var polarity = PolarityIds.RAGLowIsGood;
        var result = getTrendMarkerImage(trendMarker, polarity);
        expect(result).toContain('no_change');
    });

    it('should return html that contains no-calc', function () {
        var trendMarker = null;
        var polarity = PolarityIds.RAGLowIsGood;
        var result = getTrendMarkerImage(trendMarker, polarity);
        expect(result).toContain('no_calc');
    });

});

describe('Testing global methods',  function(){

    beforeEach(function() {
        FT.model.groupRoots=[{IID: 1},{IID: 2},{IID: 3}];
        FT.model.areaCode = "mainFakeAreaCode";
        spyOn(jQuery.fn, 'prop').and.callFake(function() { return 0 });
        spyOn(window, 'getChildAreas').and.returnValue([{Code: "fakeAreaCode"}]);
    });

    it ('should get indicator id', function (){
        var resultIid = getIid();

        expect(resultIid).toBe(1);
    });

    it ('should get areas code displayed', function (){
        FT.model.isNearestNeighbours = function () { return false; };
        var resultIid = getAreasCodeDisplayed();

        expect(resultIid.length).toBe(1);
        expect(resultIid[0]).toBe("fakeAreaCode");
    });

    it ('should get areas code displayed for nearest neighbours', function (){
        FT.model.isNearestNeighbours = function () { return true; };
        var resultIid = getAreasCodeDisplayed();

        expect(resultIid.length).toBe(2);
        expect(resultIid[0]).toBe("fakeAreaCode");
        expect(resultIid[1]).toBe("mainFakeAreaCode");
    });

    it ('should get parent area code', function (){
        FT.model.parentCode = "fakeParentAreaCode";
        FT.model.isNearestNeighbours = function () { return false; };
        var resultIid = getParentAreaCode();

        expect(resultIid).toBe("fakeParentAreaCode");
    });

    it ('should get England as parent area code', function (){
        FT.model.parentCode = "fakeParentAreaCode";
        FT.model.isNearestNeighbours = function () { return true; };
        var resultIid = getParentAreaCode();

        expect(resultIid).toBe(NATIONAL_CODE);
    });
});


