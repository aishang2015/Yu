using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Yu.Core.Extensions;

namespace Yu.Core.AutoMapper
{
    public static class AutoMapperExtension
    {
        public static void AddAutoMapper(this IServiceCollection serviceCollection)
        {
            // 取得所有Profile文件 加入配置。
            var mapperProfiles = typeof(Profile).GetAllChildType();
            foreach(var profile in mapperProfiles)
            {
                Mapper.Initialize(config => config.AddProfile(profile));
            }
        }

        private class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {

            }
        }
    }
}
