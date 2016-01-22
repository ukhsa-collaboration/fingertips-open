/*
* This file is included on Fingertips data page but only for a test environment
*/

/**
* Functions only used in a test environment. Defined in EnvironmentTest.js.
* @module EnvironmentTest
*/

/**
* Uses the on-the-fly PDF generation. This function is only used on the test site.
* @class exportPdf
*/
function exportPdf(areaCode, profileId) {

    if (profileId === ProfileIds.Liver) {
        // Liver profiles
        var url = 'http://www.endoflifecare-intelligence.org.uk/profiles/liver-disease/' + 
            areaCode + '.pdf';
    } else {

        // Where PDF url key is different to Fingertips url key
        var lookUp = {
            26: 'hp2',
            50: 'cmhp'
        };

        var pdfUrlKey = lookUp[profileId]
            ? lookUp[profileId]
            : profileUrlKey;

        lock();

        $('#download-text').html(
            '<h2>Please wait</h2><p>The PDF is being generated, please be patient this may take a while...</p>');

        url = 'http://www.nepho.org.uk/maps/dev/pdfonthefly/?f=' + pdfUrlKey +
            '&area_code=' + areaCode +
            '&region_code=' + FT.model.parentCode +
            '&child_area_type_id=' + FT.model.areaTypeId +
            '&clear_cache=silent';
    }

    window.open(url.toLowerCase(), '_blank');
}

excelExportText = "Excel files for all England data are now pre-calculated every night so any data that has been uploaded or changed today will not be included.";