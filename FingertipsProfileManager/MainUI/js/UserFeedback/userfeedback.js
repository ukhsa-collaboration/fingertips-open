function documentRead() {
    $('.sortable').tablesorter({
        theme: 'blue',
        widthFixed: true,
        widgets: ['zebra'],
        headers: {
            2: {
                sorter: false
            },
            3: {
                sorter: false
            },
            4: {
                sorter: false
            },
            5: {
                sorter: false
            },
            6: {
                sorter: false
            }
        }
    });

    $('[data-toggle="tooltip"]').tooltip();
}

function feedbackDetails(id) {
    selectedFeedbackId = id;
    lightbox.show($('#feedback-detail').html(), 300, 300, 700);
}


function feedbackCommentAndArchive() {
    var id = selectedFeedbackId,
        comment = $('#feedback-comment').val();

     if (!_.isUndefined(id) || !_.isNull(id) || id.trim().length !== 0) {
        $.post("/userfeedback/archive", { id:id, comment: comment})
            .done(function () {
                location.reload(true);
            })
            .fail(function () {
                "Failed to archive selected user feedback";
            });
     }    
}

function feedbackCancel() {
    $('#eedback-comment').val('');
    lightbox.hide();
}

var selectedFeedbackId;
$(document).ready(documentRead);