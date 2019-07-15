import { Injectable } from '@angular/core';
import { CommonConstant } from '../constants/common-constant';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {

  // 用户token
  setToken(token: string) {
    localStorage.setItem(CommonConstant.LocalStorage_AuthToken, token);
  }
  getToken(): string { return localStorage.getItem(CommonConstant.LocalStorage_AuthToken); }

  // token过期时间
  setExpires(expires: string) {
    localStorage.setItem(CommonConstant.LocalStorage_Expires, expires);
  }
  getExpires(): string { return localStorage.getItem(CommonConstant.LocalStorage_Expires); }

  // 用户名
  setUserName(userName: string) {
    localStorage.setItem(CommonConstant.LocalStorage_UserName, userName);
  }
  getUserName(): string { return localStorage.getItem(CommonConstant.LocalStorage_UserName); }

  // 用户头像地址
  setAvatarUrl(avatarUrl: string) {
    localStorage.setItem(CommonConstant.LocalStorage_AvatarUrl, avatarUrl);
  }
  getAvatarUrl(): string { return localStorage.getItem(CommonConstant.LocalStorage_AvatarUrl); }

  // 可操作的页面元素识别
  setIdentifycations(identifycations: string[]) {
    localStorage.setItem(CommonConstant.LocalStorage_Identifycation, identifycations.join(','));
  }
  getIdentifycations(): string[] {
    if (localStorage.getItem(CommonConstant.LocalStorage_Identifycation))
      return localStorage.getItem(CommonConstant.LocalStorage_Identifycation).split(',');
    return [];
  }

  // 可访问的路由
  setRoutes(routes: string[]) {
    localStorage.setItem(CommonConstant.LocalStorage_Routes, routes.join(','));
  }
  getRoutes(): string[] {
    if (localStorage.getItem(CommonConstant.LocalStorage_Routes))
      return localStorage.getItem(CommonConstant.LocalStorage_Routes).split(',');
    return [];
  }

  // 清除存储
  clear() {
    localStorage.removeItem(CommonConstant.LocalStorage_AuthToken);
    localStorage.removeItem(CommonConstant.LocalStorage_Expires);
    localStorage.removeItem(CommonConstant.LocalStorage_UserName);
    localStorage.removeItem(CommonConstant.LocalStorage_AvatarUrl);
    localStorage.removeItem(CommonConstant.LocalStorage_Identifycation);
    localStorage.removeItem(CommonConstant.LocalStorage_Routes);
  }

}
