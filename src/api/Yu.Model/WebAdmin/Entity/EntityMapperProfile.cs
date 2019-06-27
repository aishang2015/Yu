using AutoMapper;
using Yu.Model.WebAdmin.Entity.OutputModels;

namespace Yu.Model.WebAdmin.Entity
{
    public class EntityMapperProfile : Profile
    {
        public EntityMapperProfile()
        {
            CreateMap<Data.Entities.Right.Entity, EntityOutline>();
        }
    }
}
