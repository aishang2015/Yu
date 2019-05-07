import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MainComponent } from './main/main.component';
import { RouterModule } from '@angular/router';
import { NgZorroAntdModule } from 'ng-zorro-antd';
import { HttpClientModule } from '@angular/common/http';
import { CoreModule } from '../core/core.module';

@NgModule({
  declarations: [
    MainComponent
  ],
  imports: [
    CommonModule,
    CoreModule,// 动态加载icon需要在根模块配置HttpClientModule，否则会提示找不到
    RouterModule.forChild([
      { path: '', component: MainComponent }
    ]),
  ]
})
export class DashboardModule { }
