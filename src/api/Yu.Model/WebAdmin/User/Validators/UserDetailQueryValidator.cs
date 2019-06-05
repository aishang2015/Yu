using FluentValidation;
using Yu.Model.WebAdmin.User.InputModels;

namespace Yu.Model.WebAdmin.User.Validators
{
    public class UserDetailQueryValidator : AbstractValidator<UserDetailQuery>
    {
        public UserDetailQueryValidator()
        {
            RuleFor(m => m.UserId).NotEmpty().WithMessage(ErrorMessages.WebAdmin_User_E006);
        }
    }
}
