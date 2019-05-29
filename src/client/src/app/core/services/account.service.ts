import { Injectable, Injector } from '@angular/core';
import { BaseService } from './base.service';
import { LoginModel } from 'src/app/account/models/login-model';
import { HttpClient } from '@angular/common/http';
import { UriConstant } from '../constants/uri-constant';

@Injectable({
  providedIn: 'root'
})
export class AccountService extends BaseService {

  // 构造函数
  constructor(private injector: Injector,
    private http: HttpClient) { super(injector); }

  // 用户名密码登陆
  login(loginModel: LoginModel) {
    return this.http.post(UriConstant.LoginUri, loginModel, { responseType: 'text' });
  }
}
