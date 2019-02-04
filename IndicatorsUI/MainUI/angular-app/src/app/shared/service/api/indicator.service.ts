import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import {
    CoreDataSet, IndicatorMetadata, GroupRoot, FTIndicatorSearch,
    IndicatorMetadataTextProperty, ComparatorMethod, IndicatorStats, Population, Category,
    PopulationSummary
} from '../../../typings/FT.d';
import { Parameters } from './parameters';
import { FTHelperService } from '../../service/helper/ftHelper.service';
import { HttpService } from './http.service'
import { modelGroupProvider } from '@angular/forms/src/directives/ng_model_group';

/** Client library for multiple service controllers in WS:
 * - Data
 * - IndicatorMetadata
 */
@Injectable()
export class IndicatorService {

    private version: string = this.ftHelperService.version();
    private search: FTIndicatorSearch = this.ftHelperService.getSearch();

    constructor(private httpService: HttpService, private ftHelperService: FTHelperService) {
    }

    getSingleIndicatorForAllArea(groupId: number, areaTypeId: number, parentAreaCode: string,
        profileId: number, comparatorId: number, indicatorId: number, sexId: number,
        ageId: number): Observable<CoreDataSet> {

        const params = new Parameters(this.version);
        params.addGroupId(groupId);
        params.addAreaTypeId(areaTypeId);
        params.addParentAreaCode(parentAreaCode);
        params.addProfileId(profileId);
        params.addComparatorId(comparatorId);
        params.addIndicatorId(indicatorId);
        params.addSexId(sexId);
        params.addAgeId(ageId);

        return this.httpService.httpGet('api/latest_data/single_indicator_for_all_areas', params);
    }

    getCategories(categoryTypeId: number): Observable<Category[]> {

        const params = new Parameters(this.version);
        params.addCategoryTypeId(categoryTypeId);

        return this.httpService.httpGet('api/categories', params);
    }

    getPopulationSummary(areaCode: string, areaTypeId): Observable<PopulationSummary> {

        const params = new Parameters(this.version);
        params.addAreaCode(areaCode);
        params.addAreaTypeId(areaTypeId);

        return this.httpService.httpGet('api/quinary_population_summary', params);
    }

    getPopulation(areaCode: string, areaTypeId: number): Observable<Population> {

        const params = new Parameters(this.version);
        params.addAreaCode(areaCode);
        params.addAreaTypeId(areaTypeId);

        return this.httpService.httpGet('api/quinary_population', params);
    }

    getBenchmarkingMethod(benchmarkingMethodId: number): Observable<ComparatorMethod> {

        const params = new Parameters(this.version);
        params.addId(benchmarkingMethodId);

        return this.httpService.httpGet('api/comparator_method', params);
    }

    getIndicatorMetadataProperties(): Observable<IndicatorMetadataTextProperty[]> {
        const params = new Parameters(this.version);
        return this.httpService.httpGet('api/indicator_metadata_text_properties', params);
    }

    getIndicatorStatisticsTrendsForSingleIndicator(indicatorId: number, sexId: number, ageId: number,
        childAreaTypeId: number, parentAreaCode: string): Observable<IndicatorStats> {

        const params = new Parameters(this.version);
        params.addIndicatorId(indicatorId);
        params.addSexId(sexId);
        params.addAgeId(ageId);
        params.addChildAreaTypeId(childAreaTypeId);
        params.addParentAreaCode(parentAreaCode);

        return this.httpService.httpGet('api/indicator_statistics/trends_for_single_indicator', params);
    }

    getIndicatorStatistics(childAreaTypeId: number, parentAreaCode: string,
        profileId: number, groupId: number): Observable<Map<number, IndicatorStats>> {

        const params = new Parameters(this.version);
        params.addParentAreaCode(parentAreaCode);
        params.addChildAreaTypeId(childAreaTypeId);
        params.addGroupId(groupId);

        let method: string;
        if (this.search.isInSearchMode()) {
            method = 'by_indicator_id';
            this.addSearchParameters(params);
        } else {
            method = 'by_profile_id';
            params.addProfileId(profileId);
        }

        return this.httpService.httpGet('api/indicator_statistics/' + method, params);
    }

    getIndicatorMetadata(groupId: number): Observable<Map<number, IndicatorMetadata>> {

        const params = new Parameters(this.version);
        params.addIncludeSystemContent('no');
        params.addIncludeDefinition('yes');

        let method: string;
        if (this.search.isInSearchMode()) {
            method = 'by_indicator_id';
            this.addSearchParameters(params);
        } else {
            method = 'by_group_id';
            params.addGroupIds(groupId);
        }

        const serviceURL = 'api/indicator_metadata/' + method;
        return this.httpService.httpGet(serviceURL, params);
    }

    getIndicatorMetadataByIndicatorId(indicatorId: number, restrictToProfileIds: number[]): Observable<Map<number, IndicatorMetadata>> {
        const params = new Parameters(this.version);
        params.addIncludeSystemContent('no');
        params.addIncludeDefinition('yes');
        params.addIndicatorIds([indicatorId]);
        params.addRestrictToProfileIds(restrictToProfileIds);
        return this.httpService.httpGet('api/indicator_metadata/by_indicator_id', params);
    }

    getLatestDataForAllIndicatorsInProfileGroupForChildAreas(groupId: number, areaTypeId: number,
        parentAreaCode: string, profileId: number): Observable<Array<GroupRoot>> {

        const params = new Parameters(this.version);
        params.addAreaTypeId(areaTypeId);
        params.addParentAreaCode(parentAreaCode);
        params.addProfileId(profileId);

        let method: string;
        if (this.search.isInSearchMode()) {
            if (this.search.isIndicatorList()) {
                params.addIndicatorListId(this.search.getIndicatorListId());
                method = 'indicator_list_for_child_areas'
            } else {
                this.addSearchParameters(params);
                method = 'specific_indicators_for_child_areas'
            }
        } else {
            params.addGroupId(groupId);
            method = 'all_indicators_in_profile_group_for_child_areas'
        }

        const serviceURL = 'api/latest_data/' + method;
        return this.httpService.httpGet(serviceURL, params);
    }

    getGroupingSubheadings(areaTypeId: number, groupId: number) {
        const params = new Parameters(this.version);
        params.addAreaTypeId(areaTypeId);
        params.addGroupId(groupId);

        return this.httpService.httpGet('api/grouping_subheadings', params);
    }

    getIid() {
        var model = this.ftHelperService.getFTModel();
        if (model.iid === null){
            return this.getCurrentIndicatorId(model);
        }
        return model.iid;
    }
    
    private getCurrentIndicatorId(model){
        var index = $('#indicatorMenu').prop('selectedIndex');
        return model.groupRoots[index].IID;
    }
    private addSearchParameters(params: Parameters) {
        params.addIndicatorIds(this.search.getIndicatorIdList().getAllIds());
        params.addRestrictToProfileIds(this.search.getProfileIdsForSearch());
    }
}

