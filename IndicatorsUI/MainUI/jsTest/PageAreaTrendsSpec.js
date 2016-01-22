

describe('TrendData', function () {

    var getBenchmarkPoints = function(point) {
        var trendData = new TrendData();
        trendData.addBenchmarkPoint(point,point);
        return trendData.getBenchmarkPoints();
    };

    var getAreaPoints = function (point, markerColour) {
        var trendData = new TrendData();
        trendData.addAreaPoint(point, markerColour);
        return trendData.getAreaPoints();
    };

    it('addBenchmarkPoint y is set correctly', function () {
        expect(getBenchmarkPoints(1)[0].y).toBe(1);
    });

    it('addBenchmarkPoint y is null if value is null', function () {
        expect(getBenchmarkPoints(null)[0].y).toBe(null);
    });

    it('addAreaPoint y is set correctly', function () {
        expect(getAreaPoints({ D: 1 })[0].y).toBe(1);
    });

    it('addAreaPoint y is null if D is null', function () {
        expect(getAreaPoints({ D: null })[0].y).toBe(null);
    });

    it('addAreaPoint markerColour is set', function () {
        expect(getAreaPoints({ D: 1 }, 'a')[0].color).toBe('a');
    });

    it('addAreaPoint noteId is set', function () {
        expect(getAreaPoints({ D: 1, NoteId: 2 })[0].noteId).toBe(2);
    });
});


describe('TrendTableRow', function () {

    it('toggleValueCellHighlights ignored if cells are not defined', function () {
        var row = new TrendTableRow();
        row.toggleValueCellHighlights();
    });

    it('unhighlight ignored if cells are not defined', function () {
        var row = new TrendTableRow();
        row.unhighlight();
    });
});


