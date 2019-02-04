'use strict';

/*
* File: common.js
* Purpose: provides functions used by every Fingertips profile
*/

function isDefined(obj) {
    return !_.isUndefined(obj) && obj !== null;
};

function showJQs(jqs) {
    for (var i in jqs) {
        jqs[i].show();
    }
};

function hideJQs(jqs) {
    for (var i in jqs) {
        jqs[i].hide();
    }
};

String.isNullOrEmpty = function (s) {
    return s === null || s === '';
};

var ajaxCache = {};

function ajaxGet(service, parameters, successFunction) {

    var key = service + parameters;
    if (ajaxCache.hasOwnProperty(key)) {
        successFunction(ajaxCache[key]);
    } else {

        var ajaxConfig = {
            type: 'GET',
            url: FT.url.bridge + service,
            /* version added so calls can be cached between updates but are
            refreshed after an update */
            data: parameters + '&v=' + FT.version,
            cache: true,
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                ajaxCache[key] = data;
                successFunction(data);
            },
            dataType: 'json',
            error: function(xhr) {
                unlock();
                ajaxError(xhr);
            }
        };

        $.ajax(ajaxConfig);
    }
};

function roundNumber(num, dec) {
    return Math.round(num * Math.pow(10, dec)) / Math.pow(10, dec);
};

// Wrapper for Hogan.js mustache template engine
var templates = (function () {

    var compiledTemplates = {};
    var rawTemplates = {};

    return {

        renderOnce : function(template, parameters) {
            return Hogan.compile(template).render(parameters);
        },

        render: function (name, parameters) {

            var compiled = compiledTemplates[name];

            if (!isDefined(compiled)) {
                compiled = Hogan.compile(rawTemplates[name]);
                compiledTemplates[name] = compiled;
            }

            return compiled.render(parameters);
        },

        add: function (name, mustache) {
            // Do not overwrite templates that are already added
            if (!isDefined(rawTemplates[name])) {
                rawTemplates[name] = mustache;
            }
        }
    };
})();

var ajaxMonitor = {

    count: 0,
    errors: 0,
    state: null,

    setCalls: function (i) {
        var _this = this;
        _this.count = i;
        _this.errors = 0;
        _this.state = {};
    },

    // Enable state information to be accessed in the callback code
    setState: function (obj) {
        this.state = obj;
    },

    callCompleted: function () {
        this.count--;
    },

    registerError: function (xhr) {
        logError(xhr);       
        this.errors++;
        this.callCompleted();
    },

    monitor: function (fn) {
        var _this = this;
        _this.errors = 0;
        if (_this.count === 0) {
            fn();
        } else {
            _this.fnFinished = fn;
            _this._pause();
        }
    },

    _pause: function () {
        setTimeout("ajaxMonitor.checkCompleted()", 10);
    },

    checkCompleted: function () {
        var _this = this,c;

        if (_this.count === 0) {
            if (_this.fnFinished !== null) {
                // Must set null before calling in case fnFinished uses this
                c = _this.fnFinished;
                _this.fnFinished = null;
                if (_this.errors > 0) {
                    handleAjaxFailure();
                } else {
                    c();
                }
            }
        } else {
            _this._pause();
        }
    },

    fnFinished: null
};

function ajaxError(xhr) {    
    ajaxMonitor.registerError(xhr);
}

function logError(xhr) {
    var parameters = 'serviceAction=ex&errorMessage=' + xhr.statusText + '&errorStatus=' + xhr.status;

    var ajaxConfig = {
        type: 'POST',
        url: FT.url.bridge + 'log/exception?' + parameters,
        cache: false,
        contentType: 'application/json; charset=utf-8'
    };

    $.ajax(ajaxConfig);
};

/**
* Returns whether the string ends with the suffix.
* @method String.prototype.endsWith
*/
String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

/**
* Returns a string with a separator dividing the input into lines
* no greater than the specified length.
* @method String.prototype.wordWrap
*/
String.prototype.wordWrap = function (charactersPerLine, htmlSeparator) {
    var c = 1;
    var i, j, l, s, r;
    if (charactersPerLine < 1)
        return this;
    for (i = -1, l = (r = this.split("\n")).length; ++i < l; r[i] += s)
        for (s = r[i], r[i] = ""; s.length > charactersPerLine; r[i] += s.slice(0, j) + ((s = s.slice(j)).length ? htmlSeparator : ""))
            j = c == 2 || (j = s.slice(0, charactersPerLine + 1).match(/\S*(\s)?$/))[1] ? charactersPerLine : j.input.length - j[0].length
            || c == 1 && charactersPerLine || j.input.length + (j = s.slice(charactersPerLine).match(/^\S*/)).input.length;
    return r.join("\n");
};

function setUrl(url) {
        window.location.href = url;
}

//
// Builds a URL parameter string
//
function ParameterBuilder() {

    var keys = [],
        values = [];

    //
    // Add a key value pair to the parameter list
    //
    this.add = function (key, value) {
        keys.push(key);

        if (_.isArray(value)) {
            value = value.join(',');
        }

        values.push(value);
        return this;
    };

    // 
    // Generate the parameter string
    //
    this.build = function () {

        var url = [];
        for (var index in keys) {
            if (index > 0) {
                url.push('&');
            }

            url.push(keys[index], '=', values[index]);
        }

        // Do not prefix with '?' as JQuery ajax will do this
        return url.join('');
    };

    this.setNotToCache = function()
    {
        // Time stamp to beat network caching + no cache flag to indicate no caching to ajax bridge
        this.add('no_cache', new Date().getTime());
        return this;
    }
}

var ftHistory = (function () {

    var _blockNext = 0,
    _model,
    _hashChange = 'hashchange',
    _location = window.location;

    return {

        //
        // Either return the URL hash or null if none defined
        // 
        getHash : function () {
            var bits = _location.href.split('#');
            return bits.length === 2 ?
                    bits[1] : null;
        },

        //
        // Initialises the history object.
        //
        init: function (model) {
            _model = model;

             $(window).bind(_hashChange, function () {
                ftHistory.hashChanged();
            });
        },

        //
        // Sets the URL hash according to the model
        //
        setHistory: function () {
            _blockNext = 1;
            _location.hash = _model.toString();
        },

        isParameterDefinedInHash: function (parameterName) {
            var hash = this.getKeyValuePairsFromHash();
            return !!hash[parameterName];
        },

        //
        // Parses the hash into key values pairs
        //
        getKeyValuePairsFromHash: function () {

            var hashString = this.getHash();
            return this.parseParameterString(hashString);
        },

        //
        // Parses the hash string into key values pairs
        //
        parseParameterString: function(hashString) {
            var obj = {};
            if (hashString) {
                var pairs = hashString.split(/[/,]/);
                for (var i = 0; i < pairs.length; i++) {
                    var key = pairs[i];
                    if (key && key !== '') {
                        var value = pairs[i + 1];
                        if (value && value !== '') {
                            obj[key] = value;
                        }
                    }
                    i++;
                }
            }
            return obj;
        },

        //
        // Fired when the URL hash changes
        //
        hashChanged: function () {
            if (_blockNext) {
                // Used to prevent model being restored when the hash is updated
                _blockNext = 0;
                return;
            }

            if (!FT.ajaxLock) {
                lock();
            }

            // Restore model from hash state
            _model.restore();
        }
    };
})();

function sortNumericAsc(a, b) {
    return a - b;
}

/*
* Formats a number to include commas, e.g. 100000 to 100,000
* @class CommaNumber
*/
function CommaNumber(n) {
    this.n = n;
}

CommaNumber.prototype = {

    _commarate: function (number) {
        return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    },

    /**
	* Rounds the number to 0 decimal places before formatting with commass
	* @method rounded
	*/
    rounded: function () {
        return this._commarate(Math.round(this.n, 0));
    },

    unrounded: function () {
        var commaNumber = this.n.toString();
        if (commaNumber.indexOf('.') > -1) {
            var bits = commaNumber.split('.');
            return this._commarate(bits[0]) + '.' + bits[1];
        }
        return this._commarate(this.n);
    }
};

// Ensure calls to console.log don't break IE7/8
if (typeof(console) === 'undefined') {
    console = {
        log: function () { }
    };
}

// Log events to Google Analytics (Universal Analytics)
function logEvent(category, action, label) {

    //ga only initialised on the live site (Google Analytics)
    if(typeof (ga) !== 'undefined')
    {
        var _send = 'send';
        var _event = 'event';
        if (label) {          
            ga(_send, _event, category, action, label);
        } else {
            ga(_send, _event, category, action);
        }
    }
}

function browserUpgradeMessage() {
    alert('Sorry this feature is not available for your browser. Please upgrade or use an alternative.');
}

function isFeatureEnabled(feature) {

    feature = feature.toLowerCase();

    // Is the feature flag contained within the list of active features
    return _.some(FT.features, function(f) {
        return f.toLowerCase() === feature;
    });
}

function MinMaxFinder(arrayOfNumbers) {
    this.setValues(arrayOfNumbers);
}

MinMaxFinder.prototype = {
    setValues: function (arrayOfNumbers) {

        var areValues = arrayOfNumbers.length > 0;

        if (areValues) {
            this.min = _.min(arrayOfNumbers);
            this.max = _.max(arrayOfNumbers);
        }
        this.isValid = areValues;
    }

};

function openFeedback() {
    $('#footer-feedback').hide();
    $('#footer-feedback-form').show();
    return false;
}

function closeFeedback() {
    $('#footer-feedback').show();
    $('#footer-feedback-form').hide();
    clearFeedbackForm();
    return false;
}

function sendFeedback() {

    var whatWereYouDoing = $("#what-were-you-doing").val(),
        whatWentWrong = $("#what-went-wrong").val(),
        email = $("#email-for-user-feedback").val();

    if (whatWereYouDoing === '' && whatWentWrong === '') {
        $('#footer-feedback-error-summary').html('<div class="alert alert-warning">Please tell us what went wrong?</div>');
        return false;
    }

    var data = {
        url: window.location.href,
        whatUserWasDoing: whatWereYouDoing,
        whatWentWrong: whatWentWrong,
        email: email
    };

    var apiUrl = '/user_feedback';

    $.post(apiUrl, data)
        .success(function(obj) {
            $('#footer-feedback').show();
            $('#footer-feedback-form').hide();
            clearFeedbackForm();
            $('#feedback-message').html('<div style="color:white;">Thank you for your feedback</div>');
        })
        .error(function (error) {
            $('#footer-feedback-error-summary').html('<div class="alert alert-danger">Sorry, we are unable to receive your message right now.</div>');
        });
    return false;
}

function clearFeedbackForm() {
    $('#user-feedback-form').each(function () {
        this.reset();
    });
}

function downloadLatestNoInequalitiesDataCsvFileByGroup(corews, parameters){
    var url = corews + 'api/latest/no_inequalities_data/csv/by_group_id?' + parameters.build();
    window.open(url.toLowerCase(), '_blank');
}

function downloadLatestNoInequalitiesDataCsvFileByIndicator(corews, parameters){
    var url = corews + 'api/latest/no_inequalities_data/csv/by_indicator_id?' + parameters.build();
    window.open(url.toLowerCase(), '_blank');
}

function downloadAllPeriodsNoInequalitiesDataCsvFileByIndicator(corews, parameters){
    var url = corews + 'api/allPeriods/no_inequalities_data/csv/by_indicator_id?' + parameters.build();
    window.open(url.toLowerCase(), '_blank');
}

function downloadCsvFileByGroup(corews, parameters){
    var url = corews + 'api/all_data/csv/by_group_id?' + parameters.build();
    window.open(url.toLowerCase(), '_blank');
}

function downloadCsvFileByIndicator(corews, parameters){
    var url = corews + 'api/all_data/csv/by_indicator_id?' + parameters.build();
    window.open(url.toLowerCase(), '_blank');
}

function downloadLatestWithInequalitiesDataCsvFileByIndicator(corews, parameters){
    var url = corews + 'api/latest/with_inequalities_data/csv/by_indicator_id?' + parameters.build();
    window.open(url.toLowerCase(), '_blank');
}

function downloadAllPeriodsWithInequalitiesDataCsvFileByIndicator(corews, parameters){
    var url = corews + 'api/allPeriods/with_inequalities_data/csv/by_indicator_id?' + parameters.build();
    window.open(url.toLowerCase(), '_blank');
}

function getIid(){
    var index = $('#indicatorMenu').prop('selectedIndex');
    return FT.model.groupRoots[index].IID;
}

function getAreasCodeDisplayed() {

    var allDisplayedAreas = getChildAreas().map(area => area["Code"]);

    if (FT.model.isNearestNeighbours()){
        allDisplayedAreas.push(FT.model.areaCode);
        return allDisplayedAreas;
    }
    return allDisplayedAreas;
}

function getParentAreaCode() {

    if (FT.model.isNearestNeighbours()){
        return NATIONAL_CODE;
    }
    return FT.model.parentCode;
}


function PostRequestDownloadCsvFile(url, data){
    $.ajax({
        type: "POST",
        url: url,
        data: JSON.stringify(data),
        dataType: 'text',
        success: function(result){
            var blob = new Blob([result]);
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download="myFileName.csv";
            link.click();
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) { 
            // alert("Status: " + textStatus); alert("Error: " + errorThrown); 
        } 
      });
}
