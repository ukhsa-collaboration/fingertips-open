import { Component, OnInit, ViewChild, ElementRef, AfterViewInit, AfterViewChecked, ChangeDetectorRef, Input, OnChanges, SimpleChanges } from '@angular/core';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { AreaService } from '../../shared/service/api/area.service';
import { AreaCodes, AreaTypeIds, CategoryTypeIds } from '../../shared/shared';
import { AreaTextSearchResult, NearByAreas, LatitudeLongitude, AreaAddress } from '../../typings/FT.d';
import { Observable } from 'rxjs/Observable';
import { TypeaheadMatch } from 'ngx-bootstrap/typeahead';
import { autoCompleteResult, Practice } from './practice-search';
import * as _ from 'underscore';
@Component({
    selector: 'ft-practice-search',
    templateUrl: './practice-search.component.html',
    styleUrls: ['./practice-search.component.css']
})
export class PracticeSearchComponent implements OnInit, OnChanges {

    @ViewChild('scrollPracticeTable') practiceTable: ElementRef;
    @ViewChild('googleMapNew') mapEl: ElementRef;;
    @Input() IsMapUpdateRequired: boolean = false;
    @Input() searchMode: boolean = false;

    // Instance equivalent of searchMode which is more reliable
    localSearchMode: boolean;

    public placeNameText: string;
    public typeaheadLoading: boolean;
    public typeaheadNoResults: boolean;
    public dataSource: Observable<any>;
    public height: number = 0;

    practiceMap: google.maps.Map;
    isVisible: boolean = false;
    searchResults: AreaTextSearchResult[] = [];
    selectedArea: AreaTextSearchResult;
    nearByPractices: Practice[] = [];
    practiceCountText: string;
    showCcgPractices: boolean = false;
    displayCCGPracticeLink: boolean;

    ngOnChanges(changes: SimpleChanges): void {
        if (this.IsMapUpdateRequired && this.showCcgPractices) {
            this.onShowAllPracticeinCCGClick();
        }
        if (changes['searchMode']) {
            let displyCCGLink = changes['searchMode'].currentValue;
            if (displyCCGLink !== undefined) {
                if (displyCCGLink) {
                    this.localSearchMode = true;
                    this.displayCCGPracticeLink = false;
                }
            } else {
                this.localSearchMode = false;
                this.displayCCGPracticeLink = true;
            }

        }
    }

    constructor(private ftHelperService: FTHelperService,
        private areaService: AreaService,
        private ref: ChangeDetectorRef) {

        this.localSearchMode = false;
        this.displayCCGPracticeLink = true;
        this.dataSource = Observable.create((observer: any) => {
            this.areaService.getAreaSearchByText(this.placeNameText, AreaTypeIds.CcgPreApr2017, true, true)
                .subscribe((result: any) => {
                    this.searchResults = <AreaTextSearchResult[]>result;
                    let newResult = _.map(this.searchResults, function (result) {
                        return new autoCompleteResult(result.PolygonAreaCode, result.PlaceName,
                            result.PolygonAreaName);
                    });
                    observer.next(newResult);
                });
        });
    }

    ngOnInit() {
        this.isVisible = this.ftHelperService.getFTModel().areaTypeId === AreaTypeIds.Practice;
        if (!this.practiceMap) {
            this.loadMap();
        }
    }

    loadMap() {
        const mapOptions: google.maps.MapOptions = {
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
        let mapContainer = null;
        if (this.mapEl && this.mapEl.nativeElement) {
            mapContainer = this.mapEl.nativeElement;
        }
        if (mapContainer != null) {
            this.practiceMap = new google.maps.Map(mapContainer, mapOptions);
        }

        if (this.practiceMap) {
            const bounds = new google.maps.LatLngBounds();
            const position = new google.maps.LatLng(53.415649, -2.209015);
            bounds.extend(position);
            this.practiceMap.setCenter(bounds.getCenter());
            let thisCom = this;
            //This is needed as on initial load map was not visible
            google.maps.event.addListener(this.practiceMap, 'idle', function () {
                let bound = thisCom.practiceMap.getCenter();
                google.maps.event.trigger(thisCom.practiceMap, 'resize');
                thisCom.practiceMap.setCenter(bound);
            });
        }
    }
    public changeTypeaheadLoading(e: boolean): void {
        this.typeaheadLoading = e;
    }

    public changeTypeaheadNoResults(e: boolean): void {
        this.typeaheadNoResults = e;
    }

    public typeaheadOnSelect(e: TypeaheadMatch): void {
        let polygonAreaCode = (<autoCompleteResult>e.item).polygonAreaCode;
        this.selectedArea = _.find(this.searchResults, function (obj) { return obj.PolygonAreaCode === polygonAreaCode; });
        this.areaService.getAreaSearchByProximity(this.selectedArea.Easting, this.selectedArea.Northing, AreaTypeIds.Practice)
            .subscribe((result: any) => {
                let nearByAreas = <NearByAreas[]>result;
                this.nearByPractices = _.map(nearByAreas, function (area) {
                    return new Practice(area.AreaName, area.AreaCode,
                        area.AddressLine1, area.AddressLine2, area.Postcode,
                        area.DistanceValF, area.LatLng);
                });
                this.displayNumberOfPracticesFound(this.nearByPractices.length, false);
                this.displayMarkersOnMap();
                this.showCcgPractices = false;
            });
    }
    displayNumberOfPracticesFound(practiceCount, IsCCG: boolean): void {
        let placeName: string = '';
        if (IsCCG) {
            placeName = ' in ' + this.ftHelperService.getParentArea().Name;
        }
        else {
            placeName = ' within 5 miles of ' + this.selectedArea.PlaceName;
        }
        let html: string;
        if (practiceCount === 0) {
            html = 'are no practices';
        } else if (practiceCount === 1) {
            html = 'is 1 practice';
        } else {
            html = 'are ' + practiceCount + ' practices';
        }
        this.practiceCountText = 'There ' + html + placeName;
    }

    onShowAllPracticeinCCGClick(): void {
        this.areaService.getAreaAddressesByParentAreaCode(this.ftHelperService.getFTModel().parentCode,
            this.ftHelperService.getFTModel().areaTypeId)
            .subscribe((result: any) => {
                let areaAddresses = <AreaAddress[]>result;
                this.nearByPractices = _.map(areaAddresses, function (area) {
                    return new Practice(area.Name, area.Code,
                        area.A1, area.A2, area.Postcode, '', area.Pos);
                });
                this.displayNumberOfPracticesFound(this.nearByPractices.length, true);
                this.displayMarkersOnMap();
                this.showCcgPractices = true;
                this.placeNameText = '';
            });
    }

    displayMarkersOnMap(): void {
        let bounds = new google.maps.LatLngBounds();
        const infoWindow = new google.maps.InfoWindow({});
        let linkList = [];
        for (let i = 0; i < this.nearByPractices.length; i++) {
            let position = new google.maps.LatLng(this.nearByPractices[i].lat, this.nearByPractices[i].lng);
            bounds.extend(position);

            // Create marker
            let marker = new google.maps.Marker({
                position: position,
                map: this.practiceMap
            });
            marker.set('marker_id', this.nearByPractices[i].areaCode);

            // Create pop up text
            let boxText = document.createElement("a");
            boxText.id = i.toString();
            boxText.className = "select-area-link";
            boxText.style.cssText = "color: #2e3191; text-decoration: underline; font-size:16px;";
            boxText.innerHTML = this.nearByPractices[i].areaName;
            linkList.push(boxText);

            // Map marker click
            google.maps.event.addListener(marker, 'click', (event) => {
                let areaCode = marker.get('marker_id');
                infoWindow.setContent(linkList[i]);
                infoWindow.open(this.practiceMap, marker);

                let $practiceHeader = $('#' + areaCode);

                // Deselect last selected one if any
                let lastSelectedPracticeIndex = _.findIndex(this.nearByPractices, x => x.selected === true);
                if (lastSelectedPracticeIndex !== -1) {
                    const lastSelectedPractice = this.nearByPractices[lastSelectedPracticeIndex];
                    lastSelectedPractice.selected = false;
                    if (lastSelectedPracticeIndex !== -1) {
                        this.nearByPractices.splice(lastSelectedPracticeIndex, 1, lastSelectedPractice);
                    }
                    this.nearByPractices = this.nearByPractices.slice();
                }

                // Select-Highlight the current practice on table
                const currentPracticeIndex = _.findIndex(this.nearByPractices, x => x.areaCode === areaCode);
                const currentPractice = this.nearByPractices[currentPracticeIndex];
                currentPractice.selected = true;
                if (currentPracticeIndex !== -1) {
                    this.nearByPractices.splice(currentPracticeIndex, 1, currentPractice);
                }
                this.nearByPractices = this.nearByPractices.slice();

                // Scroll table so selected practice is at the top
                const scrollTop = $practiceHeader.offset().top -
                    this.practiceTable.nativeElement.offsetTop +
                    this.practiceTable.nativeElement.scrollTop;
                this.practiceTable.nativeElement.scrollTop = scrollTop;

                this.ref.detectChanges();
            });

            // Practice pop up link click
            google.maps.event.addDomListener(linkList[i], 'click', (event) => {
                let areaCode = marker.get('marker_id');
                this.onSelectPracticeClick(areaCode);
            });
        }
        this.fitMapToPracticeResults(bounds);

        this.height = 610;
    }

    fitMapToPracticeResults(bounds: google.maps.LatLngBounds) {
        this.practiceMap.fitBounds(bounds);
        let googleMapsEvent = google.maps.event;
        // Add bounds changed listener
        let thisCom = this;
        let zoomChangeBoundsListener =
            googleMapsEvent.addListenerOnce(this.practiceMap, 'bounds_changed', function () {
                const maximumZoom = 13;
                // Zoom out if to close
                if (thisCom.practiceMap.getZoom() > maximumZoom) {
                    thisCom.practiceMap.setZoom(maximumZoom);
                }
                googleMapsEvent.removeListener(zoomChangeBoundsListener);
            });
    }

    onSelectPracticeClick(areaCode: string): void {
        this.ftHelperService.lock();
        if (this.localSearchMode) {
            window.location.href = '/profile/general-practice/data#page/12/ati/' +
                AreaTypeIds.Practice + '/are/' + areaCode;
        } else {
            this.ftHelperService.setAreaCode(areaCode);
            this.ftHelperService.redirectToPopulationPage();
        }
    }


}

