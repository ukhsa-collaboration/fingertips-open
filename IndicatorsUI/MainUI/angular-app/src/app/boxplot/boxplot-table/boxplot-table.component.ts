import { Component, Input } from '@angular/core';
import { BoxplotData } from '../boxplot';

@Component({
  selector: 'ft-boxplot-table',
  templateUrl: './boxplot-table.component.html',
  styleUrls: ['./boxplot-table.component.css']
})
export class BoxplotTableComponent {

  @Input() boxplotData: BoxplotData;
}
