using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Yu.Model.WebAdmin.Element.InputModels;

namespace Yu.Model.WebAdmin.Element.Validators
{
    public class ElementQueryValidator : AbstractValidator<ElementQuery>
    {
        public ElementQueryValidator()
        {
            RuleFor(e => e.ElementId).NotEmpty().WithMessage(ErrorMessages.WebAdmin_Element_E005);
        }
    }
}
