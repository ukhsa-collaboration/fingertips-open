import { Component, Input } from '@angular/core';
import { IndicatorHeader } from './indicator-header';

@Component({
  selector: 'ft-indicator-header',
  templateUrl: './indicator-header.component.html',
  styleUrls: ['./indicator-header.component.css']
})
export class IndicatorHeaderComponent {

  @Input() header: IndicatorHeader;
}
