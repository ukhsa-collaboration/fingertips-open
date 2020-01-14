import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CompareAreaTableComponent } from './compare-area-table.component';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';

describe('CompareAreaTableComponent', () => {
  let component: CompareAreaTableComponent;
  let fixture: ComponentFixture<CompareAreaTableComponent>;

  let ftHelperService: any;

  beforeEach(async(() => {

    ftHelperService = jasmine.createSpyObj('FTHelperService', ['getFTModel', 'isNearestNeighbours']);

    TestBed.configureTestingModule({
      declarations: [CompareAreaTableComponent],
      providers: [
        { provide: FTHelperService, useValue: ftHelperService }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CompareAreaTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
