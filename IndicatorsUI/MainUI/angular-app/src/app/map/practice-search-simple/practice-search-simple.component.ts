import {
  Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Input, Output, OnChanges, SimpleChanges, EventEmitter
} from '@angular/core';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { AreaService } from '../../shared/service/api/area.service';
import { AreaListService } from '../../shared/service/api/arealist.service';
import { AreaTypeIds } from '../../shared/shared';
import { AreaTextSearchResult, NearByAreas, AreaAddress, Area } from '../../typings/FT.d';
import { Observable } from 'rxjs/Observable';
import { TypeaheadMatch } from 'ngx-bootstrap/typeahead';
import { AutoCompleteResult, Practice } from './practice-search-simple';
import * as _ from 'underscore';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
  selector: 'ft-practice-search-simple',
  templateUrl: './practice-search-simple.component.html',
  styleUrls: ['./practice-search-simple.component.css'],
  providers: [AreaListService]
})
export class PracticeSearchSimpleComponent implements OnInit, OnChanges {

  @ViewChild('scrollPracticeTable') practiceTable: ElementRef;
  @ViewChild('googleMapNew') mapEl: ElementRef;
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

  ngOnChanges(changes: SimpleChanges): void {
    if (this.IsMapUpdateRequired && this.showCcgPractices) {
      this.onShowAllPracticeinCCGClick();
    }
    if (changes['searchMode']) {
      const displyCCGLink = changes['searchMode'].currentValue;
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
    if (changes['selectedAreaAddresses']) {
      this.searchForPracticesInCCG();
    }
  }

  constructor(private ftHelperService: FTHelperService,
    private areaService: AreaService,
    private arealistService: AreaListService,
    private ref: ChangeDetectorRef) {

    this.localSearchMode = false;
    this.displayCCGPracticeLink = true;
    this.dataSource = Observable.create((observer: any) => {
      this.areaService.getAreaSearchByText(this.placeNameText, AreaTypeIds.CcgPreApr2017, true, true)
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
        this.showCcgPractices = true;
        this.placeNameText = '';
      });
  }

  displayMarkersOnMap(): void {
    const bounds = new google.maps.LatLngBounds();
    const infoWindow = new google.maps.InfoWindow({});
    const linkList = [];
    let iconUrl;

    for (let i = 0; i < this.nearByPractices.length; i++) {
      const position = new google.maps.LatLng(this.nearByPractices[i].lat, this.nearByPractices[i].lng);
      bounds.extend(position);

      let practiceInSelectedPractices: Practice;
      practiceInSelectedPractices = this.selectedPractices.find(x => x.areaCode === this.nearByPractices[i].areaCode);
      if (practiceInSelectedPractices !== null && practiceInSelectedPractices !== undefined) {
        iconUrl = 'http://maps.google.com/mapfiles/ms/icons/green-dot.png';
      } else {
        iconUrl = 'http://maps.google.com/mapfiles/ms/icons/ltblue-dot.png';
      }

      // Create marker
      const marker = new google.maps.Marker({
        position: position,
        map: this.practiceMap,
        icon: iconUrl
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

        const selectedPracticeIndex = _.findIndex(this.selectedPractices, x => x.areaCode === areaCode);
        if (selectedPracticeIndex === -1) {
          marker.setIcon('http://maps.google.com/mapfiles/ms/icons/green-dot.png')
          const currentPracticeIndex = _.findIndex(this.nearByPractices, x => x.areaCode === areaCode);
          const currentPractice = this.nearByPractices[currentPracticeIndex];
          this.selectedPractices.push(currentPractice);

          $('.available-practices-' + areaCode).addClass('bg-info text-white');
        } else {
          marker.setIcon('http://maps.google.com/mapfiles/ms/icons/ltblue-dot.png');
          this.selectedPractices.splice(selectedPracticeIndex, 1);
          $('.available-practices-' + areaCode).removeClass('bg-info text-white');
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
    this.areaService.getAreas(152)
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
    }
  }

  removeFromList(areaCode) {
    let areaCodeIndex = this.selectedPractices.findIndex(x => x.areaCode === areaCode);
    this.selectedPractices.splice(areaCodeIndex, 1);

    this.displayMarkersOnMap();
  }

  toggleMap() {
    $('#nearby-practices-list').toggle();
    $('#nearby-practices-map').toggle();

    if ($('#nearby-practices-list').is(':visible')) {
      $('#toggleMapHeading').html('Show practices on map');
    } else {
      $('#toggleMapHeading').html('Show practices as list');
    }
  }

  // Method to handle the mouse over event on the practice list
  selectPractice(itemType, item) {
    if (!$('.' + itemType + '-' + item).hasClass('bg-info')) {
      $('.' + itemType + '-' + item).addClass('bg-primary text-white cursor-pointer');
    }
  }

  // Method to handle the mouse out event on the practice list
  deselectPractice(itemType, item) {
    if (!$('.' + itemType + '-' + item).hasClass('bg-info')) {
      $('.' + itemType + '-' + item).removeClass('bg-primary text-white cursor-pointer');
    }
  }

  movePractice(itemType, item) {
    const selectedPracticeIndex = _.findIndex(this.selectedPractices, x => x.areaCode === item);
    if (selectedPracticeIndex === -1) {
      const currentPracticeIndex = _.findIndex(this.nearByPractices, x => x.areaCode === item);
      const currentPractice = this.nearByPractices[currentPracticeIndex];
      this.selectedPractices.push(currentPractice);

      $('.' + itemType + '-' + item).addClass('bg-info text-white');

    } else {
      this.selectedPractices.splice(selectedPracticeIndex, 1);
      $('.' + itemType + '-' + item).removeClass('bg-info');
    }

    this.displayMarkersOnMap();

    this.emitSelectedPractices.emit(this.selectedPractices);
  }
}
