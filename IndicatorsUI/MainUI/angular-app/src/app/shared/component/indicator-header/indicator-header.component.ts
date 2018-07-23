import { Component, Input } from '@angular/core';

@Component({
  selector: 'ft-indicator-header',
  templateUrl: './indicator-header.component.html',
  styleUrls: ['./indicator-header.component.css']
})
export class IndicatorHeaderComponent {

  @Input() header: IndicatorHeader;
}

export class IndicatorHeader {
  constructor(public indicatorName: string,
    public hasNewData: boolean,
    public comparatorName: string,
    public valueType: string,
    public unit: string,
    public ageSexLabel: string) { }
}
