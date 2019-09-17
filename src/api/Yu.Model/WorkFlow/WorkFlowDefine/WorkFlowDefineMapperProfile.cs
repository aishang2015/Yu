
using AutoMapper;
using Yu.Data.Entities.WorkFlow;
using Yu.Model.WorkFlow.WorkFlowDefine.OutputModels;

namespace Yu.Model.Areas.WorkFlow.WorkFlowDefines
{
    public class WorkFlowDefineMapperProfile : Profile
    {
        public WorkFlowDefineMapperProfile()
        {
            CreateMap<WorkFlowDefine, WorkflowDefineResult>();
        }
    }
}

