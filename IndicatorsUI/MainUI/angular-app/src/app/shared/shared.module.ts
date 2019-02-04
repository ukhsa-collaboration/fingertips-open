import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

// Helper services 
import { CoreDataHelperService } from './service/helper/coreDataHelper.service';
import { FTHelperService } from './service/helper/ftHelper.service';
import { LightBoxService } from './service/helper/light-box.service';
import { UIService } from './service/helper/ui.service';

// Components
import { IndicatorHeaderComponent } from './component/indicator-header/indicator-header.component';

// API services
import { HttpService } from './service/api/http.service';
import { StaticReportsService } from './service/api/static-reports.service';
import { ProfileService } from './service/api/profile.service';
import { ContentService } from './service/api/content.service';
import { SsrsReportService } from './service/api/ssrs-report.service';

@NgModule({
    imports: [
        CommonModule
    ],
    declarations: [
        IndicatorHeaderComponent
    ],
    exports: [
        IndicatorHeaderComponent
    ],
    providers: [CoreDataHelperService, FTHelperService, HttpService, ProfileService,
        StaticReportsService, LightBoxService, SsrsReportService, ContentService, UIService],
})

export class SharedModule { }
