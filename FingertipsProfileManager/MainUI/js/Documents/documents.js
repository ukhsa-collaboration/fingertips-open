function documentReady() {
    initTableSorter();

    $('.sortHeader').click(function() {
        loading();
    });

    $('.sortDesc').click(function() {
        loading();
    });

    $('.sortAsc').click(function() {
        loading();
    });

    $('#profileId').change(function() {
        $('#selectedProfileForm').submit();
    });

    $('#fileToBeUploaded').change(function() {
        var filePath = this.value;
        if (filePath !== '' || filePath !== 'undefined') {
            $('.browse-control').addClass('hidden');
            var selectedUploadFile = $('#selectedUploadFile');
            selectedUploadFile.html('Selected File: ' + filePath);
            selectedUploadFile.removeClass('hidden');
            upload();
        } 
    });

    // Init delete document buttons
    $('.document-delete').click(function (evt) {
        evt.preventDefault();
        var $deleteLink = $(this);
        var documentId = $deleteLink.attr("documentId");

        lightbox.show("<h3>Delete document</h3>" +
            "<p>This document will be deleted and no copy will be retained.</p><p>Are you sure you want to go ahead?</p>" +
            '<div class="form-group"><button class="btn btn-primary active" onclick="deleteDocument(' + documentId + ')">Confirm</button> ' +
            '<button class="btn btn-secondary active" onclick="lightbox.hide()">Cancel</button></div>',
            20, 400, 600);
    });

    // Init publish document buttons
    $('.document-publish').click(function (evt) {
        evt.preventDefault();
        var $publishLink = $(this);
        var documentId = $publishLink.attr("documentId");

        lightbox.show("<h3>Publish document</h3>" +
            "<p>This document will be published to Live.</p><p>Are you sure you want to go ahead?</p>" +
            '<div class="form-group"><button class="btn btn-primary active" onclick="publishDocument(' + documentId + ')">Confirm</button> ' +
            '<button class="btn btn-secondary active" onclick="lightbox.hide()">Cancel</button></div>',
            150, 400, 400);
    });
}

function deleteDocument(documentId) {

    $.post("documents/delete?id=" + documentId
      ).done(function () {
          location.reload();
      }).fail(function () {
          alert("Failed to delete");
      });
}

function publishDocument(documentId) {
    $.post("documents/publish?id=" + documentId
    ).done(function () {
        $('#document-publish-live-td-' + documentId).html('<span class="update-success">Published</span>');
        lightbox.hide();
    }).fail(function () {
        showSimpleMessagePopUp('Sorry, that did not work');
    });
}

function showUploadControl() {
    $('#upload-control').show();
}

function upload() {
    var profileId = $('#profileId').val(),
        fullPath = $('#fileToBeUploaded').val(),
        filename = fullPath.replace(/^.*[\\\/]/, '');

    $.get("/documents/is_filename_unique", { profileId: profileId, fileName: filename }
    ).done(function(data) {
        uploadCallback(data);
    }).fail(function() {
        //alert("The filename must be unique");
        lightbox.show("The filename must be unique", 250, 300, 600);
        resetUploadControls();
    });
}

var uniqueness = {
    NotUnique : 0,
    UniqueToProfile : 1,
    Unique : 2
};

function uploadCallback(data) {    
    switch (data) {
    case uniqueness.NotUnique:
        resetUploadControls();      
        lightbox.show($('#error-message').html(), 250, 300, 600);
        break;
    case uniqueness.UniqueToProfile:
        lightbox.show($('#overwrite-message').html(), 250, 300, 600);
        break;
    case uniqueness.Unique:
        submitForm();
        break;
    }
}

function clearInputFieldValues() {
    $('#fileToBeUploaded').val('');
    $('#selectedUploadFile').html('');
}

function hideUploadControls() {
    $('#selectedUploadFile').addClass('hidden');
    $('.browse-control').removeClass('hidden');
}

function resetUploadControls() {
    hideUploadControls();
    clearInputFieldValues();
}

function submitForm() {
    $('#documentUpload').submit();
}

function donotOverwrite() {
    lightbox.hide();
    hideUploadControls();
    clearInputFieldValues();
}

$(document).ready(documentReady);