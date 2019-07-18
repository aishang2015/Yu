import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, CanActivate, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { CommonConstant } from '../constants/common-constant';
import { LocalStorageService } from './local-storage.service';

@Injectable({
  providedIn: 'root'
})

// 路由守卫
export class SubPathGuard implements CanActivate {

  constructor(private router: Router,
    private _localStorageService: LocalStorageService) { }

  // 控制是否允许进入路由
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

    // 模块内跳转不会进入login-guard所以在模块内token失效并且刷新token的时间内不能转跳到模块内的其他组件。
    // 如果token已经被清空则直接跳到登录页面
    if (route.routeConfig.path != 'login' && this._localStorageService.getToken() == null) {
      this.router.navigate(['/login']);
      return false;
    }

    // 检查是否有路由权限
    var routes = this._localStorageService.getRoutes();
    if (routes.findIndex(r => r == state.url) < 0) {
      this.router.navigate(['/dashboard']);
      return false;
    }
    return true;
  }


}
