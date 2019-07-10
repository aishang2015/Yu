using FluentValidation;
using Yu.Model.Common.InputModels;
using Yu.Model.Message;

namespace Yu.Model.Common.Validators
{
    public class IdQueryValidator : AbstractValidator<IdQuery>
    {
        public IdQueryValidator()
        {
            RuleFor(a => a.Id).NotEmpty().WithMessage(ErrorMessages.WebAdmin_Common_E006);
        }
    }
}
