import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { InequalitiesTrendTableComponent } from './inequalities-trend-table.component';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';

describe('InequalitiesTrendTableComponent', () => {
  let component: InequalitiesTrendTableComponent;
  let fixture: ComponentFixture<InequalitiesTrendTableComponent>;

  let mockFTHelperService: any;

  beforeEach(async(() => {

    mockFTHelperService = jasmine.createSpyObj('FTHelperService', ['']);

    TestBed.configureTestingModule({
      declarations: [
        InequalitiesTrendTableComponent
      ],
      providers: [
        { provide: FTHelperService, useValue: mockFTHelperService }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InequalitiesTrendTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
