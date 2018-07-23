import {CoreDataHelperService} from '../shared/service/helper/coreDataHelper.service';
import {MockFTHelperService} from './ftHelper.service.mock';
import { FTRoot, CoreDataSet, Unit, ValueWithUnit } from '../typings/FT.d';

export class MockCoreDataHelperService {
    addOrderandPercentilesToData(coreDataSet: CoreDataSet) {
        return new Map<string, CoreDataSet>();
    }
    valueWithUnit(unit: Unit): ValueWithUnit {
        let value: ValueWithUnit;
        return value;;
    }
    getFullLabel(value: string, options?: any): string {
        return '';
    }
}
