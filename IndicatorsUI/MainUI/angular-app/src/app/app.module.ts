import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { AppComponent } from './app.component';
import {MapModule} from './map/map.module';
import {BridgeDataHelperService} from './shared/service/helper/bridgeDataHelper.service';
import {CoreDataHelperService} from './shared/service/helper/coreDataHelper.service';
import {FTHelperService} from './shared/service/helper/ftHelper.service';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpModule,
    MapModule
  ],
  providers: [BridgeDataHelperService,CoreDataHelperService,FTHelperService],
  bootstrap: [AppComponent]
})
export class AppModule { }

