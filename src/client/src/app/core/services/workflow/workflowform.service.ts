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
        const uri = UriConstant.WorkFlowFormUri;
        return this.SafeRequest(this.http.put(uri, data));
    }

    // 取得元素数据
    getFormElement(flowId) {
        const uri = UriConstant.WorkFlowFormElementUri + `?Id=${flowId}`;
        return this.SafeRequest(this.http.get(uri));
    }

    // 组件数据
    components = [
        { key: 'input', describe: '输入框' },
        { key: 'textarea', describe: '文本域' },
        { key: 'inputnumber', describe: '数字输入框' },
        { key: 'datepicker', describe: '日期选择' },
        { key: 'timepicker', describe: '时间选择' },
        { key: 'selecter', describe: '选择器' },
        { key: 'upload', describe: '文件上传' },
    ];
}