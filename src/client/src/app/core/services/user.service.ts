import { Injectable, Injector } from '@angular/core';
import { BaseService } from './base.service';
import { HttpClient } from '@angular/common/http';
import { UriConstant } from '../constants/uri-constant';
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
    return this.http.get(uri, { headers: this.AuthorizationHeader() });
  }

  // 取得用户详细数据
  getUserDetail(userId) {
    let uri = `${UriConstant.UserDetailUri}?userId=${userId}`;
    return this.http.get<UserDetail>(uri, { headers: this.AuthorizationHeader() });
  }

  // 更新用户信息
  updateUserDetail(userDetail) {
    let uri = UriConstant.UserDetailUri;
    return this.http.put(uri, userDetail, { headers: this.AuthorizationHeader() });
  }
}
