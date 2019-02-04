import { TestBed, getTestBed, async, inject } from '@angular/core/testing';
import { BaseRequestOptions, Response, HttpModule, Http, XHRBackend } from '@angular/http';
import { ResponseOptions } from '@angular/http';
import { AreaListService } from './arealist.service';
import { FTHelperService } from '../helper/ftHelper.service';
import { AreaTypeIds } from '../../shared';
import { HttpService } from './http.service';
import { Observable } from 'rxjs';

describe('AreaList Service', () => {

    let ftHelperService: any;
    let httpService: any

    beforeEach(async(() => {

        ftHelperService = jasmine.createSpyObj('FTHelperService', ['getURL', 'version']);
        httpService = jasmine.createSpyObj('HttpService', ['httpGet', 'httpPost']);

        TestBed.configureTestingModule({
            providers: [
                AreaListService,
                { provide: FTHelperService, useValue: ftHelperService },
                { provide: HttpService, useValue: httpService }
            ],
            imports: [
                HttpModule
            ]
        });

        ftHelperService.getURL.and.returnValue({ bridge: '' });
        ftHelperService.version.and.returnValue('');
    }));

    it('should get area lists',
        async(inject([AreaListService], (areaListService: AreaListService) => {

            let data = [
                {
                    Id: 6029,
                    ListName: 'Acc1',
                    AreaTypeId: 118,
                    PublicId: 'dI8niPkZM0'
                }];
            httpService.httpGet.and.returnValue(Observable.of(data));

            areaListService.getAreaLists('73585a17-85cb-4d7d-af08-bdce449b6c71')
                .subscribe(
                    (data) => {
                        expect(data[0].Id).toBe(6029);
                        expect(data[0].ListName).toBe('Acc1');
                        expect(data[0].AreaTypeId).toBe(118);
                        expect(data[0].PublicId).toBe('dI8niPkZM0');
                    });
        })));

    it('should get area list',
        async(inject([AreaListService], (areaListService: AreaListService) => {

            let data = [
                {
                    Id: 6029,
                    ListName: 'Acc1',
                    AreaTypeId: 118,
                    PublicId: 'dI8niPkZM0'
                }];
            httpService.httpGet.and.returnValue(Observable.of(data));

            areaListService.getAreaList(6029)
                .subscribe(
                    (data) => {
                        expect(data[0].Id).toBe(6029);
                        expect(data[0].ListName).toBe('Acc1');
                        expect(data[0].AreaTypeId).toBe(118);
                        expect(data[0].PublicId).toBe('dI8niPkZM0');
                    });
        })));

    it('should get area list by public id',
        async(inject([AreaListService], (areaListService: AreaListService) => {

            let data = [
                {
                    Id: 6029,
                    ListName: 'Acc1',
                    AreaTypeId: 118,
                    PublicId: 'dI8niPkZM0'
                }];
            httpService.httpGet.and.returnValue(Observable.of(data));

            areaListService.getAreaListByPublicId('dI8niPkZM0', '73585a17-85cb-4d7d-af08-bdce449b6c71')
                .subscribe(
                    (data) => {
                        expect(data[0].Id).toBe(6029);
                        expect(data[0].ListName).toBe('Acc1');
                        expect(data[0].AreaTypeId).toBe(118);
                        expect(data[0].PublicId).toBe('dI8niPkZM0');
                    });
        })));

    it('should get area codes from area list id',
        async(inject([AreaListService], (areaListService: AreaListService) => {

            let data = [
                {
                    AreaListId: 6029
                }];
            httpService.httpGet.and.returnValue(Observable.of(data));

            areaListService.getAreaCodesFromAreaListId(6029)
                .subscribe(
                    (data) => {
                        expect(data[0].AreaListId).toBe(6029);
                    });
        })));

    it('should get areas from area list area codes',
        async(inject([AreaListService], (areaListService: AreaListService) => {

            let data = [
                {
                    AreaTypeId: 118,
                    Code: 'RCF'
                }];
            httpService.httpGet.and.returnValue(Observable.of(data));

            const areaListAreaCodes: string[] = [];
            areaListAreaCodes.push('RCF');
            areaListAreaCodes.push('RF4');

            areaListService.getAreasFromAreaListAreaCodes(areaListAreaCodes)
                .subscribe(
                    (data) => {
                        expect(data[0].AreaTypeId).toBe(118);
                        expect(data[0].Code).toBe('RCF');
                    });
        })));

    it('should get areas with address from area list area codes',
        async(inject([AreaListService], (areaListService: AreaListService) => {

            let data = [
                {
                    AreaTypeId: 118,
                    Code: 'RCF'
                }];
            httpService.httpGet.and.returnValue(Observable.of(data));

            const areaListAreaCodes: string[] = [];
            areaListAreaCodes.push('RCF');
            areaListAreaCodes.push('RF4');

            areaListService.getAreasWithAddressFromAreaListAreaCodes(areaListAreaCodes)
                .subscribe(
                    (data) => {
                        expect(data[0].AreaTypeId).toBe(118);
                        expect(data[0].Code).toBe('RCF');
                    });
        })));
});
