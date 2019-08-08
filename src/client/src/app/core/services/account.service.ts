import { Injectable, Injector } from '@angular/core';
import { BaseService } from './base.service';
import { LoginModel } from 'src/app/account/models/login-model';
import { HttpClient } from '@angular/common/http';
import { UriConstant } from '../constants/uri-constant';
import { CommonConstant } from '../constants/common-constant';

@Injectable({
  providedIn: 'root'
})
export class AccountService extends BaseService {

  // 构造函数
  constructor(private injector: Injector,
    private http: HttpClient) { super(injector); }

  // 用户名密码登陆
  login(loginModel: LoginModel) {
    return this.http.post(UriConstant.LoginUri, loginModel);
  }

  // 取得验证码图片
  getCaptchaImage() {
    return this.http.get(UriConstant.CaptchaUri, { observe: 'response', responseType: 'blob' });
  }

  // 修改当前用户密码
  changePwd(model) {
    let uri = UriConstant.UserChangePwdUri;
    return this.SafeRequest(this.http.post(uri, model));
  }

  // 根据手机修改密码
  changePwdByPhone(phone) {
    let uri = UriConstant.ResetUserPwdByPhone + `?phoneNumber=${phone}`;
    return this.http.get(uri);
  }

  // 根据手机修改密码
  submitPwdByPhone(data) {
    let uri = UriConstant.ResetUserPwdByPhone;
    return this.http.post(uri, data);
  }
}
