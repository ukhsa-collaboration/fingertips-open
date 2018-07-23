/**
 * Open email dialog to enable users to share an indicator list
 */
function openEmailDialog(e, publicId, listName) {

    // Determine HTTP protocol
    var parser = document.createElement('a');
    parser.href = window.location.href;
    var protocol = parser.protocol;

    var subject = "Public Health Data";

    var newLine = '%0D%0A';

    var mailTo = [
        "mailto:?subject=", subject, "&body=", newLine, newLine, listName, " ",
        protocol, FT.url.bridge, "indicator-list/view/", publicId
    ].join('');

    window.location.href = mailTo;
}

function deleteConfirm(e) {

    // URL of delete action
    var href = $(e).prop('href');

    // Display pop up asking to confirm deletion of the indicator list
    tooltipManager.init();
    var template = 'confirm';
    templates.add(template, '<div style="padding:15px;"><h3>Are you sure you want to delete this list?</h3><br>' +
        '<a style="width:70px;" class="btn btn-primary" href="{{href}}">OK</a><a class="btn" href="javascript:lightbox.hide();">Cancel</a></div>');
    var html = templates.render(template, { href: href });
    var popupWidth = 500;
    var left = lightbox.getLeftForCenteredPopup(popupWidth);
    var top = 200;
    lightbox.show(html, top, left, popupWidth);

    // Ignore initial link click
    return false;
}

