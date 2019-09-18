
using AutoMapper;
using Yu.Data.Entities.WorkFlow;

namespace Yu.Model.Areas.WorkFlow.WorkFlowFlowNodes
{
    public class WorkFlowFlowNodeMapperProfile : Profile
    {
        public WorkFlowFlowNodeMapperProfile()
        {
            CreateMap<WorkFlowFlowNode, WorkFlowFlowNode>();
        }
    }
}

