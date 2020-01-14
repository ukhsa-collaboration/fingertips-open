import { Profile, AreaType } from './profile';

export class Report {
    public id: number = null;
    public name = '';
    public file = '';
    public profiles: number[] = [];
    public parameters: string[] = [];
    public notes: string;
    public isLive: boolean;
    public areaTypeIds: number[] = [];
}

export class ReportListView {
    public id: number = null;
    public name = '';
    public file = '';
    public profiles: Profile[] = [];
    public parameters: string[] = [];
    public notes: string;
    public isLive: boolean;
    public areaTypes: AreaType[] = [];
}
