import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InequalitiesBarChartComponent } from './inequalities-bar-chart.component';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { CoreDataHelperService } from '../../shared/service/helper/coreDataHelper.service';

describe('InequalitiesBarChartComponent', () => {
  let component: InequalitiesBarChartComponent;
  let fixture: ComponentFixture<InequalitiesBarChartComponent>;

  let ftHelperService: any;
  let coreDataHelperService: any;

  beforeEach(async(() => {

    ftHelperService = jasmine.createSpyObj('FTHelperService', ['']);
    coreDataHelperService = jasmine.createSpyObj('CoreDataHelperService', ['']);

    TestBed.configureTestingModule({
      declarations: [InequalitiesBarChartComponent],
      providers: [
        { provide: FTHelperService, useValue: ftHelperService },
        { provide: CoreDataHelperService, useValue: coreDataHelperService }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InequalitiesBarChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
