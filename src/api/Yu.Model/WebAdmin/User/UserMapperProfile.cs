using AutoMapper;
using System;
using System.Linq;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;
using Yu.Model.WebAdmin.User.OutputModels;

namespace Yu.Model.WebAdmin.User
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            AllowNullCollections = true;
            CreateMap<BaseIdentityUser, UserOutline>().ForMember(outline => outline.Roles,
                ex => ex.MapFrom(biu => biu.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries)));
            CreateMap<BaseIdentityUser, UserDetail>().ForMember(detail => detail.Roles,
                ex => ex.MapFrom(biu => biu.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries)));
            CreateMap<UserDetail, BaseIdentityUser>().ForMember(identityUser => identityUser.Roles,
                ex => ex.MapFrom(ud => string.Join(',', ud.Roles)));
        }
    }
}
