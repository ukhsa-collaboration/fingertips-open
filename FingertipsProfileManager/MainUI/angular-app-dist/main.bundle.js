webpackJsonp([1,4],{

/***/ 222:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_http__ = __webpack_require__(274);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_add_operator_catch__ = __webpack_require__(440);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_add_operator_catch___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2_rxjs_add_operator_catch__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_rxjs_add_operator_map__ = __webpack_require__(441);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_rxjs_add_operator_map___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3_rxjs_add_operator_map__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ProfileService; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};




var ProfileService = (function () {
    function ProfileService(http) {
        this.http = http;
    }
    ProfileService.prototype.getProfiles = function () {
        return this.http.get('profile/user-profiles')
            .map(function (res) { return res.json(); });
    };
    ProfileService = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["c" /* Injectable */])(), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__angular_http__["b" /* Http */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__angular_http__["b" /* Http */]) === 'function' && _a) || Object])
    ], ProfileService);
    return ProfileService;
    var _a;
}());
//# sourceMappingURL=profile.service.js.map

/***/ }),

/***/ 232:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
Object.defineProperty(__webpack_exports__, "__esModule", { value: true });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_core_js_es6_symbol__ = __webpack_require__(301);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_core_js_es6_symbol___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0_core_js_es6_symbol__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_core_js_es6_object__ = __webpack_require__(294);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_core_js_es6_object___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1_core_js_es6_object__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_core_js_es6_function__ = __webpack_require__(290);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_core_js_es6_function___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2_core_js_es6_function__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_core_js_es6_parse_int__ = __webpack_require__(296);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_core_js_es6_parse_int___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3_core_js_es6_parse_int__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_core_js_es6_parse_float__ = __webpack_require__(295);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_core_js_es6_parse_float___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_4_core_js_es6_parse_float__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_core_js_es6_number__ = __webpack_require__(293);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5_core_js_es6_number___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_5_core_js_es6_number__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6_core_js_es6_math__ = __webpack_require__(292);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6_core_js_es6_math___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_6_core_js_es6_math__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7_core_js_es6_string__ = __webpack_require__(300);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7_core_js_es6_string___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_7_core_js_es6_string__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8_core_js_es6_date__ = __webpack_require__(289);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8_core_js_es6_date___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_8_core_js_es6_date__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9_core_js_es6_array__ = __webpack_require__(288);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9_core_js_es6_array___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_9_core_js_es6_array__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10_core_js_es6_regexp__ = __webpack_require__(298);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10_core_js_es6_regexp___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_10_core_js_es6_regexp__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_11_core_js_es6_map__ = __webpack_require__(291);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_11_core_js_es6_map___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_11_core_js_es6_map__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_12_core_js_es6_set__ = __webpack_require__(299);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_12_core_js_es6_set___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_12_core_js_es6_set__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_13_classlist_js__ = __webpack_require__(287);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_13_classlist_js___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_13_classlist_js__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_14_web_animations_js__ = __webpack_require__(449);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_14_web_animations_js___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_14_web_animations_js__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_15_core_js_es6_reflect__ = __webpack_require__(297);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_15_core_js_es6_reflect___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_15_core_js_es6_reflect__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_16_core_js_es7_reflect__ = __webpack_require__(302);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_16_core_js_es7_reflect___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_16_core_js_es7_reflect__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_17_zone_js_dist_zone__ = __webpack_require__(450);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_17_zone_js_dist_zone___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_17_zone_js_dist_zone__);
/**
 * This file includes polyfills needed by Angular and is loaded before the app.
 * You can add your own extra polyfills to this file.
 *
 * This file is divided into 2 sections:
 *   1. Browser polyfills. These are applied before loading ZoneJS and are sorted by browsers.
 *   2. Application imports. Files imported after ZoneJS that should be loaded before your main
 *      file.
 *
 * The current setup is for so-called "evergreen" browsers; the last versions of browsers that
 * automatically update themselves. This includes Safari >= 10, Chrome >= 55 (including Opera),
 * Edge >= 13 on the desktop, and iOS 10 and Chrome on mobile.
 *
 * Learn more in https://angular.io/docs/ts/latest/guide/browser-support.html
 */


















/***************************************************************************************************
 * APPLICATION IMPORTS
 */
/**
 * Date, currency, decimal and percent pipes.
 * Needed for: All but Chrome, Firefox, Edge, IE11 and Safari 10
 */
// import 'intl';  // Run `npm install --save intl`.
//# sourceMappingURL=polyfills.js.map

/***/ }),

/***/ 286:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_http__ = __webpack_require__(274);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_Rx__ = __webpack_require__(697);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_rxjs_Rx___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2_rxjs_Rx__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_rxjs_add_operator_catch__ = __webpack_require__(440);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3_rxjs_add_operator_catch___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_3_rxjs_add_operator_catch__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_rxjs_add_operator_map__ = __webpack_require__(441);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4_rxjs_add_operator_map___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_4_rxjs_add_operator_map__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ReportsService; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};





var ReportsService = (function () {
    function ReportsService(http) {
        this.http = http;
        this.baseUrl = 'api/reports';
    }
    ReportsService.prototype.getReports = function () {
        return this.http.get(this.baseUrl)
            .map(function (res) { return res.json(); });
    };
    ReportsService.prototype.getReportById = function (id) {
        return this.http
            .get(this.baseUrl + '/' + id)
            .map(function (response) { return response.json(); });
    };
    ReportsService.prototype.deleteReportById = function (id) {
        return this.http
            .delete(this.baseUrl + '/' + id);
    };
    ReportsService.prototype.saveReport = function (report) {
        var headers = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["c" /* Headers */]({ 'Content-Type': 'application/json;charset=UTF-8' });
        var options = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["d" /* RequestOptions */]({ headers: headers });
        return this.http.post(this.baseUrl + '/new', JSON.stringify(report), options);
    };
    ReportsService.prototype.updateReport = function (report) {
        var headers = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["c" /* Headers */]({ 'Content-Type': 'application/json;charset=UTF-8' });
        var options = new __WEBPACK_IMPORTED_MODULE_1__angular_http__["d" /* RequestOptions */]({ headers: headers });
        return this.http.put(this.baseUrl + '/new', JSON.stringify(report), options);
    };
    ReportsService.prototype.handleError = function (error) {
        console.log('api error', error);
        return __WEBPACK_IMPORTED_MODULE_2_rxjs_Rx__["Observable"].throw(error);
    };
    ReportsService = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["c" /* Injectable */])(), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__angular_http__["b" /* Http */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__angular_http__["b" /* Http */]) === 'function' && _a) || Object])
    ], ReportsService);
    return ReportsService;
    var _a;
}());
//# sourceMappingURL=reports.service.js.map

/***/ }),

/***/ 565:
/***/ (function(module, exports) {

function webpackEmptyContext(req) {
	throw new Error("Cannot find module '" + req + "'.");
}
webpackEmptyContext.keys = function() { return []; };
webpackEmptyContext.resolve = webpackEmptyContext;
module.exports = webpackEmptyContext;
webpackEmptyContext.id = 565;


/***/ }),

/***/ 566:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
Object.defineProperty(__webpack_exports__, "__esModule", { value: true });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_platform_browser_dynamic__ = __webpack_require__(652);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__app_app_module__ = __webpack_require__(673);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__environments_environment__ = __webpack_require__(680);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__polyfills__ = __webpack_require__(232);





if (__WEBPACK_IMPORTED_MODULE_3__environments_environment__["a" /* environment */].production) {
    __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["a" /* enableProdMode */])();
}
__webpack_require__.i(__WEBPACK_IMPORTED_MODULE_1__angular_platform_browser_dynamic__["a" /* platformBrowserDynamic */])().bootstrapModule(__WEBPACK_IMPORTED_MODULE_2__app_app_module__["a" /* AppModule */]);
//# sourceMappingURL=main.js.map

/***/ }),

/***/ 672:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return AppComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};

var AppComponent = (function () {
    function AppComponent() {
        this.title = 'FPM';
    }
    AppComponent.prototype.ngOnInit = function () {
    };
    AppComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["U" /* Component */])({
            selector: 'app-root',
            template: __webpack_require__(689),
            styles: [__webpack_require__(682)]
        }), 
        __metadata('design:paramtypes', [])
    ], AppComponent);
    return AppComponent;
}());
//# sourceMappingURL=app.component.js.map

/***/ }),

/***/ 673:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_platform_browser__ = __webpack_require__(278);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__angular_forms__ = __webpack_require__(643);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__angular_http__ = __webpack_require__(274);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__services_reports_service__ = __webpack_require__(286);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__services_profile_service__ = __webpack_require__(222);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_6__app_component__ = __webpack_require__(672);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_7__reports_reports_component__ = __webpack_require__(678);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_8__shared_profile_list_profile_list_component__ = __webpack_require__(679);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_9__reports_report_parameters_report_parameters_component__ = __webpack_require__(676);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_10__reports_report_profiles_report_profiles_component__ = __webpack_require__(677);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_11__reports_report_list_view_report_list_view_component__ = __webpack_require__(675);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_12__reports_report_edit_view_report_edit_view_component__ = __webpack_require__(674);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return AppModule; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};













var AppModule = (function () {
    function AppModule() {
    }
    AppModule = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_1__angular_core__["b" /* NgModule */])({
            declarations: [
                __WEBPACK_IMPORTED_MODULE_6__app_component__["a" /* AppComponent */],
                __WEBPACK_IMPORTED_MODULE_7__reports_reports_component__["a" /* ReportsComponent */],
                __WEBPACK_IMPORTED_MODULE_8__shared_profile_list_profile_list_component__["a" /* ProfileListComponent */],
                __WEBPACK_IMPORTED_MODULE_9__reports_report_parameters_report_parameters_component__["a" /* ReportParametersComponent */],
                __WEBPACK_IMPORTED_MODULE_10__reports_report_profiles_report_profiles_component__["a" /* ReportProfilesComponent */],
                __WEBPACK_IMPORTED_MODULE_11__reports_report_list_view_report_list_view_component__["a" /* ReportListViewComponent */],
                __WEBPACK_IMPORTED_MODULE_12__reports_report_edit_view_report_edit_view_component__["a" /* ReportEditViewComponent */]
            ],
            imports: [
                __WEBPACK_IMPORTED_MODULE_0__angular_platform_browser__["a" /* BrowserModule */],
                __WEBPACK_IMPORTED_MODULE_2__angular_forms__["a" /* FormsModule */],
                __WEBPACK_IMPORTED_MODULE_3__angular_http__["a" /* HttpModule */]
            ],
            providers: [
                __WEBPACK_IMPORTED_MODULE_4__services_reports_service__["a" /* ReportsService */],
                __WEBPACK_IMPORTED_MODULE_5__services_profile_service__["a" /* ProfileService */]
            ],
            bootstrap: [__WEBPACK_IMPORTED_MODULE_6__app_component__["a" /* AppComponent */]]
        }), 
        __metadata('design:paramtypes', [])
    ], AppModule);
    return AppModule;
}());
//# sourceMappingURL=app.module.js.map

/***/ }),

/***/ 674:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__services_reports_service__ = __webpack_require__(286);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ReportEditViewComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var ReportEditViewComponent = (function () {
    function ReportEditViewComponent(service) {
        this.service = service;
        this.report = { id: null, name: '', file: '', parameters: [], profiles: [], notes: '' };
        this.selectedParameters = [];
        this.selectedProfiles = [];
        this.reportName = '';
        this.profiles = [];
        this.getReportViewStatus = new __WEBPACK_IMPORTED_MODULE_0__angular_core__["G" /* EventEmitter */]();
    }
    ReportEditViewComponent.prototype.ngOnInit = function () {
        var _this = this;
        if (this.editProfileId != null) {
            this.service.getReportById(this.editProfileId)
                .subscribe(function (data) {
                _this.report = data;
            }), function (error) { return _this.errorMessage = error; };
        }
    };
    ReportEditViewComponent.prototype.saveReport = function () {
        var _this = this;
        if (this.isFormValid()) {
            if (this.editProfileId != null) {
                this.service.updateReport(this.report).subscribe(function (data) {
                    _this.getReportViewStatus.emit(2);
                });
            }
            else {
                this.service.saveReport(this.report).subscribe(function (data) {
                    _this.getReportViewStatus.emit(2);
                });
            }
        }
    };
    ReportEditViewComponent.prototype.cancelReport = function () {
        // TODO : reset the model
        this.getReportViewStatus.emit(2);
    };
    ReportEditViewComponent.prototype.getProfiles = function (profiles) {
        this.selectedProfiles = profiles;
    };
    ReportEditViewComponent.prototype.getParameters = function (parameters) {
        this.selectedParameters = parameters;
    };
    ReportEditViewComponent.prototype.deleteReport = function (id) {
        var _this = this;
        this.service.deleteReportById(id)
            .subscribe(function (data) {
            _this.getReportViewStatus.emit(2);
        });
    };
    ReportEditViewComponent.prototype.isFormValid = function () {
        var isValid = false;
        if (!this.report.name) {
            this.validationError = 'Report name is required';
        }
        else if (this.report.profiles.length == 0) {
            this.validationError = 'At least one profile is required ';
        }
        else {
            isValid = true;
        }
        return isValid;
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["T" /* Output */])(), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["G" /* EventEmitter */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["G" /* EventEmitter */]) === 'function' && _a) || Object)
    ], ReportEditViewComponent.prototype, "getReportViewStatus", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["w" /* Input */])(), 
        __metadata('design:type', Number)
    ], ReportEditViewComponent.prototype, "editProfileId", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["w" /* Input */])(), 
        __metadata('design:type', Object)
    ], ReportEditViewComponent.prototype, "userProfilesHash", void 0);
    ReportEditViewComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["U" /* Component */])({
            selector: 'app-report-edit-view',
            template: __webpack_require__(690),
            styles: [__webpack_require__(683)]
        }), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__services_reports_service__["a" /* ReportsService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__services_reports_service__["a" /* ReportsService */]) === 'function' && _b) || Object])
    ], ReportEditViewComponent);
    return ReportEditViewComponent;
    var _a, _b;
}());
//# sourceMappingURL=report-edit-view.component.js.map

/***/ }),

/***/ 675:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__services_reports_service__ = __webpack_require__(286);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ReportListViewComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var ReportListViewComponent = (function () {
    function ReportListViewComponent(service) {
        this.service = service;
        this.getListViewStatus = new __WEBPACK_IMPORTED_MODULE_0__angular_core__["G" /* EventEmitter */]();
    }
    ReportListViewComponent.prototype.ngOnInit = function () {
    };
    ReportListViewComponent.prototype.ngOnChanges = function () {
        var _this = this;
        this.service.getReports().subscribe(function (data) {
            _this.reports = data;
        });
    };
    ReportListViewComponent.prototype.deleteReport = function (event, index) {
        var report = this.reports[index];
        this.service.deleteReportById(report.id);
        this.reports.splice(index, 1);
    };
    ReportListViewComponent.prototype.newReport = function () {
        this.getListViewStatus.emit({ status: 0, profileId: null });
    };
    ReportListViewComponent.prototype.editReport = function (id) {
        this.editProfileId = id;
        this.getListViewStatus.emit({ status: 1, profileId: id });
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["w" /* Input */])(), 
        __metadata('design:type', Object)
    ], ReportListViewComponent.prototype, "userProfilesHash", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["T" /* Output */])(), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["G" /* EventEmitter */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["G" /* EventEmitter */]) === 'function' && _a) || Object)
    ], ReportListViewComponent.prototype, "getListViewStatus", void 0);
    ReportListViewComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["U" /* Component */])({
            selector: 'app-report-list-view',
            template: __webpack_require__(691),
            styles: [__webpack_require__(684)]
        }), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__services_reports_service__["a" /* ReportsService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__services_reports_service__["a" /* ReportsService */]) === 'function' && _b) || Object])
    ], ReportListViewComponent);
    return ReportListViewComponent;
    var _a, _b;
}());
//# sourceMappingURL=report-list-view.component.js.map

/***/ }),

/***/ 676:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_underscore__ = __webpack_require__(564);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_underscore___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1_underscore__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ReportParametersComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var ReportParametersComponent = (function () {
    function ReportParametersComponent() {
        this.parameters = ["areaCode", "areaTypeId", "groupId", "parentCode", "parentTypeId"];
        this.selectedParameters = [];
        this.getParameters = new __WEBPACK_IMPORTED_MODULE_0__angular_core__["G" /* EventEmitter */]();
    }
    ReportParametersComponent.prototype.ngOnInit = function () {
    };
    ReportParametersComponent.prototype.addParameter = function () {
        if (!__WEBPACK_IMPORTED_MODULE_1_underscore__["contains"](this.selectedParameters, this.selectedParameter) && this.selectedParameter) {
            this.selectedParameters.push(this.selectedParameter);
            this.getParameters.emit(this.selectedParameters);
        }
    };
    ReportParametersComponent.prototype.removeParameter = function (index) {
        this.selectedParameters.splice(index, 1);
        this.getParameters.emit(this.selectedParameters);
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["w" /* Input */])(), 
        __metadata('design:type', Array)
    ], ReportParametersComponent.prototype, "selectedParameters", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["T" /* Output */])(), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["G" /* EventEmitter */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["G" /* EventEmitter */]) === 'function' && _a) || Object)
    ], ReportParametersComponent.prototype, "getParameters", void 0);
    ReportParametersComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["U" /* Component */])({
            selector: 'app-report-parameters',
            template: __webpack_require__(692),
            styles: [__webpack_require__(685)]
        }), 
        __metadata('design:paramtypes', [])
    ], ReportParametersComponent);
    return ReportParametersComponent;
    var _a;
}());
//# sourceMappingURL=report-parameters.component.js.map

/***/ }),

/***/ 677:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__services_profile_service__ = __webpack_require__(222);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_underscore__ = __webpack_require__(564);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2_underscore___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_2_underscore__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ReportProfilesComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};



var ReportProfilesComponent = (function () {
    function ReportProfilesComponent(service) {
        this.service = service;
        this.getProfiles = new __WEBPACK_IMPORTED_MODULE_0__angular_core__["G" /* EventEmitter */]();
    }
    ReportProfilesComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.service.getProfiles()
            .subscribe(function (data) {
            _this.userProfiles = data;
        });
    };
    ReportProfilesComponent.prototype.onProfileChange = function (event) {
        this.selectedProfile = event;
    };
    ReportProfilesComponent.prototype.addProfile = function () {
        // We need to convert is to number please see the link
        // https://stackoverflow.com/questions/39562430/angular2-object-property-typed-as-number-changes-to-string
        var profileId = parseInt(this.selectedProfile);
        if (!__WEBPACK_IMPORTED_MODULE_2_underscore__["contains"](this.selectedProfiles, profileId) && profileId) {
            this.selectedProfiles.push(profileId);
            this.getProfiles.emit(this.selectedProfiles);
        }
    };
    ReportProfilesComponent.prototype.removeProfile = function (index) {
        this.selectedProfiles.splice(index, 1);
        this.getProfiles.emit(this.selectedProfiles);
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["w" /* Input */])(), 
        __metadata('design:type', Object)
    ], ReportProfilesComponent.prototype, "userProfilesHash", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["w" /* Input */])(), 
        __metadata('design:type', Array)
    ], ReportProfilesComponent.prototype, "selectedProfiles", void 0);
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["T" /* Output */])(), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["G" /* EventEmitter */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["G" /* EventEmitter */]) === 'function' && _a) || Object)
    ], ReportProfilesComponent.prototype, "getProfiles", void 0);
    ReportProfilesComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["U" /* Component */])({
            selector: 'app-report-profiles',
            template: __webpack_require__(693),
            styles: [__webpack_require__(686)]
        }), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__services_profile_service__["a" /* ProfileService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__services_profile_service__["a" /* ProfileService */]) === 'function' && _b) || Object])
    ], ReportProfilesComponent);
    return ReportProfilesComponent;
    var _a, _b;
}());
//# sourceMappingURL=report-profiles.component.js.map

/***/ }),

/***/ 678:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__services_profile_service__ = __webpack_require__(222);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ReportsComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var ReportsComponent = (function () {
    function ReportsComponent(profileService) {
        this.profileService = profileService;
        this.userProfilesHash = {};
        this.isInit = false;
        // TODO: should have status enum
        this.StatusNew = 0;
        this.StatusEdit = 1;
        this.StatusList = 2;
        this.status = this.StatusList;
    }
    ReportsComponent.prototype.ngOnInit = function () {
        this.getUserProfiles();
        this.isInit = true;
    };
    ReportsComponent.prototype.getReportViewStatus = function (status) {
        this.status = status;
    };
    ReportsComponent.prototype.getListViewStatus = function (listViewState) {
        this.status = listViewState.status;
        this.editProfileId = listViewState.profileId;
    };
    ReportsComponent.prototype.getUserProfiles = function () {
        var _this = this;
        this.profileService.getProfiles()
            .subscribe(function (data) {
            _this.userProfiles = data;
            _this.buildUserProfileHash();
        });
    };
    ReportsComponent.prototype.buildUserProfileHash = function () {
        var _this = this;
        if (this.userProfiles != null) {
            this.userProfiles.forEach(function (profile) {
                _this.userProfilesHash[profile.Id] = profile;
            });
        }
    };
    ReportsComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["U" /* Component */])({
            selector: 'app-reports',
            template: __webpack_require__(694),
            styles: [__webpack_require__(687)]
        }), 
        __metadata('design:paramtypes', [(typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_1__services_profile_service__["a" /* ProfileService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__services_profile_service__["a" /* ProfileService */]) === 'function' && _a) || Object])
    ], ReportsComponent);
    return ReportsComponent;
    var _a;
}());
//# sourceMappingURL=reports.component.js.map

/***/ }),

/***/ 679:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__angular_core__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__services_profile_service__ = __webpack_require__(222);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return ProfileListComponent; });
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};


var ProfileListComponent = (function () {
    function ProfileListComponent(service) {
        var _this = this;
        this.service = service;
        this.getSelectedProfile = new __WEBPACK_IMPORTED_MODULE_0__angular_core__["G" /* EventEmitter */]();
        this.service.getProfiles().subscribe(function (data) { return _this.profiles = data; });
    }
    ProfileListComponent.prototype.ngOnInit = function () {
    };
    ProfileListComponent.prototype.profileChange = function (event) {
        this.getSelectedProfile.emit(this.selectedProfile);
    };
    __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["T" /* Output */])(), 
        __metadata('design:type', (typeof (_a = typeof __WEBPACK_IMPORTED_MODULE_0__angular_core__["G" /* EventEmitter */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_0__angular_core__["G" /* EventEmitter */]) === 'function' && _a) || Object)
    ], ProfileListComponent.prototype, "getSelectedProfile", void 0);
    ProfileListComponent = __decorate([
        __webpack_require__.i(__WEBPACK_IMPORTED_MODULE_0__angular_core__["U" /* Component */])({
            selector: 'app-profile-list',
            template: __webpack_require__(695),
            styles: [__webpack_require__(688)]
        }), 
        __metadata('design:paramtypes', [(typeof (_b = typeof __WEBPACK_IMPORTED_MODULE_1__services_profile_service__["a" /* ProfileService */] !== 'undefined' && __WEBPACK_IMPORTED_MODULE_1__services_profile_service__["a" /* ProfileService */]) === 'function' && _b) || Object])
    ], ProfileListComponent);
    return ProfileListComponent;
    var _a, _b;
}());
//# sourceMappingURL=profile-list.component.js.map

/***/ }),

/***/ 680:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return environment; });
// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.
var environment = {
    production: false
};
//# sourceMappingURL=environment.js.map

/***/ }),

/***/ 682:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(79)();
// imports


// module
exports.push([module.i, "", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 683:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(79)();
// imports


// module
exports.push([module.i, "", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 684:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(79)();
// imports


// module
exports.push([module.i, "", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 685:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(79)();
// imports


// module
exports.push([module.i, "", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 686:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(79)();
// imports


// module
exports.push([module.i, "", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 687:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(79)();
// imports


// module
exports.push([module.i, "", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 688:
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(79)();
// imports


// module
exports.push([module.i, "", ""]);

// exports


/*** EXPORTS FROM exports-loader ***/
module.exports = module.exports.toString();

/***/ }),

/***/ 689:
/***/ (function(module, exports) {

module.exports = "<app-reports></app-reports>"

/***/ }),

/***/ 690:
/***/ (function(module, exports) {

module.exports = "<div *ngIf=\"validationError\" class=\"alert alert-warning\">\r\n  <strong>Warning!</strong> {{validationError}}\r\n</div>\r\n<div class=\"col-lg-12 pull-right\">\r\n  <div class=\"pull-left\">    \r\n    <button class=\"btn btn-primary\" (click)=\"saveReport()\">Save</button>    \r\n    <button class=\"btn btn-default\" (click)=\"cancelReport()\">Cancel</button>\r\n  </div>\r\n  <div class=\"pull-right\">\r\n    <button *ngIf=\"editProfileId\" class=\"btn btn-danger\" (click)=\"deleteReport(editProfileId)\">Delete</button>\r\n  </div>\r\n  <br/>\r\n  <br/>\r\n  <br/>\r\n</div>\r\n<div >\r\n  <form class=\"form-horizontal\">\r\n    <!-- Notes -->\r\n    <div class=\"form-group\">\r\n      <label for=\"reportName\" class=\"control-label col-xs-2\">Report Name</label>\r\n      <div class=\"col-xs-8\">\r\n        <input name=\"reportName\"  [(ngModel)]=\"report.name\"  class=\"form-control\" type=\"text\" />\r\n      </div>\r\n    </div>\r\n    <!-- Filename -->\r\n    <div class=\"form-group\">\r\n      <label for=\"fileName\" class=\"control-label col-xs-2\">File Name</label>\r\n      <div class=\"col-xs-8\">\r\n        <input name=\"fileName\"  [(ngModel)]=\"report.file\"  class=\"form-control\" type=\"text\" />\r\n      </div>\r\n    </div>\r\n     <!-- Notes -->\r\n     <div class=\"form-group\">\r\n      <label for=\"notes\" class=\"control-label col-xs-2\">Notes</label>\r\n      <div class=\"col-xs-8\">\r\n        <textarea  name=\"notes\"  [(ngModel)]=\"report.notes\"  class=\"form-control\" type=\"text\"></textarea>\r\n      </div>\r\n    </div>\r\n    <br/>\r\n    <br/>\r\n    <!--Profiles -->\r\n    <app-report-profiles (getProfiles)=\"getProfiles($event)\" [selectedProfiles]=\"report.profiles\" [userProfilesHash]=\"userProfilesHash\" ></app-report-profiles>\r\n    <br/>\r\n    <br/>\r\n    \r\n    <!--Parameters -->\r\n    <app-report-parameters (getParameters)=\"getParameters($event)\" [selectedParameters]=\"report.parameters\"></app-report-parameters>\r\n  </form>\r\n</div>\r\n\r\n "

/***/ }),

/***/ 691:
/***/ (function(module, exports) {

module.exports = "<div class=\"col-lg-12\">\r\n  <div class=\"pull-right\">\r\n    <button class=\"pull-right btn btn-primary\" (click)=\"newReport($event)\" name=\"newReportButton\">New Report</button>\r\n  </div>\r\n</div>\r\n<br>\r\n<div *ngIf=\"status == StatusList\">\r\n  <br />\r\n  <table class=\"table  table-striped table-bordered table-hover\" name=\"reportListTable\">\r\n    <thead>\r\n      <tr>\r\n        <th>Name</th>\r\n        <th>Profiles</th>\r\n        <th>Parameters</th>\r\n      </tr>\r\n    </thead>\r\n    <tbody>\r\n      <tr *ngFor=\"let r of reports; let i = index\" [attr.data-index]=\"i\">        \r\n        <td><span class=\"cursor-pointer btn-link\" (click)=\"editReport(r.id)\">{{r.name}}</span></td>\r\n        <td>\r\n          <span *ngFor=\"let p of r.profiles; let i = index\">\r\n              {{userProfilesHash[p]?.Name}}<br>\r\n          </span>\r\n        </td>\r\n        <td>\r\n          <span *ngFor=\"let param of r.parameters; let isLast=last\">\r\n              {{param}}{{isLast ? '' : ', '}}\r\n          </span>\r\n        </td>        \r\n      </tr>\r\n    </tbody>\r\n  </table>\r\n</div>"

/***/ }),

/***/ 692:
/***/ (function(module, exports) {

module.exports = "<div class=\"form-group\">\r\n    <label for=\"newParameter\" class=\"control-label col-xs-2\">Parameter</label>\r\n    <div class=\"col-xs-4\">\r\n        <select class=\"form-control\" [(ngModel)]=\"selectedParameter\">\r\n                    <option *ngFor=\"let parameter of parameters\" [value]=\"parameter\">{{parameter}}</option>\r\n        </select>\r\n    </div>\r\n    <div>\r\n        <button class=\"btn\" (click)=\"addParameter()\">Add</button>\r\n    </div>\r\n</div>\r\n<!-- Parameters for the report -->\r\n<div class=\"row col-md-offset-2\">\r\n    <div *ngIf=\"selectedParameters?.length > 0\">\r\n        <table class=\"table\">\r\n            <thead>\r\n                <tr>\r\n                    <th>\r\n                        List of parameters\r\n                    </th>\r\n                    <th></th>\r\n                </tr>\r\n            </thead>\r\n            <tbody>\r\n                <tr *ngFor=\"let parameter of selectedParameters; let i = index\">\r\n                    <td>{{parameter}}</td>\r\n                    <td><button (click)=\"removeParameter(i)\" class=\"btn btn-danger\">Remove</button></td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n    </div>\r\n</div>"

/***/ }),

/***/ 693:
/***/ (function(module, exports) {

module.exports = "<div class=\"form-group\">\r\n    <label for=\"profiles\" class=\"control-label col-xs-2\">Profiles</label>\r\n    <div class=\"col-xs-4\">\r\n        <app-profile-list (getSelectedProfile)=\"onProfileChange($event)\"></app-profile-list>\r\n    </div>\r\n    <div>\r\n        <button class=\"btn\" (click)=\"addProfile()\">Add</button>\r\n    </div>\r\n</div>\r\n<div class=\"row col-md-offset-2\">\r\n    <div *ngIf=\"selectedProfiles?.length > 0\">       \r\n        <table class=\"table\">\r\n            <thead>\r\n                <tr>\r\n                    <th> List of profiles</th>\r\n                    <th></th>\r\n                </tr>\r\n            </thead>\r\n            <tbody>\r\n            <tr *ngFor=\"let profile of selectedProfiles; let i = index\">\r\n                <td>\r\n                     {{ userProfilesHash[profile].Name}}  \r\n                 </td>\r\n                <td>\r\n                    <button (click)=\"removeProfile(i)\" class=\"btn btn-danger\">Remove</button>\r\n                </td>\r\n            </tr>\r\n            </tbody>\r\n        </table>\r\n    </div>\r\n</div>"

/***/ }),

/***/ 694:
/***/ (function(module, exports) {

module.exports = "<div *ngIf=\"isInit\">\r\n  <br/>\r\n  <!--List-->\r\n  <div *ngIf=\"status== StatusList\">\r\n    <app-report-list-view (getListViewStatus)=\"getListViewStatus($event)\" [userProfilesHash]=\"userProfilesHash\"></app-report-list-view>\r\n  </div>\r\n  <!--Edit-->\r\n  <div *ngIf=\"status== StatusEdit || status== StatusNew\">\r\n    <app-report-edit-view (getReportViewStatus)=\"getReportViewStatus($event)\" [editProfileId]=\"editProfileId\" [userProfilesHash]=\"userProfilesHash\"></app-report-edit-view>\r\n  </div>\r\n</div>"

/***/ }),

/***/ 695:
/***/ (function(module, exports) {

module.exports = "<select class=\"form-control\" [(ngModel)]=\"selectedProfile\" (change)=\"profileChange($event)\">\r\n  <option *ngFor=\"let p of profiles\" [value]=\"p.Id\" >{{p.Name}}</option>\r\n</select>"

/***/ }),

/***/ 974:
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__(566);


/***/ })

},[974]);
//# sourceMappingURL=main.bundle.js.map