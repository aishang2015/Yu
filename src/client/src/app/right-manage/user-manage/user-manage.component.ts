import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/core/services/user.service';
import { NzMessageService, NzModalService, NzModalRef } from 'ng-zorro-antd';
import { UserDetail } from '../models/user-detail';

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

  // 用户详细数据(浏览)
  userDetail: UserDetail;

  // 用户详细数据(编辑)
  editUserDetail: UserDetail;

  // 性别数据
  genders = [
    { name: "未知", value: 0 },
    { name: "男性", value: 1 },
    { name: "女性", value: 2 }
  ];

  // 编辑模态框
  editModal: NzModalRef;

  constructor(private userService: UserService,
    private messageService: NzMessageService,
    private modalService: NzModalService) { }

  ngOnInit() {
    this.getUserInfo();
  }

  // 初始化用户数据
  getUserInfo() {
    this.userService.getUserOutlines(this.pageIndex, this.pageSize, this.searchText)
      .subscribe(
        (result: any) => {

          // 设置数据总数
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

  // 查看用户详细数据
  viewUserDetail(userOutline, tplContent) {

    // 取得当前用户数据
    this.userService.getUserDetail(userOutline.id)
      .subscribe(
        userDetail => {
          this.userDetail = userDetail;
          this.modalService.create({
            nzContent: tplContent, // 模板
            nzFooter: null,
            nzClosable: false
          });
        },
        error => {
          this.userService.HandleError(error);
        }
      );
  }


  // 编辑用户数据
  editUser(userOutline, tplContent) {

    // 取得当前用户数据
    this.userService.getUserDetail(userOutline.id)
      .subscribe(
        userDetail => {
          this.editUserDetail = userDetail;
          this.editModal = this.modalService.create({
            nzContent: tplContent, // 模板
            nzFooter: null,
            nzClosable: false,
            nzMaskClosable: false
          });
        },
        error => {
          this.userService.HandleError(error);
        }
      );
  }

  // 取消编辑
  cancelEdit() {
    this.editModal.destroy();
  }

  // 提交用户数据
  editSubmit(form) {

    // 数据合法
    if (form.valid) {
      this.userService.updateUserDetail(this.editUserDetail)
        .subscribe(
          result => {
            this.messageService.success("修改成功！");
            this.editModal.destroy();
            this.getUserInfo();
          },
          error => this.userService.HandleError(error)
        )
    }
  }

}
