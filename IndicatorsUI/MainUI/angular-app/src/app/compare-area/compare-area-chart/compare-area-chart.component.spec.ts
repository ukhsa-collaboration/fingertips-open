import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CompareAreaChartComponent } from './compare-area-chart.component';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { CoreDataHelperService } from '../../shared/service/helper/coreDataHelper.service';

describe('CompareAreaChartComponent', () => {
  let component: CompareAreaChartComponent;
  let fixture: ComponentFixture<CompareAreaChartComponent>;

  let ftHelperService: any;
  let coreDataHelperService: any;

  beforeEach(async(() => {

    ftHelperService = jasmine.createSpyObj('FTHelperService', ['']);
    coreDataHelperService = jasmine.createSpyObj('CoreDataHelperService', ['']);

    TestBed.configureTestingModule({
      declarations: [CompareAreaChartComponent],
      providers: [
        { provide: FTHelperService, useValue: ftHelperService },
        { provide: CoreDataHelperService, useValue: coreDataHelperService }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CompareAreaChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
