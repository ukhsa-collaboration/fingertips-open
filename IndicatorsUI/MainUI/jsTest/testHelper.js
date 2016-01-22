function HtmlChecker(html) {

    this.validate = function() {
        // Equal number of < and >
        expect(html.split('<').length).toBe(html.split('>').length);

        // Equal number of double quotes
        expect(html.split('"').length).toBe(html.split('"').length);

        // Equal number of single quotes
        expect(html.split("'").length).toBe(html.split("'").length);
    };
}

TestProfileIds = {
    SubstanceMisuse: 17,
    Tobacco : 18,
    Phof : 19,
    HealthProfiles: 26,
    Neurology:61
}
