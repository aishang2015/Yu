using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Yu.Core.Validators.CustomValidators;

namespace Yu.Core.Validators
{
    /// <summary>
    /// 扩展FluentValidation
    /// </summary>
    public static class FluentValidationExtension
    {
        /// <summary>
        /// 为PhoneValidator定义验证表达式
        /// </summary>
        /// <typeparam name="T">验证对象的类型</typeparam>
        /// <param name="ruleBuilder">验证规则构建器</param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, string> IsPhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new PhoneValidator());
        }
    }
}
