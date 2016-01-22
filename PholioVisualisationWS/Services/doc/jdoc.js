function successFunction(obj) {
    
    h = ['<table><tr><td>' + getType(obj) + '</td></tr>'];
    
    writeObject(obj, 0);
    
    h.push('</table>');
    
    $('#output').html(h.join(''));
}

function writeObject(obj, depth) {
    
    var t = getType(obj);
    
    depth = depth + 1;
    
    if (t === '{}') {  
        
        writeHash(obj, depth);
    }
    else if (t === '[]') {
        
        for (var i in obj) {
            
            // What is child
            var pad = getPadding(depth);
            h.push('<tr>' + pad + '<td>' + getType(obj[i]) + '</td></tr>');
            
            // Assume every object in array is same type
            writeObject(obj[i], depth);
            break;
        }
        
    }
}

function div(i) {
return "<div style='float:left;'>" + i + "</div>";
}

function getType(obj) {
    
    if (_.isNull(obj)) {
        return 'null';   
    }
    
    if(_.isArray(obj)){
        return '[]';
    }
    
    var t = typeof(obj);
    
    if (t === 'object') {
        
        if (areAllKeysNumbers(obj)) {
            return '[]';
        }
        
        return '{}';
    }
    
    return t;
}

function getPadding(depth) {

  var pad = [];
  for (var j=0 ; j<depth ; j++) {
    pad.push('<td></td>');
  }
  return pad.join("");
};

function areAllKeysNumbers(obj) {
    
    var isEmpty = true;
    for (var i in obj) {
        if (i !== parseInt(i).toString()) {
            return false;
        }
        isEmpty = false;
    }
    
    if (isEmpty) {
        return false;   
    }
    return true;
};

function writeHash(obj, depth) {
    
    depth = depth + 1;
    var pad = getPadding(depth);
    
    for (var i in obj) {
        
        // Write key & type
        h.push('<tr>' + pad + '<td><b>' + i + '</b> : ' + getType(obj[i]) + '</td></tr>');
        
        writeObject(obj[i], depth);
    }
};

function call(service, data) {
    
    explainParameters(data);
    
    $('#url').html(service + '?' + data);
    
    var domain = window.location.href.split('/doc/')[0];

    $.ajax({
            type: 'GET',
            url: domain + '/' + service,
            data: data,
            dataType: 'json',
            cache: false,
            contentType: 'application/json',
            success: successFunction,
            error: function(obj) {
                alert("Error");
            }
    });
};

function explainParameters(parameters) {
    
    var pairs = parameters.split('&');
    var rows = _.map(pairs, function(pair) {
            var s = pair.split('=')
            var parameter = s[0];
            var key = parameterKeys[parameter];
            return _.isUndefined(key) ? 
                null :
                {parameter:parameter, name:key[0], description:key[1]};
    });
    
    rows = _.filter(rows,function(o){return !_.isNull(o)})
    rows = rows.sort(sortByName);
    
    if (rows.length) {
        var html = templates.render('parameters', {rows : rows});
    }else {
        html='';   
    }
    $('#parameter-explanation').html(html);
};

var parameterKeys = {
    s:['Service ID','Uniquely identifies the service'],
    pyr:['Practice year', 'Practices are assessed annually as to whether or not they are included'],
    gid:['Group ID', 'As defined in the PHOLIO grouping table'],
    com:['Comparators', 'Areas to compare the specified area against'],
    cim:['Confidence interval method ID', 'Confidence interval method ID as defined in PHOLIO'],
    par:['Parent area', 'An area that is composed of other areas'],
    pat:['Parent area type', 'The parent area type'],
    are:['Area code','The area code (ad hoc where no standard available, e.g. CCGs)'],
    cm:['Comparator method ID', 'Comparator method ID as defined in PHOLIO'],
    key:['Unique key','Allows the responses of multiple AJAX call to be distinguished from each other'],
    iid:['Indicator ID', 'ID that uniquely identifies an indicator'],
    avg:['Include averages?','Whether or not the averages should be included'],
    tim:['Include time periods?','Whether or not time periods should be included'],
    def:['Include definitions?', 'Include indicator definitions'],
    sex:['Sex ID','Sex ID as defined in PHOLIO'],
    age:['Age ID','Age ID as defined in PHOLIO'],
    ign:['Ignored area codes','Areas to ignore'],
    yr:['Year','The year'],
    pid:['Profile ID','Uniquely identifies a profile'],
    off:['Time offset', 'Number of time periods into the past to offset from the most recently available time period'],
    res:['Restrict to profile', 'Restricts the data returned to a specific profile'],
    ati:['Area type ID', 'Area Type ID as defined in PHOLIO'],
};

// Hogan.js wrapper
templates = (function(){
        
        var compiledTemplates = {};
        var rawTemplates = {};
        
        return {
            
            render : function(name, parameters) {
                
                var compiled = compiledTemplates[name];
                
                if (_.isUndefined(compiled)){
                    compiled = Hogan.compile(rawTemplates[name]);
                    compiledTemplates[name] = compiled;
                }
                
                return compiled.render(parameters);
            },
            
            add : function(name, mustache) {
                // Do not overwrite templates that are already added
                if (!_.has(rawTemplates,name)) {
                    rawTemplates[name] = mustache;
                }
            }
        };
})();

function sortByName(a,b) {
    
    if (a.parameter === 's') return -1;
    if (b.parameter === 's') return 1;
    
    var nA = a.name.toLowerCase();
    var nB = b.name.toLowerCase();
    if (nA < nB) return -1;
    if (nA > nB) return 1;
    return 0;
};

templates.add('parameters', '<table cellspacing="0" class="bordered-table">' + 
    '<tr><th>Parameter</th><th>Name</th><th>Description</th></tr>' +
        '{{#rows}}<tr>' + 
        '<td>{{parameter}}</td><td>{{name}}</td><td>{{description}}</td></tr>{{/rows}}</table>');

