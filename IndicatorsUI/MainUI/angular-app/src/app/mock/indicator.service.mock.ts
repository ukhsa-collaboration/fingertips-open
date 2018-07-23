import {IndicatorService} from '../shared/service/api/indicator.service';
import {MockFTHelperService} from './ftHelper.service.mock';
import {
    FTRoot, CoreDataSet, IndicatorMetadata, GroupRoot, FTIndicatorSearch,
    IndicatorMetadataTextProperty, ComparatorMethod, IndicatorStats, Population, Category,
    PopulationSummary
} from '../typings/FT.d';
import { Observable } from 'rxjs/Observable';
export class MockIndicatorService extends IndicatorService {
    constructor() {
        super(null, new MockFTHelperService());
    }
    getSingleIndicatorForAllArea(groupId: number, areaTypeId: number, parentAreaCode: string,
        profileId: number, comparatorId: number, indicatorId: number, sexId: number,
        ageId: number):Observable<CoreDataSet>{
            let coreDatasets=[{  
                AgeId: 1,
                SexId: 4,
                AreaCode: 'E06000001',
                Denom: -1.0,
                Sig:{  
                   4:0
                },
                Val: 33.178,
                ValF: '33.2',
                Count: -1.0
             },
             {  
                AgeId: 1,
                SexId: 4,
                AreaCode: 'E06000002',
                Denom: -1.0,
                Sig:{  
                   4:0
                },
                Val: 40.216,
                ValF: '40.2',
                Count: -1.0
             }];
            // return Observable.of([
            //     coreDatasets   
            // ]);
           return  Observable.create( observer => {
                observer.next(coreDatasets);
                observer.complete();
            });
        }
}