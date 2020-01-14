import {
  Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Input, Output, OnChanges, SimpleChanges, EventEmitter
} from '@angular/core';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { AreaService } from '../../shared/service/api/area.service';
import { AreaTypeIds } from '../../shared/constants';
import { AreaTextSearchResult, NearByAreas, AreaAddress, Area } from '../../typings/FT';
import { Observable } from 'rxjs/Observable';
import { TypeaheadMatch } from 'ngx-bootstrap/typeahead/ngx-bootstrap-typeahead';
import { AutoCompleteResult, Practice } from './practice-search-simple';
import * as _ from 'underscore';
import { FormGroup, FormControl } from '@angular/forms';
import { isDefined } from '@angular/compiler/src/util';

@Component({
  selector: 'ft-practice-search-simple',
  templateUrl: './practice-search-simple.component.html',
  styleUrls: ['./practice-search-simple.component.css']
})
export class PracticeSearchSimpleComponent implements OnInit, OnChanges {

  @ViewChild('scrollPracticeTable', { static: true }) practiceTable: ElementRef;
  @ViewChild('googleMapNew', { static: true }) mapEl: ElementRef;
  @Input() IsMapUpdateRequired = false;
  @Input() searchMode = false;
  @Input() areaTypeId: number;
  @Input() selectedAreaAddresses;
  @Output() emitSelectedPractices = new EventEmitter();
  @Output() emitShowPracticesAsList = new EventEmitter();

  // Instance equivalent of searchMode which is more reliable
  localSearchMode: boolean;

  public placeNameText: string;
  public typeaheadLoading: boolean;
  public typeaheadNoResults: boolean;
  public dataSource: Observable<any>;
  public height = 0;

  practiceMap: google.maps.Map;
  isVisible = false;
  searchResults: AreaTextSearchResult[] = [];
  selectedArea: AreaTextSearchResult;
  nearByPractices: Practice[] = [];
  practiceCountText: string;
  showCcgPractices = false;
  displayCCGPracticeLink: boolean;
  practiceSearchText: string;

  practicesInCCGFormGroup: FormGroup;
  practicesInCCG: Area[] = [];
  selectedPractices: Practice[] = [];
  firstTimeLoad = true;

  private readonly CcgAreaTypeToSearch = AreaTypeIds.CcgSinceApr2019;

  ngOnChanges(changes: SimpleChanges): void {
    if (this.IsMapUpdateRequired && this.showCcgPractices) {
      this.onShowAllPracticeinCCGClick();
    }
    if (changes['searchMode']) {
      const searchMode = changes['searchMode'].currentValue;
      if (isDefined(searchMode)) {
        if (searchMode) {
          this.localSearchMode = true;
          this.displayCCGPracticeLink = false;
        }
      } else {
        this.localSearchMode = false;
        this.displayCCGPracticeLink = true;
      }
    }
    if (changes['selectedAreaAddresses']) {
      this.searchForPracticesInCCG();
    }
  }

  constructor(private ftHelperService: FTHelperService,
    private areaService: AreaService,
    private ref: ChangeDetectorRef) {

    this.localSearchMode = false;
    this.displayCCGPracticeLink = true;
    this.dataSource = Observable.create((observer: any) => {
      this.areaService.getAreaSearchByText(this.placeNameText, AreaTypeIds.DistrictUA, true, true)
        .subscribe((areaTextSearchResult: any) => {

          this.searchResults = <AreaTextSearchResult[]>areaTextSearchResult;
          const newResult = _.map(this.searchResults, function (result) {
            return new AutoCompleteResult(result.PolygonAreaCode, result.PlaceName,
              result.PolygonAreaName);
          });
          observer.next(newResult);
        });
    });

    this.practicesInCCGFormGroup = new FormGroup({
      practicesInCCG: new FormControl(),
      practiceSearchText: new FormControl()
    });
  }

  ngOnInit() {
    $('#nearby-practices-list').hide();
    this.searchForPracticesInCCG();
    this.loadMap();
  }

  setDefaultOption() {
    if (this.firstTimeLoad) {
      this.practicesInCCGFormGroup.get('practicesInCCG').setValue('-1');
      this.firstTimeLoad = false;
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
    if (mapContainer !== null) {
      this.practiceMap = new google.maps.Map(mapContainer, mapOptions);
    }

    if (this.practiceMap) {
      const bounds = new google.maps.LatLngBounds();
      const position = new google.maps.LatLng(53.415649, -2.209015);
      bounds.extend(position);
      this.practiceMap.setCenter(bounds.getCenter());
      const thisCom = this;
      // This is needed as on initial load map was not visible
      google.maps.event.addListener(this.practiceMap, 'idle', function () {
        const bound = thisCom.practiceMap.getCenter();
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
    const polygonAreaCode = (<AutoCompleteResult>e.item).polygonAreaCode;

    this.selectedArea = _.find(this.searchResults, function (obj) { return obj.PolygonAreaCode === polygonAreaCode; });

    this.areaService.getAreaSearchByProximity(this.selectedArea.Easting, this.selectedArea.Northing, AreaTypeIds.Practice)
      .subscribe((result: any) => {
        const nearByAreas = <NearByAreas[]>result;
        this.nearByPractices = _.map(nearByAreas, function (area) {
          return new Practice(area.AreaName, area.AreaCode,
            area.AddressLine1, area.AddressLine2, area.Postcode,
            area.DistanceValF, area.LatLng);
        });
        this.displayMarkersOnMap();
        this.showCcgPractices = false;
      });
  }

  onShowAllPracticeinCCGClick(): void {
    this.practiceSearchText = '';
    const practiceCode = this.practicesInCCGFormGroup.get('practicesInCCG').value;
    this.areaService.getAreaAddressesByParentAreaCode(practiceCode, AreaTypeIds.Practice)
      .subscribe((result: any) => {
        const areaAddresses = <AreaAddress[]>result;
        this.nearByPractices = _.map(areaAddresses, function (area) {
          return new Practice(area.Name, area.Code,
            area.A1, area.A2, area.Postcode, '', area.Pos);
        });

        this.displayMarkersOnMap();
        this.decorateSelectedNearbyAreas();
        this.showCcgPractices = true;
        this.placeNameText = '';
      });
  }

  displayMarkersOnMap(): void {

    const iconPath = 'http://maps.google.com/mapfiles/ms/icons/';

    const bounds = new google.maps.LatLngBounds();
    const infoWindow = new google.maps.InfoWindow({});
    const linkList = [];

    for (let i = 0; i < this.nearByPractices.length; i++) {
      const position = new google.maps.LatLng(this.nearByPractices[i].lat, this.nearByPractices[i].lng);
      bounds.extend(position);

      let practiceInSelectedPractices: Practice;
      practiceInSelectedPractices = this.selectedPractices.find(x => x.areaCode === this.nearByPractices[i].areaCode);

      const iconFile = (isDefined(practiceInSelectedPractices) && practiceInSelectedPractices)
        ? 'green-dot.png'
        : 'ltblue-dot.png';

      // Create marker
      const marker = new google.maps.Marker({
        position: position,
        map: this.practiceMap,
        icon: iconPath + iconFile
      });
      marker.set('marker_id', this.nearByPractices[i].areaCode);

      // Create pop up text
      const boxText = document.createElement('span');
      boxText.id = i.toString();
      boxText.className = 'select-area-link';
      boxText.style.cssText = 'font-size:16px;';
      boxText.innerHTML = this.nearByPractices[i].areaName;
      linkList.push(boxText);

      // Map marker click
      google.maps.event.addListener(marker, 'click', (event) => {
        const areaCode = marker.get('marker_id');

        infoWindow.setContent(linkList[i]);
        infoWindow.open(this.practiceMap, marker);

        const $area = $('.available-practices-' + areaCode);
        const selectedPracticeIndex = _.findIndex(this.selectedPractices, x => x.areaCode === areaCode);
        if (selectedPracticeIndex === -1) {
          marker.setIcon(iconPath + 'green-dot.png')
          const currentPracticeIndex = _.findIndex(this.nearByPractices, x => x.areaCode === areaCode);
          const currentPractice = this.nearByPractices[currentPracticeIndex];
          this.selectedPractices.push(currentPractice);

          $area.addClass('bg-info text-white');
        } else {
          marker.setIcon(iconPath + 'ltblue-dot.png');
          this.selectedPractices.splice(selectedPracticeIndex, 1);
          $area.removeClass('bg-info text-white');
        }

        this.emitSelectedPractices.emit(this.selectedPractices);

        this.ref.detectChanges();
      });

      // Practice pop up link click
      google.maps.event.addDomListener(linkList[i], 'click', (event) => {
        const areaCode = marker.get('marker_id');
        this.onSelectPracticeClick(areaCode);
      });
    }
    this.fitMapToPracticeResults(bounds);

    this.height = 610;
  }

  fitMapToPracticeResults(bounds: google.maps.LatLngBounds) {
    this.practiceMap.fitBounds(bounds);
    const googleMapsEvent = google.maps.event;
    // Add bounds changed listener
    const thisCom = this;
    const zoomChangeBoundsListener =
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

  searchForPracticesInCCG() {
    this.areaService.getAreas(this.CcgAreaTypeToSearch)
      .subscribe((result: any) => {
        this.practicesInCCG = <Area[]>result;
      });

    if (this.selectedAreaAddresses.length > 0) {
      this.selectedAreaAddresses.forEach(element => {
        const practice: Practice = new Practice(element.Name, element.Code, element.A1, element.A2, element.Postcode, '', element.Pos);
        this.selectedPractices.push(practice);
      });

      // Emit selected practices to the parent
      this.emitSelectedPractices.emit(this.selectedPractices);
    }
  }

  clearList() {
    if (this.selectedPractices.length > 0) {
      this.selectedPractices.forEach(element => {
        $('.available-practices-' + element.areaCode).removeClass('bg-primary bg-info text-white');
      });

      this.selectedPractices.length = 0;
      this.displayMarkersOnMap();

      this.nearByPractices.forEach(practice => {
        const $element = this.getItem('available-practices', practice.areaCode);
        $element.removeClass('bg-info text-white');
      });
    }
  }

  removeFromList(areaCode) {
    const areaCodeIndex = this.selectedPractices.findIndex(x => x.areaCode === areaCode);
    this.selectedPractices.splice(areaCodeIndex, 1);

    if (this.practicesInCCGFormGroup.get('practicesInCCG').value !== '-1') {
      this.displayMarkersOnMap();
    }

    const nearByPractice = this.nearByPractices.find(x => x.areaCode === areaCode);
    if (nearByPractice !== null && nearByPractice !== undefined) {
      const $element = this.getItem('available-practices', nearByPractice.areaCode);
      $element.removeClass('bg-primary text-white cursor-pointer bg-info');
    }
  }

  toggleMap() {
    const $practicesList = $('#nearby-practices-list');
    $practicesList.toggle();
    $('#nearby-practices-map').toggle();

    const heading = $practicesList.is(':visible')
      ? 'Show practices on map'
      : 'Show practices as list'

    this.decorateSelectedNearbyAreas();

    $('#toggleMapHeading').html(heading);
  }

  decorateSelectedNearbyAreas(): void {
    const $practicesList = $('#nearby-practices-list');
    if ($practicesList.is(':visible')) {
      this.selectedPractices.forEach(practice => {
        const nearByPractice = this.nearByPractices.find(x => x.areaCode === practice.areaCode);
        if (nearByPractice !== undefined && nearByPractice !== null) {
          const $element = this.getItem('available-practices', nearByPractice.areaCode);
          $element.addClass('bg-info text-white');
        }
      });
    }
  }

  // Method to handle the mouse over event on the practice list
  selectPractice(itemType, item) {

    const $element = this.getItem(itemType, item);
    if (!$element.hasClass('bg-info')) {
      $element.addClass('bg-primary text-white cursor-pointer');
    }
  }

  // Method to handle the mouse out event on the practice list
  deselectPractice(itemType, item) {

    const $element = this.getItem(itemType, item);
    if (!$element.hasClass('bg-info')) {
      $element.removeClass('bg-primary text-white cursor-pointer');
    }
  }

  getItem(itemType, item) {
    return $('.' + itemType + '-' + item);
  }

  movePractice(itemType, item) {
    const $element = this.getItem(itemType, item);

    const selectedPracticeIndex = _.findIndex(this.selectedPractices, x => x.areaCode === item);
    if (selectedPracticeIndex === -1) {
      const currentPracticeIndex = _.findIndex(this.nearByPractices, x => x.areaCode === item);
      const currentPractice = this.nearByPractices[currentPracticeIndex];
      this.selectedPractices.push(currentPractice);

      $element.addClass('bg-info text-white');

    } else {
      this.selectedPractices.splice(selectedPracticeIndex, 1);
      $element.removeClass('bg-info text-white');
    }

    this.displayMarkersOnMap();

    this.emitSelectedPractices.emit(this.selectedPractices);
  }
}
