import { Injectable, Injector } from '@angular/core';
import { BaseService } from './base.service';
import { HttpClient } from '@angular/common/http';
import { UriConstant } from '../constants/uri-constant';

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
}
