export class IndicatorHeader {
    constructor(public indicatorName: string,
        public hasNewData: boolean,
        public comparatorName: string,
        public valueType: string,
        public unit: string,
        public ageSexLabel: string) { }
}
