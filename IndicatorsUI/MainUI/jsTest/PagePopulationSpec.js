

describe('PopulationNumber', function () {

    it('1000 formatted "1,000"', function () {

        var number = new PopulationNumber({ Val: 1000 });

        expect(number.val()).toBe('1,000');
        expect(number.isData()).toBe(true);
    });

    it('0 is no data', function () {

        var number = new PopulationNumber({ Val: 0 });
        expect(number.isData()).toBe(false);
    });

});

describe('Population', function () {

    it('Null population', function () {

        var population = new Population(null);

        expect(population.male.length).toBe(0);
        expect(population.female.length).toBe(0);
    });

    it('male and female assigned correctly', function () {

        var population = new Population({
            Values: {
                1: [1],
                2: [1, 2]
            }
        });

        expect(population.male.length).toBe(1);
        expect(population.female.length).toBe(2);
    });
});
