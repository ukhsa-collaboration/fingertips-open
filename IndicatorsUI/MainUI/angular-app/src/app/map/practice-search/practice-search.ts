import { LatitudeLongitude } from '../../typings/FT.d';

export class autoCompleteResult {
    polygonAreaCode: string;
    displayName: string;
    constructor(polygonAreaCode: string, name: string, parentAreaName: string) {
        this.polygonAreaCode = polygonAreaCode;
        this.displayName = name + ", " + parentAreaName.replace('NHS ', '')
    }
};

export class Practice {
    lat: number;
    lng: number;
    selected: boolean;
    constructor(public areaName: string, public areaCode: string, public addressLine1: string,
        public addressLine2: string, public postcode: string, public distanceValF: string,
        pos: LatitudeLongitude) {
        this.lat = pos.Lat;
        this.lng = pos.Lng;
        this.selected = false;
    }
}
