describe('inequalities.ViewManager', function () {

    it('init only has effect on first call', function () {
        var html1 = '<div></div>';
        var $container = $(html1);
        var viewManager = new inequalities.ViewManager($container);

        // First call
        var html2 = viewManager.init();
        expect(html2).not.toBe(html1);

        // Second call
        var html3 = viewManager.init();
        expect(html3).toBe(html2);
    });
});

describe('inequalities.CategoryDataManager', function () {

    it('setData', function () {
        var dataManager = new inequalities.CategoryDataManager();
        var groupRoot = { IID: 1, SexId: 2, AgeId: 3 };
        var areaTypeId = 4;

        dataManager.setData(groupRoot, 'a', areaTypeId, 5);

        expect(dataManager.getData(groupRoot, 'a', areaTypeId)).toBe(5);
    });
});

describe('inequalities.BarChartTooltip', function () {

    var averageLabel = 'average';
    var unitLabel = 'unit';
    var tooltip = new inequalities.BarChartTooltip(
        { Id: 1, Label: unitLabel }, averageLabel, ['category label']);

    it('getHtml for average', function () {

        var highchartsObject = {
            series: { index: 1 },
            point: { ValF: '3' }
        };

        var html = tooltip.getHtml(highchartsObject);

        // Contains average label
        expect(html).toContain(averageLabel);

        // Contains ValF
        expect(html).toContain(3);

        // Contains unit
        expect(html).toContain(unitLabel);
    });

    it('getHtml for category', function () {

        var highchartsObject = {
            series: { index: 0 },
            point: { ValF: '3', index:0 },
            x: 'category'
        };

        var html = tooltip.getHtml(highchartsObject);

        //  Does not contain average label
        expect(html).not.toContain(averageLabel);

        // Contains data label
        expect(html).toContain('category label');

        // Contains ValF
        expect(html).toContain(3);

        // Contains unit
        expect(html).toContain(unitLabel);
    });
});


describe('inequalities.BarLabelOffsetCalculator', function () {

    var getOffset = function (data) {
        return new inequalities.BarLabelOffsetCalculator(data).offset;
    }

    it('setData 3 digits hundreds', function () {
        var data = [{ Val: 111, ValF: '111' }];
        expect(getOffset(data)).toBe(25);
    });

    it('setData 3 digits with decimal point', function () {
        var data = [{ Val: 1.1, ValF: '1.1' }];
        expect(getOffset(data)).toBe(22);
    });

    it('setData 4 digits with decimal point', function () {
        var data = [{ Val: 11.1, ValF: '11.1' }];
        expect(getOffset(data)).toBe(29);
    });

    it('setData 5 digits thousands', function () {
        var data = [{ Val: 1111, ValF: '1,111' }];
        expect(getOffset(data)).toBe(36);
    });
});


describe('inequalities.CategoryDataAnalyser', function () {

    it('isAnyData false if empty Data array', function () {

        var dataManager = new inequalities.CategoryDataAnalyser({ Data: [] });
        expect(dataManager.isAnyData).toBe(false);
    });

    it('isAnyData false if Data array only contains -1 Vals', function () {

        var dataManager = new inequalities.CategoryDataAnalyser({ Data: [{ Val: -1 }] });
        expect(dataManager.isAnyData).toBe(false);
    });

    it('isAnyData true if Data contains valid Vals', function () {

        var dataManager = new inequalities.CategoryDataAnalyser({ Data: [{ Val: 1 }] });
        expect(dataManager.isAnyData).toBe(true);
    });

    it('getCategoryTypeById', function () {

        var dataManager = new inequalities.CategoryDataAnalyser({
            Data: [{ Val: 1, CategoryTypeId: 2 }],
            CategoryTypes: [{ Id: 2 }]
        });

        expect(dataManager.getCategoryTypeById(2).Id).toBe(2);
    });

    it('getCategoryLabels', function () {

        var dataManager = new inequalities.CategoryDataAnalyser({
            Data: [{ Val: 1, CategoryTypeId: 2 }],
            CategoryTypes: [
            {
                Id: 2,
                Categories: [{ Name: 'a' }, { Name: 'b' }]
            }]
        });

        var labels = dataManager.getCategoryLabels(2);
        expect(labels.length).toBe(2);
        expect(labels[0]).toBe('a');
        expect(labels[1]).toBe('b');
    });

    it('only categoryTypes with data are kept', function () {

        var dataManager = new inequalities.CategoryDataAnalyser({
            Data: [{ Val: 1, CategoryTypeId: 2 }],
            CategoryTypes: [{ Id: 2 }, { Id: 3 }]
        });

        var types = dataManager.categoryTypes;
        expect(types.length).toBe(1);
        expect(types[0].Id).toBe(2);

        // Is data returned
        expect(dataManager.getDataByCategoryTypeId(2).length).toBe(1);
    });

});

