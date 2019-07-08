import { Injectable, Injector } from '@angular/core';
import { BaseService } from './base.service';
import { HttpClient } from '@angular/common/http';
import { UriConstant } from '../constants/uri-constant';

@Injectable({
  providedIn: 'root'
})
export class GroupService extends BaseService {

  constructor(private injector: Injector, private httpClient: HttpClient) { super(injector); }

  // 取得组织信息
  getGroups() {
    let uri = UriConstant.GroupUri;
    return this.SafeRequest(this.httpClient.get(uri));
  }

  // 添加组织信息
  addGroup(groupDetail) {
    let uri = UriConstant.GroupUri;
    return this.SafeRequest(this.httpClient.post(uri, groupDetail));
  }

  // 更新组织信息
  updateGroup(groupDetail) {
    let uri = UriConstant.GroupUri;
    return this.SafeRequest(this.httpClient.put(uri, groupDetail));
  }

  // 删除组织信息
  deleteGroup(groupId) {
    let uri = UriConstant.GroupUri + `?id=${groupId}`;
    return this.SafeRequest(this.httpClient.delete(uri));
  }
}
