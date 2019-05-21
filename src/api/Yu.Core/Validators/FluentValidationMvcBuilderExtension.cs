using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Yu.Core.Extensions;

namespace Yu.Core.Validators
{
    public static class FluentValidationMvcBuilderExtension
    {
        public static void AddFluentValidators(this IMvcBuilder builder)
        {
            // 通过RegisterValidatorsFromAssemblies来自动注册程序集内的所有验证器
            builder.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblies(TypeExtension.GetAssemblies()));
        }
    }
}
