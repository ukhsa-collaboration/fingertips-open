import { Http, Response, RequestOptions, URLSearchParams, Headers } from '@angular/http';
export class Parameters {

    private params: URLSearchParams = new URLSearchParams();

    constructor(version: string) {
        // Version included for cache busting between deployments
        this.params.set('v', version);
    }

    getRequestOptions(): RequestOptions {
        const requestOptions: RequestOptions = new RequestOptions({
            headers: new Headers({ 'Content-Type': 'application/json' })
        });
        requestOptions.search = this.params;
        return requestOptions;
    }

    /** Returns a concaternated string of the URL parameters */
    getParameterString() {
        return this.params.toString();
    }

    addId(id: number): void {
        this.params.set('id', id.toString());
    }

    addGroupId(groupId: number): void {
        this.params.set('group_id', groupId.toString());
    }

    addGroupIds(groupId: number): void {
        this.params.set('group_ids', groupId.toString());
    }

    addProfileId(profileId: number): void {
        this.params.set('profile_id', profileId.toString());
    }

    addAreaTypeId(areaTypeId: number): void {
        this.params.set('area_type_id', areaTypeId.toString());
    }

    addChildAreaTypeId(childAreaTypeId: number) {
        this.params.set('child_area_type_id', childAreaTypeId.toString());
    }

    addParentAreaCode(areaCode: string): void {
        this.params.set('parent_area_code', areaCode);
    }

    addAreaCode(areaCode: string): void {
        this.params.set('area_code', areaCode);
    }

    addAreaCodes(areaCodes: string): void {
        this.params.set('area_codes', areaCodes);
    }

    addComparatorId(comparatorId: number): void {
        this.params.set('comparator_id', comparatorId.toString());
    }

    addIndicatorId(indicatorId: number): void {
        this.params.set('indicator_id', indicatorId.toString());
    }

    addCategoryTypeId(categoryTypeId: number): void {
        this.params.set('category_type_id', categoryTypeId.toString());
    }

    addSexId(sexId: number): void {
        this.params.set('sex_id', sexId.toString());
    }

    addAgeId(ageId: number): void {
        this.params.set('age_id', ageId.toString());
    }

    addIndicatorListId(indicatorListId: any): void {
        this.params.set('indicator_list_id', indicatorListId.toString());
    }

    addIncludeSystemContent(yesOrNo: string): void {
        this.params.set('include_system_content', yesOrNo);
    }

    addIncludeDefinition(yesOrNo: string): void {
        this.params.set('include_definition', yesOrNo);
    }

    addRestrictToProfileIds(profileIds: Array<number>): void {
        this.params.set('restrict_to_profile_ids', profileIds.join(','));
    }

    addIndicatorIds(indicatorIds: Array<number>): void {
        this.params.set('indicator_ids', indicatorIds.join(','));
    }

    addSearchText(searchText: string): void {
        this.params.set('search_text', searchText);
    }

    addNoCache(): void {
        this.params.set('no_cache', true.toString());
    }

    addIncludeCoordinates(includeCoordinates: Boolean): void {
        this.params.set('include_coordinates', includeCoordinates.toString());
    }

    addParentAreasToIncludeInResults(parentAreasToIncludeInResults: Boolean): void {
        this.params.set('parent_areas_to_include_in_results', parentAreasToIncludeInResults.toString());
    }

    addPolygonAreaTypeId(polygonAreaTypeId: number): void {
        this.params.set('polygon_area_type_id', polygonAreaTypeId.toString());
    }

    addEasting(easting: number): void {
        this.params.set('easting', easting.toString());
    }
    addNorthing(northing: number): void {
        this.params.set('northing', northing.toString());
    }

    addUserId(userId: string): void {
        this.params.set('user_id', userId);
    }

    addAreaListId(areaListId: number): void {
        this.params.set('area_list_id', areaListId.toString());
    }

    addPublicId(publicId: string): void {
        this.params.set('public_id', publicId);
    }

    addProfileKey(profileKey: string): void {
        this.params.set('profile_key', profileKey);
    }

    addFileName(fileName: string): void {
        this.params.set('file_name', fileName);
    }

    addTimePeriod(timePeriod: string): void {
        this.params.set('time_period', timePeriod);
    }

    addKey(key: string): void {
        this.params.set('key', key);
    }
}


