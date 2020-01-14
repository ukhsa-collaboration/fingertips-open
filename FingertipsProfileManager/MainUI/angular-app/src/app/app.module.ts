import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ComponentFactoryResolver, ApplicationRef, Type } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { ReportsService } from './services/reports.service';
import { ProfileService } from './services/profile.service';
import { IndicatorsReorderViewComponent } from './indicators/indicators-reorder-view/indicators-reorder-view.component';
import { SharedModule } from './shared/shared.module';
import { ReportsModule } from './reports/reports.module';
import { UploadModule } from './upload/upload.module';
import { LightBoxModule } from './shared/component/light-box/light-box.module';
import { LightBoxIndicatorReorderModule } from './shared/component/light-box-indicator-reorder/light-box-indicator-reorder.module';
import { ReportsComponent } from './reports/reports.component';
import { NgSelect2Module } from 'ng-select2';
import { UploadComponent } from './upload/upload.component';
import { UsersService } from './services/users.service';

const rootComponents = [ReportsComponent, IndicatorsReorderViewComponent, UploadComponent];

@NgModule({
  declarations: [
    IndicatorsReorderViewComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    SharedModule,
    NgSelect2Module,
    ReportsModule,
    UploadModule,
    ReactiveFormsModule,
    LightBoxModule,
    LightBoxIndicatorReorderModule
  ],
  providers: [
    ReportsService,
    ProfileService,
    UsersService
  ],
  entryComponents: rootComponents
  // Instead of bootstraping a single component, with overriding mechanism, multiple components are bootstrapped
  // bootstrap: [AppComponent]
})
export class AppModule {
  constructor(private resolver: ComponentFactoryResolver) { }
  // Overriding angular original behaviour, This will bootsttrap all the component defined
  // in the array if the component tag is found on the document
  ngDoBootstrap(appRef: ApplicationRef) {
    rootComponents.forEach((componentDef: Type<{}>) => {
      const factory = this.resolver.resolveComponentFactory(componentDef);
      if (document.querySelector(factory.selector)) {
        appRef.bootstrap(factory);
      }
    });
  }
}
