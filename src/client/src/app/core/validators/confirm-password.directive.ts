import { Directive } from '@angular/core';
import { NG_VALIDATORS, Validator, AbstractControl, ValidationErrors, ValidatorFn, FormGroup } from '@angular/forms';

@Directive({
    selector: '[appConfirmPassword]',
    providers: [{ provide: NG_VALIDATORS, useExisting: ConfirmPasswordValidatorDirective, multi: true }]
})
export class ConfirmPasswordValidatorDirective implements Validator {
    validate(control: AbstractControl): ValidationErrors {
        return this.identityRevealedValidator(control)
    }

    identityRevealedValidator: ValidatorFn = (control: FormGroup): ValidationErrors | null => {
        const newPwd = control.get('newPassword');
        const confirmNewPwd = control.get('confirmPassword');

        return newPwd && confirmNewPwd && newPwd.value === confirmNewPwd.value ? null : { 'isNotSame': true };
    };
}