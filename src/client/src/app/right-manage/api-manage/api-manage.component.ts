import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/core/services/api.service';

@Component({
  selector: 'app-api-manage',
  templateUrl: './api-manage.component.html',
  styleUrls: ['./api-manage.component.scss']
})
export class ApiManageComponent implements OnInit {

  searchText: string = '';

  listOfData = [];

  // 分页数据
  pageIndex: number = 1;
  pageSize: number = 20;
  total: number = 0;

  constructor(private _apiService: ApiService) { }

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

}
