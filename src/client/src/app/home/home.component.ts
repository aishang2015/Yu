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
}
