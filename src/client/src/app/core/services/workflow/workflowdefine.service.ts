
import { Injectable, Injector } from '@angular/core';
import { BaseService } from '../base.service';
import { HttpClient } from '@angular/common/http';
import { UriConstant } from '../../constants/uri-constant';

@Injectable({
	providedIn: 'root'
})

export class WorkFlowDefineService extends BaseService {

	// 构造函数
	constructor(private injector: Injector,
		private http: HttpClient) { super(injector); }

	// 取得数据
	getbyid(id) {
		const uri = UriConstant.WorkFlowDefineUri + `?id=${id}`;
		return this.SafeRequest(this.http.get(uri));
	}

	// 取得数据
	get(pageIndex, pageSize, typeId) {
		const uri = UriConstant.WorkFlowDefineUri + `s?pageIndex=${pageIndex}&pageSize=${pageSize}&typeId=${typeId}`;
		return this.SafeRequest(this.http.get(uri));
	}

	// 取得全部数据
	getAll() {
		const uri = UriConstant.AllWorkFlowDefineUri;
		return this.SafeRequest(this.http.get(uri));
	}

	// 添加数据
	add(data) {
		const uri = UriConstant.WorkFlowDefineUri;
		return this.SafeRequest(this.http.post(uri, data));
	}

	// 更新数据
	update(data) {
		const uri = UriConstant.WorkFlowDefineUri;
		return this.SafeRequest(this.http.put(uri, data));
	}

	// 删除数据
	delete(id) {
		const uri = UriConstant.WorkFlowDefineUri + `?id=${id}`;
		return this.SafeRequest(this.http.delete(uri));
	}

	// 设置发布状态
	publish(id, ispublish) {
		const uri = UriConstant.WorkFlowDefineUri;
		return this.SafeRequest(this.http.patch(uri, { id: id, ispublish: ispublish }));
	}
}

