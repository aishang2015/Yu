import { Component, OnInit, ViewChild } from '@angular/core';
import { TestData } from '../models/testdata';
import { NzModalRef, NzModalService, NzMessageService } from 'ng-zorro-antd';
import { TestDataService } from 'src/app/core/services/testdata.service';

@Component({
  selector: 'app-test-data',
  templateUrl: './test-data.component.html',
  styleUrls: ['./test-data.component.scss']
})
export class TestDataComponent implements OnInit {

  // 搜索关键字
  searchText = '';

  // 表格数据
  listOfData: TestData[] = [];

  // 分页数据
  pageIndex = 1;
  pageSize = 20;
  total = 0;

  // 编辑中数据
  editedData: TestData = new TestData();

  // 编辑用的模板
  @ViewChild('editTpl')
  editTpl;

  // 模态框
  editModal: NzModalRef;

  constructor(private _modalService: NzModalService,
    private _testDataService: TestDataService,
    private _messageService: NzMessageService) { }

  ngOnInit() {
    this.initData();
  }

  // 搜索数据
  searchData() {
    this.initData();
  }

  // 页面大小发生变化
  pageSizeChange($event) {
    this.pageSize = $event;
    this.pageIndex = 1;
    this.initData();
  }

  // 页码发生变化
  pageIndexChange() {
    this.initData();
  }

  // 添加数据
  addData() {
    this.editedData = new TestData();
    this.editModal = this._modalService.create({
      nzTitle: null,
      nzContent: this.editTpl,
      nzFooter: null,
      nzMaskClosable: false,
      nzClosable: false
    });
  }

  // 编辑数据
  editData(data) {
    Object.assign(this.editedData, data);
    this.editModal = this._modalService.create({
      nzTitle: null,
      nzContent: this.editTpl,
      nzFooter: null,
      nzMaskClosable: false,
      nzClosable: false
    });
  }

  // 删除数据
  deleteData(data) {
    this._testDataService.deleteTestData(data.id).subscribe(
      result => {
        this.initData();
        this._messageService.success("删除成功");
      }
    );
  }

  // 提交数据
  submit(form) {
    if (this.editedData.id) {

      this._testDataService.updateTestData(this.editedData).subscribe(
        result => {
          this.initData();
          this.editModal.destroy();
          this._messageService.success("修改成功");
        }
      );

    } else {

      this._testDataService.addTestData(this.editedData).subscribe(
        result => {
          this.initData();
          this.editModal.destroy();
          this._messageService.success("添加成功");
        }
      );

    }
  }

  // 取消编辑数据
  cancel(form) {
    form.reset();
    this.editModal.destroy();
  }

  // 初始化数据
  initData() {
    this._testDataService.getTestDatas(this.pageIndex, this.pageSize, this.searchText).subscribe(
      (result: any) => {
        this.listOfData = result.data;
        this.total = result.total;
      }
    )
  }


}
