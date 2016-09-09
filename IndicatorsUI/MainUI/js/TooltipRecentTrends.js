/**
 * TooltipRecentTrends
 * @module TooltipRecentTrends
 */

'use strict';

/**
 * Creates tooltip for recent trends based 
 * upon trend data
 * @class RecentTrendsTooltip
 */
function RecentTrendsTooltip() {
    var keyValueStore = {};

    function buildTooltip(data) {
        var tooltipText;
        if (_.isUndefined(data)) {
            tooltipText = 'Not enough data points to calculate recent trend';
        } else if (!_.isUndefined(data.Message) && data.Message === "") {
            tooltipText = 'Trend based on most recent<br>' + data.PointsUsed + ' points';
        } else {
            tooltipText = data.Message;
        }

        return tooltipText;
    }

    return {
        /**
         * Adds trends to key/value map 
         * @method addTooltip              
         */
        addTooltip: function(key, value) {
            keyValueStore[key] = value;
        },

        /**
         * Returns tooltip text 
         * @method getTooltip              
         */
        getTooltip: function(key) {
                var data = keyValueStore[key];
            return buildTooltip(data);
        },
        /**
         * Returns tooltip text 
         * @method getTooltipByData            
         */
        getTooltipByData: function (data) {
            return buildTooltip(data);
        }
    };
}