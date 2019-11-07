using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serialize.Linq.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Yu.Core.Constants;
using Yu.Core.Expressions;
using Yu.Core.Extensions;
using Yu.Data.Entities.Right;

namespace Yu.Data.Infrasturctures.BaseIdentity.Pemission
{
    public class PermissionCacheService : IPermissionCacheService
    {

        private readonly BaseIdentityDbContext _context;

        private readonly IMemoryCache _memoryCache;

        private UserManager<BaseIdentityUser> _userManager;

        private RoleManager<BaseIdentityRole> _roleManager;

        private IHttpContextAccessor _httpContextAccessor;

        public PermissionCacheService(BaseIdentityDbContext context,
            IMemoryCache memoryCache,
            UserManager<BaseIdentityUser> userManager,
            RoleManager<BaseIdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _memoryCache = memoryCache;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 清除角色Claim缓存
        /// </summary>
        public void ClearRoleClaimCache(string roleName)
        {
            var key = CommonConstants.RoleClaimsMemoryCacheKey + roleName;
            _memoryCache.Remove(key);
        }

        /// <summary>
        /// 清除Element缓存
        /// </summary>
        public void ClearElementCache()
        {
            _memoryCache.Remove(CommonConstants.ElementCacheKey);
        }

        /// <summary>
        /// 清除ElementApi缓存
        /// </summary>
        public void ClearElementApiCache()
        {
            _memoryCache.Remove(CommonConstants.ElementApiCacheKey);
        }

        /// <summary>
        /// 清除Api缓存
        /// </summary>
        public void ClearApiCache()
        {
            _memoryCache.Remove(CommonConstants.ApiCacheKey);
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
        /// 判断当前用户是否有该api权限
        /// </summary>
        public async Task<bool> HaveApiRight(string address, string type)
        {
            var userName = _httpContextAccessor.HttpContext.User.GetClaimValue(CustomClaimTypes.UserName);
            var rolestr = _httpContextAccessor.HttpContext.User.GetClaimValue(CustomClaimTypes.Role);
            var roles = rolestr.Split(',', StringSplitOptions.RemoveEmptyEntries);

            // 系统管理员拥有全部API权限
            if (roles.Contains(CommonConstants.SystemManagerRole))
            {
                return true;
            }

            var list = new List<string>();
            foreach (var role in roles)
            {
                list.AddRange(await GetRoleClaimValuesAsync(role, CustomClaimTypes.Element));
            }

            // api列表
            var apiids = from e in GetElementSet()
                         join ea in GetElementApiSet()
                         on e.Id equals ea.ElementId
                         where list.Contains(e.Id.ToString())
                         select ea.ApiId;

            // 检查是否有该api
            var count = (from api in GetApiSet()
                         where apiids.Contains(api.Id)
                         where api.Address == address && api.Type == type
                         select api).Count();

            return count > 0;

        }

        /// <summary>
        /// 取得角色拥有的前端识别
        /// </summary>
        public async Task<List<string>> GetRoleIdentificationAsync(List<string> roles)
        {
            var result = new List<string>();
            if (roles.Contains(CommonConstants.SystemManagerRole))
            {
                var identifications = GetElementSet().Select(e => e.Identification);
                result.AddRange(identifications);
            }
            else
            {
                var elementIdList = new List<string>();
                foreach (var role in roles)
                {
                    var elementIds = await GetRoleClaimValuesAsync(role, CustomClaimTypes.Element);
                    elementIdList.AddRange(elementIds);
                }
                var identifications = from e in GetElementSet()
                                      where elementIdList.Contains(e.Id.ToString())
                                      select e.Identification;
                result.AddRange(identifications);
            }
            return result;
        }

        /// <summary>
        /// 取得角色拥有的前端路由
        /// </summary>
        public async Task<List<string>> GetRoleRoutesAsync(List<string> roles)
        {
            var result = new List<string>();
            if (roles.Contains(CommonConstants.SystemManagerRole))
            {
                var routes = GetElementSet().Select(e => e.Route);
                result.AddRange(routes);
            }
            else
            {
                var elementIdList = new List<string>();
                foreach (var role in roles)
                {
                    var elementIds = await GetRoleClaimValuesAsync(role, CustomClaimTypes.Element);
                    elementIdList.AddRange(elementIds);
                }
                var routes = from e in GetElementSet()
                             where elementIdList.Contains(e.Id.ToString())
                             select e.Route;
                result.AddRange(routes);
            }
            return result;
        }

        /// <summary>
        /// 取得用户的数据规则
        /// </summary>
        public async Task<List<string>> GetRuleAsync(string dbContextName, string entityName)
        {
            var userName = _httpContextAccessor.HttpContext.User.GetClaimValue(CustomClaimTypes.UserName);
            var key = CommonConstants.RuleMemoryCacheKey + userName + dbContextName + entityName;
            if (_memoryCache.TryGetValue(key, out List<string> list))
            {
                return list;
            }
            else
            {
                var user = await _userManager.FindByNameAsync(userName);
                var roles = user.Roles.Split(',');
                var ruleGroupStrList = new List<string> { };
                foreach (var role in roles)
                {
                    var rules = await GetRoleClaimValuesAsync(role, CustomClaimTypes.Rule);
                    var ruleGroups = _context.Set<RuleGroup>().Where(rg => rules.Contains(rg.Id.ToString())
                        && rg.DbContext == dbContextName && rg.Entity == entityName);
                    foreach (var rg in ruleGroups)
                    {
                        ruleGroupStrList.Add(GetExpressionStr(rg.Id, userName, user.UserGroupId));
                    }
                }
                _memoryCache.Set(key, ruleGroupStrList, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15),
                });
                return ruleGroupStrList;
            }
        }

        /// <summary>
        /// 获取角色的Claim
        /// </summary>
        private async Task<List<Claim>> GetRoleClaimAsync(string roleName)
        {
            return await _memoryCache.GetOrCreateAsync(CommonConstants.RoleClaimsMemoryCacheKey + roleName, async entity =>
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                var claims = await _roleManager.GetClaimsAsync(role);
                return claims.ToList();
            });
        }

        /// <summary>
        /// 取得element集合
        /// </summary>
        private List<Element> GetElementSet()
        {
            return _memoryCache.GetOrCreate(CommonConstants.ElementCacheKey, item =>
            {
                return _context.Set<Element>().ToList();
            });
        }

        /// <summary>
        /// 取得ElementApi集合
        /// </summary>
        private List<ElementApi> GetElementApiSet()
        {
            return _memoryCache.GetOrCreate(CommonConstants.ElementApiCacheKey, item =>
            {
                return _context.Set<ElementApi>().ToList();
            });

        }

        /// <summary>
        /// 取得Api集合
        /// </summary>
        private List<Api> GetApiSet()
        {
            return _memoryCache.GetOrCreate(CommonConstants.ApiCacheKey, item =>
            {
                return _context.Set<Api>().ToList();
            });

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
