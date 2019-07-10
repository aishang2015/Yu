using FluentValidation;
using Yu.Model.Common.InputModels;
using Yu.Model.Message;

namespace Yu.Model.Common.Validators
{
    public class PagedQueryValidator : AbstractValidator<PagedQuery>
    {
        public PagedQueryValidator()
        {
            RuleFor(m => m.PageIndex).NotEmpty().WithMessage(ErrorMessages.WebAdmin_Common_E001);
            RuleFor(m => m.PageSize).NotEmpty().WithMessage(ErrorMessages.WebAdmin_Common_E002);
            RuleFor(m => m.PageIndex).Must(p => p > -1).WithMessage(ErrorMessages.WebAdmin_Common_E003); // 0的情况会判断为empty,所以这里不判断0
            RuleFor(m => m.PageSize).Must(p => p > -1 && p <= 100).WithMessage(ErrorMessages.WebAdmin_Common_E004);
            RuleFor(m => m.SearchText).MaximumLength(20).WithMessage(ErrorMessages.WebAdmin_Common_E005);
        }
    }
}
