import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { TrendChartComponent } from './trend-chart.component';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { CoreDataHelperService } from '../../shared/service/helper/coreDataHelper.service';

describe('TrendChartComponent', () => {
  let component: TrendChartComponent;
  let fixture: ComponentFixture<TrendChartComponent>;

  let ftHelperService: any;
  let coreDataHelperService: any;

  beforeEach(async(() => {

    ftHelperService = jasmine.createSpyObj('FTHelperService', ['']);
    coreDataHelperService = jasmine.createSpyObj('CoreDataHelperService', ['']);

    TestBed.configureTestingModule({
      declarations: [TrendChartComponent],
      providers: [
        { provide: FTHelperService, useValue: ftHelperService },
        { provide: CoreDataHelperService, useValue: coreDataHelperService }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TrendChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
