import { Component, OnInit, ViewChild } from '@angular/core';
import { ApiService } from 'src/app/core/services/api.service';
import { ApiDetail } from '../models/api-detail';
import { NzModalRef, NzModalService, NzMessageService } from 'ng-zorro-antd';

@Component({
  selector: 'app-api-manage',
  templateUrl: './api-manage.component.html',
  styleUrls: ['./api-manage.component.scss']
})
export class ApiManageComponent implements OnInit {

  // 搜素文字串
  searchText: string = '';

  // 数据
  listOfData = [];

  // 分页数据
  pageIndex: number = 1;
  pageSize: number = 20;
  total: number = 0;

  // 编辑中的api
  editedApi: ApiDetail = new ApiDetail();

  // 模态对话框
  nzModal: NzModalRef;

  // 是否提交
  isSubmit = false;

  // 模态对话框模板
  @ViewChild('editTpl')
  editTpl;

  constructor(private _apiService: ApiService,
    private _modalService: NzModalService,
    private _messageService: NzMessageService) { }

  ngOnInit() {
    this.initData();
  }

  // 初始化
  initData() {
    this._apiService.getApis(this.pageIndex, this.pageSize, this.searchText).subscribe(
      (result: any) => {
        this.listOfData = result.data;
        this.total = result.total;
      }
    )
  }

  // 搜索
  searchData() {
    this.pageIndex = 1;
    this.initData();
  }

  // 每页数据数发生变化
  pageSizeChange($event) {
    this.pageIndex = 1;
    this.pageSize = $event;
    this.initData();
  }

  // 页码发生变化
  pageIndexChange() {
    this.initData();
  }

  // 添加API
  addApi() {
    this.editedApi = new ApiDetail();
    this.nzModal = this._modalService.create(
      {
        nzTitle: null,
        nzContent: this.editTpl,
        nzFooter: null,
        nzClosable: false,
        nzMaskClosable: false
      }
    )
  }

  // 编辑Api
  editApi(api) {
    Object.assign(this.editedApi, api);
    this.nzModal = this._modalService.create(
      {
        nzTitle: null,
        nzContent: this.editTpl,
        nzFooter: null,
        nzClosable: false,
        nzMaskClosable: false
      }
    );
  }

  // 删除Api
  deleteApi(api) {
    this._modalService.confirm(
      {
        nzTitle: '是否删除此Api数据？',
        nzContent: '删除该数据可能会导致系统错误，请确认后再进行删除',
        nzOnOk: () => {
          this._apiService.deleteApi(api.id).subscribe(result => {
            this._messageService.success("删除成功");
            this.initData();
          })
        }
      }
    )
  }

  // 提交修改
  submit(form) {
    this.isSubmit = true;
    if (form.valid) {
      this.isSubmit = false;
      if (this.editedApi.id) {
        this._apiService.updateApi(this.editedApi).subscribe(result => {
          this._messageService.success("更新成功");
          this.initData();
          this.nzModal.close();
        })
      } else {
        this._apiService.addApi(this.editedApi).subscribe(result => {
          this._messageService.success("创建成功");
          this.initData();
          this.nzModal.close();
        });
      }
    }
  }

  // 取消修改
  cancel(form) {
    this.nzModal.close();
  }

}
