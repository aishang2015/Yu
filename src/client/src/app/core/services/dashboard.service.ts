import { Injectable, Injector } from '@angular/core';
import { BaseService } from './base.service';
import { HttpClient } from '@angular/common/http';
import { CommonConstant } from '../constants/common-constant';
import { UriConstant } from '../constants/uri-constant';
import { ApiDetail } from 'src/app/right-manage/models/api-detail';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})

export class DashboardService extends BaseService {

    // 构造函数
    constructor(private injector: Injector,
        private http: HttpClient) { super(injector); }

    init(){
        return this.SafeRequest(Observable.create()).toPromise();
    }

}