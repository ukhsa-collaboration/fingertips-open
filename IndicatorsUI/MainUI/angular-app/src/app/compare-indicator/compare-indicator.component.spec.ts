import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CompareIndicatorComponent } from './compare-indicator.component';
import { ScatterPlotChartComponent } from './scatter-plot-chart/scatter-plot-chart.component';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { IndicatorService } from '../shared/service/api/indicator.service';
import { UIService } from '../shared/service/helper/ui.service';
import { DownloadService } from '../shared/service/api/download.service';
import { ExportCsvComponent } from '../shared/component/export-csv/export-csv.component';

describe('CompareIndicatorComponent', () => {
  let component: CompareIndicatorComponent;
  let fixture: ComponentFixture<CompareIndicatorComponent>;

  let ftHelperService: any;
  let uiService: any;
  let downloadService: any;
  let indicatorService: any;

  beforeEach(async(() => {

    ftHelperService = jasmine.createSpyObj('FTHelperService', ['getAreaTypeId']);
    uiService = jasmine.createSpyObj('UIService', ['']);
    downloadService = jasmine.createSpyObj('DownloadService', ['']);
    indicatorService = jasmine.createSpyObj('IndicatorService', ['']);

    TestBed.configureTestingModule({
      declarations: [
        CompareIndicatorComponent,
        ScatterPlotChartComponent,
        ExportCsvComponent
      ],
      imports: [FormsModule],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        { provide: FTHelperService, useValue: ftHelperService },
        { provide: UIService, useValue: uiService },
        { provide: DownloadService, useValue: downloadService },
        { provide: IndicatorService, useValue: indicatorService }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CompareIndicatorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should order alphabeticaly', () => {
    const sorting = [
      { IndicatorName: 'd' },
      { IndicatorName: 'c' },
      { IndicatorName: 'a' },
      { IndicatorName: 'b' },
      { IndicatorName: 'e' }
    ];

    const sorted = [
      { IndicatorName: 'a' },
      { IndicatorName: 'b' },
      { IndicatorName: 'c' },
      { IndicatorName: 'd' },
      { IndicatorName: 'e' }
    ];

    const result = sorting.sort(component.sortCompareIndicators);

    expect(result).toEqual(sorted);
  });

});
