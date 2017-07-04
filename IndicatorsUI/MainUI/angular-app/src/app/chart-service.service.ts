import { Injectable } from '@angular/core';
import {Http, Response} from "@angular/http";
import 'rxjs/add/operator/map';

@Injectable()
export class ChartServiceService {
   
    constructor(private http: Http) { }

    public GetData() : any {
        let url =
            "/api/indicator_statistics?group_id=1000049&child_area_type_id=102&parent_area_code=E92000001&profile_id=19&restrict_to_profile_ids=19&v=115";
        this.http.get(url)
            .map((res: Response) => res.json())
            .subscribe(
            data => this.callBack(data),
            err => console.log(err)
            );
       
        return [
            [760, 801, 848, 895, 965],
            [733, 853, 939, 980, 1080],
            [714, 762, 817, 870, 918],
            [724, 802, 806, 871, 1000],
            [54, 60.9, 62.5, 63.4, 71.1],
            [834, 836, 864, 882, 910],
            [834, 836, 864, 882, 910]
        ];
    }

    public callBack(data: Object): void {
        
        console.log(data);
    }

}
