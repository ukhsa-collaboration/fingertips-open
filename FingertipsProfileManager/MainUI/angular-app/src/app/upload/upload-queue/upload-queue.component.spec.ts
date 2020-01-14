import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadQueueComponent } from './upload-queue.component';

describe('UploadQueueComponent', () => {
  let component: UploadQueueComponent;
  let fixture: ComponentFixture<UploadQueueComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UploadQueueComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadQueueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
