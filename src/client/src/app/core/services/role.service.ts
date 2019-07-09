import { Injectable, Injector } from '@angular/core';
import { BaseService } from './base.service';
import { HttpClient } from '@angular/common/http';
import { UriConstant } from '../constants/uri-constant';

@Injectable({
  providedIn: 'root'
})
export class RoleService extends BaseService {

  constructor(private injector: Injector,
    private http: HttpClient) { super(injector); }

  // 取得角色概要数据
  getRoleOutlines(pageindex, pageSize, searchText) {
    let uri = `${UriConstant.RoleOutlineUri}?pageIndex=${pageindex}&pageSize=${pageSize}&searchText=${searchText}`;
    return this.SafeRequest(this.http.get(uri));
  }

  // 取得角色详细数据
  getRoleDetail(roleId) {
    let uri = `${UriConstant.RoleUri}?id=${roleId}`;
    return this.SafeRequest(this.http.get(uri));
  }

  // 添加角色信息
  addRoleDetail(roleDetail) {
    let uri = UriConstant.RoleUri;
    return this.SafeRequest(this.http.post(uri, roleDetail));
  }

  // 更新角色信息
  updateRoleDetail(roleDetail) {
    let uri = UriConstant.RoleUri;
    return this.SafeRequest(this.http.put(uri, roleDetail));
  }

  // 删除用户数据
  deleteRole(roleId) {
    let uri = UriConstant.RoleUri + `?id=${roleId}`;
    return this.SafeRequest(this.http.delete(uri));
  }

}
