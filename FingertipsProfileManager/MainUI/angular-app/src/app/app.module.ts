import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ComponentFactoryResolver, ApplicationRef, Type } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { ReportsService } from './services/reports.service';
import { ProfileService } from './services/profile.service';
import { ReportsComponent } from './reports/reports.component';
import { ReportParametersComponent } from './reports/report-parameters/report-parameters.component';
import { ReportProfilesComponent } from './reports/report-profiles/report-profiles.component';
import { ReportListViewComponent } from './reports/report-list-view/report-list-view.component';
import { ReportEditViewComponent } from './reports/report-edit-view/report-edit-view.component';
import { IndicatorsReorderViewComponent } from './indicators/indicators-reorder-view/indicators-reorder-view.component';
import { HttpService } from './services/http.service';
import { SharedModule } from './shared/shared.module';

const rootComponents = [ReportsComponent, IndicatorsReorderViewComponent];

@NgModule({
  declarations: [
    ReportsComponent,
    ReportParametersComponent,
    ReportProfilesComponent,
    ReportListViewComponent,
    ReportEditViewComponent,
    IndicatorsReorderViewComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpModule,
    SharedModule,
    ReactiveFormsModule
  ],
  providers: [
    ReportsService,
    ProfileService,
    HttpService
  ],
  entryComponents: rootComponents
  // Instead of bootstraping a single component, with overriding mechanism, multiple components are bootstrapped
  //bootstrap: [AppComponent]
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
