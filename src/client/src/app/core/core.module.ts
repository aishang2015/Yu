import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgZorroAntdModule, NZ_I18N, zh_CN } from "ng-zorro-antd";
import { GenderPipe } from './pipes/gender.pipe';
import { TelephoneValidatorDirective } from './validators/telephone-validator.directive';
import { DebounceClickDirective } from './directives/debounce-click.directive';
import { ThrottleClickDirective } from './directives/throttle-click.directive';
import { AvatarImagePipe } from './pipes/image-uri.pipe';
import { AppRuleComponent } from './components/app-rule/app-rule.component';
import { UserNameValidatorDirective } from './validators/username-validator.directive';
import { ConfirmPasswordValidatorDirective } from './validators/confirm-password.directive';
import { CanOperateDirective } from './directives/can-operate.directive';

/** 配置 angular i18n **/
import { registerLocaleData } from '@angular/common';
import zh from '@angular/common/locales/zh';
import { DragDirective } from './directives/drag.directive';
registerLocaleData(zh);

import { DragDropModule } from '@angular/cdk/drag-drop'; 
import { HtmlPipe } from './pipes/html.pipe';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    NgZorroAntdModule,
  ],
  declarations: [
    AppRuleComponent, // 规则编辑组件

    GenderPipe, // 性别转换管道
    AvatarImagePipe, // 图片地址转换
    HtmlPipe, // html安全转换

    TelephoneValidatorDirective, // 电话号码验证器
    UserNameValidatorDirective, // 用户名验证器
    ConfirmPasswordValidatorDirective,// 验证确认密码和密码是否一致

    DebounceClickDirective,  // 去抖点击 
    ThrottleClickDirective, // 节流点击
    CanOperateDirective, // 结构型指令判断用户是否有权限操作此元素
  ],
  exports: [
    FormsModule,
    ReactiveFormsModule,
    NgZorroAntdModule,
    AppRuleComponent,
    GenderPipe,
    AvatarImagePipe,
    HtmlPipe,
    TelephoneValidatorDirective,
    UserNameValidatorDirective,
    ConfirmPasswordValidatorDirective,
    CanOperateDirective,
    DebounceClickDirective,
    ThrottleClickDirective,
    DragDropModule,
  ],
  /** 配置 ng-zorro-antd 国际化（文案 及 日期） **/
  providers: [
    { provide: NZ_I18N, useValue: zh_CN }
  ]
})
export class CoreModule { }
