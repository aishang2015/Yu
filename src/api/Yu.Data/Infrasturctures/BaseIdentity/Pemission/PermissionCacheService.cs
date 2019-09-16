using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serialize.Linq.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Yu.Core.Constants;
using Yu.Core.Expressions;
using Yu.Data.Entities.Right;
using Yu.Data.Repositories;

namespace Yu.Data.Infrasturctures.BaseIdentity.Pemission
{
    public class PermissionCacheService : IPermissionCacheService
    {
        //private readonly IRepository<Api, Guid> _apiRepository;

        //private readonly IRepository<ElementApi, Guid> _elementApiRepository;

        //private readonly IRepository<Element, Guid> _elementRepository;

        //private readonly IRepository<RuleGroup, Guid> _ruleGroupRepository;

        //private readonly IRepository<GroupTree, Guid> _groupTreeRepository;

        //private readonly IRepository<Rule, Guid> _ruleRepository;

        //private readonly IRepository<RuleCondition, Guid> _ruleConditionRepository;


        private readonly BaseIdentityDbContext _context;

        private readonly IMemoryCache _memoryCache;

        private UserManager<BaseIdentityUser> _userManager;

        private RoleManager<BaseIdentityRole> _roleManager;

        public PermissionCacheService(BaseIdentityDbContext context, IMemoryCache memoryCache, UserManager<BaseIdentityUser> userManager, RoleManager<BaseIdentityRole> roleManager)
        {
            _context = context;
            _memoryCache = memoryCache;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        /// <summary>
        /// 获取角色的Claim
        /// </summary>
        /// <param name="roleName">角色名</param>
        public async Task<List<Claim>> GetRoleClaimAsync(string roleName)
        {
            return await _memoryCache.GetOrCreateAsync(CommonConstants.RoleClaimsMemoryCacheKey + roleName, async entity =>
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                var claims = await _roleManager.GetClaimsAsync(role);
                return claims.ToList();
            });
        }

        /// <summary>
        /// 获取角色的Claim
        /// </summary>
        /// <param name="roleName">角色名</param>
        /// <param name="roleName">Claim类型</param>
        public async Task<List<string>> GetRoleClaimValuesAsync(string roleName, string claimType)
        {
            var claims = await GetRoleClaimAsync(roleName);
            return claims.Where(c => c.Type == claimType).Select(c => c.Value).ToList();
        }

        /// <summary>
        /// 取得角色的api权限
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetRoleApiAsync(string roleName)
        {
            // 系统管理员拥有全部权限
            if (CommonConstants.SystemManagerRole.Equals(roleName))
            {
                return await _memoryCache.GetOrCreateAsync(CommonConstants.RoleApisMemoryCacheKey + roleName, async entity =>
                {
                    var elementIds = await GetRoleClaimValuesAsync(roleName, CustomClaimTypes.Element);
                    var apis = from a in _context.Set<Api>().AsNoTracking()
                               select a.Address + '|' + a.Type;
                    return string.Join(CommonConstants.StringConnectChar, apis);
                });
            }

            //return await _memoryCache.GetOrCreateAsync(CommonConstants.RoleApisMemoryCacheKey + roleName, async entity =>
            //{
            //    var elementIds = await GetRoleClaimValuesAsync(roleName, CustomClaimTypes.Element);
            //    var eas = from e in _context.Set<Element>().AsNoTracking()
            //              join ea in _context.Set<ElementApi>().AsNoTracking()
            //              on e.Id equals ea.ElementId
            //              where elementIds.Contains(e.Id.ToString())
            //              select ea;

            //    var apis = from ea in eas
            //               join a in _context.Set<Api>().AsNoTracking()
            //               on ea.ApiId equals a.Id
            //               select a.Address + '|' + a.Type;
            //    return string.Join(CommonConstants.StringConnectChar, apis);
            //});
            var elementIdss = await GetRoleClaimValuesAsync(roleName, CustomClaimTypes.Element);
            var eas = (from e in _context.Set<Element>().AsNoTracking()
                      join ea in _context.Set<ElementApi>().AsNoTracking()
                      on e.Id equals ea.ElementId
                      where elementIdss.Contains(e.Id.ToString())
                      select ea).ToList();

            var apiss = from ea in eas
                       join a in _context.Set<Api>().AsNoTracking()
                       on ea.ApiId equals a.Id
                       select a.Address + '|' + a.Type;
            return string.Join(CommonConstants.StringConnectChar, apiss);
        }

        /// <summary>
        /// 取得角色拥有的前端识别
        /// </summary>
        /// <param name="roleName">角色名称</param>
        public async Task<string> GetRoleIdentificationAsync(string roleName)
        {
            // 系统管理员拥有全部权限
            if (CommonConstants.SystemManagerRole.Equals(roleName))
            {
                return await _memoryCache.GetOrCreateAsync(CommonConstants.RoleIdentificationMemoryCacheKey + roleName, async entity =>
                {
                    var elementIds = await GetRoleClaimValuesAsync(roleName, CustomClaimTypes.Element);
                    var identifications = from e in _context.Set<Element>().AsNoTracking()
                                          select e.Identification;
                    return string.Join(CommonConstants.StringConnectChar, identifications);
                });
            }

            return await _memoryCache.GetOrCreateAsync(CommonConstants.RoleIdentificationMemoryCacheKey + roleName, async entity =>
            {
                var elementIds = await GetRoleClaimValuesAsync(roleName, CustomClaimTypes.Element);
                var identifications = from e in _context.Set<Element>().AsNoTracking()
                                      where elementIds.Contains(e.Id.ToString())
                                      select e.Identification;
                return string.Join(CommonConstants.StringConnectChar, identifications);
            });
        }

        /// <summary>
        /// 取得角色拥有的前端路由
        /// </summary>
        /// <param name="roleName">角色名称</param>
        public async Task<string> GetRoleRoutesAsync(string roleName)
        {
            // 系统管理员拥有全部权限
            if (CommonConstants.SystemManagerRole.Equals(roleName))
            {
                return await _memoryCache.GetOrCreateAsync(CommonConstants.RoleRouteMemoryCacheKey + roleName, async entity =>
                {
                    var elementIds = await GetRoleClaimValuesAsync(roleName, CustomClaimTypes.Element);
                    var routes = from e in _context.Set<Element>().AsNoTracking()
                                 select e.Route;
                    return string.Join(CommonConstants.StringConnectChar, routes);
                });
            }

            return await _memoryCache.GetOrCreateAsync(CommonConstants.RoleRouteMemoryCacheKey + roleName, async entity =>
            {
                var elementIds = await GetRoleClaimValuesAsync(roleName, CustomClaimTypes.Element);
                var routes = from e in _context.Set<Element>().AsNoTracking()
                             where elementIds.Contains(e.Id.ToString())
                             select e.Route;
                return string.Join(CommonConstants.StringConnectChar, routes);
            });
        }

        /// <summary>
        /// 清除角色的权限缓存
        /// </summary>
        /// <param name="roleName">角色名称</param>
        public Task ClearRolePermissionCache(string roleName)
        {
            _memoryCache.Remove(CommonConstants.RoleClaimsMemoryCacheKey + roleName);
            _memoryCache.Remove(CommonConstants.RoleApisMemoryCacheKey + roleName);
            _memoryCache.Remove(CommonConstants.RoleIdentificationMemoryCacheKey + roleName);
            _memoryCache.Remove(CommonConstants.RoleRouteMemoryCacheKey + roleName);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 清除所有角色的权限缓存
        /// </summary>
        public async Task ClearAllRolePermissionCache()
        {
            foreach (var role in _roleManager.Roles)
            {
                await ClearRolePermissionCache(role.Name);
            }
        }

        /// <summary>
        /// 取得用户规则
        /// </summary>
        public async Task<List<string>> GetUserRuleAsync(string userName, string[] userRoles)
        {
            // 系统管理员可以访问任何数据
            if (userRoles.Contains(CommonConstants.SystemManagerRole))
            {
                return _memoryCache.GetOrCreate(CommonConstants.RuleMemoryCacheKey + userName, entity => new List<string>());
            }
            else
            {
                return await _memoryCache.GetOrCreateAsync(CommonConstants.RuleMemoryCacheKey + userName,
                     async entity =>
                     {
                         var ruleGroups = new List<string> { };
                         var user = await _userManager.FindByNameAsync(userName);
                         foreach (var roleName in userRoles)
                         {
                             // 关联的数据规则
                             var ruleIds = await GetRoleClaimValuesAsync(roleName, CustomClaimTypes.Rule);
                             foreach (var ruleGroup in _context.Set<RuleGroup>().AsNoTracking().Where(rg => ruleIds.Contains(rg.Id.ToString())).ToList())
                             {
                                 ruleGroups.Add(ruleGroup.DbContext + '|' + ruleGroup.Entity + '|' + GetExpressionStr(ruleGroup.Id, userName, user.UserGroupId));
                             }
                         }

                         return ruleGroups;
                     });
            }

        }


        /// <summary>
        /// 取得规则组的表达式
        /// </summary>
        /// <param name="ruleGroupId">规则组ID</param>
        /// <returns></returns>
        private string GetExpressionStr(Guid ruleGroupId, string userName, string groupId)
        {
            // 规则信息
            var ruleGroup = _context.Set<RuleGroup>().Find(ruleGroupId);
            var rules = _context.Set<Rule>().AsNoTracking().Where(rule => rule.RuleGroupId == ruleGroupId);
            var ruleConditions = _context.Set<RuleCondition>().AsNoTracking().Where(condition => condition.RuleGroupId == ruleGroupId);

            // 生成表达式
            var topRule = rules.Where(rule => rule.UpRuleId.Equals(new Guid())).FirstOrDefault();
            var entityType = EntityTypeFinder.FindEntityType(ruleGroup.DbContext, ruleGroup.Entity);
            var expressionGroup = new ExpressionGroup(entityType);
            var keyValuePairs = new Dictionary<string, string> { };
            keyValuePairs.Add("UserName", userName);
            keyValuePairs.Add("GroupId", groupId);
            MakeExpressionGroup(topRule, rules, ruleConditions, entityType, keyValuePairs, ref expressionGroup);

            // 生成过滤表达式
            Expression lambda = expressionGroup.GetLambda();

            // 表达式序列化
            var serializer = new ExpressionSerializer(new JsonSerializer())
            {
                AutoAddKnownTypesAsListTypes = true
            };
            serializer.AddKnownType(typeof(Core.Expressions.ExpressionType));
            return serializer.SerializeText(lambda);
        }


        private void MakeExpressionGroup(Rule upRule,
            IEnumerable<Rule> rules,
            IEnumerable<RuleCondition> ruleConditions,
            Type entityType,
            Dictionary<string, string> keyValuePairs,
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
                MakeExpressionGroup(rule, rules, ruleConditions, entityType, keyValuePairs, ref eg);
                expressionGroup.ExpressionGroupsList.Add(eg);
            }

            // 规则类型
            expressionGroup.ExpressionCombineType = upRule.CombineType;

            // 规则下的条件
            var conditions = from condition in ruleConditions
                             where condition.RuleId == upRule.Id
                             select condition;

            // 添加条件
            foreach (var condition in conditions)
            {
                if ("${UserName}".Equals(condition.Value))
                {
                    condition.Value = keyValuePairs.GetValueOrDefault("UserName");
                }
                if ("${UserGroupId}".Equals(condition.Value))
                {
                    var groupId = keyValuePairs.GetValueOrDefault("GroupId");
                    if (!string.IsNullOrEmpty(groupId))
                    {
                        if (Guid.TryParse(groupId, out Guid gid))
                        {
                            var groupTrees = _context.Set<GroupTree>().AsNoTracking().Where(gt => gt.Ancestor == gid);
                            var treenodes = groupTrees.Select(g => g.Descendant.ToString()).ToList();
                            expressionGroup.TupleList.Add((condition.Field, treenodes, Core.Expressions.ExpressionType.ListContain));
                        }
                        continue;
                    }
                }
                expressionGroup.TupleList.Add((condition.Field, condition.Value, condition.OperateType));
            }

        }


    }
}
