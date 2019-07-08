using AutoMapper;
using Serialize.Linq.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Yu.Core.Expressions;
using Yu.Data.Entities.Right;
using Yu.Data.Infrasturctures;
using Yu.Data.Repositories;
using Yu.Model.WebAdmin.Rule.OutputModels;
using ExpressionType = Yu.Core.Expressions.ExpressionType;
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
        public async Task<bool> AddOrUpdateRule(IEnumerable<RuleEntityResult> rules, IEnumerable<RuleConditionResult> ruleConditions, RuleGroup ruleGroup)
        {
            var group = _ruleGroupRepository.GetByWhereNoTracking(rg => rg.Id == ruleGroup.Id).FirstOrDefault();

            // 已经存在时先删除再插入
            if (group != null)
            {
                // 先删除再插入
                _ruleRepository.DeleteRange(r => r.RuleGroupId == group.Id);
                _ruleConditionRepository.DeleteRange(r => r.RuleGroupId == group.Id);
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

            // 生成表达式保存到数据库
            // 获取实体类型
            var topRule = rules.Where(rule => string.IsNullOrEmpty(rule.UpRuleId)).FirstOrDefault();
            var entityType = EntityTypeFinder.FindEntityType(ruleGroup.DbContext, ruleGroup.Entity);
            var expressionGroup = new ExpressionGroup(entityType);
            MakeExpressionGroup(topRule, rules, ruleConditions, entityType, ref expressionGroup);

            // 生成过滤表达式
            Expression lambda;
            try
            {
                lambda = expressionGroup.GetLambda();
            }
            catch
            {
                return false;
            }

            // 表达式序列化
            var serializer = new ExpressionSerializer(new JsonSerializer());
            var value = serializer.SerializeText(lambda);
            ruleGroup.Lambda = value;

            // 更新或添加规则组
            if (group == null)
            {
                await _ruleGroupRepository.InsertAsync(ruleGroup);
            }
            else
            {
                _ruleGroupRepository.Update(ruleGroup);
            }


            // 提交事务
            await _unitOfWork.CommitAsync();

            return true;
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

        private void MakeExpressionGroup(RuleEntityResult upRule, IEnumerable<RuleEntityResult> rules,
            IEnumerable<RuleConditionResult> ruleConditions, Type entityType,
            ref ExpressionGroup expressionGroup)
        {
            // 查找子规则
            var childRules = from rule in rules
                             where rule.UpRuleId == upRule.Id
                             select rule;

            // 做成子规则
            foreach (var rule in childRules)
            {
                var eg = new ExpressionGroup(entityType) { };
                MakeExpressionGroup(rule, rules, ruleConditions, entityType, ref eg);
                expressionGroup.ExpressionGroupsList.Add(eg);
            }

            // 规则类型
            expressionGroup.ExpressionCombineType = (ExpressionCombineType)int.Parse(upRule.CombineType);

            // 规则下的条件
            var conditions = from condition in ruleConditions
                             where condition.RuleId == upRule.Id
                             select condition;

            // 添加条件
            foreach (var condition in conditions)
            {
                expressionGroup.TupleList.Add((condition.Field, condition.Value, (ExpressionType)int.Parse(condition.OperateType)));
            }

        }

    }
}
