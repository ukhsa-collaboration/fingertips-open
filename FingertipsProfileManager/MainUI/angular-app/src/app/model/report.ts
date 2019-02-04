import { Profile } from 'app/model/profile';

export class Report {
    public id: number = null;
    public name: string = '';
    public file: string = '';
    public profiles: number[] = [];
    public parameters: string[] = [];
    public notes: string;
    public isLive: boolean;
}

export class ReportListView {
    public id: number = null;
    public name: string = '';
    public file: string = '';
    public profiles: Profile[] = [];
    public parameters: string[] = [];
    public notes: string;
    public isLive: boolean;
}
