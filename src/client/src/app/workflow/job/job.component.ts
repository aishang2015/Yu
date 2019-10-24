import { Component, OnInit, ViewChild } from '@angular/core';
import { NzModalService, NzMessageService, NzModalRef } from 'ng-zorro-antd';
import { WorkFlowInstanceService } from 'src/app/core/services/workflow/workflowinstance.service';
import { WorkFlowInstance } from '../models/workflowInstance';
import { WorkFlowDefine } from '../models/workflowDefine';
import { WorkflowType } from '../models/workflowType';
import { WorkFlowTypeService } from 'src/app/core/services/workflow/workflowtype.service';
import { WorkFlowDefineService } from 'src/app/core/services/workflow/workflowdefine.service';
import { WorkFlowFlowService } from 'src/app/core/services/workflow/workflowflow.service';
import { WorkFlowInstanceNode } from '../models/WorkFlowInstanceNode';

@Component({
  selector: 'app-job',
  templateUrl: './job.component.html',
  styleUrls: ['./job.component.scss']
})
export class JobComponent implements OnInit {

  // 模态框对象
  private _nzModal: NzModalRef;

  // 工作流实例对象
  wfInstanceList: WorkFlowInstance[] = [];

  // 工作流定义
  workflowDefines: WorkFlowDefine[] = [];

  // 工作流类型
  workflowTypes: WorkflowType[] = [];

  // 工作流实例节点处理信息
  workflowInstanceNodes: WorkFlowInstanceNode[] = [];

  // 分页
  pageIndex = 1;
  pageSize = 20;
  total = 0;

  // 新建工作流模态框
  @ViewChild('createTpl', { static: true })
  createTpl;

  // 编辑工作流模态框
  @ViewChild('editTpl', { static: true })
  editTpl;

  // 编辑中的工作流
  editingWorkFlowInstance: WorkFlowInstance = new WorkFlowInstance();

  constructor(private _modalService: NzModalService,
    private _messageService: NzMessageService,
    private _workFlowFlowService: WorkFlowFlowService,
    private _workflowTypeService: WorkFlowTypeService,
    private _workflowDefineService: WorkFlowDefineService,
    private _workflowInstanceService: WorkFlowInstanceService) { }

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
    this._workflowInstanceService.get(this.pageIndex, this.pageSize).subscribe(result => {
      this.total = result.total;
      this.wfInstanceList = result.data;
    });
  }

  // 创建工作流
  addJob() {
    this._nzModal = this._modalService.create({
      nzTitle: null,
      nzContent: this.createTpl,
      nzFooter: null,
      nzMaskClosable: false,
      nzClosable: false
    });
  }

  // 确认创建
  comfirmAdd(define) {
    if (define.id) {
      this._workflowInstanceService.add({
        defineId: define.id,
      }).subscribe(result => {
        this._nzModal.close();
        this._nzModal = null;
        this.initJobList();
      });
    } else {
      this._messageService.warning('请选择一个工作流类型！');
    }
  }

  // 取消编辑
  cancel() {
    this._nzModal.close();
    this._nzModal = null;
  }

  // 页面大小发生变化
  pageSizeChange($event) {
    this.pageSize = $event;
    this.initJobList();
  }

  // 页面下标发生变化
  pageIndexChange() {
    this.initJobList();
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

  // 删除数据
  deleteData(data) {
    this._modalService.confirm(
      {
        nzTitle: '删除确认',
        nzContent: '是否删除？',
        nzOkText: '确定',
        nzCancelText: '取消',
        nzOnOk: result => {
          this._workflowInstanceService.logicDelete({ id: data.id })
            .subscribe(result => { this._messageService.success("已经移入回收站"); this.initJobList(); });

        }
      }
    );
  }

  // 确认修改
  confirmEdit(edit) {
    edit.saveContent().subscribe(result => {
      this._messageService.success('修改成功！');
      this._nzModal.close();
      this._nzModal = null;
    });
  }

  // 提交数据
  submit(edit) {
    edit.saveContent().subscribe(result => {
      this._nzModal.close();
      this._nzModal = null;
    });
  }

  // 打印数据
  print() {
    window.document.body.innerHTML = window.document.getElementsByClassName("ant-modal")[0].innerHTML;
    window.print();
    window.location.reload();
  }

}
