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

    // Determine host
    var url = FT.url.pdf + pdfUrlKey ;
    if (profileId === ProfileIds.HealthProfiles) {
        url = url + '/' + download.timePeriod;
    } 

    // Return URL with parameters
    return url + '/' + areaCode + '.pdf';
}

download.excelExportText =
        'Data files for many profiles are now pre-built every night so any data that has been uploaded or changed today will not be included.' +
        '<br><br>Downloaded data is only available if your machine is on the PHE network.';

