import { Injectable } from '@angular/core';
import { FTHelperService } from '../helper/ftHelper.service';
import { Parameters } from './parameters';
import { HttpService } from './http.service'

@Injectable()
export class DownloadService {

    private version: string = this.ftHelperService.version();
    private coreWsUrl: string;

    constructor(private httpService: HttpService,
        private ftHelperService: FTHelperService) {

        this.coreWsUrl = this.ftHelperService.getURL().corews;
    }

    csvDownloadLatestNoInequalitiesDataByGroup(parentAreaTypeId: number, childAreaTypeId: number,
        groupId: number, areasCode: string) {

        const params = new Parameters(this.version);
        params.addParentAreaTypeId(parentAreaTypeId);
        params.addChildAreaTypeId(childAreaTypeId);
        params.addGroupId(groupId);
        params.addAreaCodes(areasCode);

        const url = this.coreWsUrl + 'api/latest_data_without_inequalities/csv/by_group_id?' + params.getParameterString();
        window.open(url, '_blank');
    }

    csvDownloadLatestNoInequalitiesDataByIndicator(parentAreaTypeId: number, childAreaTypeId: number,
        profileId: number, areasCode: string, indicatorIds: number[], parentAreaCode: string, categoryAreaCode: string,
        sexIds: number[], ageIds: number[]) {

        const params = new Parameters(this.version);
        params.addParentAreaTypeId(parentAreaTypeId);
        params.addChildAreaTypeId(childAreaTypeId);
        params.addProfileId(profileId);
        params.addAreaCodes(areasCode);
        params.addIndicatorIds(indicatorIds);
        params.addParentAreaCode(parentAreaCode);
        params.addCategoryAreaCode(categoryAreaCode);
        params.addSexIds(sexIds);
        params.addAgeIds(ageIds);

        const url = this.coreWsUrl + 'api/latest_data_without_inequalities/csv/by_indicator_id?' + params.getParameterString();
        window.open(url, '_blank');
    }

    csvDownloadAllPeriodsNoInequalitiesDataByIndicator(parentAreaTypeId: number, childAreaTypeId: number,
        profileId: number, areasCode: string, indicatorIds: number[], parentAreaCode: string, categoryAreaCode: string,
        sexIds: number[], ageIds: number[] ) {

        const params = new Parameters(this.version);
        params.addParentAreaTypeId(parentAreaTypeId);
        params.addChildAreaTypeId(childAreaTypeId);
        params.addProfileId(profileId);
        params.addAreaCodes(areasCode);
        params.addIndicatorIds(indicatorIds);
        params.addParentAreaCode(parentAreaCode);
        params.addCategoryAreaCode(categoryAreaCode);
        params.addSexIds(sexIds);
        params.addAgeIds(ageIds);

        const url = this.coreWsUrl + 'api/all_data_without_inequalities/csv/by_indicator_id?' + params.getParameterString();
        window.open(url, '_blank');

    }

    csvDownloadLatestPopulationData(parentAreaTypeId: number, childAreaTypeId: number,
        areasCode: string, parentAreaCode: string, categoryAreaCode: string) {

        const params = new Parameters(this.version);
        params.addParentAreaTypeId(parentAreaTypeId);
        params.addChildAreaTypeId(childAreaTypeId);
        params.addAreaCodes(areasCode);
        params.addParentAreaCode(parentAreaCode);
        params.addCategoryAreaCode(categoryAreaCode);

        const url = this.coreWsUrl + 'api/latest/population/csv?' + params.getParameterString();
        window.open(url, '_blank');
    }

    csvDownloadAllPeriodsWithInequalitiesDataByIndicator(parentAreaTypeId: number, childAreaTypeId: number,
        profileId: number, areasCode: string, indicatorIds: number[],
        parentAreaCode: string, inequalities: string, categoryAreaCode: string) {

        const params = new Parameters(this.version);
        params.addParentAreaTypeId(parentAreaTypeId);
        params.addChildAreaTypeId(childAreaTypeId);
        params.addProfileId(profileId);
        params.addAreaCodes(areasCode);
        params.addIndicatorIds(indicatorIds);
        params.addParentAreaCode(parentAreaCode);
        params.addInequalities(inequalities);
        params.addCategoryAreaCode(categoryAreaCode);

        const url = this.coreWsUrl + 'api/all_data_with_inequalities/csv/by_indicator_id?' + params.getParameterString();
        window.open(url, '_blank');
    }

    csvDownloadLatestWithInequalitiesDataByIndicator(parentAreaTypeId: number, childAreaTypeId: number,
        profileId: number, areasCode: string, indicatorIds: number[],
        parentAreaCode: string, inequalities: string, categoryAreaCode: string) {

        const params = new Parameters(this.version);
        params.addParentAreaTypeId(parentAreaTypeId);
        params.addChildAreaTypeId(childAreaTypeId);
        params.addProfileId(profileId);
        params.addAreaCodes(areasCode);
        params.addIndicatorIds(indicatorIds);
        params.addParentAreaCode(parentAreaCode);
        params.addInequalities(inequalities);
        params.addCategoryAreaCode(categoryAreaCode);

        const url = this.coreWsUrl + 'api/latest_data_with_inequalities/csv/by_indicator_id?' + params.getParameterString();
        window.open(url, '_blank');
    }
}
