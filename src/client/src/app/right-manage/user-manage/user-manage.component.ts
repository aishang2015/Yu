import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-user-manage',
  templateUrl: './user-manage.component.html',
  styleUrls: ['./user-manage.component.scss']
})
export class UserManageComponent implements OnInit {

  // 分页数据
  pageSize = 20;
  pageIndex = 1;
  total = 100;

  // 表格数据源
  listOfData: any[] = [];

  constructor() { }

  ngOnInit() {

    // 初始化表格数据
    for (let i = 20 * (this.pageIndex - 1); i < 20 * this.pageIndex; i++) {
      this.listOfData.push({
        no: i + 1,
        userName: "admin",
        telephone: 13852145236,
        email: "GAWGAWg@163.com",
        openId: "#$HW%$Y#$134213H%$Y#^YHHEE6th"
      });
    }

  }

  pageIndexChange() {

    // 初始化表格数据
    this.listOfData = [];
    for (let i = 20 * (this.pageIndex - 1); i < 20 * this.pageIndex; i++) {
      this.listOfData.push({
        no: i + 1,
        userName: "admin",
        telephone: 13852145236,
        email: "GAWGAWg@163.com",
        openId: "#$HW%$Y#$134213H%$Y#^YHHEE6th"
      });
    }

  }

}
