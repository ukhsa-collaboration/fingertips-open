import {
  TestBed,
  getTestBed,
  async,
  inject
} from '@angular/core/testing';
import { HttpModule } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { IndicatorService } from './indicator.service';
import { FTHelperService } from '../helper/ftHelper.service';
import { HttpService } from './http.service';

describe('Indicator Service', () => {

  let ftHelperService: any;
  let httpService: any;

  beforeEach(async(() => {

    ftHelperService = jasmine.createSpyObj('FTHelperService', ['getURL', 'version', 'getSearch']);
    httpService = jasmine.createSpyObj('HttpService', ['httpGet']);

    TestBed.configureTestingModule({
      providers: [
        IndicatorService,
        { provide: FTHelperService, useValue: ftHelperService },
        { provide: HttpService, useValue: httpService }
      ],
      imports: [
        HttpModule
      ]
    });

    // Arrange: ftHelperService (TODO figure out how to arrange separately per test)
    ftHelperService.getURL.and.returnValue({ bridge: '' });
    ftHelperService.version.and.returnValue('');
    ftHelperService.getSearch.and.returnValue({});
  }));

  it('should get indicator for all area async',
    async(inject([IndicatorService], (indicatorService: IndicatorService) => {

      // Arrange: 
      let data = [
        {
          AgeId: 1,
          SexId: 4
        }];
      httpService.httpGet.and.returnValue(Observable.of(data));

      indicatorService.getSingleIndicatorForAllArea(1, 1, 'AA',
        1, 1, 1, 1, 1).
        subscribe(
          (data) => {
            expect(data[0].AgeId).toBe(1);
            expect(data[0].SexId).toBe(4);
          });
    })));
});
