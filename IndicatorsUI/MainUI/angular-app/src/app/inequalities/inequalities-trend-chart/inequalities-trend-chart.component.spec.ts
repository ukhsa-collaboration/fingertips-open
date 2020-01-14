import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { InequalitiesTrendChartComponent } from './inequalities-trend-chart.component';
import { CoreDataHelperService } from '../../shared/service/helper/coreDataHelper.service';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';

describe('InequalitiesTrendChartComponent', () => {
  let component: InequalitiesTrendChartComponent;
  let fixture: ComponentFixture<InequalitiesTrendChartComponent>;

  let ftHelperService: any;
  let coreDataHelperService: any;

  beforeEach(async(() => {

    ftHelperService = jasmine.createSpyObj('FTHelperService', ['']);
    coreDataHelperService = jasmine.createSpyObj('CoreDataHelperService', ['valueWithUnit']);

    TestBed.configureTestingModule({
      declarations: [InequalitiesTrendChartComponent],
      providers: [
        { provide: FTHelperService, useValue: ftHelperService },
        { provide: CoreDataHelperService, useValue: coreDataHelperService }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InequalitiesTrendChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
