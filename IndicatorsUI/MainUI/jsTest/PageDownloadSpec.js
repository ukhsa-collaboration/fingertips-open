
describe('PdfFileNamer', function () {

    it('name is pdf19.png for PHOF profile', function () {
        expect(new PdfFileNamer(TestProfileIds.Phof).name).toBe('pdf19.png');
    });

    it('name is pdf18.png for Tobacco profile', function () {
        expect(new PdfFileNamer(TestProfileIds.Tobacco).name).toBe('pdf18.png');
    });

    it('name is pdf17.png for Substance misuse profile', function () {
        expect(new PdfFileNamer(TestProfileIds.SubstanceMisuse).name).toBe('pdf17.png');
    });
});

