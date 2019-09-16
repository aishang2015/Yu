import { Component, OnInit, ViewChild } from '@angular/core';
import { WorkFlowTypeService } from 'src/app/core/services/workflow/workflowtype.service';
import { WorkflowType } from '../models/workflowType';
import { NzModalService, NzMessageService } from 'ng-zorro-antd';

@Component({
  selector: 'app-wfdefinition',
  templateUrl: './wfdefinition.component.html',
  styleUrls: ['./wfdefinition.component.scss']
})
export class WfdefinitionComponent implements OnInit {

  // 类型列表
  wfTypeList = [];
  wfTypeMenuList = [];
  wfTypeListSelectedIndex;

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


  // 编辑用工作流类型
  editedWfType: WorkflowType = new WorkflowType();

  constructor(private _workflowTypeService: WorkFlowTypeService,
    private _modalService: NzModalService,
    private _messageService: NzMessageService) { }

  ngOnInit() {
    this.initWfTypeList();
  }

  // 每页数据数发生变化
  pageSizeChange($event) {
    this.pageIndex = 1;
    this.pageSize = $event;
  }

  // 页码发生变化
  pageIndexChange() {
  }


  // 工作流类型选择事件
  typeSelected(index) {
    this.wfTypeListSelectedIndex = index;
    let id = this.wfTypeList[this.wfTypeListSelectedIndex].id;

    // todo 根据type的id取得wf
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

  // 取消保存
  cancel(form) {
    this.nzModal.close();
  }
}
