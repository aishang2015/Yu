using FluentValidation;
using Yu.Model.WebAdmin.Api.InputModels;

namespace Yu.Model.WebAdmin.Api.Validators
{
    public class ApiDeleteQueryValidator : AbstractValidator<ApiDeleteQuery>
    {
        public ApiDeleteQueryValidator()
        {
            RuleFor(a => a.ApiId).NotEmpty().WithMessage(ErrorMessages.WebAdmin_Api_E006);
        }
    }
}
