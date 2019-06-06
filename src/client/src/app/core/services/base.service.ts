import { HttpHeaders, HttpErrorResponse, HttpClient } from '@angular/common/http';
import { CommonConstant } from '../constants/common-constant';
import { Injector } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd';
import { HttpCodeConstant } from '../constants/httpcode-constant';
import { observable, Observable } from 'rxjs';
import { map, mergeMap } from 'rxjs/operators';
import { UriConstant } from '../constants/uri-constant';

export abstract class BaseService {

  private _messageService: NzMessageService;

  private _httpClient: HttpClient;

  // 可以从injector内获取任意对象
  constructor(private baseInjector: Injector) {
    this._messageService = this.baseInjector.get(NzMessageService);
    this._httpClient = this.baseInjector.get(HttpClient);
  }

  public SafeRequest(operate: Observable<any>) {
    return this._httpClient.post(UriConstant.RefreshTokenUri, null).pipe(
      map(token => {
        return token;
      }),
      mergeMap(_ => {
        return operate;
      })
    );
  }

  // 错误处理
  public HandleError(error: HttpErrorResponse) {
    let msg = '';
    if (error.status == HttpCodeConstant.Code401) {
      msg = '没有操作权限！';
    } else if (error.status == HttpCodeConstant.Code404) {
      msg = '没有找到资源！';
    } else if (error.status == HttpCodeConstant.Code500) {
      msg = '服务器内部发生错误！';
    } else if (error.status == HttpCodeConstant.Code400) {
      for (var i in error.error) {
        msg += error.error[i];
      }
    } else {
      msg += '发生未知错误！';
    }
    this._messageService.error(msg);
  }


}
