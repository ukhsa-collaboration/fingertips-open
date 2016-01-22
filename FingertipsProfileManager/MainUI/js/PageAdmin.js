
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

    var deleteIndicatorButton = new FpmButton('#deleteIndicators');
    var changeIndicatorOwnershipButton = new FpmButton('#changeIndicatorOwnership');
    var textBox = new FpmTextBox('#indicatorIds');

    deleteIndicatorButton.$button.click(function () {

        var indicatorIds = textBox.getString();
        //Remove any spaces and any last commas
        indicatorIds = indicatorIds.replace(/,\s*$/, '');
        indicatorIds = indicatorIds.replace(/\s/g, '');

        if (!/^[\d,\s]+$/.test(indicatorIds)) {
            showSimpleMessagePopUp('Please enter a valid comma-delimited indicator Id list to delete');
            return;
        }

        deleteIndicatorButton.disable();

        $.ajax({
            type: 'post',
            url: '/admin/deleteindicator',
            data: {
                indicatorIds:indicatorIds
            },
            success: function (data) {
                textBox.clear();
                showSimpleMessagePopUp(
                    'All chosen indicators have been deleted'
                    );
                deleteIndicatorButton.enable();
            },
            error: function (xhr, error) {
                showSimpleMessagePopUp('Sorry, that did not work');
                deleteIndicatorButton.enable();
            }
        });

    });

    changeIndicatorOwnershipButton.$button.click(function () {

        var indicatorId = textBox.getString();
        var newOwnerProfileId = $('#profileList option:selected')[0].value;
        changeIndicatorOwnershipButton.disable();

        $.ajax({
            type: 'post',
            url: '/admin/ChangeIndicatorOwner',
            data: {
                indicatorId: indicatorId,
                newOwnerProfileId: newOwnerProfileId
            },
            success: function (data) {
                textBox.clear();
                showSimpleMessagePopUp(
                    'Indicator ownership has been changed'
                    );
                changeIndicatorOwnershipButton.enable();
            },
            error: function (xhr, error) {
                showSimpleMessagePopUp('Sorry, that did not work');
                changeIndicatorOwnershipButton.enable();
            }
        });

    });
});
