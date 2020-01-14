/**
 * Open dialog to enable users to share an indicator list
 */
function openShareIndicatorListDialog(e, publicId, listName) {
    var href = $(e).prop('href');

    // Determine HTTP protocol
    var parser = document.createElement('a');
    parser.href = window.location.href;
    var protocol = parser.protocol;
    var url = protocol + FT.url.bridge + 'indicator-list/view/' + publicId;

    // Display pop up asking to copy link to clipboard
    tooltipManager.init();
    var template = 'ShareIndicatorList';
    templates.add(template,
        '<div style="padding:15px;">' +
            '<h2>Share indicator list</h2>' +
            '<p style="margin-bottom: 10px;">' +
                'Click on the button below to copy the URL to clipboard.' +
                '<br>' +
                'You can then share the copied URL by email.' +
                '<br><br>' +
                '<input type="text" id="indicator-list-url" class="indicator-list-url" value="' + url + '">' +
            '</p>' +
            '<div>' +
                '<button class="btn btn-primary" onclick="copyToClipboard()">Copy to clipboard</button>' +
                '<a class="btn" href="javascript:lightbox.hide();">Cancel</a>' +
            '</div>' +
        '</div>');

    var html = templates.render(template, { href: href });
    var popupWidth = 600;
    var left = lightbox.getLeftForCenteredPopup(popupWidth);
    var top = 300;
    lightbox.show(html, top, left, popupWidth);
}

function copyToClipboard() {
    var copyUrl = document.getElementById("indicator-list-url");
    copyUrl.select();
    document.execCommand("copy");
}

function deleteConfirm(e) {

    // URL of delete action
    var href = $(e).prop('href');

    // Display pop up asking to confirm deletion of the indicator list
    tooltipManager.init();
    var template = 'confirm';
    templates.add(template, '<div style="padding:15px;"><h3>Are you sure you want to delete this list?</h3><br>' +
        '<a id="btn-delete-confirm-indicator-list" style="width:70px;" class="btn btn-primary" href="{{href}}">OK</a><a class="btn" href="javascript:lightbox.hide();">Cancel</a></div>');
    var html = templates.render(template, { href: href });
    var popupWidth = 500;
    var left = lightbox.getLeftForCenteredPopup(popupWidth);
    var top = 400;
    lightbox.show(html, top, left, popupWidth);

    // Ignore initial link click
    return false;
}

function copyConfirm(e, publicId, listName) {
    // URL of copy action
    var href = $(e).prop('href');

    // Display pop up asking to confirm copying of the indicator list
    tooltipManager.init();
    var template = 'copyconfirm';
    templates.add(template,
        '<div style="padding:15px;">' +
        '<h3>Copy indicator list</h3>' +
        '<div style="margin-bottom: 10px;">' +
            'Name of new list' +
            '<br><br>' +
            '<input type="hidden" id="hdn-public-id" value="' + publicId + '">' +
            '<input type="text" id="indicator-list-name" class="indicator-list-name" value="' + listName + ' copy">' +
        '</div>' +
        '<div>' +
            '<button id="btn-copy-indicator-list" style="width:70px;" class="btn btn-primary" onclick="copyIndicatorList()";>OK</button>' +
            '<a class="btn" href="javascript:lightbox.hide();">Cancel</a>' +
        '</div>' +
        '</div>');

    var html = templates.render(template, { href: href });
    var popupWidth = 500;
    var left = lightbox.getLeftForCenteredPopup(popupWidth);
    var top = 400;
    lightbox.show(html, top, left, popupWidth);

    // Ignore initial link click
    return false;
}

function copyIndicatorList() {
    var publicId = $("#hdn-public-id").val(),
        listName = $("#indicator-list-name").val();

    console.log(publicId);
    console.log(listName);

    window.location.href = "/user-account/indicator-list/copy?listId=" +
        publicId +
        '&listName=' +
        listName;
}
