import { Component, OnInit } from '@angular/core';
import { UriConstant } from 'src/app/core/constants/uri-constant';
import { Observable, Observer } from 'rxjs';
import { NzMessageService, UploadFile } from 'ng-zorro-antd';
import { UserService } from 'src/app/core/services/rightmanage/user.service';
import { ImageUriPipe } from 'src/app/core/pipes/image-uri.pipe';
import { LocalStorageService } from 'src/app/core/services/local-storage.service';
import { ChangePwd } from '../models/change-pwd';
import { AccountService } from 'src/app/core/services/account.service';
import { UserDetail } from 'src/app/right-manage/models/user-detail';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.scss']
})
export class UserInfoComponent implements OnInit {

  // 头像地址
  avatarUrl: string;

  // 上传头像地址
  uploadUrl: string = UriConstant.UserAvatarUri;

  // 上传按钮状态
  loading: boolean = false;

  // 用户数据
  userInfo:UserDetail = new UserDetail();

  // 是否提交
  isSubmit = false;

  isLoading = false;

  // 密码修改模型
  changePwdModel: ChangePwd = new ChangePwd();

  constructor(private _messageService: NzMessageService,
    private _userService: UserService,
    private _accountService: AccountService,
    private _localStorageService: LocalStorageService) { }

  ngOnInit() {
    this.initUserData();
  }

  // 初始化数据
  initUserData() {
    this._userService.getUserInfo().subscribe(
      (result: any) => {
        this.userInfo = result;
        this.avatarUrl = new ImageUriPipe().transform(result.avatar);
      }
    );
  }

  // 修改密码
  changePwd(form) {
    if (form.valid) {
      this._accountService.changePwd(this.changePwdModel).subscribe(
        result => {
          this._messageService.success("密码修改成功！请重新登录。");
          this._localStorageService.clear();
          location.reload();
        }
      );
    }
  }

  // 图片上传校验
  beforeUpload = (file: File) => {
    return new Observable((observer: Observer<boolean>) => {
      const isJPG = file.type === 'image/jpeg';
      if (!isJPG) {
        this._messageService.error('你只能上传JPG格式的文件!');
        observer.complete();
        return;
      }
      const isLt2M = file.size / 1024 / 1024 < 2;
      if (!isLt2M) {
        this._messageService.error('图像必须小于2MB!');
        observer.complete();
        return;
      }
      // check height
      this.checkImageDimension(file).then(dimensionRes => {
        if (!dimensionRes) {
          this._messageService.error('图像必须高宽一致!');
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
          this._localStorageService.setAvatarUrl(info.file.response.avatar);
          this._messageService.success('头像修改成功，请刷新页面。');
        });
        break;
      case 'error':
        this._messageService.error('网络错误!');
        this.loading = false;
        break;
    }
  }

}
