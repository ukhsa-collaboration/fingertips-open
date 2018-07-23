
$(document).ready(function () {

    showSpinner();

    initEventHandlers();

    // Load profiles
    displayProfiles();

    // Display selected indicators if in edit mode
    var listId = $('#public-id').val();
    if (listId !== '') {
        getIndicatorList(listId);
    } else {
        displayYourIndicatorListHeader([]);
        displayComponents();
    }
});

function searchForIndicators() {

    if (!FT.ajaxLock) {
        lock();

        // Define and clean search text
        var userText = $('#search-indicator').val();
        var searchText = userText.trim().toLowerCase();

        // Check the user has entered something
        if (searchText === '') {
            alertMessage('Enter a topic you would like to search for');
            unlock();
            return;
        }

        // Clear search results
        $('#indicator-list').html('');

        var parameters = new ParameterBuilder()
            .add('search_text', searchText);

        // Restrict results to specific profile
        var profileId = $('#profile-list').val();
        if (profileId !== '0') {
            parameters.add('restrict_to_profile_ids', profileId);
        }

        searchLoading();

        ajaxGet('api/indicator_search',
            parameters.build(),
            function (areaTypeIdToIndicatorIds) {

                var allIndicatorIds = [];

                _.each(areaTypeIdToIndicatorIds,
                    function (indicatorIds) {
                        if (indicatorIds.length) {
                            allIndicatorIds.push(indicatorIds);
                        }
                    });

                // Flatten and distinct
                allIndicatorIds = _.flatten(allIndicatorIds);
                allIndicatorIds = _.uniq(allIndicatorIds);

                if (allIndicatorIds.length) {
                    getIndicatorsById(allIndicatorIds);
                } else {
                    alertMessage("No indicators were found for '" + userText + "'");
                    searchLoaded();
                    unlock();
                    return;
                }
            },
            errorLogging);

        logEvent('Search', 'IndicatorSearchForIndicatorList', searchText);
    }
}

function searchLoading() {
    showSpinner();
    $('#alert').hide();
    $(".indicator-list-component").hide();
}

function searchLoaded() {
    hideSpinner();
    $(".indicator-list-component").show();
}

function clearList() {
    displayIndicatorListItems([], '#selected-indicator-list');
    displayYourIndicatorListHeader();
}

/*get all Indicator details by Ids*/
function getIndicatorsById(allIndicatorIds) {

    var parameters = new ParameterBuilder()
        .add('indicator_ids', allIndicatorIds.join(','));

    ajaxGet('api/grouproot_summaries/by_indicator_id',
        parameters.build(),
        function (groupRootSummaries) {

            // Map properties for spoofing of indicator list item
            for (var i in groupRootSummaries) {
                groupRootSummaries[i].SexId = groupRootSummaries[i].Sex.Id;
                groupRootSummaries[i].AgeId = groupRootSummaries[i].Age.Id;
                groupRootSummaries[i].IndicatorId = groupRootSummaries[i].IID;
            }

            // Remove summaries that are already selected
            var selectedListItems = getSelectedListItems();
            for (var i in selectedListItems) {
                var item = selectedListItems[i];
                groupRootSummaries = _.reject(groupRootSummaries, function (root) {
                    return root.IID === item.IID &&
                        root.SexId === item.SexId &&
                        root.AgeId === item.AgeId;
                });
            }

            $('#search-result-header').show();
            displayIndicatorListItems(groupRootSummaries, '#indicator-list', showSelectItemButton);
            unlock();
        });
}

function getSelectedListItems() {
    return getListItemsFromOptions($('#selected-indicator-list div'));
}

function displayIndicatorListItems(list, selector, clickHandler) {

    // Display indicator list
    var $box = $(selector);
    $box.html('');
    $(list)
        .map(function () {
            var $item = getIndicatorListItemElement(this, clickHandler);
            $box.append($item);
        });

    displayComponents();
}

function displayComponents() {
    $("#indicator-list-details").show();
    searchLoaded();
    $('#alertBox').hide();
}

function displayYourIndicatorListHeader() {
    // Only show header if any items
    var $header = $('.indicator-list-items-header');
    if ($('#selected-indicator-list div').length) {
        $header.show();
    } else {
        $header.hide();
    }
}

function getIndicatorListItemElement(listItem, eventHandler) {
    listItem.StateSex = true;
    listItem.StateAge = true;
    var indicatorText = listItem.IndicatorName + new SexAndAge().getLabel(listItem);

    var $item = $('<div>')
        .val(listItem.IID)
        .text(indicatorText)
        .attr({
            'iid': listItem.IID,
            'sex': listItem.Sex.Id,
            'age': listItem.Age.Id
        });

    $item.prop('item', listItem);

    $item.mouseover(function () {
        eventHandler($item);
    });
    return $item;
}

/*get all profiledetails by Id*/
function displayProfiles() {

    ajaxGet('api/profiles', '',
        function (profiles) {

            var $profileList = $('#profile-list');

            // Add all profiles option
            $profileList.append($('<option>').val('0').text('ALL PROFILES'));

            // Order profiles 
            profiles = _.sortBy(profiles, 'Name');

            // Add option for each profile
            $(profiles)
                .map(function () {
                    $profileList.append($('<option>', {
                        value: this.Id,
                        text: this.Name
                    }));
                });
        });
}

function clearFilter() {
    if (!FT.ajaxLock) {
        $('#search-indicator').val('').focus();
        $('#indicator-list div').remove();
        $('#search-result-header').hide();
    }
}

/*save the indicator list*/
function saveIndicatorList() {
    if (!FT.ajaxLock) {
        lock();
        var listId = $('#list-id').val();
        var publicId = $('#public-id').val();
        var listName = $('#list-name').val();
        var listItems = getSelectedListItems();

        var data = {
            Id: listId,
            ListName: listName,
            IndicatorListItems: listItems,
            PublicId: publicId
        };

        $.post('save', data)
            .success(function (result) {
                if (result.Success) {
                    window.location = '/user-account/indicator-list';
                } else {
                    // List was not saved
                    alertMessage(result.Message);
                    unlock();
                }
            })
            .error(errorLogging);
    }
}

/*load saved list details (edit mode)*/
function getIndicatorList(listId) {

    var parameters = new ParameterBuilder(
        ).add('id', listId
        ).setNotToCache();

    ajaxGet('api/indicator-list',
           parameters.build(),
           function (indicatorList) {

               // Display list items
               var listItems = indicatorList.IndicatorListItems;

               // Map property for spoofing of group root summary
               for (var i in listItems) {
                   listItems[i].IID = listItems[i].IndicatorId;
               }

               displayIndicatorListItems(listItems,
                   '#selected-indicator-list', showRemoveOrReorderButtons);

               displayYourIndicatorListHeader();
           });
}

function back() {
    window.location.href = '/user-account/indicator-list';
}

function initEventHandlers() {

    $('#btn-all-right').click(function () {
        var $selectedOptions = $('#indicator-list option');
        showSelectItemButton($selectedOptions);
    });

    $('#btn-all-left').click(function () {
        var $selectedOptions = $('#selected-indicator-list option');
        showRemoveOrReorderButtons($selectedOptions);
    });

    $('#search-indicator').keydown(function (event) {
        var code = (window.event) ? event.keyCode : event.which;
        if (code == 13) {
            searchForIndicators();
        }
    });
}

function showSelectItemButton($item) {

    // Ignore repeated mouseovers
    if ($lastItem === $item) return;
    $lastItem = $item;

    hideListItemMouseOverButtons();

    // Padding on parent div
    var padding = 6;

    var width = $item.width() + padding;
    var height = $item.height() + padding;

    // Button to remove the item from the selected items list
    var $add = $('<div>')
        .addClass('item-button item-button-add')
        .width(width)
        .height(height)
        .append('<i class="fa fa-caret-right fa-2x" aria-hidden="true"></i>')
        .click(function () {
            moveOptions($item, '#selected-indicator-list', showRemoveOrReorderButtons);
        });

    // Add the buttons to the parent div
    $item.append($add);
}

function hideListItemMouseOverButtons() {
    $('.item-button').remove();
}

function showRemoveOrReorderButtons($item) {

    // Ignore repeated mouseovers
    if ($lastSelectedItem === $item) return;
    $lastSelectedItem = $item;

    hideListItemMouseOverButtons();

    // Padding on parent div
    var padding = 6;

    var width = $item.width() + padding;
    var height = $item.height() + padding;

    // Button to remove the item from the selected items list
    var $remove = $('<div>')
        .addClass('item-button')
        .width(width * 0.5)
        .height(height)
        .append('<i class="fa fa-times fa-2x" aria-hidden="true"></i>')
        .click(function () {
            moveOptions($item, '#indicator-list', showSelectItemButton);
            displayYourIndicatorListHeader();
        });

    // Button to move the item up the order 
    var $up = $('<div>')
    .addClass('item-button item-button-up')
    .width(width * 0.25)
    .height(height)
    .append('<i class="fa fa-caret-up fa-2x" aria-hidden="true"></i>')
    .click(function () {
        moveItemUpList($item);
    });

    // Button to move the item down the order 
    var $down = $('<div>')
    .addClass('item-button item-button-down')
    .width(width * 0.25)
    .height(height)
    .append('<i class="fa fa-caret-down fa-2x" aria-hidden="true"></i>')
    .click(function () {
        moveItemDownList($item);
    });

    // Add the buttons to the parent div
    $item.append($remove, $up, $down);
}

function moveItemDownList($item) {
    var $container = $('#selected-indicator-list');

    // Determine indexes
    var index = getIndexOfChildItem($container, $item);
    var newIndex = index + 1;
    var lastIndex = $container.children().length - 1;

    if (newIndex === lastIndex) {
        // Will be last item
        $container.append($item);
        hideListItemMouseOverButtons();
    } else if (newIndex < lastIndex) {
        // Move item down
        var listItems = getSelectedListItems();
        var item = listItems[index];
        listItems.splice(index, 1);
        listItems.splice(newIndex, 0, item);
        displayIndicatorListItems(listItems, '#selected-indicator-list', showRemoveOrReorderButtons);
    }
}

function moveItemUpList($item) {
    var $container = $('#selected-indicator-list');
    var index = getIndexOfChildItem($container, $item);
    if (index > 0) {
        var newIndex = index - 1;
        $item.insertBefore($container.find('div:eq(' + newIndex + ')'));
        hideListItemMouseOverButtons();
    }
}

function getIndexOfChildItem($container, $item) {
    var options = $container.children();
    var item = $item.prop('item');
    for (var i = 0; i < options.length; i++) {
        var item2 = $(options[i]).prop('item');
        if (item === item2) {
            return i;
        }
    }
    return -1;
}

function moveOptions($options, menuSelector, clickHandler) {
    var listItems = getListItemsFromOptions($options);

    if (listItems.length) {
        // Add options to menu
        var $menu = $(menuSelector);
        for (var i in listItems) {
            var $option = getIndicatorListItemElement(listItems[i], clickHandler);
            $menu.append($option);
        };

        // Remove options from other menu
        $($options).remove();
    }

    displayYourIndicatorListHeader();
}

function getListItemsFromOptions($options) {
    var listItems = [];
    $.each($options,
    function (i, option) {
        var listItem = $(option).prop('item');
        if (listItem) {
            listItems.push(listItem);
        }
    });
    return listItems;
}

function alertMessage(message) {

    // Show message
    var $alert = $('#alertBox');
    if ($alert != null) {

        // Set message content
        $alert.find('p').html(message);

        // Show then fade message
        $alert.show();
    }
}

function errorLogging(e, xhr) {
    console.log(e);
    $(document).ajaxError(function (e, xhr) {
        if (xhr.status == 401)
            window.location = '/user-account/login';
    });
    unlock();
}

var $lastSelectedItem = null;
var $lastItem = null;
