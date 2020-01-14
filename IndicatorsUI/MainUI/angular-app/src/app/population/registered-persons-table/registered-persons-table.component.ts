import { Component, Input, EventEmitter, Output } from '@angular/core';
import { RegisteredPersons } from '../population';
import { IndicatorIds } from '../../shared/constants';

@Component({
  selector: 'ft-registered-persons-table',
  templateUrl: './registered-persons-table.component.html',
  styleUrls: ['./registered-persons-table.component.css']
})
export class RegisteredPersonsTableComponent {

  @Input() registeredPersons: RegisteredPersons[];
  @Output() metadataToShow = new EventEmitter<number>();

  public showMetadata() {
    this.metadataToShow.emit(IndicatorIds.QofListSize);
  }
}
