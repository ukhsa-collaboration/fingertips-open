import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadIndexComponent } from './upload-index.component';

describe('UploadIndexComponent', () => {
  let component: UploadIndexComponent;
  let fixture: ComponentFixture<UploadIndexComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UploadIndexComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadIndexComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
