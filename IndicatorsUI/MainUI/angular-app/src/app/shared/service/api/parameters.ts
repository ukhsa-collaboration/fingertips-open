import { HttpParams, HttpHeaders } from '@angular/common/http';

export class Parameters {

    private params: HttpParams = new HttpParams();

    constructor(version: string) {
        // Version included for cache busting between deployments
        this.params.set('v', version);
    }

    getRequestOptions(resetCache: boolean): any {
        let headers = this.getHeaders();
        const params = this.params;

        if (resetCache) {
            headers = headers.set('reset-cache', 'true');
        }

        const options = ({
            headers: headers,
            params: params
        });

        return options;
    }

    getHeaders(): HttpHeaders {
        return new HttpHeaders({ 'Content-Type': 'application/json;charset=UTF-8' });
    }

    /** Returns a concaternated string of the URL parameters */
    getParameterString() {
        return this.params.toString();
    }

    addNoCache(): void {
        this.params = this.params.set('no_cache', true.toString());
    }

    addId(id: number): void {
        this.params = this.params.set('id', id.toString());
    }

    addGroupId(groupId: number): void {
        this.params = this.params.set('group_id', groupId.toString());
    }

    addGroupIds(groupId: number): void {
        this.params = this.params.set('group_ids', groupId.toString());
    }

    addProfileId(profileId: number): void {
        this.params = this.params.set('profile_id', profileId.toString());
    }

    addAreaTypeId(areaTypeId: number): void {
        this.params = this.params.set('area_type_id', areaTypeId.toString());
    }

    addParentAreaTypeId(parentAreaTypeId: number) {
        this.params = this.params.set('parent_area_type_id', parentAreaTypeId.toString());
    }

    addChildAreaTypeId(childAreaTypeId: number) {
        this.params = this.params.set('child_area_type_id', childAreaTypeId.toString());
    }

    addParentAreaCode(areaCode: string): void {
        this.params = this.params.set('parent_area_code', areaCode);
    }

    addCategoryAreaCode(categoryAreaCode: string): void {
        this.params = this.params.set('category_area_code', categoryAreaCode);
    }

    addAreaCode(areaCode: string): void {
        this.params = this.params.set('area_code', areaCode);
    }

    addAreaCodes(areaCodes: string): void {
        this.params = this.params.set('area_codes', areaCodes);
    }

    addComparatorId(comparatorId: number): void {
        this.params = this.params.set('comparator_id', comparatorId.toString());
    }

    addIndicatorId(indicatorId: number): void {
        this.params = this.params.set('indicator_id', indicatorId.toString());
    }

    addCategoryTypeId(categoryTypeId: number): void {
        this.params = this.params.set('category_type_id', categoryTypeId.toString());
    }

    addSexId(sexId: number): void {
        this.params = this.params.set('sex_id', sexId.toString());
    }

    addAgeId(ageId: number): void {
        this.params = this.params.set('age_id', ageId.toString());
    }

    addSexIds(sexIds: Array<number>): void {
        this.params = this.params.set('sex_ids', sexIds.join(','));
    }

    addAgeIds(ageIds: Array<number>): void {
        this.params = this.params.set('age_ids', ageIds.join(','));
    }

    addIndicatorListId(indicatorListId: any): void {
        this.params = this.params.set('indicator_list_id', indicatorListId.toString());
    }

    addIncludeSystemContent(yesOrNo: string): void {
        this.params = this.params.set('include_system_content', yesOrNo);
    }

    addIncludeDefinition(yesOrNo: string): void {
        this.params = this.params.set('include_definition', yesOrNo);
    }

    addRestrictToProfileIds(profileIds: Array<number>): void {
        this.params = this.params.set('restrict_to_profile_ids', profileIds.join(','));
    }

    addIndicatorIds(indicatorIds: Array<number>): void {
        this.params = this.params.set('indicator_ids', indicatorIds.join(','));
    }

    addSearchText(searchText: string): void {
        this.params = this.params.set('search_text', searchText);
    }

    addIncludeCoordinates(includeCoordinates: boolean): void {
        this.params = this.params.set('include_coordinates', includeCoordinates.toString());
    }

    addParentAreasToIncludeInResults(parentAreasToIncludeInResults: boolean): void {
        this.params = this.params.set('parent_areas_to_include_in_results', parentAreasToIncludeInResults.toString());
    }

    addPolygonAreaTypeId(polygonAreaTypeId: number): void {
        this.params = this.params.set('polygon_area_type_id', polygonAreaTypeId.toString());
    }

    addEasting(easting: number): void {
        this.params = this.params.set('easting', easting.toString());
    }

    addNorthing(northing: number): void {
        this.params = this.params.set('northing', northing.toString());
    }

    addUserId(userId: string): void {
        this.params = this.params.set('user_id', userId);
    }

    addAreaListId(areaListId: number): void {
        this.params = this.params.set('area_list_id', areaListId.toString());
    }

    addPublicId(publicId: string): void {
        this.params = this.params.set('public_id', publicId);
    }

    addProfileKey(profileKey: string): void {
        this.params = this.params.set('profile_key', profileKey);
    }

    addFileName(fileName: string): void {
        this.params = this.params.set('file_name', fileName);
    }

    addTimePeriod(timePeriod: string): void {
        this.params = this.params.set('time_period', timePeriod);
    }

    addKey(key: string): void {
        this.params = this.params.set('key', key);
    }

    addComparatorValue(comparatorValue: number): void {
        this.params = this.params.set('comparator_value', comparatorValue.toString());
    }

    addPopulationMin(populationMin: number): void {
        this.params = this.params.set('population_min', populationMin.toString());
    }

    addPopulationMax(populationMax: number): void {
        this.params = this.params.set('population_max', populationMax.toString());
    }

    addUnitValue(unitValue: number): void {
        this.params = this.params.set('unit_value', unitValue.toString());
    }

    addYearRange(yearRange: number): void {
        this.params = this.params.set('year_range', yearRange.toString());
    }

    addInequalities(inequalities: string): void {
        this.params = this.params.set('inequalities', inequalities);
    }

    addIncludeInternalMetadata(includeInternalMetadata: boolean): void {
        this.params = this.params.set('include_internal_metadata', includeInternalMetadata.toString());
    }

    addNearestNeighbourCode(nearestNeighbourCode: string): void {
        this.params = this.params.set('nearest_neighbour_code', nearestNeighbourCode);
    }
}
