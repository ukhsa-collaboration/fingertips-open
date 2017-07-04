function loading() {
    $('body').addClass('loading');
}

function loadingFinished() {
    $('body').removeClass('loading');
}

lightbox = (function () {

    // Private members
    fadeIn = false;
    var jqInfo = null;
    var jqLightbox = null;

    var init = function () {
        if (jqInfo === null) {
            jqLightbox = $('#lightBox');
            jqInfo = $('#infoBox');
        }
    };

    var CLICK_HIDE = ' onclick="lightbox.hide();"';

    return {
        HTML: '<div id="lightBox"' + CLICK_HIDE +
            '></div><div id="infoBox" style="display: none;"></div>',

        // Some older browsers do not fade in gracefully
        setFadeIn: function (b) {
            fadeIn = b;
        },

        // pre functions facilitate cross-browser support
        preHide: function () {
        },
        preShow: function () {
        },

        show: function (html, top, left/*always ignored so box as centered*/, width) {

            init();

            this.preShow();

            var height = $(document).height();
            if (height < 1000) {
                height = 1500;
            }
            jqLightbox.height(height);

            // Show transparent background
            if (fadeIn) {
                jqLightbox.fadeIn(400);
            } else {
                jqLightbox.show();
            }

            // Show content
            jqInfo.html(html + '<div class="close"' + CLICK_HIDE + '></div>');
            var leftPos = ($(window).width() / 2) - (width / 2);
            var topPos = document.documentElement.scrollTop || document.body.scrollTop;
            jqInfo.css({ 'top': topPos + top, 'left': leftPos, 'width': width });

            jqInfo.show();

            // Final readjustment
            var finalHeight = $(document).height();
            if (finalHeight > height) {
                jqLightbox.height(finalHeight + 20);
            }
        },

        hide: function () {
            this.preHide();
            jqInfo.hide();
            jqLightbox.hide();
        }
    };
})();

function showSimpleMessagePopUp(html) {

    lightbox.show('<div id="simple-message-popup"><h3>' + html + '</h3><br><input class="medium-button" type="button" onclick="lightbox.hide()"  value="OK" /></div>',
        250/*top*/, 0/*left*/, 500/*width*/);
}

function refresh() {
    window.location.href = window.location.href;
}

jQuery.cookie = function (name, value, options) {
    if (typeof value != 'undefined') { // name and value given, set cookie
        options = options || {};
        if (value === null) {
            value = '';
            options.expires = -1;
        }
        var expires = '';
        if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
            var date;
            if (typeof options.expires == 'number') {
                date = new Date();
                date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
            } else {
                date = options.expires;
            }
            expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE
        }
        // CAUTION: Needed to parenthesize options.path and options.domain
        // in the following expressions, otherwise they evaluate to undefined
        // in the packed version for some reason...
        var path = options.path ? '; path=' + (options.path) : '';
        var domain = options.domain ? '; domain=' + (options.domain) : '';
        var secure = options.secure ? '; secure' : '';
        document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
    } else { // only name given, get cookie
        var cookieValue = null;
        if (document.cookie && document.cookie != '') {
            var cookies = document.cookie.split(';');
            for (var i = 0; i < cookies.length; i++) {
                var cookie = $.trim(cookies[i]);
                // Does this cookie string begin with the name we want?
                if (cookie.substring(0, name.length + 1) == (name + '=')) {
                    cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                    break;
                }
            }
        }
        return cookieValue;
    }
};

function initTableSorter() {
    $(".sortable").tablesorter({
        theme: 'blue',
        widthFixed: true,
        widgets: ['zebra']
    });
}

function registerReloadPopUpDomains() {
    $(document).on('change', '#selectedProfileId', function () {

        setCopyMetadataOption();
        var selectedDomain = $('#selectedDomainId');

        $.ajax({
            type: "post",
            url: "/reloadDomains",
            data: { selectedProfile: $('#selectedProfileId').val() },
            success: function (data) {
                selectedDomain.empty();
                selectedDomain.addClass('dropdown-not-selected');
                $.each(data, function (key, value) {
                    selectedDomain.append($('<option value="' + value.Text + '"></option>').val(value.Value).html(value.Text));
                });
            },
            error: function (xhr, error) {
            }
        });
    });
}

function initSearchElements() {
    jQuery('#IndicatorId').keyup(function () {
        this.value = this.value.replace(/[^0-9\.]/g, '');
    });

    $('#search_text, #IndicatorId').keypress(function (event) {
        if (this.id == 'IndicatorId') {
            $('#search_text').val('');
        } else {
            $('#IndicatorId').val('');
        }
        if (event.keyCode == '13') {
            $('#searchAll').click();
            event.preventDefault();
        }
    });

    $('#searchAll').click(function () {
        var searchTerm = $('#search_text').val(),
            searchId = $('#IndicatorId').val(),
            body = $('body');
        if (searchTerm.length > 2 || searchId.length > 0) {
            body.addClass('loading');
            location.href = '/SearchAll?searchTerm=' + searchTerm + '&IndicatorId=' + searchId;
        } else {
            showSimpleMessagePopUp('Please enter an indicator ID or<br>at least 3 characters of text to search on');
            body.removeClass('loading');
        }
    });
}

function submitForm(name) {
    $('#' + name + 'Form').submit();
}

function setUrl(url) {
    location.href = url;
}

function setCopyMetadataOption() {
    var copyFromProfile = $('#selectedProfile').val();
    var copyToProfile = $('#selectedProfileId').val();    
    var copyToDomainId = $('#selectedDomainId').val();
    var copyToAreaTypeId = $('#selectedAreaTypeId').val();
    
    if (copyFromProfile !== copyToProfile) {

        $('#targetProfile').val(copyToProfile +  '~' + copyToDomainId + '~' + copyToAreaTypeId);

        $('#copyIndicatorMetadataOptions').show();
        $('#copyMetadataOption').prop('checked', true);
    } else {
        $('#copyIndicatorMetadataOptions').hide();
        $('#copyMetadataOption').prop('checked', false);        
    } 
}

$(document).ready(function () {

    // Init Bootstrap tooltips
    $('[data-toggle="tooltip"]').tooltip();

    $('.sortHeader').click(function () {
        loading();
    });

    $('.sortDesc').click(function () {
        loading();
    });

    $('.sortAsc').click(function () {
        loading();
    });

    $('.show-spinner').click(function () {
        loading();
    });

    $('.select-all-check-box').click(function () {
        $('.selectable-check-box').attr('checked', $(this).is(':checked'));
    });
});

function popupwindow(url, title, w, h) {
    return window.open(url, title, 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
}
