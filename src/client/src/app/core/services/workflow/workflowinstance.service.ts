
import { Injectable, Injector } from '@angular/core';
import { BaseService } from '../base.service';
import { HttpClient } from '@angular/common/http';
import { UriConstant } from '../../constants/uri-constant';

@Injectable({
	providedIn: 'root'
})

export class WorkFlowInstanceService extends BaseService {

	// 构造函数
	constructor(private injector: Injector,
		private http: HttpClient) { super(injector); }

	// 取得数据
	get(pageIndex, pageSize) {
		const uri = UriConstant.WorkFlowInstanceUri + `?pageIndex=${pageIndex}&pageSize=${pageSize}`;
		return this.SafeRequest(this.http.get(uri));
	}

	// 取得待办数据
	getHandle(pageIndex, pageSize) {
		const uri = UriConstant.HandleWorkFlowInstanceUri + `?pageIndex=${pageIndex}&pageSize=${pageSize}`;
		return this.SafeRequest(this.http.get(uri));
	}

	// 添加数据
	add(data) {
		const uri = UriConstant.WorkFlowInstanceUri;
		return this.SafeRequest(this.http.post(uri, data));
	}

	// 取得表单数据
	getForm(id) {
		const uri = UriConstant.WorkFlowInstanceFormUri + `?id=${id}`;
		return this.SafeRequest(this.http.get(uri));
	}

	// 更新表单数据
	putForm(data) {
		const uri = UriConstant.WorkFlowInstanceFormUri;
		return this.SafeRequest(this.http.put(uri, data));
	}

	// 更新数据
	update(data) {
		const uri = UriConstant.WorkFlowInstanceUri;
		return this.SafeRequest(this.http.put(uri, data));
	}

	// 删除数据
	delete(id) {
		const uri = UriConstant.WorkFlowInstanceUri + `?id=${id}`;
		return this.SafeRequest(this.http.delete(uri));
	}

	// 逻辑删除
	logicDelete(obj) {
		const uri = UriConstant.DeletedWorkFlowInstanceUri;
		return this.SafeRequest(this.http.put(uri, obj));
	}

	// 逻辑删除恢复
	revertDelete(obj) {
		const uri = UriConstant.DeletedWorkFlowInstanceUri;
		return this.SafeRequest(this.http.patch(uri, obj));
	}

	// 取得逻辑删除数据
	getDeletedInstance(pageIndex, pageSize) {
		const uri = UriConstant.DeletedWorkFlowInstanceUri + `?pageIndex=${pageIndex}&pageSize=${pageSize}`;
		return this.SafeRequest(this.http.get(uri));
	}

	// 取得节点处理数据
	getInstanceNode(id) {
		const uri = UriConstant.WorkFlowInstanceNodeUri + `?id=${id}`;
		return this.SafeRequest(this.http.get(uri));
	}

	// 办理工作流
	handle(instanceId, status, explain) {
		const uri = UriConstant.WorkFlowInstanceNodeUri;
		return this.SafeRequest(this.http.patch(uri, { 'InstanceId': instanceId, 'HandleStatus': status, 'Explain': explain }));
	}

	// 取得工作流节点元素
	getFlowNodeElement(id) {
		const uri = UriConstant.WorkFlowNodeElement + `?id=${id}`;
		return this.SafeRequest(this.http.get(uri));
	}

	instanceStatusMap = {
		'1': '开始',
		'2': '流转中',
		'3': '被退回',
		'4': '结束'
	}

	handleStatusMap = {
		'0': '未处理',
		'1': '待处理',
		'2': '拒绝',
		'3': '同意'
	}
}

