using AutoMapper;
using System;
using System.Linq;
using Yu.Data.Infrasturctures;
using Yu.Model.WebAdmin.User.OutputModels;

namespace Yu.Model.WebAdmin.User
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            AllowNullCollections = true;
            CreateMap<BaseIdentityUser, UserOutline>()
                .ForMember(uo => uo.Roles, ex => ex.MapFrom(biu => biu.Roles.Split(',', StringSplitOptions.None).ToArray()));
            CreateMap<BaseIdentityUser, UserDetail>();
            CreateMap<UserDetail, BaseIdentityUser>()
                .ForMember(biu => biu.Roles, ex => ex.MapFrom(ud => string.Join(',', ud.Roles)));
        }
    }
}
