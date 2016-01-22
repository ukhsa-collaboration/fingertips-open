function goToPopulationPage() {
    lock();
    showSpinnerIfNoPopulation();
    
    var practiceCode = PP.model.practiceCode;
    if (isDefined(practiceCode)) {
        var pop = loaded.population[PP.model.year][practiceCode];
        if (!isDefined(pop)) {
            showSpinner();
        }
    }
    
    setPageMode(PAGE_MODES.POPULATION);
    
    var offset = getYearOffset();
    
    ajaxMonitor.setCalls(10);
    ajaxMonitor.setState({year:PP.model.year, offset:offset});
    
    getPracticeAndParentLists();
    
    getIndicatorMetadata(populationGroupId);
    getMetadataProperties();
    getPracticePopulation(PP.model.year, offset);
    getLabelSeries(LABELS_QUINARY);
    getLabelSeries(LABELS_DEPRIVATION);
    getPracticeParentPopulation();
    getNationalPopulation();
    
    ajaxMonitor.monitor(displayPopulation);
};

function getPracticeParentPopulation() {

    if (PP.model.parentCode != null && 
        getSelectedParentPopulation() == null) {
        getAveragedPopulation(PP.model.parentCode,
            getPracticeParentPopulationCallback);
    } else {
        
        ajaxMonitor.callCompleted();
    }
};

function displayPopulationInfo() {

    var state = populationState;
    
    if (state.lastDecile) {
        $('#d' + state.lastDecile).html('');
        state.lastDecile = null;
    }
    
    var noPopulationLabel = $('#noPractice');
    
    var p = currentPracticePopulation;
    if (state.isAnyPopulation()) {
        
        $('#practiceLabel').html(getPracticeLabel());
        
        updateEthnicity(p)
        updateDeprivationTable(p);
        updateFurtherInfo(p.AdHocValues);
        
        noPopulationLabel.hide(); 
    } else {
        
        var label = p === null ?
            'Select a practice<br>for further information' :
            'No population data<br>available for current practice';
        noPopulationLabel.html(label);
        noPopulationLabel.show();   
    }
    
    // Practice parent name
    var practiceParentName = getPracticeParentName().toUpperCase();
    
    updateRegisteredPersons(practiceParentName);
    
    // Update parent name in key
    $('#pctLabel').html(practiceParentName);
    
    state.displayPageElements();
};

function displayPopulation() {

    if (PP.model.year != latestYear) {
        unlock();
        return;
    }

    if (!populationState.redrawChart()) {
        populationState.displayPageElements();
        unlock();
        return;
    }
    
    populationState.setDisplayedKey();

    displayPopulationInfo();
    displayPopulationChart();
    
    unlock();
};

function getAveragedPopulation(areaCode, callBack) {
    
    ajaxGet('data/quinary_population_data',
        'area_code=' + areaCode +
        '&data_point_offset=' + getYearOffset() +
        '&group_id=' + populationGroupId,
        callBack);
};

function getNationalPopulation() {
    
    if (!isDefined(nationalPopulation[PP.model.year])) {
        getAveragedPopulation(NATIONAL_CODE,getNationalPopulationCallback);
    } else {
        ajaxMonitor.callCompleted();   
    }
};

function getNationalPopulationCallback(obj) {
    
    if(isDefined(obj) && isDefined(obj.Values)) {
        nationalPopulation[PP.model.year] = obj;
        makeValuesNegative(obj.Values[SEX_MALE]);
    }

    ajaxMonitor.callCompleted();
};

function getPracticeParentPopulationCallback(obj) {
    
    if (isDefined(obj) && isDefined(obj.Values)) {
        makeValuesNegative(obj.Values[SEX_MALE]);
        loaded.population[PP.model.year][obj.Code] = obj;
    }
    
    
    ajaxMonitor.callCompleted();
};

function getPracticePopulationCallback(obj) {
    
    if (isDefined(obj) && isDefined(obj.Values)) {

        currentPracticePopulation = obj;
        makeValuesNegative(currentPracticePopulation.Values[SEX_MALE]);

        var year = ajaxMonitor.state.year;
        loaded.population[year][obj.Code] = obj;
    } else {
        currentPracticePopulation = null;   
    }
    
    ajaxMonitor.callCompleted();
};

function getSelectedParentPopulation() {
    
    var model = PP.model,
    code = model.parentCode;
    
    if (code !== null) {
        var year = model.year;
        var population = loaded.population;
        if (population.hasOwnProperty(year) && 
                population[year].hasOwnProperty(code)) {
            return population[year][code];
        }  
    }
    
    return null;
};

function getPracticePopulation(year, offset) {
    
    var practiceCode = PP.model.practiceCode;
    
    if (isDefined(practiceCode)) {
        
        var pop = loaded.population[year][practiceCode];
        if (isDefined(pop)) {
            
            currentPracticePopulation = pop;
        } else {
            
            // Year not required as offset from Grouping DataPoint used
            ajaxGet('data/quinary_population_data',
                'area_code=' + practiceCode + 
                '&data_point_offset=' + offset + 
                '&group_id=' + populationGroupId,             
                getPracticePopulationCallback);
            return;
        }
    } else {
        currentPracticePopulation = null;   
    }
    
    ajaxMonitor.callCompleted();   
};

populationState = {
    
    _displayedKey : '',
    lastDecile : '',
    
    getKey : function() {
        var model = PP.model,
        parentCode = model.parentCode,
        practiceCode = model.practiceCode;
        
        return (parentCode ? parentCode : '-') + 
            (practiceCode ? practiceCode : '-') + 
            model.year;
    },
    
    redrawChart : function() {      
        return this._displayedKey !== this.getKey();
    },
    
    setDisplayedKey : function() {
        this._displayedKey = this.getKey();
    },
    
    displayPageElements : function() {
        showAndHidePageElements();
        
        var populationInfo = $('#populationInfo');
        if (populationState.isAnyPopulation()) {
            populationInfo.show();
        } else {
            populationInfo.hide();
        }   
    },
    
    isAnyPopulation : function() {
        var p = currentPracticePopulation;
        return p && p.hasOwnProperty('ListSize');   
    }
};

function updateDeprivationTable(p) {
    var decile = p.GpDeprivationDecile;
    if (isDefined(decile)) {
        var d = labels[LABELS_DEPRIVATION][decile];
        
        $('#d' + decile).html('<div class="decileSpacer"></div><div class="decile decile' + decile + '" >' +
                decile + '</div>');
        populationState.lastDecile = decile;
    } else {
        d = '<i>Data not available for current practice</i>';
    }
    $('#deprivation').html(d);
};

function updateEthnicity(data) {
    
    var ethnicity = data.Ethnicity;
    var html = isDefined(ethnicity) ?
        ethnicity :
        '<i>Insufficient data to provide accurate summary</i>';
    
    $('#ethnicity').html(html);
};

function updateFurtherInfo(adhocValues) {
    
    var rows = [];
    
    //TODO all these IIDs could come from corews
    
    // QOF achievement
    var qof = adhocValues.Qof;
    var value = isDefined(qof) ?
        (roundNumber(qof.Count,1) + ' (out of ' + qof.Denom  + ')') : null;
    rows.push({name:'QOF achievement',val:value,iid:295});
    
    // Life expectancy
    var text = 'ale life expectancy';
    
    var life = adhocValues.LifeExpectancyMale;
    var isLife = isDefined(life);  
    value = isLife ? life.ValF + ' years' : null;
    rows.push({name:'M' + text,val:value,iid:650});
    
    life = adhocValues.LifeExpectancyFemale;
    value = isLife ? life.ValF + ' years' : null;
    rows.push({name:'Fem' + text,val:value,iid:650});
    
    // Patients that recommend practice
    var recommend = adhocValues.Recommend;
    value = isDefined(recommend) ?  recommend.ValF + '%' : null
    rows.push({name:'% of patients that would recommend their practice',val:value,iid:347});
    
    $('#furtherInfoTable').html(templates.render('furtherInfo', {rows:rows}));
};

function updateRegisteredPersons(practiceParentName) {
    
    var rows = [],
    p,data,populationNumber;
    
    // Practice
    if (populationState.isAnyPopulation()) {
        rows.push({
                name: ui.getCurrentPractice().Name,
                val: new PopulationNumber(currentPracticePopulation.ListSize).val()
        });
    } 
    
    // Practice parent
    p = getSelectedParentPopulation();
    if (p) {
        populationNumber = new PopulationNumber(p.ListSize);
        rows.push({
                name: practiceParentName,
                val: populationNumber.val(),
                average: populationNumber.isData()
        });
    }
    
    // National
    rows.push({
            name:'ENGLAND',
            val:new PopulationNumber(nationalPopulation[PP.model.year].ListSize).val(), 
            average:true
    });
    
    $('#popTable').html(templates.render('furtherInfo', {rows:rows}));
};

function getSelectedPractice() {
    return $('#practiceMenu').find("option:selected").val() == "-" ? 'Practice Not Selected' : $.parseHTML($('#practiceMenu_chosen span')[0].innerHTML)[0].data;
}

function getExportPopulationHeading() {
    var mainHeading = '<b>Age Distribution ' + PP.model.year;
    if ($('#parentAreaMenu').find("option:selected").val() == "-") {
        return mainHeading + '</b>';
    } else {
            if ($('#practiceMenu').find("option:selected").val() == '-') {
                return mainHeading + ' for ' + $('#parentAreaMenu_chosen span')[0].innerHTML + '</b>';
            } else {
                return mainHeading + ' for ' + $('#parentAreaMenu_chosen span')[0].innerHTML + ' (' + getSelectedPractice() + ')</b>';
            }
    }    
}

function exportPopulationChart() {
    populationChart.exportChart({ type: 'image/png' }, {
        chart: {
            spacingTop: 70,
            events: {
                load: function () {
                    this.renderer.text(getExportPopulationHeading(), 250, 15)
                        .attr({
                            align: 'center'
                        })
                        .css({
                            fontSize: '10px',
                            width: '450px'
                        })
                        .add();
                }
            }
        },
        title: {
            text: '',
            style: CHART_TEXT_STYLE
        },

    });
}

function displayPopulationChart() {
    
    var maleString = ' (Male)',
    femaleString = ' (Female)',
    parentColour = chartColours.pink,
    _false = false,
    parentName = getPracticeParentLabel(),
    chartTitle = 'Age Distribution ' + PP.model.year;
    
    // Define populations
    var practicePopulation = new Population(currentPracticePopulation),
    parentPopulation = new Population(getSelectedParentPopulation()),
    nationalPop = new Population(nationalPopulation[PP.model.year]);
    
    try {
        if (populationChart && populationChart.series) {
            // Modify practice data
            var serieses = populationChart.series;
            serieses[0].setData(practicePopulation.male,_false);
            serieses[1].setData(practicePopulation.female,_false);
            
            serieses[2].setData(parentPopulation.male,_false);
            serieses[3].setData(parentPopulation.female,_false);
            
            serieses[4].setData(nationalPop.male,_false);
            serieses[5].setData(nationalPop.female,_false);
            
            populationChart.setTitle({text:chartTitle});
            
            populationChart.redraw();
        } else {
            populationChart = new Highcharts.Chart({
                    chart: {
                        renderTo: 'populationChart',
                        defaultSeriesType: 'line',      
                        margin:[40,55,50,55] /* margins must be set explicitly to avoid labels being positioned outside visible chart area */
                    },
                    title: {
                        text:chartTitle,
                        style:CHART_TEXT_STYLE
                    },
                    xAxis: [{
                            categories: labels[LABELS_QUINARY],
                            reversed: _false
                        }, { // mirror axis on right side
                            opposite: true,
                            reversed: _false,
                            categories: labels[LABELS_QUINARY],
                            linkedTo: 0
                        }
                    ],
                    yAxis: {
                        title: {
                            text: 'Population (%)',
                            style:CHART_TEXT_STYLE
                        },         
                        labels: {
                            formatter: function(){
                                return Math.abs(this.value);
                            }
                        }
                    },
                    plotOptions: {
                        series: {
                            events: {
                                legendItemClick: function() {
                                    return _false;
                                }
                            }
                        }, 
                        bar : {
                            borderWidth:0,
                            pointPadding:0,
                            stacking:'normal'
                        },
                        line:{ 
                            // The symbol is a non-valid option here to work round a bug
                            // in highcharts where only half of the markers appear on hover
                            marker: HC.noLineMarker,
                            states: {
                                hover: {
                                    marker: HC.noLineMarker
                                }
                            }
                        }            
                    },
                    legend: {
                        enabled:_false,
                        layout: 'vertical',
                        align: 'center',
                        verticalAlign: 'bottom', 
                        x: 0,
                        y: 0
                    },
                    tooltip: {
                        formatter: function(){
                            return '<b>'+ this.series.name.replace(/2$/,'') +'<br>Age: '+ 
                                this.point.category +'</b><br/>'+
                                'Population: '+ Math.abs(this.point.y) + '%';
                        }
                    },
                    credits: HC.credits,
                    exporting: {
                        enabled: false
                    },
                    series: [{
                            name: 'Practice' + maleString,
                            data: practicePopulation.male,
                            type: 'bar',
                            color: colours.bobLower
                        }, {
                            name: 'Practice' + femaleString,
                            data: practicePopulation.female, 
                            type: 'bar',
                            color: colours.bobHigher
                        }, {
                            name: parentName + maleString,
                            data: parentPopulation.male,
                            color: parentColour,
                            showInLegend:_false
                        }, {
                            name: parentName + femaleString,
                            data: parentPopulation.female,  
                            color: parentColour
                        }, {
                            name: 'England' + maleString,
                            data: nationalPop.male,
                            color: colours.comparator,
                            showInLegend:_false
                        }, {
                            name: 'England' + femaleString,
                            data: nationalPop.female,
                            color: colours.comparator
                        }
                    ]
            });
        }
    } catch (e) {
        // HighChart reports errors via console.log which is not available <=IE8
    }
}

function Population(population) {
    
    if (!population) {
        var male=[], female=[];
    } else {
        male = population.Values[SEX_MALE]; 
        female = population.Values[SEX_FEMALE]; 
    }
    
    this.male = male;
    this.female = female;
}

function PopulationNumber(coreDataSet) {
    this.data = coreDataSet;
}

PopulationNumber.prototype = {
    
    isData : function() {
        return this.data && this.data.Val > 0;
    },
    
    val : function() {
        return this.isData() ?
            new CommaNumber(this.data.Val).rounded() :
            null;
    }
}

NO_DATA = '<div class="noData">-</div>';

templates.add('furtherInfo', '{{#rows}}<tr><td class="header info">{{#iid}}<div class="fl infoTooltip" onclick="showMetadata(populationGroupId,{{iid}})"></div>{{/iid}}{{name}}</td><td>{{val}}{{^val}}' +
        NO_DATA + '{{/val}}{{#average}} <span class="averageLabel">(average)</span>{{/average}}</td></tr>{{/rows}}');

