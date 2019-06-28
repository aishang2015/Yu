using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yu.Data.Entities.Right;
using Yu.Data.Infrasturctures;
using Yu.Data.Repositories;
using Yu.Model.WebAdmin.Rule.OutputModels;
using RuleEntity = Yu.Data.Entities.Right.Rule;

namespace Yu.Service.WebAdmin.Rule
{
    public class RuleService : IRuleService
    {
        private readonly IRepository<RuleEntity, Guid> _ruleRepository;

        private readonly IRepository<RuleCondition, Guid> _ruleConditionRepository;

        private readonly IRepository<RuleGroup, Guid> _ruleGroupRepository;

        private readonly IUnitOfWork<BaseIdentityDbContext> _unitOfWork;

        public RuleService(IRepository<RuleEntity, Guid> ruleEntityRepository,
            IRepository<RuleCondition, Guid> ruleConditionRepository,
            IRepository<RuleGroup, Guid> ruleGroupRepository,
            IUnitOfWork<BaseIdentityDbContext> unitOfWork)
        {
            _ruleRepository = ruleEntityRepository;
            _ruleConditionRepository = ruleConditionRepository;
            _ruleGroupRepository = ruleGroupRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 修改规则组
        /// </summary>
        /// <param name="rules">规则</param>
        /// <param name="ruleConditions">条件</param>
        /// <param name="ruleGroup">规则组</param>
        public async Task AddOrUpdateRule(IEnumerable<RuleEntityResult> rules, IEnumerable<RuleConditionResult> ruleConditions, RuleGroup ruleGroup)
        {
            var group = _ruleGroupRepository.GetById(ruleGroup.Id);

            // 已经存在时先删除再插入
            if (group != null)
            {
                // 先删除再插入
                _ruleRepository.DeleteRange(r => r.RuleGroupId == group.Id);
                _ruleConditionRepository.DeleteRange(r => r.RuleGroupId == group.Id);
                _ruleGroupRepository.Update(ruleGroup);
            }

            // 修改规则组Id
            var groupId = group != null ? group.Id : Guid.NewGuid();
            ruleGroup.Id = groupId;

            // 修改每项规则的Id和Upid
            foreach (var rule in rules)
            {
                var oldId = rule.Id;

                // 替换前端生成的ID
                var ruleId = Guid.NewGuid().ToString();
                rule.Id = ruleId;
                rule.RuleGroupId = groupId.ToString();
                foreach (var r in rules)
                {
                    if (r.UpRuleId == oldId)
                    {
                        r.UpRuleId = ruleId;
                    }
                };
                foreach (var c in ruleConditions)
                {
                    if (c.RuleId == oldId)
                    {
                        c.RuleId = ruleId;
                        c.RuleGroupId = groupId.ToString();
                    }
                };
            }

            // 生成新的Id
            ruleConditions.ToList().ForEach(condition => condition.Id = Guid.NewGuid().ToString());

            // 保存全部数据
            await _ruleRepository.InsertRangeAsync(Mapper.Map<IEnumerable<RuleEntity>>(rules));
            await _ruleConditionRepository.InsertRangeAsync(Mapper.Map<IEnumerable<RuleCondition>>(ruleConditions));
            if (group == null)
            {
                await _ruleGroupRepository.InsertAsync(ruleGroup);
            }

            // todo 生成表达式保存到数据库

            // 提交事务
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 删除规则组
        /// </summary>
        /// <param name="ruleGroupId">规则组ID</param>
        public async Task DeleteRuleGroup(Guid ruleGroupId)
        {
            _ruleGroupRepository.DeleteRange(r => r.Id == ruleGroupId);
            _ruleRepository.DeleteRange(r => r.RuleGroupId == ruleGroupId);
            _ruleConditionRepository.DeleteRange(r => r.RuleGroupId == ruleGroupId);

            // 提交事务
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 取得所有规则组
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RuleGroup> GetAllRuleGroup()
        {
            return _ruleGroupRepository.GetAllNoTracking();
        }

        /// <summary>
        /// 取得规则组相关数据
        /// </summary>
        /// <param name="ruleGroupId">规则组ID</param>
        /// <returns></returns>
        public RuleResult GetRuleResult(Guid ruleGroupId)
        {
            return new RuleResult
            {
                RuleGroup = _ruleGroupRepository.GetById(ruleGroupId),
                Rules = Mapper.Map<IEnumerable<RuleEntityResult>>(_ruleRepository.GetByWhereNoTracking(rule => rule.RuleGroupId == ruleGroupId)),
                RuleConditions = Mapper.Map<IEnumerable<RuleConditionResult>>(_ruleConditionRepository.GetByWhereNoTracking(condition => condition.RuleGroupId == ruleGroupId))
            };
        }

    }
}
