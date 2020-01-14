import { Tabs } from "../../constants";
import { isDefined } from "@angular/compiler/src/util";
import { CoreDataSet } from "../../../typings/FT";
import { SignificanceFormatter } from "../../classes/significance-formatter";

export class CsvConfig {
    tab: Tabs;
    csvData: CsvData[];
}

export class CsvData {
    indicatorId: string;
    indicatorName: string;
    parentCode: string;
    parentName: string;
    areaCode: string;
    areaName: string;
    areaType: string;
    sex: string;
    age: string;
    categoryType: string;
    category: string;
    timePeriod: string;
    value: string;
    lowerCiLimit95: string;
    upperCiLimit95: string;
    lowerCiLimit99_8: string;
    upperCiLimit99_8: string;
    count: string;
    denominator: string;
    valueNote: string;
    recentTrend: string;
    comparedToEnglandValueOrPercentiles: string;
    comparedToRegionValueOrPercentiles: string;
    timePeriodSortable: string;
    newData: string;
    comparedToGoal: string;
}

export class CsvDataHelper {
    public static getDisplayValue(value: number): string {
        if (isDefined(value) && value !== -1) {
            return value.toString();
        }

        return '';
    }

    public static getSignificanceValue(data: CoreDataSet, polarityId: number, comparatorId: number,
        comparatorMethodId: number): string {

        if (isDefined(data) && isDefined(data.Sig[comparatorId])) {
            return new SignificanceFormatter(polarityId, comparatorMethodId).getLabel(Number(data.Sig[comparatorId]));
        }

        return '';
    }
}
