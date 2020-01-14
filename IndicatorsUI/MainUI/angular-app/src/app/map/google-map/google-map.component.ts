import {
    Component, Input, Output, ElementRef,
    ViewChild, EventEmitter, OnChanges, SimpleChanges
} from '@angular/core';
import 'rxjs/rx';
import { isDefined } from '@angular/compiler/src/util';
import { GoogleMapService } from '../googleMap.service';
import { IndicatorService } from '../../shared/service/api/indicator.service';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { CoreDataHelperService } from '../../shared/service/helper/coreDataHelper.service';
import { ParentAreaHelper, AreaHelper, DeviceServiceHelper, NewDataBadgeHelper } from '../../shared/shared';
import { ComparatorIds, AreaTypeIds, Tabs, AreaCodes } from '../../shared/constants';
import * as _ from 'underscore';
import { DownloadService } from '../../shared/service/api/download.service';
import { DeviceDetectorService } from '../../../../node_modules/ngx-device-detector';
import { LightBoxConfig, LightBoxTypes } from '../../shared/component/light-box/light-box';
import { CsvConfig, CsvData, CsvDataHelper } from '../../shared/component/export-csv/export-csv';
import { TrendMarkerLabelProvider } from '../../shared/classes/trendmarker-label-provider';
import { TimePeriod } from '../../shared/classes/time-period';
import { CoreDataSet, GroupRoot, IndicatorMetadata, TrendMarkerResult } from '../../typings/FT';

@Component({
    selector: 'ft-google-map',
    templateUrl: './google-map.component.html',
    styleUrls: ['./google-map.component.css'],
    providers: [GoogleMapService]
})
export class GoogleMapComponent implements OnChanges {
    map: google.maps.Map;
    @ViewChild('googleMap', { static: true }) mapEl: ElementRef;
    @Output() mapInit = new EventEmitter();
    @Output() hoverAreaCodeChanged = new EventEmitter();
    @Output() selectedAreaChanged = new EventEmitter();
    @Output() benchmarkChanged = new EventEmitter();
    @Input() areaTypeId: number = null;
    @Input() currentAreaCode: string = null;
    @Input() areaCodeColour = null;
    @Input() refreshColour;
    @Input() isBoundaryNotSupported;
    @Input() selectedAreaList;
    @Input() sortedCoreData: Map<string, CoreDataSet> = null;
    @Input() benchmarkIndex: number = null;

    isNearestNeighbours = false;
    subNationalButtonClass: string;
    nationalButtonClass: string;
    formattedParentAreaName: string;
    path = this.ftHelperService.getURL().img;
    isError = false;
    errorMessage: string;
    showOverlay = false;
    currentPolygons: google.maps.Polygon[] = [];
    selectedPolygon: google.maps.Polygon = null;
    boundry: geoBoundry.Boundry;
    coreDataSet: CoreDataSet;
    baseMaps: BaseMap[] = [
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
    selectedbaseMap: BaseMap;
    fillOpacity = 1.0;
    opacityArray: number[] = [20, 40, 60, 80, 100];

    /*purple boundry on polygon with bold border -represent that polygon is highlighted but not in a table */
    purpleHighlightPolyOption: google.maps.PolygonOptions = { strokeColor: '#9D78D2', strokeWeight: 3 };
    /*black boundry on polygon with bold border -represent that polygon is in table */
    blackSelectedPolyOption: google.maps.PolygonOptions = { strokeColor: '#000000', strokeWeight: 3 };
    /*black boundry on polygon with bolder border -represent that polygon is in table as well as highlighted */
    blackHighlightPolyOption: google.maps.PolygonOptions = { strokeColor: '#000000', strokeWeight: 5 };
    /* gray boundry on polygon with normal regular border */
    grayPolyOption: google.maps.PolygonOptions = { strokeColor: '#333333', strokeWeight: 1 };
    /*black boundry on polygon with bold border -represent that polygon is in table */
    unselectedPolyOption: google.maps.PolygonOptions = { strokeColor: '#333333', strokeWeight: 1 };

    // Lightbox
    lightBoxConfig: LightBoxConfig;

    // Csv export
    public csvConfig: CsvConfig;

    constructor(private mapService: GoogleMapService,
        private indicatorService: IndicatorService,
        private ftHelperService: FTHelperService,
        private coreDataHelper: CoreDataHelperService,
        private downloadService: DownloadService,
        private deviceService: DeviceDetectorService
    ) { }

    ngOnChanges(changes: SimpleChanges) {
        this.isNearestNeighbours = this.ftHelperService.isNearestNeighbours();

        if (changes['areaTypeId']) {
            if (this.areaTypeId) {
                this.loadMap();
                this.loadBoundaries(this.areaTypeId, this.path);
            }
        }
        if (changes['currentAreaCode']) {
            const areaCode = changes['currentAreaCode'].currentValue;
            if (areaCode) {
                this.highlightPolygon(this.currentAreaCode);
            } else {
                this.unhighlightSelectedPolygon();
            }
        }
        if (changes['refreshColour']) {
            const localRefreshColour = changes['refreshColour'].currentValue;
            if (localRefreshColour) {
                if (this.areaCodeColour) {
                    this.colourFillPolygon(true);
                }
            }
        }
        if (changes['selectedAreaList']) {
            const localSelectedAreaList = changes['selectedAreaList'].currentValue;
            if (localSelectedAreaList) {
                this.removeSelectedPolygon();
            }
        }
    }

    unhighlightSelectedPolygon() {
        if (this.selectedPolygon) {
            this.selectedPolygon.setOptions(this.unselectedPolyOption);
        }
    }

    removeSelectedPolygon() {
        if (this.map) {
            for (let i = 0; i < this.currentPolygons.length; i++) {
                const areaCode = this.currentPolygons[i].get('areaCode');
                if (!_.contains(this.selectedAreaList, areaCode)) {
                    this.currentPolygons[i].setOptions(this.grayPolyOption);
                } else {
                    this.currentPolygons[i].setOptions(this.blackSelectedPolyOption);
                }
            }
        }
    }

    displayOverlay(): void {
        this.showOverlay = true;
    }

    hideOverlay(): void {
        this.showOverlay = false;
    }

    onOverlaySelectionChange(basemap): void {
        this.selectedbaseMap = Object.assign({}, this.selectedbaseMap, basemap);
        this.styleMap(this.selectedbaseMap);
    }

    onOpacitySelectionChange(opacity): void {
        this.fillOpacity = opacity / 100;
        this.loadBoundaries(this.areaTypeId, this.path);
    }

    loadMap() {
        if (!this.isBoundaryNotSupported) {
            // Load from GoogleMapService and style it
            const mapOptions: google.maps.MapOptions = {
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
            let mapContainer = null;
            if (this.mapEl && this.mapEl.nativeElement) {
                mapContainer = this.mapEl.nativeElement;
            }
            this.map = this.mapService.loadMap(mapContainer, mapOptions);
            this.selectedbaseMap = this.baseMaps[0];
            if (this.baseMaps) { // No Backgroud at the time of load
                this.onOverlaySelectionChange(this.baseMaps[0]);
            }
            if (this.areaTypeId) {
                this.loadBoundaries(this.areaTypeId, this.path);
            }

            this.mapInit.emit({
                map: this.map,
            });
        }
    }

    styleMap(selectedbaseMap: BaseMap): void {
        if (this.map) {
            const noTiles = 'noTiles';
            let styleArrayForNoBackground: any = [];
            if (selectedbaseMap.val === 0) {
                const visibilityOff: any = [{ visibility: 'off' }];
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
            google.maps.event.addListener(this.map, 'idle', event => {
                const bounds = this.map.getCenter();
                google.maps.event.trigger(this.map, 'resize');
                this.map.setCenter(bounds);
            });
        }
    }

    setMapCenter() {
        if (this.map) {
            const bounds = new google.maps.LatLngBounds();
            const position = new google.maps.LatLng(53.415649, -2.209015);
            bounds.extend(position);
            this.map.setCenter(bounds.getCenter());
            this.map.setZoom(6);
        }
    }

    loadBoundaries(areaTypeId: number, path: string) {
        if (this.map && this.areaTypeId && !this.isBoundaryNotSupported) {
            this.mapService.loadBoundries(areaTypeId, path)
                .subscribe(
                    data => {
                        this.boundry = <geoBoundry.Boundry>data;
                        this.isError = false;
                        this.removePolygon();
                        this.fillPolygon(this.boundry, this.fillOpacity);
                        this.colourFillPolygon(true);
                    },
                    error => {
                        this.isError = true;
                        this.errorMessage = <any>error;
                    });
        }
    }

    removePolygon(): void {
        if (isDefined(this.currentPolygons)) {
            this.currentPolygons.forEach(element => {
                element.setMap(null);
            });
            this.currentPolygons.length = 0;
        }
    }

    getPolygonCoordinates(coordinates): Array<Array<google.maps.LatLng>> {

        const coords: Array<Array<google.maps.LatLng>> = [];
        for (let i = 0; i < coordinates.length; i++) {
            for (let j = 0; j < coordinates[i].length; j++) {
                const path: Array<google.maps.LatLng> = [];
                for (let k = 0; k < coordinates[i][j].length; k++) {
                    const coord = new google.maps.LatLng(coordinates[i][j][k][1], coordinates[i][j][k][0]);
                    path.push(coord);
                }
                coords.push(path);
            }
        }
        return coords;
    }

    fillPolygon(boundry: geoBoundry.Boundry, opacity: number): any {
        if (boundry.features) {

            // Variables to track most recent mouseover event
            let overDate = null;
            let overAreaCode = null;

            const infoWindow: google.maps.InfoWindow = new google.maps.InfoWindow();

            for (let x = 0; x < boundry.features.length; x++) {
                const areaCode = boundry.features[x].properties.AreaCode;
                const coordinates = boundry.features[x].geometry.coordinates;
                const coords = this.getPolygonCoordinates(coordinates);

                // Set polygon fill colour
                let fillColour = '#B0B0B2';
                if (this.areaCodeColour && this.areaCodeColour.length > 0) {
                    fillColour = this.areaCodeColour.get(areaCode);
                }

                const polygon = new google.maps.Polygon({
                    paths: coords,
                    strokeColor: '#333333',
                    strokeOpacity: 0.8,
                    strokeWeight: 1,
                    fillColor: fillColour,
                    fillOpacity: opacity,
                    clickable: true
                });

                polygon.set('areaCode', areaCode);
                polygon.setMap(this.map);

                google.maps.event.addListener(polygon, 'mouseover', (event) => {

                    overAreaCode = areaCode;
                    overDate = new Date();

                    // Display tooltip
                    const tooltip = this.getToolTipContent(areaCode);
                    if (tooltip) {
                        infoWindow.setContent(tooltip);
                        this.setInfoWindowPosition(event, infoWindow);
                        infoWindow.open(this.map);
                    }

                    this.setPolygonBorderColour(polygon);
                });

                google.maps.event.addListener(polygon, 'mousemove', (event) => {
                    this.setInfoWindowPosition(event, infoWindow);
                });

                google.maps.event.addListener(polygon, 'mouseout', (event) => {

                    // Wait in case immediate mouseover event and this mouseover event was
                    // caused by mouse moving over the infowindow
                    setTimeout(function () {
                        const time = new Date().getTime();
                        if (time - overDate.getTime() > 25 && areaCode === overAreaCode) {
                            infoWindow.close();
                        }
                    }, 25);
                    this.setPolygonBorderColour(polygon);
                });

                google.maps.event.addListener(polygon, 'click', (event) => {
                    if (this.sortedCoreData[areaCode] && this.ftHelperService.isValuePresent(this.sortedCoreData[areaCode].ValF)) {
                        polygon.setOptions(this.blackSelectedPolyOption);
                        this.selectedAreaChanged.emit({ areaCode: areaCode });
                    }
                });

                this.currentPolygons.push(polygon);
            }
        }
    }

    setPolygonBorderColour(polygon) {
        const currentAreaCode = polygon.get('areaCode');
        if (_.contains(this.selectedAreaList, currentAreaCode)) {
            polygon.setOptions(this.blackSelectedPolyOption);
        } else {
            polygon.setOptions(this.grayPolyOption);
        }
    }

    setInfoWindowPosition(event, infoWindow) {
        const pos: google.maps.LatLng = event.latLng;
        infoWindow.setPosition(new google.maps.LatLng(pos.lat() + 0.02, pos.lng()));
    }

    getToolTipContent(areaCode: string): string {
        this.hoverAreaCodeChanged.emit({ areaCode: areaCode });
        const currentGrpRoot: GroupRoot = this.ftHelperService.getCurrentGroupRoot();
        const data: IndicatorMetadata = this.ftHelperService.getMetadata(currentGrpRoot.IID);
        const unit = data.Unit;
        let areaName = '';
        if (areaCode) {
            areaName = this.ftHelperService.getAreaName(areaCode);
        }
        let value = '';
        if (unit && isDefined(this.sortedCoreData[areaCode]) &&
            this.ftHelperService.isValuePresent(this.sortedCoreData[areaCode].ValF)) {
            value = this.sortedCoreData !== null ? '<br>' + this.coreDataHelper.valueWithUnit(unit).
                getFullLabel(this.sortedCoreData[areaCode].ValF)
                : '<br>-';
        }

        let tooltipContent;
        if (areaName !== '' || value !== '') {
            tooltipContent = '<b>' + areaName + '</b>' + value;
        }
        return tooltipContent;
    }

    highlightPolygon(areaCode): void {
        if (this.map) {
            const polygon: google.maps.Polygon = _.where(this.currentPolygons, { areaCode: areaCode })[0];
            if (this.selectedPolygon) {
                const selectedPolygonAreaCode = this.selectedPolygon.get('areaCode');
                this.selectedPolygon.setMap(null);
                if (_.contains(this.selectedAreaList, selectedPolygonAreaCode)) {
                    this.selectedPolygon.setOptions(this.blackSelectedPolyOption);
                } else {
                    this.selectedPolygon.setOptions(this.grayPolyOption);
                }
                this.selectedPolygon.setMap(this.map);
            }

            this.selectedPolygon = polygon;

            if (polygon) {
                polygon.setMap(null);
                if (!_.contains(this.selectedAreaList, areaCode)) {
                    polygon.setOptions(this.purpleHighlightPolyOption);
                } else {
                    polygon.setOptions(this.blackHighlightPolyOption);
                }
                polygon.setMap(this.map);
            }
        }
    }

    colourFillPolygon(center: boolean): void {
        const parentTypeId = this.ftHelperService.getParentTypeId();
        const areaTypeId = this.ftHelperService.getAreaTypeId();

        if (isDefined(parentTypeId) && parentTypeId !== null
            && isDefined(areaTypeId)) {

            const isSubnationalComparator = this.benchmarkIndex === ComparatorIds.SubNational;

            // Mapping between parent and child areas if subnational comparator
            let areaMappings = null;
            if (isSubnationalComparator) {
                const key = parentTypeId.toString() + '-' + areaTypeId.toString() + '-';
                areaMappings = this.ftHelperService.getAreaMappingsForParentCode(key);

                const areaCode = this.ftHelperService.getFTModel().areaCode;
                if (_.findIndex(areaMappings, areaCode) === -1) {
                    areaMappings.push(areaCode);
                }
            }

            if (this.map) {
                const subnationalPolygons: Array<google.maps.Polygon> = [];
                const undefinedColour = '#B0B0B2';

                for (let i = 0; i < (this.currentPolygons.length); i++) {
                    const polygon = this.currentPolygons[i];

                    // Set polygon fill colour
                    polygon.setMap(null);
                    const areaCode = polygon.get('areaCode');
                    let color = this.areaCodeColour.get(areaCode);

                    // Region tab button clicked
                    if (isSubnationalComparator &&
                        areaMappings.findIndex(x => x.toString() === areaCode) === -1) {

                        color = undefinedColour;
                    }

                    // Set to default color if not defined
                    if (color === undefined) {
                        color = undefinedColour;
                    }

                    polygon.set('fillColor', color);
                    polygon.setMap(this.map);

                    if (isSubnationalComparator &&
                        areaMappings.findIndex(x => x.toString() === areaCode) !== -1) {

                        const coreDataset = this.sortedCoreData[areaCode];
                        if (coreDataset) {
                            subnationalPolygons.push(polygon);
                        }
                    }
                }

                // Centre on map area
                if (center) {
                    if (subnationalPolygons.length > 0 && isSubnationalComparator && !this.isNearestNeighbours) {
                        // Benchmark is subnational, center and zoom in into that region
                        const bounds = new google.maps.LatLngBounds();
                        for (let i = 0; i < subnationalPolygons.length; i++) {
                            bounds.extend(this.getPolygonBounds(subnationalPolygons[i]).getCenter());
                        }
                        this.map.fitBounds(bounds);
                        this.map.setCenter(bounds.getCenter());

                        if (areaTypeId === AreaTypeIds.MSOA || areaTypeId === AreaTypeIds.Ward) {
                            this.map.setZoom(10);
                        } else {
                            this.map.setZoom(7);
                        }
                    }
                    if (this.benchmarkIndex === ComparatorIds.National ||
                        (isSubnationalComparator && this.isNearestNeighbours)) {
                        // Centre on
                        this.setMapCenter();
                    }
                }

                // Label for subnational option
                if (this.isNearestNeighbours) {
                    const model = this.ftHelperService.getFTModel();
                    const area = this.ftHelperService.getArea(model.areaCode);
                    const nnAreaName = new AreaHelper(area).getShortAreaNameToDisplay();
                    this.formattedParentAreaName = nnAreaName + ' and neighbours';
                } else {
                    const parentAreaName = new ParentAreaHelper(this.ftHelperService).getParentAreaName();
                    this.formattedParentAreaName = 'All in ' + parentAreaName;
                }

                setTimeout(() => {
                    this.applyTabButtonStyles();
                }, 0);
            }
        }
    }

    getPolygonBounds(polygon: google.maps.Polygon) {
        const bounds = new google.maps.LatLngBounds();
        polygon.getPath().forEach(function (element, index) { bounds.extend(element); });
        return bounds;
    }

    onExportClick(event: MouseEvent) {
        // Download map does not work on IE
        // Check whether the browser is IE
        if (new DeviceServiceHelper(this.deviceService).isIE()) {
            // Display lightbox informing user to use a different browser
            const config = new LightBoxConfig();
            config.Type = LightBoxTypes.Ok;
            config.Title = 'Browser not compatible';
            config.Html = 'Exporting the map as an image is not supported by Internet Explorer. Please use a different browser.'
            config.Height = 200;
            config.Top = 500;
            this.lightBoxConfig = config;
        } else {
            event.preventDefault();
            const chartTitle = this.buildChartTitle();
            const root = this.ftHelperService.getCurrentGroupRoot();
            const indicatorName = this.ftHelperService.getMetadataHash()[root.IID].Descriptive.Name +
                this.ftHelperService.getSexAndAgeLabel(root);

            // Define html to display the title
            const title = '<b>Map of ' + this.ftHelperService.getAreaTypeName() +
                's in ' + this.ftHelperService.getCurrentComparator().Name +
                ' for ' + indicatorName + '<br/> (' + chartTitle + ')</b>';

            // Define script to hide the zoom in, zoom out and full screen buttons
            let script = '<script>$(".gmnoprint").hide(); $(".gm-fullscreen-control").hide();</script>';

            // Inject both the title html and button hide script
            $('<div id="map-export-title" style="text-align: center; font-family:Arial;">' +
                title + script + '</div>').appendTo(this.mapEl.nativeElement);

            // Hide "Scroll to zoom" message and background in case displayed
            $('.gm-style-pbt').hide();
            $('.gm-style-pbc').hide();

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
        }
    }

    onExportCsvFileClick(event: MouseEvent) {
        const csvData: CsvData[] = [];

        const indicatorId = this.ftHelperService.getIid();
        const indicatorName = this.ftHelperService.getIndicatorName(indicatorId);
        const areaTypeName = this.ftHelperService.getAreaTypeName();
        const areaList = this.ftHelperService.getAreaList();
        const root = this.ftHelperService.getCurrentGroupRoot();
        const comparatorId = this.ftHelperService.getComparatorId();

        this.currentPolygons.forEach(polygon => {
            const areaCode: string = polygon['areaCode'];
            const fillColor: string = polygon['fillColor'];

            if (fillColor.toUpperCase() !== '#B0B0B2') {
                const area = areaList.find(x => x.Code === areaCode);
                const areaName = isDefined(area) ? area.Name : '';

                const data = this.addCsvRow(areaCode, indicatorId, indicatorName, areaName, areaTypeName, root);
                csvData.push(data);
            }
        });

        this.csvConfig = new CsvConfig();
        this.csvConfig.tab = Tabs.Map;
        this.csvConfig.csvData = csvData;
    }

    addCsvRow(areaCode: string, indicatorId: number, indicatorName: string,
        areaName: string, areaTypeName: string, root: GroupRoot): CsvData {

        const data = new CsvData();
        const coreData: CoreDataSet = this.sortedCoreData[areaCode];

        data.indicatorId = indicatorId.toString();
        data.indicatorName = indicatorName

        const parentAreaHelper = new ParentAreaHelper(this.ftHelperService);

        if (this.benchmarkIndex === ComparatorIds.National) {
            data.parentCode = AreaCodes.England;
            data.parentName = 'England';
        } else {
            data.parentCode = parentAreaHelper.getParentAreaCode();
            data.parentName = parentAreaHelper.getParentAreaNameForCSV();
        }

        data.areaCode = areaCode;
        data.areaName = areaName;
        data.areaType = areaTypeName;

        data.sex = root.Sex.Name;
        data.age = root.Age.Name;

        data.categoryType = CsvDataHelper.getDisplayValue(coreData.CategoryTypeId);
        data.category = CsvDataHelper.getDisplayValue(coreData.CategoryId);
        data.timePeriod = root.Grouping[0].Period;
        data.value = CsvDataHelper.getDisplayValue(coreData.Val);
        data.lowerCiLimit95 = CsvDataHelper.getDisplayValue(coreData.LoCI);
        data.upperCiLimit95 = CsvDataHelper.getDisplayValue(coreData.UpCI);
        data.lowerCiLimit99_8 = CsvDataHelper.getDisplayValue(coreData.LoCI99_8);
        data.upperCiLimit99_8 = CsvDataHelper.getDisplayValue(coreData.UpCI99_8);
        data.count = CsvDataHelper.getDisplayValue(coreData.Count);
        data.denominator = CsvDataHelper.getDisplayValue(coreData.Denom);

        data.valueNote = '';
        if (isDefined(coreData.NoteId)) {
            data.valueNote = this.ftHelperService.newValueNoteTooltipProvider().getTextFromNoteId(coreData.NoteId);
        }

        data.recentTrend = '';
        if (isDefined(root.RecentTrends)) {
            const recentTrends: TrendMarkerResult = root.RecentTrends[coreData.AreaCode];
            if (isDefined(recentTrends)) {
                data.recentTrend = new TrendMarkerLabelProvider(root.PolarityId).getLabel(recentTrends.Marker);
            }
        }

        data.comparedToEnglandValueOrPercentiles = CsvDataHelper.getSignificanceValue(coreData,
            root.PolarityId, ComparatorIds.National, root.ComparatorMethodId);

        data.comparedToRegionValueOrPercentiles = CsvDataHelper.getSignificanceValue(coreData,
            root.PolarityId, ComparatorIds.SubNational, root.ComparatorMethodId);

        data.timePeriodSortable = new TimePeriod(root.Grouping[0]).getSortableNumber();

        const hasDataChanged = this.ftHelperService.hasDataChanged(root);
        const isNewData = NewDataBadgeHelper.isNewData(hasDataChanged);
        data.newData = isNewData ? 'New data' : '';

        data.comparedToGoal = CsvDataHelper.getSignificanceValue(coreData, root.PolarityId, ComparatorIds.Target, root.ComparatorMethodId);

        return data;
    }

    buildChartTitle(): string {
        const currentGrpRoot: GroupRoot = this.ftHelperService.getCurrentGroupRoot();
        const data: IndicatorMetadata = this.ftHelperService.getMetadata(currentGrpRoot.IID);
        const unit = data.Unit;
        const unitLabel = (typeof unit.Label !== 'undefined') ? unit.Label : '';
        const period: string = currentGrpRoot.Grouping[0].Period;
        return data.ValueType.Name + ' - ' + unitLabel + ' ' + period;
    }

    setBenchMark(benchmarkIndex) {
        this.benchmarkIndex = benchmarkIndex;
        this.benchmarkChanged.emit({ benchmarkIndex: benchmarkIndex });
        return false;
    }

    applyTabButtonStyles() {
        if (this.ftHelperService.getParentTypeId() === AreaTypeIds.Country) {
            this.nationalButtonClass = 'button-selected';
            this.subNationalButtonClass = 'hidden';
            return;
        }

        if (this.benchmarkIndex === undefined || this.benchmarkIndex === ComparatorIds.National) {
            this.nationalButtonClass = 'button-selected';
            this.subNationalButtonClass = '';
            return;
        }

        if (this.benchmarkIndex === ComparatorIds.SubNational) {
            this.nationalButtonClass = '';
            this.subNationalButtonClass = 'button-selected';
            return;
        }
    }

    updateLightBoxActionConfirmed(event) {
        // Lightbox button event
        // Place holder in case required in future
    }
}

export class NoTileMapType implements google.maps.MapType {
    tileSize = new google.maps.Size(1024, 1024);
    maxZoom = 20;
    getTile(tileCoord: google.maps.Point, zoom: number, ownerDocument: Document): Element {
        return ownerDocument.createElement('div');
    }
    releaseTile(tile: Element): void {
        throw new Error('Method not implemented.');
    }
}

export interface BaseMap {
    name: string; val: number; cssClass: string;
}
