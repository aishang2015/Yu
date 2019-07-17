import { Injectable, Injector } from '@angular/core';
import { BaseService } from './base.service';
import { HttpClient } from '@angular/common/http';
import { CommonConstant } from '../constants/common-constant';
import { UriConstant } from '../constants/uri-constant';
import { ApiDetail } from 'src/app/right-manage/models/api-detail';

@Injectable({
  providedIn: 'root'
})

export class TestDataService extends BaseService {

  // 构造函数
  constructor(private injector: Injector,
    private http: HttpClient) { super(injector); }

  // 取得实验数据
  getTestDatas(pageIndex, pageSize, searchText) {
    const uri = UriConstant.TestDataUri + `?pageIndex=${pageIndex}&pageSize=${pageSize}&searchText=${searchText}`;
    return this.SafeRequestGeneric(this.http.get(uri));
  }

  // 添加实验数据
  addTestData(api) {
    const uri = UriConstant.TestDataUri;
    return this.SafeRequest(this.http.post(uri, api));
  }

  // 更新实验数据
  updateTestData(api) {
    const uri = UriConstant.TestDataUri;
    return this.SafeRequest(this.http.put(uri, api));
  }

  // 删除实验数据
  deleteTestData(id) {
    const uri = UriConstant.TestDataUri + `?id=${id}`;
    return this.SafeRequest(this.http.delete(uri));
  }
}
