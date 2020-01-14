import { Component, OnChanges, Input, Output, SimpleChanges, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser'
import { GroupingSubheading } from '../../../model/profile';
import { $ } from 'protractor';

@Component({
  selector: 'ft-light-box-indicator-reorder',
  templateUrl: './light-box-indicator-reorder.component.html',
  styleUrls: ['./light-box-indicator-reorder.component.css']
})
export class LightBoxIndicatorReorderComponent implements OnChanges {

  @Input() lightBoxIndicatorReorderConfig: LightBoxIndicatorReorderConfig = null;
  @Output() emitLightBoxActionConfirmed = new EventEmitter();
  @Output() emitLightBoxInputText = new EventEmitter();

  showLightBox = false;
  isInfoBox = false;
  lightBoxInputText: string;
  errorMessage = '';

  constructor(private ref: ChangeDetectorRef, private sanitizer: DomSanitizer) { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['lightBoxIndicatorReorderConfig']) {
      if (this.lightBoxIndicatorReorderConfig) {
        this.loadLightBoxIndicatorReorder();
      }
    }
  }

  loadLightBoxIndicatorReorder() {
    if (this.lightBoxIndicatorReorderConfig.ActionType === 'EDIT') {
      this.lightBoxInputText = this.lightBoxIndicatorReorderConfig.InputBoxText;
    } else {
      this.lightBoxInputText = '';
    }

    if (this.lightBoxIndicatorReorderConfig !== null) {
      this.isInfoBox = true;
    }

    this.showLightBox = true;
  }

  setSubheading(subheading: string) {
    this.lightBoxInputText = subheading;
  }

  setLightBoxInputText() {
    this.lightBoxInputText = (<HTMLInputElement>document.getElementById('txt-light-box')).value;
    this.closePopupAndEmit(true);
  }

  cancel() {
    this.closePopupAndEmit(false);
  }

  closePopupAndEmit(actionConfirmed: boolean) {
    if (actionConfirmed) {
      if (this.validateInput()) {
        this.isInfoBox = false;
        this.emitLightBoxInputText.emit(this.lightBoxInputText);
      } else {
        this.errorMessage = 'Please enter a valid ' + this.lightBoxIndicatorReorderConfig.InputBoxLabelName;
        return false;
      }
    }

    this.showLightBox = false;
    this.emitLightBoxActionConfirmed.emit(actionConfirmed);
  }

  validateInput() {
    if (this.lightBoxInputText === undefined || this.lightBoxInputText.trim().length === 0) {
      return false;
    }

    return true;
  }
}

export class LightBoxIndicatorReorderConfig {
  public Title: string;
  public Height: number;
  public OkButtonText: string;
  public CancelButtonText: string;
  public InputBoxLabelName: string;
  public InputBoxText: string;
  public GroupingSubheadings: GroupingSubheading[];
  public ActionType: string;
}
