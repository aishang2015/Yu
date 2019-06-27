import { Injectable, Injector } from '@angular/core';
import { BaseService } from './base.service';
import { HttpClient } from '@angular/common/http';
import { UriConstant } from '../constants/uri-constant';

@Injectable({
  providedIn: 'root'
})
export class EntityService extends BaseService {

  // 构造函数
  constructor(private injector: Injector,
    private http: HttpClient) { super(injector); }

  // 取得下拉框实体数据
  getDropDownEntityData() {
    let uri = UriConstant.EntitiesUri;
    return this.SafeRequest(this.http.get(uri));
  }


}
