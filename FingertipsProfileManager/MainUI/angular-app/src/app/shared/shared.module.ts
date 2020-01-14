import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SpinnerComponent } from './component/spinner/spinner.component';
import { HttpCacheService } from '../services/http-cache.service';
import { HttpService } from '../services/http.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  declarations: [
    SpinnerComponent
  ],
  exports: [
    SpinnerComponent
  ],
  providers: [
    HttpCacheService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpService,
      multi: true
    },
    HttpService,
  ]
})
export class SharedModule { }
