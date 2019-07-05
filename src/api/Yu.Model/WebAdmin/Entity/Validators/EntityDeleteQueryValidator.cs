using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Yu.Model.WebAdmin.Entity.InputModels;

namespace Yu.Model.WebAdmin.Entity.Validators
{
    public class EntityDeleteQueryValidator : AbstractValidator<EntityDeleteQuery>
    {
        public EntityDeleteQueryValidator()
        {
            RuleFor(e => e.EntityId).NotEmpty().WithMessage(ErrorMessages.WebAdmin_Entity_E006);
        }
    }
}
