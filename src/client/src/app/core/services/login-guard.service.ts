import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, CanActivate, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { CommonConstant } from '../constants/common-constant';
import { LocalStorageService } from './local-storage.service';

@Injectable({
  providedIn: 'root'
})

// 路由守卫
export class LoginGuard implements CanActivate {

  constructor(private router: Router,
    private _localStorageService: LocalStorageService) { }

  // 控制是否允许进入路由
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

    // 已登录情况下禁止跳转到login
    if (route.routeConfig.path == 'login' && this._localStorageService.getToken() != null) {
      this.router.navigate(['/dashboard']);
      return false;
    }

    // 未登录情况下禁止转跳其他页面
    if (route.routeConfig.path != 'login' && this._localStorageService.getToken() == null) {
      this.router.navigate(['/login']);
      return false;
    }

    return true;
  }


}
