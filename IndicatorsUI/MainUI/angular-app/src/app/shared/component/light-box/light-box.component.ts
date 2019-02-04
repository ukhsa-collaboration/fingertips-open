import { Component, OnChanges, Input, Output, SimpleChanges, EventEmitter, ChangeDetectorRef } from '@angular/core';

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
    switch (this.lightBoxConfig.Type) {
      case LightBoxTypes.Ok:
        this.isInfoBoxOk = false;
        break;
      case LightBoxTypes.OkCancel:
        this.isInfoBoxOkCancel = false;
        break;
    }

    this.showLightBox = false;
    this.emitLightBoxActionConfirmed.emit(actionConfirmed);
  }
}

export class LightBoxConfig {
  public Type: number;
  public Html: string;
  public Title: string;
  public Height: number;
  public Top: number = 500;
}

export class LightBoxTypes {
  public static readonly Ok = 1
  public static readonly OkCancel = 2;
};
