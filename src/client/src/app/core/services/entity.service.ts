import { Injectable, Injector } from '@angular/core';
import { BaseService } from './base.service';
import { HttpClient } from '@angular/common/http';
import { UriConstant } from '../constants/uri-constant';
import { CommonConstant } from '../constants/common-constant';

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

  // 取得entity
  getEntity(pageIndex, pageSize, searchText) {
    let uri = UriConstant.EntityUri + `?pageIndex=${pageIndex}&pageSize=${pageSize}&SearchText=${searchText}`;
    return this.SafeRequest(this.http.get(uri));
  }

  // 添加entity
  addEntity(entity) {
    let uri = UriConstant.EntityUri;
    return this.SafeRequest(this.http.post(uri, entity));
  }

  // 更新entity
  updateEntity(entity) {
    let uri = UriConstant.EntityUri;
    return this.SafeRequest(this.http.put(uri, entity));
  }

  // 删除entity
  deleteEntity(id){
    let uri = UriConstant.EntityUri + `?id=${id}`;
    return this.SafeRequest(this.http.delete(uri));
  }


}
