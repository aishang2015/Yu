using FluentValidation;
using Yu.Model.WebAdmin.Api.InputModels;

namespace Yu.Model.WebAdmin.Api.Validators
{
    public class ApiQueryValidator : AbstractValidator<ApiQuery>
    {
        public ApiQueryValidator()
        {
            RuleFor(m => m.PageIndex).NotEmpty().WithMessage(ErrorMessages.WebAdmin_Api_E001);
            RuleFor(m => m.PageSize).NotEmpty().WithMessage(ErrorMessages.WebAdmin_Api_E002);

            // -1这里因为NotEmpty已经包含了值类型为默认值的情况，例如int的0
            RuleFor(m => m.PageIndex).Must(p => p > -1).WithMessage(ErrorMessages.WebAdmin_Api_E003);
            RuleFor(m => m.PageSize).Must(p => p > -1 && p <= 100).WithMessage(ErrorMessages.WebAdmin_Api_E004);

            RuleFor(m => m.SearchText).MaximumLength(20).WithMessage(ErrorMessages.WebAdmin_Api_E005);
        }
    }
}
