import { Injectable, Injector } from '@angular/core';
import { BaseService } from '../base.service';
import { HttpClient } from '@angular/common/http';
import { UriConstant } from '../../constants/uri-constant';
import { UserDetail } from 'src/app/right-manage/models/user-detail';

@Injectable({
  providedIn: 'root'
})
export class UserService extends BaseService {

  constructor(private injector: Injector,
    private http: HttpClient) { super(injector); }


  // 取得用户概要数据
  getUserOutlines(pageindex, pageSize, searchText) {
    let uri = `${UriConstant.UserOutlineUri}?pageIndex=${pageindex}&pageSize=${pageSize}&searchText=${searchText}`;
    return this.SafeRequest(this.http.get(uri));
  }

  // 通过id取得用户概要数据
  getUserOutlinesById(ids) {
    let uri = `${UriConstant.AssignOutlineUri}?ids=${ids}`;
    return this.SafeRequest(this.http.get(uri));
  }

  // 通过岗位信息取得用户
  getUserOutlinesByPosition(userName, positionId, positionGroup) {
    let uri = `${UriConstant.PositionOutlineUri}?userName=${userName}&positionId=${positionId}&positionGroup=${positionGroup}`;
    return this.SafeRequest(this.http.get(uri));
  }

  // 取得用户详细数据
  getUserDetail(userId) {
    let uri = `${UriConstant.UserDetailUri}?id=${userId}`;
    return this.SafeRequest(this.http.get<UserDetail>(uri));
  }

  // 更新用户信息
  updateUserDetail(userDetail) {
    let uri = UriConstant.UserDetailUri;
    return this.SafeRequest(this.http.put(uri, userDetail));
  }

  // 添加用户
  addUser(userDetail) {
    let uri = UriConstant.UserDetailUri;
    return this.SafeRequest(this.http.post(uri, userDetail));
  }

  // 删除用户数据
  deleteUser(userId) {
    let uri = `${UriConstant.UserDetailUri}?id=${userId}`;
    return this.SafeRequest(this.http.delete(uri));
  }

  // 取得当前用户信息
  getUserInfo() {
    let uri = UriConstant.UserInfoUri;
    return this.SafeRequest(this.http.get(uri));
  }
}
