using FluentValidation;
using Yu.Model.WebAdmin.Group.InputModels;

namespace Yu.Model.WebAdmin.Group.Validators
{
    public class GroupQueryValidator : AbstractValidator<GroupQuery>
    {
        public GroupQueryValidator()
        {
            RuleFor(gq => gq.GroupId).NotEmpty().WithMessage(ErrorMessages.WebAdmin_Group_E001);
        }
    }
}
