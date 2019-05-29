import { Component } from '@angular/core';
import { CommonConstant } from './core/constants/common-constant';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  // 用户是否登录
  isLogin(): boolean { return localStorage.getItem(CommonConstant.AuthToken) != null }

}
