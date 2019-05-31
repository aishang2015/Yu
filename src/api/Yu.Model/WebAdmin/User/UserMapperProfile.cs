using AutoMapper;
using Yu.Data.Infrasturctures;
using Yu.Model.WebAdmin.User.OutputModels;

namespace Yu.Model.WebAdmin.User
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            AllowNullCollections = true;
            CreateMap<BaseIdentityUser, UserOutline>();
        }
    }
}
