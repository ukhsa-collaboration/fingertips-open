import { Injectable } from '@angular/core';
import { Http, Response, RequestOptions, URLSearchParams, Headers } from '@angular/http';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/publishReplay';
import 'rxjs/add/operator/catch';
import { Observable } from 'rxjs/Observable';
import {
    FTRoot, CoreDataSet, IndicatorMetadata, GroupRoot, FTIndicatorSearch,
    IndicatorMetadataTextProperty, ComparatorMethod, IndicatorStats, Population, Category,
    PopulationSummary
} from '../../../typings/FT.d';
import { Parameters } from './parameters';
import { FTHelperService } from '../../service/helper/ftHelper.service';

@Injectable()
export class IndicatorService {

    /** Observables for calls that have previously been made */
    private observables: any = {};

    private baseUrl: string = this.ftHelperService.getURL().bridge;

    /** The version number of the static assets, e.g. JS */
    private version: string = this.ftHelperService.version();
    private search = this.ftHelperService.getSearch();

    constructor(private http: Http, private ftHelperService: FTHelperService) { }

    getSingleIndicatorForAllArea(groupId: number, areaTypeId: number, parentAreaCode: string,
        profileId: number, comparatorId: number, indicatorId: number, sexId: number,
        ageId: number): Observable<CoreDataSet> {

        let params = new Parameters(this.version);
        params.addGroupId(groupId);
        params.addAreaTypeId(areaTypeId);
        params.addParentAreaCode(parentAreaCode);
        params.addProfileId(profileId);
        params.addComparatorId(comparatorId);
        params.addIndicatorId(indicatorId);
        params.addSexId(sexId);
        params.addAgeId(ageId);

        return this.getObservable('api/latest_data/single_indicator_for_all_areas', params);
    }

    getCategories(categoryTypeId: number): Observable<Category[]> {

        let params = new Parameters(this.version);
        params.addCategoryTypeId(categoryTypeId);

        return this.getObservable('api/categories', params);
    }

    getPopulationSummary(areaCode: string, areaTypeId): Observable<PopulationSummary> {

        let params = new Parameters(this.version);
        params.addAreaCode(areaCode);
        params.addAreaTypeId(areaTypeId);

        return this.getObservable('api/quinary_population_summary', params);
    }

    getPopulation(areaCode: string, areaTypeId: number): Observable<Population> {

        let params = new Parameters(this.version);
        params.addAreaCode(areaCode);
        params.addAreaTypeId(areaTypeId);

        return this.getObservable('api/quinary_population', params);
    }

    getBenchmarkingMethod(benchmarkingMethodId: number): Observable<ComparatorMethod> {

        let params = new Parameters(this.version);
        params.addId(benchmarkingMethodId);

        return this.getObservable('api/comparator_method', params);
    }

    getIndicatorMetadataProperties(): Observable<IndicatorMetadataTextProperty[]> {
        return this.getObservable('api/indicator_metadata_text_properties');
    }

    getIndicatorStatisticsTrendsForSingleIndicator(indicatorId: number, sexId: number, ageId: number,
        childAreaTypeId: number, parentAreaCode: string): Observable<IndicatorStats> {

        let params = new Parameters(this.version);
        params.addIndicatorId(indicatorId);
        params.addSexId(sexId);
        params.addAgeId(ageId);
        params.addChildAreaTypeId(childAreaTypeId);
        params.addParentAreaCode(parentAreaCode);

        return this.getObservable('api/indicator_statistics/trends_for_single_indicator', params);
    }

    getIndicatorMetadata(groupId: number): Observable<Map<number, IndicatorMetadata>> {

        let params = new Parameters(this.version);
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
        return this.getObservable(serviceURL, params);
    }

    getIndicatorMetadataByIndicatorId(indicatorId: number, restrictToProfileIds: number[]): Observable<Map<number, IndicatorMetadata>> {
        let params = new Parameters(this.version);
        params.addIncludeSystemContent('no');
        params.addIncludeDefinition('yes');
        params.addIndicatorIds([indicatorId]);
        params.addRestrictToProfileIds(restrictToProfileIds);
        return this.getObservable('api/indicator_metadata/by_indicator_id', params);
    }

    getLatestDataForAllIndicatorsInProfileGroupForChildAreas(groupId: number, areaTypeId: number,
        parentAreaCode: string, profileId: number): Observable<Array<GroupRoot>> {

        let params = new Parameters(this.version);
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
        return this.getObservable(serviceURL, params);
    }

    private getObservable(serviceUrl: string, params?: Parameters): Observable<any> {

        // Ensure paramaters is defined
        if (!params) {
            params = new Parameters(this.version);
        }

        // Check whether call has already been made and cached observable is available
        let parameterString = params.getParameterString();
        let serviceKey = serviceUrl + parameterString;
        if (this.observables[serviceKey]) {
            return this.observables[serviceKey];
        }

        let observable = this.http.get(this.baseUrl + serviceUrl, params.getRequestOptions())
            .publishReplay(1).refCount() // Call once then use same response for repeats
            .map(res => res.json())
            .catch(this.handleError);

        this.observables[serviceKey] = observable;
        return observable;
    }

    private addSearchParameters(params: Parameters) {
        params.addIndicatorIds(this.search.getIndicatorIdList().getAllIds());
        params.addRestrictToProfileIds(this.search.getProfileIdsForSearch());
    }

    private handleError(error: any) {
        console.error(error);
        const errorMessage = 'AJAX call failed';
        return Observable.throw(errorMessage);
    }

    private handleBoundariesError(error: any) {
        console.error(error);
        const errorMessage = 'Unsupported map type. Maps are not available for ' +
            this.ftHelperService.getAreaTypeName();
        return Observable.throw(errorMessage);
    }
}

