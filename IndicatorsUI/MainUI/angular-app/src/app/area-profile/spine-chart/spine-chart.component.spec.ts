import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SpineChartComponent } from './spine-chart.component';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';

describe('SpineChartComponent', () => {
  let component: SpineChartComponent;
  let fixture: ComponentFixture<SpineChartComponent>;

  let ftHelperService: any;

  beforeEach(async(() => {

    ftHelperService = jasmine.createSpyObj('FTHelperService', ['']);

    TestBed.configureTestingModule({
      declarations: [SpineChartComponent],
      providers: [
        { provide: FTHelperService, useValue: ftHelperService }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SpineChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
