using FluentValidation;
using Yu.Model.WebAdmin.Entity.InputModels;

namespace Yu.Model.WebAdmin.Entity.Validators
{
    public class EntityGetQueryValidator : AbstractValidator<EntityGetQuery>
    {
        public EntityGetQueryValidator()
        {
            RuleFor(e => e.PageIndex).NotEmpty().WithMessage(ErrorMessages.WebAdmin_Entity_E001);
            RuleFor(e => e.PageSize).NotEmpty().WithMessage(ErrorMessages.WebAdmin_Entity_E002);

            // -1这里因为NotEmpty已经包含了值类型为默认值的情况，例如int的0
            RuleFor(e => e.PageIndex).Must(p => p > -1).WithMessage(ErrorMessages.WebAdmin_Entity_E003);
            RuleFor(e => e.PageSize).Must(p => p > -1 && p <= 100).WithMessage(ErrorMessages.WebAdmin_Entity_E004);

            RuleFor(e => e.SearchText).MaximumLength(20).WithMessage(ErrorMessages.WebAdmin_Entity_E005);
        }
    }
}
