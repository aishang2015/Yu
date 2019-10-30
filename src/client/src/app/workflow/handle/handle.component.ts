import { Component, OnInit, ViewChild } from '@angular/core';
import { WorkFlowInstance } from '../models/workflowInstance';
import { WorkFlowInstanceService } from 'src/app/core/services/workflow/workflowinstance.service';
import { WorkFlowDefine } from '../models/workflowDefine';
import { WorkflowType } from '../models/workflowType';
import { WorkFlowDefineService } from 'src/app/core/services/workflow/workflowdefine.service';
import { WorkFlowTypeService } from 'src/app/core/services/workflow/workflowtype.service';
import { WorkFlowInstanceNode } from '../models/WorkFlowInstanceNode';
import { NzModalService, NzMessageService } from 'ng-zorro-antd';
import { WorkflowFlowNodeElement } from '../models/workflowFlowNodeElement';

@Component({
  selector: 'app-handle',
  templateUrl: './handle.component.html',
  styleUrls: ['./handle.component.scss']
})
export class HandleComponent implements OnInit {

  pageIndex = 1;
  total = 0;
  pageSize = 20;

  // 编辑工作流模态框
  @ViewChild('editTpl', { static: true })
  editTpl;

  _nzModal;

  explain;

  // 工作流实例对象
  wfInstanceList: WorkFlowInstance[] = [];

  // 工作流定义
  workflowDefines: WorkFlowDefine[] = [];

  // 工作流类型
  workflowTypes: WorkflowType[] = [];

  // 工作流实例节点处理信息
  workflowInstanceNodes: WorkFlowInstanceNode[] = [];

  // 编辑中的工作流
  editingWorkFlowInstance: WorkFlowInstance = new WorkFlowInstance();


  constructor(
    private _messageService: NzMessageService,
    private _modalService: NzModalService,
    private _workflowInstanceService: WorkFlowInstanceService,
    private _workflowDefineService: WorkFlowDefineService,
    private _workflowTypeService: WorkFlowTypeService) { }

  ngOnInit() {
    this._workflowDefineService.getAll().subscribe(result => {
      this.workflowDefines = result;
    });
    this._workflowTypeService.get().subscribe(result => {
      this.workflowTypes = result;
    });
    this.initJobList();
  }

  // 初始化工作流
  initJobList() {
    this._workflowInstanceService.getHandle(this.pageIndex, this.pageSize).subscribe(result => {
      this.total = result.total;
      this.wfInstanceList = result.data;
    });
  }

  pageSizeChange($event) {
    this.pageSize = $event;
  }

  pageIndexChange() {
  }

  // 取得定义名称
  getDefineName(defineid) {
    return this.workflowDefines.find(wfd => wfd.id == defineid).name;
  }

  // 取得定义类型
  getDefineType(defineid) {
    let typeid = this.workflowDefines.find(wfd => wfd.id == defineid).typeId;
    return this.workflowTypes.find(wft => wft.id == typeid).name;
  }

  //  取得状态定义
  getStatusName(state) {
    return this._workflowInstanceService.instanceStatusMap[state];
  }

  // 取得处理状态
  getHandleStatus(state) {
    return this._workflowInstanceService.handleStatusMap[state];
  }

  // 取消编辑
  cancel() {
    this._nzModal.close();
    this._nzModal = null;
  }

  // 编辑数据
  editData(data) {
    this.workflowInstanceNodes = [];
    this.editingWorkFlowInstance = data;
    this._workflowInstanceService.getInstanceNode(this.editingWorkFlowInstance.id).subscribe(
      result => {
        this.workflowInstanceNodes = result;
      }
    );

    this._nzModal = this._modalService.create({
      nzTitle: null,
      nzContent: this.editTpl,
      nzFooter: null,
      nzMaskClosable: false,
      nzClosable: false,
      nzWidth: 868
    })
  }

  // 提交数据
  submit(flg) {
    this._workflowInstanceService.handle(this.editingWorkFlowInstance.id, flg ? 3 : 2, this.explain).subscribe(result => {
      this._messageService.success('提交成功！');
      this._nzModal.close();
      this._nzModal = null;
      this.initJobList();
    });
  }
}
