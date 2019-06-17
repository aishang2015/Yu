import { Component, OnInit } from '@angular/core';
import { CommonConstant } from '../core/constants/common-constant';
import { Router } from '@angular/router';
import { LocalStorageService } from '../core/services/local-storage.service';

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

  constructor(private router: Router,
    private _localStorageService: LocalStorageService) { }

  ngOnInit() {
    this.userName = this._localStorageService.getUserName();
    this.avatarUrl = this._localStorageService.getAvatarUrl();
  }

  // 注销
  logout() {
    location.reload();
    localStorage.clear();
  }
}
