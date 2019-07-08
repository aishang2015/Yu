import { Injectable, Injector } from '@angular/core';
import { BaseService } from './base.service';
import { HttpClient } from '@angular/common/http';
import { CommonConstant } from '../constants/common-constant';
import { UriConstant } from '../constants/uri-constant';
import { ApiDetail } from 'src/app/right-manage/models/api-detail';

@Injectable({
  providedIn: 'root'
})

export class ApiService extends BaseService {

  // 构造函数
  constructor(private injector: Injector,
    private http: HttpClient) { super(injector); }

  // 取得api
  getApis(pageIndex, pageSize, searchText) {
    const uri = UriConstant.ApiUri + `?pageIndex=${pageIndex}&pageSize=${pageSize}&searchText=${searchText}`;
    return this.SafeRequestGeneric(this.http.get(uri));
  }

  // 添加api
  addApi(api) {
    const uri = UriConstant.ApiUri;
    return this.SafeRequest(this.http.post(uri, api));
  }

  // 更新api
  updateApi(api) {
    const uri = UriConstant.ApiUri;
    return this.SafeRequest(this.http.put(uri, api));
  }

  // 删除api
  deleteApi(id) {
    const uri = UriConstant.ApiUri + `?id=${id}`;
    return this.SafeRequest(this.http.delete(uri));
  }

  // 取得全部api
  getAllApi(){
    const uri = UriConstant.AllApiUri;
    return this.SafeRequest(this.http.get(uri));
  }
}
