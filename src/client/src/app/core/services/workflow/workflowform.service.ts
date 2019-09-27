import { Injectable, Injector } from '@angular/core';
import { BaseService } from '../base.service';
import { HttpClient } from '@angular/common/http';
import { UriConstant } from '../../constants/uri-constant';

@Injectable({
    providedIn: 'root'
})

export class WorkFlowFormService extends BaseService {

    // 构造函数
    constructor(private injector: Injector,
        private http: HttpClient) { super(injector); }

    // 取得数据
    get(id) {
        const uri = UriConstant.WorkFlowFormUri + `?Id=${id}`;
        return this.SafeRequest(this.http.get(uri));
    }

    // 添加数据
    addOrUpdate(data) {
        console.log(data);
        const uri = UriConstant.WorkFlowFormUri;
        return this.SafeRequest(this.http.put(uri, data));
    }
}