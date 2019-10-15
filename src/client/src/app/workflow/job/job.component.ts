import { Component, OnInit, ViewChild } from '@angular/core';
import { NzModalService, NzMessageService } from 'ng-zorro-antd';
import { WorkFlowInstanceService } from 'src/app/core/services/workflow/workflowinstance.service';
import { WorkFlowInstance } from '../models/workflowInstance';
import { WorkFlowDefine } from '../models/workflowDefine';
import { WorkflowType } from '../models/workflowType';
import { WorkFlowTypeService } from 'src/app/core/services/workflow/workflowtype.service';
import { WorkFlowDefineService } from 'src/app/core/services/workflow/workflowdefine.service';

@Component({
  selector: 'app-job',
  templateUrl: './job.component.html',
  styleUrls: ['./job.component.scss']
})
export class JobComponent implements OnInit {

  // 模态框对象
  private _nzModal;

  // 工作流实例对象
  wfInstanceList: WorkFlowInstance[] = [];

  // 工作流定义
  workflowDefines: WorkFlowDefine[] = [];

  // 工作流类型
  workflowTypes: WorkflowType[] = [];

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
    this._workflowInstanceService.add({
      defineId: define.id,
    }).subscribe(result => {
      this._nzModal.close();
      this._nzModal = null;
      this.initJobList();
    });
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

  editData(data) {
    this.editingWorkFlowInstance = data;
    this._nzModal = this._modalService.create({
      nzTitle: null,
      nzContent: this.editTpl,
      nzFooter: null,
      nzMaskClosable: false,
      nzClosable: false,
      nzWidth: 868
    })
  }

  // 确认修改
  confirmEdit(edit) {
    console.log(edit);
  }

}
