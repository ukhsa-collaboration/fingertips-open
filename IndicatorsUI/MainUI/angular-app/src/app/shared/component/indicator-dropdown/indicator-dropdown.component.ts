import { Component, Input, Output, EventEmitter } from '@angular/core';
import { TypeaheadMatch } from '../../../../../node_modules/ngx-bootstrap';

@Component({
  selector: 'ft-indicator-dropdown',
  templateUrl: './indicator-dropdown.component.html',
  styleUrls: ['./indicator-dropdown.component.css']
})

export class IndicatorDropdownComponent {

  public typeaheadLoading: boolean;
  public typeaheadNoResults: boolean;
  public indicatorName = '';

  @Input() indicatorNames: string[] = null;
  @Output() emitIndicatorNameSelected = new EventEmitter();

  constructor() { }

  public typeaheadOnSelect(e: TypeaheadMatch): void {
    this.emitIndicatorNameSelected.emit(e.item);
    this.indicatorName = '';
  }

  public changeTypeaheadLoading(e: any): void {
  }

  public changeTypeaheadNoResults(e: boolean): void {
  }
}
