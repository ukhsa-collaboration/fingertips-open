import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { Parameters } from './parameters';
import { FTHelperService } from '../helper/ftHelper.service';
import { HttpService } from './http.service'
import { SpcForDsrLimits } from '../../../compare-area/compare-area';
import {
    FTIndicatorSearch, CoreDataSet, Category, PopulationSummary, Population,
    ComparatorMethod, IndicatorMetadataTextProperty, IndicatorStats, IndicatorMetadata,
    GroupRoot, GroupRootSummary, TrendRoot, PartitionDataForAllAges,
    PartitionDataForAllCategories, PartitionDataForAllSexes,
    PartitionTrendData,
    Age,
    Sex
} from '../../../typings/FT';
import { AreaHelper } from '../../shared';

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

    getSingleIndicatorForAllAreas(groupId: number, areaTypeId: number, parentAreaCode: string,
        profileId: number, comparatorId: number, indicatorId: number, sexId: number,
        ageId: number): Observable<CoreDataSet[]> {

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

    getIndicatorMetadataProperties(includeInternalMetadata: boolean): Observable<IndicatorMetadataTextProperty[]> {
        const params = new Parameters(this.version);
        params.addIncludeInternalMetadata(includeInternalMetadata);

        return this.httpService.httpGet('api/internal/indicator_metadata_text_properties', params);
    }

    getIndicatorStatisticsTrendsForSingleIndicator(indicatorId: number, sexId: number, ageId: number,
        childAreaTypeId: number, parentAreaCode: string, profileId: number): Observable<IndicatorStats[]> {

        const params = new Parameters(this.version);
        params.addIndicatorId(indicatorId);
        params.addSexId(sexId);
        params.addAgeId(ageId);
        params.addChildAreaTypeId(childAreaTypeId);
        params.addParentAreaCode(parentAreaCode);
        params.addProfileId(profileId);

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

    getLatestGroupRootDataForOneIndicator(areaTypeId: number, indicatorId: number,
        parentAreaCode: string, profileId: number): Observable<Array<GroupRoot>> {

        const params = new Parameters(this.version);
        params.addAreaTypeId(areaTypeId);
        params.addParentAreaCode(parentAreaCode);
        params.addIndicatorIds([indicatorId]);
        params.addProfileId(profileId);

        const serviceURL = 'api/latest_data/specific_indicators_for_child_areas';
        return this.httpService.httpGet(serviceURL, params);
    }

    getGroupingDataForProfile(areaTypeId: number, profileId: number): Observable<Array<GroupRootSummary>> {
        const params = new Parameters(this.version);
        params.addAreaTypeId(areaTypeId);

        let method: string;
        if (this.search.isInSearchMode()) {
            params.addIndicatorIds(this.search.getIndicatorIdList().getAllIds());
            method = 'by_indicator_id';
        } else {
            params.addProfileId(profileId);
            method = 'by_profile_id';
        }

        const serviceURL = 'api/grouproot_summaries/' + method;
        return this.httpService.httpGet(serviceURL, params);
    }

    getGroupingSubheadings(areaTypeId: number, groupId: number) {
        const params = new Parameters(this.version);
        params.addAreaTypeId(areaTypeId);
        params.addGroupId(groupId);

        return this.httpService.httpGet('api/grouping_subheadings', params);
    }

    getRecentTrends(parentAreaCode: string, groupId: number, areaTypeId: number, indicatorId: number,
        sexId: number, ageId: number): Observable<any> {

        const params = new Parameters(this.version);
        params.addParentAreaCode(parentAreaCode);
        params.addGroupId(groupId);
        params.addAreaTypeId(areaTypeId);
        params.addIndicatorId(indicatorId);
        params.addSexId(sexId);
        params.addAgeId(ageId);

        return this.httpService.httpGet('api/recent_trends/for_child_areas', params);
    }

    getSpcForDsrLimits(comparatorValue: number, populationMin: number, populationMax: number,
        unitValue: number, yearRange: number): Observable<SpcForDsrLimits> {

        const params = new Parameters(this.version);
        params.addComparatorValue(comparatorValue);
        params.addPopulationMin(populationMin);
        params.addPopulationMax(populationMax);
        params.addUnitValue(unitValue);
        params.addYearRange(yearRange);

        return this.httpService.httpGet('api/spc_for_dsr_limits', params);
    }

    getTrendDataForAllIndicatorsInProfileGroupForChildAreas(groupId: number, areaTypeId: number,
        parentAreaCode: string, profileId: number): Observable<Array<TrendRoot>> {

        const params = new Parameters(this.version);
        params.addAreaTypeId(areaTypeId);
        params.addParentAreaCode(parentAreaCode);
        params.addProfileId(profileId);
        params.addGroupId(groupId);

        return this.httpService.httpGet('api/trend_data/all_indicators_in_profile_group_for_child_areas', params);
    }

    getTrendDataForSpecificIndicatorsForChildAreas(areaTypeId: number, parentAreaCode: string,
        indicatorIds: number[], profileIds: number[]): Observable<Array<TrendRoot>> {

        const params = new Parameters(this.version);
        params.addAreaTypeId(areaTypeId);
        params.addParentAreaCode(parentAreaCode);
        params.addIndicatorIds(indicatorIds);
        params.addRestrictToProfileIds(profileIds);

        if (AreaHelper.isAreaList(parentAreaCode)) {
            params.addNoCache();
        }

        return this.httpService.httpGet('api/trend_data/specific_indicators_for_child_areas', params);
    }

    getDataForAllCategories(profileId: number, areaCode: string, indicatorId: number,
        areaTypeId: number, sexId: number, ageId: number): Observable<PartitionDataForAllCategories> {
        const params = new Parameters(this.version);
        params.addProfileId(profileId);
        params.addAreaCode(areaCode);
        params.addIndicatorId(indicatorId);
        params.addAreaTypeId(areaTypeId);
        params.addSexId(sexId);
        params.addAgeId(ageId);

        return this.httpService.httpGet('api/partition_data/by_category', params);
    }

    getDataForAllAges(profileId: number, areaCode: string, indicatorId: number, areaTypeId: number,
        sexId: number): Observable<PartitionDataForAllAges> {

        const params = new Parameters(this.version);
        params.addProfileId(profileId);
        params.addAreaCode(areaCode);
        params.addIndicatorId(indicatorId);
        params.addAreaTypeId(areaTypeId);
        params.addSexId(sexId);

        return this.httpService.httpGet('api/partition_data/by_age', params);
    }

    getDataForAllSexes(profileId: number, areaCode: string, indicatorId: number, areaTypeId: number,
        ageId: number): Observable<PartitionDataForAllSexes> {

        const params = new Parameters(this.version);
        params.addProfileId(profileId);
        params.addAreaCode(areaCode);
        params.addIndicatorId(indicatorId);
        params.addAreaTypeId(areaTypeId);
        params.addAgeId(ageId);

        return this.httpService.httpGet('api/partition_data/by_sex', params);
    }

    getTrendsByCategories(profileId: number, areaCode: string, indicatorId: number,
        areaTypeId: number, sexId: number, ageId: number): Observable<PartitionTrendData[]> {

        const params = new Parameters(this.version);
        params.addProfileId(profileId);
        params.addAreaCode(areaCode);
        params.addIndicatorId(indicatorId);
        params.addAreaTypeId(areaTypeId);
        params.addSexId(sexId);
        params.addAgeId(ageId);

        return this.httpService.httpGet('api/partition_trend_data/by_categories', params);
    }

    getTrendsByAge(profileId: number, areaCode: string, indicatorId: number,
        areaTypeId: number, sexId: number): Observable<PartitionTrendData> {

        const params = new Parameters(this.version);
        params.addProfileId(profileId);
        params.addAreaCode(areaCode);
        params.addIndicatorId(indicatorId);
        params.addAreaTypeId(areaTypeId);
        params.addSexId(sexId);

        return this.httpService.httpGet('api/partition_trend_data/by_age', params);
    }

    getTrendsBySex(profileId: number, areaCode: string, indicatorId: number,
        areaTypeId: number, ageId: number): Observable<PartitionTrendData> {

        const params = new Parameters(this.version);
        params.addProfileId(profileId);
        params.addAreaCode(areaCode);
        params.addIndicatorId(indicatorId);
        params.addAreaTypeId(areaTypeId);
        params.addAgeId(ageId);

        return this.httpService.httpGet('api/partition_trend_data/by_sex', params);
    }

    getAllSexes(): Observable<Sex[]> {
        const params = new Parameters(this.version);

        return this.httpService.httpGet('api/sexes', params);
    }

    getAllAges(): Observable<Age[]> {
        const params = new Parameters(this.version);

        return this.httpService.httpGet('api/ages', params);
    }

    private addSearchParameters(params: Parameters) {
        params.addIndicatorIds(this.search.getIndicatorIdList().getAllIds());
        params.addRestrictToProfileIds(this.search.getProfileIdsForSearch());
    }
}

