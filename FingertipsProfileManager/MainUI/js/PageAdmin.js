
function FpmButton(jquerySelector) {
    
    var $button = $(jquerySelector);
    this.$button = $button;

    this.disable = function() {
        $button.attr('disabled', 'disabled');
    }

    this.enable = function () {
        $button.removeAttr('disabled');
    }
}

function FpmTextBox(jquerySelector) {
    
    var $textBox = $(jquerySelector);

    this.getString = function() {
        return $textBox.val();
    }

    this.clear = function() {
        $textBox.val('');
    }
}

$(document).ready(function() {
    var $indicatorId = new FpmTextBox('#indicatorIds');
    initDeleteIndicatorButton($indicatorId);
    initChangeIndicatorOwnershipButton($indicatorId);
    initShowIndicatorOwner($indicatorId);
});

function initChangeIndicatorOwnershipButton($indicatorId) {
    var $changeIndicatorOwnershipButton = new FpmButton('#changeIndicatorOwnership');

    $changeIndicatorOwnershipButton.$button.click(function () {

        var indicatorId = $indicatorId.getString().trim();

        if (indicatorId === '') {
            showSimpleMessagePopUp('Please enter an indicator Id');
            return;
        }

        var newOwnerProfileId = $('#profileList option:selected')[0].value;
        $changeIndicatorOwnershipButton.disable();

        $.ajax({
            type: 'post',
            url: '/admin/change-indicator-owner',
            data: {
                indicatorId: indicatorId,
                newOwnerProfileId: newOwnerProfileId
            },
            success: function (data) {
                $indicatorId.clear();
                showSimpleMessagePopUp(
                    'Indicator ownership has been changed'
                    );
                $changeIndicatorOwnershipButton.enable();
            },
            error: function (xhr, error) {
                $changeIndicatorOwnershipButton.enable();
                showSimpleMessagePopUp('Sorry, that did not work');
            }
        });
    });
}

function initShowIndicatorOwner($indicatorId) {
    var $showIndicatorOwner = $('#showIndicatorOwner');

    $showIndicatorOwner.click(function () {

        var indicatorId = $indicatorId.getString().trim();

        if (indicatorId === '') {
            showSimpleMessagePopUp('Please enter an indicator Id');
            return;
        }

        $.ajax({
            type: 'get',
            url: '/admin/get-indicator-owner',
            data: {
                indicatorId: indicatorId
            },
            success: function (profile) {
                var message = profile == null
                    ? 'Indicator ID does not exist'
                    : profile.Name;

                showSimpleMessagePopUp(message);
            },
            error: function (xhr, error) {
                showSimpleMessagePopUp('Sorry, that did not work');
            }
        });
    });
} 

function initDeleteIndicatorButton($indicatorId) {
    var $deleteIndicatorButton = new FpmButton('#deleteIndicators');
    $deleteIndicatorButton.$button.click(function () {

        var indicatorIds = $indicatorId.getString();
        //Remove any spaces and any last commas
        indicatorIds = indicatorIds.replace(/,\s*$/, '');
        indicatorIds = indicatorIds.replace(/\s/g, '');

        if (!/^[\d,\s]+$/.test(indicatorIds)) {
            showSimpleMessagePopUp('Please enter a valid comma-delimited indicator Id list to delete');
            return;
        }

        $deleteIndicatorButton.disable();

        $.ajax({
            type: 'post',
            url: '/admin/delete-indicator',
            data: {
                indicatorIds: indicatorIds
            },
            success: function (data) {
                $indicatorId.clear();
                showSimpleMessagePopUp(
                    'All chosen indicators have been deleted'
                    );
                $deleteIndicatorButton.enable();
            },
            error: function (xhr, error) {
                showSimpleMessagePopUp('Sorry, that did not work');
                $deleteIndicatorButton.enable();
            }
        });

    });
}