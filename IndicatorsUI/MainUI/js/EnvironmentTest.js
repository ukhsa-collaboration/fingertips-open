/**
* Functions only used in a test environment. This file is included on Fingertips data page but only for a test environment
* @module EnvironmentTest
*/

var environmentTest = {};

environmentTest.getProfileUrlKey = function() {

    var profileId = FT.model.profileId;

    // Where PDF url key is different to Fingertips url key
    var lookUp = {};
    lookUp[ProfileIds.HealthProfiles] = 'hp2';
    lookUp[ProfileIds.CommunityMentalHealth] = 'cmhp';

    return lookUp[profileId]
        ? lookUp[profileId]
        : profileUrlKey;
};

/**
* Gets the URL for a PDF.
* @class getPdfUrl
*/
function getPdfUrl(areaCode) {

    var profileId = FT.model.profileId;
    var pdfUrlKey = environmentTest.getProfileUrlKey();

    // Display please wait message
    $('#pdf-download-text').html(
        '<h2>Please wait</h2><p>The PDF is being generated, please be patient this may take a while...</p>');

    // Determine host
    if (profileId === ProfileIds.HealthProfiles) {
        var url = 'https://londevapppor01.phe.gov.uk:5022/onthefly/api/api.php';
    } else {
        url = 'https://pdfs.nepho.org.uk/onthefly/api/api.php';
    }

    // Return URL with parameters
    return url + '?c=profile&a=generatePDF&i=' + pdfUrlKey +
        '&area=' + areaCode + 
        '&region=' + FT.model.parentCode +
        '&areaTypeId=' + FT.model.areaTypeId +
        '&groupId=6&output=browser';
}

download.excelExportText =
        "Excel files for all England data are now pre-calculated every night so any data that has been uploaded or changed today will not be included.";

