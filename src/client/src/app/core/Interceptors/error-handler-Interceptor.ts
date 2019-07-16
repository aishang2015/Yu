import { HttpInterceptor, HttpHandler, HttpRequest, HttpEvent } from "@angular/common/http";
import { Observable } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { HttpCodeConstant } from '../constants/httpcode-constant';
import { CommonConstant } from '../constants/common-constant';
import { NzMessageService } from 'ng-zorro-antd';
import { Injectable } from '@angular/core';
import { LocalStorageService } from '../services/local-storage.service';

@Injectable()
export class ErrorHandlerInterceptor implements HttpInterceptor {

    constructor(private _messageService: NzMessageService,
        private _localStorageService: LocalStorageService) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(
            tap(event => { }, error => {
                let msg = '';
                if (error.status == HttpCodeConstant.Code401) {
                    msg = '您没有操作权限！请联系管理员。';
                } else if (error.status == HttpCodeConstant.Code403) {
                    msg = '您没有该操作的权限！请联系管理员。';
                } else if (error.status == HttpCodeConstant.Code404) {
                    msg = '没有找到资源！';
                } else if (error.status == HttpCodeConstant.Code500) {
                    msg = '服务器内部发生错误！';
                } else if (error.status == HttpCodeConstant.Code400) {
                    for (var key in error.error) {
                        if (key == 'Token') {
                            location.reload();
                            this._localStorageService.clear();
                            this._messageService.error('登陆信息过期，请重新登陆');
                            return;
                        }
                        msg += error.error[key];
                    }
                } else {
                    msg += '发生未知错误！';
                }
                this._messageService.error(msg);
            })
        );
    }

}