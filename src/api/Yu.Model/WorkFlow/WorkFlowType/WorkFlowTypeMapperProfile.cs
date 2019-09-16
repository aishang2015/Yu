
using AutoMapper;
using Yu.Data.Entities.WorkFlow;

namespace Yu.Model.Areas.WorkFlow.WorkFlowTypes
{
    public class WorkFlowTypeMapperProfile : Profile
    {
        public WorkFlowTypeMapperProfile()
        {
            CreateMap<WorkFlowType, WorkFlowType>();
        }
    }
}

