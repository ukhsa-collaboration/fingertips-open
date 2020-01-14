import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { ScatterPlotChartComponent } from './scatter-plot-chart.component';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { DownloadService } from '../../shared/service/api/download.service';
import { IndicatorService } from '../../shared/service/api/indicator.service';

describe('ScatterPlotChartComponent', () => {
  let component: ScatterPlotChartComponent;
  let fixture: ComponentFixture<ScatterPlotChartComponent>;
  let ftHelperService: any;
  let downloadService: any;
  let indicatorService: any;

  beforeEach(async(() => {
    ftHelperService = jasmine.createSpyObj('FTHelperService', ['']);
    downloadService = jasmine.createSpyObj('DownloadService', ['']);
    indicatorService = jasmine.createSpyObj('IndicatorService', ['']);

    TestBed.configureTestingModule({
      declarations: [ ScatterPlotChartComponent ],
      providers: [
        { provide: FTHelperService, useValue: ftHelperService },
        { provide: DownloadService, useValue: downloadService },
        { provide: IndicatorService, useValue: indicatorService }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScatterPlotChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
