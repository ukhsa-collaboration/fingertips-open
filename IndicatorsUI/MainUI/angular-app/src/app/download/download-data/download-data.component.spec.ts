import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { DownloadDataComponent } from './download-data.component';

describe('DownloadDataComponent', () => {
  let component: DownloadDataComponent;
  let fixture: ComponentFixture<DownloadDataComponent>;

  let ftHelperService: any;

  beforeEach(async(() => {

    ftHelperService = jasmine.createSpyObj('FTHelperService', ['']);

    TestBed.configureTestingModule({
      providers: [
        { provide: FTHelperService, useValue: ftHelperService }
      ],
      declarations: [DownloadDataComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DownloadDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
