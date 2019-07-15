import { Component, OnInit } from '@angular/core';
import { LoginModel } from '../models/login-model';
import { AccountService } from 'src/app/core/services/account.service';
import { CommonConstant } from 'src/app/core/constants/common-constant';
import { NzMessageService } from 'ng-zorro-antd';
import { Router } from '@angular/router';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { JwtHelperService } from "@auth0/angular-jwt"
import { LocalStorageService } from 'src/app/core/services/local-storage.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  // 验证码图片地址
  imagesrc: SafeUrl = "";

  // 编辑模型
  loginModel: LoginModel = new LoginModel();

  constructor(private accountService: AccountService,
    private messageService: NzMessageService,
    private router: Router,
    private localStorageService: LocalStorageService,
    private sanitizer: DomSanitizer) { }

  ngOnInit() {
    this.refresh();
  }

  submit(loginForm) {

    // 验证通过
    if (loginForm.valid) {
      this.accountService.login(this.loginModel)
        .subscribe(
          result => {

            // jwt 保存localstorage
            this.messageService.success("登录成功！");
            location.reload();

            // 保存token
            this.localStorageService.setToken(result['token']);

            // 解析jwt
            const jwtTokenHelper = new JwtHelperService();
            let decodeToken = jwtTokenHelper.decodeToken(result['token']);

            // 保存JwtToken过期时间
            this.localStorageService.setExpires(decodeToken['exp']);

            // 保存头像和用户名
            this.localStorageService.setUserName(result['userName']);
            this.localStorageService.setAvatarUrl(result['avatarUrl']);
            this.localStorageService.setIdentifycations(result['identifycations']);
            this.localStorageService.setRoutes(result['routes']);
          },
          error => {
            this.refresh(); // 刷新验证码
          }
        )
    }
  }

  // 点击刷新验证码
  refresh() {
    let that = this;
    this.accountService.getCaptchaImage()
      .subscribe(
        response => {
          // 保存codeid
          let codeId = response.headers.get('CaptchaCodeId');
          that.loginModel.captchaCodeId = codeId;

          let image = response.body;  // 获取blob
          let str = URL.createObjectURL(image);
          let url = this.sanitizer.bypassSecurityTrustUrl(str); // url安全转换
          that.imagesrc = url;
        }
      )
  }

}
