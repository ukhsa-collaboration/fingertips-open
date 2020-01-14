import { Component, OnChanges, Input, Output, SimpleChanges, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { LightBoxWithInputConfig, LightBoxWithInputTypes } from './light-box-with-input';

@Component({
  selector: 'ft-light-box-with-input',
  templateUrl: './light-box-with-input.component.html',
  styleUrls: ['./light-box-with-input.component.css']
})
export class LightBoxWithInputComponent implements OnChanges {

  @Input() lightBoxWithInputConfig: LightBoxWithInputConfig = null;
  @Output() emitLightBoxWithInputActionConfirmed = new EventEmitter();
  @Output() emitLightBoxInputText = new EventEmitter();

  showLightBox = false;
  isInfoBoxOk = false;
  isInfoBoxOkCancel = false;
  lightBoxInputText: string;
  errorMessage: string;

  constructor(private changeDetectorRef: ChangeDetectorRef) { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['lightBoxWithInputConfig']) {
      if (this.lightBoxWithInputConfig) {
        this.loadLightBox();
      }
    }
  }

  loadLightBox() {
    if (this.lightBoxWithInputConfig !== null) {
      this.errorMessage = '';
      this.lightBoxInputText = this.lightBoxWithInputConfig.InputText;

      // Type
      switch (this.lightBoxWithInputConfig.Type) {
        case LightBoxWithInputTypes.Ok:
          this.isInfoBoxOk = true;
          break;
        case LightBoxWithInputTypes.OkCancel:
          this.isInfoBoxOkCancel = true;
          break;
      }

      this.showLightBox = true;
    }
  }

  confirm() {
    this.lightBoxInputText = (<HTMLInputElement>document.getElementById('info-box-text-input')).value;
    this.closePopupAndEmit(true);
  }

  cancel() {
    this.closePopupAndEmit(false);
  }

  closePopupAndEmit(actionConfirmed: boolean) {
    if (actionConfirmed) {
      if (this.validateInput()) {
        switch (this.lightBoxWithInputConfig.Type) {
          case LightBoxWithInputTypes.Ok:
            this.isInfoBoxOk = false;
            break;
          case LightBoxWithInputTypes.OkCancel:
            this.isInfoBoxOkCancel = false;
            break;
        }
        this.emitLightBoxInputText.emit(this.lightBoxInputText);
      } else {
        this.errorMessage = 'Please enter a valid area list name';
        return false;
      }
    }


    this.showLightBox = false;
    this.emitLightBoxWithInputActionConfirmed.emit(actionConfirmed);
  }

  validateInput() {
    if (this.lightBoxInputText === undefined || this.lightBoxInputText.trim().length === 0) {
      return false;
    }

    return true;
  }
}
