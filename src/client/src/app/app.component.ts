import { Component } from '@angular/core';
import { CommonConstant } from './core/constants/common-constant';
import { Router, NavigationEnd } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  // 用户是否登录
  isLogin: boolean = localStorage.getItem(CommonConstant.LocalStorage_AuthToken) != null;


  constructor(private _router: Router) {
    this._router.events.forEach((event) => {
      if (event instanceof NavigationEnd) {
        (event.url === '/login' || event.urlAfterRedirects === '/login') ? this.isLogin = false : this.isLogin = true;
      }
    });
  }
}
