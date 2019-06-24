using AutoMapper;
using System;
using Yu.Model.WebAdmin.Group.InputModels;

namespace Yu.Model.WebAdmin.Group
{
    public class GroupMapperProfile : Profile
    {
        public GroupMapperProfile()
        {
            AllowNullCollections = true;
            CreateMap<GroupDetail, Data.Entities.Right.Group>()
                .ForMember(g => g.Id, ex => ex.MapFrom(gd => string.IsNullOrEmpty(gd.Id) ? Guid.NewGuid() : Guid.Parse(gd.Id)));
        }
    }
}
