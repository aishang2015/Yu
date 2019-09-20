import { Injectable, Injector } from '@angular/core';
import { BaseService } from '../base.service';
import { UriConstant } from '../../constants/uri-constant';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ElementService extends BaseService {

  constructor(private injector: Injector,
    private http: HttpClient) { super(injector) }


  // 取得所有元素
  getAllElement() {
    let uri = UriConstant.ElementUri;
    return this.SafeRequest(this.http.get(uri));
  }

  // 添加新元素
  addElement(element) {
    let uri = UriConstant.ElementUri;
    return this.SafeRequest(this.http.post(uri, element));
  }

  // 删除元素
  deleteElement(elementId) {
    let uri = UriConstant.ElementUri + `?id=${elementId}`;
    return this.SafeRequest(this.http.delete(uri));
  }

  // 更新元素
  updateElement(element) {
    let uri = UriConstant.ElementUri;
    return this.SafeRequest(this.http.put(uri, element));
  }

}
