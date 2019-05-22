import { HttpHeaders } from '@angular/common/http';
import { CommonConstant } from '../constants/common-constant';
import { Injector } from '@angular/core';

export abstract class BaseService {

  constructor(private injector: Injector) {
  }

  // 授权header
  protected AuthorizationHeader() {
    return new HttpHeaders({ 'authorization': 'Bearer ' + localStorage[CommonConstant.AuthToken] });
  }

  // 检查token是否过期
  public CheckToken() {

  }


}
