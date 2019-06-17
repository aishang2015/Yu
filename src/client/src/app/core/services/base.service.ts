import { HttpClient } from '@angular/common/http';
import { Injector } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd';
import { Observable } from 'rxjs';
import { map, mergeMap } from 'rxjs/operators';
import { UriConstant } from '../constants/uri-constant';
import { LocalStorageService } from './local-storage.service';

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

  public SafeRequest(operate: Observable<any>) {

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


}
