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