import { Injectable, ElementRef } from '@angular/core';
import {
    FTRoot, Area, GroupRoot, FTConfig, FTModel, IndicatorMetadata, Url, ValueNote,
    FTDisplay, CoreDataSet, FTIndicatorSearch, Grouping, CoreDataSetInfo, Unit,
    ValueNoteTooltipProvider, TooltipManager, TrendMarker, CommaNumber, ValueDisplayer,
    RecentTrendsTooltip, RecentTrendSelected, ComparisonConfig
} from '../typings/FT.d';
import { FTHelperService } from '../shared/service/helper/ftHelper.service';
import { AreaTypeIds } from '../shared/shared';
export class MockFTHelperService extends FTHelperService {
    getFTConfig(): FTConfig {
        return {
            hasRecentTrends: false,
            isChangeFromPreviousPeriodShown: false,
            profileCollectionUrlKey: '',
            startZeroYAxis: false,
            areAnyPdfsForProfile: false,
            hasStaticReports: false,
            nearestNeighbour: {},
            profileName: '',
            showDataQuality: false
        };
    }
    getURL(): Url {
        return {
            img: '',
            bridge: '',
            corews: '',
            search: '',
            pdf: ''
        };
    }
    version(): string {
        return '';
    }
    getSearch(): FTIndicatorSearch {
        let a: FTIndicatorSearch;
        return a;
    }
    getFTModel(): FTModel {
        return {
            areaTypeId: AreaTypeIds.AcuteTrust,
            groupId: null,
            parentCode: '',
            profileId: null,
            parentTypeId: null,
            areaCode: '',
            iid: null,
            ageId: null,
            sexId: null,
            nearestNeighbour: ''
        };
    }
    showAndHidePageElements(): void {
    }
    unlock(): void {
    }
}