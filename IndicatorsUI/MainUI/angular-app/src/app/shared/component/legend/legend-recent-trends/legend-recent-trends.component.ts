import { Component, Input, SimpleChanges, OnChanges } from '@angular/core';
import { FTHelperService } from '../../../service/helper/ftHelper.service';

@Component({
  selector: 'ft-legend-recent-trends',
  templateUrl: './legend-recent-trends.component.html',
  styleUrls: ['./legend-recent-trends.component.css']
})
export class LegendRecentTrendsComponent implements OnChanges {

  @Input() showRecentTrends: boolean = null;

  constructor(private ftHelperService: FTHelperService) { }

  ngOnChanges(changes: SimpleChanges) {
  }

  showTrendInfo() {
    this.ftHelperService.showTrendInfo();
  }
}
