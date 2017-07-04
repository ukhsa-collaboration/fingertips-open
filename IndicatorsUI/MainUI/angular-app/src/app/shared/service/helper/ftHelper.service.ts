import { Injectable } from '@angular/core';
import {FTRoot, Area, GroupRoot, FTModel, IndicatorMetadata, Url, FTDisplay,CoreDataSet} from '../../../typings/FT.d';
declare var FTWrapper: FTRoot;

@Injectable()
export class FTHelperService {
    getAreaName(areaCode: string): string{
        return FTWrapper.display.getAreaName(areaCode);
    }
    getAreaTypeName(): string{
        return FTWrapper.display.getAreaTypeName();
    }
    getFTModel(): FTModel {
        return FTWrapper.model();
    }
    getMetadata(IID: number): IndicatorMetadata{
        return FTWrapper.indicatorHelper.getMetadataHash()[IID];
    }
    getURL(): Url{
        return FTWrapper.url();
    }
}
