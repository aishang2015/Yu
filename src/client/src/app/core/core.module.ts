import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from "@angular/forms";
import { NgZorroAntdModule } from "ng-zorro-antd";

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    NgZorroAntdModule
  ],
  declarations: [],
  exports: [
    FormsModule,
    NgZorroAntdModule,
  ]
})
export class CoreModule { }
