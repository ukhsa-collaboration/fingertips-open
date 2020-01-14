import { Component, ChangeDetectorRef, OnInit } from '@angular/core';
import { AreaService } from '../../shared/service/api/area.service';
import { AreaListService } from '../../shared/service/api/arealist.service';
import { forkJoin } from 'rxjs';
import { AreaList, AreaType } from '../../typings/FT';
import { LightBoxConfig, LightBoxTypes } from '../../shared/component/light-box/light-box';
import {
  LightBoxWithInputConfig, LightBoxWithInputTypes
} from '../../shared/component/light-box-with-input/light-box-with-input';

@Component({
  selector: 'ft-arealist-index',
  templateUrl: './arealist-index.component.html',
  styleUrls: ['./arealist-index.component.css'],
  providers: [AreaListService]
})
export class ArealistIndexComponent implements OnInit {

  userId: string;
  areaListId: number;
  areaListName: string;
  selectedRecordUserId: string;
  publicId: string;
  areaLists: AreaList[] = [];
  areaTypes: AreaType[] = [];
  areaListRows: AreaListRow[] = [];

  sortOrder = false;
  sortOrderAreaListName = false;
  sortOrderAreaTypeName = false;

  lightBoxConfig: LightBoxConfig;
  lightBoxWithInputConfig: LightBoxWithInputConfig;

  actionType: ActionTypes;

  constructor(private areaListService: AreaListService, private areaService: AreaService, private ref: ChangeDetectorRef) { }

  ngOnInit() {
    this.userId = document.getElementById('ft-arealist-index').getAttribute('user-id');

    const areaListsObservable = this.areaListService.getAreaLists(this.userId);
    const areaTypesObservable = this.areaService.getAreaTypes();

    forkJoin([areaListsObservable, areaTypesObservable]).subscribe(results => {
      this.areaLists = <AreaList[]>results[0];
      this.areaTypes = <AreaType[]>results[1];

      this.areaLists.forEach(element => {
        const row: AreaListRow = new AreaListRow();
        row.AreaListId = element.Id;
        row.AreaListName = element.ListName;
        row.AreaTypeId = element.AreaTypeId;
        row.AreaTypeName = this.areaTypes.find(x => x.Id === element.AreaTypeId).Short;
        row.UserId = element.UserId;
        row.PublicId = element.PublicId;

        this.areaListRows.push(row)
      });

      this.hideSpinner();
    });
  }

  displayPage(): boolean {
    const windowLocationHref = window.location.href.trim().toLowerCase();
    if (windowLocationHref.indexOf('area-list') > 0 &&
      windowLocationHref.indexOf('create') <= 0 &&
      windowLocationHref.indexOf('edit') <= 0) {
      return true;
    } else {
      return false;
    }
  }

  isAnyData(): boolean {
    if (this.areaLists.length > 0) {
      return true;
    } else {
      return false;
    }
  }

  hideSpinner() {
    $('#spinner').hide();
  }

  sort(column: number) {
    let tempAreaListRows: Array<AreaListRow>;
    tempAreaListRows = this.areaListRows.slice(0);

    this.sortOrder = this.getSortOrder(column);

    tempAreaListRows.sort((a, b) => {
      if (this.sortOrder) {
        switch (column) {
          case SortColumns.AreaListName:
            if (a.AreaListName.toLowerCase() < b.AreaListName.toLowerCase()) {
              return -1;
            } else if (a.AreaListName.toLowerCase() > b.AreaListName.toLowerCase()) {
              return 1;
            } else {
              return 0;
            }
          case SortColumns.AreaTypeName:
            if (a.AreaTypeName.toLowerCase() < b.AreaTypeName.toLowerCase()) {
              return -1;
            } else if (a.AreaTypeName.toLowerCase() > b.AreaTypeName.toLowerCase()) {
              return 1;
            } else {
              return 0;
            }
        }
      } else {
        switch (column) {
          case SortColumns.AreaListName:
            if (a.AreaListName.toLowerCase() < b.AreaListName.toLowerCase()) {
              return 1;
            } else if (a.AreaListName.toLowerCase() > b.AreaListName.toLowerCase()) {
              return -1;
            } else {
              return 0;
            }
          case SortColumns.AreaTypeName:
            if (a.AreaTypeName.toLowerCase() < b.AreaTypeName.toLowerCase()) {
              return 1;
            } else if (a.AreaTypeName.toLowerCase() > b.AreaTypeName.toLowerCase()) {
              return -1;
            } else {
              return 0;
            }
        }
      }
    });

    this.areaListRows = tempAreaListRows;
  }

  getSortOrder(column: number): boolean {
    let sortOrder = false;

    switch (column) {
      case SortColumns.AreaListName:
        this.sortOrderAreaListName = !this.sortOrderAreaListName;
        sortOrder = this.sortOrderAreaListName;
        break;
      case SortColumns.AreaTypeName:
        this.sortOrderAreaTypeName = !this.sortOrderAreaTypeName;
        sortOrder = this.sortOrderAreaTypeName;
        break;
    }

    return sortOrder;
  }

  helpViewAreaListOnDataPage() {
    // Show light box
    const config = new LightBoxConfig();
    config.Type = LightBoxTypes.Ok;
    config.Title = 'How to view your area lists';
    config.Html = 'When viewing the data for a profile select the "Your area lists" option in the "Areas grouped by" menu.<br><br>' +
      '<img src="../images/help-view-area-list-on-data-page.png"><br><br>' +
      'You will then be able to view any existing area lists or else create new ones.';
    config.Height = 540;
    config.Top = 200;
    this.lightBoxConfig = config;
  }

  deleteAreaListConfirmation(row: AreaListRow) {
    this.areaListId = row.AreaListId;
    this.selectedRecordUserId = row.UserId;
    this.publicId = row.PublicId;
    this.actionType = ActionTypes.Delete;

    // Show light box for delete
    const config = new LightBoxConfig();
    config.Top = 400;
    config.Type = LightBoxTypes.OkCancel;
    config.Title = 'Delete';
    config.Html = 'Are you sure you want to delete this list?';
    this.lightBoxConfig = config;
  }

  updateLightBoxActionConfirmed(actionConfirmed: boolean) {
    if (actionConfirmed) {
      if (this.actionType === ActionTypes.Delete) {
        this.deleteAreaList();
      }
    }
  }

  deleteAreaList() {
    const formData: FormData = new FormData();
    formData.append('areaListId', this.areaListId.toString());
    formData.append('userId', this.selectedRecordUserId.toString());
    formData.append('publicId', this.publicId.toString());

    this.areaListService.deleteAreaList(formData)
      .subscribe(
        (response) => {
          if (response.toString().toLowerCase() === 'success') {
            window.location.reload(true);
          }
        },
        (error) => {
          // Show light box
          const config = new LightBoxConfig();
          config.Top = 400;
          config.Type = LightBoxTypes.OkCancel;
          config.Title = 'Failed';
          config.Html = 'Failed to delete the area list, please try again.<br>' +
            'If the issue persists then please contact the administrator.';
          this.lightBoxConfig = config;
        }
      );
  }

  copyAreaListConfirmation(areaListId: number) {
    this.areaListId = areaListId;
    this.areaListName = this.areaLists.find(x => x.Id === areaListId).ListName;
    this.actionType = ActionTypes.Copy;

    // Show light box
    const config = new LightBoxWithInputConfig();
    config.Type = LightBoxWithInputTypes.OkCancel;
    config.Title = 'Copy area list';
    config.Html = 'Name of new list';
    config.InputText = this.areaListName + ' copy';
    this.lightBoxWithInputConfig = config;
  }

  updateLightBoxInputText(areaListName: string) {
    this.areaListName = areaListName;
  }

  updateLightBoxWithInputActionConfirmed(actionConfirmed: boolean) {
    if (actionConfirmed) {
      if (this.actionType === ActionTypes.Copy) {
        this.copyAreaList();
      }
    }
  }

  copyAreaList() {
    const areaList = this.areaLists.find(x => x.ListName === this.areaListName);
    if (areaList === undefined || areaList === null) {
      const formData: FormData = new FormData();
      formData.append('areaListId', this.areaListId.toString());
      formData.append('areaListName', this.areaListName);
      formData.append('userId', this.userId);

      this.areaListService.copyAreaList(formData)
        .subscribe(
          (response) => {
            if (response.toString().toLowerCase() === 'success') {
              window.location.reload(true);
            }
          },
          (error) => {
            // Show light box
            const config = new LightBoxConfig();
            config.Type = LightBoxTypes.OkCancel;
            config.Title = 'Failed';
            config.Html = 'Failed to copy the area list, please try again.<br>' +
              'If the issue persists then please contact the administrator.';
            this.lightBoxConfig = config;
          }
        );
    } else {
      // Show light box
      const config = new LightBoxConfig();
      config.Type = LightBoxTypes.Ok;
      config.Title = 'Area list name already taken';
      config.Html = 'Another area list already has that name. Please choose a different one.';
      this.lightBoxConfig = config;
    }
  }
}

export class AreaListRow {
  AreaListId: number;
  AreaListName: string;
  AreaTypeId: number
  AreaTypeName: string;
  UserId: string;
  PublicId: string;
}

export class SortColumns {
  public static readonly AreaListName = 1;
  public static readonly AreaTypeName = 2;
};

export class ActionTypes {
  public static readonly Info = 'INFO';
  public static readonly Delete = 'DELETE';
  public static readonly Copy = 'COPY';
}
