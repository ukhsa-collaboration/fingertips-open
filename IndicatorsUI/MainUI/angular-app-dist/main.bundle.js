webpackJsonp(["main"],{

/***/ "./src/$$_gendir lazy recursive":
/***/ (function(module, exports) {

function webpackEmptyAsyncContext(req) {
	// Here Promise.resolve().then() is used instead of new Promise() to prevent
	// uncatched exception popping up in devtools
	return Promise.resolve().then(function() {
		throw new Error("Cannot find module '" + req + "'.");
	});
}
webpackEmptyAsyncContext.keys = function() { return []; };
webpackEmptyAsyncContext.resolve = webpackEmptyAsyncContext;
module.exports = webpackEmptyAsyncContext;
webpackEmptyAsyncContext.id = "./src/$$_gendir lazy recursive";

/***/ }),

/***/ "./src/app/app.component.css":
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/app.component.html":
/***/ (function(module, exports) {

module.exports = "<ft-map></ft-map>\r\n<ft-england></ft-england>\r\n<ft-metadata></ft-metadata>\r\n<ft-population></ft-population>\r\n<ft-boxplot></ft-boxplot>\r\n"

/***/ }),

/***/ "./src/app/app.component.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var AppComponent = (function () {
    function AppComponent() {
    }
    return AppComponent;
}());
AppComponent = __decorate([
    core_1.Component({
        selector: '[app-root]',
        template: __webpack_require__("./src/app/app.component.html"),
        styles: [__webpack_require__("./src/app/app.component.css")]
    })
], AppComponent);
exports.AppComponent = AppComponent;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/app.component.js.map

/***/ }),

/***/ "./src/app/app.module.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var platform_browser_1 = __webpack_require__("./node_modules/@angular/platform-browser/@angular/platform-browser.es5.js");
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var forms_1 = __webpack_require__("./node_modules/@angular/forms/@angular/forms.es5.js");
var http_1 = __webpack_require__("./node_modules/@angular/http/@angular/http.es5.js");
var app_component_1 = __webpack_require__("./src/app/app.component.ts");
var map_component_1 = __webpack_require__("./src/app/map/map.component.ts");
var map_module_1 = __webpack_require__("./src/app/map/map.module.ts");
var boxplot_module_1 = __webpack_require__("./src/app/boxplot/boxplot.module.ts");
var shared_module_1 = __webpack_require__("./src/app/shared/shared.module.ts");
var coreDataHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/coreDataHelper.service.ts");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var england_component_1 = __webpack_require__("./src/app/england/england.component.ts");
var metadata_module_1 = __webpack_require__("./src/app/metadata/metadata.module.ts");
var population_module_1 = __webpack_require__("./src/app/population/population.module.ts");
var ngx_bootstrap_1 = __webpack_require__("./node_modules/ngx-bootstrap/index.js");
var components = [app_component_1.AppComponent, map_component_1.MapComponent];
var AppModule = (function () {
    function AppModule(resolver) {
        this.resolver = resolver;
    }
    // Overriding angular original behaviour, This will bootsttrap all the component defined
    // in the array if the component tag is found on the document
    AppModule.prototype.ngDoBootstrap = function (appRef) {
        var _this = this;
        components.forEach(function (componentDef) {
            var factory = _this.resolver.resolveComponentFactory(componentDef);
            if (document.querySelector(factory.selector)) {
                appRef.bootstrap(factory);
            }
        });
    };
    return AppModule;
}());
AppModule = __decorate([
    core_1.NgModule({
        declarations: [
            app_component_1.AppComponent,
            england_component_1.EnglandComponent,
        ],
        imports: [
            platform_browser_1.BrowserModule,
            forms_1.FormsModule,
            http_1.HttpModule,
            map_module_1.MapModule,
            boxplot_module_1.BoxplotModule,
            population_module_1.PopulationModule,
            shared_module_1.SharedModule,
            metadata_module_1.MetadataModule,
            ngx_bootstrap_1.TypeaheadModule.forRoot()
        ],
        providers: [coreDataHelper_service_1.CoreDataHelperService, ftHelper_service_1.FTHelperService],
        entryComponents: components
        // Instead of bootstraping a single component, with overriding mechnisam, multiple componenta are bootstrapped
        // bootstrap: [AppComponent]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof core_1.ComponentFactoryResolver !== "undefined" && core_1.ComponentFactoryResolver) === "function" && _a || Object])
], AppModule);
exports.AppModule = AppModule;
var _a;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/app.module.js.map

/***/ }),

/***/ "./src/app/boxplot/boxplot-chart/boxplot-chart.component.css":
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/boxplot/boxplot-chart/boxplot-chart.component.html":
/***/ (function(module, exports) {

module.exports = "<div #chart id=\"boxplot-chart\"></div>"

/***/ }),

/***/ "./src/app/boxplot/boxplot-chart/boxplot-chart.component.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var boxplot_1 = __webpack_require__("./src/app/boxplot/boxplot.ts");
var Highcharts = __webpack_require__("./node_modules/highcharts/highcharts.js");
__webpack_require__("./node_modules/highcharts/modules/exporting.js")(Highcharts);
__webpack_require__("./node_modules/highcharts/highcharts-more.js")(Highcharts);
var BoxplotChartComponent = (function () {
    function BoxplotChartComponent() {
    }
    BoxplotChartComponent.prototype.ngAfterViewInit = function () {
        this.displayChart();
    };
    BoxplotChartComponent.prototype.ngOnChanges = function (changes) {
        this.displayChart();
    };
    BoxplotChartComponent.prototype.displayChart = function () {
        if (this.isAnyData()) {
            var chartContainer = null;
            if (this.chartEl && this.chartEl.nativeElement) {
                chartContainer = this.chartEl.nativeElement;
                this.chart = new Highcharts.Chart(chartContainer, this.getChartOptions());
            }
        }
    };
    BoxplotChartComponent.prototype.getChartOptions = function () {
        var metadata = this.boxplotData.metadata;
        var unitLabel = metadata.Unit.Label;
        // Series
        var seriesName = this.boxplotData.areaTypeName + ' in ' + this.boxplotData.comparatorName;
        var series = [
            {
                name: seriesName,
                data: this.getChartData()
            }
        ];
        // So accessible from tooltip
        var boxplotData = this.boxplotData;
        return ({
            chart: {
                type: 'boxplot',
                width: 820,
                animation: false
            },
            title: {
                text: ''
            },
            legend: {
                enabled: false
            },
            xAxis: {
                categories: this.boxplotData.periods,
                title: {
                    text: ''
                }
            },
            yAxis: {
                min: this.boxplotData.min,
                title: {
                    text: unitLabel
                }
            },
            plotOptions: {
                boxplot: {
                    animation: false,
                    color: '#1e1e1e',
                    fillColor: '#cccccc',
                    medianColor: '#ff0000'
                }
            },
            tooltip: {
                followPointer: false,
                formatter: function () {
                    var period = this.x;
                    var index = boxplotData.periods.indexOf(period);
                    var stats = boxplotData.statsFormatted[index];
                    var tooltipContent = [
                        '<b>', period, '</b><br/>',
                        '<b>', seriesName, '</b><br/>',
                        '95th Percentile: ', stats.P95, '<br/>',
                        '75th Percentile: ', stats.P75, '<br/>',
                        'Median: ', stats.Median, '<br/>',
                        '25th Percentile: ', stats.P25, '<br/>',
                        '5th Percentile: ', stats.P5, '<br/>'
                    ];
                    return tooltipContent.join('');
                }
            },
            series: series
        });
    };
    BoxplotChartComponent.prototype.getChartData = function () {
        var chartDataGrid = [];
        for (var _i = 0, _a = this.boxplotData.stats; _i < _a.length; _i++) {
            var stats = _a[_i];
            var pointData = [];
            pointData[0] = this.getValue(stats.P5);
            pointData[1] = this.getValue(stats.P25);
            pointData[2] = this.getValue(stats.Median);
            pointData[3] = this.getValue(stats.P75);
            pointData[4] = this.getValue(stats.P95);
            chartDataGrid.push(pointData);
        }
        return chartDataGrid;
    };
    BoxplotChartComponent.prototype.getValue = function (i) {
        return i == null ? 0 : i;
    };
    BoxplotChartComponent.prototype.isAnyData = function () {
        return this.boxplotData && this.boxplotData.isAnyData();
    };
    return BoxplotChartComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_a = typeof boxplot_1.BoxplotData !== "undefined" && boxplot_1.BoxplotData) === "function" && _a || Object)
], BoxplotChartComponent.prototype, "boxplotData", void 0);
__decorate([
    core_1.ViewChild('chart'),
    __metadata("design:type", typeof (_b = typeof core_1.ElementRef !== "undefined" && core_1.ElementRef) === "function" && _b || Object)
], BoxplotChartComponent.prototype, "chartEl", void 0);
BoxplotChartComponent = __decorate([
    core_1.Component({
        selector: 'ft-boxplot-chart',
        template: __webpack_require__("./src/app/boxplot/boxplot-chart/boxplot-chart.component.html"),
        styles: [__webpack_require__("./src/app/boxplot/boxplot-chart/boxplot-chart.component.css")]
    })
], BoxplotChartComponent);
exports.BoxplotChartComponent = BoxplotChartComponent;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/boxplot-chart.component.js.map

/***/ }),

/***/ "./src/app/boxplot/boxplot-table/boxplot-table.component.css":
/***/ (function(module, exports) {

module.exports = ".boxplot-table {\r\n    margin: 30px auto;\r\n}\r\n\r\n.boxplot-table table {\r\n    font-size: 13px;\r\n    min-width: 35% !important;\r\n    width: auto;\r\n    table-layout: auto !important;\r\n    border-width: 0 !important;\r\n    margin: 0 auto;\r\n}\r\n\r\n.boxplot-table th {\r\n    font-size: 11px;\r\n    text-align: right;\r\n    background-color: #f9f9f9;\r\n    border-bottom: 1px solid #eceeef;\r\n}\r\n\r\n.boxplot-table tr th:first-child {\r\n    min-width: 39px;\r\n}\r\n\r\n.boxplot-table tr td:first-child {\r\n    font-size: 11px;\r\n    font-style: normal;\r\n}"

/***/ }),

/***/ "./src/app/boxplot/boxplot-table/boxplot-table.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"boxplot-table\">\r\n    <table *ngIf=\"boxplotData\" class=\"bordered-table table-hover table \">\r\n        <thead>\r\n            <tr>\r\n                <th></th>\r\n                <th *ngFor=\"let period of boxplotData.periods \">\r\n                    {{period}}\r\n                </th>\r\n            </tr>\r\n        </thead>\r\n        <tbody>\r\n            <tr>\r\n                <td>Minimum</td>\r\n                <td *ngFor=\"let stats of boxplotData.statsFormatted \" class=\"numeric \">{{stats.Min}}</td>\r\n            </tr>\r\n            <tr>\r\n                <td>5th Percentile</td>\r\n                <td *ngFor=\"let stats of boxplotData.statsFormatted \" class=\"numeric \">{{stats.P5}}</td>\r\n            </tr>\r\n            <tr>\r\n                <td>25th Percentile</td>\r\n                <td *ngFor=\"let stats of boxplotData.statsFormatted \" class=\"numeric \">{{stats.P25}}</td>\r\n            </tr>\r\n            <tr>\r\n                <td>Median</td>\r\n                <td *ngFor=\"let stats of boxplotData.statsFormatted \" class=\"numeric \">{{stats.Median}}</td>\r\n            </tr>\r\n            <tr>\r\n                <td>75th Percentile</td>\r\n                <td *ngFor=\"let stats of boxplotData.statsFormatted \" class=\"numeric \">{{stats.P75}}</td>\r\n            </tr>\r\n            <tr>\r\n                <td>95th Percentile</td>\r\n                <td *ngFor=\"let stats of boxplotData.statsFormatted \" class=\"numeric \">{{stats.P95}}</td>\r\n            </tr>\r\n            <tr>\r\n                <td>Maximum</td>\r\n                <td *ngFor=\"let stats of boxplotData.statsFormatted \" class=\"numeric \">{{stats.Max}}</td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n</div>"

/***/ }),

/***/ "./src/app/boxplot/boxplot-table/boxplot-table.component.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var boxplot_1 = __webpack_require__("./src/app/boxplot/boxplot.ts");
var BoxplotTableComponent = (function () {
    function BoxplotTableComponent() {
    }
    return BoxplotTableComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_a = typeof boxplot_1.BoxplotData !== "undefined" && boxplot_1.BoxplotData) === "function" && _a || Object)
], BoxplotTableComponent.prototype, "boxplotData", void 0);
BoxplotTableComponent = __decorate([
    core_1.Component({
        selector: 'ft-boxplot-table',
        template: __webpack_require__("./src/app/boxplot/boxplot-table/boxplot-table.component.html"),
        styles: [__webpack_require__("./src/app/boxplot/boxplot-table/boxplot-table.component.css")]
    })
], BoxplotTableComponent);
exports.BoxplotTableComponent = BoxplotTableComponent;
var _a;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/boxplot-table.component.js.map

/***/ }),

/***/ "./src/app/boxplot/boxplot.component.css":
/***/ (function(module, exports) {

module.exports = "#boxplot-legend-img {\r\n    width: 143px;\r\n    height: 296px;\r\n    text-decoration: none;\r\n    border: 1px solid lightgrey;\r\n    float: right;\r\n    margin-top: 10px;\r\n}"

/***/ }),

/***/ "./src/app/boxplot/boxplot.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"boxplot-container\" style=\"display:none;\">\r\n    <!-- <div class=\"export-chart-box\" style=\"display:block;\">\r\n            <a class=\"export-link\" (click)=\"onExportClick($event)\"\r\n                >Export chart as image</a>\r\n        </div> -->\r\n    <div id=\"indicator-details-boxplot-data\" class=\"standard-width\">\r\n        <ft-indicator-header [header]=\"header\"></ft-indicator-header>\r\n        <div id=\"boxplot-legend-img\" *ngIf=\"isAnyData()\"></div>\r\n\r\n        <ft-boxplot-chart *ngIf=\"isAnyData()\" [boxplotData]=\"boxplotData\"></ft-boxplot-chart>\r\n\r\n        <ft-boxplot-table *ngIf=\"isAnyData()\" [boxplotData]=\"boxplotData\"></ft-boxplot-table>\r\n        <div id=\"boxplot-no-data\" class=\"no-indicator-data\" *ngIf=\"!isAnyData()\">No Data</div>\r\n    </div>\r\n</div>"

/***/ }),

/***/ "./src/app/boxplot/boxplot.component.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var Observable_1 = __webpack_require__("./node_modules/rxjs/_esm5/Observable.js");
var boxplot_1 = __webpack_require__("./src/app/boxplot/boxplot.ts");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var indicator_service_1 = __webpack_require__("./src/app/shared/service/api/indicator.service.ts");
var indicator_header_component_1 = __webpack_require__("./src/app/shared/component/indicator-header/indicator-header.component.ts");
var BoxplotComponent = (function () {
    function BoxplotComponent(ftHelperService, indicatorService) {
        this.ftHelperService = ftHelperService;
        this.indicatorService = indicatorService;
    }
    BoxplotComponent.prototype.onOutsideEvent = function (event) {
        var _this = this;
        var ftHelper = this.ftHelperService;
        var groupRoot = this.ftHelperService.getCurrentGroupRoot();
        var model = ftHelper.getFTModel();
        // Get data
        var groupRootsObservable = this.indicatorService.getLatestDataForAllIndicatorsInProfileGroupForChildAreas(model.groupId, model.areaTypeId, model.parentCode, model.profileId);
        var metadataObservable = this.indicatorService.getIndicatorMetadata(model.groupId);
        var statsObservable = this.indicatorService.getIndicatorStatisticsTrendsForSingleIndicator(groupRoot.IID, groupRoot.Sex.Id, groupRoot.Age.Id, model.areaTypeId, this.ftHelperService.getCurrentComparator().Code);
        Observable_1.Observable.forkJoin([groupRootsObservable, metadataObservable, statsObservable]).subscribe(function (results) {
            var groupRoots = results[0];
            var metadataHash = results[1];
            var statsArray = results[2];
            _this.displayBoxplot(metadataHash[groupRoot.IID], groupRoot, statsArray);
            _this.ftHelperService.showAndHidePageElements();
            _this.ftHelperService.unlock();
        });
    };
    BoxplotComponent.prototype.displayBoxplot = function (metadata, groupRoot, statsArray) {
        // Define header
        this.displayHeader(metadata, groupRoot);
        // Define data
        var data = new boxplot_1.BoxplotData(metadata, this.ftHelperService.getAreaTypeName(), this.ftHelperService.getCurrentComparator().Name);
        for (var i = 0; i < statsArray.length; i++) {
            var indicatorStats = statsArray[i];
            if (indicatorStats.Stats) {
                data.addStats(indicatorStats);
            }
        }
        this.boxplotData = data;
    };
    BoxplotComponent.prototype.displayHeader = function (metadata, groupRoot) {
        var unitLabel = metadata.Unit.Label;
        if (unitLabel !== '') {
            unitLabel = ' - ' + unitLabel;
        }
        var hasDataChangedRecently = groupRoot.DateChanges && groupRoot.DateChanges.HasDataChangedRecently;
        this.header = new indicator_header_component_1.IndicatorHeader(metadata.Descriptive['Name'], hasDataChangedRecently, this.ftHelperService.getCurrentComparator().Name, metadata.ValueType.Name, unitLabel, this.ftHelperService.getSexAndAgeLabel(groupRoot));
    };
    BoxplotComponent.prototype.isAnyData = function () {
        return this.boxplotData && this.boxplotData.isAnyData();
    };
    BoxplotComponent.prototype.onExportClick = function (event) {
        event.preventDefault();
        var boxplotTable = $('.boxplot-table').hide();
        var chart = $('#indicator-details-boxplot-data');
        $('.highcharts-credits,.highcharts-contextbutton').hide();
        this.ftHelperService.saveElementAsImage(chart, 'boxplot');
        $(boxplotTable).show();
        this.ftHelperService.logEvent('ExportImage', 'Boxplot');
    };
    return BoxplotComponent;
}());
__decorate([
    core_1.HostListener('window:BoxplotSelected', ['$event']),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [Object]),
    __metadata("design:returntype", void 0)
], BoxplotComponent.prototype, "onOutsideEvent", null);
BoxplotComponent = __decorate([
    core_1.Component({
        selector: 'ft-boxplot',
        template: __webpack_require__("./src/app/boxplot/boxplot.component.html"),
        styles: [__webpack_require__("./src/app/boxplot/boxplot.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _a || Object, typeof (_b = typeof indicator_service_1.IndicatorService !== "undefined" && indicator_service_1.IndicatorService) === "function" && _b || Object])
], BoxplotComponent);
exports.BoxplotComponent = BoxplotComponent;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/boxplot.component.js.map

/***/ }),

/***/ "./src/app/boxplot/boxplot.module.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var common_1 = __webpack_require__("./node_modules/@angular/common/@angular/common.es5.js");
var shared_module_1 = __webpack_require__("./src/app/shared/shared.module.ts");
var boxplot_component_1 = __webpack_require__("./src/app/boxplot/boxplot.component.ts");
var boxplot_chart_component_1 = __webpack_require__("./src/app/boxplot/boxplot-chart/boxplot-chart.component.ts");
var boxplot_table_component_1 = __webpack_require__("./src/app/boxplot/boxplot-table/boxplot-table.component.ts");
var BoxplotModule = (function () {
    function BoxplotModule() {
    }
    return BoxplotModule;
}());
BoxplotModule = __decorate([
    core_1.NgModule({
        imports: [
            common_1.CommonModule,
            shared_module_1.SharedModule
        ],
        declarations: [
            boxplot_component_1.BoxplotComponent,
            boxplot_chart_component_1.BoxplotChartComponent,
            boxplot_table_component_1.BoxplotTableComponent
        ],
        exports: [
            boxplot_component_1.BoxplotComponent
        ]
    })
], BoxplotModule);
exports.BoxplotModule = BoxplotModule;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/boxplot.module.js.map

/***/ }),

/***/ "./src/app/boxplot/boxplot.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var BoxplotData = (function () {
    function BoxplotData(metadata, areaTypeName, comparatorName) {
        this.metadata = metadata;
        this.areaTypeName = areaTypeName;
        this.comparatorName = comparatorName;
        this.stats = [];
        this.statsFormatted = [];
        this.periods = [];
        this.min = null;
    }
    BoxplotData.prototype.addStats = function (indicatorStats) {
        // Chart data
        this.stats.push(indicatorStats.Stats);
        // Table data
        this.statsFormatted.push(indicatorStats.StatsF);
        // Time periods
        this.periods.push(indicatorStats.Period);
        // Set min limit if zero to prevent Y axis starting at negative number
        if (indicatorStats.Limits.Min === 0) {
            this.min = indicatorStats.Limits.Min;
        }
    };
    BoxplotData.prototype.isAnyData = function () {
        return this.periods.length > 0;
    };
    return BoxplotData;
}());
exports.BoxplotData = BoxplotData;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/boxplot.js.map

/***/ }),

/***/ "./src/app/england/england.component.css":
/***/ (function(module, exports) {

module.exports = "#england-table {\r\n    font-family: Arial, Helvetica, sans-serif;\r\n    background: #fff;\r\n    margin-bottom: 30px;\r\n}"

/***/ }),

/***/ "./src/app/england/england.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"england-container\" style=\"display:none;\">\r\n    <div class=\"export-chart-box\" style=\"display:block;\">\r\n        <a class=\"export-link\" (click)=\"onExportClick($event)\">Export table as image</a>\r\n    </div>\r\n    <div id=\"england-table\">\r\n        <table class=\"bordered-table table-hover\">\r\n            <thead>\r\n                <tr>\r\n                    <th>Indicator</th>\r\n                    <th>Period</th>\r\n                    <th class=\"center\">England<br/>count</th>\r\n                    <th class=\"center\">England<br/>value</th>\r\n                    <th *ngIf=\"hasRecentTrends\" class=\"center\">Recent<br/>trend</th>\r\n                    <th *ngIf=\"isChangeFromPreviousPeriodShown\" class=\"center\">Change from<br/>previous time period</th>\r\n                </tr>\r\n            </thead>\r\n            <tbody>\r\n                <tr *ngFor=\"let row of rows\">\r\n                    <td><button class=\"pLink\" href=\"#\" (click)=indicatorNameClicked(row)>{{row.indicatorName}}</button>\r\n                    <span  *ngIf=\"row.hasNewData\" style=\"margin-right: 8px;\" class=\"badge badge-success\">New data</span>\r\n                    </td>\r\n                    <td>{{row.period}}</td>\r\n                    <td class=\"numeric\" [innerHTML]=\"row.count\"></td>\r\n                    <td class=\"numeric\" (mouseover)=showValueNoteTooltip($event,row) (mouseout)=hideTooltip() (mousemove)=positionTooltip($event) [innerHTML]=\"row.value\"></td>\r\n                    <td *ngIf=\"hasRecentTrends\" class=\"center pointer\" (click)=recentTrendClicked(row) (mouseout)=hideTooltip() (mouseover)=showRecentTrendTooltip($event,row) (mousemove)=positionTooltip($event) [innerHTML]=\"row.recentTrendHtml\"></td>\r\n                    <td *ngIf=\"isChangeFromPreviousPeriodShown\" class=\"center\" [innerHTML]=\"row.changeFromPreviousHtml\"></td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n    </div>\r\n</div>\r\n"

/***/ }),

/***/ "./src/app/england/england.component.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var Observable_1 = __webpack_require__("./node_modules/rxjs/_esm5/Observable.js");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var indicator_service_1 = __webpack_require__("./src/app/shared/service/api/indicator.service.ts");
var EnglandComponent = (function () {
    function EnglandComponent(ftHelperService, indicatorService) {
        this.ftHelperService = ftHelperService;
        this.indicatorService = indicatorService;
        this.isChangeFromPreviousPeriodShown = false;
        this.hasRecentTrends = false;
    }
    EnglandComponent.prototype.onOutsideEvent = function (event) {
        var _this = this;
        var ftHelper = this.ftHelperService;
        var model = ftHelper.getFTModel();
        var groupRootsObservable = this.indicatorService.getLatestDataForAllIndicatorsInProfileGroupForChildAreas(model.groupId, model.areaTypeId, model.parentCode, model.profileId);
        var metadataObservable = this.indicatorService.getIndicatorMetadata(model.groupId);
        Observable_1.Observable.forkJoin([groupRootsObservable, metadataObservable]).subscribe(function (results) {
            var groupRoots = results[0];
            var metadataHash = results[1];
            _this.setConfig();
            _this.tooltip = new shared_1.TooltipHelper(_this.ftHelperService.newTooltipManager());
            _this.rows = [];
            for (var rootIndex in groupRoots) {
                var root = groupRoots[rootIndex];
                var indicatorId = root.IID;
                var metadata = metadataHash[indicatorId];
                var unit = !!metadata ? metadata.Unit : null;
                var row = new EnglandRow();
                _this.rows.push(row);
                row.rootIndex = rootIndex;
                row.period = root.Grouping[0].Period;
                row.indicatorName = metadata.Descriptive['Name'] + ftHelper.getSexAndAgeLabel(root);
                row.hasNewData = root.DateChanges && root.DateChanges.HasDataChangedRecently;
                // Data
                var englandData = ftHelper.getNationalComparatorGrouping(root).ComparatorData;
                var dataInfo = ftHelper.newCoreDataSetInfo(englandData);
                row.value = ftHelper.newValueDisplayer(unit).byDataInfo(dataInfo);
                row.count = ftHelper.formatCount(dataInfo);
                row.hasValueNote = dataInfo.isNote();
                row.noteId = englandData.NoteId;
                // Recent trend
                if (!!root.RecentTrends) {
                    _this.setUpRecentTrendOnRow(row, root, englandData.AreaCode, dataInfo);
                }
            }
            ftHelper.showAndHidePageElements();
            ftHelper.unlock();
        });
    };
    EnglandComponent.prototype.setUpRecentTrendOnRow = function (row, root, areaCode, dataInfo) {
        var ftHelper = this.ftHelperService;
        var polarityId = root.PolarityId;
        if (dataInfo.isValue() && root.RecentTrends[areaCode]) {
            '';
            // Recent trend available
            var recentTrend = root.RecentTrends[areaCode];
            row.recentTrend = recentTrend;
            row.recentTrendHtml = ftHelper.getTrendMarkerImage(recentTrend.Marker, polarityId);
            row.changeFromPreviousHtml = ftHelper.getTrendMarkerImage(recentTrend.MarkerForMostRecentValueComparedWithPreviousValue, polarityId);
        }
        else {
            // No trend image
            row.recentTrendHtml = ftHelper.getTrendMarkerImage(0 /*TrendMarker.CannotBeCalculated*/, polarityId);
            row.changeFromPreviousHtml = row.recentTrendHtml;
        }
    };
    EnglandComponent.prototype.hideTooltip = function () {
        this.tooltip.hide();
    };
    EnglandComponent.prototype.indicatorNameClicked = function (row) {
        this.ftHelperService.goToMetadataPage(row.rootIndex);
    };
    EnglandComponent.prototype.showValueNoteTooltip = function (event, row) {
        if (row.hasValueNote) {
            var tooltipProvider = this.ftHelperService.newValueNoteTooltipProvider();
            this.tooltip.displayHtml(event, tooltipProvider.getHtmlFromNoteId(row.noteId));
        }
    };
    EnglandComponent.prototype.showRecentTrendTooltip = function (event, row) {
        var tooltipProvider = this.ftHelperService.newRecentTrendsTooltip();
        this.tooltip.displayHtml(event, tooltipProvider.getTooltipByData(row.recentTrend));
    };
    EnglandComponent.prototype.positionTooltip = function (event) {
        this.tooltip.reposition(event);
    };
    EnglandComponent.prototype.recentTrendClicked = function (row) {
        this.ftHelperService.recentTrendSelected().byGroupRoot(row.rootIndex);
    };
    EnglandComponent.prototype.setConfig = function () {
        var config = this.ftHelperService.getFTConfig();
        this.hasRecentTrends = config.hasRecentTrends;
        this.isChangeFromPreviousPeriodShown = config.isChangeFromPreviousPeriodShown;
    };
    EnglandComponent.prototype.onExportClick = function (event) {
        event.preventDefault();
        // Prepare view
        var $trendInfoIcon = $('.trend-info').hide();
        var trendLegend = $('<div id="trend-legend">'
            + $('#trend-marker-legend').html() +
            '</div>');
        var table = $('#england-table');
        table.prepend(trendLegend);
        // Export image
        this.ftHelperService.saveElementAsImage(table, 'england');
        // Restore view
        $('#trend-legend').remove();
        $trendInfoIcon.show();
        // Log event for analytics
        this.ftHelperService.logEvent('ExportImage', 'England');
    };
    return EnglandComponent;
}());
__decorate([
    core_1.HostListener('window:EnglandSelected', ['$event']),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [Object]),
    __metadata("design:returntype", void 0)
], EnglandComponent.prototype, "onOutsideEvent", null);
EnglandComponent = __decorate([
    core_1.Component({
        selector: 'ft-england',
        template: __webpack_require__("./src/app/england/england.component.html"),
        styles: [__webpack_require__("./src/app/england/england.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _a || Object, typeof (_b = typeof indicator_service_1.IndicatorService !== "undefined" && indicator_service_1.IndicatorService) === "function" && _b || Object])
], EnglandComponent);
exports.EnglandComponent = EnglandComponent;
var EnglandRow = (function () {
    function EnglandRow() {
    }
    return EnglandRow;
}());
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/england.component.js.map

/***/ }),

/***/ "./src/app/map/google-map/google-map.component.css":
/***/ (function(module, exports) {

module.exports = ".googleMapNg {\r\n    position: relative;\r\n    background-color: #fff;\r\n    border: 1px solid #CCC;\r\n    width: 500px;\r\n    height: 600px;\r\n    float: left;\r\n    margin-bottom: 10px;\r\n    margin-top: 10px;\r\n    clear: both;\r\n}\r\n\r\n#floating-panel {\r\n    position: absolute;\r\n    margin-top: 92px;\r\n    z-index: 5;\r\n    padding-left: 8px;\r\n    margin-left: 10px;\r\n}\r\n\r\n#wrapper {\r\n    position: relative;\r\n}\r\n\r\n.map-control {\r\n    float: left;\r\n    clear: both;\r\n    position: relative;\r\n    z-index: 7;\r\n    pointer-events: auto;\r\n}\r\n\r\n.map-control-layers-toggle {\r\n    background-image: url(/images/layers.png);\r\n    background-repeat: no-repeat;\r\n    width: 28px;\r\n    height: 28px;\r\n    display: inline-block;\r\n    margin-right: 3px\r\n}\r\n\r\n.layerControl p {\r\n    margin-left: 5px;\r\n}\r\n\r\n.layerControl div.basemap {\r\n    margin: 3px 0;\r\n    padding-right: 3px;\r\n}\r\n\r\n.layerControl label.basemapLabel {\r\n    display: block;\r\n    margin-bottom: 0px;\r\n    height: 40px;\r\n}\r\n\r\n.layerControl div.basemap:hover,\r\n.layerControl div.opacity:hover {\r\n    background-color: rgba(220, 220, 220, .7);\r\n}\r\n\r\n.layerControl div:hover span {\r\n    border: 1px solid #666;\r\n}\r\n\r\n.layerControl div.selected span {\r\n    border: 1px solid #66F;\r\n}\r\n\r\n.layerControl div .layerControl label span {\r\n    border: 1px solid #CCC;\r\n    width: 70px;\r\n    height: 30px;\r\n    background: url(/images/basemaps.jpg) no-repeat;\r\n    display: inline-block;\r\n    vertical-align: middle;\r\n}\r\n\r\n.layerControl div.basemap span {\r\n    margin: 0 5px;\r\n}\r\n\r\n.layerControl div.None span {\r\n    background: #FFF;\r\n}\r\n\r\n.layerControl div.Streets span {\r\n    background-position: -6px -113px;\r\n}\r\n\r\n.layerControl div#opacity {\r\n    margin-left: 5px;\r\n}\r\n\r\n.layerControl div.opacity {\r\n    display: inline-block;\r\n    width: 34px;\r\n    text-align: center;\r\n}\r\n\r\n.layerControl div.opacity span {\r\n    border: 1px solid #CCC;\r\n    display: inline-block;\r\n    width: 30px;\r\n    height: 25px;\r\n}\r\n\r\n.layerControl div.opacity:hover span,\r\n.layerControl div.opacity.selected span {\r\n    border-color: #666;\r\n}\r\n\r\n.layerControl div.opacity label {\r\n    font-size: .65em;\r\n}\r\n\r\n.layerControl div.opacity span {\r\n    background: url(/images/opacity.jpg) no-repeat;\r\n}\r\n\r\n.layerControl input {\r\n    display: none;\r\n}\r\n\r\n.layerControl>label {\r\n    display: block;\r\n    height: 100%;\r\n    width: 100%;\r\n}\r\n\r\n.info {\r\n    padding: 2px;\r\n    font: 14px/16px Arial, Helvetica, sans-serif;\r\n    background: white;\r\n    background: rgba(255, 255, 255, 0.8);\r\n    -webkit-box-shadow: 0 0 15px rgba(0, 0, 0, 0.2);\r\n            box-shadow: 0 0 15px rgba(0, 0, 0, 0.2);\r\n    border-radius: 5px;\r\n}\r\n\r\n.info h4 {\r\n    margin: 0;\r\n    color: #777;\r\n    padding: 0;\r\n}\r\n\r\n.export-chart-box {\r\n    margin: 0;\r\n}"

/***/ }),

/***/ "./src/app/map/google-map/google-map.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"export-chart-box\">\r\n    <a class=\"export-link\" (click)=\"onExportClick($event)\">Export map as image</a>\r\n</div>\r\n<div id=\"wrapper\">\r\n    <div [hidden]=\"isError\" id=\"google-map\" #googleMap class=\"googleMapNg\"></div>\r\n    <div *ngIf=\"isError\" id=\"polygonError\" class=\"googleMapNg\"> {{errorMessage}} </div>\r\n    <div [hidden]=\"isError\" id=\"floating-panel\" class=\"layerControl info map-control\">\r\n        <a class=\"map-control-layers-toggle\" href=\"#\" title=\"Layers\" *ngIf=\"!showOverlay\" (mouseover)=\"displayOverlay()\"></a>\r\n        <div id=\"mapOptions\" *ngIf=\"showOverlay\" (mouseleave)=\"hideOverlay()\">\r\n            <p>Background map</p>\r\n            <label *ngFor=\"let baseMap of baseMaps;let idx = index\" class=\"basemapLabel\">\r\n                    <div class=\"basemap {{baseMap.cssClass}}\">\r\n                    <input type=\"radio\" name=\"baseMap\"  [value]=\"baseMap.val\" \r\n                    [checked]=\"(idx === 0)\" (click)= \"onOverlaySelectionChange(baseMap)\">\r\n                    <span></span>{{baseMap.name}}\r\n                    </div>\r\n                    <br/>\r\n                </label>\r\n            <p>Transparency</p>\r\n            <div class=\"opacity\" *ngFor=\"let opac of opacityArray;let idx = index;\" [attr.selected]=\"opac/100 == fillOpacity?true : null\">\r\n                <input type=\"radio\" name=\"opacity\" value=\"{{opac}}\" id=\"opacity_{{opac}}\" (click)=\"onOpacitySelectionChange(opac)\">\r\n                <label for=\"opacity_{{opac}}\">\r\n                    <span [ngStyle]=\"{\r\n                    'background-position':(opac * -4.1 + 37) + 'px 0px'}\"></span>\r\n                    {{opac}} %\r\n                    </label>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>"

/***/ }),

/***/ "./src/app/map/google-map/google-map.component.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
__webpack_require__("./node_modules/rxjs/rx.js");
var googleMap_service_1 = __webpack_require__("./src/app/map/googleMap.service.ts");
var indicator_service_1 = __webpack_require__("./src/app/shared/service/api/indicator.service.ts");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var coreDataHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/coreDataHelper.service.ts");
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var _ = __webpack_require__("./node_modules/underscore/underscore.js");
var GoogleMapComponent = (function () {
    function GoogleMapComponent(mapService, indicatorService, ftHelperService, coreDataHelper) {
        this.mapService = mapService;
        this.indicatorService = indicatorService;
        this.ftHelperService = ftHelperService;
        this.coreDataHelper = coreDataHelper;
        this.mapInit = new core_1.EventEmitter();
        this.hoverAreaCodeChanged = new core_1.EventEmitter();
        this.selectedAreaChanged = new core_1.EventEmitter();
        this.areaTypeId = null;
        this.currentAreaCode = null;
        this.areaCodeColour = null;
        this.sortedCoreData = null;
        this.path = this.ftHelperService.getURL().img;
        this.isError = false;
        this.showOverlay = false;
        this.currentPolygons = [];
        this.selectedPolygon = null;
        this.baseMaps = [
            {
                name: 'No background',
                val: 0,
                cssClass: 'None'
            },
            {
                name: 'Streets',
                val: 1,
                cssClass: 'Streets'
            }
        ];
        this.fillOpacity = 1.0;
        this.opacityArray = [20, 40, 60, 80, 100];
        /*purple boundry on polygon with bold border -represent that polygon is highlighted but not in a table */
        this.purpleHighlightPolyOption = { strokeColor: '#9D78D2', strokeWeight: 3 };
        /*black boundry on polygon with bold border -represent that polygon is in table */
        this.blackSelectedPolyOption = { strokeColor: '#000000', strokeWeight: 3 };
        /*black boundry on polygon with bolder border -represent that polygon is in table as well as highlighted */
        this.blackHighlightPolyOption = { strokeColor: '#000000', strokeWeight: 5 };
        /* gray boundry on polygon with normal regular border */
        this.grayPolyOption = { strokeColor: '#333333', strokeWeight: 1 };
        /*black boundry on polygon with bold border -represent that polygon is in table */
        this.unselectedPolyOption = { strokeColor: '#333333', strokeWeight: 1 };
    }
    GoogleMapComponent.prototype.ngOnChanges = function (changes) {
        if (changes['areaTypeId']) {
            if (this.areaTypeId) {
                this.loadMap();
                this.loadPolygon(this.areaTypeId, this.path);
            }
        }
        if (changes['currentAreaCode']) {
            var areaCode = changes['currentAreaCode'].currentValue;
            if (areaCode) {
                this.highlightPolygon(this.currentAreaCode);
            }
            else {
                this.unhighlightSelectedPolygon();
            }
        }
        if (changes['refreshColour']) {
            var localRefreshColour = changes['refreshColour'].currentValue;
            if (localRefreshColour !== undefined) {
                if (this.areaCodeColour) {
                    this.colourFillPolygon(false);
                }
            }
        }
        if (changes['selectedAreaList']) {
            var localSelectedAreaList = changes['selectedAreaList'].currentValue;
            if (localSelectedAreaList) {
                this.removeSelectedPolygon();
            }
        }
    };
    GoogleMapComponent.prototype.unhighlightSelectedPolygon = function () {
        if (this.selectedPolygon) {
            this.selectedPolygon.setOptions(this.unselectedPolyOption);
        }
    };
    GoogleMapComponent.prototype.removeSelectedPolygon = function () {
        if (this.map) {
            for (var i = 0; i < this.currentPolygons.length; i++) {
                var areaCode = this.currentPolygons[i].get('areaCode');
                if (!_.contains(this.selectedAreaList, areaCode)) {
                    this.currentPolygons[i].setOptions(this.grayPolyOption);
                }
                else {
                    this.currentPolygons[i].setOptions(this.blackSelectedPolyOption);
                }
            }
        }
    };
    GoogleMapComponent.prototype.displayOverlay = function () {
        this.showOverlay = true;
    };
    GoogleMapComponent.prototype.hideOverlay = function () {
        this.showOverlay = false;
    };
    GoogleMapComponent.prototype.onOverlaySelectionChange = function (basemap) {
        this.selectedbaseMap = Object.assign({}, this.selectedbaseMap, basemap);
        this.styleMap(this.selectedbaseMap);
    };
    GoogleMapComponent.prototype.onOpacitySelectionChange = function (opacity) {
        this.fillOpacity = opacity / 100;
        this.loadPolygon(this.areaTypeId, this.path);
    };
    GoogleMapComponent.prototype.loadMap = function () {
        if (!this.isBoundaryNotSupported) {
            /// Load from GoogleMapService and style it 
            var mapOptions = {
                zoom: 6,
                disableDoubleClickZoom: false,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                panControl: false,
                zoomControl: true,
                zoomControlOptions: { position: google.maps.ControlPosition.TOP_LEFT },
                scaleControl: false,
                streetViewControl: false,
                mapTypeControl: false,
                fullscreenControl: true,
                backgroundColor: 'hsla(0, 0%, 0%, 0)',
            };
            var mapContainer = null;
            if (this.mapEl && this.mapEl.nativeElement) {
                mapContainer = this.mapEl.nativeElement;
            }
            this.map = this.mapService.loadMap(mapContainer, mapOptions);
            this.selectedbaseMap = this.baseMaps[0];
            if (this.baseMaps) {
                this.onOverlaySelectionChange(this.baseMaps[0]);
            }
            if (this.areaTypeId) {
                this.loadPolygon(this.areaTypeId, this.path);
            }
            this.mapInit.emit({
                map: this.map,
            });
        }
    };
    GoogleMapComponent.prototype.styleMap = function (selectedbaseMap) {
        var _this = this;
        if (this.map) {
            var noTiles = 'noTiles';
            var styleArrayForNoBackground = [];
            if (selectedbaseMap.val === 0) {
                var visibilityOff = [{ visibility: 'off' }];
                styleArrayForNoBackground = [
                    {
                        stylers: [
                            {
                                color: '#ffffff',
                                fillOpacity: 0.0
                            }
                        ]
                    },
                    {
                        featureType: 'road',
                        elementType: 'geometry',
                        stylers: visibilityOff
                    }, {
                        featureType: 'road',
                        elementType: 'labels',
                        stylers: visibilityOff
                    }
                ];
            }
            this.map.setOptions({ styles: styleArrayForNoBackground });
            this.map.mapTypes.set(noTiles, new NoTileMapType());
            this.setMapCenter();
            google.maps.event.addListener(this.map, 'idle', function (event) {
                var bounds = _this.map.getCenter();
                google.maps.event.trigger(_this.map, 'resize');
                _this.map.setCenter(bounds);
            });
        }
    };
    GoogleMapComponent.prototype.setMapCenter = function () {
        if (this.map) {
            var bounds = new google.maps.LatLngBounds();
            var position = new google.maps.LatLng(53.415649, -2.209015);
            bounds.extend(position);
            this.map.setCenter(bounds.getCenter());
            this.map.setZoom(6);
        }
    };
    GoogleMapComponent.prototype.loadPolygon = function (areaTypeId, path) {
        var _this = this;
        if (this.map && this.areaTypeId && !this.isBoundaryNotSupported) {
            this.mapService.loadBoundries(areaTypeId, path)
                .subscribe(function (data) {
                _this.boundry = data;
                _this.isError = false;
                _this.removePolygon();
                _this.fillPolygon(_this.boundry, _this.fillOpacity);
                _this.colourFillPolygon(true);
            }, function (error) {
                _this.isError = true;
                _this.errorMessage = error;
            });
        }
    };
    GoogleMapComponent.prototype.removePolygon = function () {
        if (this.currentPolygons !== undefined) {
            this.currentPolygons.forEach(function (element) {
                element.setMap(null);
            });
            this.currentPolygons.length = 0;
        }
    };
    GoogleMapComponent.prototype.getPolygonCoordinates = function (coordinates) {
        var coords = [];
        for (var i = 0; i < coordinates.length; i++) {
            for (var j = 0; j < coordinates[i].length; j++) {
                var path = [];
                for (var k = 0; k < coordinates[i][j].length; k++) {
                    var coord = new google.maps.LatLng(coordinates[i][j][k][1], coordinates[i][j][k][0]);
                    path.push(coord);
                }
                coords.push(path);
            }
        }
        return coords;
    };
    GoogleMapComponent.prototype.fillPolygon = function (boundry, opacity) {
        var _this = this;
        if (boundry.features) {
            // Variables to track most recent mouseover event
            var overDate_1 = null;
            var overAreaCode_1 = null;
            var infoWindow_1 = new google.maps.InfoWindow();
            var _loop_1 = function (x) {
                var areaCode = boundry.features[x].properties.AreaCode;
                var coordinates = boundry.features[x].geometry.coordinates;
                var coords = this_1.getPolygonCoordinates(coordinates);
                // Set polygon fill colour
                var fillColour = '#B0B0B2';
                if (this_1.areaCodeColour && this_1.areaCodeColour.length > 0) {
                    fillColour = this_1.areaCodeColour.get(areaCode);
                }
                var polygon = new google.maps.Polygon({
                    paths: coords,
                    strokeColor: '#333333',
                    strokeOpacity: 0.8,
                    strokeWeight: 1,
                    fillColor: fillColour,
                    fillOpacity: opacity,
                    clickable: true
                });
                polygon.set('areaCode', areaCode);
                polygon.setMap(this_1.map);
                google.maps.event.addListener(polygon, 'mouseover', function (event) {
                    overAreaCode_1 = areaCode;
                    overDate_1 = new Date();
                    // Display tooltip
                    var tooltip = _this.getToolTipContent(areaCode);
                    if (tooltip) {
                        infoWindow_1.setContent(tooltip);
                        _this.setInfoWindowPosition(event, infoWindow_1);
                        infoWindow_1.open(_this.map);
                    }
                    _this.setPolygonBorderColour(polygon);
                });
                google.maps.event.addListener(polygon, 'mousemove', function (event) {
                    _this.setInfoWindowPosition(event, infoWindow_1);
                });
                google.maps.event.addListener(polygon, 'mouseout', function (event) {
                    // Wait in case immediate mouseover event and this mouseover event was
                    // caused by mouse moving over the infowindow
                    setTimeout(function () {
                        var time = new Date().getTime();
                        if (time - overDate_1.getTime() > 25 && areaCode == overAreaCode_1)
                            infoWindow_1.close();
                    }, 25);
                    _this.setPolygonBorderColour(polygon);
                });
                google.maps.event.addListener(polygon, 'click', function (event) {
                    if (_this.sortedCoreData[areaCode] && _this.ftHelperService.isValuePresent(_this.sortedCoreData[areaCode].ValF)) {
                        polygon.setOptions(_this.blackSelectedPolyOption);
                        _this.selectedAreaChanged.emit({ areaCode: areaCode });
                    }
                });
                this_1.currentPolygons.push(polygon);
            };
            var this_1 = this;
            for (var x = 0; x < boundry.features.length; x++) {
                _loop_1(x);
            }
        }
    };
    GoogleMapComponent.prototype.setPolygonBorderColour = function (polygon) {
        var currentAreaCode = polygon.get('areaCode');
        if (_.contains(this.selectedAreaList, currentAreaCode)) {
            polygon.setOptions(this.blackSelectedPolyOption);
        }
        else {
            polygon.setOptions(this.grayPolyOption);
        }
    };
    GoogleMapComponent.prototype.setInfoWindowPosition = function (event, infoWindow) {
        var pos = event.latLng;
        infoWindow.setPosition(new google.maps.LatLng(pos.lat() + 0.02, pos.lng()));
    };
    GoogleMapComponent.prototype.getToolTipContent = function (areaCode) {
        this.hoverAreaCodeChanged.emit({ areaCode: areaCode });
        var currentGrpRoot = this.ftHelperService.getCurrentGroupRoot();
        var data = this.ftHelperService.getMetadata(currentGrpRoot.IID);
        var unit = data.Unit;
        var areaName = '';
        if (areaCode) {
            areaName = this.ftHelperService.getAreaName(areaCode);
        }
        var value = '';
        if (unit !== undefined && this.sortedCoreData[areaCode] !== undefined
            && this.ftHelperService.isValuePresent(this.sortedCoreData[areaCode].ValF)) {
            value = this.sortedCoreData !== null ? '<br>' + this.coreDataHelper.valueWithUnit(unit).
                getFullLabel(this.sortedCoreData[areaCode].ValF)
                : '<br>-';
        }
        var toolTipcontent;
        if (areaName !== '' || value !== '') {
            toolTipcontent = '<b>' + areaName + '</b>' + value;
        }
        return toolTipcontent;
    };
    GoogleMapComponent.prototype.highlightPolygon = function (areaCode) {
        if (this.map) {
            var polygon = _.where(this.currentPolygons, { areaCode: areaCode })[0];
            if (this.selectedPolygon) {
                var areaCode_1 = this.selectedPolygon.get('areaCode');
                this.selectedPolygon.setMap(null);
                if (_.contains(this.selectedAreaList, areaCode_1)) {
                    this.selectedPolygon.setOptions(this.blackSelectedPolyOption);
                }
                else {
                    this.selectedPolygon.setOptions(this.grayPolyOption);
                }
                this.selectedPolygon.setMap(this.map);
            }
            this.selectedPolygon = polygon;
            if (polygon) {
                polygon.setMap(null);
                if (!_.contains(this.selectedAreaList, areaCode)) {
                    polygon.setOptions(this.purpleHighlightPolyOption);
                }
                else {
                    polygon.setOptions(this.blackHighlightPolyOption);
                }
                polygon.setMap(this.map);
            }
        }
    };
    GoogleMapComponent.prototype.colourFillPolygon = function (center) {
        if (this.map) {
            var regionPolygons = [];
            var currentComparatorId = this.ftHelperService.getComparatorId();
            for (var i = 0; i < (this.currentPolygons.length); i++) {
                var polygon = this.currentPolygons[i];
                // Set polygon fill colour             
                polygon.setMap(null);
                var areaCode = polygon.get('areaCode');
                var color = this.areaCodeColour.get(areaCode);
                if (color === undefined) {
                    color = '#B0B0B2';
                }
                polygon.set('fillColor', color);
                polygon.setMap(this.map);
                if (currentComparatorId !== shared_1.ComparatorIds.national) {
                    var coreDataset = this.sortedCoreData[areaCode];
                    if (coreDataset) {
                        regionPolygons.push(polygon);
                    }
                }
            }
            /*if Benchmark is region, center and zoom in into that region */
            if (center) {
                if (regionPolygons.length > 0 && currentComparatorId !== shared_1.ComparatorIds.national) {
                    var bounds = new google.maps.LatLngBounds();
                    for (var i = 0; i < regionPolygons.length; i++) {
                        bounds.extend(this.getPolygonBounds(regionPolygons[i]).getCenter());
                    }
                    this.map.fitBounds(bounds);
                    this.map.setCenter(bounds.getCenter());
                    this.map.setZoom(7);
                }
                if (currentComparatorId === shared_1.ComparatorIds.national) {
                    this.setMapCenter();
                }
            }
        }
    };
    GoogleMapComponent.prototype.getPolygonBounds = function (polygon) {
        var bounds = new google.maps.LatLngBounds();
        polygon.getPath().forEach(function (element, index) { bounds.extend(element); });
        return bounds;
    };
    GoogleMapComponent.prototype.onExportClick = function (event) {
        event.preventDefault();
        var chartTitle = this.buildChartTitle();
        var root = this.ftHelperService.getCurrentGroupRoot();
        var indicatorName = this.ftHelperService.getMetadataHash()[root.IID].Descriptive.Name +
            this.ftHelperService.getSexAndAgeLabel(root);
        var title = '<b>Map of ' + this.ftHelperService.getAreaTypeName() +
            's in ' + this.ftHelperService.getCurrentComparator().Name +
            ' for ' + indicatorName + '<br/> (' + chartTitle + ')</b>';
        $('<div id="map-export-title" style="text-align: center; font-family:Arial;">' +
            title + '</div>').appendTo(this.mapEl.nativeElement);
        this.ftHelperService.saveElementAsImage(this.mapEl.nativeElement, 'Map');
        $('#map-export-title').remove();
        this.ftHelperService.logEvent('ExportImage', 'Map');
    };
    GoogleMapComponent.prototype.buildChartTitle = function () {
        var currentGrpRoot = this.ftHelperService.getCurrentGroupRoot();
        var data = this.ftHelperService.getMetadata(currentGrpRoot.IID);
        var unit = data.Unit;
        var unitLabel = (typeof unit.Label !== 'undefined') ? unit.Label : '';
        var period = currentGrpRoot.Grouping[0].Period;
        return data.ValueType.Name + ' - ' + unitLabel + ' ' + period;
    };
    return GoogleMapComponent;
}());
__decorate([
    core_1.ViewChild('googleMap'),
    __metadata("design:type", typeof (_a = typeof core_1.ElementRef !== "undefined" && core_1.ElementRef) === "function" && _a || Object)
], GoogleMapComponent.prototype, "mapEl", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], GoogleMapComponent.prototype, "mapInit", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], GoogleMapComponent.prototype, "hoverAreaCodeChanged", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], GoogleMapComponent.prototype, "selectedAreaChanged", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Number)
], GoogleMapComponent.prototype, "areaTypeId", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", String)
], GoogleMapComponent.prototype, "currentAreaCode", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], GoogleMapComponent.prototype, "areaCodeColour", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], GoogleMapComponent.prototype, "refreshColour", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], GoogleMapComponent.prototype, "isBoundaryNotSupported", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], GoogleMapComponent.prototype, "selectedAreaList", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], GoogleMapComponent.prototype, "sortedCoreData", void 0);
GoogleMapComponent = __decorate([
    core_1.Component({
        selector: 'ft-google-map',
        template: __webpack_require__("./src/app/map/google-map/google-map.component.html"),
        styles: [__webpack_require__("./src/app/map/google-map/google-map.component.css")],
        providers: [googleMap_service_1.GoogleMapService]
    }),
    __metadata("design:paramtypes", [typeof (_b = typeof googleMap_service_1.GoogleMapService !== "undefined" && googleMap_service_1.GoogleMapService) === "function" && _b || Object, typeof (_c = typeof indicator_service_1.IndicatorService !== "undefined" && indicator_service_1.IndicatorService) === "function" && _c || Object, typeof (_d = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _d || Object, typeof (_e = typeof coreDataHelper_service_1.CoreDataHelperService !== "undefined" && coreDataHelper_service_1.CoreDataHelperService) === "function" && _e || Object])
], GoogleMapComponent);
exports.GoogleMapComponent = GoogleMapComponent;
var NoTileMapType = (function () {
    function NoTileMapType() {
        this.tileSize = new google.maps.Size(1024, 1024);
        this.maxZoom = 20;
    }
    NoTileMapType.prototype.getTile = function (tileCoord, zoom, ownerDocument) {
        return ownerDocument.createElement('div');
    };
    NoTileMapType.prototype.releaseTile = function (tile) {
        throw new Error('Method not implemented.');
    };
    return NoTileMapType;
}());
exports.NoTileMapType = NoTileMapType;
var _a, _b, _c, _d, _e;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/google-map.component.js.map

/***/ }),

/***/ "./src/app/map/googleMap.service.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var http_1 = __webpack_require__("./node_modules/@angular/http/@angular/http.es5.js");
__webpack_require__("./node_modules/rxjs/rx.js");
var Observable_1 = __webpack_require__("./node_modules/rxjs/_esm5/Observable.js");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var GoogleMapService = (function () {
    function GoogleMapService(http, ftHelperservice) {
        this.http = http;
        this.ftHelperservice = ftHelperservice;
        this.isLoaded = new core_1.EventEmitter();
    }
    GoogleMapService.prototype.loadMap = function (mapDiv, mapOptions) {
        this.map = null;
        if (mapDiv != null) {
            this.map = new google.maps.Map(mapDiv, mapOptions);
        }
        return this.map;
    };
    GoogleMapService.prototype.loadBoundries = function (areaTypeId, path) {
        var baseUrl = path + 'maps/' + areaTypeId + '/geojson/boundaries.js';
        return this.http.get(baseUrl).map(function (res) { return res.json(); }).catch(this.handleError);
    };
    GoogleMapService.prototype.handleError = function (error) {
        var errorMessage = 'Unsupported map type. Maps are not available for this area type.';
        return Observable_1.Observable.throw(errorMessage);
    };
    return GoogleMapService;
}());
GoogleMapService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [typeof (_a = typeof http_1.Http !== "undefined" && http_1.Http) === "function" && _a || Object, typeof (_b = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _b || Object])
], GoogleMapService);
exports.GoogleMapService = GoogleMapService;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/googleMap.service.js.map

/***/ }),

/***/ "./src/app/map/map-chart/map-chart.component.css":
/***/ (function(module, exports) {

module.exports = ".chartClass{\r\n    height: 242px;\r\n\twidth: 490px;\r\n\tmargin-left: 10px;\r\n    float: left;\r\n}"

/***/ }),

/***/ "./src/app/map/map-chart/map-chart.component.html":
/***/ (function(module, exports) {

module.exports = "<span class=\"export-chart-box\" style=\"margin-left: 11px;float: left\">\r\n    <a class=\"export-link\" (click)=\"onExportClick($event)\">Export chart as image</a>\r\n</span>\r\n<div #mapChart class=\"chartClass\"></div>\r\n"

/***/ }),

/***/ "./src/app/map/map-chart/map-chart.component.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var coreDataHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/coreDataHelper.service.ts");
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var Highcharts = __webpack_require__("./node_modules/highcharts/highcharts.js");
__webpack_require__("./node_modules/highcharts/modules/exporting.js")(Highcharts);
__webpack_require__("./node_modules/highcharts/highcharts-more.js")(Highcharts);
var _ = __webpack_require__("./node_modules/underscore/underscore.js");
var MapChartComponent = (function () {
    function MapChartComponent(ftHelperService, coreDataHelperService) {
        this.ftHelperService = ftHelperService;
        this.coreDataHelperService = coreDataHelperService;
        this.sortedCoreData = null;
        this.areaTypeId = null;
        this.currentAreaCode = null;
        this.areaCodeColour = null;
        this.selectedAreaCodeChanged = new core_1.EventEmitter();
        this.chartData = [];
        this.hoverAreaCodeChanged = new core_1.EventEmitter();
    }
    MapChartComponent.prototype.ngOnChanges = function (changes) {
        if (changes['isBoundaryNotSupported']) {
            var localBoundryNotSupported = changes['isBoundaryNotSupported'].currentValue;
            if (localBoundryNotSupported !== undefined) {
                if (localBoundryNotSupported) {
                    this.chartData = [];
                }
            }
        }
        if (changes['sortedCoreData']) {
            var sortedataChange = changes['sortedCoreData'].currentValue;
            if (sortedataChange) {
                this.loadHighChart(sortedataChange);
            }
        }
        if (changes['currentAreaCode']) {
            var currentAreaCode = changes['currentAreaCode'].currentValue;
            if (currentAreaCode) {
                this.highlightArea(currentAreaCode);
            }
        }
        if (changes['areaCodeColour']) {
            var areaCodeColour = changes['areaCodeColour'].currentValue;
            if (areaCodeColour) {
                if (this.sortedCoreData) {
                    this.loadHighChart(this.sortedCoreData);
                }
            }
        }
    };
    MapChartComponent.prototype.loadHighChart = function (sortedata) {
        var _this = this;
        var xVal = 0;
        var xAxisCategories = [];
        var errorData = [];
        this.chartData = [];
        var regionValues = [];
        var x = 0;
        var currentGrpRoot = this.ftHelperService.getCurrentGroupRoot();
        var unit = this.ftHelperService.getMetadata(currentGrpRoot.IID).Unit;
        var valuesForBarChart = [];
        var extraTooltip = '';
        Object.keys(sortedata).forEach(function (key) {
            var value = sortedata[key];
            var colour = null;
            if (_this.areaCodeColour) {
                colour = _this.areaCodeColour.get(key);
            }
            if (value.ValF !== '-') {
                valuesForBarChart.push({
                    Colour: colour,
                    AreaCode: value.AreaCode,
                    Val: value.Val,
                    LCI: value.LoCI,
                    UCI: value.UpCI,
                    ValF: value.ValF,
                    LoCIF: value.LoCIF,
                    UpCIF: value.UpCIF,
                    NoteId: value.NoteId
                });
            }
        });
        valuesForBarChart.sort(function (leftside, righside) {
            if (leftside.Val < righside.Val) {
                return -1;
            }
            if (leftside.Val > righside.Val) {
                return 1;
            }
            return 0;
        });
        for (var _i = 0, valuesForBarChart_1 = valuesForBarChart; _i < valuesForBarChart_1.length; _i++) {
            var value = valuesForBarChart_1[_i];
            this.chartData.push({ color: value.Colour, x: xVal, key: value.AreaCode, y: value.Val });
            xVal++;
            var areaName = this.ftHelperService.getAreaName(value.AreaCode);
            xAxisCategories.push({
                AreaName: areaName, ValF: value.ValF, LoCIF: value.LoCIF,
                UpCIF: value.UpCIF, AreaCode: value.AreaCode, NoteId: value.NoteId
            });
            errorData.push([value.LoCI, value.UpCI]);
        }
        var series = [
            {
                type: 'column',
                name: 'Value',
                data: this.chartData
            },
            {
                type: 'errorbar',
                name: 'My Errors',
                data: errorData,
                zIndex: 1000,
                color: '#666666'
            }
        ];
        var comparatorName = this.ftHelperService.getCurrentComparator().Name;
        var valueWithUnit = this.coreDataHelperService.valueWithUnit(unit);
        for (var _a = 0, _b = currentGrpRoot.Grouping; _a < _b.length; _a++) {
            var grouping = _b[_a];
            if (grouping.ComparatorData.ValF === '-')
                continue;
            var valF = grouping.ComparatorData.ValF;
            var val = grouping.ComparatorData.Val;
            if (grouping.ComparatorId === 4 && grouping.ComparatorData.AreaCode === shared_1.AreaCodes.England) {
                // England data
                regionValues[series.length] = valF;
                series.push({
                    type: 'line',
                    name: 'England',
                    color: '#333333',
                    data: [[0, val], [xVal - 1, val]]
                });
                extraTooltip += '<br>England: ' + valueWithUnit.getFullLabel(valF);
            }
            else if (grouping.ComparatorId !== 4 && grouping.ComparatorId === this.ftHelperService.getComparatorId()) {
                // Subnational data
                regionValues[series.length] = valF;
                series.push({
                    type: 'line',
                    name: comparatorName,
                    data: [[0, val], [xVal - 1, val]],
                    color: '#0000FF'
                });
                extraTooltip += '<br>' + comparatorName + ': ' + valueWithUnit.getFullLabel(valF);
            }
        }
        var yAxis = {
            labels: {
                formatter: function () {
                    return this.value;
                },
                style: {
                    color: '#999999'
                }
            },
            title: {
                text: ''
            }
        };
        var chartTitle = this.buildChartTitle();
        var valueNotes = this.ftHelperService.getValueNotes();
        var areaCodeChanged = this.hoverAreaCodeChanged;
        var _thisLocal = this;
        // Locals for events
        var unhighlightArea = this.unhighlightArea;
        var chartData = this.chartData;
        var chartOptions = {
            title: {
                text: chartTitle
            },
            credits: false,
            legend: {
                enabled: true,
                layout: 'vertical',
                borderWidth: 0
            },
            xAxis: {
                labels: { enabled: false },
                tickLength: 0
            },
            yAxis: yAxis,
            tooltip: {
                shared: false,
                formatter: function () {
                    if (this.series.type === 'line') {
                        return '<b>' + this.series.name + '</b><br />'
                            + valueWithUnit.getFullLabel(regionValues[this.series.index]);
                    }
                    var data = xAxisCategories[this.x];
                    _thisLocal.hoverAreaCodeChanged.emit({ areaCode: data.AreaCode });
                    var s = '<b>' + data.AreaName + '</b>';
                    s += '<br>' + valueWithUnit.getFullLabel(data.ValF);
                    if (data.NoteId !== undefined) {
                        s += '<br><em>' + valueNotes[data.NoteId].Text + '</em>';
                    }
                    if (data.LoCIF !== undefined) {
                        s += '<br>LCI: ' + valueWithUnit.getFullLabel(data.LoCIF);
                    }
                    if (data.UpCIF !== undefined) {
                        s += '<br>UCI: ' + valueWithUnit.getFullLabel(data.UpCIF);
                    }
                    s += '<br>Rank: ' + (this.x + 1);
                    s += extraTooltip;
                    return s;
                }
            },
            plotOptions: {
                column: {
                    pointPadding: 0,
                    showInLegend: false,
                    animation: false,
                    borderWidth: 0,
                    groupPadding: 0,
                    pointWidth: 470 / this.chartData.length,
                    shadow: false,
                    states: {
                        hover: {
                            brightness: 0,
                            color: '#000000',
                        }
                    },
                    point: {
                        events: {
                            mouseOut: function (e) {
                                var areaCode = this.options.key;
                                _thisLocal.hoverAreaCodeChanged.emit({ areaCode: null });
                                unhighlightArea(chart, chartData, areaCode);
                            }
                        }
                    }
                },
                line: {
                    animation: false,
                    marker: {
                        enabled: false,
                    },
                    states: {
                        hover: {
                            lineWidth: 0,
                            marker: {
                                enabled: false,
                                symbol: 'x'
                            }
                        }
                    }
                },
                errorbar: {
                    animation: false
                }
            },
            exporting: {
                enabled: false,
                chartOptions: {
                    title: {
                        text: ''
                    }
                }
            },
            series: series
        };
        var chartContainer = null;
        if (this.chartEl && this.chartEl.nativeElement) {
            chartContainer = this.chartEl.nativeElement;
        }
        this.chart = new Highcharts.Chart(chartContainer, chartOptions);
        var chart = this.chart;
    };
    MapChartComponent.prototype.highlightArea = function (areaCode) {
        if (this.chart && this.chartData != null) {
            var data = _.where(this.chartData, { key: areaCode })[0];
            if (data !== undefined) {
                this.chart.series[0].data[data.x].select(true);
            }
        }
    };
    MapChartComponent.prototype.unhighlightArea = function (chart, chartData, areaCode) {
        if (chart && chartData != null) {
            var data = _.where(chartData, { key: areaCode })[0];
            if (data !== undefined) {
                chart.series[0].data[data['x']].select(false);
            }
        }
    };
    MapChartComponent.prototype.onExportClick = function (event) {
        var title = this.buildChartTitle();
        if (this.chart) {
            this.chart.exportChart({ type: 'image/png' }, {
                chart: {
                    spacingTop: 70,
                    height: 312,
                    width: 490,
                    events: {
                        load: function () {
                            this.renderer.text(title, 250, 15)
                                .attr({
                                align: 'center'
                            })
                                .css({
                                color: '#333',
                                fontSize: '10px',
                                width: '450px'
                            })
                                .add();
                        }
                    }
                }
            });
            this.ftHelperService.logEvent('ExportImage', 'MapBarChart');
        }
    };
    MapChartComponent.prototype.buildChartTitle = function () {
        var currentGrpRoot = this.ftHelperService.getCurrentGroupRoot();
        var data = this.ftHelperService.getMetadata(currentGrpRoot.IID);
        var unit = data.Unit;
        var unitLabel = (typeof unit.Label !== 'undefined') ? unit.Label : '';
        var period = currentGrpRoot.Grouping[0].Period;
        return data.ValueType.Name + ' - ' + unitLabel + ' ' + period;
    };
    return MapChartComponent;
}());
__decorate([
    core_1.ViewChild('mapChart'),
    __metadata("design:type", typeof (_a = typeof core_1.ElementRef !== "undefined" && core_1.ElementRef) === "function" && _a || Object)
], MapChartComponent.prototype, "chartEl", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], MapChartComponent.prototype, "sortedCoreData", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Number)
], MapChartComponent.prototype, "areaTypeId", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], MapChartComponent.prototype, "currentAreaCode", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], MapChartComponent.prototype, "areaCodeColour", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], MapChartComponent.prototype, "isBoundaryNotSupported", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], MapChartComponent.prototype, "selectedAreaCodeChanged", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], MapChartComponent.prototype, "hoverAreaCodeChanged", void 0);
MapChartComponent = __decorate([
    core_1.Component({
        selector: 'ft-map-chart',
        template: __webpack_require__("./src/app/map/map-chart/map-chart.component.html"),
        styles: [__webpack_require__("./src/app/map/map-chart/map-chart.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_b = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _b || Object, typeof (_c = typeof coreDataHelper_service_1.CoreDataHelperService !== "undefined" && coreDataHelper_service_1.CoreDataHelperService) === "function" && _c || Object])
], MapChartComponent);
exports.MapChartComponent = MapChartComponent;
var _a, _b, _c;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/map-chart.component.js.map

/***/ }),

/***/ "./src/app/map/map-legend/map-legend.component.css":
/***/ (function(module, exports) {

module.exports = ".asterisk {\r\n    font-size: 1.2em;\r\n    color: #333;\r\n    position: relative;\r\n    top: 4px;\r\n}\r\n\r\n.key-box {\r\n    display: block;\r\n    overflow: hidden;\r\n}\r\n\r\n#not-compared {\r\n    margin-left: 10px;\r\n}\r\n\r\n.key-container {\r\n    min-height: 25px;\r\n}\r\n\r\n.custom-key-table td {\r\n    width: 80px;\r\n    text-align: center;\r\n    padding: 2px;\r\n}\r\n\r\n.key-container {\r\n    float: left;\r\n    margin: 10px 0 20px 0;\r\n}\r\n\r\n#key-Quartiles,\r\n#key-Quintiles,\r\n#key-Continuous {\r\n    position: relative;\r\n}\r\n\r\n.fl {\r\n    float: left;\r\n}\r\n\r\n.key-table {\r\n    font-size: 11px;\r\n    color: #777;\r\n    float: left;\r\n}\r\n\r\n.key-label {\r\n    font-style: italic;\r\n    text-align: right;\r\n}\r\n\r\n.key-spacer {\r\n    width: 15px;\r\n}\r\n\r\ntd.better,\r\nspan.better {\r\n    background-color: #92d050;\r\n    color: #333;\r\n}\r\n\r\ntd.worse,\r\ntd.same,\r\ntd.better,\r\ntd.none,\r\n.bobLower,\r\n.bobHigher,\r\n.grade-quintile-1,\r\n.grade-quintile-2,\r\n.grade-quintile-3,\r\n.grade-quintile-4,\r\n.grade-quintile-5 {\r\n    margin: 0 0 0 0;\r\n    padding: 0 0 0 0;\r\n    text-align: center;\r\n    cursor: default;\r\n}\r\n\r\ntd.bobLower,\r\nspan.bobLower {\r\n    background-color: #5555e6;\r\n    color: #fff;\r\n}\r\n\r\ntd.bobHigher,\r\nspan.bobHigher,\r\ntd.overlapping-cis {\r\n    background-color: #bed2ff;\r\n}\r\n\r\ntd.grade-quintile-1,\r\nspan.grade-quintile-1 {\r\n    background-color: #ded3ec;\r\n    color: #000;\r\n}\r\n\r\ntd.grade-quintile-2,\r\nspan.grade-quintile-2 {\r\n    background-color: #bea7da;\r\n    color: #000;\r\n}\r\n\r\ntd.grade-quintile-3,\r\nspan.grade-quintile-3 {\r\n    background-color: #9e7cc8;\r\n    color: #fff;\r\n}\r\n\r\ntd.grade-quintile-4,\r\nspan.grade-quintile-4 {\r\n    background-color: #7e50b6;\r\n    color: #fff;\r\n}\r\n\r\ntd.grade-quintile-5,\r\nspan.grade-quintile-5 {\r\n    background-color: #5e25a4;\r\n    color: #fff;\r\n}\r\n\r\ntd.key-box-right {\r\n    border-right: 1px solid #eee;\r\n}\r\n\r\ntd.none {\r\n    background-color: #fff;\r\n}\r\n\r\n#value-note-legend {\r\n    float: right;\r\n    font-size: 11px;\r\n    color: #777;\r\n    text-align: right;\r\n    margin-top: 8px;\r\n}"

/***/ }),

/***/ "./src/app/map/map-legend/map-legend.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"row\">\r\n    <div class=\"key-container col-md-8\">\r\n        <table class=\"key-table\" cellspacing=\"0\" *ngIf=\"showRag()\">\r\n            <tr>\r\n                <td class=\"key-text key-label\">Compared with benchmark</td>\r\n                <td class=\"key-spacer\"></td>\r\n                <td class=\"better key\">\r\n                    <div class=\"tartan-box key-box\">\r\n                        <img class=\"tartan-fill print-only\" src=\"/images/better.png\" alt=\"\">\r\n                        <div class=\"tartan-text\">\r\n                            Better\r\n                        </div>\r\n                    </div>\r\n                </td>\r\n                <td class=\"same key\">\r\n                    <div class=\"tartan-box key-box\">\r\n                        <img class=\"tartan-fill print-only\" src=\"/images/same.png\" alt=\"\">\r\n                        <div class=\"tartan-text\">\r\n                            Similar\r\n                        </div>\r\n                    </div>\r\n                </td>\r\n                <td class=\"worse key\">\r\n                    <div class=\"tartan-box key-box\">\r\n                        <img class=\"tartan-fill print-only\" src=\"/images/red.png\" alt=\"\">\r\n                        <div class=\"tartan-text\">\r\n                            Worse\r\n                        </div>\r\n                    </div>\r\n                </td>\r\n            </tr>\r\n        </table>\r\n        <table class=\"key-table\" cellspacing=\"0\" *ngIf=\"showBob()\">\r\n            <tr>\r\n                <td class=\"key-text key-label\">Compared with benchmark</td>\r\n                <td class=\"key-spacer\"></td>\r\n                <td class=\"bobLower key\">\r\n                    <div class=\"tartan-box key-box\">\r\n                        <img class=\"tartan-fill print-only\" src=\"/images/bobLower.png\" alt=\"\">\r\n                        <div class=\"tartan-text\">\r\n                            Lower\r\n                        </div>\r\n                    </div>\r\n                </td>\r\n                <td class=\"same key\">\r\n                    <div class=\"tartan-box key-box\">\r\n                        <img class=\"tartan-fill print-only\" src=\"/images/same.png\" alt=\"\">\r\n                        <div class=\"tartan-text\">\r\n                            Similar\r\n                        </div>\r\n                    </div>\r\n                </td>\r\n                <td class=\"bobHigher key\">\r\n                    <div class=\"tartan-box key-box\">\r\n                        <img class=\"tartan-fill print-only\" src=\"/images/bobHigher.png\" alt=\"\">\r\n                        <div class=\"tartan-text\">\r\n                            Higher\r\n                        </div>\r\n                    </div>\r\n                </td>\r\n            </tr>\r\n        </table>\r\n        <table id=\"not-compared\" class=\"key-table\" cellspacing=\"0\" *ngIf=\"showBenchmark()\">\r\n            <tr>\r\n                <td id=\"map-key-part2\" class=\"none key key-box-right\" style=\"display: table-cell;\">\r\n                    <div class=\"tartan-box key-box\">\r\n                        Not compared\r\n                    </div>\r\n                </td>\r\n            </tr>\r\n        </table>\r\n\r\n        <table class=\"key-table custom-key-table\" cellspacing=\"2\" *ngIf=\"showQuartiles()\">\r\n            <tbody>\r\n                <tr>\r\n                    <td class=\"key-text\">Quartiles:</td>\r\n                    <td style=\"background-color: #E8C7D1;\">Low</td>\r\n                    <td style=\"background-color: #B74D6D;\" class=\"whiteText\">&nbsp;</td>\r\n                    <td style=\"background-color: #98002E;\" class=\"whiteText\">&nbsp;</td>\r\n                    <td style=\"background-color: #700023;\" class=\"whiteText\">High</td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n\r\n        <table class=\"key-table custom-key-table\" cellspacing=\"2\" *ngIf=\"showQuintiles()\">\r\n            <tr>\r\n                <td class=\"key-text\">Quintiles:</td>\r\n                <td style=\"background-color: #DED3EC;\">Low</td>\r\n                <td style=\"background-color: #BEA7DA;\">&nbsp;</td>\r\n                <td style=\"background-color: #9E7CC8;\" class=\"whiteText\">&nbsp;</td>\r\n                <td style=\"background-color: #7E50B6;\" class=\"whiteText\">&nbsp;</td>\r\n                <td style=\"background-color: #5E25A4;\" class=\"whiteText\">High</td>\r\n            </tr>\r\n        </table>\r\n\r\n        <table class=\"key-table custom-key-table\" cellspacing=\"0\" *ngIf=\"showContinuous()\">\r\n            <tr>\r\n                <td class=\"key-text\">Continuous:</td>\r\n                <td style=\"background-color: #FFE97F;\">Lowest</td>\r\n                <td class=\"continual_range whiteText\">&nbsp;</td>\r\n                <td style=\"background-color: #151C55;\" class=\"whiteText\">Highest</td>\r\n            </tr>\r\n        </table>\r\n    </div>\r\n    <div id=\"value-note-legend\" class=\"col-md-4 pull-right\">\r\n        <span class=\"asterisk\">*</span> a note is attached to the value, hover over to see more details\r\n    </div>\r\n</div>"

/***/ }),

/***/ "./src/app/map/map-legend/map-legend.component.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var MapLegendComponent = (function () {
    function MapLegendComponent() {
        this.legendDisplay = LegendDisplay.NoLegendRequired;
    }
    MapLegendComponent.prototype.showBenchmark = function () {
        return this.legendDisplay === LegendDisplay.RAG ||
            this.legendDisplay === LegendDisplay.BOB ||
            this.legendDisplay === LegendDisplay.NotCompared;
    };
    MapLegendComponent.prototype.showRag = function () {
        return this.legendDisplay === LegendDisplay.RAG;
    };
    MapLegendComponent.prototype.showBob = function () {
        return this.legendDisplay === LegendDisplay.BOB;
    };
    MapLegendComponent.prototype.showQuartiles = function () {
        return this.legendDisplay === LegendDisplay.Quartiles;
    };
    MapLegendComponent.prototype.showQuintiles = function () {
        return this.legendDisplay === LegendDisplay.Quintiles;
    };
    MapLegendComponent.prototype.showContinuous = function () {
        return this.legendDisplay === LegendDisplay.Continuous;
    };
    return MapLegendComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", Number)
], MapLegendComponent.prototype, "legendDisplay", void 0);
MapLegendComponent = __decorate([
    core_1.Component({
        selector: 'ft-map-legend',
        template: __webpack_require__("./src/app/map/map-legend/map-legend.component.html"),
        styles: [__webpack_require__("./src/app/map/map-legend/map-legend.component.css")]
    })
], MapLegendComponent);
exports.MapLegendComponent = MapLegendComponent;
var LegendDisplay = (function () {
    function LegendDisplay() {
    }
    return LegendDisplay;
}());
LegendDisplay.NoLegendRequired = 1;
LegendDisplay.RAG = 2;
LegendDisplay.BOB = 3;
LegendDisplay.NotCompared = 4;
LegendDisplay.Quintiles = 5;
LegendDisplay.Quartiles = 6;
LegendDisplay.Continuous = 7;
exports.LegendDisplay = LegendDisplay;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/map-legend.component.js.map

/***/ }),

/***/ "./src/app/map/map-table/map-table.component.css":
/***/ (function(module, exports) {

module.exports = "table {\r\n    width: 100%;\r\n    border: 1px solid #ccc;\r\n    padding: 0;\r\n    border-collapse: separate;\r\n    background-color: transparent;\r\n}\r\n\r\n\r\n/* Table header */\r\n\r\n\r\nth {\r\n    font-weight: normal;\r\n    text-align: right;\r\n    padding-right: 10px;\r\n    background-color: #EEEEEE;\r\n    cursor: pointer;\r\n}\r\n\r\n\r\ntr th:first-child {\r\n    text-align: left;\r\n    padding-left: 10px;\r\n}\r\n\r\n\r\n/* Table body */\r\n\r\n\r\ntd {\r\n    font-weight: normal;\r\n    color: #999;\r\n    padding-right: 10px;\r\n    text-align: right;\r\n}\r\n\r\n\r\ntd.value-note-symbol {\r\n    width: 100%;\r\n    text-align: center;\r\n    display: block;\r\n    cursor: default;\r\n}\r\n\r\n\r\ntr td:first-child {\r\n    padding-left: 5px;\r\n    border-left-width: 5px;\r\n    border-left-style: solid;\r\n    text-align: left;\r\n}\r\n\r\n\r\ntr.hover td:first-child {\r\n    border-left-style: solid;\r\n}\r\n\r\n\r\ntr.selected td {\r\n    background-color: aliceblue;\r\n    font-weight: bold;\r\n    font-style: italic;\r\n    cursor: pointer;\r\n    border: 1px solid gainsboro;\r\n}\r\n\r\n\r\n.rowHover {\r\n    background-color: lavender;\r\n    cursor: pointer;\r\n}"

/***/ }),

/***/ "./src/app/map/map-table/map-table.component.html":
/***/ (function(module, exports) {

module.exports = "<table *ngIf=\"selectedCoreData?.length > 0\" #maptable>\r\n    <thead>\r\n        <tr>\r\n            <th (click)=\"sortByAreaName()\" style=\"text-align:left\">Area</th>\r\n            <th (click)=\"sortByCount()\">Count</th>\r\n            <th (click)=\"sortByValue()\">Value</th>\r\n            <th>LCI</th>\r\n            <th>UCI</th>\r\n        </tr>\r\n    </thead>\r\n    <tbody>\r\n        <tr *ngFor=\"let item of selectedCoreData\" (mouseover)=\"rowMouseHovered = true;onRowMouseOver($event)\" (mouseout)=\"rowMouseHovered = false;onRowMouseLeave($event)\" [attr.areaCode]=\"item.areaCode\" [class.rowHover]=\"rowMouseHovered\" (click)=\"onRowClick($event)\">\r\n            <td [ngStyle]=\"{'border-left-color': item.colour}\">{{item.areaName}}</td>\r\n            <td>{{item.formattedCount}}</td>\r\n            <td [ngClass]=\"{'hasNote value-note-symbol': item.isNote}\" [attr.data-NoteId]=\"item.isNote?item.noteId:null\" (mouseenter)=\"onMouseEnter($event)\" (mousemove)=\"onMouseMove($event)\" (mouseleave)=\"onMouseLeave($event)\" innerHTML=\"{{item.formattedValue}}\">\r\n                <div style=\"display:inline\" *ngIf=\"item.isNote\">*</div>\r\n            </td>\r\n            <td>\r\n                <div innerHTML=\"{{item.loCI}}\"></div>\r\n            </td>\r\n            <td>\r\n                <div innerHTML=\"{{item.upCI}}\"></div>\r\n            </td>\r\n        </tr>\r\n    </tbody>\r\n</table>"

/***/ }),

/***/ "./src/app/map/map-table/map-table.component.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var _ = __webpack_require__("./node_modules/underscore/underscore.js");
var MapTableComponent = (function () {
    function MapTableComponent(ftHelperService, ref) {
        this.ftHelperService = ftHelperService;
        this.ref = ref;
        this.sortedCoreData = null;
        this.areaTypeId = null;
        this.areaCodeColour = null;
        this.hoverAreaCodeChanged = new core_1.EventEmitter();
        this.selectedAreaChanged = new core_1.EventEmitter();
        this.selectedCoreData = [];
        this.currentAreaList = [];
    }
    MapTableComponent.prototype.ngOnChanges = function (changes) {
        if (changes['isBoundaryNotSupported']) {
            var localBoundryNotSupported = changes['isBoundaryNotSupported'].currentValue;
            if (localBoundryNotSupported !== undefined) {
                if (localBoundryNotSupported) {
                    this.selectedCoreData = [];
                }
            }
        }
        if (changes['areaTypeId'] || changes['selectedAreaList'] || changes['areaCodeColour']) {
            this.loadData();
        }
    };
    MapTableComponent.prototype.onMouseEnter = function (event) {
        var noteId = event.srcElement.attributes.getNamedItem('data-NoteId');
        if (noteId) {
            var tooltipPrvdr = this.ftHelperService.newValueNoteTooltipProvider();
            var html = tooltipPrvdr.getHtmlFromNoteId(noteId.value);
            this.tooltip = this.ftHelperService.newTooltipManager();
            this.tooltip.setHtml(html);
            this.tooltip.positionXY(event.pageX + 10, event.pageY + 15);
            this.tooltip.showOnly();
        }
    };
    MapTableComponent.prototype.onMouseMove = function (event) {
        if (!_.isUndefined(this.tooltip)) {
            this.tooltip.positionXY(event.pageX + 10, event.pageY + 15);
        }
    };
    MapTableComponent.prototype.onMouseLeave = function (event) {
        if (!_.isUndefined(this.tooltip)) {
            this.tooltip.hide();
        }
    };
    MapTableComponent.prototype.onRowMouseOver = function (event) {
        var areaCode = this.getAreaCode(event);
        this.hoverAreaCodeChanged.emit({ areaCode: areaCode });
        var row = this.getRow(event.srcElement);
        this.clearRowHovers();
        $(row).addClass('rowHover');
    };
    MapTableComponent.prototype.onRowMouseLeave = function (event) {
        this.clearRowHovers();
    };
    MapTableComponent.prototype.onRowClick = function (event) {
        var areaCode = this.getAreaCode(event);
        if (areaCode !== null) {
            // Remove core data
            for (var i = 0; i < this.selectedCoreData.length; i++) {
                if (this.selectedCoreData[i].areaCode === areaCode) {
                    this.selectedCoreData.splice(i, 1);
                }
            }
            this.selectedCoreData = this.selectedCoreData.slice();
            this.selectedAreaChanged.emit({ areaCode: areaCode });
            this.ref.detectChanges();
            this.clearRowHovers();
        }
    };
    MapTableComponent.prototype.getRow = function (element) {
        while (element.tagName !== 'TR') {
            element = element.parentElement;
        }
        return element;
    };
    MapTableComponent.prototype.clearRowHovers = function () {
        $('.rowHover').removeClass('rowHover');
    };
    MapTableComponent.prototype.getAreaCode = function (event) {
        var areaCodeProp = 'areaCode';
        var areaCode = event.srcElement.attributes.getNamedItem(areaCodeProp);
        if (areaCode === null) {
            areaCode = event.srcElement.parentElement.attributes.getNamedItem(areaCodeProp);
            if (areaCode === null) {
                areaCode = event.srcElement.parentElement.parentElement.attributes.getNamedItem(areaCodeProp);
            }
        }
        return areaCode != null ? areaCode.value : null;
    };
    MapTableComponent.prototype.loadData = function () {
        var _this = this;
        var newData = [];
        this.selectedAreaList.forEach(function (areaCode) {
            var valueDisplayer = _this.ftHelperService.newValueDisplayer(null);
            var coreDataSet = _this.sortedCoreData[areaCode];
            var coreDatasetInfo = _this.ftHelperService.newCoreDataSetInfo(coreDataSet);
            // Set up data view model
            var areaName = _this.ftHelperService.getAreaName(areaCode);
            var isNote = coreDatasetInfo.isNote();
            var noteId = coreDatasetInfo.getNoteId();
            var formattedCount = coreDatasetInfo.isCount() ? _this.ftHelperService.newCommaNumber(coreDataSet.Count).rounded() : '-';
            var formattedValue = valueDisplayer.byNumberString(coreDataSet.ValF);
            var loCI = valueDisplayer.byNumberString(coreDataSet.LoCIF);
            var upCI = valueDisplayer.byNumberString(coreDataSet.UpCIF);
            var colour = _this.areaCodeColour ? _this.areaCodeColour.get(areaCode) : '#B0B0B2';
            newData.push({
                areaName: areaName, areaCode: areaCode,
                isNote: isNote,
                val: coreDataSet.Val,
                formattedValue: formattedValue,
                count: coreDataSet.Count,
                formattedCount: formattedCount,
                loCI: loCI, upCI: upCI,
                noteId: noteId, colour: colour
            });
        });
        this.selectedCoreData = newData;
    };
    MapTableComponent.prototype.sortByCount = function () {
        this.sort('count');
    };
    MapTableComponent.prototype.sortByValue = function () {
        this.sort('val');
    };
    MapTableComponent.prototype.sortByAreaName = function () {
        this.sort('areaName');
    };
    MapTableComponent.prototype.sort = function (prop) {
        this.selectedCoreData = _.sortBy(this.selectedCoreData, prop);
        this.ref.detectChanges();
    };
    MapTableComponent.prototype.loadAreaList = function () {
        this.currentAreaList = [];
        this.currentAreaList = this.ftHelperService.getAreaList();
    };
    return MapTableComponent;
}());
__decorate([
    core_1.ViewChild('maptable'),
    __metadata("design:type", typeof (_a = typeof core_1.ElementRef !== "undefined" && core_1.ElementRef) === "function" && _a || Object)
], MapTableComponent.prototype, "el", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], MapTableComponent.prototype, "sortedCoreData", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Number)
], MapTableComponent.prototype, "areaTypeId", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], MapTableComponent.prototype, "selectedAreaList", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], MapTableComponent.prototype, "areaCodeColour", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], MapTableComponent.prototype, "isBoundaryNotSupported", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], MapTableComponent.prototype, "hoverAreaCodeChanged", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], MapTableComponent.prototype, "selectedAreaChanged", void 0);
MapTableComponent = __decorate([
    core_1.Component({
        selector: 'ft-map-table',
        template: __webpack_require__("./src/app/map/map-table/map-table.component.html"),
        styles: [__webpack_require__("./src/app/map/map-table/map-table.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_b = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _b || Object, typeof (_c = typeof core_1.ChangeDetectorRef !== "undefined" && core_1.ChangeDetectorRef) === "function" && _c || Object])
], MapTableComponent);
exports.MapTableComponent = MapTableComponent;
var _a, _b, _c;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/map-table.component.js.map

/***/ }),

/***/ "./src/app/map/map.component.css":
/***/ (function(module, exports) {

module.exports = "#maps_info {\r\n    margin-left: 10px;\r\n    margin-bottom: 10px;\r\n    width: 490px;\r\n    height: 350px;\r\n    float: left;\r\n    overflow: auto;\r\n}\r\n\r\n#key-ad-hoc {\r\n    position: relative;\r\n    height: 50px;\r\n}\r\n\r\n#map_colour_box {\r\n    border: 1px solid #CCC;\r\n    background-color: #EEE;\r\n    padding: 5px 10px;\r\n    margin-top: 10px;\r\n    margin-bottom: 10px;\r\n}"

/***/ }),

/***/ "./src/app/map/map.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"map-container\" class=\"standard-width clearfix\" [style.display]=\"searchMode ? 'block' : 'none'\">\r\n    <div [hidden]=\"isBoundaryNotSupported\">\r\n        <div *ngIf=\"isInitialised\">\r\n            <div *ngIf=\"!IsPracticeAreaType\">\r\n                <div id=\"key-ad-hoc\" class=\"key-container\" *ngIf=\"showAdhocKey\" [innerHtml]=\"htmlAdhocKey\" style=\"display: block;overflow: hidden;\"></div>\r\n                <ft-map-legend [legendDisplay]=\"legendDisplay\"></ft-map-legend>\r\n                <br>\r\n                <ft-google-map (mapInit)=\"onMapInit($event)\" [areaTypeId]=\"areaTypeId\" [sortedCoreData]=\"sortedCoreData\" [currentAreaCode]=\"currentAreaCode\" [areaCodeColour]=\"areaCodeColour\" [refreshColour]=\"refreshColour\" (hoverAreaCodeChanged)=\"onhoverAreaCodeChangedMap($event)\"\r\n                    (selectedAreaChanged)=\"onSelectedAreaChanged($event)\" [isBoundaryNotSupported]=\"isBoundaryNotSupported\" [selectedAreaList]=\"selectedAreaList\">\r\n                </ft-google-map>\r\n                <div id=\"maps_info\">\r\n                    <div id='map_colour_box'>Map colour&nbsp;\r\n                        <select id='map_colour' (change)=\"onMapColourBoxChange($event.target.value)\" [(ngModel)]=\"mapColourSelectedValue\">\r\n                            <option value='benchmark'>Comparison to benchmark</option>\r\n                            <option value='quartile'>Quartiles</option>\r\n                            <option value='quintile'>Quintiles</option>\r\n                            <option value='continuous'>Continuous</option>\r\n                        </select>\r\n                    </div>\r\n                    <ft-map-table [areaTypeId]=\"areaTypeId\" [sortedCoreData]=\"sortedCoreData\" [selectedAreaList]=\"selectedAreaList\" [areaCodeColour]=\"areaCodeColour\" [isBoundaryNotSupported]=\"isBoundaryNotSupported\" (hoverAreaCodeChanged)=\"onhoverAreaCodeChangedData($event)\"\r\n                        (selectedAreaChanged)=\"onSelectedAreaChanged($event)\"></ft-map-table>\r\n                </div>\r\n                <ft-map-chart [areaTypeId]=\"areaTypeId\" [sortedCoreData]=\"sortedCoreData\" [currentAreaCode]=\"currentAreaCode\" [isBoundaryNotSupported]=\"isBoundaryNotSupported\" [areaCodeColour]=\"areaCodeColour\" (selectedAreaCodeChanged)=\"onSelectedAreaCodeChanged($event)\"\r\n                    (hoverAreaCodeChanged)=\"onhoverAreaCodeChangedChart($event)\"></ft-map-chart>\r\n            </div>\r\n            <div *ngIf=\"IsPracticeAreaType\">\r\n                <ft-practice-search [IsMapUpdateRequired]=\"updateMap\" [searchMode]=\"searchMode\" #practiceSearch></ft-practice-search>\r\n            </div>\r\n        </div>\r\n    </div>\r\n    <div [hidden]=\"!isBoundaryNotSupported\" style=\"width:100%; height:400px; padding-top:190px; text-align:center\" id=\"boundryNotSupported\">\r\n        Maps are not available for this area type\r\n    </div>\r\n</div>"

/***/ }),

/***/ "./src/app/map/map.component.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var indicator_service_1 = __webpack_require__("./src/app/shared/service/api/indicator.service.ts");
var coreDataHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/coreDataHelper.service.ts");
var area_service_1 = __webpack_require__("./src/app/shared/service/api/area.service.ts");
var map_legend_component_1 = __webpack_require__("./src/app/map/map-legend/map-legend.component.ts");
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var MapComponent = (function () {
    function MapComponent(ftHelperService, indicatorService, coreDataHelper, ref, areaService) {
        this.ftHelperService = ftHelperService;
        this.indicatorService = indicatorService;
        this.coreDataHelper = coreDataHelper;
        this.ref = ref;
        this.areaService = areaService;
        this.isInitialised = false;
        this.areaTypeId = null;
        // Legend display flags
        this.legendDisplay = map_legend_component_1.LegendDisplay.NoLegendRequired;
        this.showAdhocKey = false;
        this.selectedAreaList = new Array();
        this.areaCodeSignificance = new Map();
        this.nationalArea = new Map();
        this.areaCodeColour = new Map();
        this.areaCodeColourValue = new Map();
        this.refreshColour = 0;
        this.htmlAdhocKey = "";
        this.IsPracticeAreaType = false;
        this.isBoundaryNotSupported = false;
        this.mapColourSelectedValue = "benchmark";
        this.profileId = this.ftHelperService.getFTModel().profileId;
    }
    MapComponent.prototype.onOutsideEvent = function (event, searchMode) {
        var _this = this;
        // Determine new area type and whether search mode
        this.searchModeNoDisplay = false;
        this.searchMode = false;
        var newAreaTypeId;
        if (searchMode !== undefined && searchMode) {
            // In search mode
            if (this.profileId === shared_1.ProfileIds.PracticeProfile) {
                // Practice profiles search
                this.searchMode = true;
            }
            else {
                //HOW can get here???
                this.searchModeNoDisplay = true;
            }
            newAreaTypeId = shared_1.AreaTypeIds.Practice;
        }
        else {
            // Not in search mode
            newAreaTypeId = this.ftHelperService.getFTModel().areaTypeId;
        }
        this.areaService.getParentAreas(this.profileId).subscribe(function (result) {
            // Determine whether current area type can be shown on map
            var parentAreaTypes = result;
            if (parentAreaTypes != null) {
                for (var _i = 0, parentAreaTypes_1 = parentAreaTypes; _i < parentAreaTypes_1.length; _i++) {
                    var areaType = parentAreaTypes_1[_i];
                    if (areaType.Id === newAreaTypeId) {
                        _this.isBoundaryNotSupported = !areaType.CanBeDisplayedOnMap;
                        break;
                    }
                }
            }
            _this.initMap(newAreaTypeId);
        });
    };
    MapComponent.prototype.initMap = function (newAreaTypeId) {
        this.isInitialised = true;
        this.updateMap = true;
        this.ref.detectChanges();
        // Clear area list of area type changes
        if (this.areaTypeId !== newAreaTypeId) {
            this.selectedAreaList = new Array();
        }
        this.areaTypeId = newAreaTypeId;
        this.IsPracticeAreaType = newAreaTypeId === shared_1.AreaTypeIds.Practice;
        if (!this.isBoundaryNotSupported) {
            // Have boundaries so can load data
            this.loadData();
        }
        else {
            // No boundaries for current area type
            this.ftHelperService.showAndHidePageElements();
            this.ftHelperService.unlock();
            this.ref.detectChanges();
        }
        this.updateMap = false;
    };
    MapComponent.prototype.loadData = function () {
        var _this = this;
        if (this.IsPracticeAreaType) {
            // Don't need to display any data for practices
            if (!this.searchMode && !this.searchModeNoDisplay) {
                this.showElementsAndUnlock();
            }
        }
        else {
            // Get data for all other area types
            var currentGrpRoot = this.ftHelperService.getCurrentGroupRoot();
            var currentComparator = this.ftHelperService.getCurrentComparator();
            var ftModel = this.ftHelperService.getFTModel();
            var comparatorId = this.ftHelperService.getComparatorId();
            this.indicatorService
                .getSingleIndicatorForAllArea(currentGrpRoot.Grouping[0].GroupId, ftModel.areaTypeId, currentComparator.Code, ftModel.profileId, comparatorId, currentGrpRoot.IID, currentGrpRoot.Sex.Id, currentGrpRoot.Age.Id)
                .subscribe(function (data) {
                _this.coreDataSet = data;
                _this.sortedCoreData = _this.coreDataHelper.addOrderandPercentilesToData(_this.coreDataSet);
                _this.loadColourData();
                _this.onMapColourBoxChange(_this.mapColourSelectedValue);
                _this.ref.detectChanges();
                _this.showElementsAndUnlock();
            }, function (error) { });
        }
    };
    MapComponent.prototype.showElementsAndUnlock = function () {
        this.ftHelperService.showAndHidePageElements();
        this.ftHelperService.showTargetBenchmarkOption(this.ftHelperService.getAllGroupRoots());
        this.ftHelperService.unlock();
    };
    MapComponent.prototype.loadColourData = function () {
        var _this = this;
        var areaOrder = [];
        Object.keys(this.sortedCoreData).forEach(function (key) {
            var value = _this.sortedCoreData[key];
            areaOrder.push({ AreaCode: key, Val: value.Val, ValF: value.ValF });
        });
        areaOrder
            .sort(function (leftside, righside) {
            if (leftside.Val < righside.Val) {
                return -1;
            }
            if (leftside.Val > righside.Val) {
                return 1;
            }
            return 0;
        })
            .reverse();
        var numAreas = 0;
        $.each(areaOrder, function (i, coreData) {
            if (coreData.ValF !== "-") {
                numAreas++;
            }
        });
        var j = 0;
        var sortedCoreData = this.sortedCoreData;
        var localAreaCodeColourValue = new Map();
        $.each(areaOrder, function (i, coreData) {
            var data = sortedCoreData[coreData.AreaCode];
            if (coreData.ValF === "-") {
                var colourData = new MapColourData();
                colourData.order = -1;
                colourData.orderFrac = -1;
                localAreaCodeColourValue.set(coreData.AreaCode, colourData);
            }
            else {
                var colourData = new MapColourData();
                colourData.order = numAreas - j;
                colourData.orderFrac = 1 - j / numAreas;
                var position = numAreas + 1 - j + 1;
                colourData.quartile = Math.ceil(position / (numAreas / 4));
                colourData.quintile = Math.ceil(position / (numAreas / 5));
                j++;
                localAreaCodeColourValue.set(coreData.AreaCode, colourData);
            }
        });
        this.areaCodeColourValue = localAreaCodeColourValue;
    };
    MapComponent.prototype.showHideAdHocKey = function () {
        var currentGrpRoot = this.ftHelperService.getCurrentGroupRoot();
        var indicatorMetadata = this.ftHelperService.getMetadata(currentGrpRoot.IID);
        this.comparisonConfig = this.ftHelperService.newComparisonConfig(currentGrpRoot, indicatorMetadata);
        if (this.comparisonConfig) {
            if (this.comparisonConfig.useTarget) {
                this.legendDisplay = map_legend_component_1.LegendDisplay.NoLegendRequired;
                var targetLegend = this.ftHelperService.getTargetLegendHtml(this.comparisonConfig, indicatorMetadata);
                this.htmlAdhocKey =
                    '<div><table class="key-table" style="width: 85%;height:50px;"><tr><td class="key-text">Benchmarked against goal:</td><td class="key-spacer"></td><td>' +
                        targetLegend +
                        "</td></tr></table></div>";
                this.showAdhocKey = true;
            }
            else {
                this.showAdhocKey = false;
                this.setBenchmarkLegendDisplay(currentGrpRoot);
            }
        }
    };
    MapComponent.prototype.onMapInit = function (mapInfo) {
        this.map = mapInfo.map;
    };
    MapComponent.prototype.onhoverAreaCodeChangedMap = function (eventDetail) {
        this.currentAreaCode = eventDetail.areaCode;
        this.ref.detectChanges();
    };
    MapComponent.prototype.onSelectedAreaCodeChanged = function (eventDetail) { };
    MapComponent.prototype.onhoverAreaCodeChangedChart = function (eventDetail) {
        this.currentAreaCode = eventDetail.areaCode;
        this.ref.detectChanges();
    };
    MapComponent.prototype.onhoverAreaCodeChangedData = function (eventDetail) {
        this.currentAreaCode = eventDetail.areaCode;
        this.ref.detectChanges();
    };
    MapComponent.prototype.onBoundaryNotSupported = function (eventDetail) {
        this.isBoundaryNotSupported = true;
        this.ref.detectChanges();
    };
    MapComponent.prototype.onSelectedAreaChanged = function (eventDetail) {
        var index = this.selectedAreaList.indexOf(eventDetail.areaCode);
        if (index > -1) {
            this.selectedAreaList.splice(index, 1);
        }
        else {
            this.selectedAreaList.push(eventDetail.areaCode);
        }
        this.selectedAreaList = this.selectedAreaList.slice();
        this.ref.detectChanges();
    };
    MapComponent.prototype.onMapColourBoxChange = function (selectedColour) {
        switch (selectedColour) {
            case "quartile": {
                this.legendDisplay = map_legend_component_1.LegendDisplay.Quartiles;
                this.showAdhocKey = false;
                this.getQuartileColorData();
                break;
            }
            case "quintile": {
                this.legendDisplay = map_legend_component_1.LegendDisplay.Quintiles;
                this.showAdhocKey = false;
                this.getQuintileColorData();
                break;
            }
            case "continuous": {
                this.legendDisplay = map_legend_component_1.LegendDisplay.Continuous;
                this.showAdhocKey = false;
                this.getContinuousColorData();
                break;
            }
            case "benchmark": {
                var root = this.ftHelperService.getCurrentGroupRoot();
                this.setBenchmarkLegendDisplay(root);
                this.showHideAdHocKey();
                this.getBenchMarkColorData();
                break;
            }
            default: {
                break;
            }
        }
        this.refreshColour++;
        this.ref.detectChanges();
    };
    MapComponent.prototype.setBenchmarkLegendDisplay = function (root) {
        if (root.ComparatorMethodId === shared_1.ComparatorMethodIds.Quintiles) {
            this.legendDisplay = map_legend_component_1.LegendDisplay.Quintiles;
        }
        else {
            this.setLegendDisplayByPolarity(root.PolarityId);
        }
    };
    MapComponent.prototype.setLegendDisplayByPolarity = function (polarityId) {
        switch (polarityId) {
            case shared_1.PolarityIds.BlueOrangeBlue:
                this.legendDisplay = map_legend_component_1.LegendDisplay.BOB;
                break;
            case shared_1.PolarityIds.RAGHighIsGood:
                this.legendDisplay = map_legend_component_1.LegendDisplay.RAG;
                break;
            case shared_1.PolarityIds.RAGLowIsGood:
                this.legendDisplay = map_legend_component_1.LegendDisplay.RAG;
                break;
            default:
                this.legendDisplay = map_legend_component_1.LegendDisplay.NotCompared;
        }
    };
    MapComponent.prototype.getBenchMarkColorData = function () {
        var _this = this;
        var currentGrpRoot = this.ftHelperService.getCurrentGroupRoot();
        var indicatorMetadata = this.ftHelperService.getMetadata(currentGrpRoot.IID);
        this.comparisonConfig = this.ftHelperService.newComparisonConfig(currentGrpRoot, indicatorMetadata);
        var globalComparatorId = this.ftHelperService.getComparatorId();
        this.areaCodeSignificance = new Map();
        Object.keys(this.sortedCoreData).forEach(function (key) {
            var value = _this.sortedCoreData[key];
            if (_this.comparisonConfig !== undefined) {
                _this.areaCodeSignificance[key] =
                    value.Sig[_this.comparisonConfig.comparatorId];
            }
            else {
                _this.areaCodeSignificance[key] = value.Sig[globalComparatorId];
            }
        });
        var localAreaCodeColour = new Map();
        Object.keys(this.areaCodeSignificance).forEach(function (key) {
            var value = _this.areaCodeSignificance[key];
            var colour;
            var selectedGroupRoot;
            // If the use target is clicked change the
            // polarity of currentGroup to target polarity
            if (_this.comparisonConfig.useTarget) {
                selectedGroupRoot = Object.create(currentGrpRoot);
                selectedGroupRoot.PolarityId = indicatorMetadata.Target.PolarityId;
            }
            else {
                selectedGroupRoot = currentGrpRoot;
            }
            colour = shared_1.Colour.getSignificanceColorForBenchmark(selectedGroupRoot, _this.comparisonConfig, value);
            localAreaCodeColour.set(key, colour);
        });
        this.areaCodeColour = localAreaCodeColour;
    };
    MapComponent.prototype.getContinuousColorData = function () {
        var localAreaCodeColour = new Map();
        for (var _i = 0, _a = Array.from(this.areaCodeColourValue.keys()); _i < _a.length; _i++) {
            var key = _a[_i];
            var value = this.areaCodeColourValue.get(key);
            var colour = shared_1.Colour.getColorForContinuous(value.orderFrac);
            localAreaCodeColour.set(key, colour);
        }
        this.areaCodeColour = localAreaCodeColour;
    };
    MapComponent.prototype.getQuintileColorData = function () {
        var localAreaCodeColour = new Map();
        for (var _i = 0, _a = Array.from(this.areaCodeColourValue.keys()); _i < _a.length; _i++) {
            var key = _a[_i];
            var value = this.areaCodeColourValue.get(key);
            var colour = shared_1.Colour.getColorForQuintile(value.quintile);
            localAreaCodeColour.set(key, colour);
        }
        this.areaCodeColour = localAreaCodeColour;
    };
    MapComponent.prototype.getQuartileColorData = function () {
        var localAreaCodeColour = new Map();
        for (var _i = 0, _a = Array.from(this.areaCodeColourValue.keys()); _i < _a.length; _i++) {
            var key = _a[_i];
            var value = this.areaCodeColourValue.get(key);
            var colour = shared_1.Colour.getColorForQuartile(value.quartile);
            localAreaCodeColour.set(key, colour);
        }
        this.areaCodeColour = localAreaCodeColour;
    };
    return MapComponent;
}());
__decorate([
    core_1.HostListener("window:MapEnvReady-Event", [
        "$event",
        "$event.detail.searchMode"
    ]),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [Object, Object]),
    __metadata("design:returntype", void 0)
], MapComponent.prototype, "onOutsideEvent", null);
MapComponent = __decorate([
    core_1.Component({
        selector: "ft-map",
        template: __webpack_require__("./src/app/map/map.component.html"),
        styles: [__webpack_require__("./src/app/map/map.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _a || Object, typeof (_b = typeof indicator_service_1.IndicatorService !== "undefined" && indicator_service_1.IndicatorService) === "function" && _b || Object, typeof (_c = typeof coreDataHelper_service_1.CoreDataHelperService !== "undefined" && coreDataHelper_service_1.CoreDataHelperService) === "function" && _c || Object, typeof (_d = typeof core_1.ChangeDetectorRef !== "undefined" && core_1.ChangeDetectorRef) === "function" && _d || Object, typeof (_e = typeof area_service_1.AreaService !== "undefined" && area_service_1.AreaService) === "function" && _e || Object])
], MapComponent);
exports.MapComponent = MapComponent;
var MapColourData = (function () {
    function MapColourData() {
    }
    return MapColourData;
}());
var _a, _b, _c, _d, _e;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/map.component.js.map

/***/ }),

/***/ "./src/app/map/map.module.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var common_1 = __webpack_require__("./node_modules/@angular/common/@angular/common.es5.js");
var google_map_component_1 = __webpack_require__("./src/app/map/google-map/google-map.component.ts");
var map_component_1 = __webpack_require__("./src/app/map/map.component.ts");
var googleMap_service_1 = __webpack_require__("./src/app/map/googleMap.service.ts");
var indicator_service_1 = __webpack_require__("./src/app/shared/service/api/indicator.service.ts");
var area_service_1 = __webpack_require__("./src/app/shared/service/api/area.service.ts");
var map_chart_component_1 = __webpack_require__("./src/app/map/map-chart/map-chart.component.ts");
var script_service_1 = __webpack_require__("./src/app/shared/service/helper/script.service.ts");
var map_table_component_1 = __webpack_require__("./src/app/map/map-table/map-table.component.ts");
var practice_search_component_1 = __webpack_require__("./src/app/map/practice-search/practice-search.component.ts");
var ngx_bootstrap_1 = __webpack_require__("./node_modules/ngx-bootstrap/index.js");
var forms_1 = __webpack_require__("./node_modules/@angular/forms/@angular/forms.es5.js");
var coreDataHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/coreDataHelper.service.ts");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var map_legend_component_1 = __webpack_require__("./src/app/map/map-legend/map-legend.component.ts");
var MapModule = (function () {
    function MapModule() {
    }
    return MapModule;
}());
MapModule = __decorate([
    core_1.NgModule({
        imports: [
            common_1.CommonModule,
            ngx_bootstrap_1.TypeaheadModule.forRoot(),
            forms_1.FormsModule
        ],
        declarations: [
            google_map_component_1.GoogleMapComponent,
            map_component_1.MapComponent,
            map_chart_component_1.MapChartComponent,
            map_table_component_1.MapTableComponent,
            practice_search_component_1.PracticeSearchComponent,
            map_legend_component_1.MapLegendComponent
        ],
        exports: [
            google_map_component_1.GoogleMapComponent,
            map_component_1.MapComponent
        ],
        providers: [
            googleMap_service_1.GoogleMapService,
            indicator_service_1.IndicatorService,
            script_service_1.ScriptLoaderService,
            area_service_1.AreaService,
            coreDataHelper_service_1.CoreDataHelperService,
            ftHelper_service_1.FTHelperService
        ]
    })
], MapModule);
exports.MapModule = MapModule;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/map.module.js.map

/***/ }),

/***/ "./src/app/map/practice-search/practice-search.component.css":
/***/ (function(module, exports) {

module.exports = ".glyphicon {\r\n    display: none;\r\n}\r\n\r\n.googleMapNg {\r\n    position: relative;\r\n    background-color: #fff;\r\n    border: 1px solid #CCC;\r\n    width: 550px;\r\n    height: 600px;\r\n    margin-bottom: 10px;\r\n    margin-top: 10px;\r\n    clear: both;\r\n    overflow: hidden;\r\n}\r\n\r\n#default_search_header {\r\n    width: 400px;\r\n    padding-top: 10px;\r\n    position: absolute;\r\n}\r\n\r\n#practice-list-info {\r\n    min-height: 30px;\r\n}\r\n\r\n#practice-list-info input {\r\n    max-width: 450px;\r\n}\r\n\r\n#practice-list-info h3 {\r\n    font-size: 1.4em;\r\n    margin-top: 10px;\r\n}\r\n\r\n.practice-header {\r\n    cursor: pointer;\r\n}\r\n\r\n.nearby-practices-map {\r\n    float: right;\r\n    width: 550px;\r\n    height: 0px;\r\n    margin-top: -10px;\r\n    overflow: hidden;\r\n}\r\n\r\n.nearby-practices-map #google-map {\r\n    margin-bottom: 10px;\r\n}\r\n\r\n#table-container {\r\n    height: 600px;\r\n    overflow-y: scroll;\r\n}\r\n\r\n.nearby-practices-table {\r\n    float: left;\r\n    width: 45%;\r\n}\r\n\r\n.nearby-practices-table table {\r\n    width: 100%;\r\n    border-collapse: collapse;\r\n}\r\n\r\n.nearby-practices-table .header {\r\n    position: relative;\r\n    background-color: #02ae94;\r\n    padding: 10px;\r\n    color: #fff;\r\n}\r\n\r\n.nearby-practices-table td,\r\n.nearby-practices-table th {\r\n    padding: 0;\r\n    font-weight: normal;\r\n    text-align: left;\r\n    vertical-align: top;\r\n    font-size: 16px;\r\n}\r\n\r\n#show-all-practices {\r\n    padding: 23px 0 18px 0;\r\n}\r\n\r\n#show-all-practices a,\r\na:visited,\r\na:active,\r\na:hover {\r\n    color: #1d1dae;\r\n}\r\n\r\n#show-all-practices a,\r\na:visited,\r\na:active,\r\na:hover {\r\n    color: #2e3191;\r\n    text-decoration: underline;\r\n    font-size: 16px;\r\n}\r\n\r\n#show-all-practices a {\r\n    cursor: pointer;\r\n}\r\n\r\n.highlight-address {\r\n    background-color: #e6e6e6\r\n}\r\n\r\n.highlight-header {\r\n    background-color: #525252;\r\n    position: relative;\r\n    padding: 10px;\r\n    color: #fff;\r\n}"

/***/ }),

/***/ "./src/app/map/practice-search/practice-search.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"container\" style=\"margin-left:-15px;\">\r\n    <div class=\"row\">\r\n        <div *ngIf=\"searchMode\" class=\"col-md-8\">\r\n            <h2>Find your practice</h2>\r\n        </div>\r\n        <div id=\"practice-list-info\" class=\"col-md-6\">\r\n            <h3>Search for a practice by postcode or place name:</h3>\r\n            <input [(ngModel)]=\"placeNameText\" [typeahead]=\"dataSource\" (typeaheadLoading)=\"changeTypeaheadLoading($event)\" (typeaheadNoResults)=\"changeTypeaheadNoResults($event)\" (typeaheadOnSelect)=\"typeaheadOnSelect($event)\" typeaheadOptionsLimit=\"7\" typeaheadOptionField=\"displayName\"\r\n                placeholder=\"\" [typeaheadWaitMs]=\"200\" [typeaheadMinLength]=\"3\" class=\"form-control\" id=\"gp-practice-search-text\">\r\n        </div>\r\n        <div class=\"col-md-6\">\r\n            <div id=\"show-all-practices\" [hidden]=\"!displayCCGPracticeLink\">\r\n                <a id=\"all_ccg_practices\" (click)=\"onShowAllPracticeinCCGClick()\" title=\"Display all practices within CCG on the map\">\r\n                            Show all practices in CCG</a>\r\n            </div>\r\n            <div *ngIf=\"typeaheadNoResults===true\" class=\"\">\r\n                No Results Found\r\n            </div>\r\n        </div>\r\n    </div>\r\n    <div id=\"practice-list-info\" class=\"row col-md-12\">\r\n        <h3 *ngIf=\"nearByPractices?.length > 0 \" id=\"practice-count-info\">{{practiceCountText}}</h3>\r\n    </div>\r\n</div>\r\n<div class=\"nearby-practices-table\" *ngIf=\"nearByPractices?.length >0\">\r\n    <div class=\"table-data\" id=\"mortality-rankings\">\r\n        <div id=\"table-container\" #scrollPracticeTable>\r\n            <table id=\"search-table\">\r\n                <tbody>\r\n                    <tr *ngFor=\"let item of nearByPractices\" id=\"practice-header\">\r\n                        <table>\r\n                            <tr [attr.id]=\"item.areaCode\">\r\n                                <td colspan=\"2\">\r\n                                    <div class=\"practice-header clearfix\" [ngClass]=\"{'highlight-header': item.selected,'header': item.selected==false}\" (click)=\"onSelectPracticeClick(item.areaCode)\">\r\n                                        {{item.areaCode}} - {{item.areaName}}\r\n                                    </div>\r\n                                </td>\r\n                            </tr>\r\n                            <tr [ngClass]=\"{'highlight-address': item.selected}\">\r\n                                <td style=\"width:70%\">\r\n                                    <div>{{item.addressLine1}}</div>\r\n                                    <div>{{item.addressLine2}}</div>\r\n                                    <div>{{item.postcode}}</div>\r\n                                </td>\r\n                                <td style=\"width:30%\">\r\n                                    <div *ngIf=\"!showCcgPractices\" class=\"center-text\">{{item.distanceValF}} miles</div>\r\n                                    <div>\r\n                                        <a (click)=\"onSelectPracticeClick(item.areaCode)\" style=\"color: #2e3191;text-decoration: underline;\" id=\"select-practice\">\r\n                                         Select\r\n                                       </a>\r\n                                    </div>\r\n                                </td>\r\n                            </tr>\r\n                        </table>\r\n                    </tr>\r\n                </tbody>\r\n            </table>\r\n        </div>\r\n    </div>\r\n</div>\r\n<div [style.height]=\"height + 'px'\" class=\"nearby-practices-map\">\r\n    <div id=\"google-map\" #googleMapNew class=\"googleMapNg\">\r\n    </div>\r\n</div>"

/***/ }),

/***/ "./src/app/map/practice-search/practice-search.component.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var area_service_1 = __webpack_require__("./src/app/shared/service/api/area.service.ts");
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var Observable_1 = __webpack_require__("./node_modules/rxjs/_esm5/Observable.js");
var practice_search_1 = __webpack_require__("./src/app/map/practice-search/practice-search.ts");
var _ = __webpack_require__("./node_modules/underscore/underscore.js");
var PracticeSearchComponent = (function () {
    function PracticeSearchComponent(ftHelperService, areaService, ref) {
        var _this = this;
        this.ftHelperService = ftHelperService;
        this.areaService = areaService;
        this.ref = ref;
        this.IsMapUpdateRequired = false;
        this.searchMode = false;
        this.height = 0;
        this.isVisible = false;
        this.searchResults = [];
        this.nearByPractices = [];
        this.showCcgPractices = false;
        this.localSearchMode = false;
        this.displayCCGPracticeLink = true;
        this.dataSource = Observable_1.Observable.create(function (observer) {
            _this.areaService.getAreaSearchByText(_this.placeNameText, shared_1.AreaTypeIds.CcgPreApr2017, true, true)
                .subscribe(function (result) {
                _this.searchResults = result;
                var newResult = _.map(_this.searchResults, function (result) {
                    return new practice_search_1.autoCompleteResult(result.PolygonAreaCode, result.PlaceName, result.PolygonAreaName);
                });
                observer.next(newResult);
            });
        });
    }
    ;
    PracticeSearchComponent.prototype.ngOnChanges = function (changes) {
        if (this.IsMapUpdateRequired && this.showCcgPractices) {
            this.onShowAllPracticeinCCGClick();
        }
        if (changes['searchMode']) {
            var displyCCGLink = changes['searchMode'].currentValue;
            if (displyCCGLink !== undefined) {
                if (displyCCGLink) {
                    this.localSearchMode = true;
                    this.displayCCGPracticeLink = false;
                }
            }
            else {
                this.localSearchMode = false;
                this.displayCCGPracticeLink = true;
            }
        }
    };
    PracticeSearchComponent.prototype.ngOnInit = function () {
        this.isVisible = this.ftHelperService.getFTModel().areaTypeId === shared_1.AreaTypeIds.Practice;
        if (!this.practiceMap) {
            this.loadMap();
        }
    };
    PracticeSearchComponent.prototype.loadMap = function () {
        var mapOptions = {
            zoom: 6,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            panControl: false,
            zoomControl: true,
            zoomControlOptions: { position: google.maps.ControlPosition.TOP_LEFT },
            scaleControl: false,
            streetViewControl: false,
            mapTypeControl: false,
            fullscreenControl: false,
        };
        var mapContainer = null;
        if (this.mapEl && this.mapEl.nativeElement) {
            mapContainer = this.mapEl.nativeElement;
        }
        if (mapContainer != null) {
            this.practiceMap = new google.maps.Map(mapContainer, mapOptions);
        }
        if (this.practiceMap) {
            var bounds = new google.maps.LatLngBounds();
            var position = new google.maps.LatLng(53.415649, -2.209015);
            bounds.extend(position);
            this.practiceMap.setCenter(bounds.getCenter());
            var thisCom_1 = this;
            //This is needed as on initial load map was not visible
            google.maps.event.addListener(this.practiceMap, 'idle', function () {
                var bound = thisCom_1.practiceMap.getCenter();
                google.maps.event.trigger(thisCom_1.practiceMap, 'resize');
                thisCom_1.practiceMap.setCenter(bound);
            });
        }
    };
    PracticeSearchComponent.prototype.changeTypeaheadLoading = function (e) {
        this.typeaheadLoading = e;
    };
    PracticeSearchComponent.prototype.changeTypeaheadNoResults = function (e) {
        this.typeaheadNoResults = e;
    };
    PracticeSearchComponent.prototype.typeaheadOnSelect = function (e) {
        var _this = this;
        var polygonAreaCode = e.item.polygonAreaCode;
        this.selectedArea = _.find(this.searchResults, function (obj) { return obj.PolygonAreaCode === polygonAreaCode; });
        this.areaService.getAreaSearchByProximity(this.selectedArea.Easting, this.selectedArea.Northing, shared_1.AreaTypeIds.Practice)
            .subscribe(function (result) {
            var nearByAreas = result;
            _this.nearByPractices = _.map(nearByAreas, function (area) {
                return new practice_search_1.Practice(area.AreaName, area.AreaCode, area.AddressLine1, area.AddressLine2, area.Postcode, area.DistanceValF, area.LatLng);
            });
            _this.displayNumberOfPracticesFound(_this.nearByPractices.length, false);
            _this.displayMarkersOnMap();
            _this.showCcgPractices = false;
        });
    };
    PracticeSearchComponent.prototype.displayNumberOfPracticesFound = function (practiceCount, IsCCG) {
        var placeName = '';
        if (IsCCG) {
            placeName = ' in ' + this.ftHelperService.getParentArea().Name;
        }
        else {
            placeName = ' within 5 miles of ' + this.selectedArea.PlaceName;
        }
        var html;
        if (practiceCount === 0) {
            html = 'are no practices';
        }
        else if (practiceCount === 1) {
            html = 'is 1 practice';
        }
        else {
            html = 'are ' + practiceCount + ' practices';
        }
        this.practiceCountText = 'There ' + html + placeName;
    };
    PracticeSearchComponent.prototype.onShowAllPracticeinCCGClick = function () {
        var _this = this;
        this.areaService.getAreaAddressesByParentAreaCode(this.ftHelperService.getFTModel().parentCode, this.ftHelperService.getFTModel().areaTypeId)
            .subscribe(function (result) {
            var areaAddresses = result;
            _this.nearByPractices = _.map(areaAddresses, function (area) {
                return new practice_search_1.Practice(area.Name, area.Code, area.A1, area.A2, area.Postcode, '', area.Pos);
            });
            _this.displayNumberOfPracticesFound(_this.nearByPractices.length, true);
            _this.displayMarkersOnMap();
            _this.showCcgPractices = true;
            _this.placeNameText = '';
        });
    };
    PracticeSearchComponent.prototype.displayMarkersOnMap = function () {
        var _this = this;
        var bounds = new google.maps.LatLngBounds();
        var infoWindow = new google.maps.InfoWindow({});
        var linkList = [];
        var _loop_1 = function (i) {
            var position = new google.maps.LatLng(this_1.nearByPractices[i].lat, this_1.nearByPractices[i].lng);
            bounds.extend(position);
            // Create marker
            var marker = new google.maps.Marker({
                position: position,
                map: this_1.practiceMap
            });
            marker.set('marker_id', this_1.nearByPractices[i].areaCode);
            // Create pop up text
            var boxText = document.createElement("a");
            boxText.id = i.toString();
            boxText.className = "select-area-link";
            boxText.style.cssText = "color: #2e3191; text-decoration: underline; font-size:16px;";
            boxText.innerHTML = this_1.nearByPractices[i].areaName;
            linkList.push(boxText);
            // Map marker click
            google.maps.event.addListener(marker, 'click', function (event) {
                var areaCode = marker.get('marker_id');
                infoWindow.setContent(linkList[i]);
                infoWindow.open(_this.practiceMap, marker);
                var $practiceHeader = $('#' + areaCode);
                // Deselect last selected one if any
                var lastSelectedPracticeIndex = _.findIndex(_this.nearByPractices, function (x) { return x.selected === true; });
                if (lastSelectedPracticeIndex !== -1) {
                    var lastSelectedPractice = _this.nearByPractices[lastSelectedPracticeIndex];
                    lastSelectedPractice.selected = false;
                    if (lastSelectedPracticeIndex !== -1) {
                        _this.nearByPractices.splice(lastSelectedPracticeIndex, 1, lastSelectedPractice);
                    }
                    _this.nearByPractices = _this.nearByPractices.slice();
                }
                // Select-Highlight the current practice on table
                var currentPracticeIndex = _.findIndex(_this.nearByPractices, function (x) { return x.areaCode === areaCode; });
                var currentPractice = _this.nearByPractices[currentPracticeIndex];
                currentPractice.selected = true;
                if (currentPracticeIndex !== -1) {
                    _this.nearByPractices.splice(currentPracticeIndex, 1, currentPractice);
                }
                _this.nearByPractices = _this.nearByPractices.slice();
                // Scroll table so selected practice is at the top
                var scrollTop = $practiceHeader.offset().top -
                    _this.practiceTable.nativeElement.offsetTop +
                    _this.practiceTable.nativeElement.scrollTop;
                _this.practiceTable.nativeElement.scrollTop = scrollTop;
                _this.ref.detectChanges();
            });
            // Practice pop up link click
            google.maps.event.addDomListener(linkList[i], 'click', function (event) {
                var areaCode = marker.get('marker_id');
                _this.onSelectPracticeClick(areaCode);
            });
        };
        var this_1 = this;
        for (var i = 0; i < this.nearByPractices.length; i++) {
            _loop_1(i);
        }
        this.fitMapToPracticeResults(bounds);
        this.height = 610;
    };
    PracticeSearchComponent.prototype.fitMapToPracticeResults = function (bounds) {
        this.practiceMap.fitBounds(bounds);
        var googleMapsEvent = google.maps.event;
        // Add bounds changed listener
        var thisCom = this;
        var zoomChangeBoundsListener = googleMapsEvent.addListenerOnce(this.practiceMap, 'bounds_changed', function () {
            var maximumZoom = 13;
            // Zoom out if to close
            if (thisCom.practiceMap.getZoom() > maximumZoom) {
                thisCom.practiceMap.setZoom(maximumZoom);
            }
            googleMapsEvent.removeListener(zoomChangeBoundsListener);
        });
    };
    PracticeSearchComponent.prototype.onSelectPracticeClick = function (areaCode) {
        this.ftHelperService.lock();
        if (this.localSearchMode) {
            window.location.href = '/profile/general-practice/data#page/12/ati/' +
                shared_1.AreaTypeIds.Practice + '/are/' + areaCode;
        }
        else {
            this.ftHelperService.setAreaCode(areaCode);
            this.ftHelperService.redirectToPopulationPage();
        }
    };
    return PracticeSearchComponent;
}());
__decorate([
    core_1.ViewChild('scrollPracticeTable'),
    __metadata("design:type", typeof (_a = typeof core_1.ElementRef !== "undefined" && core_1.ElementRef) === "function" && _a || Object)
], PracticeSearchComponent.prototype, "practiceTable", void 0);
__decorate([
    core_1.ViewChild('googleMapNew'),
    __metadata("design:type", typeof (_b = typeof core_1.ElementRef !== "undefined" && core_1.ElementRef) === "function" && _b || Object)
], PracticeSearchComponent.prototype, "mapEl", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Boolean)
], PracticeSearchComponent.prototype, "IsMapUpdateRequired", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Boolean)
], PracticeSearchComponent.prototype, "searchMode", void 0);
PracticeSearchComponent = __decorate([
    core_1.Component({
        selector: 'ft-practice-search',
        template: __webpack_require__("./src/app/map/practice-search/practice-search.component.html"),
        styles: [__webpack_require__("./src/app/map/practice-search/practice-search.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_c = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _c || Object, typeof (_d = typeof area_service_1.AreaService !== "undefined" && area_service_1.AreaService) === "function" && _d || Object, typeof (_e = typeof core_1.ChangeDetectorRef !== "undefined" && core_1.ChangeDetectorRef) === "function" && _e || Object])
], PracticeSearchComponent);
exports.PracticeSearchComponent = PracticeSearchComponent;
var _a, _b, _c, _d, _e;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/practice-search.component.js.map

/***/ }),

/***/ "./src/app/map/practice-search/practice-search.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var autoCompleteResult = (function () {
    function autoCompleteResult(polygonAreaCode, name, parentAreaName) {
        this.polygonAreaCode = polygonAreaCode;
        this.displayName = name + ", " + parentAreaName.replace('NHS ', '');
    }
    return autoCompleteResult;
}());
exports.autoCompleteResult = autoCompleteResult;
;
var Practice = (function () {
    function Practice(areaName, areaCode, addressLine1, addressLine2, postcode, distanceValF, pos) {
        this.areaName = areaName;
        this.areaCode = areaCode;
        this.addressLine1 = addressLine1;
        this.addressLine2 = addressLine2;
        this.postcode = postcode;
        this.distanceValF = distanceValF;
        this.lat = pos.Lat;
        this.lng = pos.Lng;
        this.selected = false;
    }
    return Practice;
}());
exports.Practice = Practice;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/practice-search.js.map

/***/ }),

/***/ "./src/app/metadata/metadata-table/metadata-table.component.css":
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/metadata/metadata-table/metadata-table.component.html":
/***/ (function(module, exports) {

module.exports = "<div #table class=\"definition-table\">\r\n    <h2>Indicator Definitions and Supporting Information</h2>\r\n    <tbody>\r\n        <table class=\"bordered-table\" cellspacing=\"0\">\r\n            <tr *ngFor=\"let row of rows\">\r\n                <td class=\"header\" [innerHTML]=\"row.header\"></td>\r\n                <td [innerHTML]=\"row.text\"></td>\r\n            </tr>\r\n        </table>\r\n    </tbody>\r\n</div>"

/***/ }),

/***/ "./src/app/metadata/metadata-table/metadata-table.component.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var Observable_1 = __webpack_require__("./node_modules/rxjs/_esm5/Observable.js");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var indicator_service_1 = __webpack_require__("./src/app/shared/service/api/indicator.service.ts");
var _ = __webpack_require__("./node_modules/underscore/underscore.js");
var common_1 = __webpack_require__("./node_modules/@angular/common/@angular/common.es5.js");
var MetadataTableComponent = (function () {
    function MetadataTableComponent(ftHelperService, indicatorService) {
        this.ftHelperService = ftHelperService;
        this.indicatorService = indicatorService;
        this.isReady = new core_1.EventEmitter();
        this.NotApplicable = 'n/a';
        this.showDataQuality = ftHelperService.getFTConfig().showDataQuality;
    }
    MetadataTableComponent.prototype.showInLightbox = function () {
        this.ftHelperService.showIndicatorMetadataInLightbox(this.table.nativeElement);
    };
    MetadataTableComponent.prototype.displayMetadataForGroupRoot = function (root) {
        var _this = this;
        this.isReady.emit(false);
        // Get data by AJAX
        var metadataPropertiesObservable = this.indicatorService.getIndicatorMetadataProperties();
        var metadataObservable = this.indicatorService.getIndicatorMetadata(root.Grouping[0].GroupId);
        Observable_1.Observable.forkJoin([metadataPropertiesObservable, metadataObservable]).subscribe(function (results) {
            _this.metadataProperties = results[0];
            var metadataHash = results[1];
            var indicatorMetadata = metadataHash[root.IID];
            _this.displayMetadata(indicatorMetadata, root);
            _this.ftHelperService.showAndHidePageElements();
            _this.ftHelperService.unlock();
            _this.isReady.emit(true);
        });
    };
    MetadataTableComponent.prototype.displayMetadataForIndicator = function (indicatorId, restrictToProfileIds) {
        var _this = this;
        this.isReady.emit(false);
        // Get data by AJAX
        var metadataPropertiesObservable = this.indicatorService.getIndicatorMetadataProperties();
        var metadataObservable = this.indicatorService.getIndicatorMetadataByIndicatorId(indicatorId, restrictToProfileIds);
        Observable_1.Observable.forkJoin([metadataPropertiesObservable, metadataObservable]).subscribe(function (results) {
            _this.metadataProperties = results[0];
            var metadataHash = results[1];
            var indicatorMetadata = metadataHash[indicatorId];
            _this.displayMetadata(indicatorMetadata);
            _this.isReady.emit(true);
        });
    };
    MetadataTableComponent.prototype.displayMetadata = function (indicatorMetadata, root) {
        this.tempRows = new Array();
        // Assign key variables
        var descriptive = indicatorMetadata.Descriptive;
        // Define IDs
        var propertyIndex, benchmarkingMethodId, benchmarkingSigLevel;
        if (root) {
            benchmarkingMethodId = root.ComparatorMethodId;
            benchmarkingSigLevel = root.Grouping[0].SigLevel;
        }
        else {
            benchmarkingMethodId = benchmarkingSigLevel = null;
        }
        this.addMetadataRow('Indicator ID', indicatorMetadata.IID);
        if (root) {
            if (root.DateChanges && root.DateChanges.DateOfLastChange && root.DateChanges.DateOfLastChange !== '') {
                var dateOfLastChange = new common_1.DatePipe('en-GB').transform(root.DateChanges.DateOfLastChange, 'dd MMM yyyy');
                if (root.DateChanges.HasDataChangedRecently) {
                    dateOfLastChange = dateOfLastChange + ' <span class="badge badge-success">New data</span>';
                }
                this.addMetadataRow('Date updated', dateOfLastChange);
            }
        }
        // Initial indicator text properties
        for (propertyIndex = 0; propertyIndex < this.metadataProperties.length; propertyIndex++) {
            var property = this.metadataProperties[propertyIndex];
            if (property.Order > 59) {
                break;
            }
            // Do not dislay name as full name is displayed
            if ((property.ColumnName !== 'Name')) {
                this.addMetadataRowByProperty(descriptive, property);
            }
        }
        // Value type
        this.addMetadataRow('Value type', indicatorMetadata.ValueType.Name);
        // Text - Methodology
        this.addMetadataRowByProperty(descriptive, this.metadataProperties[propertyIndex++]);
        // Unit
        var unit = indicatorMetadata.Unit.Label;
        if (unit) {
            this.addMetadataRow('Unit', indicatorMetadata.Unit.Label);
        }
        // Text - Standard population
        this.addMetadataRowByProperty(descriptive, this.metadataProperties[propertyIndex++]);
        if (root) {
            // Age
            this.addMetadataRow('Age', root.Age.Name);
            // Sex
            this.addMetadataRow('Sex', root.Sex.Name);
        }
        // Year type
        this.addMetadataRow('Year type', indicatorMetadata.YearType.Name);
        // Frequency
        this.addMetadataRowByProperty(descriptive, this.metadataProperties[propertyIndex++]);
        // Benchmarking method
        if (benchmarkingMethodId) {
            var row = new MetadataRow('Benchmarking method', '');
            this.tempRows.push(row);
            this.assignBenchmarkingMethod(row, benchmarkingMethodId);
        }
        // Benchmarking significance level
        if (benchmarkingSigLevel) {
            var text = (benchmarkingSigLevel <= 0) ?
                this.NotApplicable :
                benchmarkingSigLevel + '%';
            this.addMetadataRow('Benchmarking significance level', text);
        }
        // Confidence interval method
        var ciMethod = indicatorMetadata.ConfidenceIntervalMethod;
        if (ciMethod) {
            this.addMetadataRow('Confidence interval method', ciMethod.Name);
            // Display CI method description
            var cimDescription = ciMethod.Description;
            if (cimDescription) {
                this.addMetadataRow('Confidence interval methodology', cimDescription);
            }
        }
        // Confidence level
        var confidenceLevel = indicatorMetadata.ConfidenceLevel;
        if (confidenceLevel > -1) {
            this.addMetadataRow('Confidence level', confidenceLevel + '%');
        }
        // Display all remaining properties
        while (propertyIndex < this.metadataProperties.length) {
            this.addMetadataRowByProperty(descriptive, this.metadataProperties[propertyIndex]);
            propertyIndex++;
        }
        // Trigger view refresh
        this.rows = this.tempRows;
    };
    MetadataTableComponent.prototype.addMetadataRow = function (header, text) {
        this.tempRows.push(new MetadataRow(header, text));
    };
    MetadataTableComponent.prototype.addMetadataRowByProperty = function (textMetadata, property) {
        var columnName = property.ColumnName;
        if (textMetadata !== null && textMetadata.hasOwnProperty(columnName)) {
            var text = textMetadata[columnName];
            if (!_.isUndefined(text) && text !== '') {
                if ((columnName === 'DataQuality') && this.showDataQuality) {
                    // Add data quality flags instead of number
                    var dataQualityCount = parseInt(text);
                    var displayText = this.ftHelperService.getIndicatorDataQualityHtml(text) + ' ' +
                        this.ftHelperService.getIndicatorDataQualityTooltipText(dataQualityCount);
                }
                else {
                    displayText = text;
                }
                var row = new MetadataRow(property.DisplayName, displayText);
                this.tempRows.push(row);
            }
        }
    };
    MetadataTableComponent.prototype.assignBenchmarkingMethod = function (row, benchmarkingMethodId) {
        var metadataObservable = this.indicatorService.getBenchmarkingMethod(benchmarkingMethodId).subscribe(function (data) {
            var method = data;
            row.text = method.Name;
        }, function (error) { });
    };
    return MetadataTableComponent;
}());
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], MetadataTableComponent.prototype, "isReady", void 0);
__decorate([
    core_1.ViewChild('table'),
    __metadata("design:type", typeof (_a = typeof core_1.ElementRef !== "undefined" && core_1.ElementRef) === "function" && _a || Object)
], MetadataTableComponent.prototype, "table", void 0);
MetadataTableComponent = __decorate([
    core_1.Component({
        selector: 'ft-metadata-table',
        template: __webpack_require__("./src/app/metadata/metadata-table/metadata-table.component.html"),
        styles: [__webpack_require__("./src/app/metadata/metadata-table/metadata-table.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_b = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _b || Object, typeof (_c = typeof indicator_service_1.IndicatorService !== "undefined" && indicator_service_1.IndicatorService) === "function" && _c || Object])
], MetadataTableComponent);
exports.MetadataTableComponent = MetadataTableComponent;
var MetadataRow = (function () {
    function MetadataRow(header, text) {
        this.header = header;
        this.text = text;
    }
    return MetadataRow;
}());
var _a, _b, _c;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/metadata-table.component.js.map

/***/ }),

/***/ "./src/app/metadata/metadata.component.css":
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/metadata/metadata.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"metadata-container\" class=\"container\" style=\"display:none;\">\r\n    <ft-metadata-table #table></ft-metadata-table>\r\n</div>"

/***/ }),

/***/ "./src/app/metadata/metadata.component.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
__webpack_require__("./node_modules/rxjs/_esm5/Rx.js");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var MetadataComponent = (function () {
    function MetadataComponent(ftHelperService) {
        this.ftHelperService = ftHelperService;
    }
    MetadataComponent.prototype.onOutsideEvent = function (event) {
        var root = this.ftHelperService.getCurrentGroupRoot();
        var grouping = root.Grouping[0];
        this.table.displayMetadataForGroupRoot(root);
    };
    return MetadataComponent;
}());
__decorate([
    core_1.ViewChild('table'),
    __metadata("design:type", Object)
], MetadataComponent.prototype, "table", void 0);
__decorate([
    core_1.HostListener('window:MetadataSelected', ['$event']),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [Object]),
    __metadata("design:returntype", void 0)
], MetadataComponent.prototype, "onOutsideEvent", null);
MetadataComponent = __decorate([
    core_1.Component({
        selector: 'ft-metadata',
        template: __webpack_require__("./src/app/metadata/metadata.component.html"),
        styles: [__webpack_require__("./src/app/metadata/metadata.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _a || Object])
], MetadataComponent);
exports.MetadataComponent = MetadataComponent;
var _a;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/metadata.component.js.map

/***/ }),

/***/ "./src/app/metadata/metadata.module.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var common_1 = __webpack_require__("./node_modules/@angular/common/@angular/common.es5.js");
var metadata_component_1 = __webpack_require__("./src/app/metadata/metadata.component.ts");
var metadata_table_component_1 = __webpack_require__("./src/app/metadata/metadata-table/metadata-table.component.ts");
var MetadataModule = (function () {
    function MetadataModule() {
    }
    return MetadataModule;
}());
MetadataModule = __decorate([
    core_1.NgModule({
        imports: [
            common_1.CommonModule
        ],
        declarations: [
            metadata_component_1.MetadataComponent,
            metadata_table_component_1.MetadataTableComponent
        ],
        exports: [
            metadata_component_1.MetadataComponent,
            metadata_table_component_1.MetadataTableComponent
        ],
    })
], MetadataModule);
exports.MetadataModule = MetadataModule;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/metadata.module.js.map

/***/ }),

/***/ "./src/app/population/population-chart/population-chart.component.css":
/***/ (function(module, exports) {

module.exports = "#population-chart {\r\n    float: left;\r\n    clear: both;\r\n    width: 600px;\r\n    height: 600px;\r\n}"

/***/ }),

/***/ "./src/app/population/population-chart/population-chart.component.html":
/***/ (function(module, exports) {

module.exports = "<div style=\"float: left;width: 600px;\">\r\n    <div class=\"export-chart-box\">\r\n        <button class=\"export-link\" (click)=\"exportChart()\">Export chart as image</button>\r\n    </div>\r\n</div>\r\n<div id=\"population-chart\" #chart>\r\n</div>"

/***/ }),

/***/ "./src/app/population/population-chart/population-chart.component.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var population_1 = __webpack_require__("./src/app/population/population.ts");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var _ = __webpack_require__("./node_modules/underscore/underscore.js");
var Highcharts = __webpack_require__("./node_modules/highcharts/highcharts.js");
__webpack_require__("./node_modules/highcharts/modules/exporting.js")(Highcharts);
__webpack_require__("./node_modules/highcharts/highcharts-more.js")(Highcharts);
var PopulationChartComponent = (function () {
    function PopulationChartComponent(ftHelperService) {
        this.ftHelperService = ftHelperService;
        this.chartColours = {
            label: '#333',
            bar: '#76B3B3',
            pink: '#FF66FC'
        };
        this.chartTextStyle = {
            color: this.chartColours.label,
            fontWeight: 'normal',
            fontFamily: 'Verdana'
        };
    }
    PopulationChartComponent.prototype.ngOnChanges = function (changes) {
        if (this.isAnyData()) {
            var chartContainer = null;
            if (this.chartEl && this.chartEl.nativeElement) {
                chartContainer = this.chartEl.nativeElement;
                this.chart = new Highcharts.Chart(chartContainer, this.getChartOptions());
            }
        }
    };
    PopulationChartComponent.prototype.isAnyData = function () {
        return !_.isUndefined(this.populations);
    };
    PopulationChartComponent.prototype.getChartOptions = function () {
        var model = this.ftHelperService.getFTModel();
        var max = new population_1.PopulationMaxFinder().getMaximumValue(this.populations);
        // Populations
        var areaPopulation = this.populations.areaPopulation;
        var areaParentPopulation = this.populations.areaParentPopulation;
        var nationalPopulation = this.populations.nationalPopulation;
        // Labels
        var subtitle = nationalPopulation.IndicatorName + " " + nationalPopulation.Period;
        var maleString = ' (Male)';
        var femaleString = ' (Female)';
        this.chartTitle = '<div style="text-align:center;">Age Profile<br><span style="font-size:12px;">' + subtitle + '</span></div>';
        var series = [];
        // Area series
        var areaName = this.ftHelperService.getAreaName(model.areaCode);
        series.push({
            name: areaName + maleString,
            data: areaPopulation.Values[shared_1.SexIds.Male],
            type: 'bar',
            color: shared_1.Colour.bobLower
        }, {
            name: areaName + femaleString,
            data: areaPopulation.Values[shared_1.SexIds.Female],
            type: 'bar',
            color: shared_1.Colour.bobHigher
        });
        // Parent area series
        var isParentNotEngland = model.parentCode.toUpperCase() !== shared_1.AreaCodes.England;
        if (isParentNotEngland) {
            var parentAreaName = this.ftHelperService.getParentArea().Name;
            var parentColour = this.chartColours.pink;
            series.push({
                name: parentAreaName,
                data: areaParentPopulation.Values[shared_1.SexIds.Male],
                color: parentColour,
            }, {
                name: parentAreaName + femaleString,
                data: areaParentPopulation.Values[shared_1.SexIds.Female],
                color: parentColour,
                showInLegend: false
            });
        }
        // England series
        series.push({
            name: 'England',
            data: nationalPopulation.Values[shared_1.SexIds.Male],
            color: shared_1.Colour.comparator
        }, {
            name: 'England' + femaleString,
            data: nationalPopulation.Values[shared_1.SexIds.Female],
            color: shared_1.Colour.comparator,
            showInLegend: false
        });
        return ({
            chart: {
                defaultSeriesType: 'line',
                margin: [60, 55, 150, 55],
                /* margins must be set explicitly to avoid labels being positioned outside visible chart area */
                width: 600,
                height: 550
            },
            title: {
                text: this.chartTitle,
                style: this.chartTextStyle,
                useHTML: true
            },
            xAxis: [{
                    categories: nationalPopulation.Labels,
                    reversed: false
                }, {
                    opposite: true,
                    reversed: false,
                    categories: nationalPopulation.Labels,
                    linkedTo: 0
                }
            ],
            yAxis: [{
                    min: -max,
                    max: max,
                    title: {
                        text: '% of total population',
                        style: this.chartTextStyle
                    },
                    labels: {
                        formatter: function () {
                            return Math.abs(this.value);
                        }
                    }
                }],
            plotOptions: {
                series: {
                    events: {
                        legendItemClick: function () {
                            return false;
                        }
                    }
                },
                bar: {
                    borderWidth: 0,
                    pointPadding: 0,
                    stacking: 'normal',
                    animation: false
                },
                line: {
                    // The symbol is a non-valid option here to work round a bug
                    // in highcharts where only half of the markers appear on hover
                    marker: shared_1.HC.noLineMarker,
                    animation: false,
                    states: {
                        hover: {
                            marker: shared_1.HC.noLineMarker
                        }
                    }
                }
            },
            legend: {
                enabled: true,
                layout: 'vertical',
                align: 'center',
                verticalAlign: 'bottom',
                itemStyle: {
                    fontWeight: 'normal'
                }
            },
            tooltip: {
                formatter: function () {
                    // Get series name
                    var name = this.series.name;
                    var alreadyHasSexLabel = new RegExp(/e[\)]$/);
                    if (!alreadyHasSexLabel.test(name)) {
                        name += maleString;
                    }
                    return '<b>' + name + '<br>Age: ' +
                        this.point.category + '</b><br/>' +
                        'Population: ' + Math.abs(this.point.y) + '%';
                }
            },
            credits: shared_1.HC.credits,
            exporting: {
                enabled: false
            },
            series: series
        });
    };
    PopulationChartComponent.prototype.exportChart = function () {
        var chartTitle = this.chartTitle;
        this.chart.exportChart({ type: 'image/png' }, {
            chart: {
                spacingTop: 70,
                events: {
                    load: function () {
                        this.renderer.text(chartTitle, 300, 15)
                            .attr({
                            align: 'center'
                        })
                            .css({
                            fontSize: '14px',
                            width: '600px'
                        })
                            .add();
                    }
                }
            },
            title: {
                text: '',
                style: this.chartTextStyle
            }
        });
        this.ftHelperService.logEvent('ExportImage', 'Population');
    };
    return PopulationChartComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_a = typeof population_1.Populations !== "undefined" && population_1.Populations) === "function" && _a || Object)
], PopulationChartComponent.prototype, "populations", void 0);
__decorate([
    core_1.ViewChild('chart'),
    __metadata("design:type", typeof (_b = typeof core_1.ElementRef !== "undefined" && core_1.ElementRef) === "function" && _b || Object)
], PopulationChartComponent.prototype, "chartEl", void 0);
PopulationChartComponent = __decorate([
    core_1.Component({
        selector: 'ft-population-chart',
        template: __webpack_require__("./src/app/population/population-chart/population-chart.component.html"),
        styles: [__webpack_require__("./src/app/population/population-chart/population-chart.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_c = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _c || Object])
], PopulationChartComponent);
exports.PopulationChartComponent = PopulationChartComponent;
var _a, _b, _c;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/population-chart.component.js.map

/***/ }),

/***/ "./src/app/population/population-summary/population-summary.component.css":
/***/ (function(module, exports) {

module.exports = ":host /deep/ #population-info,\r\n#registered-persons {\r\n    width: 355px;\r\n    float: left;\r\n    margin-top: 6px;\r\n}\r\n\r\n:host /deep/ #population-info .bordered-table {\r\n    margin-bottom: 15px;\r\n    width: 100%;\r\n    float: left;\r\n}\r\n\r\n:host /deep/ #registered-persons td.header,\r\n#further-info-table td.header {\r\n    width: 200px;\r\n    background: #fafafa;\r\n    padding: 3px 3px 3px 0;\r\n    vertical-align: top;\r\n    text-align: right;\r\n}\r\n\r\n#population-info #decile-key {\r\n    width: 100%;\r\n}\r\n\r\n.practice-label {\r\n    width: 100%;\r\n    float: left;\r\n    text-align: center;\r\n    font-size: 14px;\r\n}\r\n\r\n#decile-key td {\r\n    height: 9px;\r\n    width: 30px;\r\n    border: 1px solid white;\r\n    border-top: 2px solid white;\r\n    border-bottom: none;\r\n}\r\n\r\n#deprivation-table,\r\n#ethnicity-table {\r\n    margin-top: 15px;\r\n}\r\n\r\n#deprivation {\r\n    text-align: center;\r\n}\r\n\r\n.deprivation-label {\r\n    font-size: 10px;\r\n    line-height: 10px;\r\n    color: #666;\r\n    margin-bottom: 3px;\r\n}"

/***/ }),

/***/ "./src/app/population/population-summary/population-summary.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"population-info\" style=\"float: right; width: 360px;\" *ngIf=\"shouldDisplay()\">\r\n    <ft-metadata-table #metadata style=\"display:none;\" (isReady)=\"metadataIsReady($event)\"></ft-metadata-table>\r\n    <ft-registered-persons-table [registeredPersons]=\"registeredPersons\" (metadataToShow)=\"showMetadata($event)\"></ft-registered-persons-table>\r\n    <div class=\"practice-label\">{{practiceLabel}}</div>\r\n    <table id=\"further-info-table\" class=\"bordered-table\" cellspacing=\"0\">\r\n        <tr *ngFor=\"let row of adHocIndicatorRows\">\r\n            <td class=\"header\">\r\n                <div class=\"fl info-tooltip\" (click)=\"showMetadata(row.indicatorId)\"></div>\r\n                {{row.indicatorName}}\r\n            </td>\r\n            <td [innerHTML]=\"row.valueText\"></td>\r\n        </tr>\r\n    </table>\r\n    <table id=\"deprivation-table\" class=\"bordered-table\" cellspacing=\"0\">\r\n        <thead>\r\n            <tr>\r\n                <th>\r\n                    <div class=\"w100\" style=\"position: relative;\">\r\n                        Deprivation\r\n                        <div class=\"right-tooltip-icon info-tooltip\" (click)=\"showDeprivationMetadata()\"></div>\r\n                    </div>\r\n                </th>\r\n            </tr>\r\n        </thead>\r\n        <tbody>\r\n            <tr>\r\n                <td id=\"deprivation\" style=\"border-bottom: none;\" [innerHtml]=\"decileLabel\"></td>\r\n            </tr>\r\n            <tr>\r\n                <td style=\"border-top: none;\">\r\n                    <table id=\"decile-key\" cellspacing=\"0\">\r\n                        <tr>\r\n                            <td *ngFor=\"let decile of decileLabels\" [innerHtml]=\"decile.text\"></td>\r\n                        </tr>\r\n                        <tr>\r\n                            <td *ngFor=\"let decile of decileLabels\" [class]=\"decile.getClass()\"></td>\r\n                        </tr>\r\n                    </table>\r\n                    <div class=\"fl deprivation-label\">More deprived</div>\r\n                    <div class=\"fr deprivation-label\">Less deprived</div>\r\n                </td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n    <table id=\"ethnicity-table\" class=\"bordered-table\" cellspacing=\"0\">\r\n        <thead>\r\n            <tr>\r\n                <th>\r\n                    <div class=\"w100\" style=\"position: relative;\">\r\n                        Ethnicity Estimate\r\n                        <div class=\"right-tooltip-icon info-tooltip\" (click)=\"showEthnicityMetadata()\">\r\n                        </div>\r\n                    </div>\r\n                </th>\r\n            </tr>\r\n        </thead>\r\n        <tbody>\r\n            <tr>\r\n                <td [innerHtml]=\"ethnicityLabel\"></td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n</div>"

/***/ }),

/***/ "./src/app/population/population-summary/population-summary.component.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var population_1 = __webpack_require__("./src/app/population/population.ts");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var _ = __webpack_require__("./node_modules/underscore/underscore.js");
var metadata_table_component_1 = __webpack_require__("./src/app/metadata/metadata-table/metadata-table.component.ts");
var PopulationSummaryComponent = (function () {
    function PopulationSummaryComponent(ftHelperService) {
        this.ftHelperService = ftHelperService;
        this.noData = '<div class="no-data">-</div>';
    }
    PopulationSummaryComponent.prototype.ngOnChanges = function (changes) {
        if (this.allPopulationData) {
            var model = this.ftHelperService.getFTModel();
            var areaName = this.ftHelperService.getAreaName(model.areaCode);
            this.defineRegisteredPersons(areaName);
            this.practiceLabel = model.areaCode + ' - ' + areaName;
            this.defineAdHocIndicatorRows();
            this.defineDeprivationDecile();
            this.defineEthnicityLabel();
        }
    };
    PopulationSummaryComponent.prototype.defineDeprivationDecile = function () {
        // Decile label
        var decileNumber = this.allPopulationData.populationSummary.GpDeprivationDecile;
        if (_.isUndefined(decileNumber)) {
            this.decileLabel = '<i>Data not available for current practice</i>';
        }
        else {
            var deciles = this.allPopulationData.deprivationDeciles;
            this.decileLabel = _.find(deciles, function (decile) { return decile.Id === decileNumber; }).Name;
        }
        // Decile blocks
        var decileLabels = [];
        for (var i = 1; i <= 10; i++) {
            decileLabels.push(new DecileLabel(i));
        }
        decileLabels[decileNumber - 1].text = '<div class="decile decile' +
            decileNumber + '" >' + decileNumber + '</div>';
        this.decileLabels = decileLabels;
    };
    PopulationSummaryComponent.prototype.defineAdHocIndicatorRows = function () {
        var rows = [];
        var values = this.allPopulationData.populationSummary.AdHocValues;
        // QOF achievement
        var qof = values.Qof;
        var text = _.isUndefined(qof)
            ? this.noData
            : shared_1.NumberHelper.roundNumber(qof.Count, 1) + ' (out of ' + qof.Denom + ')';
        rows.push(new AdHocIndicatorRow(shared_1.IndicatorIds.QofPoints, 'QOF achievement', text));
        // Life expectancy
        rows.push(this.getLifeExpectancy('Male', values.LifeExpectancyMale));
        rows.push(this.getLifeExpectancy('Female', values.LifeExpectancyFemale));
        // Patients that recommend practice
        var recommend = values.Recommend;
        text = _.isUndefined(recommend) ?
            this.noData
            : recommend.ValF + '%';
        rows.push(new AdHocIndicatorRow(shared_1.IndicatorIds.WouldRecommendPractice, '% of patients that would recommend their practice', text));
        this.adHocIndicatorRows = rows;
    };
    PopulationSummaryComponent.prototype.getLifeExpectancy = function (sex, data) {
        var text = _.isUndefined(data)
            ? this.noData
            : data.ValF + ' years';
        return new AdHocIndicatorRow(shared_1.IndicatorIds.LifeExpectancy, 'Life expectancy (' + sex + ')', text);
    };
    PopulationSummaryComponent.prototype.defineEthnicityLabel = function () {
        var ethnicity = this.allPopulationData.populationSummary.Ethnicity;
        this.ethnicityLabel = _.isUndefined(ethnicity)
            ? '<i>Insufficient data to provide accurate summary</i>'
            : ethnicity;
    };
    PopulationSummaryComponent.prototype.shouldDisplay = function () {
        return this.ftHelperService.getFTModel().areaTypeId === shared_1.AreaTypeIds.Practice;
    };
    PopulationSummaryComponent.prototype.defineRegisteredPersons = function (areaName) {
        var populations = this.allPopulationData.populations;
        var parentAreaName = this.ftHelperService.getParentArea().Name;
        this.registeredPersons = [
            this.getRegisteredPersons(areaName, false, populations.areaPopulation),
            this.getRegisteredPersons(parentAreaName, true, populations.areaParentPopulation),
            this.getRegisteredPersons('ENGLAND', true, populations.nationalPopulation)
        ];
    };
    PopulationSummaryComponent.prototype.getRegisteredPersons = function (areaName, isAverage, population) {
        var persons = new population_1.RegisteredPersons();
        persons.isAverage = isAverage;
        persons.areaName = areaName;
        persons.personCount = this.formatListSize(population.ListSize);
        return persons;
    };
    PopulationSummaryComponent.prototype.formatListSize = function (listSize) {
        return listSize && listSize.Val > 0
            ? new shared_1.CommaNumber(listSize.Val).rounded()
            : null;
    };
    PopulationSummaryComponent.prototype.showDeprivationMetadata = function () {
        this.showMetadata(shared_1.IndicatorIds.DeprivationScore);
    };
    PopulationSummaryComponent.prototype.showEthnicityMetadata = function () {
        this.showMetadata(shared_1.IndicatorIds.EthnicityEstimates);
    };
    PopulationSummaryComponent.prototype.showMetadata = function (indicatorId) {
        this.metadata.displayMetadataForIndicator(indicatorId, [shared_1.ProfileIds.PracticeProfileSupportingIndicators]);
    };
    PopulationSummaryComponent.prototype.metadataIsReady = function (isReady) {
        if (isReady) {
            var metadata_1 = this.metadata;
            // Set timeout so metadata table view will have finished rendering
            setTimeout(function () { metadata_1.showInLightbox(); });
        }
    };
    return PopulationSummaryComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_a = typeof population_1.AllPopulationData !== "undefined" && population_1.AllPopulationData) === "function" && _a || Object)
], PopulationSummaryComponent.prototype, "allPopulationData", void 0);
__decorate([
    core_1.ViewChild('metadata'),
    __metadata("design:type", typeof (_b = typeof metadata_table_component_1.MetadataTableComponent !== "undefined" && metadata_table_component_1.MetadataTableComponent) === "function" && _b || Object)
], PopulationSummaryComponent.prototype, "metadata", void 0);
PopulationSummaryComponent = __decorate([
    core_1.Component({
        selector: 'ft-population-summary',
        template: __webpack_require__("./src/app/population/population-summary/population-summary.component.html"),
        styles: [__webpack_require__("./src/app/population/population-summary/population-summary.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_c = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _c || Object])
], PopulationSummaryComponent);
exports.PopulationSummaryComponent = PopulationSummaryComponent;
var AdHocIndicatorRow = (function () {
    function AdHocIndicatorRow(indicatorId, indicatorName, valueText) {
        this.indicatorId = indicatorId;
        this.indicatorName = indicatorName;
        this.valueText = valueText;
    }
    return AdHocIndicatorRow;
}());
var DecileLabel = (function () {
    function DecileLabel(num) {
        this.num = num;
        this.text = '';
    }
    DecileLabel.prototype.getClass = function () {
        return 'decile' + this.num;
    };
    return DecileLabel;
}());
var _a, _b, _c;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/population-summary.component.js.map

/***/ }),

/***/ "./src/app/population/population.component.css":
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/population/population.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"population-container\" style=\"display:none;\">\r\n    <div class=\"standard-width\">\r\n        <ft-population-chart [populations]=\"populations\" *ngIf=\"isAnyData()\"></ft-population-chart>\r\n        <ft-population-summary *ngIf=\"showSummary\" [allPopulationData]=\"allPopulationData\"></ft-population-summary>\r\n        <div class=\"no-indicator-data\" *ngIf=\"!isAnyData()\">\r\n            No population data available for current area type\r\n        </div>\r\n    </div>\r\n</div>"

/***/ }),

/***/ "./src/app/population/population.component.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var Observable_1 = __webpack_require__("./node_modules/rxjs/_esm5/Observable.js");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var indicator_service_1 = __webpack_require__("./src/app/shared/service/api/indicator.service.ts");
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var population_1 = __webpack_require__("./src/app/population/population.ts");
var PopulationComponent = (function () {
    function PopulationComponent(ftHelperService, indicatorService) {
        this.ftHelperService = ftHelperService;
        this.indicatorService = indicatorService;
        this.populations = null;
    }
    PopulationComponent.prototype.onOutsideEvent = function (event) {
        var _this = this;
        var model = this.ftHelperService.getFTModel();
        this.showSummary = model.areaTypeId === shared_1.AreaTypeIds.Practice;
        // Get populations
        var observables = [];
        observables.push(this.indicatorService.getPopulation(model.areaCode, model.areaTypeId), this.indicatorService.getPopulation(model.parentCode, model.areaTypeId), this.indicatorService.getPopulation(shared_1.AreaCodes.England, model.areaTypeId));
        // Get summary data
        if (this.showSummary) {
            observables.push(this.indicatorService.getPopulationSummary(model.areaCode, model.areaTypeId), this.indicatorService.getCategories(shared_1.CategoryTypeIds.DeprivationDecileGp2015));
        }
        Observable_1.Observable.forkJoin(observables).subscribe(function (results) {
            // Init populations
            var populations = new population_1.Populations();
            populations.areaPopulation = results[0];
            populations.areaParentPopulation = results[1];
            populations.nationalPopulation = results[2];
            new population_1.PopulationModifier(populations).makeMalePopulationsNegative();
            _this.populations = populations;
            // Summary
            if (_this.showSummary) {
                var deprivationDeciles = results[4];
                var populationSummary = results[3];
                _this.allPopulationData = new population_1.AllPopulationData(_this.populations, populationSummary, deprivationDeciles);
            }
            _this.ftHelperService.showAndHidePageElements();
            _this.ftHelperService.unlock();
        });
    };
    PopulationComponent.prototype.isAnyData = function () {
        return this.populations &&
            this.populations.nationalPopulation.IndicatorName !== null;
    };
    return PopulationComponent;
}());
__decorate([
    core_1.HostListener('window:PopulationSelected', ['$event']),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [Object]),
    __metadata("design:returntype", void 0)
], PopulationComponent.prototype, "onOutsideEvent", null);
PopulationComponent = __decorate([
    core_1.Component({
        selector: 'ft-population',
        template: __webpack_require__("./src/app/population/population.component.html"),
        styles: [__webpack_require__("./src/app/population/population.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _a || Object, typeof (_b = typeof indicator_service_1.IndicatorService !== "undefined" && indicator_service_1.IndicatorService) === "function" && _b || Object])
], PopulationComponent);
exports.PopulationComponent = PopulationComponent;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/population.component.js.map

/***/ }),

/***/ "./src/app/population/population.module.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var common_1 = __webpack_require__("./node_modules/@angular/common/@angular/common.es5.js");
var shared_module_1 = __webpack_require__("./src/app/shared/shared.module.ts");
var population_component_1 = __webpack_require__("./src/app/population/population.component.ts");
var population_chart_component_1 = __webpack_require__("./src/app/population/population-chart/population-chart.component.ts");
var population_summary_component_1 = __webpack_require__("./src/app/population/population-summary/population-summary.component.ts");
var registered_persons_table_component_1 = __webpack_require__("./src/app/population/registered-persons-table/registered-persons-table.component.ts");
var metadata_module_1 = __webpack_require__("./src/app/metadata/metadata.module.ts");
var PopulationModule = (function () {
    function PopulationModule() {
    }
    return PopulationModule;
}());
PopulationModule = __decorate([
    core_1.NgModule({
        imports: [
            common_1.CommonModule,
            shared_module_1.SharedModule,
            metadata_module_1.MetadataModule
        ],
        declarations: [
            population_component_1.PopulationComponent,
            population_chart_component_1.PopulationChartComponent,
            population_summary_component_1.PopulationSummaryComponent,
            registered_persons_table_component_1.RegisteredPersonsTableComponent
        ],
        exports: [
            population_component_1.PopulationComponent
        ]
    })
], PopulationModule);
exports.PopulationModule = PopulationModule;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/population.module.js.map

/***/ }),

/***/ "./src/app/population/population.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var _ = __webpack_require__("./node_modules/underscore/underscore.js");
var Populations = (function () {
    function Populations() {
    }
    return Populations;
}());
exports.Populations = Populations;
var AllPopulationData = (function () {
    function AllPopulationData(populations, populationSummary, deprivationDeciles) {
        this.populations = populations;
        this.populationSummary = populationSummary;
        this.deprivationDeciles = deprivationDeciles;
    }
    return AllPopulationData;
}());
exports.AllPopulationData = AllPopulationData;
var RegisteredPersons = (function () {
    function RegisteredPersons() {
    }
    RegisteredPersons.prototype.hasPersonCount = function () {
        return this.personCount !== null;
    };
    return RegisteredPersons;
}());
exports.RegisteredPersons = RegisteredPersons;
var PopulationModifier = (function () {
    function PopulationModifier(populations) {
        this.populations = populations;
    }
    /** Make male population percentages -ve so they are displayed on the opposite side of the axis
     * to female values
     */
    PopulationModifier.prototype.makeMalePopulationsNegative = function () {
        this.makeMaleValuesNegative(this.populations.areaPopulation);
        this.makeMaleValuesNegative(this.populations.areaParentPopulation);
        this.makeMaleValuesNegative(this.populations.nationalPopulation);
    };
    PopulationModifier.prototype.makeMaleValuesNegative = function (population) {
        var values = population.Values;
        if (!_.isUndefined(values[shared_1.SexIds.Male]) || !_.isUndefined(shared_1.SexIds.Female)) {
            var maleValues = values[shared_1.SexIds.Male];
            for (var i in maleValues) {
                maleValues[i] = -Math.abs(maleValues[i]);
            }
        }
    };
    return PopulationModifier;
}());
exports.PopulationModifier = PopulationModifier;
var PopulationMaxFinder = (function () {
    function PopulationMaxFinder() {
    }
    /** Finds the maximum population value to enable equal axis limits to be
     * set on both the male and female sides
     */
    PopulationMaxFinder.prototype.getMaximumValue = function (populations) {
        return this.getPopulationMax([
            populations.areaPopulation,
            populations.areaParentPopulation,
            populations.nationalPopulation
        ]);
    };
    PopulationMaxFinder.prototype.getPopulationMax = function (populations) {
        var max = 5;
        var min = -max;
        for (var i in populations) {
            if (populations[i] != null) {
                min = _.min([min, _.min(populations[i].Values[shared_1.SexIds.Male])]);
                max = _.max([max, _.max(populations[i].Values[shared_1.SexIds.Female])]);
            }
        }
        return Math.ceil(_.max([max, -min]));
    };
    return PopulationMaxFinder;
}());
exports.PopulationMaxFinder = PopulationMaxFinder;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/population.js.map

/***/ }),

/***/ "./src/app/population/registered-persons-table/registered-persons-table.component.css":
/***/ (function(module, exports) {

module.exports = ".population-heading {\r\n    margin-top: 15px;\r\n    float: left;\r\n    text-align: center;\r\n    width: 100%;\r\n    font-weight: bold;\r\n    position: relative;\r\n}\r\n\r\n.average {\r\n    font-size: 9px;\r\n}"

/***/ }),

/***/ "./src/app/population/registered-persons-table/registered-persons-table.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"registered-persons\">\r\n    <div class=\"population-heading\">\r\n        <div class=\"right-tooltip-icon info-tooltip\" (click)=\"showMetadata()\">\r\n        </div>\r\n        <div>Registered Persons</div>\r\n    </div>\r\n    <table class=\"bordered-table\" cellspacing=\"0\">\r\n        <tbody>\r\n            <tr *ngFor=\"let row of registeredPersons\">\r\n                <td class=\"header\">\r\n                    {{row.areaName}}\r\n                </td>\r\n                <td>{{row.personCount}}\r\n                    <div *ngIf=\"!row.hasPersonCount()\" class=\"no-data\">-</div>\r\n                    <span *ngIf=\"row.isAverage\" class=\"average\">(average)</span>\r\n                </td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n</div>"

/***/ }),

/***/ "./src/app/population/registered-persons-table/registered-persons-table.component.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var RegisteredPersonsTableComponent = (function () {
    function RegisteredPersonsTableComponent() {
        this.metadataToShow = new core_1.EventEmitter();
    }
    RegisteredPersonsTableComponent.prototype.showMetadata = function () {
        this.metadataToShow.emit(shared_1.IndicatorIds.QofListSize);
    };
    return RegisteredPersonsTableComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", Array)
], RegisteredPersonsTableComponent.prototype, "registeredPersons", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], RegisteredPersonsTableComponent.prototype, "metadataToShow", void 0);
RegisteredPersonsTableComponent = __decorate([
    core_1.Component({
        selector: 'ft-registered-persons-table',
        template: __webpack_require__("./src/app/population/registered-persons-table/registered-persons-table.component.html"),
        styles: [__webpack_require__("./src/app/population/registered-persons-table/registered-persons-table.component.css")]
    })
], RegisteredPersonsTableComponent);
exports.RegisteredPersonsTableComponent = RegisteredPersonsTableComponent;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/registered-persons-table.component.js.map

/***/ }),

/***/ "./src/app/shared/component/indicator-header/indicator-header.component.css":
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/shared/component/indicator-header/indicator-header.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"indicator-details-header\" *ngIf=\"header\">\r\n    <div class=\"trend-header\">\r\n        <div class=\"trend-title\">\r\n            <a class=\"trend-link\" title=\"More about this indicator\" href=\"javascript:goToMetadataPage(rootIndex);\">\r\n               {{header.indicatorName}} {{header.ageSexLabel}} </a><span *ngIf=\"header.hasNewData\" style=\"margin-right: 8px;\" class=\"badge badge-success\">New data</span>\r\n            <span class=\"benchmark-name\">{{header.comparatorName}}</span></div>\r\n        <div class=\"trend-unit\"><span> {{header.valueType}} {{header.unit}} </span>\r\n        </div>\r\n    </div>\r\n</div>\r\n"

/***/ }),

/***/ "./src/app/shared/component/indicator-header/indicator-header.component.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var IndicatorHeaderComponent = (function () {
    function IndicatorHeaderComponent() {
    }
    return IndicatorHeaderComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", IndicatorHeader)
], IndicatorHeaderComponent.prototype, "header", void 0);
IndicatorHeaderComponent = __decorate([
    core_1.Component({
        selector: 'ft-indicator-header',
        template: __webpack_require__("./src/app/shared/component/indicator-header/indicator-header.component.html"),
        styles: [__webpack_require__("./src/app/shared/component/indicator-header/indicator-header.component.css")]
    })
], IndicatorHeaderComponent);
exports.IndicatorHeaderComponent = IndicatorHeaderComponent;
var IndicatorHeader = (function () {
    function IndicatorHeader(indicatorName, hasNewData, comparatorName, valueType, unit, ageSexLabel) {
        this.indicatorName = indicatorName;
        this.hasNewData = hasNewData;
        this.comparatorName = comparatorName;
        this.valueType = valueType;
        this.unit = unit;
        this.ageSexLabel = ageSexLabel;
    }
    return IndicatorHeader;
}());
exports.IndicatorHeader = IndicatorHeader;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/indicator-header.component.js.map

/***/ }),

/***/ "./src/app/shared/service/api/area.service.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var http_1 = __webpack_require__("./node_modules/@angular/http/@angular/http.es5.js");
__webpack_require__("./node_modules/rxjs/rx.js");
var Observable_1 = __webpack_require__("./node_modules/rxjs/_esm5/Observable.js");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var parameters_1 = __webpack_require__("./src/app/shared/service/api/parameters.ts");
var AreaService = (function () {
    function AreaService(http, ftHelperService) {
        this.http = http;
        this.ftHelperService = ftHelperService;
        /** Observables for calls that have previously been made */
        this.observables = {};
        this.baseUrl = this.ftHelperService.getURL().bridge;
        this.version = this.ftHelperService.version();
    }
    AreaService.prototype.getAreaSearchByText = function (text, areaTypeId, shouldSearchRetreiveCoordinates, parentAreasToIncludeInResults) {
        var params = new parameters_1.Parameters(this.version);
        params.addPolygonAreaTypeId(areaTypeId);
        params.addNoCache(true);
        params.addIncludeCoordinates(shouldSearchRetreiveCoordinates);
        params.addParentAreasToIncludeInResults(parentAreasToIncludeInResults);
        params.addSearchText(text);
        return this.getObservable("api/area_search_by_text", params);
    };
    AreaService.prototype.getAreaSearchByProximity = function (easting, northing, areaTypeId) {
        var params = new parameters_1.Parameters(this.version);
        params.addAreaTypeId(areaTypeId);
        params.addEasting(easting);
        params.addNorhing(northing);
        return this.getObservable("api/area_search_by_proximity", params);
    };
    AreaService.prototype.getAreaAddressesByParentAreaCode = function (parentAreaCode, areaTypeId) {
        var params = new parameters_1.Parameters(this.version);
        params.addAreaTypeId(areaTypeId);
        params.addParentAreaCode(parentAreaCode);
        return this.getObservable("api/area_addresses/by_parent_area_code", params);
    };
    AreaService.prototype.getParentAreas = function (profileId) {
        var params = new parameters_1.Parameters(this.version);
        params.addProfileId(profileId);
        return this.getObservable("api/area_types/parent_area_types", params);
    };
    AreaService.prototype.getObservable = function (serviceUrl, params) {
        // Ensure paramaters is defined
        if (!params) {
            params = new parameters_1.Parameters(this.version);
        }
        // Check whether call has already been made and cached observable is available
        var parameterString = params.getParameterString();
        var serviceKey = serviceUrl + parameterString;
        if (this.observables[serviceKey]) {
            return this.observables[serviceKey];
        }
        var observable = this.http.get(this.baseUrl + serviceUrl, params.getRequestOptions())
            .publishReplay(1).refCount() // Call once then use same response for repeats
            .map(function (res) { return res.json(); })
            .catch(this.handleError);
        this.observables[serviceKey] = observable;
        return observable;
    };
    AreaService.prototype.handleError = function (error) {
        console.error(error);
        var errorMessage = "AJAX call failed";
        return Observable_1.Observable.throw(errorMessage);
    };
    return AreaService;
}());
AreaService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [typeof (_a = typeof http_1.Http !== "undefined" && http_1.Http) === "function" && _a || Object, typeof (_b = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _b || Object])
], AreaService);
exports.AreaService = AreaService;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/area.service.js.map

/***/ }),

/***/ "./src/app/shared/service/api/indicator.service.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var http_1 = __webpack_require__("./node_modules/@angular/http/@angular/http.es5.js");
__webpack_require__("./node_modules/rxjs/_esm5/add/operator/map.js");
__webpack_require__("./node_modules/rxjs/_esm5/add/operator/publishReplay.js");
__webpack_require__("./node_modules/rxjs/_esm5/add/operator/catch.js");
var Observable_1 = __webpack_require__("./node_modules/rxjs/_esm5/Observable.js");
var parameters_1 = __webpack_require__("./src/app/shared/service/api/parameters.ts");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var IndicatorService = (function () {
    function IndicatorService(http, ftHelperService) {
        this.http = http;
        this.ftHelperService = ftHelperService;
        /** Observables for calls that have previously been made */
        this.observables = {};
        this.baseUrl = this.ftHelperService.getURL().bridge;
        /** The version number of the static assets, e.g. JS */
        this.version = this.ftHelperService.version();
        this.search = this.ftHelperService.getSearch();
    }
    IndicatorService.prototype.getSingleIndicatorForAllArea = function (groupId, areaTypeId, parentAreaCode, profileId, comparatorId, indicatorId, sexId, ageId) {
        var params = new parameters_1.Parameters(this.version);
        params.addGroupId(groupId);
        params.addAreaTypeId(areaTypeId);
        params.addParentAreaCode(parentAreaCode);
        params.addProfileId(profileId);
        params.addComparatorId(comparatorId);
        params.addIndicatorId(indicatorId);
        params.addSexId(sexId);
        params.addAgeId(ageId);
        return this.getObservable('api/latest_data/single_indicator_for_all_areas', params);
    };
    IndicatorService.prototype.getCategories = function (categoryTypeId) {
        var params = new parameters_1.Parameters(this.version);
        params.addCategoryTypeId(categoryTypeId);
        return this.getObservable('api/categories', params);
    };
    IndicatorService.prototype.getPopulationSummary = function (areaCode, areaTypeId) {
        var params = new parameters_1.Parameters(this.version);
        params.addAreaCode(areaCode);
        params.addAreaTypeId(areaTypeId);
        return this.getObservable('api/quinary_population_summary', params);
    };
    IndicatorService.prototype.getPopulation = function (areaCode, areaTypeId) {
        var params = new parameters_1.Parameters(this.version);
        params.addAreaCode(areaCode);
        params.addAreaTypeId(areaTypeId);
        return this.getObservable('api/quinary_population', params);
    };
    IndicatorService.prototype.getBenchmarkingMethod = function (benchmarkingMethodId) {
        var params = new parameters_1.Parameters(this.version);
        params.addId(benchmarkingMethodId);
        return this.getObservable('api/comparator_method', params);
    };
    IndicatorService.prototype.getIndicatorMetadataProperties = function () {
        return this.getObservable('api/indicator_metadata_text_properties');
    };
    IndicatorService.prototype.getIndicatorStatisticsTrendsForSingleIndicator = function (indicatorId, sexId, ageId, childAreaTypeId, parentAreaCode) {
        var params = new parameters_1.Parameters(this.version);
        params.addIndicatorId(indicatorId);
        params.addSexId(sexId);
        params.addAgeId(ageId);
        params.addChildAreaTypeId(childAreaTypeId);
        params.addParentAreaCode(parentAreaCode);
        return this.getObservable('api/indicator_statistics/trends_for_single_indicator', params);
    };
    IndicatorService.prototype.getIndicatorMetadata = function (groupId) {
        var params = new parameters_1.Parameters(this.version);
        params.addIncludeSystemContent('no');
        params.addIncludeDefinition('yes');
        var method;
        if (this.search.isInSearchMode()) {
            method = 'by_indicator_id';
            this.addSearchParameters(params);
        }
        else {
            method = 'by_group_id';
            params.addGroupIds(groupId);
        }
        var serviceURL = 'api/indicator_metadata/' + method;
        return this.getObservable(serviceURL, params);
    };
    IndicatorService.prototype.getIndicatorMetadataByIndicatorId = function (indicatorId, restrictToProfileIds) {
        var params = new parameters_1.Parameters(this.version);
        params.addIncludeSystemContent('no');
        params.addIncludeDefinition('yes');
        params.addIndicatorIds([indicatorId]);
        params.addRestrictToProfileIds(restrictToProfileIds);
        return this.getObservable('api/indicator_metadata/by_indicator_id', params);
    };
    IndicatorService.prototype.getLatestDataForAllIndicatorsInProfileGroupForChildAreas = function (groupId, areaTypeId, parentAreaCode, profileId) {
        var params = new parameters_1.Parameters(this.version);
        params.addAreaTypeId(areaTypeId);
        params.addParentAreaCode(parentAreaCode);
        params.addProfileId(profileId);
        var method;
        if (this.search.isInSearchMode()) {
            if (this.search.isIndicatorList()) {
                params.addIndicatorListId(this.search.getIndicatorListId());
                method = 'indicator_list_for_child_areas';
            }
            else {
                this.addSearchParameters(params);
                method = 'specific_indicators_for_child_areas';
            }
        }
        else {
            params.addGroupId(groupId);
            method = 'all_indicators_in_profile_group_for_child_areas';
        }
        var serviceURL = 'api/latest_data/' + method;
        return this.getObservable(serviceURL, params);
    };
    IndicatorService.prototype.getObservable = function (serviceUrl, params) {
        // Ensure paramaters is defined
        if (!params) {
            params = new parameters_1.Parameters(this.version);
        }
        // Check whether call has already been made and cached observable is available
        var parameterString = params.getParameterString();
        var serviceKey = serviceUrl + parameterString;
        if (this.observables[serviceKey]) {
            return this.observables[serviceKey];
        }
        var observable = this.http.get(this.baseUrl + serviceUrl, params.getRequestOptions())
            .publishReplay(1).refCount() // Call once then use same response for repeats
            .map(function (res) { return res.json(); })
            .catch(this.handleError);
        this.observables[serviceKey] = observable;
        return observable;
    };
    IndicatorService.prototype.addSearchParameters = function (params) {
        params.addIndicatorIds(this.search.getIndicatorIdList().getAllIds());
        params.addRestrictToProfileIds(this.search.getProfileIdsForSearch());
    };
    IndicatorService.prototype.handleError = function (error) {
        console.error(error);
        var errorMessage = 'AJAX call failed';
        return Observable_1.Observable.throw(errorMessage);
    };
    IndicatorService.prototype.handleBoundariesError = function (error) {
        console.error(error);
        var errorMessage = 'Unsupported map type. Maps are not available for ' +
            this.ftHelperService.getAreaTypeName();
        return Observable_1.Observable.throw(errorMessage);
    };
    return IndicatorService;
}());
IndicatorService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [typeof (_a = typeof http_1.Http !== "undefined" && http_1.Http) === "function" && _a || Object, typeof (_b = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _b || Object])
], IndicatorService);
exports.IndicatorService = IndicatorService;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/indicator.service.js.map

/***/ }),

/***/ "./src/app/shared/service/api/parameters.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var http_1 = __webpack_require__("./node_modules/@angular/http/@angular/http.es5.js");
var Parameters = (function () {
    function Parameters(version) {
        this.params = new http_1.URLSearchParams();
        // Version included for cache busting between deployments
        this.params.set('v', version);
    }
    Parameters.prototype.getRequestOptions = function () {
        var requestOptions = new http_1.RequestOptions({
            headers: new http_1.Headers({ 'Content-Type': 'application/json' })
        });
        requestOptions.search = this.params;
        return requestOptions;
    };
    /** Returns a concaternated string of the URL parameters */
    Parameters.prototype.getParameterString = function () {
        return this.params.toString();
    };
    Parameters.prototype.addId = function (id) {
        this.params.set('id', id.toString());
    };
    Parameters.prototype.addGroupId = function (groupId) {
        this.params.set('group_id', groupId.toString());
    };
    Parameters.prototype.addGroupIds = function (groupId) {
        this.params.set('group_ids', groupId.toString());
    };
    Parameters.prototype.addProfileId = function (profileId) {
        this.params.set('profile_id', profileId.toString());
    };
    Parameters.prototype.addAreaTypeId = function (areaTypeId) {
        this.params.set('area_type_id', areaTypeId.toString());
    };
    Parameters.prototype.addChildAreaTypeId = function (childAreaTypeId) {
        this.params.set('child_area_type_id', childAreaTypeId.toString());
    };
    Parameters.prototype.addParentAreaCode = function (areaCode) {
        this.params.set('parent_area_code', areaCode);
    };
    Parameters.prototype.addAreaCode = function (areaCode) {
        this.params.set('area_code', areaCode);
    };
    Parameters.prototype.addComparatorId = function (comparatorId) {
        this.params.set('comparator_id', comparatorId.toString());
    };
    Parameters.prototype.addIndicatorId = function (indicatorId) {
        this.params.set('indicator_id', indicatorId.toString());
    };
    Parameters.prototype.addCategoryTypeId = function (categoryTypeId) {
        this.params.set('category_type_id', categoryTypeId.toString());
    };
    Parameters.prototype.addSexId = function (sexId) {
        this.params.set('sex_id', sexId.toString());
    };
    Parameters.prototype.addAgeId = function (ageId) {
        this.params.set('age_id', ageId.toString());
    };
    Parameters.prototype.addIndicatorListId = function (indicatorListId) {
        this.params.set('indicator_list_id', indicatorListId.toString());
    };
    Parameters.prototype.addIncludeSystemContent = function (yesOrNo) {
        this.params.set('include_system_content', yesOrNo);
    };
    Parameters.prototype.addIncludeDefinition = function (yesOrNo) {
        this.params.set('include_definition', yesOrNo);
    };
    Parameters.prototype.addRestrictToProfileIds = function (profileIds) {
        this.params.set('restrict_to_profile_ids', profileIds.join(','));
    };
    Parameters.prototype.addIndicatorIds = function (indicatorIds) {
        this.params.set('indicator_ids', indicatorIds.join(','));
    };
    Parameters.prototype.addSearchText = function (searchText) {
        this.params.set('search_text', searchText);
    };
    Parameters.prototype.addNoCache = function (noCache) {
        this.params.set('no_cache', noCache.toString());
    };
    Parameters.prototype.addIncludeCoordinates = function (includeCoordinates) {
        this.params.set('include_coordinates', includeCoordinates.toString());
    };
    Parameters.prototype.addParentAreasToIncludeInResults = function (parentAreasToIncludeInResults) {
        this.params.set('parent_areas_to_include_in_results', parentAreasToIncludeInResults.toString());
    };
    Parameters.prototype.addPolygonAreaTypeId = function (polygonAreaTypeId) {
        this.params.set('polygon_area_type_id', polygonAreaTypeId.toString());
    };
    Parameters.prototype.addEasting = function (easting) {
        this.params.set('easting', easting.toString());
    };
    Parameters.prototype.addNorhing = function (northing) {
        this.params.set('northing', northing.toString());
    };
    return Parameters;
}());
exports.Parameters = Parameters;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/parameters.js.map

/***/ }),

/***/ "./src/app/shared/service/helper/coreDataHelper.service.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var CoreDataHelperService = (function () {
    function CoreDataHelperService() {
    }
    CoreDataHelperService.prototype.addOrderandPercentilesToData = function (coreDataSet) {
        return FTWrapper.coreDataHelper.addOrderandPercentilesToData(coreDataSet);
    };
    CoreDataHelperService.prototype.valueWithUnit = function (unit) {
        return FTWrapper.coreDataHelper.valueWithUnit(unit);
    };
    CoreDataHelperService.prototype.getFullLabel = function (value, options) {
        return FTWrapper.valueWithUnit.getFullLabel(value, options);
    };
    return CoreDataHelperService;
}());
CoreDataHelperService = __decorate([
    core_1.Injectable()
], CoreDataHelperService);
exports.CoreDataHelperService = CoreDataHelperService;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/coreDataHelper.service.js.map

/***/ }),

/***/ "./src/app/shared/service/helper/ftHelper.service.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var FTHelperService = (function () {
    function FTHelperService() {
    }
    FTHelperService.prototype.getAreaName = function (areaCode) {
        return FTWrapper.display.getAreaName(areaCode);
    };
    FTHelperService.prototype.getAreaTypeName = function () {
        return FTWrapper.display.getAreaTypeName();
    };
    FTHelperService.prototype.getAreaList = function () {
        return FTWrapper.display.getAreaList();
    };
    FTHelperService.prototype.getComparatorId = function () {
        return FTWrapper.display.getComparatorId();
    };
    FTHelperService.prototype.getCurrentComparator = function () {
        return FTWrapper.bridgeDataHelper.getCurrentComparator();
    };
    FTHelperService.prototype.getCurrentGroupRoot = function () {
        return FTWrapper.bridgeDataHelper.getGroopRoot();
    };
    FTHelperService.prototype.getAllGroupRoots = function () {
        return FTWrapper.bridgeDataHelper.getAllGroupRoots();
    };
    FTHelperService.prototype.getValueNotes = function () {
        return FTWrapper.display.getValueNotes();
    };
    FTHelperService.prototype.getValueNoteById = function (id) {
        return FTWrapper.display.getValueNoteById(id);
    };
    FTHelperService.prototype.formatCount = function (dataInfo) {
        return FTWrapper.formatCount(dataInfo);
    };
    FTHelperService.prototype.newCoreDataSetInfo = function (data) {
        return FTWrapper.newCoreDataSetInfo(data);
    };
    FTHelperService.prototype.newValueDisplayer = function (unit) {
        return FTWrapper.newValueDisplayer(unit);
    };
    FTHelperService.prototype.newCommaNumber = function (n) {
        return FTWrapper.newCommaNumber(n);
    };
    FTHelperService.prototype.newValueNoteTooltipProvider = function () {
        return FTWrapper.newValueNoteTooltipProvider();
    };
    FTHelperService.prototype.newTooltipManager = function () {
        return FTWrapper.newTooltipManager();
    };
    FTHelperService.prototype.newRecentTrendsTooltip = function () {
        return FTWrapper.newRecentTrendsTooltip();
    };
    FTHelperService.prototype.newComparisonConfig = function (groupRoot, indicatorMetadata) {
        return FTWrapper.newComparisonConfig(groupRoot, indicatorMetadata);
    };
    FTHelperService.prototype.getNationalComparatorGrouping = function (root) {
        return FTWrapper.getNationalComparatorGrouping(root);
    };
    FTHelperService.prototype.getFTConfig = function () {
        return FTWrapper.config();
    };
    FTHelperService.prototype.getFTModel = function () {
        return FTWrapper.model();
    };
    FTHelperService.prototype.getMetadata = function (IID) {
        return FTWrapper.indicatorHelper.getMetadataHash()[IID];
    };
    FTHelperService.prototype.getMetadataHash = function () {
        return FTWrapper.indicatorHelper.getMetadataHash();
    };
    FTHelperService.prototype.getIndicatorIndex = function () {
        return FTWrapper.indicatorHelper.getIndicatorIndex();
    };
    FTHelperService.prototype.getParentArea = function () {
        return FTWrapper.getParentArea();
    };
    FTHelperService.prototype.getSearch = function () {
        return FTWrapper.search;
    };
    FTHelperService.prototype.getSexAndAgeLabel = function (groupRoot) {
        return FTWrapper.getSexAndAgeLabel(groupRoot);
    };
    /** Returns IMG HTML for a recent trend */
    FTHelperService.prototype.getTrendMarkerImage = function (trendMarker, polarity) {
        return FTWrapper.getTrendMarkerImage(trendMarker, polarity);
    };
    FTHelperService.prototype.getURL = function () {
        return FTWrapper.url();
    };
    FTHelperService.prototype.getIndicatorDataQualityHtml = function (text) {
        return FTWrapper.getIndicatorDataQualityHtml(text);
    };
    FTHelperService.prototype.getIndicatorDataQualityTooltipText = function (dataQualityCount) {
        return FTWrapper.getIndicatorDataQualityTooltipText(dataQualityCount);
    };
    FTHelperService.prototype.goToMetadataPage = function (rootIndex) {
        FTWrapper.goToMetadataPage(rootIndex);
    };
    FTHelperService.prototype.recentTrendSelected = function () {
        return FTWrapper.recentTrendSelected();
    };
    FTHelperService.prototype.setAreaCode = function (areaCode) {
        FTWrapper.setAreaCode(areaCode);
    };
    FTHelperService.prototype.showIndicatorMetadataInLightbox = function (element) {
        FTWrapper.showIndicatorMetadataInLightbox(element);
    };
    FTHelperService.prototype.showAndHidePageElements = function () {
        FTWrapper.showAndHidePageElements();
    };
    FTHelperService.prototype.showTargetBenchmarkOption = function (roots) {
        FTWrapper.showTargetBenchmarkOption(roots);
    };
    FTHelperService.prototype.getTargetLegendHtml = function (comparisonConfig, metadata) {
        return FTWrapper.getTargetLegendHtml(comparisonConfig, metadata);
    };
    FTHelperService.prototype.lock = function () {
        FTWrapper.lock();
    };
    FTHelperService.prototype.unlock = function () {
        FTWrapper.unlock();
    };
    /** The version number of the static assets, e.g. JS */
    FTHelperService.prototype.version = function () {
        return FTWrapper.version();
    };
    FTHelperService.prototype.saveElementAsImage = function (element, outputFilename) {
        return FTWrapper.saveElementAsImage(element, outputFilename);
    };
    FTHelperService.prototype.redirectToPopulationPage = function () {
        return FTWrapper.redirectToPopulationPage();
    };
    FTHelperService.prototype.isValuePresent = function (val) {
        return val !== undefined && val !== '-' && val !== '';
    };
    FTHelperService.prototype.logEvent = function (category, action, label) {
        if (label === void 0) { label = null; }
        FTWrapper.logEvent(category, action, label);
    };
    return FTHelperService;
}());
FTHelperService = __decorate([
    core_1.Injectable()
], FTHelperService);
exports.FTHelperService = FTHelperService;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/ftHelper.service.js.map

/***/ }),

/***/ "./src/app/shared/service/helper/script.service.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var Observable_1 = __webpack_require__("./node_modules/rxjs/_esm5/Observable.js");
var ScriptLoaderService = (function () {
    function ScriptLoaderService() {
        this.scripts = [];
    }
    ScriptLoaderService.prototype.load = function (script) {
        var _this = this;
        return new Observable_1.Observable(function (observer) {
            var existingScript = _this.scripts.find(function (s) { return s.name === script.name; });
            // Complete if already loaded
            if (existingScript && existingScript.loaded) {
                observer.next(existingScript);
                observer.complete();
            }
            else {
                // Add the script
                _this.scripts = _this.scripts.concat([script]);
                // Load the script
                var scriptElement = document.createElement('script');
                //scriptElement.baseURI = '';
                scriptElement.type = 'text/javascript';
                scriptElement.src = script.src;
                scriptElement.onload = function () {
                    script.loaded = true;
                    observer.next(script);
                    observer.complete();
                };
                scriptElement.onerror = function (error) {
                    observer.error('Couldn\'t load script ' + script.src);
                };
                document.getElementsByTagName('body')[0].appendChild(scriptElement);
            }
        });
    };
    return ScriptLoaderService;
}());
ScriptLoaderService = __decorate([
    core_1.Injectable()
], ScriptLoaderService);
exports.ScriptLoaderService = ScriptLoaderService;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/script.service.js.map

/***/ }),

/***/ "./src/app/shared/shared.module.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var common_1 = __webpack_require__("./node_modules/@angular/common/@angular/common.es5.js");
var indicator_header_component_1 = __webpack_require__("./src/app/shared/component/indicator-header/indicator-header.component.ts");
var SharedModule = (function () {
    function SharedModule() {
    }
    return SharedModule;
}());
SharedModule = __decorate([
    core_1.NgModule({
        imports: [
            common_1.CommonModule
        ],
        declarations: [
            indicator_header_component_1.IndicatorHeaderComponent
        ],
        exports: [
            indicator_header_component_1.IndicatorHeaderComponent
        ]
    })
], SharedModule);
exports.SharedModule = SharedModule;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/shared.module.js.map

/***/ }),

/***/ "./src/app/shared/shared.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var TooltipHelper = (function () {
    function TooltipHelper(tooltipManager) {
        this.tooltipManager = tooltipManager;
    }
    TooltipHelper.prototype.displayHtml = function (event, html) {
        this.tooltipManager.setHtml(html);
        this.tooltipManager.positionXY(event.pageX + 10, event.pageY + 15);
        this.tooltipManager.showOnly();
    };
    TooltipHelper.prototype.reposition = function (event) {
        this.tooltipManager.positionXY(event.pageX + 10, event.pageY + 15);
    };
    TooltipHelper.prototype.hide = function () {
        this.tooltipManager.hide();
    };
    return TooltipHelper;
}());
exports.TooltipHelper = TooltipHelper;
var AreaCodes = (function () {
    function AreaCodes() {
    }
    return AreaCodes;
}());
AreaCodes.England = "E92000001";
exports.AreaCodes = AreaCodes;
var SexIds = (function () {
    function SexIds() {
    }
    return SexIds;
}());
SexIds.Male = 1;
SexIds.Female = 2;
SexIds.Person = 4;
exports.SexIds = SexIds;
/** Highcharts helper */
var HC = (function () {
    function HC() {
    }
    return HC;
}());
HC.credits = { enabled: false };
HC.noLineMarker = { enabled: false, symbol: 'x' };
exports.HC = HC;
var ComparatorIds = (function () {
    function ComparatorIds() {
    }
    return ComparatorIds;
}());
ComparatorIds.national = 4;
ComparatorIds.sub_national = 1;
exports.ComparatorIds = ComparatorIds;
var AreaTypeIds = (function () {
    function AreaTypeIds() {
    }
    return AreaTypeIds;
}());
AreaTypeIds.District = 1;
AreaTypeIds.Region = 6;
AreaTypeIds.Practice = 7;
AreaTypeIds.County = 9;
AreaTypeIds.AcuteTrust = 14;
AreaTypeIds.Country = 15;
AreaTypeIds.UnitaryAuthority = 16;
AreaTypeIds.GpShape = 18;
AreaTypeIds.DeprivationDecile = 23;
AreaTypeIds.Subregion = 46;
AreaTypeIds.DistrictUA = 101;
AreaTypeIds.CountyUA = 102;
AreaTypeIds.PheCentres2013 = 103;
AreaTypeIds.PheCentres2015 = 104;
AreaTypeIds.OnsClusterGroup = 110;
AreaTypeIds.Scn = 112;
AreaTypeIds.AcuteTrusts = 118;
AreaTypeIds.Stp = 120;
AreaTypeIds.CombinedAuthorities = 126;
AreaTypeIds.CcgSinceApr2017 = 152;
AreaTypeIds.CcgPreApr2017 = 153;
AreaTypeIds.CcgSinceApr2018 = 154;
exports.AreaTypeIds = AreaTypeIds;
;
var IndicatorIds = (function () {
    function IndicatorIds() {
    }
    return IndicatorIds;
}());
IndicatorIds.QofListSize = 114;
IndicatorIds.QofPoints = 295;
IndicatorIds.WouldRecommendPractice = 347;
IndicatorIds.LifeExpectancy = 650;
IndicatorIds.EthnicityEstimates = 1679;
IndicatorIds.DeprivationScore = 91872;
exports.IndicatorIds = IndicatorIds;
var CategoryTypeIds = (function () {
    function CategoryTypeIds() {
    }
    return CategoryTypeIds;
}());
CategoryTypeIds.DeprivationDecileCountyUA2015 = 39;
CategoryTypeIds.DeprivationDecileDistrictUA2015 = 40;
CategoryTypeIds.HealthProfilesSSILimit = 5;
CategoryTypeIds.DeprivationDecileCCG2010 = 11;
CategoryTypeIds.DeprivationDecileGp2015 = 38;
exports.CategoryTypeIds = CategoryTypeIds;
var ProfileIds = (function () {
    function ProfileIds() {
    }
    return ProfileIds;
}());
ProfileIds.PracticeProfileSupportingIndicators = 21;
ProfileIds.PracticeProfile = 20;
exports.ProfileIds = ProfileIds;
var CommaNumber = (function () {
    function CommaNumber(value) {
        this.value = value;
    }
    CommaNumber.prototype.commarate = function (value) {
        return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    };
    /**
    * Rounds the number to 0 decimal places before formatting with commas
    */
    CommaNumber.prototype.rounded = function () {
        return this.commarate(Math.round(this.value));
    };
    CommaNumber.prototype.unrounded = function () {
        var commaNumber = this.value.toString();
        if (commaNumber.indexOf('.') > -1) {
            var bits = commaNumber.split('.');
            return this.commarate(bits[0]) + '.' + bits[1];
        }
        return this.commarate(this.value);
    };
    return CommaNumber;
}());
exports.CommaNumber = CommaNumber;
;
var NumberHelper = (function () {
    function NumberHelper() {
    }
    /**
     * Rounds a number to a fixed amount of decimal places
     * @param num Number to round
     * @param dec Number of decimal place to round to
     */
    NumberHelper.roundNumber = function (num, dec) {
        return Math.round(num * Math.pow(10, dec)) / Math.pow(10, dec);
    };
    ;
    return NumberHelper;
}());
exports.NumberHelper = NumberHelper;
var ComparatorMethodIds = (function () {
    function ComparatorMethodIds() {
    }
    return ComparatorMethodIds;
}());
ComparatorMethodIds.SuicidePlan = 14;
ComparatorMethodIds.Quintiles = 15;
exports.ComparatorMethodIds = ComparatorMethodIds;
var PolarityIds = (function () {
    function PolarityIds() {
    }
    return PolarityIds;
}());
PolarityIds.RAGLowIsGood = 0;
PolarityIds.RAGHighIsGood = 1;
PolarityIds.Quintiles = 15;
PolarityIds.BlueOrangeBlue = 99;
exports.PolarityIds = PolarityIds;
var Colour = (function () {
    function Colour() {
    }
    Colour.getSignificanceColorForBenchmark = function (groupRoot, comparisonConfig, sig) {
        var polarityId = groupRoot.PolarityId;
        // Quintiles
        if (comparisonConfig.useQuintileColouring ||
            groupRoot.ComparatorMethodId === ComparatorMethodIds.Quintiles) {
            if (sig > 0 && sig < 6) {
                var quintile = 'quintile' + sig;
                switch (quintile) {
                    case 'quintile1': {
                        return Colour.quintile1;
                    }
                    case 'quintile2': {
                        return Colour.quintile2;
                    }
                    case 'quintile3': {
                        return Colour.quintile3;
                    }
                    case 'quintile4': {
                        return Colour.quintile4;
                    }
                    case 'quintile5': {
                        return Colour.quintile5;
                    }
                }
            }
            else {
                return Colour.noComparison;
            }
        }
        // No colour comparison should be made
        if (polarityId === -1) {
            return Colour.noComparison;
        }
        // Blues
        if (polarityId === PolarityIds.BlueOrangeBlue) {
            return sig === 1 ? Colour.bobLower :
                sig === 2 ? Colour.same :
                    sig === 3 ? Colour.bobHigher :
                        Colour.noComparison;
        }
        // RAG
        return sig === 1 ? Colour.worse :
            sig === 2 ? Colour.same :
                sig === 3 ? Colour.better :
                    Colour.noComparison;
    };
    Colour.getColorForQuartile = function (quartile) {
        switch (quartile) {
            case 1:
                return '#E8C7D1';
            case 2:
                return '#B74D6D';
            case 3:
                return '#98002E';
            case 4:
            case 5:
                return '#700023';
            default:
                return '#B0B0B2';
        }
    };
    Colour.getColorForQuintile = function (quintile) {
        switch (quintile) {
            case 1:
                return Colour.quintile1;
            case 2:
                return Colour.quintile2;
            case 3:
                return Colour.quintile3;
            case 4:
                return Colour.quintile4;
            case 5:
            case 6:
                return Colour.quintile5;
            default:
                return '#B0B0B2';
        }
    };
    Colour.getColorForContinuous = function (orderFrac) {
        if (orderFrac === -1) {
            return '#B0B0B2';
        }
        var seed = orderFrac;
        var r = 21;
        var g = 28;
        var b = 85;
        r = 255 - Math.round(seed * (255 - r));
        g = 233 - Math.round(seed * (233 - g));
        b = 127 - Math.round(seed * (127 - b));
        return 'rgb(' + r + ',' + g + ',' + b + ')';
    };
    return Colour;
}());
Colour.chart = '#a8a8cc';
Colour.better = '#92d050';
Colour.same = '#ffc000';
Colour.worse = '#c00000';
Colour.none = '#ffffff';
Colour.limit99 = '#a8a8cc';
Colour.limit95 = '#444444';
Colour.border = '#666666';
Colour.comparator = '#000000';
Colour.bobLower = '#5555E6';
Colour.bobHigher = '#C2CCFF';
Colour.bodyText = '#333';
Colour.noComparison = '#c9c9c9';
Colour.quintile1 = '#DED3EC';
Colour.quintile2 = '#BEA7DA';
Colour.quintile3 = '#9E7CC8';
Colour.quintile4 = '#7E50B6';
Colour.quintile5 = '#5E25A4';
exports.Colour = Colour;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/shared.js.map

/***/ }),

/***/ "./src/environments/environment.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `angular-cli.json`.
Object.defineProperty(exports, "__esModule", { value: true });
exports.environment = {
    production: false
};
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/environment.js.map

/***/ }),

/***/ "./src/main.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var platform_browser_dynamic_1 = __webpack_require__("./node_modules/@angular/platform-browser-dynamic/@angular/platform-browser-dynamic.es5.js");
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var environment_1 = __webpack_require__("./src/environments/environment.ts");
var app_module_1 = __webpack_require__("./src/app/app.module.ts");
if (environment_1.environment.production) {
    core_1.enableProdMode();
}
platform_browser_dynamic_1.platformBrowserDynamic().bootstrapModule(app_module_1.AppModule);
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/main.js.map

/***/ }),

/***/ 0:
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__("./src/main.ts");


/***/ })

},[0]);
//# sourceMappingURL=main.bundle.js.map