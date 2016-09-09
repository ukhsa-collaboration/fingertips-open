
$(document).ready(function() {
    $('#select-simple-spreadsheet').click(function () {
        $("form#simpleUploadForm").submit();
        $('#spinner').show();
    });

    $('#select-batch-spreadsheet').click(function () {
        $("form#batchUploadForm").submit();
        $('#spinner').show();
    });

    $('#upload-Spreadsheet').click(function () {

        jQuery.fn.timer = function () {
        };
        // timer function end

        window.setInterval(function () {
            $("#example").timer();
        }, 200);

        $("form#uploadSpreadsheetForm").submit();
        $('#spinner').show();
    });

    $('#show-datatype-errors-link').click(function() {
        loading();
        lightbox.show($('#show-datatype-errors').html(), 250, 300, 600);
    });

    $('#show-duplicateData-errors-link').click(function() {
        loading();
        lightbox.show($('#show-duplicateData-errors').html(), 250, 300, 600);

        $('.show-spinner').click(function() {
            $("#infoBox").addClass("disable-infoBox");
            $("#spinner").show();
        });
    });

    $('#show-duplicateSpreadsheet-errors-link').click(function() {
        loading();
        lightbox.show($('#show-duplicateSpreadsheet-errors').html(), 250, 300, 600);
    });

    $('#insert-data-to-holding-table').click(function() {
        $("form#insertDataForm").submit();
        $('#spinner').show();
    });

    $('.excel-file').change(function () {
        var filePath = this.value;
        var ext = filePath.substring(filePath.lastIndexOf('.') + 1).toLowerCase();
        if (ext != 'xlsx' && ext != 'xls' && ext != 'csv') {
            showSimpleMessagePopUp('Only Excel files can be uploaded');
        } else {
            $('.browse-control').addClass('hidden');
            var uploadFile = $('.upload-browse');
            uploadFile.html('Selected File: ' + filePath);
            uploadFile.show();
            $('.select-spreadsheet').show();
        }
    });

    $('.upload-help').click(function() {
        var id = $(this).hasClass('upload-simple-help') ?
            '#uploadSimpleSummary' :
            '#uploadBatchSummary';

        lightbox.show($(id).html(), 30, 0, 650);
    });
});

function isFileNameOk(filePath) {
    var ext = filePath.substring(filePath.lastIndexOf('.') + 1).toLowerCase();
    return ext === 'xlsx' || ext === 'xls';
}

function uploadSimple() {
    toggleBrowseControl();
    $('.upload-batch').removeClass('selected').addClass('unselected');
    $('.upload-simple').addClass('selected').removeClass('unselected');
    $('#upload-batch-browse-control').addClass('hidden');
    $('#upload-simple-browse-control').removeClass('hidden');
}

function uploadBatch() {
    toggleBrowseControl();
    $('.upload-simple').removeClass('selected').addClass('unselected');
    $('.upload-batch').addClass('selected').removeClass('unselected');
    $('#upload-simple-browse-control').addClass('hidden');
    $('#upload-batch-browse-control').removeClass('hidden');
}

function toggleBrowseControl() {
    $('.select-spreadsheet').hide();
    $('.browse-control').removeClass('hidden');
    $('.upload-browse').hide();
}
