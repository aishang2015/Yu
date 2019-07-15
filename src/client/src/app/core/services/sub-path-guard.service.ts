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

    // 检查是否有路由权限
    if (this._localStorageService.getToken() == null) {
    }

    return true;
  }


}
