var tooltipManager = new function() {
    
    var tooltipId = '#tooltip',
    tooltipElement = null,
    manager = this,
    isVisible = false,
    isInitialised = false,
    _tooltipProvider;
    
    this.init = function() {
        
        if (!isInitialised) {
            // Add necessary elements to page
            $('body').append(lightbox.HTML +
                /* Tooltip HTML */'<div id="tooltip" style="display: none;" onclick="tooltipManager.hide();"></div>'
            );   
            
            if (!tooltipElement) { 
                tooltipElement = $(tooltipId);
            }
            
            isInitialised = true;
        }
    };
    
    // Make an element trigger a tooltip
    this.initElement = function (id) {
        
        var jq = $('#' + id);

        jq.mouseenter(function (e) {
                manager.show(e);
        });
        jq.mouseleave(function (e) {
                manager.hide();
        });
        jq.mousemove(function (e) {
                manager.move(e);
        });
    };
    
    // Display the tooltip
    this.show = function (e, html) {
        
        isVisible = true;
        
        this.positionByEvent(e);
        
        var targetId = getElementIdFromJQueryEvent(e);
        
        var text = isDefined(html) ?
            html :
            this.getTooltipHtml(targetId);
        
        if (text !== '') {
            tooltipElement.html(text);
            tooltipElement.show();
        }
    };
    
    this.hide = function () {
        if (tooltipElement !== null) {
            tooltipElement.hide();
            isVisible=false;
        }
    };
    
    // Move the tooltip with the mouse pointer
    this.move = function (e) {
        this.positionByEvent(e);
    };
    
    this.positionByEvent = function (e) {
        this.positionXY(e.pageX + 10,e.pageY + 15);
    };
    
    this.positionXY = function (x,y) {
        tooltipElement.css('top', y);
        tooltipElement.css('left', x);
    };
    
    // Display a fixed position pop-up
    this.showText = function (jq, text, x, y) {
        
        isVisible = true;
        
        var pos = jq.position();
        this.positionXY(pos.left + x, pos.top + y);
        tooltipElement.html(text);
        tooltipElement.show();
    };
    
    this.setHtml = function(text) {
        tooltipElement.html(text);
    };
    
    this.showOnly = function() {
        tooltipElement.show();
    };
    
    this.showHtmlInPosition = function (text, x, y) {
        
        isVisible = true;
        
        this.positionXY(x, y);
        tooltipElement.html(text);
        tooltipElement.show();
    };
    
    this.getTooltipHtml = function (id) {
        return _tooltipProvider ? 
            _tooltipProvider.getHtml(id) :  
            '';
    };
    
    this.setTooltipProvider = function(tooltipProvider) {
        _tooltipProvider = tooltipProvider;
    };
};

lightbox = (function() {
        
        // Private members
        fadeIn = true;
        var jqInfo = null;
        var jqLightbox = null;
        
        var init = function() {
            if (jqInfo === null) {
                jqLightbox = $('#lightBox');
                jqInfo = $('#infoBox');
            }   
        };
        
        var CLICK_HIDE = ' onclick="lightbox.hide();"';
        var leafletClass = '.leaflet-control';
        
        return {
            
            HTML : '<div id="lightBox"' + CLICK_HIDE + 
                '></div><div id="infoBox" style="display: none;"></div>',
            
            // Some older browsers do not fade in gracefully
            setFadeIn : function(b) {
                fadeIn = b;
            },
            
            // pre functions facilitate cross-browser support
            preHide : function() {},
            preShow : function() {},
            
            setHtml : function(html) {
                jqInfo.html(html + '<div class="close"' + CLICK_HIDE + '></div>');
            },
            
            // The left position for a pop-up to appear in the middle of the window
            leftForMiddle : function(width) {
                return ($(window).width() - width) / 2;
            },
            
            show : function(html, top, left, width) {
                
                if (!FT.ajaxLock) {
                    
                    init();
                    
                    this.preShow();
                    
                    var height = $(document).height();
                    if (height < 1000) {
                        height = 1000;   
                    }
                    jqLightbox.height(height);
                    
                    // Show transparent background
                    if (fadeIn){
                        jqLightbox.fadeIn(400);
                    } else {
                        jqLightbox.show();
                    }
                    
                    // Show content
                    this.setHtml(html);
                    jqInfo.css({'top': top, 'left': left, 'width': width});
                    jqInfo.show();
                    
                    // Hide map controls
                    $(leafletClass).hide();
                    
                    // Final readjustment
                    var finalHeight = $(document).height();
                    if (finalHeight > height) {
                        jqLightbox.height(finalHeight + 20); 
                    }
                }
            },
            
            
            hide : function() {
                this.preHide();
                
                if (jqInfo !== null) {
                    jqInfo.hide();
                    jqLightbox.hide();
                }
                
                // Show map controls
                $(leafletClass).show();
            }
        }
})();

