import { Directive } from '@angular/core';
import { Validator, NG_VALIDATORS, AbstractControl, ValidationErrors } from '@angular/forms';

@Directive({
  selector: '[appTelephone]',
  providers: [{ provide: NG_VALIDATORS, useExisting: TelephoneValidatorDirective, multi: true }]
})

// 模板驱动表单中，需要通过指令的方式来实现自定义验证器
export class TelephoneValidatorDirective implements Validator {

  validate(control: AbstractControl): ValidationErrors | null {

    // 手机号码正则表达式
    const telePhonePattern = new RegExp('^((13[0-9])|(14[5,7])|(15[0-3,5-9])|(17[0,3,5-8])|(18[0-9])|166|198|199|(147))\\d{8}$');
    const isTelePhone = telePhonePattern.test(control.value);
    return control.value ? (isTelePhone ? null : { 'telephone': control.valid }) : null;
  }
}
