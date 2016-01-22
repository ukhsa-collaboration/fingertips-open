
function setActiveOption(id, index) {

    var active = 'active',
    items = $('#' + id + ' li');
    items.removeClass(active);
    $(items[index]).addClass(active);
}
