import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadProgressTableComponent } from './upload-progress-table.component';

describe('UploadProgressTableComponent', () => {
  let component: UploadProgressTableComponent;
  let fixture: ComponentFixture<UploadProgressTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UploadProgressTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadProgressTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
