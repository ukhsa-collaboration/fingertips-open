import { Injectable } from '@angular/core';
import {FTRoot, Area, GroupRoot} from '../../../typings/FT.d';
declare var FTWrapper: FTRoot;

@Injectable()
export class BridgeDataHelperService {
    getCurrentComparator(): Area{
        return FTWrapper.bridgeDataHelper.getCurrentComparator();
    }
    getCurrentGroupRoot(): GroupRoot {
        return FTWrapper.bridgeDataHelper.getGroopRoot();
    }
    getComparatorId(): number {
        return FTWrapper.bridgeDataHelper.getComparatorId();
    }
}
