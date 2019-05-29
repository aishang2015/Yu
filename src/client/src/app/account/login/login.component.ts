import { Component, OnInit } from '@angular/core';
import { LoginModel } from '../models/login-model';
import { AccountService } from 'src/app/core/services/account.service';
import { CommonConstant } from 'src/app/core/constants/common-constant';
import { NzMessageService } from 'ng-zorro-antd';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  imagesrc: string = "https://localhost:44334/captcha";

  loginModel: LoginModel = new LoginModel();

  constructor(private accountService: AccountService,
    private messageService: NzMessageService,
    private router: Router) { }

  ngOnInit() { }

  submit(loginForm) {

    // 验证通过
    if (loginForm.valid) {
      this.accountService.login(this.loginModel)
        .subscribe(
          token => {
            
            // jwt 保存localstorage
            localStorage.setItem(CommonConstant.AuthToken, token);
            this.messageService.success("登录成功！");
            this.router.navigate(["/dashboard"]);
          },
          error => {
            this.accountService.HandleError(error); // 通用错误处理
            this.imagesrc = "https://localhost:44334/captcha?" + Math.random(); // 刷新验证码
          }
        )
    }
  }

  // 点击刷新验证码
  refresh() {
    this.imagesrc = "https://localhost:44334/captcha?" + Math.random();
  }

}
