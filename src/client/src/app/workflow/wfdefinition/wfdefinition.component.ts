import { Component, OnInit, ViewChild } from '@angular/core';
import { WorkFlowTypeService } from 'src/app/core/services/workflow/workflowtype.service';
import { WorkflowType } from '../models/workflowType';
import { NzModalService, NzMessageService } from 'ng-zorro-antd';
import { WorkFlowDefine } from '../models/workflowDefine';
import { WorkFlowDefineService } from 'src/app/core/services/workflow/workflowdefine.service';

@Component({
  selector: 'app-wfdefinition',
  templateUrl: './wfdefinition.component.html',
  styleUrls: ['./wfdefinition.component.scss']
})
export class WfdefinitionComponent implements OnInit {

  // 类型列表
  wfTypeList: WorkflowType[] = [];
  wfTypeMenuList = [];
  wfTypeListSelectedIndex = null;

  // 编辑用工作流类型
  editedWfType: WorkflowType = new WorkflowType();

  // 工作流列表
  wfDefineList: WorkFlowDefine[] = [];

  // 编辑工作流定义
  editedWfDefine: WorkFlowDefine = new WorkFlowDefine();

  // 工作流列表
  wfList = [];

  // 分页
  pageIndex = 1;
  pageSize = 20;
  total = 0;

  // 模态框对象
  nzModal;

  // 是否提交
  isSubmit: boolean = false;

  // 是否加载
  isLoading: boolean = false;

  @ViewChild('wfTypeTpl', { static: true })
  wfTypeTpl;

  @ViewChild('wfDefineTpl', { static: true })
  wfDefineTpl;

  constructor(private _workflowTypeService: WorkFlowTypeService,
    private _workflowDefineService: WorkFlowDefineService,
    private _modalService: NzModalService,
    private _messageService: NzMessageService) { }

  ngOnInit() {
    this.initWfTypeList();
    this.initWfDefineList();
  }

  // 每页数据数发生变化
  pageSizeChange($event) {
    this.pageIndex = 1;
    this.pageSize = $event;
    this.initWfDefineList();
  }

  // 页码发生变化
  pageIndexChange() {
    this.initWfDefineList();
  }

  //#region 工作流类型

  // 工作流类型选择事件
  typeSelected(index) {
    this.wfTypeListSelectedIndex = index;
    this.initWfDefineList();
  }

  // 所有工作流选中
  allTypeSelected() {
    this.wfTypeListSelectedIndex = null;
    this.initWfDefineList();
  }

  // 添加工作流类型
  addWfType() {
    this.editedWfType = new WorkflowType();
    this.editedWfType.order = this.wfTypeList.length + 1;
    this.nzModal = this._modalService.create({
      nzContent: this.wfTypeTpl,
      nzFooter: null,
      nzTitle: null,
      nzMaskClosable: false,
      nzClosable: false
    });
  }

  // 编辑工作流类型
  editWfType() {
    Object.assign(this.editedWfType, this.wfTypeList[this.wfTypeListSelectedIndex]);
    this.nzModal = this._modalService.create({
      nzContent: this.wfTypeTpl,
      nzFooter: null,
      nzTitle: null,
      nzMaskClosable: false,
      nzClosable: false
    });
  }

  // 删除工作流模型
  removeWfType() {
    this.isLoading = true;
    this._workflowTypeService.delete(this.wfTypeList[this.wfTypeListSelectedIndex].id).subscribe(result => {
      this._messageService.success("删除成功！");
      this.initWfTypeList();
      this.wfTypeListSelectedIndex = null;
      this.isLoading = false;
    },
      error => {
        this.isLoading = false;
      });
  }

  // 保存工作流类型
  submitWfType(form) {
    this.isLoading = true;
    if (form.valid) {
      if (this.editedWfType.id) {
        this._workflowTypeService.update(this.editedWfType).subscribe(result => {
          this._messageService.success("更新成功！");
          this.initWfTypeList();
          this.nzModal.close();
          this.isLoading = false;
        },
          error => {
            this.isLoading = false;
          });
      } else {
        this._workflowTypeService.add(this.editedWfType).subscribe(result => {
          this._messageService.success("添加成功！");
          this.initWfTypeList();
          this.nzModal.close();
          this.isLoading = false;
        },
          error => {
            this.isLoading = false;
          });
      }
    }

  }

  // 初始化工作流类型列表
  initWfTypeList() {
    this._workflowTypeService.get().subscribe(result => {
      this.wfTypeList = [];
      this.wfTypeMenuList = [];
      result.forEach(wftype => {
        this.wfTypeList.push(wftype);
        this.wfTypeMenuList.push(wftype.name);
      });
    })
  }

  //#endregion

  //#region 工作流定义
  initWfDefineList() {
    let id = this.wfTypeListSelectedIndex != null ? this.wfTypeList[this.wfTypeListSelectedIndex].id : '';
    this._workflowDefineService.get(this.pageIndex, this.pageSize, id).subscribe(result => {
      this.wfDefineList = [];
      result.data.forEach(wfDefine => {
        this.wfDefineList.push(wfDefine);
      });
      this.total = result.total;
    })
  }

  // 添加工作流定义
  addWfDefine() {
    this.editedWfDefine = new WorkFlowDefine();
    this.nzModal = this._modalService.create({
      nzContent: this.wfDefineTpl,
      nzFooter: null,
      nzTitle: null,
      nzMaskClosable: false,
      nzClosable: false
    });
  }

  // 编辑工作流定义
  editWfDefine(data) {
    this._workflowDefineService.getbyid(data.id).subscribe(result => {
      this.editedWfDefine = result;
      this.nzModal = this._modalService.create({
        nzContent: this.wfDefineTpl,
        nzFooter: null,
        nzTitle: null,
        nzMaskClosable: false,
        nzClosable: false
      });
    })
  }

  // 删除工作流定义
  removeWfDefine(data) {
    this._modalService.confirm(
      {
        nzTitle: '是否删除此办公流？',
        nzContent: '请确认后再进行删除',
        nzOnOk: () => {
          this.isLoading = true;
          this._workflowDefineService.delete(data.id).subscribe(result => {
            this._messageService.success("删除成功！");
            this.initWfDefineList();
            this.isLoading = false;
          }, error => this.isLoading = false);
        }
      }
    )
  }

  // 提交工作流定义
  submitWfDefine() {
    const callback = () => {
      this._messageService.success("更新成功！");
      this.initWfDefineList();
      this.nzModal.close();
      this.isLoading = false;
    }
    this.isLoading = true;
    if (this.editedWfDefine.id) {
      this._workflowDefineService.update(this.editedWfDefine).subscribe(
        result => callback(),
        error => this.isLoading = false
      );
    } else {
      this._workflowDefineService.add(this.editedWfDefine).subscribe(
        result => callback(),
        error => this.isLoading = false
      );
    }
  }

  //#endregion

  // 取消保存
  cancel(form) {
    this.nzModal.close();
  }

  publish(data) {
    this._workflowDefineService.publish(data.id, !data.isPublish).subscribe(result => data.isPublish = !data.isPublish);
  }
}
