import { Component } from '@angular/core';
import { FTHelperService } from '../../shared/service/helper/ftHelper.service';
import { ParameterBuilder, ParentAreaHelper, SexAndAgeLabelHelper, CategoryAreaCodeHelper, AreaHelper } from '../../shared/shared';
import { AreaTypeIds, ProfileIds, GroupIds } from '../../shared/constants';
import { AreaCodes } from '../../shared/constants';
import { FTModel, GroupRoot, FTIndicatorSearch, FTUrls, FTConfig } from '../../typings/FT';

@Component({
  selector: 'ft-download-data',
  templateUrl: './download-data.component.html',
  styleUrls: ['./download-data.component.css']
})
export class DownloadDataComponent {

  // Display toggle flags
  public showProfile = false;
  public showGroup = false;
  public showAllIndicators = false;
  public showNational = true;
  public showSubnational = false;
  public isNearestNeighbours = false;
  public showAddresses = false;
  public showPopulation = false;
  public isEnvironmentTest = false;

  // Text
  public profileName = '';
  public groupName = '';
  public indicatorName = '';
  public allLabel = '';
  public parentLabel = '';
  public nearestNeighboursLabel = '';
  public apiUrl = '';

  // Area codes
  public parentCode = '';
  public nationalCode = '';
  public nearestNeighbourCode = '';

  // Fingertips info
  private model: FTModel;
  private groupRoot: GroupRoot;
  private search: FTIndicatorSearch;
  private urls: FTUrls;
  private config: FTConfig;

  constructor(private ftHelperService: FTHelperService) { }

  public refresh() {

    // Set instance level variables
    this.model = this.ftHelperService.getFTModel();
    this.groupRoot = this.ftHelperService.getCurrentGroupRoot();
    this.search = this.ftHelperService.getSearch();
    this.urls = this.ftHelperService.getURL();
    this.config = this.ftHelperService.getFTConfig();

    const config = this.ftHelperService.getFTConfig();

    this.isNearestNeighbours = this.ftHelperService.isNearestNeighbours();
    this.profileName = config.profileName;
    this.groupName = this.ftHelperService.getCurrentDomainName();

    this.showSubnational = !this.ftHelperService.isParentCountry() &&
      this.model.areaTypeId !== AreaTypeIds.Country &&
      !this.isNearestNeighbours;

    this.showAllIndicators = this.search.isInSearchMode();
    this.showProfile = !this.showAllIndicators;
    this.showGroup = this.groupName !== null &&
      this.groupName !== '' && this.showProfile;

    const areaTypeName = this.ftHelperService.getAreaTypeName();

    const isEnglandAreaType = areaTypeName === 'England';

    const dataLabel = 'Data for ' + areaTypeName;
    if (!isEnglandAreaType) {
      dataLabel.concat(' in England');
    }

    this.allLabel = dataLabel;

    this.nationalCode = AreaCodes.England;
    this.parentCode = this.model.parentCode;
    this.nearestNeighbourCode = this.model.nearestNeighbour;

    this.parentLabel = '';

    if (!isEnglandAreaType) {
      this.parentLabel = this.isNearestNeighbours ? ''
        : 'Data for ' + areaTypeName + ' in ' + new ParentAreaHelper(this.ftHelperService).getParentAreaName();
    }

    if (this.isNearestNeighbours) {
      const areaName = this.ftHelperService.getArea(this.model.areaCode).Name;
      this.nearestNeighboursLabel = areaName + ' ' + this.config.nearestNeighbour[this.model.areaTypeId].SelectedText;
    }

    // Indicator name
    const sexAndAgeLabel = new SexAndAgeLabelHelper(this.groupRoot).getSexAndAgeLabel();
    this.indicatorName = this.ftHelperService.getMetadataHash()[this.groupRoot.IID].Descriptive.Name + sexAndAgeLabel;

    if (this.model.profileId === ProfileIds.PracticeProfile) {
      // Too much data to download for all of practice profiles
      this.showProfile = false;
      this.showPopulation = true;
    } else {
      this.showPopulation = false;
    }

    this.isEnvironmentTest = config.environment === 'test';

    this.showAddresses = this.model.areaTypeId === AreaTypeIds.Practice;

    this.setApiUrl();
  }

  setApiUrl() {
    if (this.apiUrl === '') {
      this.apiUrl = this.urls.bridge + 'api';
    }
  }

  /**
  * Export all indicator metadata for specific profile.
  */
  exportProfileMetadata(): void {
    const parameters = new ParameterBuilder();
    this.addProfileIdParameter(parameters);
    this.downloadCsvMetadata('by_profile_id', parameters);
    this.logEvent('MetadataForProfile', parameters.build());
  }

  /**
  * Export indicator metadata for profile group
  */
  exportGroupMetadata(): void {
    const parameters = new ParameterBuilder(
    ).add('group_id', this.model.groupId);
    this.downloadCsvMetadata('by_group_id', parameters);
    this.logEvent('MetadataForDomain', parameters.build());
  }

  /**
  * Export all indicator metadata for search results.
  */
  exportAllIndicatorMetadata(): void {
    const parameters = new ParameterBuilder();
    parameters.add('indicator_ids', this.search.getIndicatorIdsParameter());
    this.addProfileIdParameter(parameters);
    this.downloadCsvMetadata('by_indicator_id', parameters);
    this.logEvent('MetadataForAllSearchIndicators', parameters.build());
  }

  /**
  * Export indicator metadata for single indicator.
  */
  exportIndicatorMetadata(): void {
    const parameters = new ParameterBuilder();
    parameters.add('indicator_ids', this.groupRoot.IID);
    this.addProfileIdParameter(parameters);
    this.downloadCsvMetadata('by_indicator_id', parameters);
    this.logEvent('MetadataForSearchIndicator', parameters.build());
  }

  /**
  * Export all indicator data for specific profile.
  */
  exportProfileData(parentCode: string): void {
    const parameters = this.getExportParameters(parentCode);
    this.addProfileIdParameter(parameters);
    parameters.add('category_area_code', CategoryAreaCodeHelper.getCategoryAreaCode(this.model.parentCode));
    this.downloadCsvData('by_profile_id', parameters);
    this.logEvent('DataForProfile', parameters.build());
  }

  /**
  * Export indicator data for profile group
  */
  exportGroupData(parentAreaCode: string): void {
    const parameters = this.getExportParameters(parentAreaCode);
    parameters.add('group_id', this.model.groupId);
    parameters.add('category_area_code', CategoryAreaCodeHelper.getCategoryAreaCode(this.model.parentCode));
    this.downloadCsvData('by_group_id', parameters);
    this.logEvent('DataForGroup', parameters.build());
  }

  /**
  * Export all indicator data for search results.
  */
  exportAllIndicatorData(parentAreaCode): void {
    const parameters = this.getExportParameters(parentAreaCode);
    parameters.add('indicator_ids', this.search.getIndicatorIdsParameter());
    parameters.add('category_area_code', CategoryAreaCodeHelper.getCategoryAreaCode(this.model.parentCode));
    this.addProfileIdParameter(parameters);
    this.downloadCsvData('by_indicator_id', parameters);
    this.logEvent('DataForAllSearchIndicators', parameters.build());
  }

  /**
  * Export indicator data for single indicator.
  */
  exportIndicatorData(parentAreaCode): void {
    const parameters = this.getExportParameters(parentAreaCode);
    this.addProfileIdParameter(parameters);
    parameters.add('indicator_ids', this.groupRoot.IID);
    parameters.add('category_area_code', CategoryAreaCodeHelper.getCategoryAreaCode(this.model.parentCode));
    parameters.add('sex_id', this.model.sexId);
    parameters.add('age_id', this.model.ageId);
    this.downloadCsvData('by_indicator_id', parameters);
    this.logEvent('DataForIndicator', parameters.build());
  }

  /**
  * Export all nearest neighbours indicator data for specific profile.
  */
  exportProfileNearestNeighboursData() {
    const parameters = this.getExportParameters(this.nearestNeighbourCode);
    this.addProfileIdParameter(parameters);
    parameters.add('category_area_code', CategoryAreaCodeHelper.getCategoryAreaCode(this.model.parentCode));
    this.downloadCsvData('by_profile_id', parameters);
    this.logEvent('DataForProfile', parameters.build());
  }

  /**
  * Export nearest neighbours indicator data for profile group
  */
  exportGroupNearestNeighboursData() {
    const parameters = this.getExportParameters(this.nearestNeighbourCode);
    parameters.add('group_id', this.model.groupId);
    parameters.add('category_area_code', CategoryAreaCodeHelper.getCategoryAreaCode(this.model.parentCode));
    this.downloadCsvData('by_group_id', parameters);
    this.logEvent('DataForGroup', parameters.build());
  }

  /**
  * Export nearest neighbour indicator data for single indicator
  */
  exportIndicatorNearestNeighboursData() {
    const parameters = this.getExportParameters(this.nearestNeighbourCode);
    this.addProfileIdParameter(parameters);
    parameters.add('indicator_ids', this.groupRoot.IID);
    parameters.add('category_area_code', CategoryAreaCodeHelper.getCategoryAreaCode(this.model.parentCode));
    parameters.add('sex_id', this.model.sexId);
    parameters.add('age_id', this.model.ageId);
    this.downloadCsvData('by_indicator_id', parameters);
    this.logEvent('DataForIndicator', parameters.build());
  }

  exportPopulation(parentAreaCode: string): void {
    const parameters = new ParameterBuilder()
      .add('are', parentAreaCode)
      .add('gid', GroupIds.PracticeProfiles_Population)
      .add('ati', this.model.parentTypeId).build();

    const url = this.urls.corews + 'GetData.ashx?s=db&pro=qp&' + parameters;
    this.openFile(url);
    this.logEvent('Population', parameters);
  }

  exportAddresses(parentAreaCode: string): void {
    const parameters = new ParameterBuilder()
      .add('parent_area_code', parentAreaCode)
      .add('area_type_id', this.model.areaTypeId).build();
    const url = this.urls.corews + 'api/area_addresses/csv/by_parent_area_code?' + parameters;
    this.openFile(url);
    this.logEvent('Addresses', parameters);
  }

  logEvent(action: string, parameters: string) {
    this.ftHelperService.logEvent('Download', action, parameters);
  }

  addProfileIdParameter(parameters) {
    const search = this.ftHelperService.getSearch();
    if (!search.isInSearchMode()) {
      parameters.add('profile_id', this.model.profileId);
    }
  }

  downloadCsvData(byTerm: string, parameters: ParameterBuilder) {
    const url = this.urls.corews + 'api/all_data/csv/' + byTerm + '?' + parameters.build();
    this.openFile(url);
  }

  downloadCsvMetadata(byTerm: string, parameters: ParameterBuilder) {
    const url = this.urls.corews + 'api/indicator_metadata/csv/' + byTerm + '?' + parameters.build();
    this.openFile(url);
  }

  getExportParameters(parentAreaCode: string): ParameterBuilder {
    const model = this.model;
    const parameters = new ParameterBuilder()
      .add('parent_area_code', parentAreaCode)
      .add('parent_area_type_id', model.parentTypeId)
      .add('child_area_type_id', model.areaTypeId);
    return parameters;
  }

  openFile(url: string) {
    window.open(url, '_blank');
  }
}
