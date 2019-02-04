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
var ngx_bootstrap_1 = __webpack_require__("./node_modules/ngx-bootstrap/index.js");
var map_component_1 = __webpack_require__("./src/app/map/map.component.ts");
var map_module_1 = __webpack_require__("./src/app/map/map.module.ts");
var shared_module_1 = __webpack_require__("./src/app/shared/shared.module.ts");
var boxplot_module_1 = __webpack_require__("./src/app/boxplot/boxplot.module.ts");
var metadata_module_1 = __webpack_require__("./src/app/metadata/metadata.module.ts");
var population_module_1 = __webpack_require__("./src/app/population/population.module.ts");
var arealist_module_1 = __webpack_require__("./src/app/arealist/arealist.module.ts");
var arealist_index_component_1 = __webpack_require__("./src/app/arealist/arealist-index/arealist-index.component.ts");
var arealist_manage_component_1 = __webpack_require__("./src/app/arealist/arealist-manage/arealist-manage.component.ts");
var download_module_1 = __webpack_require__("./src/app/download/download.module.ts");
var data_view_component_1 = __webpack_require__("./src/app/data-view/data-view.component.ts");
var reports_component_1 = __webpack_require__("./src/app/reports/reports.component.ts");
var area_profile_module_1 = __webpack_require__("./src/app/area-profile/area-profile.module.ts");
var england_module_1 = __webpack_require__("./src/app/england/england.module.ts");
var rootComponents = [map_component_1.MapComponent, arealist_index_component_1.ArealistIndexComponent, arealist_manage_component_1.ArealistManageComponent,
    data_view_component_1.DataViewComponent];
var AppModule = (function () {
    function AppModule(resolver) {
        this.resolver = resolver;
    }
    // Overriding angular original behaviour, This will bootsttrap all the component defined
    // in the array if the component tag is found on the document
    AppModule.prototype.ngDoBootstrap = function (appRef) {
        var _this = this;
        rootComponents.forEach(function (componentDef) {
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
            data_view_component_1.DataViewComponent,
            reports_component_1.ReportsComponent,
        ],
        imports: [
            platform_browser_1.BrowserModule,
            forms_1.FormsModule,
            forms_1.ReactiveFormsModule,
            http_1.HttpModule,
            map_module_1.MapModule,
            boxplot_module_1.BoxplotModule,
            download_module_1.DownloadModule,
            population_module_1.PopulationModule,
            area_profile_module_1.AreaProfileModule,
            shared_module_1.SharedModule,
            metadata_module_1.MetadataModule,
            arealist_module_1.ArealistModule,
            england_module_1.EnglandModule,
            ngx_bootstrap_1.TypeaheadModule.forRoot(),
        ],
        entryComponents: rootComponents
        // Instead of bootstraping a single component, with overriding mechanism, multiple components are bootstrapped
        // bootstrap: [AppComponent]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof core_1.ComponentFactoryResolver !== "undefined" && core_1.ComponentFactoryResolver) === "function" && _a || Object])
], AppModule);
exports.AppModule = AppModule;
var _a;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/app.module.js.map

/***/ }),

/***/ "./src/app/area-profile/area-profile.component.css":
/***/ (function(module, exports) {

module.exports = "/* Indicator name */\r\n\r\ntr td:first-child {\r\n    color: #2e3191;\r\n    cursor: pointer;\r\n}\r\n\r\n/* Period header */\r\n\r\ntr th:nth-child(2) {\r\n    text-align: center;\r\n}\r\n\r\n/* Period */\r\n\r\ntr td:nth-child(2) {\r\n    text-align: center;\r\n}\r\n\r\n/* Recent trendn */\r\n\r\ntr td:nth-child(3) {\r\n    text-align: center;\r\n    cursor: pointer;\r\n}\r\n\r\ntr:hover td {\r\n    border-top-color: #333;\r\n    border-bottom-color: #333;\r\n    background: #fffdea;\r\n}\r\n\r\ntr:hover td:first-child {\r\n    border-left-color: #333;\r\n}\r\n\r\ntr:hover td:last-child {\r\n    border-right-color: #333;\r\n}\r\n\r\n#england-header {\r\n    position: relative;\r\n    min-width: 386px;\r\n}\r\n\r\n.export-chart-box, .export-chart-box-csv {\r\n    padding-top: 20px;\r\n}\r\n\r\n#single-area-table {\r\n    table-layout: auto;\r\n    margin: 8px auto 30px auto;\r\n}\r\n\r\n#no-spine-box {\r\n    width: 233px;\r\n    top: 94px;\r\n    left: 75px;\r\n    line-height: 1.5em;\r\n    font-weight: normal;\r\n    position: absolute;\r\n    border: 2px solid #ddd;\r\n    background: #FFFFF0;\r\n    z-index: 10;\r\n    -webkit-box-shadow: 0 0 2px #888;\r\n            box-shadow: 0 0 2px #888;\r\n}\r\n\r\n#no-spine-box p {\r\n    padding: 5px;\r\n    margin: 0;\r\n}"

/***/ }),

/***/ "./src/app/area-profile/area-profile.component.html":
/***/ (function(module, exports) {

module.exports = "<div *ngIf=\"isAnyData\" id=\"area-profile-container\" style=\"display:none;\">\r\n    <div class=\"row\">\r\n        <div class=\"col-md-12\">\r\n            <ft-legend [pageType]=\"pageType\" [showRAG3]=\"showRAG3\" [showRAG5]=\"showRAG5\" [showBOB]=\"showBOB\"\r\n                [showQuintilesRAG]=\"showQuintilesRAG\" [showQuintilesBOB]=\"showQuintilesBOB\" [showRecentTrends]=\"showRecentTrends\"></ft-legend>\r\n        </div>\r\n    </div>\r\n    <div class=\"row\">\r\n        <div class=\"export-chart-box col-md-3\">\r\n            <a class=\"export-link\" (click)=\"onExportClick($event)\">Export table as image</a>\r\n        </div>\r\n        <div class=\"export-chart-box-csv col-md-4\">\r\n            <a class=\"export-link-csv hidden\" (click)=\"onExportCsvFileClick($event)\">Export table as CSV file</a>\r\n        </div>\r\n        <div class=\"col-md-5 text-right\"> <img src=\"{{spineChartImageUrl}}\" />\r\n        </div>\r\n    </div>\r\n\r\n    <div class=\"row\">\r\n        <div class=\"col-md-12\">\r\n            <table id=\"single-area-table\" class=\"bordered-table\" cellspacing=\"0\" cellpadding=\"0\">\r\n                <thead>\r\n                    <tr>\r\n                        <th id=\"spine-indicator-header\" rowspan=\"2\">Indicator</th>\r\n                        <th id=\"spine-period-header\" rowspan=\"2\">Period</th>\r\n                        <th style=\"position: relative;\" class=\"numericHeader areaName topRow\" [attr.colspan]=\"trendColSpan\">\r\n                            <div>{{shortAreaName}}</div>&nbsp;\r\n                        </th>\r\n                        <th *ngIf=\"!isParentUk && isNationalAndRegional && isNotNN\" class=\"numericHeader topRow parent-area-type\">{{parentType}}</th>\r\n                        <th *ngIf=\"!isParentUk && isNationalAndRegional\" class=\"numericHeader topRow\" style=\"position:relative;\">England\r\n                        </th>\r\n                        <th id=\"england-header\" *ngIf=\"!isParentUk\" [attr.colspan]=\"colSpan\" class=\"numericHeader topRow\" style=\"width: 390px;\">{{benchmarkName}}\r\n                            <div *ngIf=\"isAreaIgnored\" id=\"no-spine-box\">\r\n                                <p>Due to its small population <i>{{areaName}}</i> is not used to determine the lowest,\r\n                                    highest and percentile values required for the spine charts. However the area value\r\n                                    is included in the <i>{{benchmarkName}}</i> average.</p>\r\n                            </div>\r\n                        </th>\r\n                    </tr>\r\n                    <tr>\r\n                        <th *ngIf=\"areRecentTrendsDisplayed\" class=\"numericHeader\">Recent Trend</th>\r\n                        <th class=\"numericHeader\">Count</th>\r\n                        <th class=\"numericHeader\">Value</th>\r\n                        <th *ngIf=\"!isParentUk && isNationalAndRegional && isNotNN\" class=\"numericHeader\">Value</th>\r\n                        <th *ngIf=\"!isParentUk\" class=\"numericHeader\">Value</th>\r\n                        <th *ngIf=\"!isParentUk\" class=\"numericHeader\">{{lowest}}</th>\r\n                        <th *ngIf=\"!isParentUk\" class=\"spine250\">Range</th>\r\n                        <th *ngIf=\"!isParentUk\" class=\"numericHeader\">{{highest}}</th>\r\n                    </tr>\r\n                </thead>\r\n                <tbody id=\"spine-body\">\r\n                    <tr *ngFor=\"let row of indicatorRows\">\r\n                        <td [innerHtml]=\"getIndicatorNameHtml(row)\" (click)=\"goToBarChart(row)\" (mouseover)=\"showIndicatorTooltip($event,row)\"\r\n                            (mouseout)=\"hideTooltip()\" (mousemove)=\"repositionTooltip($event)\"></td>\r\n                        <td [innerHtml]=\"row.period\"></td>\r\n                        <td *ngIf=\"areRecentTrendsDisplayed\" [innerHtml]=\"row.recentTrendImage\" (click)=\"goToTrends(row)\"\r\n                            (mouseover)=\"showRecentTrendTooltip($event,row)\" (mouseout)=\"hideTooltip()\" (mousemove)=\"repositionTooltip($event)\"></td>\r\n                        <td class=\"numeric\" [innerHtml]=\"row.areaCount\"></td>\r\n                        <td class=\"numeric\" [innerHtml]=\"row.areaValue\" data-toggle=\"tooltip\" data-placement=\"top\"\r\n                            title=\"{{row.areaValueTooltip}}\"></td>\r\n                        <td *ngIf=\"!isParentUk && isNationalAndRegional && isNotNN\" class=\"numeric\" [innerHtml]=\"row.subnationalValue\"\r\n                            data-toggle=\"tooltip\" data-placement=\"top\" title=\"{{row.subnationalValueTooltip}}\"></td>\r\n                        <td *ngIf=\"!isParentUk\" class=\"numeric\" [innerHtml]=\"row.englandValue\" data-toggle=\"tooltip\" data-placement=\"top\"\r\n                            title=\"{{row.englandValueTooltip}}\"></td>\r\n                        <td *ngIf=\"!isParentUk\" class=\"numeric\" data-toggle=\"tooltip\" data-placement=\"top\" [innerHtml]=\"row.englandMin\"\r\n                            title=\"{{row.minTooltip}}\"></td>\r\n                        <td *ngIf=\"!isParentUk\" >\r\n                            <ft-spine-chart [dimensions]=\"row.spineChartDimensions\" [tooltip]=\"tooltip\" [indicatorRow]=\"row\"></ft-spine-chart>\r\n                        </td>\r\n                        <td *ngIf=\"!isParentUk\" class=\"numeric\" data-toggle=\"tooltip\" data-placement=\"top\" [innerHtml]=\"row.englandMax\"\r\n                            title=\"{{row.maxTooltip}}\"></td>\r\n                    </tr>\r\n                </tbody>\r\n            </table>\r\n        </div>\r\n    </div>\r\n</div>"

/***/ }),

/***/ "./src/app/area-profile/area-profile.component.ts":
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
var Observable_1 = __webpack_require__("./node_modules/rxjs/_esm5/Observable.js");
var ui_service_1 = __webpack_require__("./src/app/shared/service/helper/ui.service.ts");
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var _ = __webpack_require__("./node_modules/underscore/underscore.js");
var spine_chart_classes_1 = __webpack_require__("./src/app/area-profile/spine-chart.classes.ts");
var legend_component_1 = __webpack_require__("./src/app/shared/component/legend/legend.component.ts");
var AreaProfileComponent = (function () {
    function AreaProfileComponent(ftHelperService, indicatorService, uiService) {
        this.ftHelperService = ftHelperService;
        this.indicatorService = indicatorService;
        this.uiService = uiService;
        this.areRecentTrendsDisplayed = false;
        this.spineChartImageUrl = null;
        this.isParentUk = false;
        this.isNationalAndRegional = false;
        this.isNotNN = false;
        this.trendColSpan = 0;
        this.colSpan = 0;
        this.isAnyData = false;
        // HTML
        this.NoData = '<div class="no-data">-</div>';
        // Legend display properties
        this.pageType = legend_component_1.PageType.None;
        this.showRAG3 = false;
        this.showRAG5 = false;
        this.showBOB = false;
        this.showQuintilesRAG = false;
        this.showQuintilesBOB = false;
        this.showRecentTrends = false;
    }
    AreaProfileComponent.prototype.onOutsideEvent = function (event) {
        var _this = this;
        var ftHelper = this.ftHelperService;
        var groupRoots = ftHelper.getAllGroupRoots();
        this.model = ftHelper.getFTModel();
        this.config = ftHelper.getFTConfig();
        this.isAnyData = groupRoots.length > 0;
        this.scrollTop = this.uiService.getScrollTop();
        var comparatorId = ftHelper.getComparatorId();
        var parentAreaCode = ftHelper.getComparatorById(comparatorId).Code;
        this.setDisplayConfig();
        this.setIsAreaIgnored();
        // AJAX calls
        var indicatorStatisticsObservable = this.indicatorService.getIndicatorStatistics(this.model.areaTypeId, parentAreaCode, this.model.profileId, this.model.groupId);
        Observable_1.Observable.forkJoin([indicatorStatisticsObservable]).subscribe(function (results) {
            _this.indicatorStats = _.values(results[0]);
            _this.metadataHash = ftHelper.getMetadataHash();
            _this.setAreaProfileHtml();
            // Unlock UI
            ftHelper.showAndHidePageElements();
            ftHelper.showDataQualityLegend();
            ftHelper.showTargetBenchmarkOption(groupRoots);
            _this.uiService.setScrollTop(_this.scrollTop);
            // Legend display configurations
            _this.pageType = legend_component_1.PageType.AreaProfiles;
            _this.showRecentTrends = _this.areRecentTrendsDisplayed;
            ftHelper.unlock();
        });
        this.tooltip = new shared_1.TooltipHelper(this.ftHelperService.newTooltipManager());
    };
    AreaProfileComponent.prototype.setAreaProfileHtml = function () {
        var ftHelper = this.ftHelperService;
        // Benchmark
        var comparatorId = ftHelper.getComparatorId();
        var isNationalBenchmark = (comparatorId === shared_1.ComparatorIds.National);
        var benchmark = isNationalBenchmark ?
            ftHelper.getNationalComparator() :
            ftHelper.getParentArea();
        var areaCode = this.model.areaCode;
        this.area = ftHelper.getArea(areaCode);
        this.uiService.toggleQuintileLegend($('#quintile-key'), false);
        var isSubnationalColumn = !this.model.isNearestNeighbours() && ftHelper.isSubnationalColumn();
        var indicatorRows = [];
        var rootIndex = 0;
        var groupRoots = ftHelper.getAllGroupRoots();
        for (var _i = 0, groupRoots_1 = groupRoots; _i < groupRoots_1.length; _i++) {
            var root = groupRoots_1[_i];
            var metadata = this.metadataHash[root.IID];
            var areaData = new shared_1.CoreDataListHelper(root.Data).findByAreaCode(areaCode);
            // Create indicator row
            var row = new IndicatorRow();
            row.rootIndex = rootIndex;
            indicatorRows.push(row);
            rootIndex += 1;
            row.comparisonConfig = ftHelper.newComparisonConfig(root, metadata);
            this.assignStatsToRow(root, row);
            // Parent data
            var subnationalData = ftHelper.getRegionalComparatorGrouping(root).ComparatorData;
            var nationalData = ftHelper.getNationalComparatorGrouping(root).ComparatorData;
            var benchmarkData = isNationalBenchmark ?
                nationalData :
                subnationalData;
            row.benchmarkData = benchmarkData;
            // Init formatter
            var formatter = ftHelper.newIndicatorFormatter(root, metadata, areaData, row.statsF);
            formatter.averageData = benchmarkData;
            // Value displayer
            var unit = !!metadata ? metadata.Unit : null;
            var valueDisplayer = ftHelper.newValueDisplayer(unit);
            // Set piggy back object
            row.formatter = formatter;
            row.areaData = areaData;
            row.groupRoot = root;
            row.indicatorMetadata = metadata;
            row.area = this.area;
            // Set display properties
            row.period = root.Grouping[0].Period;
            row.areaCount = formatter.getAreaCount();
            row.areaValue = formatter.getAreaValue();
            // Area data note tooltip
            var dataInfo = ftHelper.newCoreDataSetInfo(row.areaData);
            if (dataInfo.isNote()) {
                row.areaValueTooltip = ftHelper.getValueNoteById(areaData.NoteId).Text;
            }
            // England value
            var nationalDataInfo = ftHelper.newCoreDataSetInfo(nationalData);
            row.englandValue = valueDisplayer.byDataInfo(nationalDataInfo, { noCommas: 'y' });
            if (nationalDataInfo.isNote()) {
                row.englandValueTooltip = ftHelper.getValueNoteById(nationalData.NoteId).Text;
            }
            // Subnational value
            if (isSubnationalColumn) {
                var subnationalDataInfo = ftHelper.newCoreDataSetInfo(subnationalData);
                row.subnationalValue = valueDisplayer.byDataInfo(subnationalDataInfo, { noCommas: 'y' });
                if (subnationalDataInfo.isNote()) {
                    row.subnationalValueTooltip = ftHelper.getValueNoteById(subnationalData.NoteId).Text;
                }
            }
            this.setSpineChartDimensions(row);
            // Min / Max
            var dimensions = row.spineChartDimensions;
            if (dimensions.isAnyData && dimensions.isSufficientData) {
                // Data available
                row.englandMin = formatter.getMin();
                row.englandMax = formatter.getMax();
                row.minTooltip = this.polarityToText(root.PolarityId, true);
                row.maxTooltip = this.polarityToText(root.PolarityId, false);
            }
            else {
                // No data
                row.englandMin = this.NoData;
                row.englandMax = this.NoData;
            }
            // Trend
            if (root.RecentTrends) {
                var trendMarkerValue = void 0, polarityId = void 0;
                if (areaData) {
                    var recentTrends = root.RecentTrends[areaData.AreaCode];
                    trendMarkerValue = recentTrends.Marker;
                    polarityId = root.PolarityId;
                    row.trendMarkerResult = recentTrends;
                }
                else {
                    trendMarkerValue = shared_1.TrendMarkerValue.CannotCalculate;
                    polarityId = 0;
                }
                row.recentTrendImage = ftHelper.getTrendMarkerImage(trendMarkerValue, polarityId);
            }
            if (this.isAreaIgnored) {
                // Do not show any message or chart if area is ignored for spine chart
                dimensions.isAnyData = false;
            }
        }
        this.setAreaHeadings(benchmark);
        this.indicatorRows = indicatorRows;
        this.setLegendDisplay(groupRoots);
        ftHelper.initBootstrapTooltips();
    };
    AreaProfileComponent.prototype.setLegendDisplay = function (groupRoots) {
        this.showRAG3 = false;
        this.showRAG5 = false;
        this.showBOB = false;
        this.showQuintilesRAG = false;
        this.showQuintilesBOB = false;
        // Show Quintile BOB legend
        if (groupRoots.findIndex(function (x) { return x.ComparatorMethodId === shared_1.ComparatorMethodIds.Quintiles && x.PolarityId === shared_1.PolarityIds.NotApplicable; }) > -1) {
            this.showQuintilesBOB = true;
        }
        // Show Quintile RAG legend
        if (groupRoots.findIndex(function (x) { return x.ComparatorMethodId === shared_1.ComparatorMethodIds.Quintiles && (x.PolarityId === shared_1.PolarityIds.RAGLowIsGood || x.PolarityId === shared_1.PolarityIds.RAGHighIsGood); }) > -1) {
            this.showQuintilesRAG = true;
        }
        // Show RAG5 legend
        if (groupRoots.findIndex(function (x) { return x.ComparatorMethodId === shared_1.ComparatorMethodIds.SingleOverlappingCIsForTwoCiLevels; }) > -1) {
            this.showRAG5 = true;
        }
        else {
            // Show RAG3 legend
            if (groupRoots.findIndex(function (x) { return x.ComparatorMethodId === shared_1.ComparatorMethodIds.SingleOverlappingCIsForOneCiLevel; }) > -1) {
                this.showRAG3 = true;
            }
            if (this.showRAG3 === false &&
                (groupRoots.findIndex(function (x) { return x.PolarityId === shared_1.PolarityIds.RAGHighIsGood; }) > -1 ||
                    groupRoots.findIndex(function (x) { return x.PolarityId === shared_1.PolarityIds.RAGLowIsGood; }) > -1)) {
                this.showRAG3 = true;
            }
        }
        // Show BOB legend
        if (groupRoots.findIndex(function (x) { return x.PolarityId === shared_1.PolarityIds.BlueOrangeBlue; }) > -1) {
            this.showBOB = true;
        }
    };
    AreaProfileComponent.prototype.polarityToText = function (polarity, isForLowest) {
        if (polarity === shared_1.PolarityIds.RAGLowIsGood || polarity === shared_1.PolarityIds.RAGHighIsGood) {
            return isForLowest ? 'Worst' : 'Best';
        }
        return isForLowest ? 'Lowest' : 'Highest';
    };
    AreaProfileComponent.prototype.assignStatsToRow = function (root, row) {
        var statsBase = new shared_1.GroupRootHelper(root).findMatchingItemBySexAgeAndIndicatorId(this.indicatorStats);
        if (statsBase) {
            row.statsF = statsBase.StatsF;
            row.stats = statsBase.Stats;
            row.haveRequiredValuesForSpineChart = statsBase.HaveRequiredValues;
        }
    };
    AreaProfileComponent.prototype.setSpineChartDimensions = function (row) {
        var ftHelper = this.ftHelperService;
        var benchmarkDataInfo = ftHelper.newCoreDataSetInfo(row.benchmarkData);
        var isValidBenchmarkValue = benchmarkDataInfo.isValue();
        if (isValidBenchmarkValue && !row.haveRequiredValuesForSpineChart) {
            // Should show not enough values message
            var dimensions = new spine_chart_classes_1.SpineChartDimensions();
            dimensions.isSufficientData = false;
            row.spineChartDimensions = dimensions;
        }
        else if (!row.stats ||
            !isValidBenchmarkValue ||
            row.indicatorMetadata.ValueType.Id === shared_1.ValueTypeIds.Count) {
            // Should show no data message
            var dimensions = new spine_chart_classes_1.SpineChartDimensions();
            dimensions.isAnyData = false;
            row.spineChartDimensions = dimensions;
        }
        else {
            // Have data can show spine chart
            var polarityId = row.groupRoot.PolarityId;
            var spineChart = new spine_chart_classes_1.SpineChartCalculator();
            var proportions = spineChart.getSpineProportions(row.benchmarkData.Val, row.stats, polarityId);
            var dataInfo = ftHelper.newCoreDataSetInfo(row.areaData);
            var spineDimensions = spineChart.getDimensions(proportions, dataInfo, this.imgUrl, row.comparisonConfig, row.indicatorMetadata.IID, ftHelper.getMarkerImageFromSignificance);
            row.spineChartDimensions = spineDimensions;
        }
    };
    AreaProfileComponent.prototype.getIndicatorNameHtml = function (row) {
        var formatter = row.formatter;
        var ftHelper = this.ftHelperService;
        var root = row.groupRoot;
        var metadata = row.indicatorMetadata;
        // Indicator name & data quality
        var html = [
            formatter.getIndicatorName(),
            this.ftHelperService.getIndicatorDataQualityHtml(formatter.getDataQuality())
        ];
        // New data badge
        if (ftHelper.hasDataChanged(root)) {
            html.push('&nbsp;<span style="margin-right:8px;" class="badge badge-success">New data</span>');
        }
        // Target legend
        var targetLegend = ftHelper.getTargetLegendHtml(row.comparisonConfig, metadata);
        if (targetLegend) {
            html.push('<br>', targetLegend);
        }
        return html.join('');
    };
    AreaProfileComponent.prototype.setAreaHeadings = function (benchmark) {
        this.shortAreaName = new shared_1.AreaHelper(this.area).getShortAreaNameToDisplay();
        this.areaName = this.area.Name;
        this.benchmarkName = benchmark.Name;
    };
    AreaProfileComponent.prototype.setDisplayConfig = function () {
        var ftHelper = this.ftHelperService;
        var urls = ftHelper.getURL();
        this.imgUrl = urls.img;
        var groupRoots = ftHelper.getAllGroupRoots();
        this.isParentUk = ftHelper.isParentUk();
        this.isNationalAndRegional = ftHelper.getEnumParentDisplay() === shared_1.ParentDisplay.NationalAndRegional &&
            this.model.parentTypeId !== shared_1.AreaTypeIds.Country;
        var trendMarkerFound = groupRoots && groupRoots[0] && groupRoots[0].RecentTrends;
        this.trendColSpan = trendMarkerFound ? 3 : 2;
        this.areRecentTrendsDisplayed = trendMarkerFound && this.config.hasRecentTrends;
        this.isNotNN = !this.model.isNearestNeighbours();
        this.colSpan = this.isNationalAndRegional && !this.model.isNearestNeighbours() ? 3 : 4;
        this.parentType = ftHelper.getParentTypeName();
        this.setSpineHeadersAndChartLabelImage(urls.img, groupRoots);
        this.lowest = this.config.spineHeaders.min;
        this.highest = this.config.spineHeaders.max;
    };
    AreaProfileComponent.prototype.setSpineHeadersAndChartLabelImage = function (imageUrl, groupRoots) {
        if (groupRoots.findIndex(function (x) { return x.ComparatorMethodId === shared_1.ComparatorMethodIds.SingleOverlappingCIsForOneCiLevel; }) > -1 ||
            groupRoots.findIndex(function (x) { return x.ComparatorMethodId === shared_1.ComparatorMethodIds.SingleOverlappingCIsForTwoCiLevels; }) > -1) {
            this.config.spineHeaders.min = shared_1.SpineChartMinMaxLabelDescription.Worst;
            this.config.spineHeaders.max = shared_1.SpineChartMinMaxLabelDescription.Best;
            this.config.spineChartMinMaxLabelId = shared_1.SpineChartMinMaxLabel.WorstAndBest;
        }
        else {
            if (groupRoots.findIndex(function (x) { return x.PolarityId === shared_1.PolarityIds.RAGHighIsGood; }) > -1 ||
                groupRoots.findIndex(function (x) { return x.PolarityId === shared_1.PolarityIds.RAGLowIsGood; }) > -1) {
                this.config.spineHeaders.min = shared_1.SpineChartMinMaxLabelDescription.Worst;
                this.config.spineHeaders.max = shared_1.SpineChartMinMaxLabelDescription.Best;
                this.config.spineChartMinMaxLabelId = shared_1.SpineChartMinMaxLabel.WorstAndBest;
            }
        }
        if (groupRoots.findIndex(function (x) { return x.PolarityId === shared_1.PolarityIds.BlueOrangeBlue; }) > -1) {
            if (this.config.spineChartMinMaxLabelId === shared_1.SpineChartMinMaxLabel.WorstAndBest) {
                this.config.spineHeaders.min = shared_1.SpineChartMinMaxLabelDescription.WorstLowest;
                this.config.spineHeaders.max = shared_1.SpineChartMinMaxLabelDescription.BestHighest;
                this.config.spineChartMinMaxLabelId = shared_1.SpineChartMinMaxLabel.WorstLowestAndBestHighest;
            }
            else {
                this.config.spineHeaders.min = shared_1.SpineChartMinMaxLabelDescription.Lowest;
                this.config.spineHeaders.max = shared_1.SpineChartMinMaxLabelDescription.Highest;
                this.config.spineChartMinMaxLabelId = shared_1.SpineChartMinMaxLabel.LowestAndHighest;
            }
        }
        this.spineChartImageUrl = imageUrl + 'spine-key-label-id-' + this.config.spineChartMinMaxLabelId + '.png';
    };
    /** Whether the current area should is ignored for the spine charts */
    AreaProfileComponent.prototype.setIsAreaIgnored = function () {
        var ignoredSpineChartAreas = this.config.ignoredSpineChartAreas;
        // Is area too small for spine charts
        var isIgnored = false;
        if (ignoredSpineChartAreas) {
            var areaCode_1 = this.model.areaCode;
            var areas = ignoredSpineChartAreas.split(',');
            isIgnored = _.any(areas, function (area) {
                return area === areaCode_1;
            });
        }
        this.isAreaIgnored = isIgnored;
    };
    AreaProfileComponent.prototype.goToBarChart = function (row) {
        this.ftHelperService.goToBarChartPage(row.rootIndex);
    };
    AreaProfileComponent.prototype.goToTrends = function (row) {
        this.ftHelperService.goToAreaTrendsPage(row.rootIndex);
    };
    AreaProfileComponent.prototype.onExportClick = function (event) {
        this.ftHelperService.exportTableAsImage('area-profile-container', 'AreaProfilesTable', '#key-spine-chart,#spine-range-key');
    };
    AreaProfileComponent.prototype.onExportCsvFileClick = function (event) {
        alert('It works!');
    };
    AreaProfileComponent.prototype.showIndicatorTooltip = function (event, row) {
        this.currentTooltipHtml = this.ftHelperService.getIndicatorNameTooltip(row.rootIndex, this.area);
        this.tooltip.displayHtml(event, this.currentTooltipHtml);
    };
    AreaProfileComponent.prototype.showRecentTrendTooltip = function (event, row) {
        var tooltipProvider = this.ftHelperService.newRecentTrendsTooltip();
        this.currentTooltipHtml = tooltipProvider.getTooltipByData(row.trendMarkerResult);
        this.tooltip.displayHtml(event, this.currentTooltipHtml);
    };
    AreaProfileComponent.prototype.hideTooltip = function () {
        this.tooltip.hide();
        this.currentTooltipHtml = null;
    };
    AreaProfileComponent.prototype.repositionTooltip = function (event) {
        this.tooltip.reposition(event);
    };
    return AreaProfileComponent;
}());
__decorate([
    core_1.HostListener('window:AreaProfileSelected', ['$event']),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [Object]),
    __metadata("design:returntype", void 0)
], AreaProfileComponent.prototype, "onOutsideEvent", null);
AreaProfileComponent = __decorate([
    core_1.Component({
        selector: 'ft-area-profile',
        template: __webpack_require__("./src/app/area-profile/area-profile.component.html"),
        styles: [__webpack_require__("./src/app/area-profile/area-profile.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _a || Object, typeof (_b = typeof indicator_service_1.IndicatorService !== "undefined" && indicator_service_1.IndicatorService) === "function" && _b || Object, typeof (_c = typeof ui_service_1.UIService !== "undefined" && ui_service_1.UIService) === "function" && _c || Object])
], AreaProfileComponent);
exports.AreaProfileComponent = AreaProfileComponent;
var IndicatorRow = (function () {
    function IndicatorRow() {
        this.stats = null;
        this.statsF = null;
        this.haveRequiredValuesForSpineChart = false;
    }
    return IndicatorRow;
}());
exports.IndicatorRow = IndicatorRow;
var _a, _b, _c;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/area-profile.component.js.map

/***/ }),

/***/ "./src/app/area-profile/area-profile.module.ts":
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
var area_profile_component_1 = __webpack_require__("./src/app/area-profile/area-profile.component.ts");
var spine_chart_component_1 = __webpack_require__("./src/app/area-profile/spine-chart/spine-chart.component.ts");
var legend_module_1 = __webpack_require__("./src/app/shared/component/legend/legend.module.ts");
var AreaProfileModule = (function () {
    function AreaProfileModule() {
    }
    return AreaProfileModule;
}());
AreaProfileModule = __decorate([
    core_1.NgModule({
        imports: [
            common_1.CommonModule,
            legend_module_1.LegendModule
        ],
        declarations: [
            area_profile_component_1.AreaProfileComponent,
            spine_chart_component_1.SpineChartComponent,
        ],
        exports: [
            area_profile_component_1.AreaProfileComponent,
            spine_chart_component_1.SpineChartComponent
        ]
    })
], AreaProfileModule);
exports.AreaProfileModule = AreaProfileModule;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/area-profile.module.js.map

/***/ }),

/***/ "./src/app/area-profile/spine-chart.classes.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var SpineChartCalculator = (function () {
    function SpineChartCalculator() {
        // Not 1 for float precision reasons
        this.one = 0.9999;
        this.halfMarkerWidth = 18 / 2;
    }
    SpineChartCalculator.prototype.getSpineProportions = function (average, stats, polarityId) {
        var min, max, p25, p75;
        var prop = new SpineChartProportions();
        prop.isHighestLeft = this.isHighestLeft(polarityId);
        if (prop.isHighestLeft) {
            min = stats.Max;
            max = stats.Min;
            p25 = stats.P75;
            p75 = stats.P25;
        }
        else {
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
        }
        else {
            prop.unitsOfLargestSide = maxAverageDifference;
            prop.q1Offset = (maxAverageDifference - minAverageDifference);
        }
        prop.q1 = Math.abs(p25 - min);
        prop.q2 = Math.abs(p75 - p25);
        prop.q4 = Math.abs(max - p75);
        prop.min = min;
        return prop;
    };
    SpineChartCalculator.prototype.isHighestLeft = function (polarityId) {
        return polarityId === 0;
    };
    SpineChartCalculator.prototype.getDimensions = function (proportions, dataInfo, imgUrl, comparisonConfig, indicatorId, getMarkerImageFromSignificance) {
        var dimensions = new SpineChartDimensions();
        dimensions.imgUrl = imgUrl;
        var calc = new SpineChartDimensionCalculator(proportions.unitsOfLargestSide);
        dimensions.q1 = calc.getDimension(proportions.q1);
        dimensions.q2 = calc.getDimension(proportions.q2);
        dimensions.q4 = calc.getDimension(proportions.q4);
        dimensions.q1Offset = calc.getDimension(proportions.q1Offset);
        dimensions.q1Offset += 12 /*margin left*/;
        dimensions.pixelPerUnit = calc.pixelPerUnit;
        // Marker
        if (dataInfo.isValue()) {
            var markerOffset = calc.getMarkerOffset(dataInfo.data.Val, proportions);
            dimensions.markerOffset = markerOffset + dimensions.q1Offset;
            var suffix = comparisonConfig.useQuintileColouring
                ? '_medium'
                : '';
            dimensions.markerImage = getMarkerImageFromSignificance(dataInfo.data.Sig[comparisonConfig.comparatorId], comparisonConfig.useRagColours, suffix, comparisonConfig.useQuintileColouring, indicatorId, dataInfo.data.SexId, dataInfo.data.AgeId);
        }
        this.adjustDimensionsForRounding(dimensions, calc.roundDifference);
        return dimensions;
    };
    SpineChartCalculator.prototype.adjustDimensionsForRounding = function (dimensions, roundDiff) {
        //Note: q4 may not be most appropriate section to adjust
        if (roundDiff <= -this.one) {
            dimensions.q4 -= 1;
        }
        else if (roundDiff >= this.one) {
            dimensions.q4 += 1;
        }
    };
    ;
    return SpineChartCalculator;
}());
exports.SpineChartCalculator = SpineChartCalculator;
var SpineChartDimensionCalculator = (function () {
    function SpineChartDimensionCalculator(unitsOfLargestSide) {
        this.roundDifference = 0;
        this.halfMarkerWidth = 18 / 2;
        var halfSpineChartWidth = 250 / 2;
        this.pixelPerUnit = halfSpineChartWidth / unitsOfLargestSide;
    }
    SpineChartDimensionCalculator.prototype.getDimension = function (proportion) {
        var val = proportion * this.pixelPerUnit;
        var rounded = Math.round(val);
        this.roundDifference += val - rounded;
        return rounded;
    };
    /** Left offset to position the centre of marker image appropriate for the value */
    SpineChartDimensionCalculator.prototype.getMarkerOffset = function (value, proportions) {
        if (value) {
            var min = proportions.min;
            var marker = proportions.isHighestLeft ?
                min - value :
                value - min;
            return Math.round((marker * this.pixelPerUnit) - this.halfMarkerWidth);
        }
        return null;
    };
    return SpineChartDimensionCalculator;
}());
/** Proportion values are expressed in the unit of the indicator */
var SpineChartProportions = (function () {
    function SpineChartProportions() {
    }
    return SpineChartProportions;
}());
exports.SpineChartProportions = SpineChartProportions;
var SpineChartDimensions = (function () {
    function SpineChartDimensions() {
        this.isSufficientData = true;
        this.isAnyData = true;
    }
    return SpineChartDimensions;
}());
exports.SpineChartDimensions = SpineChartDimensions;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/spine-chart.classes.js.map

/***/ }),

/***/ "./src/app/area-profile/spine-chart/spine-chart.component.css":
/***/ (function(module, exports) {

module.exports = ".spine250 {\r\n    width: 274px;\r\n    /*SPINE_WIDTH + (margin-left calculateSpineDimensions x2)*/\r\n    position: relative;\r\n    text-align: center;\r\n}\r\n\r\n.average250 {\r\n    width: 2px;\r\n    left: 136px;\r\n    /*half spine250 width - 1(i.e. half average bar width)*/\r\n    position: absolute;\r\n    height: 23px;\r\n}\r\n\r\n.no-spine {\r\n    height: 23px;\r\n    line-height: 23px;\r\n    width: 100%;\r\n    text-align: center;\r\n    font-style: italic;\r\n    font-size: 11px;\r\n}\r\n\r\n.q1,\r\n.q2,\r\n.q4,\r\n.spine {\r\n    height: 23px;\r\n    float: left;\r\n}\r\n\r\n.spine {\r\n    overflow: hidden;\r\n    padding: 0 0 0 0;\r\n}\r\n\r\n.marker {\r\n    position: absolute;\r\n    top: 2px;\r\n}"

/***/ }),

/***/ "./src/app/area-profile/spine-chart/spine-chart.component.html":
/***/ (function(module, exports) {

module.exports = "<div *ngIf=\"dimensions && dimensions.isSufficientData && dimensions.isAnyData\" class=\"spine spine250\">\r\n    <img src=\"{{dimensions.imgUrl}}lightgrey.png\" class=\"q1\" [style.width]=\"dimensions.q1 + 'px'\" [style.marginLeft]=\"dimensions.q1Offset + 'px'\" alt=\"\" (mouseover)=\"showLeftQuartileTooltip($event)\" (mouseout)=\"hideTooltip()\" (mousemove)=\"repositionTooltip($event)\"\r\n    />\r\n    <img src=\"{{dimensions.imgUrl}}darkgrey.png\" class=\"q2\" [style.width]=\"dimensions.q2 + 'px'\" alt=\"\" (mouseover)=\"showInnerQuartilesTooltip($event)\" (mouseout)=\"hideTooltip()\" (mousemove)=\"repositionTooltip($event)\" />\r\n    <img src=\"{{dimensions.imgUrl}}lightgrey.png\" class=\"q4\" [style.width]=\"dimensions.q4 + 'px'\" alt=\"\" (mouseover)=\"showRightQuartileTooltip($event)\" (mouseout)=\"hideTooltip()\" (mousemove)=\"repositionTooltip($event)\" />\r\n    <img src=\"{{dimensions.imgUrl}}red.png\" class=\"average250\" alt=\"\" (mouseover)=\"showBenchmarkTooltip($event)\" (mouseout)=\"hideTooltip()\" (mousemove)=\"repositionTooltip($event)\" />\r\n    <img src=\"{{dimensions.imgUrl}}{{dimensions.markerImage}}\" class=\"marker\" [style.left]=\"dimensions.markerOffset + 'px'\" alt=\"\" (mouseover)=\"showMarkerTooltip($event)\" (mouseout)=\"hideTooltip()\" (mousemove)=\"repositionTooltip($event)\" />\r\n</div>\r\n<div *ngIf=\"dimensions && dimensions.isAnyData && !dimensions.isSufficientData\" class=\"no-spine\">Insufficient number of values for a spine chart</div>\r\n<div *ngIf=\"dimensions && !dimensions.isAnyData\" class=\"no-spine\">-</div>"

/***/ }),

/***/ "./src/app/area-profile/spine-chart/spine-chart.component.ts":
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
var spine_chart_classes_1 = __webpack_require__("./src/app/area-profile/spine-chart.classes.ts");
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var area_profile_component_1 = __webpack_require__("./src/app/area-profile/area-profile.component.ts");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var SpineChartComponent = (function () {
    function SpineChartComponent(ftHelperService) {
        this.ftHelperService = ftHelperService;
    }
    SpineChartComponent.prototype.showMarkerTooltip = function (event) {
        this.html = [];
        var formatter = this.indicatorRow.formatter;
        // Value
        this.addDataText(formatter.getAreaValue() + formatter.getSuffixIfNoShort());
        // Area name
        var areaName = this.ftHelperService.getAreaNameToDisplay(this.indicatorRow.area);
        this.html.push('<span id="tooltipArea">', areaName, '</span>');
        this.addIndicatorName();
        this.addValueNote();
        this.showTooltip(event);
    };
    SpineChartComponent.prototype.showInnerQuartilesTooltip = function (event) {
        var formatter = this.indicatorRow.formatter;
        var values = formatter.get25() + ' - ' + formatter.get75();
        var labels = '25th Percentile to 75th Percentile';
        this.showRangeTooltip(event, values, labels);
    };
    SpineChartComponent.prototype.showLeftQuartileTooltip = function (event) {
        var formatter = this.indicatorRow.formatter;
        var values = formatter.getMin() + ' - ' + formatter.get25();
        var labels = this.getSpineHeaders().min + ' to 25th Percentile';
        this.showRangeTooltip(event, values, labels);
    };
    SpineChartComponent.prototype.showRightQuartileTooltip = function (event) {
        var formatter = this.indicatorRow.formatter;
        var values = formatter.get75() + ' - ' + formatter.getMax();
        var labels = '75th Percentile to ' + this.getSpineHeaders().max;
        this.showRangeTooltip(event, values, labels);
    };
    SpineChartComponent.prototype.showBenchmarkTooltip = function (event) {
        this.html = [];
        var formatter = this.indicatorRow.formatter;
        this.addDataText(formatter.getAverage() + formatter.getSuffixIfNoShort());
        this.addComparatorName();
        this.addIndicatorName();
        this.showTooltip(event);
    };
    SpineChartComponent.prototype.addDataText = function (text) {
        this.html.push('<span id="tooltipData">', text, '</span>');
    };
    SpineChartComponent.prototype.showRangeTooltip = function (event, values, labels) {
        this.html = [];
        var formatter = this.indicatorRow.formatter;
        this.addDataText(values);
        this.addRangeText(labels);
        this.addComparatorName();
        this.addIndicatorName();
        this.showTooltip(event);
    };
    SpineChartComponent.prototype.addComparatorName = function () {
        this.html.push('<span id="tooltipArea">', this.ftHelperService.getCurrentComparator().Name, '</span>');
    };
    SpineChartComponent.prototype.getSpineHeaders = function () {
        return this.ftHelperService.getFTConfig().spineHeaders;
    };
    SpineChartComponent.prototype.addIndicatorName = function () {
        this.html.push('<span id="tooltipIndicator">', this.indicatorRow.formatter.getIndicatorName(), '</span>');
    };
    SpineChartComponent.prototype.addRangeText = function (text) {
        this.html.push('<span id="range-text">', text, '</span>');
    };
    SpineChartComponent.prototype.addValueNote = function () {
        var noteId = this.indicatorRow.areaData.NoteId;
        if (noteId) {
            var valueNoteHtml = this.ftHelperService.newValueNoteTooltipProvider().getHtmlFromNoteId(noteId);
            // Add extra CSS class
            valueNoteHtml = valueNoteHtml.replace('tooltipValueNote', 'tooltipValueNote tooltip-value-vote-spine-chart');
            this.html.push(valueNoteHtml);
        }
    };
    SpineChartComponent.prototype.showTooltip = function (event) {
        this.currentTooltipHtml = this.html.join('');
        this.tooltip.displayHtml(event, this.currentTooltipHtml);
    };
    SpineChartComponent.prototype.hideTooltip = function () {
        this.tooltip.hide();
        this.currentTooltipHtml = null;
    };
    SpineChartComponent.prototype.repositionTooltip = function (event) {
        this.tooltip.reposition(event);
    };
    return SpineChartComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_a = typeof spine_chart_classes_1.SpineChartDimensions !== "undefined" && spine_chart_classes_1.SpineChartDimensions) === "function" && _a || Object)
], SpineChartComponent.prototype, "dimensions", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_b = typeof shared_1.TooltipHelper !== "undefined" && shared_1.TooltipHelper) === "function" && _b || Object)
], SpineChartComponent.prototype, "tooltip", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_c = typeof area_profile_component_1.IndicatorRow !== "undefined" && area_profile_component_1.IndicatorRow) === "function" && _c || Object)
], SpineChartComponent.prototype, "indicatorRow", void 0);
SpineChartComponent = __decorate([
    core_1.Component({
        selector: 'ft-spine-chart',
        template: __webpack_require__("./src/app/area-profile/spine-chart/spine-chart.component.html"),
        styles: [__webpack_require__("./src/app/area-profile/spine-chart/spine-chart.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_d = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _d || Object])
], SpineChartComponent);
exports.SpineChartComponent = SpineChartComponent;
var _a, _b, _c, _d;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/spine-chart.component.js.map

/***/ }),

/***/ "./src/app/arealist/arealist-index/arealist-index.component.css":
/***/ (function(module, exports) {

module.exports = "#div-content a.btn {\r\n    text-decoration: none;\r\n    color: #ffffff;\r\n}\r\n\r\n.webgrid-table {\r\n    font-family: \"Trebuchet MS\", Arial, Helvetica, sans-serif;\r\n    font-size: 1.1em;\r\n    width: 100%;\r\n    display: table;\r\n    border-collapse: collapse;\r\n    border: solid 1px #98BF21;\r\n    background-color: white;\r\n}\r\n\r\n.webgrid-table td,\r\nth {\r\n    border: 1px solid #98BF21;\r\n    padding: 3px 7px 2px;\r\n}\r\n\r\n.webgrid-header {\r\n    padding-bottom: 4px;\r\n    padding-top: 5px;\r\n    text-align: center;\r\n}\r\n\r\n.webgrid-header th {\r\n    text-align: center;\r\n}\r\n\r\n.webgrid-row-style {\r\n    padding: 3px 7px 2px;\r\n}\r\n\r\n.webgrid-row-style a,\r\n.webgrid-alternating-row a {\r\n    text-decoration: none;\r\n    color: #ffffff;\r\n}\r\n\r\n.webgrid-alternating-row {\r\n    padding: 3px 7px 2px;\r\n}\r\n\r\nthead tr th:first-child,\r\ntbody tr td:first-child {\r\n    width: 40em;\r\n}\r\n\r\n.list-name {\r\n    color: #2e3191;\r\n    font-weight: bold;\r\n    text-decoration: underline;\r\n}\r\n\r\n#lightBox {\r\n    display: none;\r\n}\r\n\r\n#infoBox {\r\n    display: none;\r\n    width: 438px;\r\n    top: 50%;\r\n    left: 50%;\r\n    margin-top: -50px;\r\n    margin-left: -50px;\r\n}\r\n\r\n.help-link-button {\r\n    text-decoration: underline;\r\n    color: #2e3191;\r\n    float: right;\r\n    z-index: 1000;\r\n    margin-top: -59px;\r\n    padding-right: 0px;\r\n}"

/***/ }),

/***/ "./src/app/arealist/arealist-index/arealist-index.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"row\" *ngIf=\"displayPage()\">\r\n    <div class=\"col-md-12\">\r\n        <button id=\"help-view-area-list-on-data-page\" class=\"btn btn-link help-link-button\" (click)=\"helpViewAreaListOnDataPage()\">\r\n            How do I view my area lists?\r\n        </button>\r\n    </div>\r\n</div>\r\n<div class=\"row\">\r\n    <div id=\"grid-content\" class=\"col-md-12\" *ngIf=\"isAnyData()\">\r\n        <table class=\"webgrid-table\">\r\n            <thead>\r\n                <tr class=\"webgrid-header\">\r\n                    <th scope=\"col\">\r\n                        <button class=\"btn btn-link list-name\" (click)=\"sort(1)\">\r\n                            <u>List name</u>\r\n                        </button>\r\n                    </th>\r\n                    <th scope=\"col\">\r\n                        <button class=\"btn btn-link list-name\" (click)=\"sort(2)\">\r\n                            <u>Area type</u>\r\n                        </button>\r\n                    </th>\r\n                    <th scope=\"col\">\r\n                        Edit\r\n                    </th>\r\n                    <th scope=\"col\">\r\n                        Copy\r\n                    </th>\r\n                    <th scope=\"col\">\r\n                        Delete\r\n                    </th>\r\n                </tr>\r\n            </thead>\r\n            <tbody>\r\n                <tr *ngFor=\"let areaListRow of areaListRows; let last=last\" class=\"webgrid-row-style\">\r\n                    <td>\r\n                        {{areaListRow.AreaListName}} {{last? hideSpinner() : ''}}\r\n                    </td>\r\n                    <td>\r\n                        {{areaListRow.AreaTypeName}}\r\n                    </td>\r\n                    <td class=\"center\">\r\n                        <a id=\"edit-area-list\" href=\"area-list/edit?list_id={{areaListRow.PublicId}}\" class=\"btn btn-outline-primary edit-link-button\">Edit</a>\r\n                    </td>\r\n                    <td class=\"center\">\r\n                        <button id=\"copy-area-list\" class=\"btn btn-outline-primary\" (click)=\"copyAreaListConfirmation(areaListRow.AreaListId)\">Copy</button>\r\n                    </td>\r\n                    <td class=\"center\">\r\n                        <button id=\"delete-area-list\" class=\"btn btn-outline-primary\" (click)=\"deleteAreaListConfirmation(areaListRow)\">Delete</button>\r\n                    </td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n    </div>\r\n</div>\r\n<ft-light-box [lightBoxConfig]=\"lightBoxConfig\" (emitLightBoxActionConfirmed)=\"updateLightBoxActionConfirmed($event)\"></ft-light-box>\r\n<ft-light-box-with-input [lightBoxWithInputConfig]=\"lightBoxWithInputConfig\" (emitLightBoxWithInputActionConfirmed)=\"updateLightBoxWithInputActionConfirmed($event)\" (emitLightBoxInputText)=\"updateLightBoxInputText($event)\"></ft-light-box-with-input>"

/***/ }),

/***/ "./src/app/arealist/arealist-index/arealist-index.component.ts":
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
var area_service_1 = __webpack_require__("./src/app/shared/service/api/area.service.ts");
var arealist_service_1 = __webpack_require__("./src/app/shared/service/api/arealist.service.ts");
var Observable_1 = __webpack_require__("./node_modules/rxjs/_esm5/Observable.js");
var light_box_component_1 = __webpack_require__("./src/app/shared/component/light-box/light-box.component.ts");
var light_box_with_input_component_1 = __webpack_require__("./src/app/shared/component/light-box-with-input/light-box-with-input.component.ts");
var ArealistIndexComponent = (function () {
    function ArealistIndexComponent(areaListService, areaService, ref) {
        this.areaListService = areaListService;
        this.areaService = areaService;
        this.ref = ref;
        this.areaLists = [];
        this.areaTypes = [];
        this.areaListRows = [];
        this.sortOrder = false;
        this.sortOrderAreaListName = false;
        this.sortOrderAreaTypeName = false;
    }
    ArealistIndexComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.userId = document.getElementById('ft-arealist-index').getAttribute('user-id');
        var areaListsObservable = this.areaListService.getAreaLists(this.userId);
        var areaTypesObservable = this.areaService.getAreaTypes();
        Observable_1.Observable.forkJoin([areaListsObservable, areaTypesObservable]).subscribe(function (results) {
            _this.areaLists = results[0];
            _this.areaTypes = results[1];
            _this.areaLists.forEach(function (element) {
                var row = new AreaListRow();
                row.AreaListId = element.Id;
                row.AreaListName = element.ListName;
                row.AreaTypeId = element.AreaTypeId;
                row.AreaTypeName = _this.areaTypes.find(function (x) { return x.Id === element.AreaTypeId; }).Short;
                row.UserId = element.UserId;
                row.PublicId = element.PublicId;
                _this.areaListRows.push(row);
            });
            _this.hideSpinner();
        });
    };
    ArealistIndexComponent.prototype.displayPage = function () {
        var windowLocationHref = window.location.href.trim().toLowerCase();
        if (windowLocationHref.indexOf('area-list') > 0 &&
            windowLocationHref.indexOf('create') <= 0 &&
            windowLocationHref.indexOf('edit') <= 0) {
            return true;
        }
        else {
            return false;
        }
    };
    ArealistIndexComponent.prototype.isAnyData = function () {
        if (this.areaLists.length > 0) {
            return true;
        }
        else {
            return false;
        }
    };
    ArealistIndexComponent.prototype.hideSpinner = function () {
        $('#spinner').hide();
    };
    ArealistIndexComponent.prototype.sort = function (column) {
        var _this = this;
        var tempAreaListRows;
        tempAreaListRows = this.areaListRows.slice(0);
        this.sortOrder = this.getSortOrder(column);
        tempAreaListRows.sort(function (a, b) {
            if (_this.sortOrder) {
                switch (column) {
                    case SortColumns.AreaListName:
                        if (a.AreaListName.toLowerCase() < b.AreaListName.toLowerCase()) {
                            return -1;
                        }
                        else if (a.AreaListName.toLowerCase() > b.AreaListName.toLowerCase()) {
                            return 1;
                        }
                        else {
                            return 0;
                        }
                    case SortColumns.AreaTypeName:
                        if (a.AreaTypeName.toLowerCase() < b.AreaTypeName.toLowerCase()) {
                            return -1;
                        }
                        else if (a.AreaTypeName.toLowerCase() > b.AreaTypeName.toLowerCase()) {
                            return 1;
                        }
                        else {
                            return 0;
                        }
                }
            }
            else {
                switch (column) {
                    case SortColumns.AreaListName:
                        if (a.AreaListName.toLowerCase() < b.AreaListName.toLowerCase()) {
                            return 1;
                        }
                        else if (a.AreaListName.toLowerCase() > b.AreaListName.toLowerCase()) {
                            return -1;
                        }
                        else {
                            return 0;
                        }
                    case SortColumns.AreaTypeName:
                        if (a.AreaTypeName.toLowerCase() < b.AreaTypeName.toLowerCase()) {
                            return 1;
                        }
                        else if (a.AreaTypeName.toLowerCase() > b.AreaTypeName.toLowerCase()) {
                            return -1;
                        }
                        else {
                            return 0;
                        }
                }
            }
        });
        this.areaListRows = tempAreaListRows;
    };
    ArealistIndexComponent.prototype.getSortOrder = function (column) {
        var sortOrder = false;
        switch (column) {
            case SortColumns.AreaListName:
                this.sortOrderAreaListName = !this.sortOrderAreaListName;
                sortOrder = this.sortOrderAreaListName;
                break;
            case SortColumns.AreaTypeName:
                this.sortOrderAreaTypeName = !this.sortOrderAreaTypeName;
                sortOrder = this.sortOrderAreaTypeName;
                break;
        }
        return sortOrder;
    };
    ArealistIndexComponent.prototype.helpViewAreaListOnDataPage = function () {
        // Show light box
        var config = new light_box_component_1.LightBoxConfig();
        config.Type = light_box_component_1.LightBoxTypes.Ok;
        config.Title = 'How to view your area lists';
        config.Html = 'When viewing the data for a profile select the "Your area lists" option in the "Areas grouped by" menu.<br><br>' +
            '<img src="../images/help-view-area-list-on-data-page.png"><br><br>' +
            'You will then be able to view any existing area lists or else create new ones.';
        config.Height = 540;
        config.Top = 200;
        this.lightBoxConfig = config;
    };
    ArealistIndexComponent.prototype.deleteAreaListConfirmation = function (row) {
        this.areaListId = row.AreaListId;
        this.selectedRecordUserId = row.UserId;
        this.publicId = row.PublicId;
        this.actionType = ActionTypes.Delete;
        // Show light box for delete
        var config = new light_box_component_1.LightBoxConfig();
        config.Top = 400;
        config.Type = light_box_component_1.LightBoxTypes.OkCancel;
        config.Title = 'Delete';
        config.Html = 'Are you sure you want to delete this list?';
        this.lightBoxConfig = config;
    };
    ArealistIndexComponent.prototype.updateLightBoxActionConfirmed = function (actionConfirmed) {
        if (actionConfirmed) {
            if (this.actionType === ActionTypes.Delete) {
                this.deleteAreaList();
            }
        }
    };
    ArealistIndexComponent.prototype.deleteAreaList = function () {
        var _this = this;
        var formData = new FormData();
        formData.append('areaListId', this.areaListId.toString());
        formData.append('userId', this.selectedRecordUserId.toString());
        formData.append('publicId', this.publicId.toString());
        this.areaListService.deleteAreaList(formData)
            .subscribe(function (response) {
            if (response.status === 200) {
                window.location.reload(true);
            }
        }, function (error) {
            // Show light box
            var config = new light_box_component_1.LightBoxConfig();
            config.Top = 400;
            config.Type = light_box_component_1.LightBoxTypes.OkCancel;
            config.Title = 'Failed';
            config.Html = 'Failed to delete the area list, please try again.<br>' +
                'If the issue persists then please contact the administrator.';
            _this.lightBoxConfig = config;
        });
    };
    ArealistIndexComponent.prototype.copyAreaListConfirmation = function (areaListId) {
        this.areaListId = areaListId;
        this.areaListName = this.areaLists.find(function (x) { return x.Id === areaListId; }).ListName;
        this.actionType = ActionTypes.Copy;
        // Show light box
        var config = new light_box_with_input_component_1.LightBoxWithInputConfig();
        config.Type = light_box_with_input_component_1.LightBoxWithInputTypes.OkCancel;
        config.Title = 'Copy area list';
        config.Html = 'Name of new list';
        config.InputText = this.areaListName + ' copy';
        this.lightBoxWithInputConfig = config;
    };
    ArealistIndexComponent.prototype.updateLightBoxInputText = function (areaListName) {
        this.areaListName = areaListName;
    };
    ArealistIndexComponent.prototype.updateLightBoxWithInputActionConfirmed = function (actionConfirmed) {
        if (actionConfirmed) {
            if (this.actionType === ActionTypes.Copy) {
                this.copyAreaList();
            }
        }
    };
    ArealistIndexComponent.prototype.copyAreaList = function () {
        var _this = this;
        var areaList = this.areaLists.find(function (x) { return x.ListName === _this.areaListName; });
        if (areaList === undefined || areaList === null) {
            var formData = new FormData();
            formData.append('areaListId', this.areaListId.toString());
            formData.append('areaListName', this.areaListName);
            formData.append('userId', this.userId);
            this.areaListService.copyAreaList(formData)
                .subscribe(function (response) {
                if (response.status === 200) {
                    window.location.reload(true);
                }
            }, function (error) {
                // Show light box
                var config = new light_box_component_1.LightBoxConfig();
                config.Type = light_box_component_1.LightBoxTypes.OkCancel;
                config.Title = 'Failed';
                config.Html = 'Failed to copy the area list, please try again.<br>' +
                    'If the issue persists then please contact the administrator.';
                _this.lightBoxConfig = config;
            });
        }
        else {
            // Show light box
            var config = new light_box_component_1.LightBoxConfig();
            config.Type = light_box_component_1.LightBoxTypes.Ok;
            config.Title = 'Area list name already taken';
            config.Html = 'Another area list already has that name. Please choose a different one.';
            this.lightBoxConfig = config;
        }
    };
    return ArealistIndexComponent;
}());
ArealistIndexComponent = __decorate([
    core_1.Component({
        selector: 'ft-arealist-index',
        template: __webpack_require__("./src/app/arealist/arealist-index/arealist-index.component.html"),
        styles: [__webpack_require__("./src/app/arealist/arealist-index/arealist-index.component.css")],
        providers: [arealist_service_1.AreaListService]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof arealist_service_1.AreaListService !== "undefined" && arealist_service_1.AreaListService) === "function" && _a || Object, typeof (_b = typeof area_service_1.AreaService !== "undefined" && area_service_1.AreaService) === "function" && _b || Object, typeof (_c = typeof core_1.ChangeDetectorRef !== "undefined" && core_1.ChangeDetectorRef) === "function" && _c || Object])
], ArealistIndexComponent);
exports.ArealistIndexComponent = ArealistIndexComponent;
var AreaListRow = (function () {
    function AreaListRow() {
    }
    return AreaListRow;
}());
exports.AreaListRow = AreaListRow;
var SortColumns = (function () {
    function SortColumns() {
    }
    return SortColumns;
}());
SortColumns.AreaListName = 1;
SortColumns.AreaTypeName = 2;
exports.SortColumns = SortColumns;
;
var ActionTypes = (function () {
    function ActionTypes() {
    }
    return ActionTypes;
}());
ActionTypes.Info = "INFO";
ActionTypes.Delete = "DELETE";
ActionTypes.Copy = "COPY";
exports.ActionTypes = ActionTypes;
var _a, _b, _c;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/arealist-index.component.js.map

/***/ }),

/***/ "./src/app/arealist/arealist-manage/arealist-manage.component.css":
/***/ (function(module, exports) {

module.exports = ".top-margin {\r\n    margin-top: 10px;\r\n}\r\n\r\n.available-areas-list {\r\n    height: 400px;\r\n    overflow-y: scroll;\r\n}\r\n\r\n.available-areas,\r\n.searched-areas,\r\n.selected-areas {\r\n    border-top: 1px solid #ccc;\r\n    margin: 0px;\r\n    padding-top: 5px;\r\n    padding-bottom: 5px;\r\n}\r\n\r\n.cursor-pointer {\r\n    cursor: pointer;\r\n}\r\n\r\n#areaSearchText {\r\n    background: #FFF url(\"data:image/jpeg;base64,/9j/4AAQSkZJRgABAgAAZABkAAD/7AARRHVja3kAAQAEAAAAZAAA/+4ADkFkb2JlAGTAAAAAAf/bAIQAAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQICAgICAgICAgICAwMDAwMDAwMDAwEBAQEBAQECAQECAgIBAgIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMD/8AAEQgAGAAYAwERAAIRAQMRAf/EAFwAAQEBAQAAAAAAAAAAAAAAAAgHBAoBAQAAAAAAAAAAAAAAAAAAAAAQAAIDAQABBQADAQAAAAAAAAIDAQQFBgcAERITCCExQSIRAQAAAAAAAAAAAAAAAAAAAAD/2gAMAwEAAhEDEQA/AO93pOjxOQwdbp+k0UZODhUX6Wro2fslVSnWCTayQSDXtP8AwFrE2MOYERIpiJAi0/3L45ZYzb+rwnlrnOA2b45uT5Q3OOmrxNpxt+oLE313XMikRCc/JYtaIj7ksf8Ar4Az69hFtCLVV6bNWylditZrsByLCHALEvQ5ZEtqWrKCEhmRIZiYn29Adv1rwnTeSPAPectyCXXN56cjSq5aCkW668Tczte1mpj5hDbDq1IySufl9rgAIj3KJgCP5e/Y3h3rPz90fA4vN7Ydjr8m3mm+PbHOXaiOKfTqKW67at/QvOHP5Y60urfURNg66/sUqPl8QbP5t08jT8EeKSxt+p0lbP4bm8exo1GGYBoZOVVo36DQbA2K7s2ykkktogwYCPcY/r0Fu9BkZn0HFYNtKo07iCq2yZWSZWqxj8Dr2CIJl6CCPaQL3GY/j29BKPFPgzhfDNztLHCq1M+n22ynatYjtJz8LGYivKYq4GZ7AihWJjDOZmDbIyK/n9SlAAf/2Q==\") right center no-repeat;\r\n    width: 100%;\r\n}\r\n\r\n.clear-list-button {\r\n    padding: 0px;\r\n    font-size: 14px;\r\n    text-decoration: underline;\r\n    color: #2e3191;\r\n}\r\n\r\n#spinner {\r\n    margin: 200px 0;\r\n    color: #999;\r\n    font-size: 10px;\r\n    text-align: center;\r\n    display: none;\r\n}\r\n\r\n#lightBox {\r\n    display: none;\r\n}\r\n\r\n#infoBox {\r\n    display: none;\r\n    width: 438px;\r\n    top: 50%;\r\n    left: 50%;\r\n    margin-top: -50px;\r\n    margin-left: -50px;\r\n}\r\n\r\n.btn-link {\r\n    color: #2e3191;\r\n}"

/***/ }),

/***/ "./src/app/arealist/arealist-manage/arealist-manage.component.html":
/***/ (function(module, exports) {

module.exports = "<form [formGroup]=\"arealistForm\" *ngIf=\"isAnyData()\">\r\n    <div formGroupName=\"arealist\">\r\n        <div class=\"row top-margin\">\r\n            <div class=\"col-md-6\">\r\n                <h6 class=\"font-weight-bold\">Area type</h6>\r\n                <select id=\"areaTypeList\" formControlName=\"areaTypeList\" class=\"form-control\" (change)=\"changeAreaTypeList()\"\r\n                    *ngIf=\"isCreate()\">\r\n                    <option [value]=\"-1\">SELECT AREA TYPE</option>\r\n                    <option *ngFor=\"let areaType of areaTypes; let last=last\" [value]=\"areaType.Id\">\r\n                        {{areaType.Short}} {{last? setDefaultOption() : ''}}\r\n                    </option>\r\n                </select>\r\n                <h6 id=\"areaTypeName\" [innerHtml]=\"areaTypeName\" *ngIf=\"!isCreate()\">\r\n                </h6>\r\n            </div>\r\n            <div class=\"col-md-6\">\r\n                <h6 class=\"font-weight-bold\">List name</h6>\r\n                <input type=\"text\" id=\"areaListName\" formControlName=\"areaListName\" class=\"form-control\" [value]=\"this.areaListName\">\r\n            </div>\r\n        </div>\r\n        <div class=\"row top-margin\">\r\n            <div class=\"col-md-6\">\r\n                &nbsp;\r\n            </div>\r\n            <div class=\"col-md-6\">\r\n                <button id=\"btn-save-area-list\" class=\"btn btn-primary\" (click)=\"saveAreaList()\">Save list</button>\r\n                <button id=\"btn-cancel\" class=\"btn btn-default\" (click)=\"cancel()\">Cancel</button>\r\n                <br />\r\n                <div id=\"error-details\" class=\"top-margin text-danger\">\r\n                </div>\r\n            </div>\r\n        </div>\r\n        <hr>\r\n        <div class=\"row\" *ngIf=\"isPracticeSearch()\">\r\n            <div class=\"col-md-12\" id=\"practice-search-map\">\r\n                <ft-practice-search-simple [IsMapUpdateRequired]=\"updateMap\" [areaTypeId]=\"areaTypeId\"\r\n                    [selectedAreaAddresses]=\"selectedAreaAddresses\" [searchMode]=\"searchMode\" #practiceSearch\r\n                    (emitSelectedPractices)=\"updateSelectedPractices($event)\" (emitShowPracticesAsList)=\"showPracticesAsList($event)\"></ft-practice-search-simple>\r\n            </div>\r\n        </div>\r\n        <div id=\"area-list-details\" class=\"row\" *ngIf=\"isCreateAndAreaTypeSelected()\">\r\n            <div class=\"col-md-6\">\r\n                <div *ngIf=\"isList()\">\r\n                    <h3>Select an area to add it to your list</h3>\r\n                </div>\r\n                <div id=\"area-list-table\" class=\"top-margin\" *ngIf=\"isList()\">\r\n                    <div class=\"top-margin\">\r\n                        Map is not available for\r\n                        <span [innerHtml]=\"areaTypeName\"></span>\r\n                    </div>\r\n                    <ft-arealist-areas [areaTypeId]=\"areaTypeId\" [selectedAreas]=\"selectedAreas\" (emitAreas)=\"updateAvailableAreas($event)\"\r\n                        (emitSelectedArea)=\"updateSelectedArea($event)\" (emitDeSelectedArea)=\"updateDeSelectedArea($event)\"\r\n                        (emitMouseOverArea)=\"updateMouseOverArea($event)\" (emitMouseOutArea)=\"updateMouseOutArea($event)\">\r\n                    </ft-arealist-areas>\r\n                </div>\r\n                <div id=\"area-list-map\" *ngIf=\"isMap()\">\r\n                    <h3>Select an area on the map to add it to your list</h3>\r\n                    <button class=\"btn btn-link clear-list-button\" (click)=\"showMapList()\">Show all areas as a list</button>\r\n                    <div id=\"map-container\" class=\"standard-width clearfix\">\r\n                        <div class=\"top-margin\" style=\"display: none\">\r\n                            <button id=\"toggleMapHeading\" class=\"btn btn-link clear-list-button\" (click)=\"toggleMap()\">\r\n                                Show all areas as list\r\n                            </button>\r\n                        </div>\r\n                        <ft-google-map-simple (mapInit)=\"onMapInit($event)\" [areaTypeId]=\"areaTypeId\" [areaCode]=\"areaCode\"\r\n                            [selectedAreas]=\"selectedAreas\" [availableAreas]=\"availableAreas\" [mapAreaCodes]=\"mapAreaCodes\"\r\n                            [mapPolygonSelected]=\"mapPolygonSelected\" [areaSearchText]=\"areaSearchText\"\r\n                            (selectedAreaChanged)=\"onSelectedAreaChanged($event)\">\r\n                        </ft-google-map-simple>\r\n                    </div>\r\n                </div>\r\n                <div id=\"area-list-list\" *ngIf=\"isMapList()\">\r\n                    <h3>Select an area to add it to your list</h3>\r\n                    <button class=\"btn btn-link clear-list-button\" (click)=\"showMap()\">Show all areas as a map</button>\r\n                    <ft-arealist-areas [areaTypeId]=\"areaTypeId\" [selectedAreas]=\"selectedAreas\" (emitAreas)=\"updateAvailableAreas($event)\"\r\n                        (emitSelectedArea)=\"updateSelectedArea($event)\" (emitDeSelectedArea)=\"updateDeSelectedArea($event)\"\r\n                        (emitMouseOverArea)=\"updateMouseOverArea($event)\" (emitMouseOutArea)=\"updateMouseOutArea($event)\">\r\n                    </ft-arealist-areas>\r\n                </div>\r\n            </div>\r\n            <div class=\"col-md-3\" *ngIf=\"displaySelectedAreasSection\">\r\n                <h3>Search for an area</h3>\r\n                <div>\r\n                    <input type=\"text\" id=\"areaSearchText\" formControlName=\"areaSearchText\" class=\"form-control\"\r\n                        (keyup)=\"searchArea()\" [value]=\"areaSearchText\">\r\n                </div>\r\n                <div class=\"top-margin\">\r\n                    <span *ngIf=\"searchedAreas.length > 0\">\r\n                        Select area to add it to your list\r\n                    </span>\r\n                    <div class=\"top-margin\">\r\n                        <div *ngFor=\"let area of searchedAreas; let last=last\" #elem [attr.id]=\"area.Code\" class=\"searched-areas searched-areas-{{area.Code}}\"\r\n                            (mouseover)=\"mouseOverArea('searched-areas', area.Code)\" (mouseout)=\"mouseOutArea('searched-areas', area.Code)\"\r\n                            (click)=\"moveArea(area.Code)\">\r\n                            {{area.Name}} {{last ? decorateSearchedAreasAfterInitialLoad() : ''}}\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n            <div class=\"col-md-3\" *ngIf=\"displaySelectedAreasSection\">\r\n                <h3>\r\n                    Areas in your list\r\n                    <br />\r\n                    <button class=\"btn btn-link clear-list-button\" (click)=\"clearList()\">Clear list</button>\r\n                </h3>\r\n                <div class=\"top-margin\">\r\n                    <span *ngIf=\"selectedAreas.length > 0\">\r\n                        Select an area below to remove it from your list\r\n                    </span>\r\n                    <div class=\"top-margin\">\r\n                        <div *ngFor=\"let area of selectedAreas; let last=last\" #elem [attr.id]=\"area.Code\" class=\"selected-areas selected-areas-{{area.Code}}\"\r\n                            (mouseover)=\"mouseOverArea('selected-areas', area.Code)\" (mouseout)=\"mouseOutArea('selected-areas', area.Code)\"\r\n                            (click)=\"moveAreaOut('selected-areas', area.Code)\">\r\n                            {{area.Name}} {{last ? decorateAvailableAreasAfterInitialLoad() : ''}}\r\n                        </div>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n    <ft-light-box [lightBoxConfig]=\"lightBoxConfig\" (emitLightBoxActionConfirmed)=\"updateLightBoxActionConfirmed($event)\"></ft-light-box>\r\n</form>"

/***/ }),

/***/ "./src/app/arealist/arealist-manage/arealist-manage.component.ts":
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
var forms_1 = __webpack_require__("./node_modules/@angular/forms/@angular/forms.es5.js");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var area_service_1 = __webpack_require__("./src/app/shared/service/api/area.service.ts");
var arealist_service_1 = __webpack_require__("./src/app/shared/service/api/arealist.service.ts");
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var light_box_component_1 = __webpack_require__("./src/app/shared/component/light-box/light-box.component.ts");
var ArealistManageComponent = (function () {
    function ArealistManageComponent(ftHelperService, areaService, arealistService, ref) {
        this.ftHelperService = ftHelperService;
        this.areaService = areaService;
        this.arealistService = arealistService;
        this.ref = ref;
        this.isInitialised = false;
        this.areaCodeColour = new Map();
        this.areaCodeColourValue = new Map();
        this.refreshColour = 0;
        this.isBoundaryNotSupported = false;
        this.mapColourSelectedValue = 'benchmark';
        this.selectedAreaList = new Array();
        this.flag = true;
        this.areas = [];
        this.areaTypes = [];
        this.areaLists = [];
        this.availableAreas = [];
        this.searchedAreas = [];
        this.selectedAreas = [];
        this.availablePractices = [];
        this.areaCodes = [];
        this.mapAreaCodes = [];
        this.selectedPractices = [];
        this.selectedAreaAddresses = [];
        this.firstTimeLoad = true;
        this.displayMap = true;
        this.displaySelectedAreasSection = false;
        // Setup new form group and form controls
        this.arealistForm = new forms_1.FormGroup({
            'arealist': new forms_1.FormGroup({
                'areaSearchText': new forms_1.FormControl(null),
                'areaTypeList': new forms_1.FormControl(null),
                'areaListName': new forms_1.FormControl(null)
            })
        });
    }
    ArealistManageComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.userId = document.getElementById('ft-arealist-manage').getAttribute('user-id');
        this.publicId = document.getElementById('ft-arealist-manage').getAttribute('public-id');
        this.actionType = document.getElementById('ft-arealist-manage').getAttribute('action-type');
        this.areaSearchText = '';
        this.resolveQueryStringParams();
        if (this.actionType === 'create') {
            this.create = true;
            this.areaListName = '';
        }
        else {
            $('#spinner').show();
            this.create = false;
            this.arealistService.getAreaListByPublicId(this.publicId, this.userId)
                .subscribe(function (areaListResult) {
                _this.areaList = areaListResult;
                _this.areaListName = _this.areaList.ListName;
                _this.areaTypeId = _this.areaList.AreaTypeId;
                _this.areaService.getAreas(_this.areaTypeId)
                    .subscribe(function (areaResult) {
                    _this.availableAreas = areaResult;
                    _this.ignoreLegacyAreas();
                });
                _this.areaList.AreaListAreaCodes.forEach(function (element) {
                    _this.areaCodes.push(element.AreaCode);
                });
                if (_this.areaTypeId === shared_1.AreaTypeIds.Practice) {
                    _this.LoadSelectedPractices();
                }
                else {
                    _this.loadSelectedAreas();
                }
            });
            this.displaySelectedAreasSection = true;
            this.ref.detectChanges();
        }
        this.loadAvailableAreaLists();
        this.loadAreaTypes();
    };
    ArealistManageComponent.prototype.ignoreLegacyAreas = function () {
        var _this = this;
        var areaCodesToIgnore = [];
        areaCodesToIgnore.push('09');
        areaCodesToIgnore.forEach(function (areaCode) {
            var areaCodeIndex = _this.availableAreas.findIndex(function (x) { return x.Code === areaCode; });
            _this.availableAreas.splice(areaCodeIndex, 1);
        });
    };
    ArealistManageComponent.prototype.loadAvailableAreaLists = function () {
        var _this = this;
        this.arealistService.getAreaLists(this.userId)
            .subscribe(function (result) {
            _this.areaLists = result;
        });
    };
    ArealistManageComponent.prototype.loadAreaTypes = function () {
        var _this = this;
        this.areaService.getAreaTypes()
            .subscribe(function (result) {
            _this.areaTypes = result;
            if (_this.actionType === 'edit') {
                var areaType = _this.areaTypes.find(function (x) { return x.Id === _this.areaTypeId; });
                if (areaType !== undefined && areaType !== null) {
                    _this.areaTypeName = _this.areaTypes.find(function (x) { return x.Id === _this.areaTypeId; }).Short;
                }
                else {
                    _this.areaTypeName = '';
                }
            }
            $('#spinner').hide();
        });
    };
    ArealistManageComponent.prototype.loadAvailableAreas = function () {
        var _this = this;
        this.areaService.getAreas(this.areaTypeId)
            .subscribe(function (result) {
            _this.availableAreas = result;
            _this.ignoreLegacyAreas();
        });
    };
    ArealistManageComponent.prototype.loadSelectedAreas = function () {
        var _this = this;
        this.arealistService.getAreasFromAreaListAreaCodes(this.areaCodes)
            .subscribe(function (result) {
            var selectedAreas = result;
            var tempSelectedAreas;
            if (selectedAreas.length > 0) {
                tempSelectedAreas = selectedAreas.slice(0);
                tempSelectedAreas.sort(function (a, b) {
                    if (a.Name < b.Name) {
                        return -1;
                    }
                    else if (a.Name > b.Name) {
                        return 1;
                    }
                    else {
                        return 0;
                    }
                });
            }
            _this.selectedAreas = tempSelectedAreas;
        });
    };
    ArealistManageComponent.prototype.LoadSelectedPractices = function () {
        var _this = this;
        this.arealistService.getAreasWithAddressFromAreaListAreaCodes(this.areaCodes)
            .subscribe(function (result) {
            _this.selectedAreaAddresses = result;
        });
    };
    // Initialise the map
    ArealistManageComponent.prototype.onMapInit = function (mapInfo) {
        this.map = mapInfo.map;
    };
    // Capture the event emitted by google maps component when polygon (area) gets changed
    ArealistManageComponent.prototype.onSelectedAreaChanged = function (eventDetail) {
        // Find the selected area from the list of available areas
        var areaSelected = this.availableAreas.find(function (x) { return x.Code === eventDetail.areaCode; });
        // If area on the map is selected then add to the selected areas array
        if (eventDetail.add) {
            var areaToAdd = this.selectedAreas.find(function (x) { return x.Code === eventDetail.areaCode; });
            if (areaToAdd === undefined) {
                this.selectedAreas.push(areaSelected);
            }
        }
        else {
            // If the area already exists in the selected areas array, then remove it
            var indexOfSelectedArea = this.selectedAreas.findIndex(function (x) { return x.Code === areaSelected.Code; });
            this.selectedAreas.splice(indexOfSelectedArea, 1);
        }
        this.ref.detectChanges();
    };
    ArealistManageComponent.prototype.changeAreaTypeList = function () {
        var _this = this;
        var tempAreaTypeId = this.areaTypeId;
        var tempAvailableAreas = this.availableAreas;
        this.areaTypeId = Number(this.arealistForm.get('arealist').get('areaTypeList').value);
        if (this.selectedAreas.length > 0 || this.selectedPractices.length > 0) {
            this.arealistForm.get('arealist').get('areaTypeList').setValue(tempAreaTypeId);
            this.areaTypeId = tempAreaTypeId;
            this.availableAreas = tempAvailableAreas;
            // Show lightbox
            this.showAreaTypes();
        }
        else {
            this.loadAvailableAreas();
            if (this.areaTypeId !== -1) {
                this.areaTypeName = this.areaTypes.find(function (x) { return x.Id === _this.areaTypeId; }).Short;
            }
            this.displayMap = true;
        }
        this.displaySelectedAreasSection = true;
    };
    ArealistManageComponent.prototype.showAreaTypes = function () {
        var config = new light_box_component_1.LightBoxConfig();
        config.Type = light_box_component_1.LightBoxTypes.Ok;
        config.Title = 'One area type per list';
        config.Html = 'All the areas in a list must be from the same area type.<br />' +
            'Please clear the list first if you need to use a different area type.';
        config.Height = 200;
        this.lightBoxConfig = config;
    };
    ArealistManageComponent.prototype.setDefaultOption = function () {
        if (this.areaTypeId === undefined) {
            if (this.firstTimeLoad) {
                this.arealistForm.get('arealist').get('areaTypeList').setValue('-1');
                this.areaTypeId = -1;
                this.firstTimeLoad = false;
            }
        }
        else {
            this.arealistForm.get('arealist').get('areaTypeList').setValue(this.areaTypeId);
            this.loadAvailableAreas();
            if (this.availableAreas.length > 0) {
                this.displaySelectedAreasSection = true;
            }
        }
    };
    ArealistManageComponent.prototype.updateAvailableAreas = function (areas) {
        this.availableAreas = areas;
    };
    ArealistManageComponent.prototype.updateSelectedPractices = function (selectedPractices) {
        this.selectedPractices = selectedPractices;
    };
    ArealistManageComponent.prototype.updateSelectedArea = function (selectedArea) {
        this.moveArea(selectedArea.Code);
    };
    ArealistManageComponent.prototype.updateDeSelectedArea = function (deselectedArea) {
        this.moveArea(deselectedArea.Code);
    };
    ArealistManageComponent.prototype.updateMouseOverArea = function (area) {
        this.mouseOverArea('areas', area.Code);
    };
    ArealistManageComponent.prototype.updateMouseOutArea = function (area) {
        this.mouseOutArea('areas', area.Code);
    };
    ArealistManageComponent.prototype.updateLightBoxActionConfirmed = function (actionConfirmed) {
    };
    ArealistManageComponent.prototype.showPracticesAsList = function () {
        this.practicesAsList = true;
    };
    // Method to decide whether a map can be displayed
    ArealistManageComponent.prototype.canAreaTypeBeDisplayedOnMap = function () {
        var _this = this;
        if (this.areaTypeId === null || this.areaTypeId === -1) {
            return false;
        }
        if (this.areaTypes === null || this.areaTypes.length === 0) {
            return false;
        }
        return this.areaTypes.find(function (x) { return x.Id === _this.areaTypeId; }).CanBeDisplayedOnMap;
    };
    // Method to decide whether to load the contents of the page during initial load
    // The loading of the content is decided based on the url
    ArealistManageComponent.prototype.isAnyData = function () {
        var windowLocationHref = window.location.href.trim().toLowerCase();
        if (windowLocationHref.indexOf('area-list') > 0 &&
            (windowLocationHref.indexOf('create') > 0 ||
                windowLocationHref.indexOf('edit') > 0)) {
            return true;
        }
        else {
            return false;
        }
    };
    ArealistManageComponent.prototype.isList = function () {
        if (this.areaTypeId === null || this.areaTypeId === -1) {
            return false;
        }
        if (!this.canAreaTypeBeDisplayedOnMap() && !this.isPracticeSearch()) {
            return true;
        }
        else {
            return false;
        }
    };
    // Method to decided whether the page contains a map
    ArealistManageComponent.prototype.isMap = function () {
        if (this.canAreaTypeBeDisplayedOnMap() && !this.isPracticeSearch() && this.displayMap) {
            return true;
        }
        else {
            return false;
        }
    };
    ArealistManageComponent.prototype.isMapList = function () {
        if (this.canAreaTypeBeDisplayedOnMap() && !this.isPracticeSearch() && !this.displayMap) {
            return true;
        }
        else {
            return false;
        }
    };
    ArealistManageComponent.prototype.isPracticeSearch = function () {
        if (this.areaTypeId !== undefined && this.areaTypeId === shared_1.AreaTypeIds.Practice) {
            return true;
        }
        else {
            return false;
        }
    };
    ArealistManageComponent.prototype.isCreate = function () {
        if (this.actionType === 'create') {
            return true;
        }
        else {
            return false;
        }
    };
    ArealistManageComponent.prototype.showMap = function () {
        this.displayMap = true;
    };
    ArealistManageComponent.prototype.showMapList = function () {
        this.displayMap = false;
    };
    // Display the bottom section of the page only if at least one of the below condition satisfy
    // 1) Edit area list functionality
    // 2) Create area list functionality with area type selected
    ArealistManageComponent.prototype.isCreateAndAreaTypeSelected = function () {
        if (this.actionType === 'edit' && this.areaTypeId !== undefined && this.areaTypeId !== shared_1.AreaTypeIds.Practice) {
            return true;
        }
        else if (this.actionType === 'create' && this.areaTypeId !== undefined && this.areaTypeId !== shared_1.AreaTypeIds.Practice) {
            return true;
        }
        else {
            return false;
        }
    };
    // Method for auto-complete search
    ArealistManageComponent.prototype.searchArea = function () {
        var _this = this;
        // Read the characters typed by the user and convert it to lower case
        this.areaSearchText = this.arealistForm.get('arealist').get('areaSearchText').value.toLowerCase();
        // If the user has edited the area search text
        if (this.areaSearchText !== undefined && this.areaSearchText !== null) {
            // Clear the areas searches array
            this.searchedAreas.length = 0;
            // populate the areas searched array based on the user input
            this.availableAreas.forEach(function (element) {
                if (_this.areaSearchText.length > 0 && element.Name.toLowerCase().indexOf(_this.areaSearchText) > -1) {
                    _this.searchedAreas.push(element);
                }
            });
        }
    };
    ArealistManageComponent.prototype.decorateSearchedAreasAfterInitialLoad = function () {
        this.decorateAreas('.searched-areas-', this.searchedAreas);
    };
    ArealistManageComponent.prototype.decorateAvailableAreasAfterInitialLoad = function () {
        this.decorateAreas('.areas-', this.selectedAreas);
    };
    ArealistManageComponent.prototype.decorateAreas = function (className, areas) {
        var _this = this;
        areas.forEach(function (element) {
            var elementInSelectedAreas = _this.selectedAreas.find(function (x) { return x.Code === element.Code; });
            if (elementInSelectedAreas !== null && elementInSelectedAreas !== undefined) {
                $(className + element.Code).addClass('bg-info bg-primary text-white');
            }
        });
    };
    ArealistManageComponent.prototype.mouseOverArea = function (itemType, item) {
        if (!$('.' + itemType + '-' + item).hasClass('bg-info')) {
            $('.' + itemType + '-' + item).addClass('bg-primary text-white cursor-pointer');
        }
    };
    ArealistManageComponent.prototype.mouseOutArea = function (itemType, item) {
        if (!$('.' + itemType + '-' + item).hasClass('bg-info')) {
            $('.' + itemType + '-' + item).removeClass('bg-primary text-white cursor-pointer');
        }
    };
    ArealistManageComponent.prototype.moveArea = function (item) {
        var areaInAvailableAreas = this.availableAreas.find(function (x) { return x.Code === item; });
        var areaInSearchedAreas;
        var areaInSelectedAreas;
        var areaSelected;
        this.areaCode = item;
        if (this.searchedAreas !== undefined) {
            areaInSearchedAreas = this.searchedAreas.find(function (x) { return x.Code === item; });
        }
        if (this.selectedAreas !== undefined) {
            areaInSelectedAreas = this.selectedAreas.find(function (x) { return x.Code === item; });
        }
        if (areaInAvailableAreas !== undefined) {
            areaSelected = areaInAvailableAreas;
        }
        else if (areaInSearchedAreas !== undefined) {
            areaSelected = areaInSearchedAreas;
        }
        else {
            return false;
        }
        if (areaSelected !== undefined && areaInSelectedAreas !== undefined && areaSelected.Code === areaInSelectedAreas.Code) {
            var indexOfSelectedArea = this.selectedAreas.findIndex(function (x) { return x.Code === areaInSelectedAreas.Code; });
            this.selectedAreas.splice(indexOfSelectedArea, 1);
            if (areaInAvailableAreas !== undefined) {
                $('.areas-' + item).removeClass('bg-info bg-primary text-white');
            }
            if (areaInSearchedAreas !== undefined) {
                $('.searched-areas-' + item).removeClass('bg-info bg-primary text-white');
            }
            if (this.isMap()) {
                this.mapPolygonSelected = false;
            }
        }
        else {
            this.selectedAreas.push(areaInAvailableAreas);
            if (areaInAvailableAreas !== undefined) {
                $('.areas-' + item).addClass('bg-info text-white');
            }
            if (areaInSearchedAreas !== undefined) {
                $('.searched-areas-' + item).addClass('bg-info text-white');
            }
            if (this.isMap()) {
                this.mapPolygonSelected = true;
            }
        }
    };
    ArealistManageComponent.prototype.moveAreaOut = function (itemType, item) {
        var areaInSelectedAreas = this.selectedAreas.find(function (x) { return x.Code === item; });
        var indexOfSelectedArea = this.selectedAreas.indexOf(areaInSelectedAreas);
        this.selectedAreas.splice(indexOfSelectedArea, 1);
        var areaInAvailableAreas = this.availableAreas.find(function (x) { return x.Code === item; });
        if (areaInAvailableAreas !== undefined) {
            $('.areas-' + item).removeClass('bg-info bg-primary text-white');
        }
        var areaInSearchedAreas = this.searchedAreas.find(function (x) { return x.Code === item; });
        if (areaInSearchedAreas !== undefined) {
            $('.searched-areas-' + item).removeClass('bg-info bg-primary text-white');
        }
        if (this.isMap()) {
            this.mapPolygonSelected = false;
            this.areaCode = item;
            this.ref.detectChanges();
        }
    };
    ArealistManageComponent.prototype.toggleMap = function () {
        var $areaListTable = $('#area-list-table');
        var $toggleMapHeading = $('#toggleMapHeading');
        $areaListTable.toggle();
        $('#area-list-map').toggle();
        if ($areaListTable.is(':visible')) {
            $toggleMapHeading.html('Show all areas as a map');
        }
        else {
            $toggleMapHeading.html('Show all areas as a list');
        }
    };
    // Method to validate
    ArealistManageComponent.prototype.validateSave = function () {
        var _this = this;
        var isValid = true;
        if (this.areaListName === null || this.areaListName === undefined || this.areaListName === '') {
            this.areaListName = this.arealistForm.get('arealist').get('areaListName').value;
        }
        if (this.areaListName === null || this.areaListName.trim().length === 0) {
            if (this.areaTypeId === null || this.areaTypeId === -1) {
                $('#error-details').html('Please enter a valid list name, select an area type and add areas to the list');
            }
            else {
                $('#error-details').html('Please enter a valid list name');
            }
            isValid = false;
        }
        if (isValid && (this.areaTypeId !== shared_1.AreaTypeIds.Practice && this.selectedAreas.length === 0)) {
            if (this.areaTypeId === null || this.areaTypeId === -1) {
                $('#error-details').html('Please select an area type and add areas to the list');
            }
            else {
                $('#error-details').html('Please add areas to the list');
            }
            isValid = false;
        }
        if (isValid && (this.areaTypeId === shared_1.AreaTypeIds.Practice && this.selectedPractices.length === 0)) {
            $('#error-details').html('Please add practices to the list');
            isValid = false;
        }
        if (isValid && this.actionType === 'create') {
            if (this.areaLists !== undefined && this.areaLists !== null) {
                if (this.areaLists.find(function (x) { return x.ListName === _this.areaListName; }) !== undefined) {
                    $('#error-details').html('The area list name you have entered is already in use.');
                    isValid = false;
                }
            }
        }
        return isValid;
    };
    ArealistManageComponent.prototype.saveAreaList = function () {
        var _this = this;
        this.areaListName = document.getElementById("areaListName").value;
        if (this.validateSave()) {
            var areaCodeList_1 = [];
            if (this.areaTypeId === shared_1.AreaTypeIds.Practice) {
                this.selectedPractices.forEach(function (element) {
                    areaCodeList_1.push(element.areaCode);
                });
            }
            else {
                this.selectedAreas.forEach(function (element) {
                    areaCodeList_1.push(element.Code);
                });
            }
            if (this.actionType === 'create') {
                var formData = new FormData();
                formData.append('areaTypeId', this.areaTypeId.toString());
                formData.append('areaListName', this.areaListName);
                formData.append('areaCodeList', areaCodeList_1.toString());
                formData.append('userId', this.userId);
                this.arealistService.saveAreaList(formData)
                    .subscribe(function (response) {
                    _this.goToDataPage();
                }, function (error) {
                    $('#error-details').html('Unable to save the area list');
                });
            }
            else {
                var formData = new FormData();
                formData.append('areaListId', this.areaList.Id.toString());
                formData.append('areaListName', this.areaListName);
                formData.append('areaCodeList', areaCodeList_1.toString());
                formData.append('userId', this.userId);
                formData.append('publicId', this.publicId);
                this.arealistService.updateAreaList(formData)
                    .subscribe(function (response) {
                    _this.goToDataPage();
                }, function (error) {
                    $('#error-details').html('Unable to update the area list');
                });
            }
        }
    };
    ArealistManageComponent.prototype.goToDataPage = function () {
        if (this.parentUrl === undefined) {
            this.back();
        }
        else {
            window.location.href = this.parentUrl;
        }
    };
    ArealistManageComponent.prototype.clearList = function () {
        var _this = this;
        if (this.isMap()) {
            this.selectedAreas.forEach(function (element) {
                _this.areaCode = element.Code;
                _this.mapPolygonSelected = false;
                _this.ref.detectChanges();
            });
        }
        this.selectedAreas.length = 0;
        this.searchedAreas.length = 0;
        this.availableAreas.forEach(function (element) {
            $('.areas-' + element.Code).removeClass('bg-info bg-primary text-white');
        });
        this.areaSearchText = '';
    };
    ArealistManageComponent.prototype.cancel = function () {
        this.goToDataPage();
    };
    ArealistManageComponent.prototype.back = function () {
        window.location.href = '/user-account/area-list';
    };
    ArealistManageComponent.prototype.resolveQueryStringParams = function () {
        var url = window.location.href;
        var urlParams = url.split('?')[1];
        if (urlParams !== undefined) {
            if (urlParams.split('&')[0].split('=')[0].toLowerCase() === 'list_id') {
                this.publicId = urlParams.split('&')[0].split('=')[1];
            }
            if (urlParams.split('&')[0].split('=')[0].toLowerCase() === 'area-type-id') {
                this.areaTypeId = Number(urlParams.split('&')[0].split('=')[1]);
            }
            if (urlParams.split('&')[1] !== undefined) {
                this.parentUrl = urlParams.split('&')[1].split('=')[1];
            }
        }
    };
    return ArealistManageComponent;
}());
ArealistManageComponent = __decorate([
    core_1.Component({
        selector: 'ft-arealist-manage',
        template: __webpack_require__("./src/app/arealist/arealist-manage/arealist-manage.component.html"),
        styles: [__webpack_require__("./src/app/arealist/arealist-manage/arealist-manage.component.css")],
        providers: [arealist_service_1.AreaListService]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _a || Object, typeof (_b = typeof area_service_1.AreaService !== "undefined" && area_service_1.AreaService) === "function" && _b || Object, typeof (_c = typeof arealist_service_1.AreaListService !== "undefined" && arealist_service_1.AreaListService) === "function" && _c || Object, typeof (_d = typeof core_1.ChangeDetectorRef !== "undefined" && core_1.ChangeDetectorRef) === "function" && _d || Object])
], ArealistManageComponent);
exports.ArealistManageComponent = ArealistManageComponent;
var MapColourData = (function () {
    function MapColourData() {
    }
    return MapColourData;
}());
var _a, _b, _c, _d;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/arealist-manage.component.js.map

/***/ }),

/***/ "./src/app/arealist/arealist.module.ts":
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
var http_1 = __webpack_require__("./node_modules/@angular/http/@angular/http.es5.js");
var forms_1 = __webpack_require__("./node_modules/@angular/forms/@angular/forms.es5.js");
var arealist_index_component_1 = __webpack_require__("./src/app/arealist/arealist-index/arealist-index.component.ts");
var arealist_manage_component_1 = __webpack_require__("./src/app/arealist/arealist-manage/arealist-manage.component.ts");
var google_map_simple_component_1 = __webpack_require__("./src/app/map/google-map-simple/google-map-simple.component.ts");
var arealist_areas_component_1 = __webpack_require__("./src/app/shared/component/arealist/arealist-areas/arealist-areas.component.ts");
var practice_search_simple_component_1 = __webpack_require__("./src/app/map/practice-search-simple/practice-search-simple.component.ts");
var ngx_bootstrap_1 = __webpack_require__("./node_modules/ngx-bootstrap/index.js");
var light_box_component_1 = __webpack_require__("./src/app/shared/component/light-box/light-box.component.ts");
var light_box_with_input_component_1 = __webpack_require__("./src/app/shared/component/light-box-with-input/light-box-with-input.component.ts");
var ArealistModule = (function () {
    function ArealistModule() {
    }
    return ArealistModule;
}());
ArealistModule = __decorate([
    core_1.NgModule({
        imports: [
            common_1.CommonModule,
            http_1.HttpModule,
            forms_1.FormsModule,
            forms_1.ReactiveFormsModule,
            ngx_bootstrap_1.TypeaheadModule.forRoot()
        ],
        declarations: [
            arealist_index_component_1.ArealistIndexComponent,
            arealist_manage_component_1.ArealistManageComponent,
            google_map_simple_component_1.GoogleMapSimpleComponent,
            arealist_areas_component_1.ArealistAreasComponent,
            practice_search_simple_component_1.PracticeSearchSimpleComponent,
            light_box_component_1.LightBoxComponent,
            light_box_with_input_component_1.LightBoxWithInputComponent
        ],
        exports: [
            arealist_index_component_1.ArealistIndexComponent,
            arealist_manage_component_1.ArealistManageComponent,
            google_map_simple_component_1.GoogleMapSimpleComponent,
            arealist_areas_component_1.ArealistAreasComponent,
            light_box_component_1.LightBoxComponent,
            light_box_with_input_component_1.LightBoxWithInputComponent
        ]
    })
], ArealistModule);
exports.ArealistModule = ArealistModule;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/arealist.module.js.map

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

module.exports = "#boxplot-legend-img {\r\n    width: 143px;\r\n    height: 296px;\r\n    text-decoration: none;\r\n    border: 1px solid lightgrey;\r\n    float: right;\r\n    margin-top: 10px;\r\n}\r\n\r\n#boxplot-not-available {\r\n    width:100%; \r\n    height:400px;\r\n    padding-top:190px; \r\n    text-align:center;\r\n    font-size:17px;\r\n}"

/***/ }),

/***/ "./src/app/boxplot/boxplot.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"boxplot-container\" style=\"display:none;\">\r\n    <!-- <div class=\"export-chart-box\" style=\"display:block;\">\r\n            <a class=\"export-link\" (click)=\"onExportClick($event)\"\r\n                >Export chart as image</a>\r\n        </div> -->\r\n    <div id=\"indicator-details-boxplot-data\" class=\"standard-width\" [hidden]=\"!isAvailable\">\r\n        <ft-indicator-header [header]=\"header\"></ft-indicator-header>\r\n        <div id=\"boxplot-legend-img\" *ngIf=\"isAnyData()\"></div>\r\n\r\n        <ft-boxplot-chart *ngIf=\"isAnyData()\" [boxplotData]=\"boxplotData\"></ft-boxplot-chart>\r\n\r\n        <ft-boxplot-table *ngIf=\"isAnyData()\" [boxplotData]=\"boxplotData\"></ft-boxplot-table>\r\n        <div id=\"boxplot-no-data\" class=\"no-indicator-data\" *ngIf=\"!isAnyData()\">No Data</div>\r\n    </div>\r\n    <div [hidden]=\"isAvailable\" id=\"boxplot-not-available\" class=\"standard-width\">\r\n       Not applicable for England data\r\n    </div>\r\n</div>"

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
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var BoxplotComponent = (function () {
    function BoxplotComponent(ftHelperService, indicatorService) {
        this.ftHelperService = ftHelperService;
        this.indicatorService = indicatorService;
        this.isAvailable = true;
    }
    BoxplotComponent.prototype.onOutsideEvent = function (event) {
        var _this = this;
        var ftHelper = this.ftHelperService;
        var groupRoot = this.ftHelperService.getCurrentGroupRoot();
        var model = ftHelper.getFTModel();
        this.isAvailable = model.areaTypeId !== shared_1.AreaTypeIds.Country;
        // Get data
        var metadataObservable = this.indicatorService.getIndicatorMetadata(model.groupId);
        var statsObservable = this.indicatorService.getIndicatorStatisticsTrendsForSingleIndicator(groupRoot.IID, groupRoot.Sex.Id, groupRoot.Age.Id, model.areaTypeId, this.ftHelperService.getCurrentComparator().Code);
        Observable_1.Observable.forkJoin([metadataObservable, statsObservable]).subscribe(function (results) {
            var metadataHash = results[0];
            var statsArray = results[1];
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

/***/ "./src/app/data-view/data-view.component.css":
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/data-view/data-view.component.html":
/***/ (function(module, exports) {

module.exports = "<ft-map></ft-map>\r\n<ft-england></ft-england>\r\n<ft-metadata></ft-metadata>\r\n<ft-population></ft-population>\r\n<ft-boxplot></ft-boxplot>\r\n<ft-download></ft-download>\r\n<ft-reports></ft-reports>\r\n<ft-area-profile></ft-area-profile>"

/***/ }),

/***/ "./src/app/data-view/data-view.component.ts":
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
var DataViewComponent = (function () {
    function DataViewComponent() {
    }
    return DataViewComponent;
}());
DataViewComponent = __decorate([
    core_1.Component({
        selector: 'ft-data-view',
        template: __webpack_require__("./src/app/data-view/data-view.component.html"),
        styles: [__webpack_require__("./src/app/data-view/data-view.component.css")]
    })
    /** Components for viewing data in Fingertips */
    ,
    __metadata("design:paramtypes", [])
], DataViewComponent);
exports.DataViewComponent = DataViewComponent;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/data-view.component.js.map

/***/ }),

/***/ "./src/app/download/download-data/download-data.component.css":
/***/ (function(module, exports) {

module.exports = "h2 {\r\n    padding: 0;\r\n    color: #555;\r\n    padding-bottom: 0 !important;\r\n}\r\n\r\n.btn-link {\r\n    color: #2e3191;\r\n    padding: 0;\r\n    width: 100%;\r\n    text-align: left;\r\n    margin: 1px 0;\r\n}\r\n\r\nh2.secondary-heading {\r\n    margin-top: 30px;\r\n}\r\n\r\nh3 {\r\n    margin: 20px 0 0 0;\r\n    }"

/***/ }),

/***/ "./src/app/download/download-data/download-data.component.html":
/***/ (function(module, exports) {

module.exports = "<h2>Get the data</h2>\r\n<p>Download indicator data and definitions</p>\r\n\r\n<!-- Download data for profile -->\r\n<div [hidden]=\"!showProfile\">\r\n    <h3 [innerHtml]=\"'Profile: ' + profileName\"> </h3>\r\n    <button class=\"btn btn-link\" (click)=\"exportProfileData(nationalCode)\">{{allLabel}}</button>\r\n    <button class=\"btn btn-link\" (click)=\"exportProfileData(parentCode)\" [hidden]=\"!showSubnational\">\r\n          {{parentLabel}}</button>\r\n    <button class=\"btn btn-link\" (click)=\"exportProfileMetadata()\">Indicator definitions</button>\r\n</div>\r\n\r\n<!-- Download data for domain -->\r\n<div [hidden]=\"!showGroup\">\r\n    <h3 [innerHtml]=\"'Domain: ' + groupName\"></h3>\r\n    <button class=\"btn btn-link\" (click)=\"exportGroupData(nationalCode)\">{{allLabel}}</button>\r\n    <button class=\"btn btn-link\" (click)=\"exportGroupData(parentCode)\" [hidden]=\"!showSubnational\">{{parentLabel}}</button>\r\n    <button class=\"btn btn-link\" (click)=\"exportGroupMetadata()\">Indicator definitions</button>\r\n</div>\r\n\r\n<!-- Download data for search results / list of indicators -->\r\n<div [hidden]=\"!showAllIndicators\">\r\n    <h3>All indicators</h3>\r\n    <button class=\"btn btn-link\" (click)=\"exportAllIndicatorData(nationalCode)\">{{allLabel}}</button>\r\n    <button class=\"btn btn-link\" (click)=\"exportAllIndicatorData(parentCode)\" [hidden]=\"!showSubnational\">{{parentLabel}}</button>\r\n    <button class=\"btn btn-link\" (click)=\"exportAllIndicatorMetadata()\">Indicator definitions</button>\r\n</div>\r\n\r\n<!-- Download data for single indicator -->\r\n<h3 [innerHtml]=\"'Indicator: ' + indicatorName\"></h3>\r\n<button class=\"btn btn-link\" (click)=\"exportIndicatorData(nationalCode)\">{{allLabel}}</button>\r\n<button class=\"btn btn-link\" (click)=\"exportIndicatorData(parentCode)\" [hidden]=\"!showSubnational\">{{parentLabel}}</button>\r\n<button class=\"btn btn-link\" (click)=\"exportIndicatorMetadata()\">Indicator definition</button>\r\n\r\n<!-- Download data for practice addresses -->\r\n<div [hidden]=\"!showAddresses\">\r\n    <h3>GP practice addresses</h3>\r\n    <button class=\"btn btn-link\" (click)=\"exportAddresses(nationalCode)\">{{allLabel}}</button>\r\n    <button class=\"btn btn-link\" (click)=\"exportAddresses(parentCode)\" [hidden]=\"!showSubnational\">{{parentLabel}}</button>\r\n</div>\r\n\r\n<!-- Download data for populations -->\r\n<div [hidden]=\"!showPopulation\">\r\n    <h3>Population age distribution</h3>\r\n    <button class=\"btn btn-link\" (click)=\"exportPopulation(nationalCode)\">{{allLabel}}</button>\r\n    <button class=\"btn btn-link\" (click)=\"exportPopulation(parentCode)\" [hidden]=\"!showSubnational\">{{parentLabel}}</button>\r\n</div>\r\n\r\n<p *ngIf=\"isEnvironmentTest\">Data files for many profiles are pre-built every night so any data that has been uploaded or changed today will not be included.\r\n    <br>\r\n    <br> Downloaded data is only available if your machine is on the PHE network.\r\n</p>\r\n\r\n<h2 class=\"secondary-heading\">Get the data with R</h2>\r\n<p>The <a style=\"display:inline;\" href=\"https://cran.r-project.org/web/packages/fingertipsR/index.html\" target=\"_blank\">fingertipsR</a> package allows you to download public health data using R</p>\r\n\r\n<h2 class=\"secondary-heading\">Public Health Data API</h2>\r\n<p>The <a style=\"display:inline;\" href=\"{{apiUrl}}\" target=\"_blank\">Fingertips API</a> (Chrome or Firefox only) allows public health data to be retrieved in either JSON or CSV formats</p>"

/***/ }),

/***/ "./src/app/download/download-data/download-data.component.ts":
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
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var DownloadDataComponent = (function () {
    function DownloadDataComponent(ftHelperService) {
        this.ftHelperService = ftHelperService;
        // Display toggle flags
        this.showProfile = false;
        this.showGroup = false;
        this.showAllIndicators = false;
        this.showSubnational = false;
        this.showAddresses = false;
        this.showPopulation = false;
        this.isEnvironmentTest = false;
        // Text
        this.profileName = '';
        this.groupName = '';
        this.indicatorName = '';
        this.allLabel = '';
        this.parentLabel = '';
        this.apiUrl = '';
        // Area codes
        this.parentCode = '';
        this.nationalCode = '';
        /**
        * Export all indicator metadata for specific profile.
        */
        this.exportProfileMetadata = function () {
            var parameters = new shared_1.ParameterBuilder();
            this.addProfileIdParameter(parameters);
            this.downloadCsvMetadata('by_profile_id', parameters);
            this.logEvent('MetadataForProfile', parameters.build());
        };
        /**
        * Export indicator metadata for profile group
        */
        this.exportGroupMetadata = function () {
            var parameters = new shared_1.ParameterBuilder().add('group_id', this.model.groupId);
            this.downloadCsvMetadata('by_group_id', parameters);
            this.logEvent('MetadataForDomain', parameters.build());
        };
        /**
        * Export all indicator metadata for search results.
        */
        this.exportAllIndicatorMetadata = function () {
            var parameters = new shared_1.ParameterBuilder();
            parameters.add('indicator_ids', this.getIndicatorIdsParameter());
            this.addProfileIdParameter(parameters);
            this.downloadCsvMetadata('by_indicator_id', parameters);
            this.logEvent('MetadataForAllSearchIndicators', parameters.build());
        };
        /**
        * Export indicator metadata for single indicator.
        */
        this.exportIndicatorMetadata = function () {
            var parameters = new shared_1.ParameterBuilder();
            parameters.add('indicator_ids', this.groupRoot.IID);
            this.addProfileIdParameter(parameters);
            this.downloadCsvMetadata('by_indicator_id', parameters);
            this.logEvent('MetadataForSearchIndicator', parameters.build());
        };
        /**
        * Export all indicator data for specific profile.
        */
        this.exportProfileData = function (parentCode) {
            var parameters = this.getExportParameters(parentCode);
            this.addProfileIdParameter(parameters);
            this.downloadCsvData('by_profile_id', parameters);
            this.logEvent('DataForProfile', parameters.build());
        };
        /**
        * Export indicator data for profile group
        */
        this.exportGroupData = function (parentAreaCode) {
            var parameters = this.getExportParameters(parentAreaCode);
            parameters.add('group_id', this.model.groupId);
            this.downloadCsvData('by_group_id', parameters);
            this.logEvent('DataForGroup', parameters.build());
        };
        this.exportPopulation = function (parentAreaCode) {
            var parameters = new shared_1.ParameterBuilder()
                .add('are', parentAreaCode)
                .add('gid', shared_1.GroupIds.PracticeProfiles_Population)
                .add('ati', this.model.parentTypeId).build();
            var url = this.urls.corews + 'GetData.ashx?s=db&pro=qp&' + parameters;
            this.openFile(url);
            this.logEvent('Population', parameters);
        };
        this.exportAddresses = function (parentAreaCode) {
            var parameters = new shared_1.ParameterBuilder()
                .add('parent_area_code', parentAreaCode)
                .add('area_type_id', this.model.areaTypeId).build();
            var url = this.urls.corews + 'api/area_addresses/csv/by_parent_area_code?' + parameters;
            this.openFile(url);
            this.logEvent('Addresses', parameters);
        };
    }
    DownloadDataComponent.prototype.refresh = function () {
        // Set instance level variables
        this.model = this.ftHelperService.getFTModel();
        this.groupRoot = this.ftHelperService.getCurrentGroupRoot();
        this.search = this.ftHelperService.getSearch();
        this.urls = this.ftHelperService.getURL();
        var config = this.ftHelperService.getFTConfig();
        var isNearestNeighbours = this.ftHelperService.isNearestNeighbours(this.model);
        this.profileName = config.profileName;
        this.groupName = this.ftHelperService.getCurrentDomainName();
        this.showSubnational = !this.ftHelperService.isParentCountry(this.model) &&
            this.model.areaTypeId !== shared_1.AreaTypeIds.Country &&
            !isNearestNeighbours;
        this.showAllIndicators = this.search.isInSearchMode();
        this.showProfile = !this.showAllIndicators;
        this.showGroup = this.groupName !== null &&
            this.groupName !== '' && this.showProfile;
        var areaTypeName = this.ftHelperService.getAreaTypeName();
        var isEnglandAreaType = areaTypeName === 'England';
        var dataLabel = 'Data for ' + areaTypeName;
        if (!isEnglandAreaType) {
            dataLabel.concat(' in England');
        }
        this.allLabel = dataLabel;
        this.nationalCode = shared_1.AreaCodes.England;
        this.parentCode = this.model.parentCode;
        this.parentLabel = '';
        if (!isEnglandAreaType) {
            this.parentLabel = isNearestNeighbours ? ''
                : 'Data for ' + areaTypeName + ' in ' + this.ftHelperService.getParentArea().Name;
        }
        // Indicator name
        this.indicatorName = this.ftHelperService.getMetadataHash()[this.groupRoot.IID].Descriptive.Name;
        if (this.model.profileId === shared_1.ProfileIds.PracticeProfile) {
            // Too much data to download for all of practice profiles
            this.showProfile = false;
            this.showPopulation = true;
        }
        else {
            this.showPopulation = false;
        }
        this.isEnvironmentTest = config.environment === 'test';
        this.showAddresses = this.model.areaTypeId === shared_1.AreaTypeIds.Practice;
        this.setApiUrl();
    };
    DownloadDataComponent.prototype.setApiUrl = function () {
        if (this.apiUrl === '') {
            this.apiUrl = this.urls.bridge + 'api';
        }
    };
    /**
    * Export all indicator data for search results.
    */
    DownloadDataComponent.prototype.exportAllIndicatorData = function (parentAreaCode) {
        var parameters = this.getExportParameters(parentAreaCode);
        parameters.add('indicator_ids', this.search.getIndicatorIdsParameter());
        this.addProfileIdParameter(parameters);
        this.downloadCsvData('by_indicator_id', parameters);
        this.logEvent('DataForAllSearchIndicators', parameters.build());
    };
    /**
    * Export indicator data for single indicator.
    */
    DownloadDataComponent.prototype.exportIndicatorData = function (parentAreaCode) {
        var parameters = this.getExportParameters(parentAreaCode);
        this.addProfileIdParameter(parameters);
        parameters.add('indicator_ids', this.groupRoot.IID);
        this.downloadCsvData('by_indicator_id', parameters);
        this.logEvent('DataForIndicator', parameters.build());
    };
    DownloadDataComponent.prototype.logEvent = function (action, parameters) {
        this.ftHelperService.logEvent('Download', action, parameters);
    };
    DownloadDataComponent.prototype.addProfileIdParameter = function (parameters) {
        var search = this.ftHelperService.getSearch();
        if (!search.isInSearchMode()) {
            parameters.add('profile_id', this.model.profileId);
        }
    };
    DownloadDataComponent.prototype.downloadCsvData = function (byTerm, parameters) {
        var url = this.urls.corews + 'api/all_data/csv/' + byTerm + '?' + parameters.build();
        this.openFile(url);
    };
    DownloadDataComponent.prototype.downloadCsvMetadata = function (byTerm, parameters) {
        var url = this.urls.corews + 'api/indicator_metadata/csv/' + byTerm + '?' + parameters.build();
        this.openFile(url);
    };
    DownloadDataComponent.prototype.getExportParameters = function (parentAreaCode) {
        var model = this.model;
        var parameters = new shared_1.ParameterBuilder()
            .add('parent_area_code', parentAreaCode)
            .add('parent_area_type_id', model.parentTypeId)
            .add('child_area_type_id', model.areaTypeId);
        return parameters;
    };
    DownloadDataComponent.prototype.openFile = function (url) {
        window.open(url.toLowerCase(), '_blank');
    };
    return DownloadDataComponent;
}());
DownloadDataComponent = __decorate([
    core_1.Component({
        selector: 'ft-download-data',
        template: __webpack_require__("./src/app/download/download-data/download-data.component.html"),
        styles: [__webpack_require__("./src/app/download/download-data/download-data.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _a || Object])
], DownloadDataComponent);
exports.DownloadDataComponent = DownloadDataComponent;
var _a;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/download-data.component.js.map

/***/ }),

/***/ "./src/app/download/download-report/download-report.component.css":
/***/ (function(module, exports) {

module.exports = ".btn-link {\r\n    color: #2e3191;\r\n    padding: 3px 0 0 0;\r\n    width: 100%;\r\n    text-align: left;\r\n    margin: 1px 0;\r\n    font-size: 16px;\r\n}\r\n\r\n img.pdf {\r\n    cursor: pointer;\r\n    height: 300px;\r\n    border: 2px solid #eee;\r\n    float: right;\r\n    }"

/***/ }),

/***/ "./src/app/download/download-report/download-report.component.html":
/***/ (function(module, exports) {

module.exports = "<div *ngIf=\"areAnyPdfsForProfile\" class=\"row col-md-12\">\r\n    <div id=\"pdf-download-text\" class=\"text col-md-6\">\r\n        <h2>{{title}}</h2>\r\n\r\n        <div *ngIf=\"arePdfsForCurrentAreaType\">\r\n            <p>Download a detailed report of the data for</p>\r\n\r\n            <p *ngIf=\"showTimePeriodsMenu\">\r\n                {{reportsLabel}}\r\n                <select (change)=\"timePeriodChange($event.target.value)\">\r\n                    <option *ngFor=\"let period of timePeriods\">{{period}}</option>\r\n                </select>\r\n            </p>\r\n\r\n            <button class=\"bnt btn-link pdf\" (click)=\"exportPdf()\">{{areaName}}</button>\r\n        </div>\r\n\r\n        <p [hidden]=\"arePdfsForCurrentAreaType\">PDF profiles are not currently available for {{noPdfsMessage}}</p>\r\n\r\n        <br>\r\n        <p *ngIf=\"showMentalHealthSurvey\">Please note the PDF reports are not updated with the profile. Consequently,\r\n            many of the indicators in the PDF reports will be based on older data than the profile data. We will be\r\n            developing downloadable dynamic reports which will address this\r\n            problem but it is likely to be several months before these are available.</p>\r\n    </div>\r\n    <div class=\"col-md-6\">\r\n        <img src=\"{{imagesUrl}}download/{{fileName}}\" class=\"pdf\" (click)=\"exportPdf()\" />\r\n    </div>\r\n</div>\r\n<div class=\"row col-md-12\">&nbsp;</div>\r\n\r\n<!-- At a glance reports -->\r\n<div *ngIf=\"areAtAGlanceReports\" class=\"row col-md-12\">\r\n    <div id=\"phof-html-download-text\" class=\"text col-md-6\">\r\n        <h2>At a Glance</h2>\r\n        <p>View a web summary report of the data for</p>\r\n        <p *ngIf=\"showTimePeriodsMenu\">\r\n            {{reportsLabel}}\r\n            <select (change)=\"timePeriodChange($event.target.value)\">\r\n                <option *ngFor=\"let period of timePeriods\">{{period}}</option>\r\n            </select>\r\n        </p>\r\n        <button class=\"bnt btn-link pdf\" *ngFor=\"let area of areasWithPdfs\" (click)=\"downloadStaticReportAtaGlance(area.Code)\">{{area.Name}}</button>\r\n    </div>\r\n    <div class=\"col-md-6\"><img src=\"{{imagesUrl}}download/html{{profileId}}-at-a-glance.png\" class=\"pdf\" /></div>\r\n</div>\r\n\r\n<!-- Child health behaviours -->\r\n<div *ngIf=\"isChildHealthProfile\" class=\"row col-md-12\">\r\n    <div id=\"pdf-download-text\" class=\"text col-md-6\">\r\n        <h2>Health behaviours in young people</h2>\r\n\r\n        <div *ngIf=\"arePdfsForCurrentAreaType\">\r\n            <p>Download a detailed report of the data for</p>\r\n            <button class=\"bnt btn-link pdf\" (click)=\"exportChildHealthBehavioursPdf()\">{{areaName}}</button>\r\n        </div>\r\n\r\n        <p [hidden]=\"arePdfsForCurrentAreaType\">PDF profiles are not curently available for {{noPdfsMessage}}</p>\r\n\r\n    </div>\r\n    <div class=\"col-md-6\">\r\n        <img src=\"{{imagesUrl}}download/{{childHealthBehavioursFileName}}\" class=\"pdf\" (click)=\"exportChildHealthBehavioursPdf()\" />\r\n    </div>\r\n</div>\r\n<div class=\"row col-md-12\">&nbsp;</div>\r\n\r\n<!-- Child health dental report -->\r\n<div *ngIf=\"isChildHealthProfile\" class=\"row col-md-12\">\r\n    <div id=\"pdf-download-text\" class=\"text col-md-6\">\r\n        <h2>Oral health profile of five year olds</h2>\r\n\r\n        <div *ngIf=\"arePdfsForCurrentAreaType\">\r\n            <p>Download a detailed report of the data for</p>\r\n            <p>\r\n                Report year\r\n                <select (change)=\"timePeriodChangeForDentalHealth($event.target.value)\">\r\n                    <option>2015</option>\r\n                    <option>2012</option>\r\n                </select>\r\n            </p>\r\n            <button class=\"bnt btn-link pdf\" (click)=\"exportChildHealthDentalReport()\">{{areaName}}</button>\r\n        </div>\r\n\r\n        <p [hidden]=\"arePdfsForCurrentAreaType\">PDF profiles are not curently available for {{noPdfsMessage}}</p>\r\n\r\n    </div>\r\n    <div class=\"col-md-6\">\r\n        <img src=\"{{imagesUrl}}download/pdf-dental-health.png\" class=\"pdf\" (click)=\"exportChildHealthDentalReport()\" />\r\n    </div>\r\n</div>"

/***/ }),

/***/ "./src/app/download/download-report/download-report.component.ts":
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
var profile_service_1 = __webpack_require__("./src/app/shared/service/api/profile.service.ts");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var static_reports_service_1 = __webpack_require__("./src/app/shared/service/api/static-reports.service.ts");
var _ = __webpack_require__("./node_modules/underscore/underscore.js");
var light_box_component_1 = __webpack_require__("./src/app/shared/component/light-box/light-box.component.ts");
var light_box_service_1 = __webpack_require__("./src/app/shared/service/helper/light-box.service.ts");
var DownloadReportComponent = (function () {
    function DownloadReportComponent(profileService, ftHelperService, staticReportsService, lightBoxService) {
        this.profileService = profileService;
        this.ftHelperService = ftHelperService;
        this.staticReportsService = staticReportsService;
        this.lightBoxService = lightBoxService;
        this.showMentalHealthSurvey = true;
        this.showTimePeriodsMenu = true;
        this.isChildHealthProfile = false;
        this.arePdfsForCurrentAreaType = true;
        this.areAnyPdfsForProfile = false;
        this.areAtAGlanceReports = false;
        this.timePeriods = [];
        this.selectedTimePeriod = null;
        this.selectedTimePeriodForDentalHealth = '2017';
    }
    DownloadReportComponent.prototype.refresh = function () {
        var _this = this;
        this.model = this.ftHelperService.getFTModel();
        this.config = this.ftHelperService.getFTConfig();
        this.urls = this.ftHelperService.getURL();
        this.profileService.areaTypesWithPdfs(this.model.profileId).subscribe(function (areaTypesWithPdfs) {
            _this.areaTypesWithPdfs = areaTypesWithPdfs;
            _this.setPdfHtml();
        });
    };
    DownloadReportComponent.prototype.setPdfHtml = function () {
        this.areAnyPdfsForProfile = this.config.areAnyPdfsForProfile;
        this.imagesUrl = this.urls.img;
        this.profileId = this.model.profileId;
        if (this.profileId === shared_1.ProfileIds.SearchResults) {
            this.noPdfsMessage = 'search results';
        }
        else if (!this.isPdfAvailableForCurrentAreaType()) {
            this.noPdfsMessage = this.ftHelperService.getAreaTypeName();
        }
        else {
            this.noPdfsMessage = null;
        }
        this.arePdfsForCurrentAreaType = this.noPdfsMessage === null;
        // Time periods
        this.timePeriods = this.config.staticReportsFolders;
        this.showTimePeriodsMenu = this.timePeriods.length > 1;
        if (this.showTimePeriodsMenu) {
            this.selectedTimePeriod = this.timePeriods[0];
        }
        this.reportsLabel = this.config.staticReportsLabel;
        this.setShowMentalHealthSurvey();
        this.setTitle();
        this.fileName = this.getPdfFileName(this.profileId);
        this.areaName = this.ftHelperService.getAreaName(this.model.areaCode);
        // At a glance
        this.areAtAGlanceReports =
            this.profileId === shared_1.ProfileIds.Phof || this.profileId === shared_1.ProfileIds.Tobacco;
        if (this.areAtAGlanceReports) {
            this.displayAtAGlanceReports();
        }
        // Child Health behaviours
        this.isChildHealthProfile = this.profileId === shared_1.ProfileIds.ChildHealth;
        if (this.isChildHealthProfile) {
            this.childHealthBehavioursFileName = this.getPdfFileName(shared_1.ProfileIds.ChildHealthBehaviours);
        }
    };
    DownloadReportComponent.prototype.displayAtAGlanceReports = function () {
        var areaList = [];
        // Add England
        areaList.push({ Name: "England", Code: shared_1.AreaCodes.England });
        // Add parent area 
        var parentArea = this.ftHelperService.getParentArea();
        var parentAreaTypesWithReports = [shared_1.AreaTypeIds.Region, shared_1.AreaTypeIds.CombinedAuthorities, shared_1.AreaTypeIds.County];
        if (_.contains(parentAreaTypesWithReports, parentArea.AreaTypeId)) {
            areaList.push({ Name: parentArea.Short, Code: parentArea.Code });
        }
        // Add current area
        areaList.push(this.ftHelperService.getArea(this.model.areaCode));
        this.areasWithPdfs = areaList;
    };
    DownloadReportComponent.prototype.setTitle = function () {
        // Set PDF section title
        this.title = 'Area profile';
        if (this.model.profileId === shared_1.ProfileIds.ChildHealth) {
            this.title = 'Child health profile';
        }
    };
    /* Returns true if PDFs are available for the user to download.*/
    DownloadReportComponent.prototype.isPdfAvailableForCurrentAreaType = function () {
        var areaTypeId = this.model.areaTypeId;
        return _.some(this.areaTypesWithPdfs, function (areaType) {
            return areaType.Id === areaTypeId;
        });
    };
    DownloadReportComponent.prototype.getPdfFileName = function (profileId) {
        return 'pdf' + profileId + '.png';
    };
    DownloadReportComponent.prototype.setShowMentalHealthSurvey = function () {
        this.showMentalHealthSurvey = _.contains([
            shared_1.ProfileIds.Dementia,
            shared_1.ProfileIds.ChildrenYoungPeoplesWellBeing,
            shared_1.ProfileIds.CommonMentalHealthDisorders,
            shared_1.ProfileIds.MentalHealthJsna,
            shared_1.ProfileIds.SevereMentalIllness,
            shared_1.ProfileIds.SuicidePrevention
        ], this.model.profileId);
    };
    DownloadReportComponent.prototype.exportChildHealthBehavioursPdf = function () {
        this.downloadStaticReport(this.model.areaCode, shared_1.ProfileUrlKeys.ChildHealthBehaviours);
    };
    DownloadReportComponent.prototype.exportChildHealthDentalReport = function () {
        this.downloadStaticReport(this.model.areaCode, shared_1.ProfileUrlKeys.DentalHealth);
    };
    DownloadReportComponent.prototype.exportPdf = function () {
        this.downloadCachedPdf(this.model.areaCode);
        this.logReportDownload(this.ftHelperService.getProfileUrlKey());
    };
    /**
    * Downloads a cached PDF. This function is only used on the live site.
    */
    DownloadReportComponent.prototype.downloadCachedPdf = function (areaCode) {
        var profileId = this.model.profileId;
        var url;
        if (this.config.hasStaticReports) {
            this.downloadStaticReport(areaCode, this.ftHelperService.getProfileUrlKey());
            return;
        }
        else if (profileId === shared_1.ProfileIds.Liver) {
            // Liver profiles
            url = 'http://www.endoflifecare-intelligence.org.uk/profiles/liver-disease/' + areaCode + '.pdf';
        }
        else {
            url = this.getPdfUrl(areaCode);
        }
        this.openFile(url);
    };
    DownloadReportComponent.prototype.getPdfUrl = function (areaCode) {
        return this.urls.pdf + this.ftHelperService.getProfileUrlKey() +
            '/' + areaCode + '.pdf';
    };
    /**
    * Downloads a static document after first checking whether it is available.
    */
    DownloadReportComponent.prototype.downloadStaticReport = function (areaCode, profileKey) {
        this.checkStaticReportExistsThenDownload(areaCode, profileKey);
    };
    DownloadReportComponent.prototype.checkStaticReportExistsThenDownload = function (areaCode, profileKey) {
        var _this = this;
        var fileName = areaCode + '.pdf';
        // Select time period
        var timePeriod;
        if (profileKey === shared_1.ProfileUrlKeys.DentalHealth) {
            timePeriod = this.selectedTimePeriodForDentalHealth;
        }
        else {
            timePeriod = this.selectedTimePeriod;
        }
        this.staticReportsService.doesStaticReportExist(profileKey, fileName, timePeriod).subscribe(function (doesReportExist) {
            if (doesReportExist) {
                _this.openStaticReport(profileKey, fileName, timePeriod);
            }
            else {
                var top = profileKey === shared_1.ProfileUrlKeys.DentalHealth ? 1200 : 600;
                _this.showDocumentNotExistMessage(top);
            }
        });
    };
    DownloadReportComponent.prototype.downloadStaticReportAtaGlance = function (areaCode) {
        var url = this.urls.bridge + 'static-reports/' +
            this.ftHelperService.getProfileUrlKey() + '/at-a-glance/' + areaCode + '.html';
        this.openFile(url);
        this.logReportDownload('PhofAtAGlance');
    };
    DownloadReportComponent.prototype.logReportDownload = function (report) {
        this.ftHelperService.logEvent('Download-Report', report, this.areaName);
    };
    DownloadReportComponent.prototype.timePeriodChange = function (timePeriod) {
        this.selectedTimePeriod = timePeriod;
    };
    DownloadReportComponent.prototype.timePeriodChangeForDentalHealth = function (timePeriod) {
        this.selectedTimePeriodForDentalHealth = timePeriod;
    };
    DownloadReportComponent.prototype.showDocumentNotExistMessage = function (top) {
        var config = new light_box_component_1.LightBoxConfig();
        config.Type = light_box_component_1.LightBoxTypes.OkCancel;
        config.Title = 'Sorry, this document is not available';
        config.Html = '';
        config.Top = top;
        this.lightBoxService.display(config);
    };
    DownloadReportComponent.prototype.openStaticReport = function (profileKey, fileName, timePeriod) {
        // Define URL parameters
        var params = new shared_1.ParameterBuilder();
        params.add('profile_key', profileKey);
        params.add('file_name', fileName);
        if (timePeriod) {
            params.add('time_period', timePeriod);
        }
        // Download file
        var url = this.urls.corews + 'static-reports?' + params.build();
        this.openFile(url);
        this.logReportDownload(profileKey);
    };
    DownloadReportComponent.prototype.openFile = function (url) {
        window.open(url.toLowerCase(), '_blank');
    };
    return DownloadReportComponent;
}());
DownloadReportComponent = __decorate([
    core_1.Component({
        selector: 'ft-download-report',
        template: __webpack_require__("./src/app/download/download-report/download-report.component.html"),
        styles: [__webpack_require__("./src/app/download/download-report/download-report.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof profile_service_1.ProfileService !== "undefined" && profile_service_1.ProfileService) === "function" && _a || Object, typeof (_b = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _b || Object, typeof (_c = typeof static_reports_service_1.StaticReportsService !== "undefined" && static_reports_service_1.StaticReportsService) === "function" && _c || Object, typeof (_d = typeof light_box_service_1.LightBoxService !== "undefined" && light_box_service_1.LightBoxService) === "function" && _d || Object])
], DownloadReportComponent);
exports.DownloadReportComponent = DownloadReportComponent;
var _a, _b, _c, _d;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/download-report.component.js.map

/***/ }),

/***/ "./src/app/download/download.component.css":
/***/ (function(module, exports) {

module.exports = "#download-container {\r\n    color: #666;\r\n    padding: 60px 0 120px 0;\r\n}\r\n\r\nft-download-data {\r\n    border-right: 2px solid #eee;\r\n}\r\n\r\n#download-container p {\r\nline-height: 1.4em;\r\nmargin: 0;\r\n}\r\n\r\n"

/***/ }),

/***/ "./src/app/download/download.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"download-container\" class=\"row\" style=\"display:none;\">\r\n    <ft-download-data class=\"col-md-6\" #downloadData></ft-download-data>\r\n    <ft-download-report class=\"col-md-6\" #downloadReport></ft-download-report>\r\n</div>"

/***/ }),

/***/ "./src/app/download/download.component.ts":
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
var DownloadComponent = (function () {
    function DownloadComponent(ftHelperService) {
        this.ftHelperService = ftHelperService;
    }
    DownloadComponent.prototype.onOutsideEvent = function (event) {
        // Refresh child components
        this.downloadDataComponent.refresh();
        this.downloadReportComponent.refresh();
        // Unlock UI
        this.ftHelperService.showAndHidePageElements();
        this.ftHelperService.unlock();
    };
    return DownloadComponent;
}());
__decorate([
    core_1.ViewChild('downloadData'),
    __metadata("design:type", Object)
], DownloadComponent.prototype, "downloadDataComponent", void 0);
__decorate([
    core_1.ViewChild('downloadReport'),
    __metadata("design:type", Object)
], DownloadComponent.prototype, "downloadReportComponent", void 0);
__decorate([
    core_1.HostListener('window:DownloadSelected', ['$event']),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [Object]),
    __metadata("design:returntype", void 0)
], DownloadComponent.prototype, "onOutsideEvent", null);
DownloadComponent = __decorate([
    core_1.Component({
        selector: 'ft-download',
        template: __webpack_require__("./src/app/download/download.component.html"),
        styles: [__webpack_require__("./src/app/download/download.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _a || Object])
], DownloadComponent);
exports.DownloadComponent = DownloadComponent;
var _a;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/download.component.js.map

/***/ }),

/***/ "./src/app/download/download.module.ts":
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
var download_component_1 = __webpack_require__("./src/app/download/download.component.ts");
var download_data_component_1 = __webpack_require__("./src/app/download/download-data/download-data.component.ts");
var download_report_component_1 = __webpack_require__("./src/app/download/download-report/download-report.component.ts");
var DownloadModule = (function () {
    function DownloadModule() {
    }
    return DownloadModule;
}());
DownloadModule = __decorate([
    core_1.NgModule({
        imports: [
            common_1.CommonModule,
            shared_module_1.SharedModule
        ],
        declarations: [
            download_component_1.DownloadComponent,
            download_data_component_1.DownloadDataComponent,
            download_report_component_1.DownloadReportComponent
        ],
        exports: [
            download_component_1.DownloadComponent
        ]
    })
], DownloadModule);
exports.DownloadModule = DownloadModule;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/download.module.js.map

/***/ }),

/***/ "./src/app/england/england.component.css":
/***/ (function(module, exports) {

module.exports = "#england-table {\r\n    font-family: Arial, Helvetica, sans-serif;\r\n    background: #fff;\r\n    margin-bottom: 30px;\r\n    margin-top: 10px;\r\n    width: 80%;\r\n}\r\n\r\n    #england-table tr th:first-child {\r\n        width: 50%;\r\n    }\r\n\r\n    #england-table tr td {\r\n        padding: 2px;\r\n    }\r\n\r\n    #england-table tr td:first-child button {\r\n            text-align: left;\r\n        }\r\n\r\n    .table-hover tbody tr:hover td {\r\n    color: #000;\r\n    background: #FFFDEA;\r\n}\r\n\r\n    .table-hover tbody tr:hover td a {\r\n        color: #2e3191;\r\n    }\r\n"

/***/ }),

/***/ "./src/app/england/england.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"england-container\" style=\"display:none;\">\r\n    <div class=\"row\">\r\n        <div class=\"col-md-12\">\r\n            <ft-legend [keyType]=\"keyType\" [legendType]=\"legendType\" [showRecentTrends]=\"showRecentTrends\"></ft-legend>\r\n        </div>\r\n    </div>\r\n    <div class=\"export-chart-box\" style=\"display:block;\">\r\n        <a class=\"export-link\" (click)=\"onExportClick($event)\">Export table as image</a>\r\n    </div>\r\n    <div class=\"export-chart-box-csv\" style=\"display:block;\">\r\n        <a class=\"export-link-csv hidden\" (click)=\"onExportCsvFileClick($event)\">Export table as CSV file</a>\r\n    </div>\r\n    <div id=\"england-table\">\r\n        <table class=\"bordered-table table-hover\">\r\n            <thead>\r\n                <tr>\r\n                    <th>Indicator</th>\r\n                    <th>Period</th>\r\n                    <th class=\"center\">England<br />count</th>\r\n                    <th class=\"center\">England<br />value</th>\r\n                    <th *ngIf=\"hasRecentTrends\" class=\"center\">Recent<br />trend</th>\r\n                    <th *ngIf=\"isChangeFromPreviousPeriodShown\" class=\"center\">Change from<br />previous <br/>time period</th>\r\n                </tr>\r\n            </thead>\r\n            <tbody>\r\n                <tr *ngFor=\"let row of rows\">\r\n                    <td *ngIf=\"(row.isSubheading == true)\" colspan=\"6\" title=\"{{row.indicatorName}}\" data-toggle=\"tooltip\"\r\n                        data-placement=\"top\" class=\"rug-subheading numeric boot-tooltip\">{{row.indicatorName}}</td>\r\n                    <td *ngIf=\"(row.isSubheading == false)\">\r\n                        <button class=\"pLink\" href=\"#\" (click)=indicatorNameClicked(row)>{{row.indicatorName}}</button>\r\n                        <span *ngIf=\"row.hasNewData\" style=\"margin-right: 8px;\" class=\"badge badge-success\">New data</span>\r\n                    </td>\r\n                    <td *ngIf=\"(row.isSubheading == false)\">{{row.period}}</td>\r\n                    <td *ngIf=\"(row.isSubheading == false)\" class=\"numeric\" [innerHTML]=\"row.count\"></td>\r\n                    <td *ngIf=\"(row.isSubheading == false)\" class=\"numeric\" (mouseover)=showValueNoteTooltip($event,row)\r\n                        (mouseout)=hideTooltip() (mousemove)=positionTooltip($event) [innerHTML]=\"row.value\"></td>\r\n                    <td *ngIf=\"hasRecentTrends && (row.isSubheading == false)\" class=\"center pointer\" (click)=recentTrendClicked(row)\r\n                        (mouseout)=hideTooltip() (mouseover)=showRecentTrendTooltip($event,row) (mousemove)=positionTooltip($event)\r\n                        [innerHTML]=\"row.recentTrendHtml\"></td>\r\n                    <td *ngIf=\"isChangeFromPreviousPeriodShown && (row.isSubheading == false)\" class=\"center\"\r\n                        [innerHTML]=\"row.changeFromPreviousHtml\"></td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n    </div>\r\n</div>"

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
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var indicator_service_1 = __webpack_require__("./src/app/shared/service/api/indicator.service.ts");
var legend_component_1 = __webpack_require__("./src/app/shared/component/legend/legend.component.ts");
var EnglandComponent = (function () {
    function EnglandComponent(ftHelperService, indicatorService) {
        this.ftHelperService = ftHelperService;
        this.indicatorService = indicatorService;
        this.keyType = legend_component_1.KeyType.None;
        this.legendType = legend_component_1.LegendType.None;
        this.showRecentTrends = true;
        this.isChangeFromPreviousPeriodShown = false;
        this.hasRecentTrends = false;
    }
    EnglandComponent.prototype.onOutsideEvent = function (event) {
        var _this = this;
        var ftHelper = this.ftHelperService;
        this.setConfig();
        this.tooltip = new shared_1.TooltipHelper(this.ftHelperService.newTooltipManager());
        var groupingSubheadings = ftHelper.getGroupingSubheadings();
        var metadataHash = ftHelper.getMetadataHash();
        var groupRoots = ftHelper.getAllGroupRoots();
        this.rows = [];
        var _loop_1 = function (rootIndex) {
            var root = groupRoots[rootIndex];
            var indicatorId = root.IID;
            var metadata = metadataHash[indicatorId];
            var unit = !!metadata ? metadata.Unit : null;
            var subheadings = groupingSubheadings.filter(function (x) { return x.Sequence === root.Sequence; });
            if (subheadings !== undefined) {
                subheadings.forEach(function (subheading) {
                    var row = new EnglandRow();
                    _this.rows.push(row);
                    row.indicatorName = subheading.Subheading;
                    row.isSubheading = true;
                });
            }
            var row = new EnglandRow();
            this_1.rows.push(row);
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
                this_1.setUpRecentTrendOnRow(row, root, englandData.AreaCode, dataInfo);
            }
            row.isSubheading = false;
        };
        var this_1 = this;
        for (var rootIndex in groupRoots) {
            _loop_1(rootIndex);
        }
        ftHelper.showAndHidePageElements();
        ftHelper.unlock();
        // Enable tooltip
        setTimeout(function () {
            $('[data-toggle="tooltip"]').tooltip();
        }, 0);
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
    EnglandComponent.prototype.hideTooltip = function () {
        this.tooltip.hide();
    };
    EnglandComponent.prototype.indicatorNameClicked = function (row) {
        this.ftHelperService.goToMetadataPage(row.rootIndex);
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
    EnglandComponent.prototype.onExportCsvFileClick = function (event) {
        event.preventDefault();
        alert('It works!');
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

/***/ "./src/app/england/england.module.ts":
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
var england_component_1 = __webpack_require__("./src/app/england/england.component.ts");
var legend_module_1 = __webpack_require__("./src/app/shared/component/legend/legend.module.ts");
var EnglandModule = (function () {
    function EnglandModule() {
    }
    return EnglandModule;
}());
EnglandModule = __decorate([
    core_1.NgModule({
        imports: [
            common_1.CommonModule,
            legend_module_1.LegendModule
        ],
        declarations: [
            england_component_1.EnglandComponent
        ],
        exports: [
            england_component_1.EnglandComponent
        ]
    })
], EnglandModule);
exports.EnglandModule = EnglandModule;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/england.module.js.map

/***/ }),

/***/ "./src/app/map/google-map-simple/google-map-simple.component.css":
/***/ (function(module, exports) {

module.exports = ".googleMapNg {\r\n    position: relative;\r\n    background-color: #fff;\r\n    border: 1px solid #CCC;\r\n    width: 500px;\r\n    height: 600px;\r\n    float: left;\r\n    margin-bottom: 10px;\r\n    margin-top: 0px;\r\n    clear: both;\r\n}\r\n\r\n#floating-panel {\r\n    position: absolute;\r\n    margin-top: 92px;\r\n    z-index: 5;\r\n    padding-left: 8px;\r\n    margin-left: 10px;\r\n}\r\n\r\n#wrapper {\r\n    position: relative;\r\n}\r\n\r\n.map-control {\r\n    float: left;\r\n    clear: both;\r\n    position: relative;\r\n    z-index: 7;\r\n    pointer-events: auto;\r\n}\r\n\r\n.map-control-layers-toggle {\r\n    background-image: url(/images/layers.png);\r\n    background-repeat: no-repeat;\r\n    width: 28px;\r\n    height: 28px;\r\n    display: inline-block;\r\n    margin-right: 3px\r\n}\r\n\r\n.layerControl p {\r\n    margin-left: 5px;\r\n}\r\n\r\n.layerControl div.basemap {\r\n    margin: 3px 0;\r\n    padding-right: 3px;\r\n}\r\n\r\n.layerControl label.basemapLabel {\r\n    display: block;\r\n    margin-bottom: 5px;\r\n    height: 40px;\r\n}\r\n\r\n.layerControl div.basemap:hover,\r\n.layerControl div.opacity:hover {\r\n    cursor: pointer;\r\n}\r\n\r\n.layerControl div.selected span {\r\n    border: 1px solid #66F;\r\n}\r\n\r\n.layerControl div.basemap span {\r\n    margin: 0 5px;\r\n}\r\n\r\n.layerControl div.None span {\r\n    background: #FFF;\r\n}\r\n\r\n.layerControl div.Streets span {\r\n    background-position: -6px -113px;\r\n}\r\n\r\n.layerControl div#opacity {\r\n    margin-left: 5px;\r\n}\r\n\r\n.layerControl div.opacity {\r\n    display: inline-block;\r\n    width: 34px;\r\n    text-align: center;\r\n}\r\n\r\n.layerControl div.opacity span {\r\n    border: 1px solid #CCC;\r\n    display: inline-block;\r\n    width: 30px;\r\n    height: 25px;\r\n}\r\n\r\n.layerControl div.opacity:hover span,\r\n.layerControl div.opacity.selected span {\r\n    border-color: #666;\r\n    cursor: pointer;\r\n}\r\n\r\n.layerControl div.opacity label {\r\n    font-size: .65em;\r\n}\r\n\r\n.layerControl div.opacity span {\r\n    background: url(/images/opacity.jpg) no-repeat;\r\n}\r\n\r\n.layerControl input {\r\n    display: none;\r\n}\r\n\r\n.layerControl>label {\r\n    display: block;\r\n    height: 100%;\r\n    width: 100%;\r\n}\r\n\r\n.info {\r\n    padding: 2px;\r\n    font: 14px/16px Arial, Helvetica, sans-serif;\r\n    background: white;\r\n    background: rgba(255, 255, 255, 0.8);\r\n    -webkit-box-shadow: 0 0 15px rgba(0, 0, 0, 0.2);\r\n            box-shadow: 0 0 15px rgba(0, 0, 0, 0.2);\r\n    border-radius: 5px;\r\n}\r\n\r\n.info h4 {\r\n    margin: 0;\r\n    color: #777;\r\n    padding: 0;\r\n}\r\n\r\n.export-chart-box {\r\n    margin: 0;\r\n}\r\n\r\n.label-name {\r\n    background: #FFF;\r\n    width: 60px;\r\n    height: 30px;\r\n    display: inline-block;\r\n    vertical-align: middle;\r\n}\r\n\r\n.label-image {\r\n    background: url(/images/basemaps.jpg) no-repeat;\r\n    width: 60px;\r\n    height: 30px;\r\n    display: inline-block;\r\n    border: 1px solid;\r\n    vertical-align: middle;\r\n}\r\n"

/***/ }),

/***/ "./src/app/map/google-map-simple/google-map-simple.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"wrapper\">\r\n    <div [hidden]=\"isError\" id=\"google-map\" #googleMap class=\"googleMapNg\"></div>\r\n    <div *ngIf=\"isError\" id=\"polygonError\" class=\"googleMapNg\"> {{errorMessage}} </div>\r\n    <div [hidden]=\"isError\" id=\"floating-panel\" class=\"layerControl info map-control\">\r\n        <a class=\"map-control-layers-toggle\" href=\"#\" title=\"Layers\" *ngIf=\"!showOverlay\" (mouseover)=\"displayOverlay()\"></a>\r\n        <div id=\"mapOptions\" *ngIf=\"showOverlay\" (mouseleave)=\"hideOverlay()\">\r\n            <p>Background map</p>\r\n            <label *ngFor=\"let baseMap of baseMaps;let idx = index\" class=\"basemapLabel\">\r\n                <div class=\"basemap {{baseMap.cssClass}}\">\r\n                    <input type=\"radio\" name=\"baseMap\" [value]=\"baseMap.val\" [checked]=\"(idx === 0)\" (click)=\"onOverlaySelectionChange(baseMap)\">\r\n                    <span class=\"label-image\"></span>\r\n                    <span class=\"label-name\">{{baseMap.name}}</span>\r\n                </div>\r\n                <br/>\r\n            </label>\r\n            <p>Transparency</p>\r\n            <div class=\"opacity\" *ngFor=\"let opac of opacityArray;let idx = index;\" [attr.selected]=\"opac/100 == fillOpacity?true : null\">\r\n                <input type=\"radio\" name=\"opacity\" value=\"{{opac}}\" id=\"opacity_{{opac}}\" (click)=\"onOpacitySelectionChange(opac)\">\r\n                <label for=\"opacity_{{opac}}\">\r\n                    <span [ngStyle]=\"{\r\n                    'background-position':(opac * -4.1 + 37) + 'px 0px'}\"></span>\r\n                    {{opac}} %\r\n                </label>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>"

/***/ }),

/***/ "./src/app/map/google-map-simple/google-map-simple.component.ts":
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
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var GoogleMapSimpleComponent = (function () {
    function GoogleMapSimpleComponent(mapService, ftHelperService) {
        this.mapService = mapService;
        this.ftHelperService = ftHelperService;
        this.mapInit = new core_1.EventEmitter();
        this.selectedAreaChanged = new core_1.EventEmitter();
        this.areaTypeId = null;
        this.areaSearchText = null;
        this.areaCode = null;
        this.availableAreas = null;
        this.selectedAreas = null;
        this.mapAreaCodes = null;
        this.mapPolygonSelected = null;
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
    }
    GoogleMapSimpleComponent.prototype.ngOnChanges = function (changes) {
        if (changes['areaTypeId']) {
            if (this.areaTypeId) {
                this.path = this.ftHelperService.getURL().img;
                this.loadMap();
                this.loadPolygon(this.areaTypeId, this.path);
            }
        }
        if (changes['areaCode']) {
            if (this.areaCode) {
                this.updateMapSelection(this.areaCode);
            }
        }
        if (changes['mapAreaCodes']) {
            if (this.mapAreaCodes) {
                this.initialiseMapSelection(this.mapAreaCodes);
            }
        }
    };
    GoogleMapSimpleComponent.prototype.initialiseMapSelection = function (areaCodes) {
        var _this = this;
        areaCodes.forEach(function (element) {
            _this.updateMapSelection(element);
        });
    };
    GoogleMapSimpleComponent.prototype.updateMapSelection = function (areaCode) {
        for (var i = 0; i < (this.currentPolygons.length); i++) {
            var polygon = this.currentPolygons[i];
            var polygonAreaCode = polygon.get('areaCode');
            if (polygonAreaCode === undefined || polygonAreaCode === areaCode) {
                if (this.mapPolygonSelected) {
                    polygon.set('fillColor', '#7FFF92');
                }
                else {
                    polygon.set('fillColor', '#63A1C3');
                }
            }
        }
    };
    GoogleMapSimpleComponent.prototype.loadMap = function () {
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
    };
    GoogleMapSimpleComponent.prototype.loadPolygon = function (areaTypeId, path) {
        var _this = this;
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
    };
    GoogleMapSimpleComponent.prototype.removePolygon = function () {
        if (this.currentPolygons !== undefined) {
            this.currentPolygons.forEach(function (element) {
                element.setMap(null);
            });
            this.currentPolygons.length = 0;
        }
    };
    GoogleMapSimpleComponent.prototype.fillPolygon = function (boundry, opacity) {
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
                var polygon = new google.maps.Polygon({
                    paths: coords,
                    strokeColor: '#333333',
                    strokeOpacity: 0.8,
                    strokeWeight: 1,
                    fillColor: '#63A1C3',
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
                });
                google.maps.event.addListener(polygon, 'mousemove', function (event) {
                    infoWindow_1.close();
                });
                google.maps.event.addListener(polygon, 'mouseout', function (event) {
                    // Wait in case immediate mouseover event and this mouseover event was
                    // caused by mouse moving over the infowindow
                    setTimeout(function () {
                        var time = new Date().getTime();
                        if (time - overDate_1.getTime() > 25 && areaCode === overAreaCode_1) {
                            infoWindow_1.close();
                        }
                    }, 25);
                });
                google.maps.event.addListener(polygon, 'click', function (event) {
                    if (polygon.get('fillColor') === '#63A1C3') {
                        polygon.set('fillColor', '#7FFF92');
                        _this.selectedAreaChanged.emit({ areaCode: areaCode, add: true });
                    }
                    else {
                        polygon.set('fillColor', '#63A1C3');
                        _this.selectedAreaChanged.emit({ areaCode: areaCode, add: false });
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
    GoogleMapSimpleComponent.prototype.getToolTipContent = function (areaCode) {
        var areaName;
        var areaCodeInAvailableArea = this.availableAreas.find(function (x) { return x.Code === areaCode; });
        if (areaCodeInAvailableArea == null || areaCodeInAvailableArea === undefined) {
            areaName = '';
        }
        else {
            areaName = areaCodeInAvailableArea.Name;
        }
        return areaName;
    };
    GoogleMapSimpleComponent.prototype.setInfoWindowPosition = function (event, infoWindow) {
        var pos = event.latLng;
        infoWindow.setPosition(new google.maps.LatLng(pos.lat() + 0.02, pos.lng()));
    };
    GoogleMapSimpleComponent.prototype.colourFillPolygon = function (center) {
        if (this.map) {
            var regionPolygons = [];
            var currentComparatorId = this.ftHelperService.getComparatorId();
            var _loop_2 = function (i) {
                var polygon = this_2.currentPolygons[i];
                // Set polygon fill colour
                polygon.setMap(null);
                var areaCode = polygon.get('areaCode');
                var areaCodeInSelectedAreas = this_2.selectedAreas.find(function (x) { return x.Code === areaCode; });
                if (areaCodeInSelectedAreas !== null && areaCodeInSelectedAreas !== undefined) {
                    polygon.set('fillColor', '#7FFF92');
                }
                else {
                    polygon.set('fillColor', '#63A1C3');
                }
                polygon.setMap(this_2.map);
            };
            var this_2 = this;
            for (var i = 0; i < (this.currentPolygons.length); i++) {
                _loop_2(i);
            }
            if (center) {
                this.setMapCenter();
            }
        }
    };
    GoogleMapSimpleComponent.prototype.setMapCenter = function () {
        if (this.map) {
            var bounds = new google.maps.LatLngBounds();
            var position = new google.maps.LatLng(53.415649, -2.209015);
            bounds.extend(position);
            this.map.setCenter(bounds.getCenter());
            this.map.setZoom(6);
        }
    };
    GoogleMapSimpleComponent.prototype.getPolygonCoordinates = function (coordinates) {
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
    GoogleMapSimpleComponent.prototype.displayOverlay = function () {
        this.showOverlay = true;
    };
    GoogleMapSimpleComponent.prototype.hideOverlay = function () {
        this.showOverlay = false;
    };
    GoogleMapSimpleComponent.prototype.onOverlaySelectionChange = function (basemap) {
        this.selectedbaseMap = Object.assign({}, this.selectedbaseMap, basemap);
        this.styleMap(this.selectedbaseMap);
    };
    GoogleMapSimpleComponent.prototype.onOpacitySelectionChange = function (opacity) {
        this.fillOpacity = opacity / 100;
        this.loadPolygon(this.areaTypeId, this.path);
    };
    GoogleMapSimpleComponent.prototype.styleMap = function (selectedbaseMap) {
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
            google.maps.event.addListener(this.map, 'idle', function (event) {
                var bounds = _this.map.getCenter();
                google.maps.event.trigger(_this.map, 'resize');
                _this.map.setCenter(bounds);
            });
        }
    };
    return GoogleMapSimpleComponent;
}());
__decorate([
    core_1.ViewChild('googleMap'),
    __metadata("design:type", typeof (_a = typeof core_1.ElementRef !== "undefined" && core_1.ElementRef) === "function" && _a || Object)
], GoogleMapSimpleComponent.prototype, "mapEl", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], GoogleMapSimpleComponent.prototype, "mapInit", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], GoogleMapSimpleComponent.prototype, "selectedAreaChanged", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Number)
], GoogleMapSimpleComponent.prototype, "areaTypeId", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], GoogleMapSimpleComponent.prototype, "areaSearchText", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], GoogleMapSimpleComponent.prototype, "areaCode", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], GoogleMapSimpleComponent.prototype, "availableAreas", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], GoogleMapSimpleComponent.prototype, "selectedAreas", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], GoogleMapSimpleComponent.prototype, "mapAreaCodes", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], GoogleMapSimpleComponent.prototype, "mapPolygonSelected", void 0);
GoogleMapSimpleComponent = __decorate([
    core_1.Component({
        selector: 'ft-google-map-simple',
        template: __webpack_require__("./src/app/map/google-map-simple/google-map-simple.component.html"),
        styles: [__webpack_require__("./src/app/map/google-map-simple/google-map-simple.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_b = typeof googleMap_service_1.GoogleMapService !== "undefined" && googleMap_service_1.GoogleMapService) === "function" && _b || Object, typeof (_c = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _c || Object])
], GoogleMapSimpleComponent);
exports.GoogleMapSimpleComponent = GoogleMapSimpleComponent;
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
var _a, _b, _c;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/google-map-simple.component.js.map

/***/ }),

/***/ "./src/app/map/google-map/google-map.component.css":
/***/ (function(module, exports) {

module.exports = ".googleMapNg {\r\n    position: relative;\r\n    background-color: #fff;\r\n    border: 1px solid #CCC;\r\n    width: 500px;\r\n    height: 600px;\r\n    float: left;\r\n    margin-bottom: 10px;\r\n    margin-top: 10px;\r\n    clear: both;\r\n}\r\n\r\n#google-map {\r\n    overflow: auto !important;\r\n}\r\n\r\n#google-map::-webkit-scrollbar { \r\n    display: none; \r\n}\r\n\r\n#floating-panel {\r\n    position: absolute;\r\n    margin-top: 148px;\r\n    z-index: 5;\r\n    padding-left: 8px;\r\n    margin-left: 10px;\r\n}\r\n\r\n#wrapper {\r\n    position: relative;\r\n}\r\n\r\n.map-control {\r\n    float: left;\r\n    clear: both;\r\n    position: relative;\r\n    z-index: 7;\r\n    pointer-events: auto;\r\n}\r\n\r\n.map-control-layers-toggle {\r\n    background-image: url(/images/layers.png);\r\n    background-repeat: no-repeat;\r\n    width: 28px;\r\n    height: 28px;\r\n    display: inline-block;\r\n    margin-right: 3px\r\n}\r\n\r\n.layerControl p {\r\n    margin-left: 5px;\r\n}\r\n\r\n.layerControl div.basemap {\r\n    margin: 3px 0;\r\n    padding-right: 3px;\r\n}\r\n\r\n.layerControl label.basemapLabel {\r\n    display: block;\r\n    margin-bottom: 0px;\r\n    height: 40px;\r\n}\r\n\r\n.layerControl div.basemap:hover,\r\n.layerControl div.opacity:hover {\r\n    background-color: rgba(220, 220, 220, .7);\r\n}\r\n\r\n.layerControl div:hover span {\r\n    border: 1px solid #666;\r\n}\r\n\r\n.layerControl div.selected span {\r\n    border: 1px solid #66F;\r\n}\r\n\r\n.layerControl div .layerControl label span {\r\n    border: 1px solid #CCC;\r\n    width: 70px;\r\n    height: 30px;\r\n    background: url(/images/basemaps.jpg) no-repeat;\r\n    display: inline-block;\r\n    vertical-align: middle;\r\n}\r\n\r\n.layerControl div.basemap span {\r\n    margin: 0 5px;\r\n}\r\n\r\n.layerControl div.None span {\r\n    background: #FFF;\r\n}\r\n\r\n.layerControl div.Streets span {\r\n    background-position: -6px -113px;\r\n}\r\n\r\n.layerControl div#opacity {\r\n    margin-left: 5px;\r\n}\r\n\r\n.layerControl div.opacity {\r\n    display: inline-block;\r\n    width: 34px;\r\n    text-align: center;\r\n}\r\n\r\n.layerControl div.opacity span {\r\n    border: 1px solid #CCC;\r\n    display: inline-block;\r\n    width: 30px;\r\n    height: 25px;\r\n}\r\n\r\n.layerControl div.opacity:hover span,\r\n.layerControl div.opacity.selected span {\r\n    border-color: #666;\r\n}\r\n\r\n.layerControl div.opacity label {\r\n    font-size: .65em;\r\n}\r\n\r\n.layerControl div.opacity span {\r\n    background: url(/images/opacity.jpg) no-repeat;\r\n}\r\n\r\n.layerControl input {\r\n    display: none;\r\n}\r\n\r\n.layerControl>label {\r\n    display: block;\r\n    height: 100%;\r\n    width: 100%;\r\n}\r\n\r\n.info {\r\n    padding: 2px;\r\n    font: 14px/16px Arial, Helvetica, sans-serif;\r\n    background: white;\r\n    background: rgba(255, 255, 255, 0.8);\r\n    -webkit-box-shadow: 0 0 15px rgba(0, 0, 0, 0.2);\r\n            box-shadow: 0 0 15px rgba(0, 0, 0, 0.2);\r\n    border-radius: 5px;\r\n}\r\n\r\n.info h4 {\r\n    margin: 0;\r\n    color: #777;\r\n    padding: 0;\r\n}\r\n\r\n.export-chart-box {\r\n    margin: 0;\r\n}\r\n\r\n.tab-options button.button-selected, .tab-options button:hover {\r\n    color: #fff;\r\n    background-color: #02ae94;\r\n}\r\n\r\n.tab-options div.export-chart-box {\r\n    margin-top: 15px;\r\n}\r\n"

/***/ }),

/***/ "./src/app/map/google-map/google-map.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"tab-specific-options\" class=\"tab-options clearfix\">\r\n    <div>\r\n        <span>Areas </span>\r\n        <button id=\"tab-option-0\" [class]=\"this.subNationalButtonClass\" (click)=\"setBenchMark(1)\" [innerHTML]=\"this.formattedParentAreaName\"></button>\r\n        <button id=\"tab-option-1\" [class]=\"this.nationalButtonClass\" (click)=\"setBenchMark(4)\">All in England</button>\r\n    </div>\r\n    <div class=\"export-chart-box\">\r\n        <a class=\"export-link\" (click)=\"onExportClick($event)\">Export map as image</a>\r\n    </div>\r\n    <div class=\"export-chart-box-csv\">\r\n        <a class=\"export-link-csv hidden\" (click)=\"onExportCsvFileClick($event)\">Export map as CSV file</a>\r\n    </div>\r\n</div>\r\n<div id=\"wrapper\">\r\n    <div [hidden]=\"isError\" id=\"google-map\" #googleMap class=\"googleMapNg\"></div>\r\n    <div *ngIf=\"isError\" id=\"polygonError\" class=\"googleMapNg\"> {{errorMessage}} </div>\r\n    <div [hidden]=\"isError\" id=\"floating-panel\" class=\"layerControl info map-control\">\r\n        <a class=\"map-control-layers-toggle\" href=\"#\" title=\"Layers\" *ngIf=\"!showOverlay\" (mouseover)=\"displayOverlay()\"></a>\r\n        <div id=\"mapOptions\" *ngIf=\"showOverlay\" (mouseleave)=\"hideOverlay()\">\r\n            <p>Background map</p>\r\n            <label *ngFor=\"let baseMap of baseMaps;let idx = index\" class=\"basemapLabel\">\r\n                <div class=\"basemap {{baseMap.cssClass}}\">\r\n                    <input type=\"radio\" name=\"baseMap\" [value]=\"baseMap.val\" [checked]=\"(idx === 0)\" (click)=\"onOverlaySelectionChange(baseMap)\">\r\n                    <span></span>{{baseMap.name}}\r\n                </div>\r\n                <br />\r\n            </label>\r\n            <p>Transparency</p>\r\n            <div class=\"opacity\" *ngFor=\"let opac of opacityArray;let idx = index;\" [attr.selected]=\"opac/100 == fillOpacity?true : null\">\r\n                <input type=\"radio\" name=\"opacity\" value=\"{{opac}}\" id=\"opacity_{{opac}}\" (click)=\"onOpacitySelectionChange(opac)\">\r\n                <label for=\"opacity_{{opac}}\">\r\n                    <span [ngStyle]=\"{\r\n                    'background-position':(opac * -4.1 + 37) + 'px 0px'}\"></span>\r\n                    {{opac}} %\r\n                </label>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>"

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
        this.benchmarkChanged = new core_1.EventEmitter();
        this.areaTypeId = null;
        this.currentAreaCode = null;
        this.areaCodeColour = null;
        this.sortedCoreData = null;
        this.benchmarkIndex = null;
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
                    this.colourFillPolygon(true);
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
            // Load from GoogleMapService and style it
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
                        if (time - overDate_1.getTime() > 25 && areaCode === overAreaCode_1) {
                            infoWindow_1.close();
                        }
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
        var _this = this;
        var parentTypeId = this.ftHelperService.getParentTypeId();
        var areaTypeId = this.ftHelperService.getAreaTypeId();
        if (parentTypeId !== null && parentTypeId !== undefined &&
            areaTypeId !== null && areaTypeId !== undefined) {
            var key = parentTypeId.toString() + "-" + areaTypeId.toString() + "-";
            var areaMappings = this.ftHelperService.getAreaMappingsForParentCode(key);
            if (this.map) {
                var regionPolygons = [];
                var _loop_2 = function (i) {
                    var polygon = this_2.currentPolygons[i];
                    // Set polygon fill colour
                    polygon.setMap(null);
                    var areaCode = polygon.get('areaCode');
                    var color = this_2.areaCodeColour.get(areaCode);
                    // Region tab button clicked
                    if (this_2.benchmarkIndex === shared_1.ComparatorIds.SubNational &&
                        areaMappings.findIndex(function (x) { return x.toString() === areaCode; }) === -1) {
                        color = '#B0B0B2';
                    }
                    // Set to default color if not defined
                    if (color === undefined) {
                        color = '#B0B0B2';
                    }
                    polygon.set('fillColor', color);
                    polygon.setMap(this_2.map);
                    if (this_2.benchmarkIndex === shared_1.ComparatorIds.SubNational &&
                        areaMappings.findIndex(function (x) { return x.toString() === areaCode; }) !== -1) {
                        var coreDataset = this_2.sortedCoreData[areaCode];
                        if (coreDataset) {
                            regionPolygons.push(polygon);
                        }
                    }
                };
                var this_2 = this;
                for (var i = 0; i < (this.currentPolygons.length); i++) {
                    _loop_2(i);
                }
                /*if Benchmark is region, center and zoom in into that region */
                if (center) {
                    if (regionPolygons.length > 0 && this.benchmarkIndex !== shared_1.ComparatorIds.National) {
                        var bounds = new google.maps.LatLngBounds();
                        for (var i = 0; i < regionPolygons.length; i++) {
                            bounds.extend(this.getPolygonBounds(regionPolygons[i]).getCenter());
                        }
                        this.map.fitBounds(bounds);
                        this.map.setCenter(bounds.getCenter());
                        if (areaTypeId === shared_1.AreaTypeIds.MSOA || areaTypeId === shared_1.AreaTypeIds.Ward) {
                            this.map.setZoom(10);
                        }
                        else {
                            this.map.setZoom(7);
                        }
                    }
                    if (this.benchmarkIndex === shared_1.ComparatorIds.National) {
                        this.setMapCenter();
                    }
                }
                var parentAreaName = this.ftHelperService.getParentArea().Name;
                if (parentAreaName !== undefined) {
                    this.formattedParentAreaName = 'All in ' + parentAreaName;
                }
                setTimeout(function () {
                    _this.applyTabButtonStyles();
                }, 0);
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
        // Define html to display the title
        var title = '<b>Map of ' + this.ftHelperService.getAreaTypeName() +
            's in ' + this.ftHelperService.getCurrentComparator().Name +
            ' for ' + indicatorName + '<br/> (' + chartTitle + ')</b>';
        // Define script to hide the zoom in, zoom out and full screen buttons
        var script = '<script>$(".gmnoprint").hide(); $(".gm-fullscreen-control").hide();</script>';
        // Inject both the title html and button hide script
        $('<div id="map-export-title" style="text-align: center; font-family:Arial;">' +
            title + script + '</div>').appendTo(this.mapEl.nativeElement);
        // Download as image
        this.ftHelperService.saveElementAsImage(this.mapEl.nativeElement, 'Map');
        // Define script to show the zoom in, zoom out and full screen buttons
        script = '<script>$(".gmnoprint").show(); $(".gm-fullscreen-control").show();</script>';
        // Inject button show script
        $('<div id="show-buttons">' + script + '</div>').appendTo(this.mapEl.nativeElement);
        // Remove the injected html and scripts
        $('#map-export-title').remove();
        $('#show-buttons').remove();
        // Log export image event
        this.ftHelperService.logEvent('ExportImage', 'Map');
    };
    GoogleMapComponent.prototype.onExportCsvFileClick = function (event) {
        alert('It works!');
    };
    GoogleMapComponent.prototype.buildChartTitle = function () {
        var currentGrpRoot = this.ftHelperService.getCurrentGroupRoot();
        var data = this.ftHelperService.getMetadata(currentGrpRoot.IID);
        var unit = data.Unit;
        var unitLabel = (typeof unit.Label !== 'undefined') ? unit.Label : '';
        var period = currentGrpRoot.Grouping[0].Period;
        return data.ValueType.Name + ' - ' + unitLabel + ' ' + period;
    };
    GoogleMapComponent.prototype.setBenchMark = function (benchmarkIndex) {
        this.benchmarkIndex = benchmarkIndex;
        this.benchmarkChanged.emit({ benchmarkIndex: benchmarkIndex });
        return false;
    };
    GoogleMapComponent.prototype.applyTabButtonStyles = function () {
        if (this.ftHelperService.getParentTypeId() === shared_1.AreaTypeIds.Country) {
            this.nationalButtonClass = 'button-selected';
            this.subNationalButtonClass = 'hidden';
        }
        else if (this.benchmarkIndex === undefined || this.benchmarkIndex === shared_1.ComparatorIds.National) {
            this.nationalButtonClass = 'button-selected';
            this.subNationalButtonClass = '';
        }
        else {
            this.nationalButtonClass = '';
            this.subNationalButtonClass = 'button-selected';
        }
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
    core_1.Output(),
    __metadata("design:type", Object)
], GoogleMapComponent.prototype, "benchmarkChanged", void 0);
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
__decorate([
    core_1.Input(),
    __metadata("design:type", Number)
], GoogleMapComponent.prototype, "benchmarkIndex", void 0);
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

module.exports = "<span class=\"export-chart-box\" style=\"margin-left: 11px;float: left\">\r\n    <a class=\"export-link\" (click)=\"onExportClick($event)\">Export chart as image</a>\r\n</span>\r\n<div #mapChart class=\"chartClass\"></div>"

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
            if (grouping.ComparatorData.ValF === '-') {
                continue;
            }
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

/***/ "./src/app/map/map-table/map-table.component.css":
/***/ (function(module, exports) {

module.exports = "table {\r\n    width: 100%;\r\n    border: 1px solid #ccc;\r\n    padding: 0;\r\n    border-collapse: separate;\r\n    background-color: transparent;\r\n}\r\n\r\n\r\n/* Table header */\r\n\r\n\r\nth {\r\n    font-weight: normal;\r\n    text-align: right;\r\n    padding-right: 10px;\r\n    background-color: #EEEEEE;\r\n    cursor: pointer;\r\n}\r\n\r\n\r\ntr th:first-child {\r\n    text-align: left;\r\n    padding-left: 10px;\r\n}\r\n\r\n\r\n/* Table body */\r\n\r\n\r\ntd {\r\n    font-weight: normal;\r\n    color: #999;\r\n    padding-right: 10px;\r\n    text-align: right;\r\n}\r\n\r\n\r\ntd.value-note-symbol {\r\n    width: 100%;\r\n    text-align: center;\r\n    display: block;\r\n    cursor: default;\r\n}\r\n\r\n\r\ntr td:first-child {\r\n    padding-left: 5px;\r\n    border-left-width: 5px;\r\n    border-left-style: solid;\r\n    text-align: left;\r\n}\r\n\r\n\r\ntr.hover td:first-child {\r\n    border-left-style: solid;\r\n}\r\n\r\n\r\ntr.selected td {\r\n    background-color: aliceblue;\r\n    font-weight: bold;\r\n    font-style: italic;\r\n    cursor: pointer;\r\n    border: 1px solid gainsboro;\r\n}\r\n\r\n\r\n.rowHover {\r\n    background-color: lavender;\r\n    cursor: pointer;\r\n}"

/***/ }),

/***/ "./src/app/map/map-table/map-table.component.html":
/***/ (function(module, exports) {

module.exports = "<table *ngIf=\"selectedCoreData?.length > 0\" #maptable>\r\n    <thead>\r\n        <tr>\r\n            <th (click)=\"sortByAreaName()\" style=\"text-align:left\">Area</th>\r\n            <th (click)=\"sortByCount()\">Count</th>\r\n            <th (click)=\"sortByValue()\">Value</th>\r\n            <th>LCI</th>\r\n            <th>UCI</th>\r\n        </tr>\r\n    </thead>\r\n    <tbody>\r\n        <tr *ngFor=\"let item of selectedCoreData\" (mouseover)=\"onRowMouseOver($event)\" (mouseout)=\"onRowMouseLeave($event)\" [attr.areaCode]=\"item.areaCode\" [class.rowHover]=\"rowMouseHovered\" (click)=\"onRowClick($event)\">\r\n            <td [ngStyle]=\"{'border-left-color': item.colour}\">{{item.areaName}}</td>\r\n            <td>{{item.formattedCount}}</td>\r\n            <td [ngClass]=\"{'hasNote value-note-symbol': item.isNote}\" [attr.data-NoteId]=\"item.isNote?item.noteId:null\" (mouseenter)=\"onMouseEnter($event)\" (mousemove)=\"onMouseMove($event)\" (mouseleave)=\"onMouseLeave($event)\" innerHTML=\"{{item.formattedValue}}\">\r\n                <div style=\"display:inline\" *ngIf=\"item.isNote\">*</div>\r\n            </td>\r\n            <td>\r\n                <div innerHTML=\"{{item.loCI}}\"></div>\r\n            </td>\r\n            <td>\r\n                <div innerHTML=\"{{item.upCI}}\"></div>\r\n            </td>\r\n        </tr>\r\n    </tbody>\r\n</table>"

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

module.exports = "#maps_info {\r\n    margin-left: 10px;\r\n    margin-bottom: 10px;\r\n    width: 490px;\r\n    height: 350px;\r\n    float: left;\r\n    overflow: auto;\r\n}\r\n\r\n#key-ad-hoc {\r\n    position: relative;\r\n    height: 50px;\r\n}\r\n\r\n#map_colour_box {\r\n    border: 1px solid #CCC;\r\n    background-color: #EEE;\r\n    padding: 5px 10px;\r\n    margin-top: 10px;\r\n    margin-bottom: 10px;\r\n}\r\n\r\n#boundryNotSupported {\r\n    width:100%; \r\n    height:400px;\r\n    padding-top:190px; \r\n    text-align:center;\r\n    font-size:17px;\r\n}\r\n"

/***/ }),

/***/ "./src/app/map/map.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"map-container\" class=\"standard-width clearfix\" [style.display]=\"searchMode ? 'block' : 'none'\">\r\n    <div [hidden]=\"isBoundaryNotSupported\">\r\n        <div *ngIf=\"isInitialised\">\r\n            <div *ngIf=\"!IsPracticeAreaType\">\r\n                <div id=\"key-ad-hoc\" class=\"key-container\" *ngIf=\"showAdhocKey\" [innerHtml]=\"htmlAdhocKey\" style=\"display: block;overflow: hidden;\"></div>\r\n                <ft-legend [pageType]=\"pageType\" [showRAG3]=\"showRAG3\" [showRAG5]=\"showRAG5\" [showBOB]=\"showBOB\"\r\n                    [showQuartiles]=\"showQuartiles\" [showQuintilesRAG]=\"showQuintilesRAG\" [showQuintilesBOB]=\"showQuintilesBOB\"\r\n                    [showContinuous]=\"showContinuous\"></ft-legend>\r\n                <br>\r\n                <ft-google-map (mapInit)=\"onMapInit($event)\" [areaTypeId]=\"areaTypeId\" [sortedCoreData]=\"sortedCoreData\"\r\n                    [currentAreaCode]=\"currentAreaCode\" [areaCodeColour]=\"areaCodeColour\" [refreshColour]=\"refreshColour\"\r\n                    (hoverAreaCodeChanged)=\"onhoverAreaCodeChangedMap($event)\" (selectedAreaChanged)=\"onSelectedAreaChanged($event)\"\r\n                    (benchmarkChanged)=\"OnBenchMarkIndexChanged($event)\" [isBoundaryNotSupported]=\"isBoundaryNotSupported\"\r\n                    [selectedAreaList]=\"selectedAreaList\" [benchmarkIndex]=\"benchmarkIndex\">\r\n                </ft-google-map>\r\n                <div id=\"maps_info\">\r\n                    <div id='map_colour_box'>Map colour&nbsp;\r\n                        <select id='map_colour' (change)=\"onMapColourBoxChange($event.target.value)\" [(ngModel)]=\"mapColourSelectedValue\">\r\n                            <option value='benchmark'>Comparison to benchmark</option>\r\n                            <option value='quartile'>Quartiles</option>\r\n                            <option value='quintile'>Quintiles</option>\r\n                            <option value='continuous'>Continuous</option>\r\n                        </select>\r\n                    </div>\r\n                    <ft-map-table [areaTypeId]=\"areaTypeId\" [sortedCoreData]=\"sortedCoreData\" [selectedAreaList]=\"selectedAreaList\"\r\n                        [areaCodeColour]=\"areaCodeColour\" [isBoundaryNotSupported]=\"isBoundaryNotSupported\"\r\n                        (hoverAreaCodeChanged)=\"onhoverAreaCodeChangedData($event)\" (selectedAreaChanged)=\"onSelectedAreaChanged($event)\"></ft-map-table>\r\n                </div>\r\n                <ft-map-chart [areaTypeId]=\"areaTypeId\" [sortedCoreData]=\"sortedCoreData\" [currentAreaCode]=\"currentAreaCode\"\r\n                    [isBoundaryNotSupported]=\"isBoundaryNotSupported\" [areaCodeColour]=\"areaCodeColour\"\r\n                    (selectedAreaCodeChanged)=\"onSelectedAreaCodeChanged($event)\" (hoverAreaCodeChanged)=\"onhoverAreaCodeChangedChart($event)\"></ft-map-chart>\r\n            </div>\r\n            <div *ngIf=\"IsPracticeAreaType\">\r\n                <ft-practice-search [IsMapUpdateRequired]=\"updateMap\" [searchMode]=\"searchMode\" #practiceSearch></ft-practice-search>\r\n            </div>\r\n        </div>\r\n    </div>\r\n    <div [hidden]=\"!isBoundaryNotSupported\" id=\"boundryNotSupported\">\r\n        Maps are not available for this area type\r\n    </div>\r\n</div>"

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
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var legend_component_1 = __webpack_require__("./src/app/shared/component/legend/legend.component.ts");
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
        this.pageType = legend_component_1.PageType.Map;
        this.showRAG3 = false;
        this.showRAG5 = false;
        this.showBOB = false;
        this.showQuartiles = false;
        this.showQuintilesRAG = false;
        this.showQuintilesBOB = false;
        this.showContinuous = false;
        this.showAdhocKey = false;
        this.selectedAreaList = new Array();
        this.areaCodeSignificance = new Map();
        this.nationalArea = new Map();
        this.areaCodeColour = new Map();
        this.areaCodeColourValue = new Map();
        this.refreshColour = 0;
        this.htmlAdhocKey = '';
        this.IsPracticeAreaType = false;
        this.isBoundaryNotSupported = false;
        this.mapColourSelectedValue = 'benchmark';
        this.ftModel = this.ftHelperService.getFTModel();
        this.profileId = this.ftModel.profileId;
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
                // HOW can get here???
                this.searchModeNoDisplay = true;
            }
            newAreaTypeId = shared_1.AreaTypeIds.Practice;
        }
        else {
            // Not in search mode
            newAreaTypeId = this.ftModel.areaTypeId;
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
            var currentComparator = void 0;
            currentComparator = this.ftHelperService.getCurrentComparator();
            this.comparatorId = this.ftHelperService.getComparatorId();
            this.benchmarkIndex = this.comparatorId;
            this.indicatorService
                .getSingleIndicatorForAllArea(currentGrpRoot.Grouping[0].GroupId, this.ftModel.areaTypeId, currentComparator.Code, this.ftModel.profileId, this.comparatorId, currentGrpRoot.IID, currentGrpRoot.Sex.Id, currentGrpRoot.Age.Id)
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
            .sort(function (leftside, rightside) {
            if (leftside.Val < rightside.Val) {
                return -1;
            }
            if (leftside.Val > rightside.Val) {
                return 1;
            }
            return 0;
        })
            .reverse();
        var numAreas = 0;
        $.each(areaOrder, function (i, coreData) {
            if (coreData.ValF !== '-') {
                numAreas++;
            }
        });
        var j = 0;
        var sortedCoreData = this.sortedCoreData;
        var localAreaCodeColourValue = new Map();
        $.each(areaOrder, function (i, coreData) {
            var data = sortedCoreData[coreData.AreaCode];
            if (coreData.ValF === '-') {
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
                var targetLegend = this.ftHelperService.getTargetLegendHtml(this.comparisonConfig, indicatorMetadata);
                this.htmlAdhocKey =
                    '<div><table class="key-table" style="width: 85%;height:50px;"><tr><td class="key-text">Benchmarked against goal:</td><td class="key-spacer"></td><td>' +
                        targetLegend +
                        '</td></tr></table></div>';
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
    MapComponent.prototype.onSelectedAreaCodeChanged = function (eventDetail) {
    };
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
    MapComponent.prototype.OnBenchMarkIndexChanged = function (eventDetail) {
        this.benchmarkIndex = eventDetail.benchmarkIndex;
        if (this.benchmarkIndex === shared_1.ComparatorIds.National) {
            this.ftHelperService.setComparatorId(this.benchmarkIndex);
        }
        else {
            this.loadData();
        }
        this.ref.detectChanges();
    };
    MapComponent.prototype.resetLegends = function () {
        this.showRAG3 = false;
        this.showRAG5 = false;
        this.showBOB = false;
        this.showQuartiles = false;
        this.showQuintilesRAG = false;
        this.showQuintilesBOB = false;
        this.showContinuous = false;
    };
    MapComponent.prototype.onMapColourBoxChange = function (selectedColour) {
        // Reset legend flags
        this.resetLegends();
        switch (selectedColour) {
            case 'quartile': {
                this.showQuartiles = true;
                this.showAdhocKey = false;
                this.getQuartileColorData();
                break;
            }
            case 'quintile': {
                var root = this.ftHelperService.getCurrentGroupRoot();
                if (root.PolarityId === shared_1.PolarityIds.NotApplicable) {
                    this.showQuintilesBOB = true;
                }
                else {
                    this.showQuintilesRAG = true;
                }
                this.showAdhocKey = false;
                this.getQuintileColorData(root);
                break;
            }
            case 'continuous': {
                this.showContinuous = true;
                this.showAdhocKey = false;
                this.getContinuousColorData();
                break;
            }
            case 'benchmark': {
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
        switch (root.ComparatorMethodId) {
            case shared_1.ComparatorMethodIds.Quintiles:
                if (root.PolarityId === shared_1.PolarityIds.NotApplicable) {
                    // Quintile BOB
                    this.showQuintilesBOB = true;
                }
                else {
                    // Quintile RAG
                    this.showQuintilesRAG = true;
                }
                break;
            case shared_1.ComparatorMethodIds.SingleOverlappingCIsForOneCiLevel:
                this.showRAG3 = true;
                break;
            case shared_1.ComparatorMethodIds.SingleOverlappingCIsForTwoCiLevels:
                this.showRAG5 = true;
                break;
            default:
                this.setLegendDisplayByPolarity(root.PolarityId);
                break;
        }
    };
    MapComponent.prototype.setLegendDisplayByPolarity = function (polarityId) {
        switch (polarityId) {
            case shared_1.PolarityIds.BlueOrangeBlue:
                this.showBOB = true;
                break;
            case shared_1.PolarityIds.RAGHighIsGood:
                this.showRAG3 = true;
                break;
            case shared_1.PolarityIds.RAGLowIsGood:
                this.showRAG3 = true;
                break;
            default:
                break;
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
    MapComponent.prototype.getQuintileColorData = function (root) {
        var localAreaCodeColour = new Map();
        for (var _i = 0, _a = Array.from(this.areaCodeColourValue.keys()); _i < _a.length; _i++) {
            var key = _a[_i];
            var value = this.areaCodeColourValue.get(key);
            var colour = shared_1.Colour.getColorForQuintile(value.quintile, root.PolarityId);
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
    core_1.HostListener('window:MapSelected', [
        '$event',
        '$event.detail.searchMode'
    ]),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [Object, Object]),
    __metadata("design:returntype", void 0)
], MapComponent.prototype, "onOutsideEvent", null);
MapComponent = __decorate([
    core_1.Component({
        selector: 'ft-map',
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
var map_table_component_1 = __webpack_require__("./src/app/map/map-table/map-table.component.ts");
var practice_search_component_1 = __webpack_require__("./src/app/map/practice-search/practice-search.component.ts");
var ngx_bootstrap_1 = __webpack_require__("./node_modules/ngx-bootstrap/index.js");
var forms_1 = __webpack_require__("./node_modules/@angular/forms/@angular/forms.es5.js");
var coreDataHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/coreDataHelper.service.ts");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var legend_module_1 = __webpack_require__("./src/app/shared/component/legend/legend.module.ts");
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
            forms_1.FormsModule,
            legend_module_1.LegendModule
        ],
        declarations: [
            google_map_component_1.GoogleMapComponent,
            map_component_1.MapComponent,
            map_chart_component_1.MapChartComponent,
            map_table_component_1.MapTableComponent,
            practice_search_component_1.PracticeSearchComponent
        ],
        exports: [
            google_map_component_1.GoogleMapComponent,
            map_component_1.MapComponent
        ],
        providers: [
            googleMap_service_1.GoogleMapService,
            indicator_service_1.IndicatorService,
            area_service_1.AreaService,
            coreDataHelper_service_1.CoreDataHelperService,
            ftHelper_service_1.FTHelperService
        ]
    })
], MapModule);
exports.MapModule = MapModule;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/map.module.js.map

/***/ }),

/***/ "./src/app/map/practice-search-simple/practice-search-simple.component.css":
/***/ (function(module, exports) {

module.exports = ".glyphicon {\r\n    display: none;\r\n}\r\n\r\n.googleMapNg {\r\n    position: relative;\r\n    background-color: #fff;\r\n    border: 1px solid #CCC;\r\n    width: 636px;\r\n    height: 600px;\r\n    margin-bottom: 10px;\r\n    margin-top: 10px;\r\n    clear: both;\r\n    overflow: hidden;\r\n}\r\n\r\n#default_search_header {\r\n    width: 400px;\r\n    padding-top: 10px;\r\n    position: absolute;\r\n}\r\n\r\n#practice-list-info {\r\n    min-height: 30px;\r\n}\r\n\r\n#practice-list-info input {\r\n    max-width: 450px;\r\n}\r\n\r\n#practice-list-info h3 {\r\n    font-size: 1.4em;\r\n    margin-top: 10px;\r\n}\r\n\r\n.practice-header {\r\n    cursor: pointer;\r\n}\r\n\r\n.nearby-practices-map {\r\n    width: 636px;\r\n    height: 600px;\r\n    overflow: hidden;\r\n}\r\n\r\n.nearby-practices-map #google-map {\r\n    margin-bottom: 10px;\r\n}\r\n\r\n#table-container {\r\n    height: 600px;\r\n    overflow-y: scroll;\r\n}\r\n\r\n.nearby-practices-table {\r\n    float: left;\r\n    width: 45%;\r\n}\r\n\r\n.nearby-practices-table table {\r\n    width: 100%;\r\n    border-collapse: collapse;\r\n}\r\n\r\n.nearby-practices-table .header {\r\n    position: relative;\r\n    background-color: #02ae94;\r\n    padding: 10px;\r\n    color: #fff;\r\n}\r\n\r\n.nearby-practices-table td,\r\n.nearby-practices-table th {\r\n    padding: 0;\r\n    font-weight: normal;\r\n    text-align: left;\r\n    vertical-align: top;\r\n    font-size: 16px;\r\n}\r\n\r\n#show-all-practices {\r\n    padding: 23px 0 18px 0;\r\n}\r\n\r\n#show-all-practices a,\r\na:visited,\r\na:active,\r\na:hover {\r\n    color: #1d1dae;\r\n}\r\n\r\n#show-all-practices a,\r\na:visited,\r\na:active,\r\na:hover {\r\n    color: #2e3191;\r\n    text-decoration: underline;\r\n    font-size: 16px;\r\n}\r\n\r\n#show-all-practices a {\r\n    cursor: pointer;\r\n}\r\n\r\n.highlight-address {\r\n    background-color: #e6e6e6\r\n}\r\n\r\n.highlight-header {\r\n    background-color: #525252;\r\n    position: relative;\r\n    padding: 10px;\r\n    color: #fff;\r\n}\r\n\r\n.available-practices-list {\r\n    width: 636px;\r\n    height: 600px;\r\n    overflow-y: scroll;\r\n}\r\n\r\n.available-practices {\r\n    border-top: 1px solid #ccc;\r\n    margin: 0px;\r\n    padding-top: 5px;\r\n    padding-bottom: 5px;\r\n}\r\n\r\n.cursor-pointer {\r\n    cursor: pointer;\r\n}\r\n\r\n#practiceSearchText {\r\n    background: #FFF url(\"data:image/jpeg;base64,/9j/4AAQSkZJRgABAgAAZABkAAD/7AARRHVja3kAAQAEAAAAZAAA/+4ADkFkb2JlAGTAAAAAAf/bAIQAAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQICAgICAgICAgICAwMDAwMDAwMDAwEBAQEBAQECAQECAgIBAgIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMD/8AAEQgAGAAYAwERAAIRAQMRAf/EAFwAAQEBAQAAAAAAAAAAAAAAAAgHBAoBAQAAAAAAAAAAAAAAAAAAAAAQAAIDAQABBQADAQAAAAAAAAIDAQQFBgcAERITCCExQSIRAQAAAAAAAAAAAAAAAAAAAAD/2gAMAwEAAhEDEQA/AO93pOjxOQwdbp+k0UZODhUX6Wro2fslVSnWCTayQSDXtP8AwFrE2MOYERIpiJAi0/3L45ZYzb+rwnlrnOA2b45uT5Q3OOmrxNpxt+oLE313XMikRCc/JYtaIj7ksf8Ar4Az69hFtCLVV6bNWylditZrsByLCHALEvQ5ZEtqWrKCEhmRIZiYn29Adv1rwnTeSPAPectyCXXN56cjSq5aCkW668Tczte1mpj5hDbDq1IySufl9rgAIj3KJgCP5e/Y3h3rPz90fA4vN7Ydjr8m3mm+PbHOXaiOKfTqKW67at/QvOHP5Y60urfURNg66/sUqPl8QbP5t08jT8EeKSxt+p0lbP4bm8exo1GGYBoZOVVo36DQbA2K7s2ykkktogwYCPcY/r0Fu9BkZn0HFYNtKo07iCq2yZWSZWqxj8Dr2CIJl6CCPaQL3GY/j29BKPFPgzhfDNztLHCq1M+n22ynatYjtJz8LGYivKYq4GZ7AihWJjDOZmDbIyK/n9SlAAf/2Q==\") right center no-repeat;\r\n}\r\n\r\n.top-margin-10 {\r\n    margin-top: 10px;\r\n}\r\n\r\n.top-margin-20 {\r\n    margin-top: 20px;\r\n}\r\n\r\n#toggleMapHeading {\r\n    padding: 0px;\r\n}\r\n\r\n.clear-list-button {\r\n    padding: 0px;\r\n    font-size: 14px;\r\n    text-decoration: underline;\r\n    color: #2e3191;\r\n}\r\n\r\n.btn-link {\r\n    color: #2e3191;\r\n}"

/***/ }),

/***/ "./src/app/map/practice-search-simple/practice-search-simple.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"container\" style=\"margin-left:-15px;\" [formGroup]=\"practicesInCCGFormGroup\">\r\n  <div class=\"row\">\r\n    <div class=\"col-md-8\">\r\n      <h3>Show all practices in CCG</h3>\r\n      <div class=\"top-margin-10\">\r\n        <select id=\"practicesInCCG\" formControlName=\"practicesInCCG\" class=\"form-control\" (change)=\"onShowAllPracticeinCCGClick()\">\r\n          <option [value]=\"-1\">SELECT CCG</option>\r\n          <option *ngFor=\"let practice of practicesInCCG; let last=last\" [value]=\"practice.Code\">\r\n            {{practice.Name}} {{last? setDefaultOption() : ''}}\r\n          </option>\r\n        </select>\r\n        <div id=\"practiceInCCGName\"></div>\r\n      </div>\r\n      <div class=\"top-margin-20\">\r\n        <h3>Or search for practices near a postcode or place</h3>\r\n        <input [(ngModel)]=\"placeNameText\" [typeahead]=\"dataSource\" (typeaheadLoading)=\"changeTypeaheadLoading($event)\" (typeaheadNoResults)=\"changeTypeaheadNoResults($event)\"\r\n          (typeaheadOnSelect)=\"typeaheadOnSelect($event)\" typeaheadOptionsLimit=\"7\" typeaheadOptionField=\"displayName\" placeholder=\"\"\r\n          [typeaheadWaitMs]=\"200\" [typeaheadMinLength]=\"3\" class=\"form-control\" id=\"gp-practice-search-text\" [ngModelOptions]=\"{standalone: true}\"\r\n          autocomplete=\"off\">\r\n      </div>\r\n      <div *ngIf=\"typeaheadNoResults===true\" class=\"\">\r\n        No Results Found\r\n      </div>\r\n      <div class=\"top-margin-20\" *ngIf=\"nearByPractices.length > 0\">\r\n        <button id=\"toggleMapHeading\" class=\"btn btn-link clear-list-button\" (click)=\"toggleMap()\">Show practices as list</button>\r\n      </div>\r\n      <div id=\"nearby-practices-map\" class=\"nearby-practices-map\" [style.height]=\"height + 'px'\">\r\n        <div id=\"google-map\" #googleMapNew class=\"googleMapNg\">\r\n        </div>\r\n      </div>\r\n      <div id=\"nearby-practices-list\" class=\"available-practices-list\">\r\n        <div *ngFor=\"let practice of nearByPractices;\" #elem [attr.id]=\"practice.areaCode\" class=\"available-practices available-practices-{{practice.areaCode}}\"\r\n          (mouseover)=\"selectPractice('available-practices', practice.areaCode)\" (mouseout)=\"deselectPractice('available-practices', practice.areaCode)\"\r\n          (click)=\"movePractice('available-practices', practice.areaCode)\">\r\n          {{practice.areaName}}\r\n        </div>\r\n      </div>\r\n    </div>\r\n    <div class=\"col-md-4\">\r\n      <h3>\r\n        Practices in your list\r\n        <br />\r\n        <button class=\"btn btn-link clear-list-button\" (click)=\"clearList()\">Clear list</button>\r\n      </h3>\r\n      <div class=\"top-margin-10\" *ngIf=\"selectedPractices.length > 0\">\r\n        Select a practice below to remove\r\n        <br />it from your list\r\n        <div class=\"top-margin-20\">\r\n          <div *ngFor=\"let practice of selectedPractices;\" #elem [attr.id]=\"practice.areaName\" class=\"top-margin-10 selected-practices selected-practices-{{practice.Code}}\"\r\n            (mouseover)=\"selectPractice('selected-practices', practice.Code)\" (mouseout)=\"deselectPractice('selected-practices', practice.Code)\"\r\n            (click)=\"movePracticeOut('selected-practices', practice.Code)\">\r\n            <h6>\r\n              <b><span class=\"cursor-pointer\" (click)=\"removeFromList(practice.areaCode)\">{{practice.areaCode}} - {{practice.areaName}}</span></b>\r\n              <ng-template [ngIf]=\"practice?.addressLine1\">\r\n                <br />{{practice.addressLine1}}\r\n              </ng-template>\r\n              <ng-template [ngIf]=\"practice?.addressLine2\">\r\n                <br />{{practice.addressLine2}}\r\n              </ng-template>\r\n              <br />{{practice.postcode}}\r\n            </h6>\r\n          </div>\r\n        </div>\r\n      </div>\r\n    </div>\r\n  </div>"

/***/ }),

/***/ "./src/app/map/practice-search-simple/practice-search-simple.component.ts":
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
var arealist_service_1 = __webpack_require__("./src/app/shared/service/api/arealist.service.ts");
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var Observable_1 = __webpack_require__("./node_modules/rxjs/_esm5/Observable.js");
var practice_search_simple_1 = __webpack_require__("./src/app/map/practice-search-simple/practice-search-simple.ts");
var _ = __webpack_require__("./node_modules/underscore/underscore.js");
var forms_1 = __webpack_require__("./node_modules/@angular/forms/@angular/forms.es5.js");
var PracticeSearchSimpleComponent = (function () {
    function PracticeSearchSimpleComponent(ftHelperService, areaService, arealistService, ref) {
        var _this = this;
        this.ftHelperService = ftHelperService;
        this.areaService = areaService;
        this.arealistService = arealistService;
        this.ref = ref;
        this.IsMapUpdateRequired = false;
        this.searchMode = false;
        this.emitSelectedPractices = new core_1.EventEmitter();
        this.emitShowPracticesAsList = new core_1.EventEmitter();
        this.height = 0;
        this.isVisible = false;
        this.searchResults = [];
        this.nearByPractices = [];
        this.showCcgPractices = false;
        this.practicesInCCG = [];
        this.selectedPractices = [];
        this.firstTimeLoad = true;
        this.localSearchMode = false;
        this.displayCCGPracticeLink = true;
        this.dataSource = Observable_1.Observable.create(function (observer) {
            _this.areaService.getAreaSearchByText(_this.placeNameText, shared_1.AreaTypeIds.CcgPreApr2017, true, true)
                .subscribe(function (areaTextSearchResult) {
                _this.searchResults = areaTextSearchResult;
                var newResult = _.map(_this.searchResults, function (result) {
                    return new practice_search_simple_1.AutoCompleteResult(result.PolygonAreaCode, result.PlaceName, result.PolygonAreaName);
                });
                observer.next(newResult);
            });
        });
        this.practicesInCCGFormGroup = new forms_1.FormGroup({
            practicesInCCG: new forms_1.FormControl(),
            practiceSearchText: new forms_1.FormControl()
        });
    }
    PracticeSearchSimpleComponent.prototype.ngOnChanges = function (changes) {
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
        if (changes['selectedAreaAddresses']) {
            this.searchForPracticesInCCG();
        }
    };
    PracticeSearchSimpleComponent.prototype.ngOnInit = function () {
        $('#nearby-practices-list').hide();
        this.searchForPracticesInCCG();
        this.loadMap();
    };
    PracticeSearchSimpleComponent.prototype.setDefaultOption = function () {
        if (this.firstTimeLoad) {
            this.practicesInCCGFormGroup.get('practicesInCCG').setValue('-1');
            this.firstTimeLoad = false;
        }
    };
    PracticeSearchSimpleComponent.prototype.loadMap = function () {
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
        if (mapContainer !== null) {
            this.practiceMap = new google.maps.Map(mapContainer, mapOptions);
        }
        if (this.practiceMap) {
            var bounds = new google.maps.LatLngBounds();
            var position = new google.maps.LatLng(53.415649, -2.209015);
            bounds.extend(position);
            this.practiceMap.setCenter(bounds.getCenter());
            var thisCom_1 = this;
            // This is needed as on initial load map was not visible
            google.maps.event.addListener(this.practiceMap, 'idle', function () {
                var bound = thisCom_1.practiceMap.getCenter();
                google.maps.event.trigger(thisCom_1.practiceMap, 'resize');
                thisCom_1.practiceMap.setCenter(bound);
            });
        }
    };
    PracticeSearchSimpleComponent.prototype.changeTypeaheadLoading = function (e) {
        this.typeaheadLoading = e;
    };
    PracticeSearchSimpleComponent.prototype.changeTypeaheadNoResults = function (e) {
        this.typeaheadNoResults = e;
    };
    PracticeSearchSimpleComponent.prototype.typeaheadOnSelect = function (e) {
        var _this = this;
        var polygonAreaCode = e.item.polygonAreaCode;
        this.selectedArea = _.find(this.searchResults, function (obj) { return obj.PolygonAreaCode === polygonAreaCode; });
        this.areaService.getAreaSearchByProximity(this.selectedArea.Easting, this.selectedArea.Northing, shared_1.AreaTypeIds.Practice)
            .subscribe(function (result) {
            var nearByAreas = result;
            _this.nearByPractices = _.map(nearByAreas, function (area) {
                return new practice_search_simple_1.Practice(area.AreaName, area.AreaCode, area.AddressLine1, area.AddressLine2, area.Postcode, area.DistanceValF, area.LatLng);
            });
            _this.displayMarkersOnMap();
            _this.showCcgPractices = false;
        });
    };
    PracticeSearchSimpleComponent.prototype.onShowAllPracticeinCCGClick = function () {
        var _this = this;
        this.practiceSearchText = '';
        var practiceCode = this.practicesInCCGFormGroup.get('practicesInCCG').value;
        this.areaService.getAreaAddressesByParentAreaCode(practiceCode, shared_1.AreaTypeIds.Practice)
            .subscribe(function (result) {
            var areaAddresses = result;
            _this.nearByPractices = _.map(areaAddresses, function (area) {
                return new practice_search_simple_1.Practice(area.Name, area.Code, area.A1, area.A2, area.Postcode, '', area.Pos);
            });
            _this.displayMarkersOnMap();
            _this.showCcgPractices = true;
            _this.placeNameText = '';
        });
    };
    PracticeSearchSimpleComponent.prototype.displayMarkersOnMap = function () {
        var _this = this;
        var bounds = new google.maps.LatLngBounds();
        var infoWindow = new google.maps.InfoWindow({});
        var linkList = [];
        var iconUrl;
        var _loop_1 = function (i) {
            var position = new google.maps.LatLng(this_1.nearByPractices[i].lat, this_1.nearByPractices[i].lng);
            bounds.extend(position);
            var practiceInSelectedPractices = void 0;
            practiceInSelectedPractices = this_1.selectedPractices.find(function (x) { return x.areaCode === _this.nearByPractices[i].areaCode; });
            if (practiceInSelectedPractices !== null && practiceInSelectedPractices !== undefined) {
                iconUrl = 'http://maps.google.com/mapfiles/ms/icons/green-dot.png';
            }
            else {
                iconUrl = 'http://maps.google.com/mapfiles/ms/icons/ltblue-dot.png';
            }
            // Create marker
            var marker = new google.maps.Marker({
                position: position,
                map: this_1.practiceMap,
                icon: iconUrl
            });
            marker.set('marker_id', this_1.nearByPractices[i].areaCode);
            // Create pop up text
            var boxText = document.createElement('span');
            boxText.id = i.toString();
            boxText.className = 'select-area-link';
            boxText.style.cssText = 'font-size:16px;';
            boxText.innerHTML = this_1.nearByPractices[i].areaName;
            linkList.push(boxText);
            // Map marker click
            google.maps.event.addListener(marker, 'click', function (event) {
                var areaCode = marker.get('marker_id');
                infoWindow.setContent(linkList[i]);
                infoWindow.open(_this.practiceMap, marker);
                var selectedPracticeIndex = _.findIndex(_this.selectedPractices, function (x) { return x.areaCode === areaCode; });
                if (selectedPracticeIndex === -1) {
                    marker.setIcon('http://maps.google.com/mapfiles/ms/icons/green-dot.png');
                    var currentPracticeIndex = _.findIndex(_this.nearByPractices, function (x) { return x.areaCode === areaCode; });
                    var currentPractice = _this.nearByPractices[currentPracticeIndex];
                    _this.selectedPractices.push(currentPractice);
                    $('.available-practices-' + areaCode).addClass('bg-info text-white');
                }
                else {
                    marker.setIcon('http://maps.google.com/mapfiles/ms/icons/ltblue-dot.png');
                    _this.selectedPractices.splice(selectedPracticeIndex, 1);
                    $('.available-practices-' + areaCode).removeClass('bg-info text-white');
                }
                _this.emitSelectedPractices.emit(_this.selectedPractices);
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
    PracticeSearchSimpleComponent.prototype.fitMapToPracticeResults = function (bounds) {
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
    PracticeSearchSimpleComponent.prototype.onSelectPracticeClick = function (areaCode) {
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
    PracticeSearchSimpleComponent.prototype.searchForPracticesInCCG = function () {
        var _this = this;
        this.areaService.getAreas(152)
            .subscribe(function (result) {
            _this.practicesInCCG = result;
        });
        if (this.selectedAreaAddresses.length > 0) {
            this.selectedAreaAddresses.forEach(function (element) {
                var practice = new practice_search_simple_1.Practice(element.Name, element.Code, element.A1, element.A2, element.Postcode, '', element.Pos);
                _this.selectedPractices.push(practice);
            });
            // Emit selected practices to the parent
            this.emitSelectedPractices.emit(this.selectedPractices);
        }
    };
    PracticeSearchSimpleComponent.prototype.clearList = function () {
        if (this.selectedPractices.length > 0) {
            this.selectedPractices.forEach(function (element) {
                $('.available-practices-' + element.areaCode).removeClass('bg-primary bg-info text-white');
            });
            this.selectedPractices.length = 0;
            this.displayMarkersOnMap();
        }
    };
    PracticeSearchSimpleComponent.prototype.removeFromList = function (areaCode) {
        var areaCodeIndex = this.selectedPractices.findIndex(function (x) { return x.areaCode === areaCode; });
        this.selectedPractices.splice(areaCodeIndex, 1);
        this.displayMarkersOnMap();
    };
    PracticeSearchSimpleComponent.prototype.toggleMap = function () {
        $('#nearby-practices-list').toggle();
        $('#nearby-practices-map').toggle();
        if ($('#nearby-practices-list').is(':visible')) {
            $('#toggleMapHeading').html('Show practices on map');
        }
        else {
            $('#toggleMapHeading').html('Show practices as list');
        }
    };
    // Method to handle the mouse over event on the practice list
    PracticeSearchSimpleComponent.prototype.selectPractice = function (itemType, item) {
        if (!$('.' + itemType + '-' + item).hasClass('bg-info')) {
            $('.' + itemType + '-' + item).addClass('bg-primary text-white cursor-pointer');
        }
    };
    // Method to handle the mouse out event on the practice list
    PracticeSearchSimpleComponent.prototype.deselectPractice = function (itemType, item) {
        if (!$('.' + itemType + '-' + item).hasClass('bg-info')) {
            $('.' + itemType + '-' + item).removeClass('bg-primary text-white cursor-pointer');
        }
    };
    PracticeSearchSimpleComponent.prototype.movePractice = function (itemType, item) {
        var selectedPracticeIndex = _.findIndex(this.selectedPractices, function (x) { return x.areaCode === item; });
        if (selectedPracticeIndex === -1) {
            var currentPracticeIndex = _.findIndex(this.nearByPractices, function (x) { return x.areaCode === item; });
            var currentPractice = this.nearByPractices[currentPracticeIndex];
            this.selectedPractices.push(currentPractice);
            $('.' + itemType + '-' + item).addClass('bg-info text-white');
        }
        else {
            this.selectedPractices.splice(selectedPracticeIndex, 1);
            $('.' + itemType + '-' + item).removeClass('bg-info');
        }
        this.displayMarkersOnMap();
        this.emitSelectedPractices.emit(this.selectedPractices);
    };
    return PracticeSearchSimpleComponent;
}());
__decorate([
    core_1.ViewChild('scrollPracticeTable'),
    __metadata("design:type", typeof (_a = typeof core_1.ElementRef !== "undefined" && core_1.ElementRef) === "function" && _a || Object)
], PracticeSearchSimpleComponent.prototype, "practiceTable", void 0);
__decorate([
    core_1.ViewChild('googleMapNew'),
    __metadata("design:type", typeof (_b = typeof core_1.ElementRef !== "undefined" && core_1.ElementRef) === "function" && _b || Object)
], PracticeSearchSimpleComponent.prototype, "mapEl", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], PracticeSearchSimpleComponent.prototype, "IsMapUpdateRequired", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], PracticeSearchSimpleComponent.prototype, "searchMode", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Number)
], PracticeSearchSimpleComponent.prototype, "areaTypeId", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], PracticeSearchSimpleComponent.prototype, "selectedAreaAddresses", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], PracticeSearchSimpleComponent.prototype, "emitSelectedPractices", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], PracticeSearchSimpleComponent.prototype, "emitShowPracticesAsList", void 0);
PracticeSearchSimpleComponent = __decorate([
    core_1.Component({
        selector: 'ft-practice-search-simple',
        template: __webpack_require__("./src/app/map/practice-search-simple/practice-search-simple.component.html"),
        styles: [__webpack_require__("./src/app/map/practice-search-simple/practice-search-simple.component.css")],
        providers: [arealist_service_1.AreaListService]
    }),
    __metadata("design:paramtypes", [typeof (_c = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _c || Object, typeof (_d = typeof area_service_1.AreaService !== "undefined" && area_service_1.AreaService) === "function" && _d || Object, typeof (_e = typeof arealist_service_1.AreaListService !== "undefined" && arealist_service_1.AreaListService) === "function" && _e || Object, typeof (_f = typeof core_1.ChangeDetectorRef !== "undefined" && core_1.ChangeDetectorRef) === "function" && _f || Object])
], PracticeSearchSimpleComponent);
exports.PracticeSearchSimpleComponent = PracticeSearchSimpleComponent;
var _a, _b, _c, _d, _e, _f;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/practice-search-simple.component.js.map

/***/ }),

/***/ "./src/app/map/practice-search-simple/practice-search-simple.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var AutoCompleteResult = (function () {
    function AutoCompleteResult(polygonAreaCode, name, parentAreaName) {
        this.polygonAreaCode = polygonAreaCode;
        this.displayName = name + ', ' + parentAreaName.replace('NHS ', '');
    }
    return AutoCompleteResult;
}());
exports.AutoCompleteResult = AutoCompleteResult;
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
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/practice-search-simple.js.map

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
                    return new practice_search_1.AutoCompleteResult(result.PolygonAreaCode, result.PlaceName, result.PolygonAreaName);
                });
                observer.next(newResult);
            });
        });
    }
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
            // This is needed as on initial load map was not visible
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
            var boxText = document.createElement('a');
            boxText.id = i.toString();
            boxText.className = 'select-area-link';
            boxText.style.cssText = 'color: #2e3191; text-decoration: underline; font-size:16px;';
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
    __metadata("design:type", Object)
], PracticeSearchComponent.prototype, "IsMapUpdateRequired", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
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
var AutoCompleteResult = (function () {
    function AutoCompleteResult(polygonAreaCode, name, parentAreaName) {
        this.polygonAreaCode = polygonAreaCode;
        this.displayName = name + ", " + parentAreaName.replace('NHS ', '');
    }
    return AutoCompleteResult;
}());
exports.AutoCompleteResult = AutoCompleteResult;
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
var profile_service_1 = __webpack_require__("./src/app/shared/service/api/profile.service.ts");
var _ = __webpack_require__("./node_modules/underscore/underscore.js");
var common_1 = __webpack_require__("./node_modules/@angular/common/@angular/common.es5.js");
var MetadataTableComponent = (function () {
    function MetadataTableComponent(ftHelperService, indicatorService, profileService) {
        this.ftHelperService = ftHelperService;
        this.indicatorService = indicatorService;
        this.profileService = profileService;
        this.isReady = new core_1.EventEmitter();
        this.NotApplicable = 'n/a';
        this.showDataQuality = ftHelperService.getFTConfig().showDataQuality;
    }
    MetadataTableComponent.prototype.showInLightbox = function () {
        this.ftHelperService.showIndicatorMetadataInLightbox(this.table.nativeElement);
    };
    /**
     * For displaying metadata on the Definitions tab
     */
    MetadataTableComponent.prototype.displayMetadataForGroupRoot = function (root) {
        var _this = this;
        this.isReady.emit(false);
        // Get data by AJAX
        var metadataPropertiesObservable = this.indicatorService.getIndicatorMetadataProperties();
        var metadataObservable = this.indicatorService.getIndicatorMetadata(root.Grouping[0].GroupId);
        var indicatorProfilesObservable = this.profileService.getIndicatorProfiles([root.IID]);
        Observable_1.Observable.forkJoin([metadataPropertiesObservable, metadataObservable, indicatorProfilesObservable]).subscribe(function (results) {
            _this.metadataProperties = results[0];
            var metadataHash = results[1];
            _this.indicatorProfiles = results[2];
            var indicatorMetadata = metadataHash[root.IID];
            _this.displayMetadata(indicatorMetadata, root);
            _this.ftHelperService.showAndHidePageElements();
            _this.ftHelperService.unlock();
            _this.isReady.emit(true);
        });
    };
    /**
     * For displaying metadata in a pop up
     */
    MetadataTableComponent.prototype.displayMetadataForIndicator = function (indicatorId, restrictToProfileIds) {
        var _this = this;
        this.isReady.emit(false);
        // Get data by AJAX
        var metadataPropertiesObservable = this.indicatorService.getIndicatorMetadataProperties();
        var metadataObservable = this.indicatorService.getIndicatorMetadataByIndicatorId(indicatorId, restrictToProfileIds);
        Observable_1.Observable.forkJoin([metadataPropertiesObservable, metadataObservable]).subscribe(function (results) {
            _this.metadataProperties = results[0];
            var metadataHash = results[1];
            _this.indicatorProfiles = null;
            var indicatorMetadata = metadataHash[indicatorId];
            _this.displayMetadata(indicatorMetadata);
            _this.isReady.emit(true);
        });
    };
    MetadataTableComponent.prototype.displayMetadataForAjaxResults = function () {
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
        this.addIndicatorProfiles(indicatorMetadata.IID);
        // Trigger view refresh
        this.rows = this.tempRows;
    };
    MetadataTableComponent.prototype.addIndicatorProfiles = function (indicatorId) {
        var indicatorProfiles = this.indicatorProfiles;
        if (indicatorProfiles) {
            var profiles = [];
            for (var i = 0; i < indicatorProfiles[indicatorId].length; i++) {
                var profilePerIndicator = indicatorProfiles[indicatorId][i];
                profiles.push('<a href="' + profilePerIndicator.Url + '" target="_blank">' + profilePerIndicator.ProfileName + '</a>');
            }
            this.addMetadataRow('Profiles included in', profiles.join(', '));
        }
    };
    MetadataTableComponent.prototype.addMetadataRow = function (header, text) {
        this.tempRows.push(new MetadataRow(header, text));
    };
    MetadataTableComponent.prototype.addMetadataRowByProperty = function (textMetadata, property) {
        var columnName = property.ColumnName;
        if (textMetadata !== null && textMetadata.hasOwnProperty(columnName)) {
            var text = textMetadata[columnName];
            if (!_.isUndefined(text) && text !== '') {
                var displayText = void 0;
                if ((columnName === 'DataQuality') && this.showDataQuality) {
                    // Add data quality flags instead of number
                    var dataQualityCount = parseInt(text);
                    displayText = this.ftHelperService.getIndicatorDataQualityHtml(text) + ' ' +
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
    __metadata("design:paramtypes", [typeof (_b = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _b || Object, typeof (_c = typeof indicator_service_1.IndicatorService !== "undefined" && indicator_service_1.IndicatorService) === "function" && _c || Object, typeof (_d = typeof profile_service_1.ProfileService !== "undefined" && profile_service_1.ProfileService) === "function" && _d || Object])
], MetadataTableComponent);
exports.MetadataTableComponent = MetadataTableComponent;
var MetadataRow = (function () {
    function MetadataRow(header, text) {
        this.header = header;
        this.text = text;
    }
    return MetadataRow;
}());
var _a, _b, _c, _d;
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

module.exports = "<div style=\"float: left;width: 600px;\">\r\n    <div class=\"export-chart-box\">\r\n        <button class=\"export-link\" (click)=\"exportChart()\">Export chart as image</button>\r\n    </div>\r\n    <div class=\"export-chart-box-csv\">\r\n        <button class=\"export-link-csv hidden\" (click)=\"exportChartAsCsv()\">Export chart as CSV file</button>\r\n    </div>\r\n</div>\r\n<div id=\"population-chart\" #chart>\r\n</div>"

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
        var subtitle = nationalPopulation.IndicatorName + ' ' + nationalPopulation.Period;
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
        var isParentNotUk = model.parentCode.toUpperCase() !== shared_1.AreaCodes.Uk;
        if (isParentNotEngland && isParentNotUk) {
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
        if (isParentNotUk) {
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
        }
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
                    marker: shared_1.HC.NoLineMarker,
                    animation: false,
                    states: {
                        hover: {
                            marker: shared_1.HC.NoLineMarker
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
            credits: shared_1.HC.Credits,
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
    PopulationChartComponent.prototype.exportChartAsCsv = function () {
        alert("It works!");
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

module.exports = ":host /deep/ #population-info,\r\n#registered-persons {\r\n    width: 355px;\r\n    float: left;\r\n    margin-top: 6px;\r\n}\r\n\r\n:host /deep/ #population-info .bordered-table {\r\n    margin-bottom: 15px;\r\n    width: 100%;\r\n    float: left;\r\n}\r\n\r\n:host /deep/ #registered-persons td.header,\r\n#further-info-table td.header {\r\n    width: 220px;\r\n    background: #fafafa;\r\n    padding: 3px 3px 3px 0;\r\n    vertical-align: top;\r\n    text-align: right;\r\n}\r\n\r\n:host /deep/ #registered-persons tr td:last-child,\r\n#further-info-table tr td:last-child {\r\n    padding-left: 3px;\r\n}\r\n\r\n#population-info #decile-key {\r\n    width: 100%;\r\n}\r\n\r\n.practice-label {\r\n    width: 100%;\r\n    float: left;\r\n    text-align: center;\r\n    font-size: 14px;\r\n}\r\n\r\n#decile-key td {\r\n    height: 9px;\r\n    width: 30px;\r\n    border: 1px solid white;\r\n    border-top: 2px solid white;\r\n    border-bottom: none;\r\n}\r\n\r\n#deprivation-table,\r\n#ethnicity-table {\r\n    margin-top: 15px;\r\n}\r\n\r\n#deprivation {\r\n    text-align: center;\r\n}\r\n\r\n.deprivation-label {\r\n    font-size: 10px;\r\n    line-height: 10px;\r\n    color: #666;\r\n    margin-bottom: 3px;\r\n}"

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
        this.ftModel = this.ftHelperService.getFTModel();
    }
    PopulationSummaryComponent.prototype.ngOnChanges = function (changes) {
        if (this.allPopulationData) {
            var areaName = this.ftHelperService.getAreaName(this.ftModel.areaCode);
            this.defineRegisteredPersons(areaName);
            this.practiceLabel = this.ftModel.areaCode + ' - ' + areaName;
            this.defineAdHocIndicatorRows();
            this.defineDeprivationDecile();
            this.defineEthnicityLabel();
        }
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
    PopulationSummaryComponent.prototype.shouldDisplay = function () {
        return this.ftModel.areaTypeId === shared_1.AreaTypeIds.Practice;
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
        rows.push(new AdHocIndicatorRow(shared_1.IndicatorIds.WouldRecommendPractice, '% having a positive experience of their practice', text));
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

module.exports = "<div id=\"registered-persons\">\r\n    <div class=\"population-heading\">\r\n        <div class=\"right-tooltip-icon info-tooltip\" (click)=\"showMetadata()\">\r\n        </div>\r\n        <div>Registered Persons</div>\r\n    </div>\r\n    <table class=\"bordered-table\" cellspacing=\"0\">\r\n        <tbody>\r\n            <tr *ngFor=\"let row of registeredPersons\">\r\n                <td class=\"header\">\r\n                    {{row.areaName}}\r\n                </td>\r\n                <td>{{row.personCount}}\r\n                    <div *ngIf=\"!row.hasPersonCount()\" class=\"no-data\">-</div>\r\n                    <span *ngIf=\"row.isAverage && row.hasPersonCount()\" class=\"average\">(average)</span>\r\n                </td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n</div>"

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

/***/ "./src/app/reports/reports.component.css":
/***/ (function(module, exports) {

module.exports = ".btn-link {\r\n    color: #2e3191;\r\n}\r\n\r\ntable tr td {\r\n    font-size: 17px;\r\n    padding-left: 3px;\r\n}\r\n\r\ntable tr th {\r\n    font-size: 17px;\r\n    padding-left: 3px;\r\n}\r\n\r\ntable tr td.format {\r\n    width: 90px;\r\n    text-align: center;\r\n}\r\n\r\ntable tr td:first-child {\r\n    width: 300px;\r\n}"

/***/ }),

/***/ "./src/app/reports/reports.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"reports-container\" style=\"display:none;\">\r\n    <div *ngIf=\"hasReports\">\r\n        <h2>Related reports</h2>\r\n        <table class=\"bordered-table table-hover\">\r\n            <thead>\r\n                <tr>\r\n                    <th>Name</th>\r\n                    <th style=\"width:400px;\">Notes</th>\r\n                    <th colspan=\"3\" style=\"text-align:center;width:400px;\">Download</th>\r\n                </tr>\r\n            </thead>\r\n            <tbody>\r\n                <tr *ngFor=\"let report of reports\">\r\n                    <td>{{report.Name}}</td>\r\n                    <td>{{report.Notes}}</td>\r\n                    <td class=\"format\">\r\n                        <button class=\"btn btn-link\" (click)=\"openReport(report, 'pdf')\">PDF</button>\r\n                    </td>\r\n                    <td class=\"format\">\r\n                        <button class=\"btn btn-link\" (click)=\"openReport(report, 'word')\">Word</button>\r\n                    </td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n        <br><br>\r\n        <div>Please note reports open in a separate tab or window. Reports may take a minute or two to load.</div>\r\n        <br>\r\n        <div [innerHtml]=\"extraNotes\"></div>\r\n    </div>\r\n\r\n    <div *ngIf=\"isInitialised && !hasReports\">There are no reports available for this profile</div>\r\n</div>"

/***/ }),

/***/ "./src/app/reports/reports.component.ts":
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
var ssrs_report_service_1 = __webpack_require__("./src/app/shared/service/api/ssrs-report.service.ts");
var content_service_1 = __webpack_require__("./src/app/shared/service/api/content.service.ts");
var ReportsComponent = (function () {
    function ReportsComponent(ftHelperService, ssrsReportService, contentService) {
        this.ftHelperService = ftHelperService;
        this.ssrsReportService = ssrsReportService;
        this.contentService = contentService;
        this.isInitialised = false;
        this.hasReports = false;
        this.reports = [];
    }
    ReportsComponent.prototype.onOutsideEvent = function (event) {
        var _this = this;
        this.model = this.ftHelperService.getFTModel();
        var profileId = this.model.profileId;
        // Get data
        var reportsObservable = this.ssrsReportService.getSsrsReports(profileId);
        var contentObservable = this.contentService.getContent(profileId, 'ssrs-report-extra-notes');
        Observable_1.Observable.forkJoin([reportsObservable, contentObservable]).subscribe(function (results) {
            _this.reports = results[0];
            _this.extraNotes = results[1];
            _this.hasReports = _this.reports.length > 0;
            _this.isInitialised = true;
            _this.ftHelperService.showAndHidePageElements();
            _this.ftHelperService.unlock();
        });
    };
    ReportsComponent.prototype.openReport = function (report, format) {
        var reportUrl = this.getReportUrl(report, format);
        window.open(reportUrl, '_blank');
        this.ftHelperService.logEvent('SsrsReportView', format, report.Name);
    };
    ReportsComponent.prototype.getReportUrl = function (report, format) {
        var parameters = report.Parameters;
        var model = this.model;
        var url = ["/reports/ssrs/?reportName=", encodeURIComponent(report.File)];
        if (parameters.includes('areaCode')) {
            url.push("&areaCode=" + model.areaCode);
        }
        if (parameters.includes('areaTypeId')) {
            url.push("&areaTypeId=" + model.areaTypeId);
        }
        if (parameters.includes('parentCode')) {
            url.push("&parentCode=" + model.parentCode);
        }
        if (parameters.includes('parentTypeId')) {
            url.push("&parentTypeId=" + model.parentTypeId);
        }
        if (parameters.includes('groupId')) {
            url.push("&groupId=" + model.groupId);
        }
        url.push("&format=" + format);
        return url.join('');
    };
    return ReportsComponent;
}());
__decorate([
    core_1.HostListener('window:ReportsSelected', ['$event']),
    __metadata("design:type", Function),
    __metadata("design:paramtypes", [Object]),
    __metadata("design:returntype", void 0)
], ReportsComponent.prototype, "onOutsideEvent", null);
ReportsComponent = __decorate([
    core_1.Component({
        selector: 'ft-reports',
        template: __webpack_require__("./src/app/reports/reports.component.html"),
        styles: [__webpack_require__("./src/app/reports/reports.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _a || Object, typeof (_b = typeof ssrs_report_service_1.SsrsReportService !== "undefined" && ssrs_report_service_1.SsrsReportService) === "function" && _b || Object, typeof (_c = typeof content_service_1.ContentService !== "undefined" && content_service_1.ContentService) === "function" && _c || Object])
], ReportsComponent);
exports.ReportsComponent = ReportsComponent;
var _a, _b, _c;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/reports.component.js.map

/***/ }),

/***/ "./src/app/shared/component/arealist/arealist-areas/arealist-areas.component.css":
/***/ (function(module, exports) {

module.exports = ".areas {\r\n    border-top: 1px solid #ccc;\r\n    margin: 0px;\r\n    padding-top: 5px;\r\n    padding-bottom: 5px;\r\n}\r\n\r\n.top-margin {\r\n    margin-top: 10px;\r\n}\r\n\r\n.areas-list {\r\n    height: 400px;\r\n    overflow-y: scroll;\r\n}\r\n"

/***/ }),

/***/ "./src/app/shared/component/arealist/arealist-areas/arealist-areas.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"areas-list\" class=\"areas-list top-margin\">\r\n  <div *ngFor=\"let area of areas\" #elem [attr.id]=\"area.Code\" class=\"areas areas-{{area.Code}}\" (mouseover)=\"mouseOverArea(area)\"\r\n    (mouseout)=\"mouseOutArea(area)\" (click)=\"selectArea(area.Code)\">\r\n    {{area.Name}}\r\n  </div>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/component/arealist/arealist-areas/arealist-areas.component.ts":
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
var area_service_1 = __webpack_require__("./src/app/shared/service/api/area.service.ts");
var arealist_service_1 = __webpack_require__("./src/app/shared/service/api/arealist.service.ts");
var ArealistAreasComponent = (function () {
    function ArealistAreasComponent(arealistService, areaService) {
        this.arealistService = arealistService;
        this.areaService = areaService;
        this.emitSelectedArea = new core_1.EventEmitter();
        this.emitDeSelectedArea = new core_1.EventEmitter();
        this.emitMouseOverArea = new core_1.EventEmitter();
        this.emitMouseOutArea = new core_1.EventEmitter();
        this.emitAreas = new core_1.EventEmitter();
        this.areas = [];
    }
    ArealistAreasComponent.prototype.ngOnChanges = function (changes) {
        if (changes['areaTypeId']) {
            if (this.areaTypeId) {
                this.searchForAreas();
            }
        }
    };
    ArealistAreasComponent.prototype.searchForAreas = function () {
        var _this = this;
        this.areaService.getAreas(this.areaTypeId)
            .subscribe(function (result) {
            _this.areas = result;
            _this.emitAreas.emit(_this.areas);
        });
    };
    ArealistAreasComponent.prototype.mouseOverArea = function (area) {
        this.emitMouseOverArea.emit(area);
    };
    ArealistAreasComponent.prototype.mouseOutArea = function (area) {
        this.emitMouseOutArea.emit(area);
    };
    ArealistAreasComponent.prototype.selectArea = function (item) {
        var area = this.areas.find(function (x) { return x.Code === item; });
        var areaInSelectedAreas = this.selectedAreas.find(function (x) { return x.Code === item; });
        if (areaInSelectedAreas === undefined) {
            this.emitSelectedArea.emit(area);
        }
        else {
            this.emitDeSelectedArea.emit(area);
        }
    };
    return ArealistAreasComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], ArealistAreasComponent.prototype, "areaTypeId", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], ArealistAreasComponent.prototype, "selectedAreas", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], ArealistAreasComponent.prototype, "emitSelectedArea", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], ArealistAreasComponent.prototype, "emitDeSelectedArea", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], ArealistAreasComponent.prototype, "emitMouseOverArea", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], ArealistAreasComponent.prototype, "emitMouseOutArea", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], ArealistAreasComponent.prototype, "emitAreas", void 0);
ArealistAreasComponent = __decorate([
    core_1.Component({
        selector: 'ft-arealist-areas',
        template: __webpack_require__("./src/app/shared/component/arealist/arealist-areas/arealist-areas.component.html"),
        styles: [__webpack_require__("./src/app/shared/component/arealist/arealist-areas/arealist-areas.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof arealist_service_1.AreaListService !== "undefined" && arealist_service_1.AreaListService) === "function" && _a || Object, typeof (_b = typeof area_service_1.AreaService !== "undefined" && area_service_1.AreaService) === "function" && _b || Object])
], ArealistAreasComponent);
exports.ArealistAreasComponent = ArealistAreasComponent;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/arealist-areas.component.js.map

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

/***/ "./src/app/shared/component/legend/legend-area-profiles/legend-area-profiles.component.css":
/***/ (function(module, exports) {

module.exports = ".left {\r\n    float: left;\r\n}\r\n\r\n.spine-rag-3,\r\n.spine-rag-5,\r\n.spine-bob,\r\n.spine-not-compared\r\n {\r\n    float: left;\r\n    margin-left: 5px;\r\n}\r\n\r\n.margin-top-10 {\r\n    margin-top: 10px !important;\r\n    padding-top: 0 !important;\r\n}\r\n\r\n.key-table {\r\n    color: #000 !important;\r\n}\r\n"

/***/ }),

/***/ "./src/app/shared/component/legend/legend-area-profiles/legend-area-profiles.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"row\" *ngIf=\"showRAG3 || showRAG5 || showBOB\">\r\n  <div class=\"col-md-12 spine-table key-table\">\r\n    <div class=\"spine-compared-with-text\">\r\n      <i>Compared with benchmark:</i>\r\n    </div>\r\n    <div class=\"key-spine spine-rag-5\" *ngIf=\"showRAG5\">\r\n      <img class=\"key-circle\" src=\"/images/circle_dark_green_mini.png\" alt=\"\" /> Better 99.8%\r\n      <img class=\"key-circle\" src=\"/images/circle_green_mini.png\" alt=\"\" /> Better 95%\r\n      <img class=\"key-circle\" src=\"/images/circle_orange_mini.png\" alt=\"\" /> Similar\r\n      <img class=\"key-circle\" src=\"/images/circle_red_mini.png\" alt=\"\" /> Worse 95%\r\n      <img class=\"key-circle\" src=\"/images/circle_dark_red_mini.png\" alt=\"\" /> Worse 99.8%\r\n    </div>\r\n    <div class=\"key-spine spine-rag-3\" *ngIf=\"showRAG3\">\r\n      <img class=\"key-circle\" src=\"/images/circle_green_mini.png\" alt=\"\" /> Better\r\n      <img class=\"key-circle\" src=\"/images/circle_orange_mini.png\" alt=\"\" /> Similar\r\n      <img class=\"key-circle\" src=\"/images/circle_red_mini.png\" alt=\"\" /> Worse\r\n    </div>\r\n    <div class=\"key-spine spine-bob\" *ngIf=\"showBOB\">\r\n      <img class=\"key-circle\" src=\"/images/circle_darkblue_mini.png\" alt=\"\" /> Lower\r\n      <img class=\"key-circle\" src=\"/images/circle_orange_mini.png\" alt=\"\" /> Similar\r\n      <img class=\"key-circle\" src=\"/images/circle_lightblue_mini.png\" alt=\"\" /> Higher\r\n    </div>\r\n    <div class=\"key-spine spine-not-compared\">\r\n      <img class=\"key-circle\" src=\"/images/circle_white_mini.png\" alt=\"\" /> Not compared\r\n    </div>\r\n  </div>\r\n</div>\r\n<div class=\"row margin-top-10\" *ngIf=\"showQuintilesRAG || showQuintilesBOB\">\r\n  <div class=\"col-md-12\">\r\n    <span class=\"spine-compared-with-text\">\r\n      <i>Quintiles:</i>\r\n    </span>\r\n    <div class=\"spine-chart-legend-outer-box\" *ngIf=\"showQuintilesRAG\">\r\n      Best\r\n      <img class=\"key-circle\" src=\"/images/circle_rag_quintile1_mini.png\" alt=\"\" />&nbsp;\r\n      <img class=\"key-circle\" src=\"/images/circle_rag_quintile2_mini.png\" alt=\"\" />&nbsp;\r\n      <img class=\"key-circle\" src=\"/images/circle_rag_quintile3_mini.png\" alt=\"\" />&nbsp;\r\n      <img class=\"key-circle\" src=\"/images/circle_rag_quintile4_mini.png\" alt=\"\" />&nbsp;\r\n      <img class=\"key-circle\" src=\"/images/circle_rag_quintile5_mini.png\" alt=\"\" />&nbsp;\r\n      Worst\r\n    </div>\r\n    <div class=\"spine-chart-legend-outer-box\" *ngIf=\"showQuintilesBOB\">\r\n      Low\r\n      <img class=\"key-circle\" src=\"/images/circle_bob_quintile1_mini.png\" alt=\"\" />&nbsp;\r\n      <img class=\"key-circle\" src=\"/images/circle_bob_quintile2_mini.png\" alt=\"\" />&nbsp;\r\n      <img class=\"key-circle\" src=\"/images/circle_bob_quintile3_mini.png\" alt=\"\" />&nbsp;\r\n      <img class=\"key-circle\" src=\"/images/circle_bob_quintile4_mini.png\" alt=\"\" />&nbsp;\r\n      <img class=\"key-circle\" src=\"/images/circle_bob_quintile5_mini.png\" alt=\"\" />&nbsp;\r\n      High\r\n    </div>\r\n    <div class=\"spine-chart-legend-outer-box\">\r\n      <img class=\"key-circle\" src=\"/images/circle_white_mini.png\" alt=\"\" /> Not applicable\r\n    </div>\r\n  </div>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/component/legend/legend-area-profiles/legend-area-profiles.component.ts":
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
var LegendAreaProfilesComponent = (function () {
    function LegendAreaProfilesComponent() {
        this.showRAG3 = null;
        this.showRAG5 = null;
        this.showBOB = null;
        this.showQuintilesRAG = null;
        this.showQuintilesBOB = null;
    }
    LegendAreaProfilesComponent.prototype.ngOnChanges = function (changes) {
        if (changes['showRAG3']) {
            if (this.showRAG3) {
            }
        }
        if (changes['showRAG5']) {
            if (this.showRAG5) {
            }
        }
        if (changes['showBOB']) {
            if (this.showBOB) {
            }
        }
        if (changes['showQuintilesRAG']) {
            if (this.showQuintilesRAG) {
            }
        }
        if (changes['showQuintilesBOB']) {
            if (this.showQuintilesBOB) {
            }
        }
    };
    return LegendAreaProfilesComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendAreaProfilesComponent.prototype, "showRAG3", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendAreaProfilesComponent.prototype, "showRAG5", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendAreaProfilesComponent.prototype, "showBOB", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendAreaProfilesComponent.prototype, "showQuintilesRAG", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendAreaProfilesComponent.prototype, "showQuintilesBOB", void 0);
LegendAreaProfilesComponent = __decorate([
    core_1.Component({
        selector: 'ft-legend-area-profiles',
        template: __webpack_require__("./src/app/shared/component/legend/legend-area-profiles/legend-area-profiles.component.html"),
        styles: [__webpack_require__("./src/app/shared/component/legend/legend-area-profiles/legend-area-profiles.component.css")]
    }),
    __metadata("design:paramtypes", [])
], LegendAreaProfilesComponent);
exports.LegendAreaProfilesComponent = LegendAreaProfilesComponent;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/legend-area-profiles.component.js.map

/***/ }),

/***/ "./src/app/shared/component/legend/legend-bob/legend-bob.component.css":
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/shared/component/legend/legend-bob/legend-bob.component.html":
/***/ (function(module, exports) {

module.exports = "<div *ngIf=\"showBob\">\r\n  <table class=\"key-table\" cellspacing=\"0\">\r\n    <tr>\r\n      <td class=\"key-text key-label\">Compared with benchmark</td>\r\n      <td class=\"key-spacer\"></td>\r\n      <td class=\"bobLower key\">\r\n        <div class=\"tartan-box key-box\">\r\n          <img class=\"tartan-fill print-only\" src=\"/images/bobLower.png\" alt=\"\">\r\n          <div class=\"tartan-text\">\r\n            Lower\r\n          </div>\r\n        </div>\r\n      </td>\r\n      <td class=\"same key\">\r\n        <div class=\"tartan-box key-box\">\r\n          <img class=\"tartan-fill print-only\" src=\"/images/same.png\" alt=\"\">\r\n          <div class=\"tartan-text\">\r\n            Similar\r\n          </div>\r\n        </div>\r\n      </td>\r\n      <td class=\"bobHigher key\">\r\n        <div class=\"tartan-box key-box\">\r\n          <img class=\"tartan-fill print-only\" src=\"/images/bobHigher.png\" alt=\"\">\r\n          <div class=\"tartan-text\">\r\n            Higher\r\n          </div>\r\n        </div>\r\n      </td>\r\n    </tr>\r\n  </table>\r\n  <table id=\"not-compared\" class=\"key-table\" cellspacing=\"0\" *ngIf=\"showBenchmark\">\r\n    <tr>\r\n      <td id=\"map-key-part2\" class=\"none key key-box-right\" style=\"display: table-cell;\">\r\n        <div class=\"tartan-box key-box\">\r\n          Not compared\r\n        </div>\r\n      </td>\r\n    </tr>\r\n  </table>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/component/legend/legend-bob/legend-bob.component.ts":
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
var legend_component_1 = __webpack_require__("./src/app/shared/component/legend/legend.component.ts");
var LegendBobComponent = (function () {
    function LegendBobComponent() {
        this.legendType = null;
        this.keyType = null;
        this.showBob = false;
        this.showBenchmark = true;
    }
    LegendBobComponent.prototype.ngOnChanges = function (changes) {
        if (changes['legendType']) {
            if (this.legendType) {
                if (this.legendType === legend_component_1.LegendType.BOB) {
                    this.showBob = true;
                }
                else {
                    this.showBob = false;
                }
            }
        }
    };
    return LegendBobComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_a = typeof legend_component_1.LegendType !== "undefined" && legend_component_1.LegendType) === "function" && _a || Object)
], LegendBobComponent.prototype, "legendType", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_b = typeof legend_component_1.KeyType !== "undefined" && legend_component_1.KeyType) === "function" && _b || Object)
], LegendBobComponent.prototype, "keyType", void 0);
LegendBobComponent = __decorate([
    core_1.Component({
        selector: 'ft-legend-bob',
        template: __webpack_require__("./src/app/shared/component/legend/legend-bob/legend-bob.component.html"),
        styles: [__webpack_require__("./src/app/shared/component/legend/legend-bob/legend-bob.component.css")]
    }),
    __metadata("design:paramtypes", [])
], LegendBobComponent);
exports.LegendBobComponent = LegendBobComponent;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/legend-bob.component.js.map

/***/ }),

/***/ "./src/app/shared/component/legend/legend-continuous/legend-continuous.component.css":
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/shared/component/legend/legend-continuous/legend-continuous.component.html":
/***/ (function(module, exports) {

module.exports = "<table class=\"key-table custom-key-table\" cellspacing=\"0\" *ngIf=\"showContinuous\">\r\n  <tr>\r\n    <td class=\"key-text\">Continuous:</td>\r\n    <td style=\"background-color: #FFE97F;\">Lowest</td>\r\n    <td class=\"continual_range whiteText\">&nbsp;</td>\r\n    <td style=\"background-color: #151C55;\" class=\"whiteText\">Highest</td>\r\n  </tr>\r\n</table>"

/***/ }),

/***/ "./src/app/shared/component/legend/legend-continuous/legend-continuous.component.ts":
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
var legend_component_1 = __webpack_require__("./src/app/shared/component/legend/legend.component.ts");
var LegendContinuousComponent = (function () {
    function LegendContinuousComponent() {
        this.legendType = null;
        this.keyType = null;
        this.showContinuous = false;
    }
    LegendContinuousComponent.prototype.ngOnChanges = function (changes) {
        if (changes['legendType']) {
            if (this.legendType) {
                if (this.legendType === legend_component_1.LegendType.Continuous) {
                    this.showContinuous = true;
                }
                else {
                    this.showContinuous = false;
                }
            }
        }
    };
    return LegendContinuousComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_a = typeof legend_component_1.LegendType !== "undefined" && legend_component_1.LegendType) === "function" && _a || Object)
], LegendContinuousComponent.prototype, "legendType", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_b = typeof legend_component_1.KeyType !== "undefined" && legend_component_1.KeyType) === "function" && _b || Object)
], LegendContinuousComponent.prototype, "keyType", void 0);
LegendContinuousComponent = __decorate([
    core_1.Component({
        selector: 'ft-legend-continuous',
        template: __webpack_require__("./src/app/shared/component/legend/legend-continuous/legend-continuous.component.html"),
        styles: [__webpack_require__("./src/app/shared/component/legend/legend-continuous/legend-continuous.component.css")]
    }),
    __metadata("design:paramtypes", [])
], LegendContinuousComponent);
exports.LegendContinuousComponent = LegendContinuousComponent;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/legend-continuous.component.js.map

/***/ }),

/***/ "./src/app/shared/component/legend/legend-map/legend-map.component.css":
/***/ (function(module, exports) {

module.exports = ".compared-with-text {\r\n    float: left;\r\n    font-size: 11px;\r\n    margin-right: 5px;\r\n}\r\n\r\n.map-legend-box {\r\n    float: left;\r\n    font-size: 11px;\r\n}\r\n\r\n.tartan-legend-box {\r\n    float: left;\r\n    margin-left: 2px;\r\n    padding: 2px 0px 2px 0px;\r\n    width: 70px;\r\n    text-align: center;\r\n}\r\n\r\n.not-compared {\r\n    background-color: #fff;\r\n    border: 1px solid #ccc;\r\n    width: 80px;\r\n    height: 19px;\r\n}\r\n\r\n.left-margin {\r\n    margin-left: 18px;\r\n}\r\n"

/***/ }),

/***/ "./src/app/shared/component/legend/legend-map/legend-map.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"row\" *ngIf=\"showTartanLegends\">\r\n    <div class=\"col-md-12\">\r\n        <div class=\"compared-with-text\">\r\n            <i>Compared with benchmark:</i>\r\n        </div>\r\n        <div class=\"map-legend-box\" *ngIf=\"showRAG5\">\r\n            <div class=\"tartan-legend-box better-99\">\r\n                Better 99.8%\r\n            </div>\r\n            <div class=\"tartan-legend-box better-95\">\r\n                Better 95%\r\n            </div>\r\n            <div class=\"tartan-legend-box similar\">\r\n                Similar\r\n            </div>\r\n            <div class=\"tartan-legend-box worse-95\">\r\n                Worse 95%\r\n            </div>\r\n            <div class=\"tartan-legend-box worse-99\">\r\n                Worse 99.8%\r\n            </div>\r\n        </div>\r\n        <div class=\"map-legend-box\" *ngIf=\"showRAG3\">\r\n            <div class=\"tartan-legend-box better-95\">\r\n                Better\r\n            </div>\r\n            <div class=\"tartan-legend-box similar\">\r\n                Similar\r\n            </div>\r\n            <div class=\"tartan-legend-box worse-95\">\r\n                Worse\r\n            </div>\r\n        </div>\r\n        <div class=\"map-legend-box left-margin\" *ngIf=\"showBOB\">\r\n            <div class=\"tartan-legend-box bob-lower\">\r\n                Lower\r\n            </div>\r\n            <div class=\"tartan-legend-box bob-similar\">\r\n                Similar\r\n            </div>\r\n            <div class=\"tartan-legend-box bob-higher\">\r\n                Higher\r\n            </div>\r\n        </div>\r\n        <div class=\"map-legend-box left-margin\">\r\n            <div class=\"tartan-legend-box not-compared\">\r\n                Not Compared\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n<div class=\"row\" *ngIf=\"showQuartiles\">\r\n    <div class=\"col-md-12\">\r\n        <div class=\"compared-with-text\">\r\n            <i>Quartiles:</i>\r\n        </div>\r\n        <div class=\"map-legend-box left-margin\">\r\n            <div class=\"tartan-legend-box quartiles-low\">\r\n                Low\r\n            </div>\r\n            <div class=\"tartan-legend-box quartiles-medium-1\">\r\n                &nbsp;\r\n            </div>\r\n            <div class=\"tartan-legend-box quartiles-medium-2\">\r\n                &nbsp;\r\n            </div>\r\n            <div class=\"tartan-legend-box quartiles-high\">\r\n                High\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n<div class=\"row\" *ngIf=\"showQuintilesRAG\">\r\n    <div class=\"col-md-12\">\r\n        <div class=\"compared-with-text\">\r\n            <i>Quintiles:</i>\r\n        </div>\r\n        <div class=\"map-legend-box left-margin\">\r\n            <div class=\"tartan-legend-box rag-quintiles-1\">\r\n                Best\r\n            </div>\r\n            <div class=\"tartan-legend-box rag-quintiles-2\">\r\n                &nbsp;\r\n            </div>\r\n            <div class=\"tartan-legend-box rag-quintiles-3\">\r\n                &nbsp;\r\n            </div>\r\n            <div class=\"tartan-legend-box rag-quintiles-4\">\r\n                &nbsp;\r\n            </div>\r\n            <div class=\"tartan-legend-box rag-quintiles-5\">\r\n                Worst\r\n            </div>\r\n        </div>\r\n        <div class=\"map-legend-box left-margin\">\r\n            <div class=\"tartan-legend-box not-compared\">\r\n                Not applicable\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n<div class=\"row\" *ngIf=\"showQuintilesBOB\">\r\n    <div class=\"col-md-12\">\r\n        <div class=\"compared-with-text\">\r\n            <i>Quintiles:</i>\r\n        </div>\r\n        <div class=\"map-legend-box left-margin\">\r\n            <div class=\"tartan-legend-box bob-quintiles-1\">\r\n                Low\r\n            </div>\r\n            <div class=\"tartan-legend-box bob-quintiles-2\">\r\n                &nbsp;\r\n            </div>\r\n            <div class=\"tartan-legend-box bob-quintiles-3\">\r\n                &nbsp;\r\n            </div>\r\n            <div class=\"tartan-legend-box bob-quintiles-4\">\r\n                &nbsp;\r\n            </div>\r\n            <div class=\"tartan-legend-box bob-quintiles-5\">\r\n                High\r\n            </div>\r\n        </div>\r\n        <div class=\"map-legend-box left-margin\">\r\n            <div class=\"tartan-legend-box not-compared\">\r\n                Not applicable\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n<div class=\"row\" *ngIf=\"showContinuous\">\r\n    <div class=\"col-md-12\">\r\n        <div class=\"compared-with-text\">\r\n            <i>Continuous:</i>\r\n        </div>\r\n        <div class=\"map-legend-box left-margin\">\r\n            <div class=\"tartan-legend-box continuous-lowest\">\r\n                Lowest\r\n            </div>\r\n            <div class=\"tartan-legend-box continuous-range\">\r\n                &nbsp;\r\n            </div>\r\n            <div class=\"tartan-legend-box continuous-highest\">\r\n                Highest\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/component/legend/legend-map/legend-map.component.ts":
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
var LegendMapComponent = (function () {
    function LegendMapComponent() {
        this.showRAG3 = null;
        this.showRAG5 = null;
        this.showBOB = null;
        this.showQuartiles = null;
        this.showQuintilesRAG = null;
        this.showQuintilesBOB = null;
        this.showContinuous = null;
        this.showTartanLegends = false;
    }
    LegendMapComponent.prototype.ngOnChanges = function (changes) {
        if (changes['showRAG3']) {
            if (this.showRAG3) {
            }
        }
        if (changes['showRAG5']) {
            if (this.showRAG5) {
            }
        }
        if (changes['showBOB']) {
            if (this.showBOB) {
            }
        }
        if (changes['showQuartiles']) {
            if (this.showQuartiles) {
            }
        }
        if (changes['showQuintilesRAG']) {
            if (this.showQuintilesRAG) {
            }
        }
        if (changes['showQuintilesBOB']) {
            if (this.showQuintilesBOB) {
            }
        }
        if (changes['showContinuous']) {
            if (this.showContinuous) {
            }
        }
        this.showHideTartanLegends();
    };
    LegendMapComponent.prototype.showHideTartanLegends = function () {
        if (this.showRAG3 || this.showRAG5 || this.showBOB) {
            this.showTartanLegends = true;
        }
        else {
            this.showTartanLegends = false;
        }
    };
    return LegendMapComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendMapComponent.prototype, "showRAG3", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendMapComponent.prototype, "showRAG5", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendMapComponent.prototype, "showBOB", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendMapComponent.prototype, "showQuartiles", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendMapComponent.prototype, "showQuintilesRAG", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendMapComponent.prototype, "showQuintilesBOB", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendMapComponent.prototype, "showContinuous", void 0);
LegendMapComponent = __decorate([
    core_1.Component({
        selector: 'ft-legend-map',
        template: __webpack_require__("./src/app/shared/component/legend/legend-map/legend-map.component.html"),
        styles: [__webpack_require__("./src/app/shared/component/legend/legend-map/legend-map.component.css")]
    }),
    __metadata("design:paramtypes", [])
], LegendMapComponent);
exports.LegendMapComponent = LegendMapComponent;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/legend-map.component.js.map

/***/ }),

/***/ "./src/app/shared/component/legend/legend-quartiles/legend-quartiles.component.css":
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/shared/component/legend/legend-quartiles/legend-quartiles.component.html":
/***/ (function(module, exports) {

module.exports = "<table class=\"key-table custom-key-table\" cellspacing=\"2\" *ngIf=\"showQuartiles\">\r\n  <tbody>\r\n    <tr>\r\n      <td class=\"key-text\">Quartiles:</td>\r\n      <td style=\"background-color: #E8C7D1;\">Low</td>\r\n      <td style=\"background-color: #B74D6D;\" class=\"whiteText\">&nbsp;</td>\r\n      <td style=\"background-color: #98002E;\" class=\"whiteText\">&nbsp;</td>\r\n      <td style=\"background-color: #700023;\" class=\"whiteText\">High</td>\r\n    </tr>\r\n  </tbody>\r\n</table>"

/***/ }),

/***/ "./src/app/shared/component/legend/legend-quartiles/legend-quartiles.component.ts":
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
var legend_component_1 = __webpack_require__("./src/app/shared/component/legend/legend.component.ts");
var LegendQuartilesComponent = (function () {
    function LegendQuartilesComponent() {
        this.legendType = null;
        this.keyType = null;
        this.showQuartiles = false;
    }
    LegendQuartilesComponent.prototype.ngOnChanges = function (changes) {
        if (changes['legendType']) {
            if (this.legendType) {
                if (this.legendType === legend_component_1.LegendType.Quartiles) {
                    this.showQuartiles = true;
                }
                else {
                    this.showQuartiles = false;
                }
            }
        }
    };
    return LegendQuartilesComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_a = typeof legend_component_1.LegendType !== "undefined" && legend_component_1.LegendType) === "function" && _a || Object)
], LegendQuartilesComponent.prototype, "legendType", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_b = typeof legend_component_1.KeyType !== "undefined" && legend_component_1.KeyType) === "function" && _b || Object)
], LegendQuartilesComponent.prototype, "keyType", void 0);
LegendQuartilesComponent = __decorate([
    core_1.Component({
        selector: 'ft-legend-quartiles',
        template: __webpack_require__("./src/app/shared/component/legend/legend-quartiles/legend-quartiles.component.html"),
        styles: [__webpack_require__("./src/app/shared/component/legend/legend-quartiles/legend-quartiles.component.css")]
    }),
    __metadata("design:paramtypes", [])
], LegendQuartilesComponent);
exports.LegendQuartilesComponent = LegendQuartilesComponent;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/legend-quartiles.component.js.map

/***/ }),

/***/ "./src/app/shared/component/legend/legend-quintiles/legend-quintiles.component.css":
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/shared/component/legend/legend-quintiles/legend-quintiles.component.html":
/***/ (function(module, exports) {

module.exports = "<table class=\"key-table custom-key-table\" cellspacing=\"2\" *ngIf=\"showQuintiles\">\r\n  <tr>\r\n    <td class=\"key-text\">Quintiles:</td>\r\n    <td style=\"background-color: #DED3EC;\">Low</td>\r\n    <td style=\"background-color: #BEA7DA;\">&nbsp;</td>\r\n    <td style=\"background-color: #9E7CC8;\" class=\"whiteText\">&nbsp;</td>\r\n    <td style=\"background-color: #7E50B6;\" class=\"whiteText\">&nbsp;</td>\r\n    <td style=\"background-color: #5E25A4;\" class=\"whiteText\">High</td>\r\n  </tr>\r\n</table>"

/***/ }),

/***/ "./src/app/shared/component/legend/legend-quintiles/legend-quintiles.component.ts":
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
var legend_component_1 = __webpack_require__("./src/app/shared/component/legend/legend.component.ts");
var LegendQuintilesComponent = (function () {
    function LegendQuintilesComponent() {
        this.legendType = null;
        this.keyType = null;
        this.showQuintiles = false;
    }
    LegendQuintilesComponent.prototype.ngOnChanges = function (changes) {
        if (changes['legendType']) {
            if (this.legendType) {
                if (this.legendType === legend_component_1.LegendType.Quintiles) {
                    this.showQuintiles = true;
                }
                else {
                    this.showQuintiles = false;
                }
            }
        }
    };
    return LegendQuintilesComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_a = typeof legend_component_1.LegendType !== "undefined" && legend_component_1.LegendType) === "function" && _a || Object)
], LegendQuintilesComponent.prototype, "legendType", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_b = typeof legend_component_1.KeyType !== "undefined" && legend_component_1.KeyType) === "function" && _b || Object)
], LegendQuintilesComponent.prototype, "keyType", void 0);
LegendQuintilesComponent = __decorate([
    core_1.Component({
        selector: 'ft-legend-quintiles',
        template: __webpack_require__("./src/app/shared/component/legend/legend-quintiles/legend-quintiles.component.html"),
        styles: [__webpack_require__("./src/app/shared/component/legend/legend-quintiles/legend-quintiles.component.css")]
    }),
    __metadata("design:paramtypes", [])
], LegendQuintilesComponent);
exports.LegendQuintilesComponent = LegendQuintilesComponent;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/legend-quintiles.component.js.map

/***/ }),

/***/ "./src/app/shared/component/legend/legend-rag-3/legend-rag-3.component.css":
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/shared/component/legend/legend-rag-3/legend-rag-3.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"keyTartanRug\" class=\"key-container\" *ngIf=\"showRag\">\r\n  <table class=\"key-table\" *ngIf=\"showTartanRug\">\r\n    <tr>\r\n      <td class=\"key-text key-label\">Compared with benchmark</td>\r\n      <td class=\"key-spacer\"></td>\r\n      <td id=\"td-better-key\" class='better key'>\r\n        <div class=\"tartan-box key-box\">\r\n          <img class=\"tartan-fill print-only\" src=\"/images/better.png\" alt=\"\" />\r\n          <div class=\"tartan-text\">\r\n            Better\r\n          </div>\r\n        </div>\r\n      </td>\r\n      <td id=\"td-same-key\" class='same key'>\r\n        <div class=\"tartan-box key-box\">\r\n          <img class=\"tartan-fill print-only\" src=\"/images/same.png\" alt=\"\" />\r\n          <div class=\"tartan-text\">\r\n            Similar\r\n          </div>\r\n        </div>\r\n      </td>\r\n      <td id=\"td-worse-key\" class='worse key'>\r\n        <div class=\"tartan-box key-box\">\r\n          <img class=\"tartan-fill print-only\" src=\"/images/red.png\" alt=\"\" />\r\n          <div class=\"tartan-text\">\r\n            Worse\r\n          </div>\r\n        </div>\r\n      </td>\r\n      <td id=\"map-key-part2\" class=\"none key key-box-right\" style=\"display: table-cell;\" *ngIf=\"showBenchmark\">\r\n        <div class=\"tartan-box key-box\">\r\n          Not compared\r\n        </div>\r\n      </td>\r\n    </tr>\r\n  </table>\r\n  <table class=\"key-table\" *ngIf=\"showSpineChart\">\r\n    <tr>\r\n      <td class=\"key-text key-label\">Compared with benchmark</td>\r\n      <td class=\"key-spacer\"></td>\r\n      <td class='key-spine'>\r\n        <img class=\"key-circle\" src=\"/images/circle_green_mini.png\" alt=\"\" />\r\n        Better\r\n        <img class=\"key-circle\" src=\"/images/circle_orange_mini.png\" alt=\"\" />\r\n        Similar\r\n        <img class=\"key-circle\" src=\"/images/circle_red_mini.png\" alt=\"\" />\r\n        Worse\r\n      </td>\r\n      <td class='key-spine' *ngIf=\"showBenchmark\">\r\n        <img class=\"key-circle\" src=\"/images/circle_white_mini.png\" alt=\"\" />\r\n        Not Compared\r\n      </td>\r\n    </tr>\r\n  </table>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/component/legend/legend-rag-3/legend-rag-3.component.ts":
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
var legend_component_1 = __webpack_require__("./src/app/shared/component/legend/legend.component.ts");
var LegendRag3Component = (function () {
    function LegendRag3Component() {
        this.legendType = null;
        this.keyType = null;
        this.showRag = false;
        this.showTartanRug = false;
        this.showSpineChart = false;
        this.showBenchmark = true;
    }
    LegendRag3Component.prototype.ngOnChanges = function (changes) {
        if (changes['legendType']) {
            if (this.legendType) {
                if (this.legendType === legend_component_1.LegendType.RAG3) {
                    this.showRag = true;
                }
                else {
                    this.showRag = false;
                }
            }
        }
        if (changes['keyType']) {
            if (this.keyType) {
                switch (this.keyType) {
                    case legend_component_1.KeyType.TartanRug:
                        this.showTartanRug = true;
                        break;
                    case legend_component_1.KeyType.SpineChart:
                        this.showSpineChart = true;
                        break;
                    default:
                        this.showTartanRug = false;
                        this.showSpineChart = false;
                        break;
                }
            }
        }
    };
    return LegendRag3Component;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_a = typeof legend_component_1.LegendType !== "undefined" && legend_component_1.LegendType) === "function" && _a || Object)
], LegendRag3Component.prototype, "legendType", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_b = typeof legend_component_1.KeyType !== "undefined" && legend_component_1.KeyType) === "function" && _b || Object)
], LegendRag3Component.prototype, "keyType", void 0);
LegendRag3Component = __decorate([
    core_1.Component({
        selector: 'ft-legend-rag-3',
        template: __webpack_require__("./src/app/shared/component/legend/legend-rag-3/legend-rag-3.component.html"),
        styles: [__webpack_require__("./src/app/shared/component/legend/legend-rag-3/legend-rag-3.component.css")]
    }),
    __metadata("design:paramtypes", [])
], LegendRag3Component);
exports.LegendRag3Component = LegendRag3Component;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/legend-rag-3.component.js.map

/***/ }),

/***/ "./src/app/shared/component/legend/legend-rag-5/legend-rag-5.component.css":
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/shared/component/legend/legend-rag-5/legend-rag-5.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"keyTartanRug\" class=\"key-container\" *ngIf=\"showRag\">\r\n  <table class=\"key-table\" *ngIf=\"showTartanRug\">\r\n    <tr>\r\n      <td class=\"key-text key-label\">Compared with benchmark</td>\r\n      <td class=\"key-spacer\"></td>\r\n      <td id=\"td-best-key\" class='best key'>\r\n        <div class=\"tartan-box key-box\">\r\n          <img class=\"tartan-fill print-only\" src=\"/images/best.png\" alt=\"\" />\r\n          <div class=\"tartan-text\">\r\n            Better 99.8%\r\n          </div>\r\n        </div>\r\n      </td>\r\n      <td id=\"td-better-key\" class='better key'>\r\n        <div class=\"tartan-box key-box\">\r\n          <img class=\"tartan-fill print-only\" src=\"/images/better.png\" alt=\"\" />\r\n          <div class=\"tartan-text\">\r\n            Better 95%\r\n          </div>\r\n        </div>\r\n      </td>\r\n      <td id=\"td-same-key\" class='same key'>\r\n        <div class=\"tartan-box key-box\">\r\n          <img class=\"tartan-fill print-only\" src=\"/images/same.png\" alt=\"\" />\r\n          <div class=\"tartan-text\">\r\n            Similar\r\n          </div>\r\n        </div>\r\n      </td>\r\n      <td id=\"td-worse-key\" class='worse key'>\r\n        <div class=\"tartan-box key-box\">\r\n          <img class=\"tartan-fill print-only\" src=\"/images/red.png\" alt=\"\" />\r\n          <div class=\"tartan-text\">\r\n            Worse 95%\r\n          </div>\r\n        </div>\r\n      </td>\r\n      <td id=\"td-worst-key\" class='worst key'>\r\n        <div class=\"tartan-box key-box\">\r\n          <img class=\"tartan-fill print-only\" src=\"/images/worst.png\" alt=\"\" />\r\n          <div class=\"tartan-text\">\r\n            Worse 99.8%\r\n          </div>\r\n        </div>\r\n      </td>\r\n      <td id=\"map-key-part2\" class=\"none key key-box-right\" style=\"display: table-cell;\" *ngIf=\"showBenchmark\">\r\n        <div class=\"tartan-box key-box\">\r\n          Not compared\r\n        </div>\r\n      </td>\r\n    </tr>\r\n  </table>\r\n  <table class=\"key-table\" *ngIf=\"showSpineChart\">\r\n    <tr>\r\n      <td class=\"key-text key-label\">Compared with benchmark</td>\r\n      <td class=\"key-spacer\"></td>\r\n      <td class='key-spine'>\r\n        <img class=\"key-circle\" src=\"/images/circle_dark_green_mini.png\" alt=\"\" />\r\n        Better 99.8%\r\n        <img class=\"key-circle\" src=\"/images/circle_green_mini.png\" alt=\"\" />\r\n        Better 95%\r\n        <img class=\"key-circle\" src=\"/images/circle_orange_mini.png\" alt=\"\" />\r\n        Similar\r\n        <img class=\"key-circle\" src=\"/images/circle_red_mini.png\" alt=\"\" />\r\n        Worse 95%\r\n        <img class=\"key-circle\" src=\"/images/circle_dark_red_mini.png\" alt=\"\" />\r\n        Worse 99.8%\r\n      </td>\r\n      <td class='key-spine' *ngIf=\"showBenchmark\">\r\n        <img class=\"key-circle\" src=\"/images/circle_white_mini.png\" alt=\"\" />\r\n        Not Compared\r\n      </td>\r\n    </tr>\r\n  </table>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/component/legend/legend-rag-5/legend-rag-5.component.ts":
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
var legend_component_1 = __webpack_require__("./src/app/shared/component/legend/legend.component.ts");
var LegendRag5Component = (function () {
    function LegendRag5Component() {
        this.legendType = null;
        this.keyType = null;
        this.showRag = false;
        this.showTartanRug = false;
        this.showSpineChart = false;
        this.showBenchmark = true;
    }
    LegendRag5Component.prototype.ngOnChanges = function (changes) {
        if (changes['legendType']) {
            if (this.legendType) {
                if (this.legendType === legend_component_1.LegendType.RAG5) {
                    this.showRag = true;
                }
                else {
                    this.showRag = false;
                }
            }
        }
        if (changes['keyType']) {
            if (this.keyType) {
                switch (this.keyType) {
                    case legend_component_1.KeyType.TartanRug:
                        this.showTartanRug = true;
                        break;
                    case legend_component_1.KeyType.SpineChart:
                        this.showSpineChart = true;
                        break;
                    default:
                        this.showTartanRug = false;
                        this.showSpineChart = false;
                        break;
                }
            }
        }
    };
    return LegendRag5Component;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_a = typeof legend_component_1.LegendType !== "undefined" && legend_component_1.LegendType) === "function" && _a || Object)
], LegendRag5Component.prototype, "legendType", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", typeof (_b = typeof legend_component_1.KeyType !== "undefined" && legend_component_1.KeyType) === "function" && _b || Object)
], LegendRag5Component.prototype, "keyType", void 0);
LegendRag5Component = __decorate([
    core_1.Component({
        selector: 'ft-legend-rag-5',
        template: __webpack_require__("./src/app/shared/component/legend/legend-rag-5/legend-rag-5.component.html"),
        styles: [__webpack_require__("./src/app/shared/component/legend/legend-rag-5/legend-rag-5.component.css")]
    }),
    __metadata("design:paramtypes", [])
], LegendRag5Component);
exports.LegendRag5Component = LegendRag5Component;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/legend-rag-5.component.js.map

/***/ }),

/***/ "./src/app/shared/component/legend/legend-recent-trends/legend-recent-trends.component.css":
/***/ (function(module, exports) {

module.exports = ""

/***/ }),

/***/ "./src/app/shared/component/legend/legend-recent-trends/legend-recent-trends.component.html":
/***/ (function(module, exports) {

module.exports = "<div id=\"trend-marker-legend\" *ngIf=\"showRecentTrends\">\r\n  <table class=\"key-table\" cellspacing=\"0\">\r\n    <tr>\r\n      <td><img src=\"/images/trends/info.png\" class=\"trend-info\" (click)=\"showTrendInfo()\" />&nbsp;&nbsp;&nbsp;</td>\r\n\r\n      <td class=\"key-label\">Recent trends:</td>\r\n      <td><img src=\"/images/trends/no_calc.png\" /></td>\r\n      <td class=\"legendText\">Could not be<br>calculated</td>\r\n\r\n      <td><img src=\"/images/trends/up_red.png\" /></td>\r\n      <td class=\"legendText\">Increasing /<br>Getting worse</td>\r\n\r\n      <td><img src=\"/images/trends/up_green.png\" /></td>\r\n      <td class=\"legendText\">Increasing /<br>Getting better</td>\r\n\r\n      <td><img src=\"/images/trends/down_red.png\" /></td>\r\n      <td class=\"legendText\">Decreasing /<br>Getting worse</td>\r\n\r\n      <td><img src=\"/images/trends/down_green.png\" /></td>\r\n      <td class=\"legendText\">Decreasing /<br>Getting better</td>\r\n\r\n      <td><img src=\"/images/trends/no_change.png\" /></td>\r\n      <td class=\"legendText\">No significant<br>change</td>\r\n\r\n      <td><img src=\"/images/trends/up_blue.png\" /></td>\r\n      <td class=\"legendText\">Increasing</td>\r\n\r\n      <td><img src=\"/images/trends/down_blue.png\" /></td>\r\n      <td class=\"legendText\">Decreasing</td>\r\n    </tr>\r\n  </table>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/component/legend/legend-recent-trends/legend-recent-trends.component.ts":
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
var LegendRecentTrendsComponent = (function () {
    function LegendRecentTrendsComponent(ftHelperService) {
        this.ftHelperService = ftHelperService;
        this.showRecentTrends = null;
    }
    LegendRecentTrendsComponent.prototype.ngOnChanges = function (changes) {
    };
    LegendRecentTrendsComponent.prototype.showTrendInfo = function () {
        this.ftHelperService.showTrendInfo();
    };
    return LegendRecentTrendsComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendRecentTrendsComponent.prototype, "showRecentTrends", void 0);
LegendRecentTrendsComponent = __decorate([
    core_1.Component({
        selector: 'ft-legend-recent-trends',
        template: __webpack_require__("./src/app/shared/component/legend/legend-recent-trends/legend-recent-trends.component.html"),
        styles: [__webpack_require__("./src/app/shared/component/legend/legend-recent-trends/legend-recent-trends.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _a || Object])
], LegendRecentTrendsComponent);
exports.LegendRecentTrendsComponent = LegendRecentTrendsComponent;
var _a;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/legend-recent-trends.component.js.map

/***/ }),

/***/ "./src/app/shared/component/legend/legend.component.css":
/***/ (function(module, exports) {

module.exports = ".value-note-legend {\r\n    color: #777;\r\n    font-size: 11px;\r\n    float: right;\r\n    margin-bottom: 10px;\r\n    text-align: right;\r\n}"

/***/ }),

/***/ "./src/app/shared/component/legend/legend.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"row\">\r\n    <div class=\"col-md-8\">\r\n        &nbsp;\r\n    </div>\r\n    <div class=\"col-md-4 value-note-legend\">\r\n        <span class=\"asterisk\">*</span> a note is attached to the value, hover over to see more details\r\n    </div>\r\n</div>\r\n<div class=\"row\">\r\n    <div class=\"col-md-12\">\r\n        <ft-legend-area-profiles [showRAG3]=\"showRAG3\" [showRAG5]=\"showRAG5\" [showBOB]=\"showBOB\" [showQuintilesRAG]=\"showQuintilesRAG\"\r\n            [showQuintilesBOB]=\"showQuintilesBOB\" *ngIf=\"showAreaProfiles()\"></ft-legend-area-profiles>\r\n        <ft-legend-map [showRAG3]=\"showRAG3\" [showRAG5]=\"showRAG5\" [showBOB]=\"showBOB\" [showQuartiles]=\"showQuartiles\"\r\n            [showQuintilesRAG]=\"showQuintilesRAG\" [showQuintilesBOB]=\"showQuintilesBOB\" [showContinuous]=\"showContinuous\"\r\n            *ngIf=\"showMap()\"></ft-legend-map>\r\n        <ft-legend-bob [legendType]=\"legendType\" [keyType]=\"keyType\"></ft-legend-bob>\r\n        <ft-legend-continuous [legendType]=\"legendType\" [keyType]=\"keyType\"></ft-legend-continuous>\r\n        <ft-legend-quartiles [legendType]=\"legendType\" [keyType]=\"keyType\"></ft-legend-quartiles>\r\n        <ft-legend-quintiles [legendType]=\"legendType\" [keyType]=\"keyType\"></ft-legend-quintiles>\r\n        <ft-legend-rag-3 [legendType]=\"legendType\" [keyType]=\"keyType\"></ft-legend-rag-3>\r\n        <ft-legend-rag-5 [legendType]=\"legendType\" [keyType]=\"keyType\"></ft-legend-rag-5>\r\n    </div>\r\n</div>\r\n<div class=\"row\">\r\n    <div class=\"col-md-12\">\r\n        <ft-legend-recent-trends [showRecentTrends]=\"showRecentTrends\"></ft-legend-recent-trends>\r\n    </div>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/component/legend/legend.component.ts":
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
var LegendComponent = (function () {
    function LegendComponent() {
        this.pageType = null;
        this.keyType = null;
        this.legendType = null;
        this.showRAG3 = null;
        this.showRAG5 = null;
        this.showBOB = null;
        this.showQuartiles = null;
        this.showQuintilesRAG = null;
        this.showQuintilesBOB = null;
        this.showContinuous = null;
        this.showRecentTrends = null;
    }
    LegendComponent.prototype.ngOnChanges = function (changes) {
        if (changes['pageType']) {
            if (this.pageType) {
            }
        }
        if (changes['keyType']) {
            if (this.keyType) {
            }
        }
        if (changes['legendType']) {
            if (this.legendType) {
            }
        }
    };
    LegendComponent.prototype.showOverview = function () {
        return this.pageType === PageType.Overview;
    };
    LegendComponent.prototype.showMap = function () {
        return this.pageType === PageType.Map;
    };
    LegendComponent.prototype.showTrends = function () {
        return this.pageType === PageType.Trends;
    };
    LegendComponent.prototype.showCompareAreas = function () {
        return this.pageType === PageType.CompareAreas;
    };
    LegendComponent.prototype.showAreaProfiles = function () {
        return this.pageType === PageType.AreaProfiles;
    };
    LegendComponent.prototype.showInequalities = function () {
        return this.pageType === PageType.Inequalities;
    };
    LegendComponent.prototype.showEngland = function () {
        return this.pageType === PageType.England;
    };
    return LegendComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", PageType)
], LegendComponent.prototype, "pageType", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", KeyType)
], LegendComponent.prototype, "keyType", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", LegendType)
], LegendComponent.prototype, "legendType", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendComponent.prototype, "showRAG3", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendComponent.prototype, "showRAG5", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendComponent.prototype, "showBOB", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendComponent.prototype, "showQuartiles", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendComponent.prototype, "showQuintilesRAG", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendComponent.prototype, "showQuintilesBOB", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendComponent.prototype, "showContinuous", void 0);
__decorate([
    core_1.Input(),
    __metadata("design:type", Object)
], LegendComponent.prototype, "showRecentTrends", void 0);
LegendComponent = __decorate([
    core_1.Component({
        selector: 'ft-legend',
        template: __webpack_require__("./src/app/shared/component/legend/legend.component.html"),
        styles: [__webpack_require__("./src/app/shared/component/legend/legend.component.css")]
    })
], LegendComponent);
exports.LegendComponent = LegendComponent;
var PageType = (function () {
    function PageType() {
    }
    return PageType;
}());
PageType.None = 0;
PageType.Overview = 1;
PageType.Map = 2;
PageType.Trends = 3;
PageType.CompareAreas = 4;
PageType.AreaProfiles = 5;
PageType.Inequalities = 6;
PageType.England = 7;
exports.PageType = PageType;
var KeyType = (function () {
    function KeyType() {
    }
    return KeyType;
}());
KeyType.None = 0;
KeyType.BarChart = 1;
KeyType.ValueNote = 2;
KeyType.SpineChart = 3;
KeyType.TartanRug = 4;
KeyType.InEquality = 5;
KeyType.RecentTrends = 6;
KeyType.DataQuality = 7;
exports.KeyType = KeyType;
var LegendType = (function () {
    function LegendType() {
    }
    return LegendType;
}());
LegendType.None = 0;
LegendType.RAG3 = 1;
LegendType.RAG5 = 2;
LegendType.BOB = 3;
LegendType.NotCompared = 4;
LegendType.Quintiles = 5;
LegendType.Quartiles = 6;
LegendType.Continuous = 7;
exports.LegendType = LegendType;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/legend.component.js.map

/***/ }),

/***/ "./src/app/shared/component/legend/legend.module.ts":
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
var legend_bob_component_1 = __webpack_require__("./src/app/shared/component/legend/legend-bob/legend-bob.component.ts");
var legend_continuous_component_1 = __webpack_require__("./src/app/shared/component/legend/legend-continuous/legend-continuous.component.ts");
var legend_quartiles_component_1 = __webpack_require__("./src/app/shared/component/legend/legend-quartiles/legend-quartiles.component.ts");
var legend_quintiles_component_1 = __webpack_require__("./src/app/shared/component/legend/legend-quintiles/legend-quintiles.component.ts");
var legend_rag_3_component_1 = __webpack_require__("./src/app/shared/component/legend/legend-rag-3/legend-rag-3.component.ts");
var legend_rag_5_component_1 = __webpack_require__("./src/app/shared/component/legend/legend-rag-5/legend-rag-5.component.ts");
var legend_component_1 = __webpack_require__("./src/app/shared/component/legend/legend.component.ts");
var legend_recent_trends_component_1 = __webpack_require__("./src/app/shared/component/legend/legend-recent-trends/legend-recent-trends.component.ts");
var legend_area_profiles_component_1 = __webpack_require__("./src/app/shared/component/legend/legend-area-profiles/legend-area-profiles.component.ts");
var legend_map_component_1 = __webpack_require__("./src/app/shared/component/legend/legend-map/legend-map.component.ts");
var LegendModule = (function () {
    function LegendModule() {
    }
    return LegendModule;
}());
LegendModule = __decorate([
    core_1.NgModule({
        imports: [
            common_1.CommonModule,
        ],
        declarations: [
            legend_component_1.LegendComponent,
            legend_bob_component_1.LegendBobComponent,
            legend_continuous_component_1.LegendContinuousComponent,
            legend_quartiles_component_1.LegendQuartilesComponent,
            legend_quintiles_component_1.LegendQuintilesComponent,
            legend_rag_3_component_1.LegendRag3Component,
            legend_rag_5_component_1.LegendRag5Component,
            legend_recent_trends_component_1.LegendRecentTrendsComponent,
            legend_area_profiles_component_1.LegendAreaProfilesComponent,
            legend_map_component_1.LegendMapComponent
        ],
        exports: [
            legend_component_1.LegendComponent
        ]
    })
], LegendModule);
exports.LegendModule = LegendModule;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/legend.module.js.map

/***/ }),

/***/ "./src/app/shared/component/light-box-with-input/light-box-with-input.component.css":
/***/ (function(module, exports) {

module.exports = ".light-box {\r\n    position: absolute;\r\n    top: 0;\r\n    left: 0;\r\n    background: #000;\r\n    height: 1500px;\r\n    width: 100%;\r\n    opacity: 0.50;\r\n    -ms-filter: \"progid:DXImageTransform.Microsoft.Alpha(Opacity=50)\";\r\n    filter: alpha(opacity=50);\r\n    z-index: 1;\r\n}\r\n\r\n.info-box {\r\n    position: absolute;\r\n    background: white;\r\n    border: 5px solid #eee;\r\n    padding: 6px;\r\n    z-index: 1000;\r\n    border-radius: 5px;\r\n    top: 30%;\r\n    left: calc(50% - 250px);\r\n    width: 500px;\r\n    height: 230px;\r\n}\r\n\r\n.info-text {\r\n    margin: 10px 0px 10px 0px;\r\n}\r\n\r\n.close-button {\r\n    width: 100%;\r\n    text-align: center;\r\n    font-style: italic;\r\n    margin-top: 6px;\r\n    float: right;\r\n    font-size: 1.5rem;\r\n    font-weight: 700;\r\n    line-height: 27;\r\n    color: #000;\r\n    text-shadow: 0 1px 0 #fff;\r\n    opacity: .5;\r\n    cursor: pointer;\r\n}\r\n\r\n.ok {\r\n    width: 90px;\r\n}\r\n\r\nh2 {\r\n    border-bottom: none;\r\n    margin: 0 0 3px 0 !important;\r\n}\r\n\r\n.error-message {\r\n    color: #ff0000;\r\n    margin-top: 10px;\r\n    font-size: 14px;\r\n    height: 10px;\r\n}\r\n\r\n.control-buttons-container {\r\n    margin-top: 24px;\r\n}\r\n\r\n.info-box-text-input {\r\n    border: 1px solid;\r\n    padding: 10px;\r\n    width: 300px;\r\n}\r\n"

/***/ }),

/***/ "./src/app/shared/component/light-box-with-input/light-box-with-input.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"light-box\" *ngIf=\"showLightBox\" (click)=\"cancel()\">\r\n</div>\r\n<div class=\"info-box\" *ngIf=\"showLightBox\" [style.height]=\"this.lightBoxWithInputConfig.Height + 'px'\">\r\n  <h2>{{this.lightBoxWithInputConfig.Title}}</h2>\r\n  <div class=\"info-text\" [innerHtml]=\"this.lightBoxWithInputConfig.Html\"></div>\r\n  <div class=\"info-text-box\">\r\n    <input type=\"text\" id=\"info-box-text-input\" [value]=\"this.lightBoxWithInputConfig.InputText\" class=\"info-box-text-input\">\r\n  </div>\r\n  <div class=\"error-message\" [innerHtml]=\"this.errorMessage\">\r\n    &nbsp;\r\n  </div>\r\n\r\n  <div *ngIf=\"isInfoBoxOkCancel\" class=\"control-buttons-container\">\r\n    <button class=\"btn btn-primary active ok\" (click)=\"confirm()\">OK</button>\r\n    <button class=\"btn btn-link active\" (click)=\"cancel()\">Cancel</button>\r\n  </div>\r\n\r\n  <div *ngIf=\"isInfoBoxOk\">\r\n    <button class=\"btn btn-primary active ok\" (click)=\"confirm()\">OK</button>\r\n  </div>\r\n\r\n  <div class=\"close\" (click)=\"cancel()\"></div>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/component/light-box-with-input/light-box-with-input.component.ts":
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
var LightBoxWithInputComponent = (function () {
    function LightBoxWithInputComponent(changeDetectorRef) {
        this.changeDetectorRef = changeDetectorRef;
        this.lightBoxWithInputConfig = null;
        this.emitLightBoxWithInputActionConfirmed = new core_1.EventEmitter();
        this.emitLightBoxInputText = new core_1.EventEmitter();
        this.showLightBox = false;
        this.isInfoBoxOk = false;
        this.isInfoBoxOkCancel = false;
    }
    LightBoxWithInputComponent.prototype.ngOnChanges = function (changes) {
        if (changes['lightBoxWithInputConfig']) {
            if (this.lightBoxWithInputConfig) {
                this.loadLightBox();
            }
        }
    };
    LightBoxWithInputComponent.prototype.loadLightBox = function () {
        if (this.lightBoxWithInputConfig !== null) {
            this.errorMessage = '';
            this.lightBoxInputText = this.lightBoxWithInputConfig.InputText;
            // Type
            switch (this.lightBoxWithInputConfig.Type) {
                case LightBoxWithInputTypes.Ok:
                    this.isInfoBoxOk = true;
                    break;
                case LightBoxWithInputTypes.OkCancel:
                    this.isInfoBoxOkCancel = true;
                    break;
            }
            this.showLightBox = true;
        }
    };
    LightBoxWithInputComponent.prototype.confirm = function () {
        this.lightBoxInputText = document.getElementById("info-box-text-input").value;
        this.closePopupAndEmit(true);
    };
    LightBoxWithInputComponent.prototype.cancel = function () {
        this.closePopupAndEmit(false);
    };
    LightBoxWithInputComponent.prototype.closePopupAndEmit = function (actionConfirmed) {
        if (actionConfirmed) {
            if (this.validateInput()) {
                switch (this.lightBoxWithInputConfig.Type) {
                    case LightBoxWithInputTypes.Ok:
                        this.isInfoBoxOk = false;
                        break;
                    case LightBoxWithInputTypes.OkCancel:
                        this.isInfoBoxOkCancel = false;
                        break;
                }
                this.emitLightBoxInputText.emit(this.lightBoxInputText);
            }
            else {
                this.errorMessage = "Please enter a valid area list name";
                return false;
            }
        }
        this.showLightBox = false;
        this.emitLightBoxWithInputActionConfirmed.emit(actionConfirmed);
    };
    LightBoxWithInputComponent.prototype.validateInput = function () {
        if (this.lightBoxInputText === undefined || this.lightBoxInputText.trim().length === 0) {
            return false;
        }
        return true;
    };
    return LightBoxWithInputComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", LightBoxWithInputConfig)
], LightBoxWithInputComponent.prototype, "lightBoxWithInputConfig", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], LightBoxWithInputComponent.prototype, "emitLightBoxWithInputActionConfirmed", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], LightBoxWithInputComponent.prototype, "emitLightBoxInputText", void 0);
LightBoxWithInputComponent = __decorate([
    core_1.Component({
        selector: 'ft-light-box-with-input',
        template: __webpack_require__("./src/app/shared/component/light-box-with-input/light-box-with-input.component.html"),
        styles: [__webpack_require__("./src/app/shared/component/light-box-with-input/light-box-with-input.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof core_1.ChangeDetectorRef !== "undefined" && core_1.ChangeDetectorRef) === "function" && _a || Object])
], LightBoxWithInputComponent);
exports.LightBoxWithInputComponent = LightBoxWithInputComponent;
var LightBoxWithInputConfig = (function () {
    function LightBoxWithInputConfig() {
    }
    return LightBoxWithInputConfig;
}());
exports.LightBoxWithInputConfig = LightBoxWithInputConfig;
var LightBoxWithInputTypes = (function () {
    function LightBoxWithInputTypes() {
    }
    return LightBoxWithInputTypes;
}());
LightBoxWithInputTypes.Ok = 1;
LightBoxWithInputTypes.OkCancel = 2;
exports.LightBoxWithInputTypes = LightBoxWithInputTypes;
;
var _a;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/light-box-with-input.component.js.map

/***/ }),

/***/ "./src/app/shared/component/light-box/light-box.component.css":
/***/ (function(module, exports) {

module.exports = ".light-box {\r\n    position: absolute;\r\n    top: 0;\r\n    left: 0;\r\n    background: #000;\r\n    height: 1500px;\r\n    width: 100%;\r\n    opacity: 0.50;\r\n    -ms-filter: \"progid:DXImageTransform.Microsoft.Alpha(Opacity=50)\";\r\n    filter: alpha(opacity=50);\r\n    z-index: 1;\r\n}\r\n\r\n.info-box {\r\n    position: absolute;\r\n    background: white;\r\n    border: 5px solid #eee;\r\n    padding: 6px;\r\n    z-index: 1000;\r\n    border-radius: 5px;\r\n    top: 30%;\r\n    left: calc(50% - 250px);\r\n    width: 500px;\r\n    height: 180px;\r\n}\r\n\r\n.info-text {\r\n    padding-bottom: 20px;\r\n}\r\n\r\n.close-button {\r\n    width: 100%;\r\n    text-align: center;\r\n    font-style: italic;\r\n    margin-top: 6px;\r\n    float: right;\r\n    font-size: 1.5rem;\r\n    font-weight: 700;\r\n    line-height: 27;\r\n    color: #000;\r\n    text-shadow: 0 1px 0 #fff;\r\n    opacity: .5;\r\n    cursor: pointer;\r\n}\r\n\r\n.ok {\r\n    width: 90px;\r\n}\r\n\r\nh2 {\r\n    border-bottom: none;\r\n    margin: 0 0 3px 0 !important;\r\n}"

/***/ }),

/***/ "./src/app/shared/component/light-box/light-box.component.html":
/***/ (function(module, exports) {

module.exports = "<div class=\"light-box\" *ngIf=\"showLightBox\" (click)=\"cancel()\">\r\n</div>\r\n<div class=\"info-box\" *ngIf=\"showLightBox\" [style.height]=\"lightBoxConfig.Height + 'px'\" [style.top]=\"lightBoxConfig.Top + 'px'\">\r\n    <h2>{{this.lightBoxConfig.Title}}</h2>\r\n    <p class=\"info-text\" [innerHtml]=\"this.lightBoxConfig.Html\"></p>\r\n\r\n    <div *ngIf=\"isInfoBoxOkCancel\">\r\n        <button class=\"btn btn-primary active ok\" (click)=\"confirm()\">OK</button>\r\n        <button class=\"btn btn-link active\" (click)=\"cancel()\">Cancel</button>\r\n    </div>\r\n\r\n    <div *ngIf=\"isInfoBoxOk\">\r\n        <button class=\"btn btn-primary active ok\" (click)=\"confirm()\">OK</button>\r\n    </div>\r\n\r\n    <div class=\"close\" (click)=\"cancel()\"></div>\r\n</div>"

/***/ }),

/***/ "./src/app/shared/component/light-box/light-box.component.ts":
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
var LightBoxComponent = (function () {
    function LightBoxComponent(changeDetectorRef) {
        this.changeDetectorRef = changeDetectorRef;
        this.lightBoxConfig = null;
        this.emitLightBoxActionConfirmed = new core_1.EventEmitter();
        this.showLightBox = false;
        this.isInfoBoxOk = false;
        this.isInfoBoxOkCancel = false;
    }
    LightBoxComponent.prototype.ngOnChanges = function (changes) {
        if (changes['lightBoxConfig']) {
            if (this.lightBoxConfig) {
                this.loadLightBox();
            }
        }
    };
    LightBoxComponent.prototype.loadLightBox = function () {
        if (this.lightBoxConfig !== null) {
            // Type
            switch (this.lightBoxConfig.Type) {
                case LightBoxTypes.Ok:
                    this.isInfoBoxOk = true;
                    break;
                case LightBoxTypes.OkCancel:
                    this.isInfoBoxOkCancel = true;
                    break;
            }
            this.showLightBox = true;
        }
    };
    LightBoxComponent.prototype.confirm = function () {
        this.closePopupAndEmit(true);
    };
    LightBoxComponent.prototype.cancel = function () {
        this.closePopupAndEmit(false);
    };
    LightBoxComponent.prototype.closePopupAndEmit = function (actionConfirmed) {
        switch (this.lightBoxConfig.Type) {
            case LightBoxTypes.Ok:
                this.isInfoBoxOk = false;
                break;
            case LightBoxTypes.OkCancel:
                this.isInfoBoxOkCancel = false;
                break;
        }
        this.showLightBox = false;
        this.emitLightBoxActionConfirmed.emit(actionConfirmed);
    };
    return LightBoxComponent;
}());
__decorate([
    core_1.Input(),
    __metadata("design:type", LightBoxConfig)
], LightBoxComponent.prototype, "lightBoxConfig", void 0);
__decorate([
    core_1.Output(),
    __metadata("design:type", Object)
], LightBoxComponent.prototype, "emitLightBoxActionConfirmed", void 0);
LightBoxComponent = __decorate([
    core_1.Component({
        selector: 'ft-light-box',
        template: __webpack_require__("./src/app/shared/component/light-box/light-box.component.html"),
        styles: [__webpack_require__("./src/app/shared/component/light-box/light-box.component.css")]
    }),
    __metadata("design:paramtypes", [typeof (_a = typeof core_1.ChangeDetectorRef !== "undefined" && core_1.ChangeDetectorRef) === "function" && _a || Object])
], LightBoxComponent);
exports.LightBoxComponent = LightBoxComponent;
var LightBoxConfig = (function () {
    function LightBoxConfig() {
        this.Top = 500;
    }
    return LightBoxConfig;
}());
exports.LightBoxConfig = LightBoxConfig;
var LightBoxTypes = (function () {
    function LightBoxTypes() {
    }
    return LightBoxTypes;
}());
LightBoxTypes.Ok = 1;
LightBoxTypes.OkCancel = 2;
exports.LightBoxTypes = LightBoxTypes;
;
var _a;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/light-box.component.js.map

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
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var parameters_1 = __webpack_require__("./src/app/shared/service/api/parameters.ts");
var http_service_1 = __webpack_require__("./src/app/shared/service/api/http.service.ts");
var AreaService = (function () {
    function AreaService(httpService, ftHelperService) {
        this.httpService = httpService;
        this.ftHelperService = ftHelperService;
        this.version = this.ftHelperService.version();
    }
    AreaService.prototype.getAreaSearchByText = function (text, areaTypeId, shouldSearchRetreiveCoordinates, parentAreasToIncludeInResults) {
        var params = new parameters_1.Parameters(this.version);
        params.addPolygonAreaTypeId(areaTypeId);
        params.addNoCache();
        params.addIncludeCoordinates(shouldSearchRetreiveCoordinates);
        params.addParentAreasToIncludeInResults(parentAreasToIncludeInResults);
        params.addSearchText(text);
        return this.httpService.httpGet('api/area_search_by_text', params);
    };
    AreaService.prototype.getAreaSearchByProximity = function (easting, northing, areaTypeId) {
        var params = new parameters_1.Parameters(this.version);
        params.addAreaTypeId(areaTypeId);
        params.addEasting(easting);
        params.addNorthing(northing);
        return this.httpService.httpGet('api/area_search_by_proximity', params);
    };
    AreaService.prototype.getAreaAddressesByParentAreaCode = function (parentAreaCode, areaTypeId) {
        var params = new parameters_1.Parameters(this.version);
        params.addAreaTypeId(areaTypeId);
        params.addParentAreaCode(parentAreaCode);
        return this.httpService.httpGet('api/area_addresses/by_parent_area_code', params);
    };
    AreaService.prototype.getParentAreas = function (profileId) {
        var params = new parameters_1.Parameters(this.version);
        params.addProfileId(profileId);
        return this.httpService.httpGet('api/area_types/parent_area_types', params);
    };
    AreaService.prototype.getAreaTypes = function () {
        var params = new parameters_1.Parameters(this.version);
        return this.httpService.httpGet('api/area_types/with_data', params);
    };
    AreaService.prototype.getAreas = function (areaTypeId) {
        var params = new parameters_1.Parameters(this.version);
        params.addAreaTypeId(areaTypeId);
        params.addNoCache();
        return this.httpService.httpGet('api/areas/by_area_type', params);
    };
    return AreaService;
}());
AreaService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [typeof (_a = typeof http_service_1.HttpService !== "undefined" && http_service_1.HttpService) === "function" && _a || Object, typeof (_b = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _b || Object])
], AreaService);
exports.AreaService = AreaService;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/area.service.js.map

/***/ }),

/***/ "./src/app/shared/service/api/arealist.service.ts":
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
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var parameters_1 = __webpack_require__("./src/app/shared/service/api/parameters.ts");
var http_service_1 = __webpack_require__("./src/app/shared/service/api/http.service.ts");
var AreaListService = (function () {
    function AreaListService(httpService, ftHelperService) {
        this.httpService = httpService;
        this.ftHelperService = ftHelperService;
        this.version = this.ftHelperService.version();
    }
    AreaListService.prototype.getAreaLists = function (userId) {
        var params = new parameters_1.Parameters(this.version);
        params.addUserId(userId);
        params.addNoCache();
        return this.httpService.httpGet('api/arealists', params);
    };
    AreaListService.prototype.getAreaList = function (areaListId) {
        var params = new parameters_1.Parameters(this.version);
        params.addAreaListId(areaListId);
        params.addNoCache();
        return this.httpService.httpGet('api/arealist', params);
    };
    AreaListService.prototype.getAreaListByPublicId = function (publicId, userId) {
        var params = new parameters_1.Parameters(this.version);
        params.addPublicId(publicId);
        params.addUserId(userId);
        params.addNoCache();
        return this.httpService.httpGet('api/arealist/by_public_id', params);
    };
    AreaListService.prototype.getAreaCodesFromAreaListId = function (areaListId) {
        var params = new parameters_1.Parameters(this.version);
        params.addAreaListId(areaListId);
        params.addNoCache();
        return this.httpService.httpGet('api/arealist/areacodes', params);
    };
    AreaListService.prototype.getAreasFromAreaListAreaCodes = function (areaCodes) {
        var areaCodesCommaSeparated = areaCodes.join(',');
        var params = new parameters_1.Parameters(this.version);
        params.addAreaCodes(areaCodesCommaSeparated);
        return this.httpService.httpGet('api/areas/by_area_code', params);
    };
    AreaListService.prototype.getAreasWithAddressFromAreaListAreaCodes = function (areaCodes) {
        var areaCodesCommaSeparated = areaCodes.join(',');
        var params = new parameters_1.Parameters(this.version);
        params.addAreaCodes(areaCodesCommaSeparated);
        return this.httpService.httpGet('api/areas_with_addresses/by_area_code', params);
    };
    AreaListService.prototype.saveAreaList = function (formData) {
        return this.httpService.httpPost('api/arealist/save', formData);
    };
    AreaListService.prototype.updateAreaList = function (formData) {
        return this.httpService.httpPost('api/arealist/update', formData);
    };
    AreaListService.prototype.deleteAreaList = function (formData) {
        return this.httpService.httpPost('api/arealist/delete', formData);
    };
    AreaListService.prototype.copyAreaList = function (formData) {
        return this.httpService.httpPost('api/arealist/copy', formData);
    };
    return AreaListService;
}());
AreaListService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [typeof (_a = typeof http_service_1.HttpService !== "undefined" && http_service_1.HttpService) === "function" && _a || Object, typeof (_b = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _b || Object])
], AreaListService);
exports.AreaListService = AreaListService;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/arealist.service.js.map

/***/ }),

/***/ "./src/app/shared/service/api/content.service.ts":
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
var parameters_1 = __webpack_require__("./src/app/shared/service/api/parameters.ts");
var http_service_1 = __webpack_require__("./src/app/shared/service/api/http.service.ts");
var ContentService = (function () {
    function ContentService(httpService, ftHelperService) {
        this.httpService = httpService;
        this.ftHelperService = ftHelperService;
        this.version = this.ftHelperService.version();
    }
    ContentService.prototype.getContent = function (profileId, key) {
        var params = new parameters_1.Parameters(this.version);
        params.addProfileId(profileId);
        params.addKey(key);
        return this.httpService.httpGet('api/content', params);
    };
    return ContentService;
}());
ContentService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [typeof (_a = typeof http_service_1.HttpService !== "undefined" && http_service_1.HttpService) === "function" && _a || Object, typeof (_b = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _b || Object])
], ContentService);
exports.ContentService = ContentService;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/content.service.js.map

/***/ }),

/***/ "./src/app/shared/service/api/http.service.ts":
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
var Observable_1 = __webpack_require__("./node_modules/rxjs/_esm5/Observable.js");
var core_1 = __webpack_require__("./node_modules/@angular/core/@angular/core.es5.js");
var http_1 = __webpack_require__("./node_modules/@angular/http/@angular/http.es5.js");
__webpack_require__("./node_modules/rxjs/_esm5/add/operator/map.js");
__webpack_require__("./node_modules/rxjs/_esm5/add/operator/publishReplay.js");
__webpack_require__("./node_modules/rxjs/_esm5/add/operator/catch.js");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var HttpService = (function () {
    function HttpService(http, ftHelperService) {
        this.http = http;
        this.ftHelperService = ftHelperService;
        this.observables = {};
        this.baseUrl = ftHelperService.getURL().bridge;
    }
    HttpService.prototype.httpGet = function (serviceUrl, params) {
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
    HttpService.prototype.httpPost = function (serviceUrl, formData) {
        return this.http.post(this.baseUrl + serviceUrl, formData)
            .map(function (response) {
            return response;
        }).catch(this.handleError);
    };
    HttpService.prototype.handleError = function (error) {
        console.error(error);
        var errorMessage = 'AJAX call failed';
        return Observable_1.Observable.throw(errorMessage);
    };
    return HttpService;
}());
HttpService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [typeof (_a = typeof http_1.Http !== "undefined" && http_1.Http) === "function" && _a || Object, typeof (_b = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _b || Object])
], HttpService);
exports.HttpService = HttpService;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/http.service.js.map

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
var parameters_1 = __webpack_require__("./src/app/shared/service/api/parameters.ts");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var http_service_1 = __webpack_require__("./src/app/shared/service/api/http.service.ts");
/** Client library for multiple service controllers in WS:
 * - Data
 * - IndicatorMetadata
 */
var IndicatorService = (function () {
    function IndicatorService(httpService, ftHelperService) {
        this.httpService = httpService;
        this.ftHelperService = ftHelperService;
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
        return this.httpService.httpGet('api/latest_data/single_indicator_for_all_areas', params);
    };
    IndicatorService.prototype.getCategories = function (categoryTypeId) {
        var params = new parameters_1.Parameters(this.version);
        params.addCategoryTypeId(categoryTypeId);
        return this.httpService.httpGet('api/categories', params);
    };
    IndicatorService.prototype.getPopulationSummary = function (areaCode, areaTypeId) {
        var params = new parameters_1.Parameters(this.version);
        params.addAreaCode(areaCode);
        params.addAreaTypeId(areaTypeId);
        return this.httpService.httpGet('api/quinary_population_summary', params);
    };
    IndicatorService.prototype.getPopulation = function (areaCode, areaTypeId) {
        var params = new parameters_1.Parameters(this.version);
        params.addAreaCode(areaCode);
        params.addAreaTypeId(areaTypeId);
        return this.httpService.httpGet('api/quinary_population', params);
    };
    IndicatorService.prototype.getBenchmarkingMethod = function (benchmarkingMethodId) {
        var params = new parameters_1.Parameters(this.version);
        params.addId(benchmarkingMethodId);
        return this.httpService.httpGet('api/comparator_method', params);
    };
    IndicatorService.prototype.getIndicatorMetadataProperties = function () {
        var params = new parameters_1.Parameters(this.version);
        return this.httpService.httpGet('api/indicator_metadata_text_properties', params);
    };
    IndicatorService.prototype.getIndicatorStatisticsTrendsForSingleIndicator = function (indicatorId, sexId, ageId, childAreaTypeId, parentAreaCode) {
        var params = new parameters_1.Parameters(this.version);
        params.addIndicatorId(indicatorId);
        params.addSexId(sexId);
        params.addAgeId(ageId);
        params.addChildAreaTypeId(childAreaTypeId);
        params.addParentAreaCode(parentAreaCode);
        return this.httpService.httpGet('api/indicator_statistics/trends_for_single_indicator', params);
    };
    IndicatorService.prototype.getIndicatorStatistics = function (childAreaTypeId, parentAreaCode, profileId, groupId) {
        var params = new parameters_1.Parameters(this.version);
        params.addParentAreaCode(parentAreaCode);
        params.addChildAreaTypeId(childAreaTypeId);
        params.addGroupId(groupId);
        var method;
        if (this.search.isInSearchMode()) {
            method = 'by_indicator_id';
            this.addSearchParameters(params);
        }
        else {
            method = 'by_profile_id';
            params.addProfileId(profileId);
        }
        return this.httpService.httpGet('api/indicator_statistics/' + method, params);
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
        return this.httpService.httpGet(serviceURL, params);
    };
    IndicatorService.prototype.getIndicatorMetadataByIndicatorId = function (indicatorId, restrictToProfileIds) {
        var params = new parameters_1.Parameters(this.version);
        params.addIncludeSystemContent('no');
        params.addIncludeDefinition('yes');
        params.addIndicatorIds([indicatorId]);
        params.addRestrictToProfileIds(restrictToProfileIds);
        return this.httpService.httpGet('api/indicator_metadata/by_indicator_id', params);
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
        return this.httpService.httpGet(serviceURL, params);
    };
    IndicatorService.prototype.getGroupingSubheadings = function (areaTypeId, groupId) {
        var params = new parameters_1.Parameters(this.version);
        params.addAreaTypeId(areaTypeId);
        params.addGroupId(groupId);
        return this.httpService.httpGet('api/grouping_subheadings', params);
    };
    IndicatorService.prototype.addSearchParameters = function (params) {
        params.addIndicatorIds(this.search.getIndicatorIdList().getAllIds());
        params.addRestrictToProfileIds(this.search.getProfileIdsForSearch());
    };
    return IndicatorService;
}());
IndicatorService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [typeof (_a = typeof http_service_1.HttpService !== "undefined" && http_service_1.HttpService) === "function" && _a || Object, typeof (_b = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _b || Object])
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
    Parameters.prototype.addAreaCodes = function (areaCodes) {
        this.params.set('area_codes', areaCodes);
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
    Parameters.prototype.addNoCache = function () {
        this.params.set('no_cache', true.toString());
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
    Parameters.prototype.addNorthing = function (northing) {
        this.params.set('northing', northing.toString());
    };
    Parameters.prototype.addUserId = function (userId) {
        this.params.set('user_id', userId);
    };
    Parameters.prototype.addAreaListId = function (areaListId) {
        this.params.set('area_list_id', areaListId.toString());
    };
    Parameters.prototype.addPublicId = function (publicId) {
        this.params.set('public_id', publicId);
    };
    Parameters.prototype.addProfileKey = function (profileKey) {
        this.params.set('profile_key', profileKey);
    };
    Parameters.prototype.addFileName = function (fileName) {
        this.params.set('file_name', fileName);
    };
    Parameters.prototype.addTimePeriod = function (timePeriod) {
        this.params.set('time_period', timePeriod);
    };
    Parameters.prototype.addKey = function (key) {
        this.params.set('key', key);
    };
    return Parameters;
}());
exports.Parameters = Parameters;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/parameters.js.map

/***/ }),

/***/ "./src/app/shared/service/api/profile.service.ts":
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
var http_service_1 = __webpack_require__("./src/app/shared/service/api/http.service.ts");
var parameters_1 = __webpack_require__("./src/app/shared/service/api/parameters.ts");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var ProfileService = (function () {
    function ProfileService(httpService, ftHelperService) {
        this.httpService = httpService;
        this.ftHelperService = ftHelperService;
        this.version = this.ftHelperService.version();
    }
    ProfileService.prototype.areaTypesWithPdfs = function (profileId) {
        var params = new parameters_1.Parameters(this.version);
        params.addProfileId(profileId);
        return this.httpService.httpGet('api/profile/area_types_with_pdfs', params);
    };
    ProfileService.prototype.getIndicatorProfiles = function (indicatorIds) {
        var params = new parameters_1.Parameters(this.version);
        params.addIndicatorIds(indicatorIds);
        return this.httpService.httpGet('api/profiles_containing_indicators', params);
    };
    return ProfileService;
}());
ProfileService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [typeof (_a = typeof http_service_1.HttpService !== "undefined" && http_service_1.HttpService) === "function" && _a || Object, typeof (_b = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _b || Object])
], ProfileService);
exports.ProfileService = ProfileService;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/profile.service.js.map

/***/ }),

/***/ "./src/app/shared/service/api/ssrs-report.service.ts":
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
var parameters_1 = __webpack_require__("./src/app/shared/service/api/parameters.ts");
var http_service_1 = __webpack_require__("./src/app/shared/service/api/http.service.ts");
var SsrsReportService = (function () {
    function SsrsReportService(httpService, ftHelperService) {
        this.httpService = httpService;
        this.ftHelperService = ftHelperService;
        this.version = this.ftHelperService.version();
    }
    SsrsReportService.prototype.getSsrsReports = function (profileId) {
        var params = new parameters_1.Parameters(this.version);
        return this.httpService.httpGet('api/ssrs_reports/' + profileId, params);
    };
    return SsrsReportService;
}());
SsrsReportService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [typeof (_a = typeof http_service_1.HttpService !== "undefined" && http_service_1.HttpService) === "function" && _a || Object, typeof (_b = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _b || Object])
], SsrsReportService);
exports.SsrsReportService = SsrsReportService;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/ssrs-report.service.js.map

/***/ }),

/***/ "./src/app/shared/service/api/static-reports.service.ts":
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
var http_service_1 = __webpack_require__("./src/app/shared/service/api/http.service.ts");
var parameters_1 = __webpack_require__("./src/app/shared/service/api/parameters.ts");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var StaticReportsService = (function () {
    function StaticReportsService(httpService, ftHelperService) {
        this.httpService = httpService;
        this.ftHelperService = ftHelperService;
        this.version = this.ftHelperService.version();
    }
    StaticReportsService.prototype.doesStaticReportExist = function (profileKey, fileName, timePeriod) {
        var params = new parameters_1.Parameters(this.version);
        params.addProfileKey(profileKey);
        params.addFileName(fileName);
        if (timePeriod) {
            params.addTimePeriod(timePeriod);
        }
        return this.httpService.httpGet('api/static-reports/exists', params);
    };
    return StaticReportsService;
}());
StaticReportsService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [typeof (_a = typeof http_service_1.HttpService !== "undefined" && http_service_1.HttpService) === "function" && _a || Object, typeof (_b = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _b || Object])
], StaticReportsService);
exports.StaticReportsService = StaticReportsService;
var _a, _b;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/static-reports.service.js.map

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
var shared_1 = __webpack_require__("./src/app/shared/shared.ts");
var FTHelperService = (function () {
    function FTHelperService() {
    }
    FTHelperService.prototype.hasDataChanged = function (groupRoot) {
        return FTWrapper.hasDataChanged(groupRoot);
    };
    FTHelperService.prototype.getIndicatorNameTooltip = function (rootIndex, area) {
        return FTWrapper.getIndicatorNameTooltip(rootIndex, area);
    };
    FTHelperService.prototype.initBootstrapTooltips = function () {
        FTWrapper.initBootstrapTooltips();
    };
    FTHelperService.prototype.isSubnationalColumn = function () {
        return FTWrapper.isSubnationalColumn();
    };
    FTHelperService.prototype.exportTableAsImage = function (containerId, fileNamePrefix, legends) {
        FTWrapper.exportTableAsImage(containerId, fileNamePrefix, legends);
    };
    FTHelperService.prototype.getMarkerImageFromSignificance = function (significance, useRag, suffix, useQuintileColouring, indicatorId, sexId, ageId) {
        return FTWrapper.getMarkerImageFromSignificance(significance, useRag, suffix, useQuintileColouring, indicatorId, sexId, ageId);
    };
    FTHelperService.prototype.getArea = function (areaCode) {
        return FTWrapper.getArea(areaCode);
    };
    FTHelperService.prototype.getAreaName = function (areaCode) {
        return FTWrapper.getAreaName(areaCode);
    };
    FTHelperService.prototype.getAreaNameToDisplay = function (area) {
        return FTWrapper.getAreaNameToDisplay(area);
    };
    FTHelperService.prototype.getEnumParentDisplay = function () {
        return enumParentDisplay;
    };
    FTHelperService.prototype.getNationalComparator = function () {
        return FTWrapper.getNationalComparator();
    };
    FTHelperService.prototype.getParentTypeId = function () {
        return FTWrapper.getParentTypeId();
    };
    FTHelperService.prototype.getParentTypeName = function () {
        return FTWrapper.getParentTypeName();
    };
    FTHelperService.prototype.getAreaTypeId = function () {
        return FTWrapper.getAreaTypeId();
    };
    FTHelperService.prototype.getAreaTypeName = function () {
        return FTWrapper.getAreaTypeName();
    };
    FTHelperService.prototype.getAreaList = function () {
        return FTWrapper.getAreaList();
    };
    FTHelperService.prototype.getComparatorId = function () {
        return FTWrapper.getComparatorId();
    };
    FTHelperService.prototype.getComparatorById = function (comparatorId) {
        return FTWrapper.getComparatorById(comparatorId);
    };
    FTHelperService.prototype.getCurrentDomainName = function () {
        return FTWrapper.getCurrentDomainName();
    };
    FTHelperService.prototype.getCurrentComparator = function () {
        return FTWrapper.getCurrentComparator();
    };
    FTHelperService.prototype.getCurrentGroupRoot = function () {
        return FTWrapper.getGroopRoot();
    };
    FTHelperService.prototype.getGroupingSubheadings = function () {
        return FTWrapper.getGroupingSubheadings();
    };
    FTHelperService.prototype.getAllGroupRoots = function () {
        return groupRoots;
    };
    FTHelperService.prototype.getValueNotes = function () {
        return FTWrapper.getValueNotes();
    };
    FTHelperService.prototype.getValueNoteById = function (id) {
        return FTWrapper.getValueNoteById(id);
    };
    FTHelperService.prototype.formatCount = function (dataInfo) {
        return FTWrapper.formatCount(dataInfo);
    };
    FTHelperService.prototype.newCoreDataSetInfo = function (data) {
        return FTWrapper.newCoreDataSetInfo(data);
    };
    FTHelperService.prototype.newIndicatorFormatter = function (groupRoot, metadata, coreDataSet, indicatorStatsF) {
        return FTWrapper.newIndicatorFormatter(groupRoot, metadata, coreDataSet, indicatorStatsF);
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
    FTHelperService.prototype.getRegionalComparatorGrouping = function (root) {
        return FTWrapper.getRegionalComparatorGrouping(root);
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
    FTHelperService.prototype.goToBarChartPage = function (rootIndex) {
        FTWrapper.goToBarChartPage(rootIndex);
    };
    FTHelperService.prototype.goToMetadataPage = function (rootIndex) {
        FTWrapper.goToMetadataPage(rootIndex);
    };
    FTHelperService.prototype.goToAreaTrendsPage = function (rootIndex) {
        FTWrapper.goToAreaTrendsPage(rootIndex);
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
    FTHelperService.prototype.showDataQualityLegend = function () {
        FTWrapper.showDataQualityLegend();
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
    FTHelperService.prototype.isParentCountry = function (model) {
        return model.parentTypeId === shared_1.AreaTypeIds.Country;
    };
    FTHelperService.prototype.isParentUk = function () {
        return FTWrapper.getParentTypeId() === shared_1.AreaTypeIds.Uk;
    };
    FTHelperService.prototype.isNearestNeighbours = function (model) {
        switch (model.nearestNeighbour) {
            case undefined:
            case null:
            case "":
                return false;
            default:
                return true;
        }
    };
    FTHelperService.prototype.getProfileUrlKey = function () {
        return FTWrapper.getProfileUrlKey();
    };
    FTHelperService.prototype.setComparatorId = function (id) {
        FTWrapper.setComparatorId(id);
    };
    FTHelperService.prototype.getAreaMappingsForParentCode = function (key) {
        return FTWrapper.getAreaMappingsForParentCode(key);
    };
    FTHelperService.prototype.showTrendInfo = function () {
        return FTWrapper.showTrendInfo();
    };
    FTHelperService.prototype.lightboxShow = function (html, top, left, popupWidth) {
        FTWrapper.lightboxShow(html, top, left, popupWidth);
    };
    return FTHelperService;
}());
FTHelperService = __decorate([
    core_1.Injectable()
], FTHelperService);
exports.FTHelperService = FTHelperService;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/ftHelper.service.js.map

/***/ }),

/***/ "./src/app/shared/service/helper/light-box.service.ts":
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
var LightBoxService = (function () {
    function LightBoxService(ftHelperService) {
        this.ftHelperService = ftHelperService;
    }
    /**
     * Display a light box on the Fingertips data page
     */
    LightBoxService.prototype.display = function (lightBoxConfig) {
        // To enable a full screen lightbox on the data page
        var html = "<div style=\"padding:15px;\">\n    <h3>" + lightBoxConfig.Title + "</h3>\n    <div>\n    " + lightBoxConfig.Html + "\n    </div>\n    <div class=\"lightbox-button-box\">\n    <button class=\"btn btn-primary active lightbox-button\" onclick=\"lightbox.hide()\">OK</button>\n    </div>\n    </div>";
        var popupWidth = 500;
        var left = ($(window).width() - popupWidth) / 2;
        var top = lightBoxConfig.Top;
        this.ftHelperService.lightboxShow(html, top, left, popupWidth);
    };
    return LightBoxService;
}());
LightBoxService = __decorate([
    core_1.Injectable(),
    __metadata("design:paramtypes", [typeof (_a = typeof ftHelper_service_1.FTHelperService !== "undefined" && ftHelper_service_1.FTHelperService) === "function" && _a || Object])
], LightBoxService);
exports.LightBoxService = LightBoxService;
var _a;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/light-box.service.js.map

/***/ }),

/***/ "./src/app/shared/service/helper/ui.service.ts":
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
/** Wrapper for JQuery access to page elements not defined in Angular */
var UIService = (function () {
    function UIService() {
    }
    UIService.prototype.setScrollTop = function (scrollTop) {
        if (scrollTop) {
            $(window).scrollTop(scrollTop);
        }
    };
    UIService.prototype.getScrollTop = function () {
        return $(window).scrollTop();
    };
    UIService.prototype.toggleQuintileLegend = function ($element, useQuintileColouring) {
        if (!useQuintileColouring) {
            $element.hide();
        }
        else {
            $element.show();
        }
    };
    return UIService;
}());
UIService = __decorate([
    core_1.Injectable()
], UIService);
exports.UIService = UIService;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/ui.service.js.map

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
// Helper services 
var coreDataHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/coreDataHelper.service.ts");
var ftHelper_service_1 = __webpack_require__("./src/app/shared/service/helper/ftHelper.service.ts");
var light_box_service_1 = __webpack_require__("./src/app/shared/service/helper/light-box.service.ts");
var ui_service_1 = __webpack_require__("./src/app/shared/service/helper/ui.service.ts");
// Components
var indicator_header_component_1 = __webpack_require__("./src/app/shared/component/indicator-header/indicator-header.component.ts");
// API services
var http_service_1 = __webpack_require__("./src/app/shared/service/api/http.service.ts");
var static_reports_service_1 = __webpack_require__("./src/app/shared/service/api/static-reports.service.ts");
var profile_service_1 = __webpack_require__("./src/app/shared/service/api/profile.service.ts");
var content_service_1 = __webpack_require__("./src/app/shared/service/api/content.service.ts");
var ssrs_report_service_1 = __webpack_require__("./src/app/shared/service/api/ssrs-report.service.ts");
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
        ],
        providers: [coreDataHelper_service_1.CoreDataHelperService, ftHelper_service_1.FTHelperService, http_service_1.HttpService, profile_service_1.ProfileService,
            static_reports_service_1.StaticReportsService, light_box_service_1.LightBoxService, ssrs_report_service_1.SsrsReportService, content_service_1.ContentService, ui_service_1.UIService],
    })
], SharedModule);
exports.SharedModule = SharedModule;
//# sourceMappingURL=C:/fingertips/IndicatorsUI/MainUI/angular-app/src/shared.module.js.map

/***/ }),

/***/ "./src/app/shared/shared.ts":
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var _ = __webpack_require__("./node_modules/underscore/underscore.js");
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
AreaCodes.England = 'E92000001';
AreaCodes.Uk = 'UK0000000';
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
var ParentDisplay = (function () {
    function ParentDisplay() {
    }
    return ParentDisplay;
}());
ParentDisplay.NationalAndRegional = 0;
ParentDisplay.RegionalOnly = 1;
ParentDisplay.NationalOnly = 2;
exports.ParentDisplay = ParentDisplay;
;
/** Highcharts helper */
var HC = (function () {
    function HC() {
    }
    return HC;
}());
HC.Credits = { enabled: false };
HC.NoLineMarker = { enabled: false, symbol: 'x' };
exports.HC = HC;
var ComparatorIds = (function () {
    function ComparatorIds() {
    }
    return ComparatorIds;
}());
ComparatorIds.National = 4;
ComparatorIds.SubNational = 1;
exports.ComparatorIds = ComparatorIds;
var AreaTypeIds = (function () {
    function AreaTypeIds() {
    }
    return AreaTypeIds;
}());
AreaTypeIds.District = 1;
AreaTypeIds.MSOA = 3;
AreaTypeIds.Region = 6;
AreaTypeIds.Practice = 7;
AreaTypeIds.Ward = 8;
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
AreaTypeIds.Uk = 159;
exports.AreaTypeIds = AreaTypeIds;
;
var ProfileUrlKeys = (function () {
    function ProfileUrlKeys() {
    }
    return ProfileUrlKeys;
}());
ProfileUrlKeys.ChildHealthBehaviours = 'child-health-behaviours';
ProfileUrlKeys.DentalHealth = 'dental-health';
exports.ProfileUrlKeys = ProfileUrlKeys;
var TrendMarkerValue = (function () {
    function TrendMarkerValue() {
    }
    return TrendMarkerValue;
}());
TrendMarkerValue.Up = 1;
TrendMarkerValue.Down = 2;
TrendMarkerValue.NoChange = 3;
TrendMarkerValue.CannotCalculate = 4;
exports.TrendMarkerValue = TrendMarkerValue;
;
var IndicatorIds = (function () {
    function IndicatorIds() {
    }
    return IndicatorIds;
}());
IndicatorIds.QofListSize = 114;
IndicatorIds.QofPoints = 295;
IndicatorIds.LifeExpectancy = 650;
IndicatorIds.EthnicityEstimates = 1679;
IndicatorIds.DeprivationScore = 91872;
IndicatorIds.WouldRecommendPractice = 93438;
exports.IndicatorIds = IndicatorIds;
var CategoryTypeIds = (function () {
    function CategoryTypeIds() {
    }
    return CategoryTypeIds;
}());
CategoryTypeIds.HealthProfilesSSILimit = 5;
CategoryTypeIds.DeprivationDecileCCG2010 = 11;
CategoryTypeIds.DeprivationDecileGp2015 = 38;
CategoryTypeIds.DeprivationDecileCountyUA2015 = 39;
CategoryTypeIds.DeprivationDecileDistrictUA2015 = 40;
exports.CategoryTypeIds = CategoryTypeIds;
var ProfileIds = (function () {
    function ProfileIds() {
    }
    return ProfileIds;
}());
ProfileIds.SearchResults = 13;
ProfileIds.Tobacco = 18;
ProfileIds.Phof = 19;
ProfileIds.PracticeProfile = 20;
ProfileIds.PracticeProfileSupportingIndicators = 21;
ProfileIds.HealthProfiles = 26;
ProfileIds.CommonMentalHealthDisorders = 40;
ProfileIds.SevereMentalIllness = 41;
ProfileIds.Liver = 55;
ProfileIds.Dementia = 84;
ProfileIds.SuicidePrevention = 91;
ProfileIds.MentalHealthJsna = 98;
ProfileIds.ChildHealth = 105;
ProfileIds.ChildHealthBehaviours = 129;
ProfileIds.ChildrenYoungPeoplesWellBeing = 133;
exports.ProfileIds = ProfileIds;
var GroupIds = (function () {
    function GroupIds() {
    }
    return GroupIds;
}());
GroupIds.PracticeProfiles_Population = 1200006;
exports.GroupIds = GroupIds;
var ValueTypeIds = (function () {
    function ValueTypeIds() {
    }
    return ValueTypeIds;
}());
ValueTypeIds.Count = 7;
exports.ValueTypeIds = ValueTypeIds;
var CommaNumber = (function () {
    function CommaNumber(value) {
        this.value = value;
    }
    CommaNumber.prototype.commarate = function (value) {
        return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
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
ComparatorMethodIds.SingleOverlappingCIsForOneCiLevel = 1;
ComparatorMethodIds.SuicidePlan = 14;
ComparatorMethodIds.Quintiles = 15;
ComparatorMethodIds.Quartiles = 16;
ComparatorMethodIds.SingleOverlappingCIsForTwoCiLevels = 17;
exports.ComparatorMethodIds = ComparatorMethodIds;
var PolarityIds = (function () {
    function PolarityIds() {
    }
    return PolarityIds;
}());
PolarityIds.NotApplicable = -1;
PolarityIds.RAGLowIsGood = 0;
PolarityIds.RAGHighIsGood = 1;
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
                if (polarityId === PolarityIds.NotApplicable) {
                    switch (quintile) {
                        case 'quintile1': {
                            return Colour.bobQuintile1;
                        }
                        case 'quintile2': {
                            return Colour.bobQuintile2;
                        }
                        case 'quintile3': {
                            return Colour.bobQuintile3;
                        }
                        case 'quintile4': {
                            return Colour.bobQuintile4;
                        }
                        case 'quintile5': {
                            return Colour.bobQuintile5;
                        }
                    }
                }
                else {
                    switch (quintile) {
                        case 'quintile1': {
                            return Colour.ragQuintile1;
                        }
                        case 'quintile2': {
                            return Colour.ragQuintile2;
                        }
                        case 'quintile3': {
                            return Colour.ragQuintile3;
                        }
                        case 'quintile4': {
                            return Colour.ragQuintile4;
                        }
                        case 'quintile5': {
                            return Colour.ragQuintile5;
                        }
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
    Colour.getColorForQuintile = function (quintile, polarityId) {
        if (polarityId === PolarityIds.NotApplicable) {
            switch (quintile) {
                case 1:
                    return Colour.bobQuintile1;
                case 2:
                    return Colour.bobQuintile2;
                case 3:
                    return Colour.bobQuintile3;
                case 4:
                    return Colour.bobQuintile4;
                case 5:
                case 6:
                    return Colour.bobQuintile5;
                default:
                    return '#B0B0B2';
            }
        }
        else {
            switch (quintile) {
                case 1:
                    return Colour.ragQuintile1;
                case 2:
                    return Colour.ragQuintile2;
                case 3:
                    return Colour.ragQuintile3;
                case 4:
                    return Colour.ragQuintile4;
                case 5:
                case 6:
                    return Colour.ragQuintile5;
                default:
                    return '#B0B0B2';
            }
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
Colour.ragQuintile1 = '#DED3EC';
Colour.ragQuintile2 = '#BEA7DA';
Colour.ragQuintile3 = '#9E7CC8';
Colour.ragQuintile4 = '#7E50B6';
Colour.ragQuintile5 = '#5E25A4';
Colour.bobQuintile1 = '#254BA4';
Colour.bobQuintile2 = '#506EB6';
Colour.bobQuintile3 = '#7C93C8';
Colour.bobQuintile4 = '#A7B6DA';
Colour.bobQuintile5 = '#D3DBEC';
exports.Colour = Colour;
var ParameterBuilder = (function () {
    function ParameterBuilder() {
        this.keys = [];
        this.values = [];
    }
    //
    // Add a key value pair to the parameter list
    //
    ParameterBuilder.prototype.add = function (key, value) {
        this.keys.push(key);
        if (_.isArray(value)) {
            value = value.join(',');
        }
        this.values.push(value);
        return this;
    };
    ;
    // 
    // Generate the parameter string
    //
    ParameterBuilder.prototype.build = function () {
        var url = [];
        for (var index in this.keys) {
            if (index !== '0') {
                url.push('&');
            }
            url.push(this.keys[index], '=', this.values[index]);
        }
        // Do not prefix with '?' as JQuery ajax will do this
        return url.join('');
    };
    ;
    return ParameterBuilder;
}());
exports.ParameterBuilder = ParameterBuilder;
var GroupRootHelper = (function () {
    function GroupRootHelper(groupRoot) {
        this.groupRoot = groupRoot;
    }
    GroupRootHelper.prototype.findMatchingItemBySexAgeAndIndicatorId = function (items) {
        var groupRoot = this.groupRoot;
        return _.find(items, function (item) {
            return item.IID === groupRoot.IID &&
                item.Sex.Id === groupRoot.Sex.Id &&
                item.Age.Id === groupRoot.Age.Id;
        });
    };
    return GroupRootHelper;
}());
exports.GroupRootHelper = GroupRootHelper;
var CoreDataListHelper = (function () {
    function CoreDataListHelper(coreDataList) {
        this.coreDataList = coreDataList;
    }
    CoreDataListHelper.prototype.findByAreaCode = function (areaCode) {
        for (var _i = 0, _a = this.coreDataList; _i < _a.length; _i++) {
            var data = _a[_i];
            if (data.AreaCode === areaCode) {
                return data;
            }
        }
        return null;
    };
    return CoreDataListHelper;
}());
exports.CoreDataListHelper = CoreDataListHelper;
var AreaHelper = (function () {
    function AreaHelper(area) {
        this.area = area;
    }
    AreaHelper.prototype.getAreaNameToDisplay = function () {
        return this.getName(this.area.Name);
    };
    AreaHelper.prototype.getShortAreaNameToDisplay = function () {
        return this.getName(this.area.Short);
    };
    AreaHelper.prototype.getName = function (name) {
        if (this.area.AreaTypeId === AreaTypeIds.Practice) {
            return this.area.Code + ' - ' + name;
        }
        return name;
    };
    return AreaHelper;
}());
exports.AreaHelper = AreaHelper;
var SpineChartMinMaxLabel = (function () {
    function SpineChartMinMaxLabel() {
    }
    return SpineChartMinMaxLabel;
}());
SpineChartMinMaxLabel.DeriveFromLegendColours = 0;
SpineChartMinMaxLabel.LowestAndHighest = 1;
SpineChartMinMaxLabel.WorstAndBest = 2;
SpineChartMinMaxLabel.WorstLowestAndBestHighest = 3;
exports.SpineChartMinMaxLabel = SpineChartMinMaxLabel;
var SpineChartMinMaxLabelDescription = (function () {
    function SpineChartMinMaxLabelDescription() {
    }
    return SpineChartMinMaxLabelDescription;
}());
SpineChartMinMaxLabelDescription.Lowest = "Lowest";
SpineChartMinMaxLabelDescription.Highest = "Highest";
SpineChartMinMaxLabelDescription.Worst = "Worst";
SpineChartMinMaxLabelDescription.Best = "Best";
SpineChartMinMaxLabelDescription.WorstLowest = "Worst/ Lowest";
SpineChartMinMaxLabelDescription.BestHighest = "Best/ Highest";
exports.SpineChartMinMaxLabelDescription = SpineChartMinMaxLabelDescription;
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