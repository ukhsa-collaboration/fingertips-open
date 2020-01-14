import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ComponentFactoryResolver, ApplicationRef, Type } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { TypeaheadModule } from 'ngx-bootstrap';
import { MapComponent } from './map/map.component';
import { MapModule } from './map/map.module';
import { SharedModule } from './shared/shared.module';
import { BoxplotModule } from './boxplot/boxplot.module';
import { MetadataModule } from './metadata/metadata.module';
import { PopulationModule } from './population/population.module';
import { ArealistModule } from './arealist/arealist.module';
import { ArealistIndexComponent } from './arealist/arealist-index/arealist-index.component';
import { ArealistManageComponent } from './arealist/arealist-manage/arealist-manage.component';
import { DownloadModule } from './download/download.module';
import { DataViewComponent } from './data-view/data-view.component';
import { ReportsComponent } from './reports/reports.component';
import { AreaProfileModule } from './area-profile/area-profile.module';
import { EnglandModule } from './england/england.module';
import { CompareIndicatorModule } from './compare-indicator/compare-indicator.module';
import { CompareAreaModule } from './compare-area/compare-area.module';
import { TrendModule } from './trend/trend.module';
import { InequalitiesModule } from './inequalities/inequalities.module';
import { LightBoxModule } from './shared/component/light-box/light-box.module';
import { LightBoxWithInputModule } from './shared/component/light-box-with-input/light-box-with-input.module';

const rootComponents = [MapComponent, ArealistIndexComponent, ArealistManageComponent,
  DataViewComponent];

@NgModule({
  declarations: [
    DataViewComponent,
    ReportsComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    MapModule,
    BoxplotModule,
    DownloadModule,
    PopulationModule,
    AreaProfileModule,
    SharedModule,
    MetadataModule,
    ArealistModule,
    EnglandModule,
    CompareIndicatorModule,
    CompareAreaModule,
    TrendModule,
    InequalitiesModule,
    LightBoxModule,
    LightBoxWithInputModule,
    TypeaheadModule.forRoot(),
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
