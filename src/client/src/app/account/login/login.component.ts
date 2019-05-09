import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  imagesrc: string = "https://localhost:44334/captcha";

  constructor() { }

  ngOnInit() {
  }

  submit() {
  }

  refresh() {
    this.imagesrc = "https://localhost:44334/captcha?" + Math.random();
  }

}
