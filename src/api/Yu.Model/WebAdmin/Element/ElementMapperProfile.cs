using AutoMapper;
using System;
using Yu.Data.Entities.Enums;
using Yu.Data.Infrasturctures;
using Yu.Model.WebAdmin.Element.InputModels;
using Yu.Model.WebAdmin.User.OutputModels;

namespace Yu.Model.WebAdmin.Element
{
    public class ElementMapperProfile : Profile
    {
        public ElementMapperProfile()
        {
            AllowNullCollections = true;

            // 映射 如果id为空则创建新id
            CreateMap<ElementDetail, Data.Entities.Right.Element>()
                .ForMember(e => e.Id,
                ex => ex.MapFrom(ed => string.IsNullOrEmpty(ed.Id) ? Guid.NewGuid() : Guid.Parse(ed.Id)))
                .ForMember(e => e.ElementType,
                ex => ex.MapFrom(ed => (ElementType)ed.ElementType));
        }
    }
}
