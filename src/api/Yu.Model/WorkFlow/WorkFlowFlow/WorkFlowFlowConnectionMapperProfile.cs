
using AutoMapper;
using Yu.Data.Entities.WorkFlow;

namespace Yu.Model.Areas.WorkFlow.WorkFlowFlowConnections
{
    public class WorkFlowFlowConnectionMapperProfile : Profile
    {
        public WorkFlowFlowConnectionMapperProfile()
        {
            CreateMap<WorkFlowFlowConnection, WorkFlowFlowConnection>();
        }
    }
}

