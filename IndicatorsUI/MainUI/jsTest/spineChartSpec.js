describe('SpineChartStems', function () {

    it('5 keys, one for each section of the spine chart', function () {
        var stems = new SpineChartStems({ min: '', max: '' });
        expect(stems.getKeys().length).toEqual(5);
    });
});
