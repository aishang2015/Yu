import { HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { CommonConstant } from '../constants/common-constant';
import { Injector } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd';
import { HttpCodeConstant } from '../constants/httpcode-constant';

export abstract class BaseService {

  protected messageService: NzMessageService;

  // 可以从injector内获取任意对象
  constructor(private baseInjector: Injector) {
    this.messageService = this.baseInjector.get(NzMessageService);
  }

  // 授权header
  protected AuthorizationHeader() {
    return new HttpHeaders({ 'authorization': 'Bearer ' + localStorage[CommonConstant.AuthToken] });
  }

  // 检查token是否过期
  public CheckToken() {

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
    this.messageService.error(msg);
  }


}
