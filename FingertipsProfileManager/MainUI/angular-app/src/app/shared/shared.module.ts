import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SpinnerComponent } from './component/spinner/spinner.component';
import { LightBoxComponent } from './component/light-box/light-box.component';
import { LightBoxIndicatorReorderComponent } from './component/light-box-indicator-reorder/light-box-indicator-reorder.component';
import { ProfileListComponent } from './component/profile-list/profile-list.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  declarations: [
    SpinnerComponent,
    LightBoxComponent,
    LightBoxIndicatorReorderComponent,
    ProfileListComponent
  ],
  exports: [
    SpinnerComponent,
    LightBoxComponent,
    LightBoxIndicatorReorderComponent,
    ProfileListComponent
  ],
})
export class SharedModule { }
