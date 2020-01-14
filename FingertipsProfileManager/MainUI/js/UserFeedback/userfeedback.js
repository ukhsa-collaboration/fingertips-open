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

function feedbackCommentAndArchive() {
    var id = selectedFeedbackId,
        comment = $('#feedback-comment').val();

     if (!_.isUndefined(id) || !_.isNull(id) || id.trim().length !== 0) {
        $.post("/user-feedback/archive", { id:id, comment: comment})
            .done(function () {
                location.reload(true);
            })
            .fail(function () {
                "Failed to archive selected user feedback";
            });
     }    
}

function closeUserFeedbackItem() {

    var comment = $('#feedback-comment').val().trim();

    if (comment.length) {
        saveUserFeedbackItem("/user-feedback/archive");
    } else {
        showSimpleMessagePopUp("Add a comment");
    }
}

function saveUserFeedbackItem(url) {
    var id = $('#Id').val();
    var comment = $('#feedback-comment').val().trim();

    $.post(url, { id: id, comment: comment })
        .done(function () {
            setUrl("/user-feedback");
        })
        .fail(function (e) {
            showSimpleMessagePopUp("Failed to close");
        });
}

function addComment(comment) {
    var $comment = $('#feedback-comment');

    var text = $comment.val().trim();

    if (text.length) {
        text += '. ' + comment;
    } else {
        text = comment;
    }

    $comment.val(text);
}

function clearComment() {
    $('#feedback-comment').val('');
}


function feedbackCancel() {
    $('#feedback-comment').val('');
    lightbox.hide();
}

var selectedFeedbackId;
$(document).ready(documentRead);