using FluentValidation;
using Yu.Model.Account.InputModels;

namespace Yu.Model.Account.Validators
{
    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            RuleFor(m => m.UserName).NotEmpty().WithMessage(ErrorMessages.Account_E001);
            RuleFor(m => m.Password).NotEmpty().WithMessage(ErrorMessages.Account_E002);
            RuleFor(m => m.CaptchaCode).NotEmpty().WithMessage(ErrorMessages.Account_E003);
        }
    }
}
