'use strict';

/**
* Defined in PageMetadata.js
* @module metadata
*/

function goToMetadataPage(rootIndex) {

    lock();

    setPageMode(PAGE_MODES.METADATA);

    if (!areIndicatorsInDomain()) {
        displayNoData();
    } else {

        // Establish indicator
        if (!isDefined(rootIndex)) {
            rootIndex = getIndicatorIndex();
        }
        setIndicatorIndexAndUpdateModel(rootIndex);

        callAngularEvent('MetadataSelected');

        if ($(window).scrollTop() > 300) {
            $(window).scrollTop(0);
        }
    }
};

function setIndicatorIndexAndUpdateModel(i) {
    setIndicatorIndex(i);

    // Update model
    var root = groupRoots[i];
    var model = FT.model;
    model.iid = root.IID;
    model.ageId = root.Age.Id;
    model.sexId = root.Sex.Id;
}

pages.add(PAGE_MODES.METADATA, {
    id: 'metadata',
    title: 'Definitions',
    goto: goToMetadataPage,
    gotoName: 'goToMetadataPage',
    needsContainer: false,
    jqIds: ['metadata-container', 'indicator-menu-div', 'areaTypeBox']
});
