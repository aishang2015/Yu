
using AutoMapper;
using Yu.Data.Entities.WebAdmin;

namespace Yu.Model.Areas.WebAdmin.Positions
{
    public class PositionMapperProfile : Profile
    {
        public PositionMapperProfile()
        {
            CreateMap<Position, Position>();
        }
    }
}

