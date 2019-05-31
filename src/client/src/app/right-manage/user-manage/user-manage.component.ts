import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/core/services/user.service';
import { NzMessageService } from 'ng-zorro-antd';

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

  // 搜索关键字
  searchText = '';

  // 表格数据源
  listOfData: any[] = [];

  constructor(private userService: UserService,
    private messageService: NzMessageService) { }

  ngOnInit() {

    this.getUserInfo();
  }


  // 初始化用户数据
  getUserInfo() {
    this.userService.getUserOutlines(this.pageIndex, this.pageSize, this.searchText)
      .subscribe(
        (result: any) => {
          this.total = result.total;
          this.listOfData = result.data;
          this.messageService.success("数据取得完毕。")
        },
        error => {
          this.userService.HandleError(error);
        }
      )
  }


  // 页码发生变化
  pageIndexChange() {
    this.getUserInfo();
  }

  // 搜索数据
  searchData() {
    this.getUserInfo();
  }


}
