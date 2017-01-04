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

    $('#selectedProfile').change(function() {
        $('form#DocManagementForm').submit();
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
            "<p>This document will be deleted and no copy will be retained.</p>" +
            "<p>Are you sure you want to go ahead?</p>" +
            '<button class="btn btn-primary active" onclick="deleteDocument(' + documentId + ')">Confirm</button>' +
            '<button class="btn btn-secondary active" onclick="lightbox.hide()">Cancel</button>',
            20, 400, 600);
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

function showUploadControl() {
    $('#upload-control').show();

    var selectedProfileId = $('#selectedProfile').val();
    $('#selectedProfileId').val(selectedProfileId);
}

function upload() {
    var selectedProfileId = $('#selectedProfile').val(),
        fullPath = $('#fileToBeUploaded').val(),
        filename = fullPath.replace(/^.*[\\\/]/, '');

    $.get("/validateDocumentName", { fileName: filename, selectedProfileId: selectedProfileId }
    ).done(function(data) {
        uploadCallback(data);
    }).fail(function() {
        alert("failed to validate");
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
        hideUploadControls();
        clearInputFieldValues();        
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
    $('#selectedProfileId').val('');
    $('#fileToBeUploaded').val('');
    $('#selectedUploadFile').html('');
}

function hideUploadControls() {
    $('#selectedUploadFile').addClass('hidden');
    $('.browse-control').removeClass('hidden');
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