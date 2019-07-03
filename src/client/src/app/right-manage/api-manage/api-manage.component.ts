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

  // 提交修改
  submit(form) {
  }

  // 取消修改
  cancel(form) {
    this.nzModal.close();
  }

}
