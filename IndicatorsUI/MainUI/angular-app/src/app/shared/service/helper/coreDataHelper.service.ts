import { Injectable } from '@angular/core';
import { FTRoot, CoreDataSet, Unit, ValueWithUnit } from '../../../typings/FT';
declare let FTWrapper: FTRoot;

@Injectable()
export class CoreDataHelperService {
    addOrderandPercentilesToData(coreDataSets: CoreDataSet[]) {
        return FTWrapper.coreDataHelper.addOrderandPercentilesToData(coreDataSets);
    }
    valueWithUnit(unit: Unit): ValueWithUnit {
        return FTWrapper.coreDataHelper.valueWithUnit(unit);
    }
    getFullLabel(value: string, options?: any): string {
        return FTWrapper.valueWithUnit.getFullLabel(value, options);
    }
}
