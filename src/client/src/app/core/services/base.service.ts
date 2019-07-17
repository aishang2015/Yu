import { HttpClient } from '@angular/common/http';
import { Injector } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd';
import { Observable } from 'rxjs';
import { map, mergeMap } from 'rxjs/operators';
import { UriConstant } from '../constants/uri-constant';
import { LocalStorageService } from './local-storage.service';
import { JwtHelperService } from '@auth0/angular-jwt';

export abstract class BaseService {

  private _messageService: NzMessageService;

  private _httpClient: HttpClient;

  private _localStorageService: LocalStorageService;

  // 可以从injector内获取任意对象
  constructor(private baseInjector: Injector) {
    this._messageService = this.baseInjector.get(NzMessageService);
    this._httpClient = this.baseInjector.get(HttpClient);
    this._localStorageService = this.baseInjector.get(LocalStorageService);
  }

  protected SafeRequest(operate: Observable<any>): Observable<any> {

    // 取得token过期时间
    const expire = parseInt(this._localStorageService.getExpires());

    // 过期情况下，刷新token再执行操作
    if (expire < Date.now() / 1000) {
      return this._httpClient.post(UriConstant.RefreshTokenUri, null).pipe(
        map(result => {

          // 保存token
          this._localStorageService.setToken(result['token']);

          // 解析jwt
          const jwtTokenHelper = new JwtHelperService();
          let decodeToken = jwtTokenHelper.decodeToken(result['token']);

          // 保存过期时间
          this._localStorageService.setExpires(decodeToken['exp']);

          // 保存头像，用户名，元素，路由
          this._localStorageService.setUserName(result['userName']);
          this._localStorageService.setAvatarUrl(result['avatarUrl']);
          this._localStorageService.setIdentifycations(result['identifycations']);
          this._localStorageService.setRoutes(result['routes']);

          return result;
        }),
        mergeMap(_ => {
          return operate;
        })
      );
    } else {

      // 未过期情况下直接返回对象
      return operate;
    }
  }

  protected SafeRequestGeneric<T>(operate: Observable<T>): Observable<T> {

    // 取得token过期时间
    const expire = parseInt(this._localStorageService.getExpires());

    // 过期情况下，刷新token再执行操作
    if (expire < Date.now() / 1000) {
      return this._httpClient.post(UriConstant.RefreshTokenUri, null).pipe(
        map(token => {
          return token;
        }),
        mergeMap(_ => {
          return operate;
        })
      );
    } else {

      // 未过期情况下直接返回对象
      return operate;
    }
  }

  // 刷新token
  // 某些情况下，例如编辑完菜单后通过刷新token来获取新的权限
  public RefreshToken() {
    this._httpClient.post(UriConstant.RefreshTokenUri, null).toPromise().then(
      result => {

        // 保存token
        this._localStorageService.setToken(result['token']);

        // 解析jwt
        const jwtTokenHelper = new JwtHelperService();
        let decodeToken = jwtTokenHelper.decodeToken(result['token']);

        // 保存过期时间
        this._localStorageService.setExpires(decodeToken['exp']);

        // 保存头像，用户名，元素，路由
        this._localStorageService.setUserName(result['userName']);
        this._localStorageService.setAvatarUrl(result['avatarUrl']);
        this._localStorageService.setIdentifycations(result['identifycations']);
        this._localStorageService.setRoutes(result['routes']);
      });
  }

}
