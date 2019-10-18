import { Component, OnInit } from '@angular/core';
import { WorkFlowDefine } from '../models/workflowDefine';
import { WorkflowType } from '../models/workflowType';
import { NzModalService, NzMessageService } from 'ng-zorro-antd';
import { WorkFlowTypeService } from 'src/app/core/services/workflow/workflowtype.service';
import { WorkFlowDefineService } from 'src/app/core/services/workflow/workflowdefine.service';
import { WorkFlowInstanceService } from 'src/app/core/services/workflow/workflowinstance.service';
import { WorkFlowInstance } from '../models/workflowInstance';

@Component({
  selector: 'app-recyclebin',
  templateUrl: './recyclebin.component.html',
  styleUrls: ['./recyclebin.component.scss']
})
export class RecyclebinComponent implements OnInit {

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
      this.initJobList();
    });
  }

  // 初始化工作流
  initJobList() {
    this._workflowInstanceService.getDeletedInstance(this.pageIndex, this.pageSize).subscribe(result => {
      this.total = result.total;
      this.wfInstanceList = result.data;
    });
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

  // 恢复数据
  revertData(data) {
    this._workflowInstanceService.revertDelete({ id: data.id })
      .subscribe(result => { this._messageService.success("已经恢复数据！"); this.initJobList(); });
  }

  delete(data) {
    this._modalService.confirm({
      nzTitle: '删除确认',
      nzContent: '是否删除？',
      nzOkText: '确定',
      nzCancelText: '取消',
      nzOnOk: result => {
        this._workflowInstanceService.delete(data.id)
          .subscribe(result => {
            this._messageService.success("删除成功");
            this.initJobList();
          });

      }
    });
  }

}
