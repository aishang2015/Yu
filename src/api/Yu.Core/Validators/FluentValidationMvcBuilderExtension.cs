using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Yu.Core.Extensions;
using Yu.Core.Utils;

namespace Yu.Core.Validators
{
    public static class FluentValidationMvcBuilderExtension
    {
        public static void AddFluentValidators(this IMvcBuilder builder)
        {
            // 通过RegisterValidatorsFromAssemblies来自动注册程序集内的所有验证器
            builder.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblies(ReflectionUtil.GetAssemblies()));

            // 验证模式设置为遇错即终止
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
        }

        // 统一模型验证结果的格式
        public static void ConfigureFluentValidationModelErrors(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                // 关于模型验证，当模型验证结果错误的时候，框架会自动返回400
                // 在这里可以自行定义返回内容。
                // 针对FluentValidation，在这里统一错误的返回形式，来达到和
                // controller一致的效果。
                options.InvalidModelStateResponseFactory = (context) =>
                {
                    return new BadRequestObjectResult(context.ModelState);
                };
            });
        }
    }
}
