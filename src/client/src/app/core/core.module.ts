import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from "@angular/forms";
import { NgZorroAntdModule } from "ng-zorro-antd";
import { GenderPipe } from './pipes/gender.pipe';
import { TelephoneValidatorDirective } from './validators/telephone-validator.directive';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    NgZorroAntdModule
  ],
  declarations: [
    GenderPipe,
    TelephoneValidatorDirective
  ],
  exports: [
    FormsModule,
    NgZorroAntdModule,
    GenderPipe,
    TelephoneValidatorDirective
  ]
})
export class CoreModule { }
