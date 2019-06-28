using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Yu.Data.Entities.Right;
using Yu.Model.WebAdmin.Rule.OutputModels;

namespace Yu.Model.WebAdmin.Rule
{
    public class RuleMapperProfile : Profile
    {
        public RuleMapperProfile()
        {
            CreateMap<Yu.Data.Entities.Right.Rule, RuleEntityResult>()
                .ForMember(rr => rr.UpRuleId, ex => ex.MapFrom(rr => rr.UpRuleId == new Guid() ? "" : rr.UpRuleId.ToString()))
                .ForMember(rr => rr.CombineType, ex => ex.MapFrom(r => ((int)r.CombineType).ToString()));
            CreateMap<RuleCondition, RuleConditionResult>()
                .ForMember(rcr => rcr.OperateType, ex => ex.MapFrom(rc => ((int)rc.OperateType).ToString()));

            CreateMap<RuleEntityResult, Yu.Data.Entities.Right.Rule>()
                .ForMember(ruleEntity => ruleEntity.UpRuleId, ex => ex.MapFrom(rr => string.IsNullOrEmpty(rr.UpRuleId) ? new Guid() : Guid.Parse(rr.UpRuleId)));
            CreateMap<RuleConditionResult, RuleCondition>();
        }
    }
}
