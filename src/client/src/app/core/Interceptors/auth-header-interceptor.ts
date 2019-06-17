import { Injectable } from "@angular/core";
import { HttpInterceptor, HttpRequest, HttpHandler } from '@angular/common/http';
import { CommonConstant } from '../constants/common-constant';
import { LocalStorageService } from '../services/local-storage.service';


// 设置授权header
@Injectable()
export class AuthHeaderInterceptor implements HttpInterceptor {

    constructor(private _localStorageService: LocalStorageService) { }

    // 参照官方例子，在请求中添加认证header
    intercept(req: HttpRequest<any>, next: HttpHandler) {
        const token = 'Bearer ' + this._localStorageService.getToken();
        const authReq = req.clone({ setHeaders: { 'Authorization': token } });
        return next.handle(authReq);
    }

}