import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import * as _ from 'underscore';
import { FormGroup, FormControl } from '@angular/forms';


@Component({
  selector: 'app-report-parameters',
  templateUrl: './report-parameters.component.html',
  styleUrls: ['./report-parameters.component.css']
})
export class ReportParametersComponent implements OnInit {

  parameters: string[] = ["areaCode", "areaTypeId", "groupId", "parentCode", "parentTypeId"]
  selectedParameter: string;

  @Input() selectedParameters: string[] = [];
  @Output() getParameters: EventEmitter<string[]> = new EventEmitter<string[]>();

  parametersForm: FormGroup;

  constructor() {
    this.parametersForm = new FormGroup({
      parameterControl: new FormControl(null)
    });
  }

  ngOnInit() {
  }

  addParameter() {

    this.selectedParameter = this.parametersForm.get('parameterControl').value;

    if (!_.contains(this.selectedParameters, this.selectedParameter) && this.selectedParameter) {
      this.selectedParameters.push(this.selectedParameter);
      this.getParameters.emit(this.selectedParameters);
    }
  }

  removeParameter(index) {
    this.selectedParameters.splice(index, 1);
    this.getParameters.emit(this.selectedParameters);
  }
}
