using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Yu.Data.Entities.Enums;
using Yu.Model.WebAdmin.Element.InputModels;

namespace Yu.Model.WebAdmin.Element.Validators
{
    public class ElementDetailValidator : AbstractValidator<ElementDetail>
    {
        public ElementDetailValidator()
        {
            RuleFor(e => e.Name).NotEmpty().WithMessage(ErrorMessages.WebAdmin_Element_E001);
            RuleFor(e => e.ElementType).Must(elementType =>
               elementType == (int)ElementType.按钮 ||
               elementType == (int)ElementType.菜单 ||
               elementType == (int)ElementType.链接).WithMessage(ErrorMessages.WebAdmin_Element_E002);
        }
    }
}
