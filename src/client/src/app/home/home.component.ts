import { Component, OnInit } from '@angular/core';
import { CommonConstant } from '../core/constants/common-constant';
import { Router } from '@angular/router';
import { LocalStorageService } from '../core/services/local-storage.service';
import { UriConstant } from '../core/constants/uri-constant';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  isCollapsed = false;
  isReverseArrow = false;
  width = 200;

  // 用户名
  userName = '';

  // 头像地址
  avatarUrl = '';

  // 面包渣导航
  prefix = '仪表盘';
  endfix = '';

  // 展开map
  openMap: { [name: string]: boolean } = {
    'right': false,
    'test': false,
  }

  // 选择的菜单项
  selectMap: { [name: string]: boolean } = {
    'rightuser': false,
    'rightrole': false,
    'rightgroup': false,
    'rightmenu': false,
    'rightapi': false,
    'rightrule': false,
    'rightentity': false,

    'testdata':false,
  }

  constructor(private router: Router,
    private _localStorageService: LocalStorageService) { }

  ngOnInit() {
    this.userName = this._localStorageService.getUserName();
    this.avatarUrl = this._localStorageService.getAvatarUrl() ? UriConstant.AvatarBaseUri + this._localStorageService.getAvatarUrl() : '';
  }

  // 注销
  logout() {
    location.reload();
    localStorage.clear();
  }

  setBreadCrumb(item1, item2) {
    this.prefix = item1;
    this.endfix = item2;
  }

  setDashboardBreadCrumb() {
    this.setBreadCrumb('仪表盘', '');

    for (const key in this.openMap) {
      this.openMap[key] = false;
    };

    for (const key in this.selectMap) {
      this.selectMap[key] = false;
    };
  }

  // 是否有操作权限
  canOperate(identifycation) {
    var identifycations = this._localStorageService.getIdentifycations();
    return identifycations.findIndex(i => i == identifycation) > -1;
  }



}
