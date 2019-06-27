import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from "@angular/forms";
import { NgZorroAntdModule } from "ng-zorro-antd";
import { GenderPipe } from './pipes/gender.pipe';
import { TelephoneValidatorDirective } from './validators/telephone-validator.directive';
import { DebounceClickDirective } from './directives/debounce-click.directive';
import { ThrottleClickDirective } from './directives/throttle-click.directive';
import { ImageUriPipe } from './pipes/image-uri.pipe';
import { AppRuleComponent } from './components/app-rule/app-rule.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    NgZorroAntdModule
  ],
  declarations: [
    AppRuleComponent, // 规则编辑组件

    GenderPipe, // 性别转换管道
    ImageUriPipe, // 图片地址转换

    TelephoneValidatorDirective, // 电话号码验证器
    DebounceClickDirective,  // 去抖点击 
    ThrottleClickDirective // 节流点击
  ],
  exports: [
    FormsModule,
    NgZorroAntdModule,
    AppRuleComponent,
    GenderPipe,
    ImageUriPipe,
    TelephoneValidatorDirective,
    DebounceClickDirective,
    ThrottleClickDirective
  ]
})
export class CoreModule { }
