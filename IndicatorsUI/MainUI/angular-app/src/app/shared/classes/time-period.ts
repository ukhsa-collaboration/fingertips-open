import { Grouping } from '../../typings/FT';
import { String } from 'typescript-string-operations';

export class TimePeriod {
    year: number;
    yearRange: number;
    quarter: number;
    month: number;

    constructor(private grouping: Grouping) {
        this.year = this.grouping.DataPointYear;
        this.yearRange = this.grouping.YearRange;
        this.quarter = this.grouping.DataPointQuarter;
        this.month = this.grouping.DataPointMonth;
    }

    getSortableNumber(): string {
        const quarter = this.quarter === -1 ? 0 : this.quarter;
        const month = this.month === -1 ? 0 : this.month;
        return String.Format('{0:0000}{1:00}{2:00}', this.year, quarter, month);
    }
}
