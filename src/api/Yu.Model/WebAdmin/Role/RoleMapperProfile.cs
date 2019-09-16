using AutoMapper;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;
using Yu.Model.WebAdmin.Role.InputOuputModels;
using Yu.Model.WebAdmin.Role.OutputModels;

namespace Yu.Model.WebAdmin.Role
{
    public class RoleMapperProfile : Profile
    {
        public RoleMapperProfile()
        {
            AllowNullCollections = true;
            CreateMap<BaseIdentityRole, RoleOutline>();
            CreateMap<BaseIdentityRole, RoleDetail>();
            CreateMap<RoleDetail, BaseIdentityRole>();
        }
    }
}
