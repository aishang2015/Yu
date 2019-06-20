import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/core/services/user.service';
import { NzMessageService, NzModalService, NzModalRef, UploadFile } from 'ng-zorro-antd';
import { UserDetail } from '../models/user-detail';
import { Observable, Observer } from 'rxjs';
import { CommonConstant } from 'src/app/core/constants/common-constant';
import { UriConstant } from 'src/app/core/constants/uri-constant';

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

  // 头像地址
  avatarUrl: string;

  // 上传头像地址
  uploadUrl: string;

  // 上传按钮状态
  loading: boolean = false;

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
        }
      )
  }

  // 页码发生变化
  pageIndexChange() {
    this.getUserInfo();
  }

  // 每页条数发生变化
  pageSizeChange($event) {
    this.pageSize = $event;
    this.getUserInfo();
  }

  // 搜索数据
  searchData() {
    this.pageIndex = 1;
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
          }
        )
    }
  }

  // 删除用户
  deleteUser(userOutline) {
    this.modalService.confirm({
      nzTitle: '是否要删除该用户？',
      nzOnOk: _ => {
        this.userService.deleteUser(userOutline.id)
          .subscribe(
            result => {
              this.messageService.success("删除成功！");
              this.getUserInfo();
            }
          )
      }
    })
  }

  // 编辑用户头像
  editAvatar(userOutline, tplContent) {
    this.uploadUrl = UriConstant.UserAvatarUri + `?userId=${userOutline.id}`
    this.editModal = this.modalService.create({
      nzContent: tplContent, // 模板
      nzFooter: null,
      nzClosable: false,
      nzMaskClosable: false,
    });

  }

  // 关闭编辑头像对话框
  closeModal() {
    this.getUserInfo();
    this.editModal.destroy();
  }

  // 图片上传校验
  beforeUpload = (file: File) => {
    return new Observable((observer: Observer<boolean>) => {
      const isJPG = file.type === 'image/jpeg';
      if (!isJPG) {
        this.messageService.error('你只能上传JPG格式的文件!');
        observer.complete();
        return;
      }
      const isLt2M = file.size / 1024 / 1024 < 2;
      if (!isLt2M) {
        this.messageService.error('图像必须小于2MB!');
        observer.complete();
        return;
      }
      // check height
      this.checkImageDimension(file).then(dimensionRes => {
        if (!dimensionRes) {
          this.messageService.error('图像必须高宽一致!');
          observer.complete();
          return;
        }

        observer.next(isJPG && isLt2M && dimensionRes);
        observer.complete();
      });
    });
  };

  // 校验图片大小
  private checkImageDimension(file: File): Promise<boolean> {
    return new Promise(resolve => {
      const img = new Image(); // create image
      img.src = window.URL.createObjectURL(file);
      img.onload = () => {
        const width = img.naturalWidth;
        const height = img.naturalHeight;
        window.URL.revokeObjectURL(img.src!);
        resolve(width === height);
      };
    });
  }

  // 文件转化为base64
  private getBase64(img: File, callback: (img: string) => void): void {
    const reader = new FileReader();
    reader.addEventListener('load', () => callback(reader.result!.toString()));
    reader.readAsDataURL(img);
  }


  // 控制上传图标变化
  handleChange(info: { file: UploadFile }): void {
    switch (info.file.status) {
      case 'uploading':
        this.loading = true;
        break;
      case 'done':
        this.getBase64(info.file!.originFileObj!, (img: string) => {
          this.loading = false;
          this.avatarUrl = img;
        });
        break;
      case 'error':
        this.messageService.error('网络错误!');
        this.loading = false;
        break;
    }
  }



}
