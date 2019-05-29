import { Component, OnInit } from '@angular/core';
import { CommonConstant } from '../core/constants/common-constant';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  isCollapsed = false;
  isReverseArrow = false;
  width = 200;

  constructor(private router: Router) { }

  ngOnInit() {
  }


  // 注销
  logout() {
    localStorage.removeItem(CommonConstant.AuthToken);
    this.router.navigate(["login"]);
  }
}
