import { Injectable } from "@angular/core";
import { HttpInterceptor, HttpRequest, HttpHandler } from '@angular/common/http';
import { CommonConstant } from '../constants/common-constant';


// 设置授权header
@Injectable()
export class AuthHeaderInterceptor implements HttpInterceptor {

    // 参照官方例子，在请求中添加认证header
    intercept(req: HttpRequest<any>, next: HttpHandler) {
        const token = 'Bearer ' + localStorage[CommonConstant.AuthToken];
        const authReq = req.clone({ setHeaders: { 'Authorization': token } });
        return next.handle(authReq);
    }

}