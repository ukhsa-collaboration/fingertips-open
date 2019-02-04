import { Injectable } from '@angular/core';
import { FTRoot, CoreDataSet, Unit, ValueWithUnit } from '../../../typings/FT.d';
declare let FTWrapper: FTRoot;

@Injectable()
export class CoreDataHelperService {
    addOrderandPercentilesToData(coreDataSet: CoreDataSet) {
        return FTWrapper.coreDataHelper.addOrderandPercentilesToData(coreDataSet);
    }
    valueWithUnit(unit: Unit): ValueWithUnit {
        return FTWrapper.coreDataHelper.valueWithUnit(unit);
    }
    getFullLabel(value: string, options?: any): string {
        return FTWrapper.valueWithUnit.getFullLabel(value, options);
    }
}
