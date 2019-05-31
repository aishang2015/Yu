using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yu.Core.Extensions;

namespace Yu.Core.AutoMapper
{
    public static class AutoMapperServiceCollectionExtension
    {
        public static void AddAutoMapper(this IServiceCollection serviceCollection)
        {
            // 取得所有Profile文件 加入配置。
            var mapperProfiles = typeof(Profile).GetAllChildType();

            // 整合profile添加 初始化mapper
            var mapperConfigurationExpression = new MapperConfigurationExpression();
            foreach (var profile in mapperProfiles)
            {
                mapperConfigurationExpression.AddProfile(profile);
            }
            Mapper.Initialize(mapperConfigurationExpression);
        }

        private class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {

            }
        }
    }
}
