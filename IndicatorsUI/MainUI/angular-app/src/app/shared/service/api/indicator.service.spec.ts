import {
  TestBed,
  getTestBed,
  async,
  inject
} from '@angular/core/testing';
import {
  Headers, BaseRequestOptions,
  Response, HttpModule, Http, XHRBackend, RequestMethod
} from '@angular/http';

import { ResponseOptions } from '@angular/http';
import { MockBackend, MockConnection } from '@angular/http/testing';
import { IndicatorService } from './indicator.service';
import { FTHelperService } from '../helper/ftHelper.service';
import { MockFTHelperService } from '../../../mock/ftHelper.service.mock';

describe('Indicator Service', () => {
  let mockBackend: MockBackend;
  let ftService: FTHelperService;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      providers: [
        IndicatorService,
        MockBackend,
        BaseRequestOptions,
        {
          provide: Http,
          deps: [MockBackend, BaseRequestOptions],
          useFactory:
            (backend: XHRBackend, defaultOptions: BaseRequestOptions) => {
              return new Http(backend, defaultOptions);
            }
        },
        { provide: FTHelperService, useClass: MockFTHelperService }
      ],
      imports: [
        HttpModule
      ]
    });
    mockBackend = getTestBed().get(MockBackend);
    ftService = getTestBed().get(FTHelperService);
  }));

  it('should get indicator for all area async',
    async(inject([IndicatorService], (indicatorService: IndicatorService) => {
      mockBackend.connections.subscribe(
        (connection: MockConnection) => {
          connection.mockRespond(new Response(
            new ResponseOptions({
              body: [
                {
                  AgeId: 1,
                  SexId: 4
                }]
            }
            )));
        });

      indicatorService.getSingleIndicatorForAllArea(1, 1, 'AA',
        1, 1, 1, 1, 1).
        subscribe(
          (data) => {
            expect(data[0].AgeId).toBe(1);
            expect(data[0].SexId).toBe(4);
          });
    })));
});