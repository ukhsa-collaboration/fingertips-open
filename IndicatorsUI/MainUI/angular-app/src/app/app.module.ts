import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ComponentFactoryResolver, ApplicationRef, Type  } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { AppComponent } from './app.component';
import {MapComponent} from './map/map.component';
import { MapModule } from './map/map.module';
import { BoxplotModule } from './boxplot/boxplot.module';
import { SharedModule } from './shared/shared.module';
import { CoreDataHelperService } from './shared/service/helper/coreDataHelper.service';
import { FTHelperService } from './shared/service/helper/ftHelper.service';
import { EnglandComponent } from './england/england.component';
import { MetadataModule } from './metadata/metadata.module';
import { PopulationModule } from './population/population.module';
import { MetadataTableComponent } from './metadata/metadata-table/metadata-table.component';
import { TypeaheadModule } from 'ngx-bootstrap';

const components = [AppComponent, MapComponent];
@NgModule({
  declarations: [
    AppComponent,
    EnglandComponent,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpModule,
    MapModule,
    BoxplotModule,
    PopulationModule,
    SharedModule,
    MetadataModule,
    TypeaheadModule.forRoot()
  ],
  providers: [CoreDataHelperService, FTHelperService],
  entryComponents: components
  // Instead of bootstraping a single component, with overriding mechnisam, multiple componenta are bootstrapped
  // bootstrap: [AppComponent]
})
export class AppModule {
  constructor(private resolver: ComponentFactoryResolver) { }
  // Overriding angular original behaviour, This will bootsttrap all the component defined
  // in the array if the component tag is found on the document
  ngDoBootstrap(appRef: ApplicationRef) {
      components.forEach((componentDef: Type<{}>) => {
          const factory = this.resolver.resolveComponentFactory(componentDef);
          if (document.querySelector(factory.selector)) {
              appRef.bootstrap(factory);
          }
      });
  }
}
