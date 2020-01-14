import { Component, ChangeDetectorRef, OnInit } from '@angular/core';
import { FormControl, FormGroup, } from '@angular/forms';
import { AreaService } from '../../shared/service/api/area.service';
import { AreaListService } from '../../shared/service/api/arealist.service';
import { Observable, forkJoin } from 'rxjs';
import { Area, AreaType, AreaList, AreaAddress } from '../../typings/FT';
import { AreaTypeIds } from '../../shared/constants';
import { Practice } from '../../map/practice-search/practice-search';
import { LightBoxConfig, LightBoxTypes } from '../../shared/component/light-box/light-box';
import { isDefined } from '@angular/compiler/src/util';

@Component({
    selector: 'ft-arealist-manage',
    templateUrl: './arealist-manage.component.html',
    styleUrls: ['./arealist-manage.component.css'],
    providers: [AreaListService]
})

export class ArealistManageComponent implements OnInit {
    // Initialise variables
    userId: string;
    areaListId: number;
    publicId: string;
    actionType: string;

    practicesAsList: boolean;

    isInitialised = false;
    map: google.maps.Map;

    // Area type ID in URL
    initialAreaTypeId: number;
    areaTypeId: number;
    areaTypeName: string;
    currentAreaCode: string;
    areaCodeColour: Map<string, string> = new Map();
    areaCodeColourValue: Map<string, MapColourData> = new Map();
    refreshColour = 0;
    isBoundaryNotSupported = false;
    mapColourSelectedValue = 'benchmark';
    updateMap: boolean;
    searchMode: boolean;
    searchModeNoDisplay: boolean;

    selectedAreaList: Array<string> = new Array<string>();

    create = false;

    arealistForm: FormGroup;
    areaSearchText: string;
    areaListName: string;
    public flag = true;

    public datasource: Observable<any>;

    areas: Area[] = [];
    areaTypes: AreaType[] = [];
    areaLists: AreaList[] = [];

    availableAreas: Area[] = [];
    searchedAreas: Area[] = [];
    selectedAreas: Area[] = [];
    availablePractices: Area[] = [];

    areaList: AreaList;
    areaCodes: string[] = [];
    mapAreaCodes: string[] = [];
    areaCode: string;
    mapPolygonSelected: boolean;

    selectedPractices: Practice[] = [];
    selectedAreaAddresses: AreaAddress[] = [];

    firstTimeLoad = true;
    displayMap = true;
    displaySelectedAreasSection = false;
    parentUrl: string;

    lightBoxConfig: LightBoxConfig;

    constructor(private areaService: AreaService, private arealistService: AreaListService,
        private ref: ChangeDetectorRef) {

        // Setup new form group and form controls
        this.arealistForm = new FormGroup({
            'arealist': new FormGroup({
                'areaSearchText': new FormControl(null),
                'areaTypeList': new FormControl(null),
                'areaListName': new FormControl(null)
            })
        });
    }

    ngOnInit() {

        const element = document.getElementById('ft-arealist-manage');
        this.userId = element.getAttribute('user-id');
        this.publicId = element.getAttribute('public-id');
        this.actionType = element.getAttribute('action-type');
        this.areaSearchText = '';

        this.resolveQueryStringParams();

        this.initialAreaTypeId = this.areaTypeId;

        const areaTypesObservable = this.areaService.getAreaTypes();

        forkJoin([areaTypesObservable]).subscribe(areaTypesResult => {
            this.areaTypes = <AreaType[]>areaTypesResult[0];

            if (this.actionType === 'create') {
                this.create = true;
                this.areaListName = '';
                $('#spinner').hide();
            } else {
                const areaListByPublicIdObservable = this.arealistService.getAreaListByPublicId(this.publicId, this.userId);
                const areaListsObservable = this.arealistService.getAreaLists(this.userId);

                forkJoin([areaListByPublicIdObservable, areaListsObservable]).subscribe(results => {
                    this.areaList = <AreaList>results[0];
                    this.areaLists = <AreaList[]>results[1];

                    this.areaListName = this.areaList.ListName;
                    this.areaTypeId = this.areaList.AreaTypeId;

                    this.loadAvailableAreas();

                    this.areaTypeName = this.getAreaTypeName();
                    this.areaCodes = this.getAreaCodes();

                    if (this.areaTypeId === AreaTypeIds.Practice) {
                        this.LoadSelectedPractices();
                    } else {
                        this.loadSelectedAreas();
                    }

                    $('#spinner').hide();
                });

                this.displaySelectedAreasSection = true;
                this.ref.detectChanges();
            }

            this.loadAvailableAreaLists();
        });
    }

    getAreaCodes(): string[] {
        const areaCodes: string[] = [];
        this.areaList.AreaListAreaCodes.forEach(areaListAreaCode => {
            areaCodes.push(areaListAreaCode.AreaCode);
        });

        return areaCodes;
    }

    ignoreLegacyAreas() {
        const areaCodesToIgnore: String[] = [];
        areaCodesToIgnore.push('09');
        areaCodesToIgnore.forEach(areaCode => {
            const areaCodeIndex = this.availableAreas.findIndex(x => x.Code === areaCode);
            this.availableAreas.splice(areaCodeIndex, 1);
        });
    }

    loadAvailableAreaLists() {
        this.arealistService.getAreaLists(this.userId)
            .subscribe((result: any) => {
                this.areaLists = <AreaList[]>result;
            });
    }

    getAreaTypeName(): string {
        let areaTypeName = '';
        const areaType = this.areaTypes.find(x => x.Id === this.areaTypeId);
        if (isDefined(areaType) && areaType) {
            areaTypeName = this.areaTypes.find(x => x.Id === this.areaTypeId).Short;
        }
        return areaTypeName;
    }

    loadAvailableAreas() {
        this.areaService.getAreas(this.areaTypeId)
            .subscribe((result: any) => {
                this.availableAreas = <Area[]>result;
                this.ignoreLegacyAreas();
            });
    }

    loadSelectedAreas() {
        this.arealistService.getAreasFromAreaListAreaCodes(this.areaCodes)
            .subscribe((result: any) => {
                const selectedAreas = <Area[]>result;
                let tempSelectedAreas: Area[];

                if (selectedAreas.length > 0) {
                    tempSelectedAreas = selectedAreas.slice(0);
                    tempSelectedAreas.sort((a, b) => {
                        if (a.Name < b.Name) {
                            return -1;
                        } else if (a.Name > b.Name) {
                            return 1;
                        } else {
                            return 0;
                        }
                    });
                }

                this.selectedAreas = tempSelectedAreas;
            });
    }

    LoadSelectedPractices() {
        this.arealistService.getAreasWithAddressFromAreaListAreaCodes(this.areaCodes)
            .subscribe((result: any) => {
                this.selectedAreaAddresses = <AreaAddress[]>result;
            });
    }

    // Initialise the map
    onMapInit(mapInfo: { map: google.maps.Map }) {
        this.map = mapInfo.map;
    }

    // Capture the event emitted by google maps component when polygon (area) gets changed
    onSelectedAreaChanged(eventDetail: { areaCode: string, add: boolean }) {
        // Find the selected area from the list of available areas
        const areaSelected = this.availableAreas.find(x => x.Code === eventDetail.areaCode);

        if (areaSelected !== undefined) {
            // If area on the map is selected then add to the selected areas array
            if (eventDetail.add) {
                const areaToAdd = this.selectedAreas.find(x => x.Code === eventDetail.areaCode);
                if (areaToAdd === undefined) {
                    this.selectedAreas.push(areaSelected);
                }
            } else {
                // If the area already exists in the selected areas array, then remove it
                const indexOfSelectedArea = this.selectedAreas.findIndex(x => x.Code === areaSelected.Code);
                this.selectedAreas.splice(indexOfSelectedArea, 1);
            }

            this.ref.detectChanges();
        }
    }

    changeAreaTypeList() {
        const tempAreaTypeId: number = this.areaTypeId;
        const tempAvailableAreas: Area[] = this.availableAreas;
        this.areaTypeId = Number(this.arealistForm.get('arealist').get('areaTypeList').value);
        if (this.selectedAreas.length > 0 || this.selectedPractices.length > 0) {
            this.arealistForm.get('arealist').get('areaTypeList').setValue(tempAreaTypeId);
            this.areaTypeId = tempAreaTypeId;
            this.availableAreas = tempAvailableAreas;

            // Show lightbox
            this.showAreaTypes();

        } else {
            this.loadAvailableAreas();
            if (this.areaTypeId !== -1) {
                this.areaTypeName = this.areaTypes.find(x => x.Id === this.areaTypeId).Short;
            }

            this.displayMap = true;
        }

        this.displaySelectedAreasSection = true;
    }

    showAreaTypes() {
        const config = new LightBoxConfig();
        config.Type = LightBoxTypes.Ok;
        config.Title = 'One area type per list';
        config.Html = 'All the areas in a list must be from the same area type.<br />' +
            'Please clear the list first if you need to use a different area type.';
        config.Height = 200;
        this.lightBoxConfig = config;
    }

    setDefaultOption() {
        if (this.areaTypeId === undefined) {
            if (this.firstTimeLoad) {
                this.arealistForm.get('arealist').get('areaTypeList').setValue('-1');
                this.areaTypeId = -1;
                this.firstTimeLoad = false;
            }
        } else {
            this.arealistForm.get('arealist').get('areaTypeList').setValue(this.areaTypeId);
            this.loadAvailableAreas();
            if (this.availableAreas.length > 0) {
                this.displaySelectedAreasSection = true;
            }
        }
    }

    updateAvailableAreas(areas) {
        this.availableAreas = areas;
    }

    updateSelectedPractices(selectedPractices) {
        this.selectedPractices = selectedPractices;
    }

    updateSelectedArea(selectedArea) {
        this.moveArea(selectedArea.Code);
    }

    updateDeSelectedArea(deselectedArea) {
        this.moveArea(deselectedArea.Code);
    }

    updateMouseOverArea(area) {
        this.mouseOverArea('areas', area.Code);
    }

    updateMouseOutArea(area) {
        this.mouseOutArea('areas', area.Code);
    }

    updateLightBoxActionConfirmed(actionConfirmed: boolean) {
    }

    showPracticesAsList() {
        this.practicesAsList = true;
    }

    // Method to decide whether a map can be displayed
    canAreaTypeBeDisplayedOnMap(): boolean {
        if (this.areaTypeId === null || this.areaTypeId === -1) {
            return false;
        }

        if (this.areaTypes === null || this.areaTypes.length === 0) {
            return false;
        }

        return this.areaTypes.find(x => x.Id === this.areaTypeId).CanBeDisplayedOnMap;
    }

    // Method to decide whether to load the contents of the page during initial load
    // The loading of the content is decided based on the url
    isAnyData(): boolean {
        const href = window.location.href.trim().toLowerCase();
        if (href.indexOf('area-list') > 0 &&
            (href.indexOf('create') > 0 ||
                href.indexOf('edit') > 0)) {
            return true;
        } else {
            return false;
        }
    }

    isList(): boolean {
        if (this.areaTypeId === null || this.areaTypeId === -1) {
            return false;
        }

        return !this.canAreaTypeBeDisplayedOnMap() && !this.isPracticeSearch();
    }

    // Method to decided whether the page contains a map
    isMap(): boolean {
        return this.canAreaTypeBeDisplayedOnMap() && !this.isPracticeSearch() && this.displayMap;
    }

    isMapList(): boolean {
        return this.canAreaTypeBeDisplayedOnMap() && !this.isPracticeSearch() && !this.displayMap;
    }

    isPracticeSearch(): boolean {
        return isDefined(this.areaTypeId) && this.areaTypeId === AreaTypeIds.Practice;
    }

    isCreate() {
        return this.actionType === 'create';
    }

    showMap() {
        this.displayMap = true;
    }

    showMapList() {
        this.displayMap = false;
    }

    // Display the bottom section of the page only if at least one of the below condition satisfy
    // 1) Edit area list functionality
    // 2) Create area list functionality with area type selected
    isCreateAndAreaTypeSelected() {
        if (isDefined(this.areaTypeId) && this.areaTypeId !== AreaTypeIds.Practice) {
            if (this.actionType === 'edit' || this.actionType === 'create') {
                return true;
            }
        }
        return false;
    }

    // Method for auto-complete search
    searchArea() {
        // Read the characters typed by the user and convert it to lower case
        this.areaSearchText = this.arealistForm.get('arealist').get('areaSearchText').value.toLowerCase();

        // If the user has edited the area search text
        if (isDefined(this.areaSearchText) && this.areaSearchText) {
            // Clear the areas searches array
            this.searchedAreas.length = 0;

            // populate the areas searched array based on the user input
            this.availableAreas.forEach(element => {
                if (this.areaSearchText.length > 0 && element.Name.toLowerCase().indexOf(this.areaSearchText) > -1) {
                    this.searchedAreas.push(element);
                }
            });
        }
    }

    decorateSearchedAreasAfterInitialLoad() {
        this.decorateAreas('.searched-areas-', this.searchedAreas);
    }

    decorateAvailableAreasAfterInitialLoad() {
        this.decorateAreas('.areas-', this.selectedAreas);
    }

    decorateAreas(className: string, areas: Area[]) {
        areas.forEach(element => {
            const elementInSelectedAreas = this.selectedAreas.find(x => x.Code === element.Code);
            if (isDefined(elementInSelectedAreas) && elementInSelectedAreas) {
                $(className + element.Code).addClass('bg-info bg-primary text-white');
            }
        });
    }

    mouseOverArea(itemType, item) {
        if (!$('.' + itemType + '-' + item).hasClass('bg-info')) {
            $('.' + itemType + '-' + item).addClass('bg-primary text-white cursor-pointer');
        }
    }

    mouseOutArea(itemType, item) {
        if (!$('.' + itemType + '-' + item).hasClass('bg-info')) {
            $('.' + itemType + '-' + item).removeClass('bg-primary text-white cursor-pointer');
        }
    }

    moveArea(item) {
        const areaInAvailableAreas = this.availableAreas.find(x => x.Code === item);
        let areaInSearchedAreas: Area;
        let areaInSelectedAreas: Area;
        let areaSelected: Area;

        this.areaCode = item;

        if (isDefined(this.searchedAreas)) {
            areaInSearchedAreas = this.searchedAreas.find(x => x.Code === item);
        }

        if (isDefined(this.selectedAreas)) {
            areaInSelectedAreas = this.selectedAreas.find(x => x.Code === item);
        }

        if (isDefined(areaInAvailableAreas)) {
            areaSelected = areaInAvailableAreas;
        } else if (isDefined(areaInSearchedAreas)) {
            areaSelected = areaInSearchedAreas;
        } else {
            return false;
        }

        if (isDefined(areaSelected) && isDefined(areaInSelectedAreas) && areaSelected.Code === areaInSelectedAreas.Code) {
            const indexOfSelectedArea = this.selectedAreas.findIndex(x => x.Code === areaInSelectedAreas.Code);
            this.selectedAreas.splice(indexOfSelectedArea, 1);

            if (isDefined(areaInAvailableAreas)) {
                $('.areas-' + item).removeClass('bg-info bg-primary text-white');
            }

            if (isDefined(areaInSearchedAreas)) {
                $('.searched-areas-' + item).removeClass('bg-info bg-primary text-white');
            }

            if (this.isMap()) {
                this.mapPolygonSelected = false;
            }
        } else {
            this.selectedAreas.push(areaInAvailableAreas);

            if (isDefined(areaInAvailableAreas)) {
                $('.areas-' + item).addClass('bg-info text-white');
            }

            if (isDefined(areaInSearchedAreas)) {
                $('.searched-areas-' + item).addClass('bg-info text-white');
            }

            if (this.isMap()) {
                this.mapPolygonSelected = true;
            }
        }
    }

    moveAreaOut(itemType, item) {
        const areaInSelectedAreas = this.selectedAreas.find(x => x.Code === item);
        const indexOfSelectedArea = this.selectedAreas.indexOf(areaInSelectedAreas);
        this.selectedAreas.splice(indexOfSelectedArea, 1);

        const areaInAvailableAreas = this.availableAreas.find(x => x.Code === item);
        if (isDefined(areaInAvailableAreas)) {
            $('.areas-' + item).removeClass('bg-info bg-primary text-white');
        }

        const areaInSearchedAreas = this.searchedAreas.find(x => x.Code === item);
        if (isDefined(areaInSearchedAreas)) {
            $('.searched-areas-' + item).removeClass('bg-info bg-primary text-white');
        }

        if (this.isMap()) {
            this.mapPolygonSelected = false;
            this.areaCode = item;
            this.ref.detectChanges();
        }
    }

    toggleMap() {
        const $areaListTable = $('#area-list-table');
        const $toggleMapHeading = $('#toggleMapHeading');

        $areaListTable.toggle();
        $('#area-list-map').toggle();

        if ($areaListTable.is(':visible')) {
            $toggleMapHeading.html('Show all areas as a map');
        } else {
            $toggleMapHeading.html('Show all areas as a list');
        }
    }

    // Method to validate
    validateSave() {
        let isValid = true;

        if (this.areaListName === null || this.areaListName === undefined || this.areaListName === '') {
            this.areaListName = this.arealistForm.get('arealist').get('areaListName').value;
        }

        if (this.areaListName === null || this.areaListName.trim().length === 0) {
            if (this.areaTypeId === null || this.areaTypeId === -1) {
                $('#error-details').html('Please enter a valid list name, select an area type and add areas to the list');
            } else {
                $('#error-details').html('Please enter a valid list name');
            }
            isValid = false;
        }

        if (isValid && (this.areaTypeId !== AreaTypeIds.Practice && this.selectedAreas.length === 0)) {
            if (this.areaTypeId === null || this.areaTypeId === -1) {
                $('#error-details').html('Please select an area type and add areas to the list');
            } else {
                $('#error-details').html('Please add areas to the list');
            }
            isValid = false;
        }

        if (isValid && (this.areaTypeId === AreaTypeIds.Practice && this.selectedPractices.length === 0)) {
            $('#error-details').html('Please add practices to the list');
            isValid = false;
        }

        if (isValid && this.actionType === 'create') {
            if (isDefined(this.areaLists) && this.areaLists) {
                if (this.areaLists.find(x => x.ListName === this.areaListName)) {
                    $('#error-details').html('The area list name you have entered is already in use.');
                    isValid = false;
                }
            }
        }

        return isValid;
    }

    saveAreaList() {
        this.areaListName = (<HTMLInputElement>document.getElementById('areaListName')).value;

        if (this.validateSave()) {

            const areaCodeList = [];

            if (this.areaTypeId === AreaTypeIds.Practice) {
                this.selectedPractices.forEach(element => {
                    areaCodeList.push(element.areaCode);
                });
            } else {
                this.selectedAreas.forEach(element => {
                    areaCodeList.push(element.Code);
                });
            }

            const formData: FormData = new FormData();
            formData.append('areaListName', this.areaListName);
            formData.append('areaCodeList', areaCodeList.toString());
            formData.append('userId', this.userId);

            if (this.actionType === 'create') {
                // Save new list
                formData.append('areaTypeId', this.areaTypeId.toString());

                this.arealistService.saveAreaList(formData)
                    .subscribe(
                        (response) => {
                            this.goToDataPage(true);
                        },
                        (error) => {
                            $('#error-details').html('Unable to save the area list');
                        }
                    );
            } else {
                // Update existing list
                formData.append('areaListId', this.areaList.Id.toString());
                formData.append('publicId', this.publicId);

                this.arealistService.updateAreaList(formData)
                    .subscribe(
                        (response) => {
                            this.goToDataPage();
                        },
                        (error) => {
                            $('#error-details').html('Unable to update the area list');
                        }
                    );
            }
        }
    }

    goToDataPage(isNewList = false) {

        let url;

        if (this.parentUrl === undefined) {
            // Area list index
            url = '/user-account/area-list';
        } else {
            url = this.parentUrl;

            // Change URL if new list and list has been created for last area type viewed
            if (isNewList && this.initialAreaTypeId === this.areaTypeId) {

                // Remove area because might not be in list
                url = url.replace(/\/are\/[^\\]+/, '');

                // Remove parent area as will redirect to list
                url = url.replace(/\/par\/[^\\]+/, '');

                // Set parent area type to be area lists
                url = url.replace(/\/pat\/\d+/, '/pat/30000');
            }
        }

        window.location.href = url;
    }

    clearList() {
        if (this.isMap()) {
            this.selectedAreas.forEach(element => {
                this.areaCode = element.Code;
                this.mapPolygonSelected = false;
                this.ref.detectChanges();
            });
        }

        this.selectedAreas.length = 0;
        this.searchedAreas.length = 0;

        this.availableAreas.forEach(element => {
            $('.areas-' + element.Code).removeClass('bg-info bg-primary text-white');
        });

        this.areaSearchText = '';
    }

    cancel() {
        this.goToDataPage();
    }

    back() {
        window.location.href = '/user-account/area-list';
    }

    resolveQueryStringParams() {
        const url = window.location.href;
        if (url.indexOf('?') > -1) {
            const parameterSection = url.split('?')[1];
            const params = parameterSection.split('&');

            const param0 = params[0].split('=');

            if (param0[0].toLowerCase() === 'list_id') {
                this.publicId = param0[1];
            } else if (param0[0].toLowerCase() === 'area-type-id') {
                this.areaTypeId = Number(param0[1]);
            }

            if (params[1]) {
                this.parentUrl = params[1].split('=')[1];
            }
        }
    }
}

class MapColourData {
    quartile: number;
    quintile: number;
    orderFrac: number;
    order: number;
}
