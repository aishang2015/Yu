import { Component, OnInit } from '@angular/core';
import { LoginModel } from '../models/login-model';
import { AccountService } from 'src/app/core/services/account.service';
import { CommonConstant } from 'src/app/core/constants/common-constant';
import { NzMessageService } from 'ng-zorro-antd';
import { Router } from '@angular/router';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

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
    private sanitizer: DomSanitizer) { }

  ngOnInit() {
    this.refresh();
  }

  submit(loginForm) {

    // 验证通过
    if (loginForm.valid) {
      this.accountService.login(this.loginModel)
        .subscribe(
          token => {

            // jwt 保存localstorage
            localStorage.setItem(CommonConstant.AuthToken, token['token']);
            this.messageService.success("登录成功！");
            this.router.navigate(["dashboard"]);
          },
          error => {
            this.accountService.HandleError(error); // 通用错误处理
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
