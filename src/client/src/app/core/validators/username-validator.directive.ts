import { Directive } from '@angular/core';
import { Validator, NG_VALIDATORS, AbstractControl, ValidationErrors } from '@angular/forms';

@Directive({
    selector: '[appUserName]',
    providers: [{ provide: NG_VALIDATORS, useExisting: UserNameValidatorDirective, multi: true }]
})

// 模板驱动表单中，需要通过指令的方式来实现自定义验证器
export class UserNameValidatorDirective implements Validator {

    validate(control: AbstractControl): ValidationErrors | null {

        // 用户名验证表达式
        // const userNamePattern = new RegExp('[a|b|c|d|e|f|g|h|i|j|k|l|m|n|o|p|q|r|s|t|u|v|w|x|y|z|A|B|C|D|E|F|G|H|I|J|K|L|M|N|O|P|Q|R|S|T|U|V|W|X|Y|Z|0|1|2|3|4|5|6|7|8|9|-|\.|_|@|\+]*');
        const userNamePattern = new RegExp('^[a-zA-Z0-9\._@\+]+$');
        const isUserName = userNamePattern.test(control.value);
        return control.value ? (isUserName ? null : { 'userName': control.valid }) : null;
    }
}
