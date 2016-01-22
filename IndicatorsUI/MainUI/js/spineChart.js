function getSpineProportions(average, stats, polarity) {
    
    /* Properties of returned proportions object:
    q1Offset, q1, q2, q4, marker, unitsOfLargestSide 
    
    Proportion values are expressed in the unit of the indicator */
    var prop = {};
    
    if (spineChart.isHighestLeft(polarity)) {
        var min = stats.Max;
        var max = stats.Min;   
        var p25 = stats.P75;
        var p75 = stats.P25;
    } else {  
        min = stats.Min;
        max = stats.Max;  
        p25 = stats.P25;
        p75 = stats.P75;
    }
    
    // Q1 Offset
    var minAverageDifference = Math.abs(average - min);
    var maxAverageDifference = Math.abs(average - max);
    if (minAverageDifference > maxAverageDifference) {
        prop.unitsOfLargestSide = minAverageDifference;
        prop.q1Offset = 0;
    } else {
        prop.unitsOfLargestSide = maxAverageDifference;
        prop.q1Offset = (maxAverageDifference - minAverageDifference);
    }
    
    prop.q1 = Math.abs(p25 - min);
    prop.q2 = Math.abs(p75 - p25);
    prop.q4 = Math.abs(max - p75);
    prop.min = min;
    
    return prop;
};

spineChart = (function(){
        
        // Not 1 for float precision reasons
        var one = 0.9999;
        
        // Regions that are left floated and rounded
        var a = ['q1Offset', 'q1', 'q2', 'q4'];
        
        var halfMarkerWidth, spineWidth, halfWidth;
        
        var setWidths = function(sWidth, mWidth) {
            spineWidth = sWidth;
            halfWidth = sWidth / 2;
            halfMarkerWidth = mWidth / 2;
        };
        
        var adjustDimensionsForRounding = function(sd,roundDiff) {
            //Note: q4 may not be most appropriate section to adjust
            if (roundDiff <= -one) {
                sd.q4 -= 1;
            } else if (roundDiff >= one) {
                sd.q4 += 1;
            }
            return sd;
        };
        
        var key = 'spineChart';     
        templates.add(key,
            '<div class="spine spine{{width}}">' +
                '<img id="q1_{{row}}" src="{{imgDir}}lightgrey.png" class="q1" style="width:{{q1}}px;margin-left:{{q1Offset}}px;" alt=""/>' + 
                '<img id="q2_{{row}}" src="{{imgDir}}darkgrey.png" class="q2" style="width:{{q2}}px;" alt=""/>' +   
                '<img id="q4_{{row}}" src="{{imgDir}}lightgrey.png" class="q4" style="width:{{q4}}px;" alt=""/>' +
                '<img id="average_{{row}}" src="{{imgDir}}red.png" class="average{{width}}" alt=""/>{{{markerHtml}}}</div>'
        );
        
        return {
            
            isHighestLeft : function(polarity) {
                return correctForPolarity && polarity == 0;
            },
            
            getWidth : function(){
                return spineWidth;
            },
            
            init : function(spineWidth, markerWidth) {
                setWidths(spineWidth, markerWidth);
            },
            
            getDimensions : function(proportions) {
                
                var pixelPerUnit = halfWidth / proportions.unitsOfLargestSide;
                
                var sd = {}
                
                var roundDiff = 0;
                for (var i in a) {
                    var key = a[i];
                    val = proportions[key] * pixelPerUnit;
                    rounded = Math.round(val);
                    roundDiff += val - rounded;
                    sd[key] = rounded;
                }
                
                sd.q1Offset += 12/*margin left*/;
                sd.pixelPerUnit = pixelPerUnit;
                
                return adjustDimensionsForRounding(sd, roundDiff);
            },
            
            getHtml : function(dimensions, rowNumber, markerHtml) {
                dimensions.width = spineWidth;
                dimensions.row = rowNumber;
                dimensions.imgDir = FT.url.img;
                dimensions.markerHtml = markerHtml;
                return templates.render(key, dimensions);
            },
            
            // Left offset to position the centre of marker image appropriate for the value
            getMarkerOffset : function(value, dimensions, proportions, polarity) {
                
                if (isDefined(value)) {
                    var min = proportions.min;
                    var marker = this.isHighestLeft(polarity) ? 
                        min - value : 
                        value - min;
                    
                    return Math.round((marker * dimensions.pixelPerUnit) - halfMarkerWidth);
                }
            }
        };   
})();

function SpineChartStems(spineLimitLabels) {
    
    var p25 = '25th Percentile',
    p75 = '75th Percentile',
    to = ' - ',
    labels = {
        q1 : spineLimitLabels.min + to + p25,
        q2 : p25 + to + p75,
        q4 : p75 + to + spineLimitLabels.max,
        average: ' Average',
        m: '' // Marker
    },
    keys = _.keys(labels);
    
    this.getStemQualifier = function (stem) {
        return stem != 'marker' && stem != 'average' ?
            '<span id="tooltipQualifier">' + labels[stem] + '</span>' :
            '';
    }
    
    this.getStemText = function (stem, formatter) {
        switch (stem) {
            case 'q1':
                return formatter.getMin() + to + formatter.get25();
            case 'q2':
                return formatter.get25() + to + formatter.get75();
            case 'q4':
                return formatter.get75() + to + formatter.getMax();
            case 'average':
                return formatter.getAverage();
            case 'm':
                return formatter.getAreaValue();
        }
    }
    
    this.getKeys = function() {
        return keys;
    }
}

