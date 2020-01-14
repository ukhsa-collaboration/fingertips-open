import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

// Helper services
import { CoreDataHelperService } from './service/helper/coreDataHelper.service';
import { FTHelperService } from './service/helper/ftHelper.service';
import { LightBoxService } from './service/helper/light-box.service';
import { UIService } from './service/helper/ui.service';

// Components
import { IndicatorHeaderComponent } from './component/indicator-header/indicator-header.component';

// API services
import { HttpService } from './service/api/http.service';
import { HttpCacheService } from './service/api/http-cache.service';
import { StaticReportsService } from './service/api/static-reports.service';
import { ProfileService } from './service/api/profile.service';
import { ContentService } from './service/api/content.service';
import { SsrsReportService } from './service/api/ssrs-report.service';
import { DownloadService } from './service/api/download.service';

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
    providers: [
        HttpCacheService,
        {
            provide: HTTP_INTERCEPTORS,
            useClass: HttpService,
            multi: true
        },
        HttpService,
        CoreDataHelperService,
        FTHelperService,
        ProfileService,
        StaticReportsService,
        LightBoxService,
        SsrsReportService,
        ContentService,
        UIService,
        DownloadService
    ]
})

export class SharedModule { }
