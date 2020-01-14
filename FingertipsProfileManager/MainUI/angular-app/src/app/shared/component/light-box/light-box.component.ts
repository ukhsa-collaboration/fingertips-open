import {
  Component, OnChanges, Input, Output, SimpleChanges, EventEmitter, ChangeDetectorRef
} from '@angular/core';
import { LightBoxConfig, LightBoxTypes } from './light-box';

@Component({
  selector: 'ft-light-box',
  templateUrl: './light-box.component.html',
  styleUrls: ['./light-box.component.css']
})
export class LightBoxComponent implements OnChanges {

  @Input() lightBoxConfig: LightBoxConfig = null;
  @Output() emitLightBoxActionConfirmed = new EventEmitter();

  showLightBox = false;
  isInfoBoxOk = false;
  isInfoBoxOkCancel = false;
  okButtonText = 'OK';

  constructor(private changeDetectorRef: ChangeDetectorRef) { }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['lightBoxConfig']) {
      if (this.lightBoxConfig) {
        this.loadLightBox();
      }
    }
  }

  loadLightBox() {
    if (this.lightBoxConfig !== null) {

      // Type
      switch (this.lightBoxConfig.Type) {
        case LightBoxTypes.Ok:
          this.isInfoBoxOk = true;
          break;
        case LightBoxTypes.OkCancel:
          this.isInfoBoxOkCancel = true;
          break;
      }

      // Update OK button text if provided
      if (this.lightBoxConfig.OkButtonText !== '') {
        this.okButtonText = this.lightBoxConfig.OkButtonText;
      }

      // Display the light box
      this.showLightBox = true;
    }
  }

  confirm() {
    this.closePopupAndEmit(true);
  }

  cancel() {
    this.closePopupAndEmit(false);
  }

  closePopupAndEmit(actionConfirmed: boolean) {
    if (actionConfirmed) {
      switch (this.lightBoxConfig.Type) {
        case LightBoxTypes.Ok:
          this.isInfoBoxOk = false;
          break;
        case LightBoxTypes.OkCancel:
          this.isInfoBoxOkCancel = false;
          break;
      }
    }

    this.showLightBox = false;

    this.emitLightBoxActionConfirmed.emit(actionConfirmed);
  }
}
