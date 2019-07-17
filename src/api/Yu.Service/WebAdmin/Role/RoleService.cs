﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Yu.Core.Constants;
using Yu.Core.Expressions;
using Yu.Data.Entities;
using Yu.Data.Entities.Right;
using Yu.Data.Infrasturctures;
using Yu.Data.Repositories;
using Yu.Model.WebAdmin.Role.InputOuputModels;
using Yu.Model.WebAdmin.Role.OutputModels;
using ApiEntity = Yu.Data.Entities.Right.Api;
using ElementEntity = Yu.Data.Entities.Right.Element;

namespace Yu.Service.WebAdmin.Role
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<BaseIdentityRole> _roleManager;

        private readonly IRepository<ElementEntity, Guid> _elementRepository;

        private readonly IRepository<ElementTree, Guid> _elementTreeRepository;

        private readonly IRepository<ElementApi, Guid> _elementApiRepository;

        private readonly IRepository<ApiEntity, Guid> _apiRepository;

        private readonly IRepository<RuleGroup, Guid> _ruleGroupRespository;

        private readonly IMemoryCache _memoryCache;

        public RoleService(RoleManager<BaseIdentityRole> roleManager,
            IRepository<ElementEntity, Guid> elementRepository,
            IRepository<ElementTree, Guid> elementTreeRepository,
            IRepository<ElementApi, Guid> elementApiRepository,
            IRepository<ApiEntity, Guid> apiRepository,
            IRepository<RuleGroup, Guid> ruleGroupRespository,
            IMemoryCache memoryCache)
        {
            _roleManager = roleManager;
            _elementRepository = elementRepository;
            _elementTreeRepository = elementTreeRepository;
            _elementApiRepository = elementApiRepository;
            _apiRepository = apiRepository;
            _ruleGroupRespository = ruleGroupRespository;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="role">角色</param>
        public async Task<bool> AddRole(RoleDetail role)
        {
            var identityRole = new BaseIdentityRole
            {
                Name = role.Name,
                Describe = role.Describe
            };

            // 保存角色
            var result = await _roleManager.CreateAsync(identityRole);
            if (result.Succeeded)
            {
                // 保存关联页面元素
                if (role.Elements != null)
                {
                    var elements = new List<string>();
                    foreach (var element in role.Elements)
                    {
                        // 找到所有的父元素
                        var ancestorElements = _elementTreeRepository.GetByWhereNoTracking(e => e.Descendant == Guid.Parse(element));
                        elements.AddRange(ancestorElements.Select(e => e.Ancestor.ToString()));

                        // 找到所有的子元素
                        var descendantElemnts = _elementTreeRepository.GetByWhereNoTracking(e => e.Ancestor == Guid.Parse(element));
                        elements.AddRange(descendantElemnts.Select(e => e.Descendant.ToString()));
                    }
                    elements = elements.Distinct().ToList();
                    foreach (var element in elements)
                    {
                        await _roleManager.AddClaimAsync(identityRole, new Claim(CustomClaimTypes.Element, element));
                    }
                    foreach (var element in role.Elements)
                    {
                        await _roleManager.AddClaimAsync(identityRole, new Claim(CustomClaimTypes.DisPlayElement, element));
                    }
                }

                // 保存关联数据规则
                if (role.DataRules != null)
                {
                    foreach (var rule in role.DataRules)
                    {
                        await _roleManager.AddClaimAsync(identityRole, new Claim(CustomClaimTypes.Rule, rule));
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">角色id</param>
        public async Task DeleteRole(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            var claims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in claims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }
            await _roleManager.DeleteAsync(role);
        }

        /// <summary>
        /// 取得全部角色名
        /// </summary>
        public string[] GetAllRoleNames()
        {
            return _roleManager.Roles.Select(r => r.Name).ToArray();
        }

        /// <summary>
        /// 取得角色概要
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="searchText">搜索内容</param>
        public PagedData<RoleOutline> GetRole(int pageIndex, int pageSize, string searchText)
        {
            // 生成表达式组
            var tupleList = new List<(string, object, ExpressionType)>
            {
                ("Name",searchText,ExpressionType.StringContain),
                ("Describe",searchText,ExpressionType.StringContain)
            };

            // 表达式组
            var group = new ExpressionGroup<BaseIdentityRole>(
                tupleList: tupleList,
                expressionCombineType: ExpressionCombineType.Or,
                expressionGroupsList: null);

            var filter = group.GetLambda();

            // 分页取得用户
            var skip = pageSize * (pageIndex - 1);
            var roles = _roleManager.Roles.Where(filter).Skip(skip).Take(pageSize);

            // 生成结果
            return new PagedData<RoleOutline>
            {
                Total = _roleManager.Roles.Where(filter).Count(),
                Data = Mapper.Map<List<RoleOutline>>(roles)
            };
        }

        /// <summary>
        /// 取得角色
        /// </summary>
        /// <param name="id">角色id</param>
        public async Task<RoleDetail> GetRole(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            var roleDetail = Mapper.Map<RoleDetail>(role);
            var claims = await _roleManager.GetClaimsAsync(role);
            roleDetail.Elements = claims.Where(c => c.Type == CustomClaimTypes.DisPlayElement).Select(c => c.Value).ToArray();
            roleDetail.DataRules = claims.Where(c => c.Type == CustomClaimTypes.Rule).Select(c => c.Value).ToArray();
            return roleDetail;
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="role">角色</param>
        public async Task UpdateRole(RoleDetail role)
        {
            // 更新角色
            var identityRole = await _roleManager.FindByIdAsync(role.Id.ToString());
            identityRole.Name = role.Name;
            identityRole.Describe = role.Describe;
            await _roleManager.UpdateAsync(identityRole);

            // 找到关联元素
            var elements = new List<string>();
            foreach (var element in role.Elements)
            {
                var ancestorElements = _elementTreeRepository.GetByWhereNoTracking(e => e.Descendant == Guid.Parse(element));
                elements.AddRange(ancestorElements.Select(e => e.Ancestor.ToString()));
                var descendantElemnts = _elementTreeRepository.GetByWhereNoTracking(e => e.Ancestor == Guid.Parse(element));
                elements.AddRange(descendantElemnts.Select(e => e.Descendant.ToString()));
            }
            elements = elements.Distinct().ToList();

            // 更新声明
            await UpdateRoleClaim(identityRole, elements.ToArray(), CustomClaimTypes.Element);
            await UpdateRoleClaim(identityRole, role.Elements.ToArray(), CustomClaimTypes.DisPlayElement);
            await UpdateRoleClaim(identityRole, role.DataRules, CustomClaimTypes.Rule);

            // 更新缓存
            await UpdateRolePermissionCache(identityRole.Name);
        }

        /// <summary>
        /// 更新声明
        /// </summary>
        /// <param name="identityRole"></param>
        /// <param name="claimValues"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        private async Task UpdateRoleClaim(BaseIdentityRole identityRole, string[] claimValues, string claimType)
        {
            // 取得全部声明
            var claims = await _roleManager.GetClaimsAsync(identityRole);

            // 找出当前类型的声明
            var elementClaims = claims.Where(c => c.Type == claimType);

            // 找到需要删除的声明
            var deleteClaims = elementClaims.Where(e => !claimValues.Contains(e.Value));

            // 找到需要添加声明的值
            var addClaimValues = claimValues.Where(e => !elementClaims.Select(c => c.Value).Contains(e));

            // 执行操作
            foreach (var claim in deleteClaims)
            {
                await _roleManager.RemoveClaimAsync(identityRole, claim);
            }
            foreach (var value in addClaimValues)
            {
                await _roleManager.AddClaimAsync(identityRole, new Claim(claimType, value));
            }
        }


        /// <summary>
        /// 取得角色拥有的所有权限
        /// </summary>
        /// <param name="roleName">角色名称</param>
        public async Task<List<(string, string)>> GetRolePermission(string roleName)
        {
            if (!CommonConstants.SystemManagerRole.Equals(roleName))
            {
                // 判断是否在缓存中
                return await _memoryCache.GetOrCreateAsync(
                    CommonConstants.RoleMemoryCacheKey + roleName,
                    async entity =>
                    {

                        var result = new List<(string, string)>();

                        // 获取所有claim
                        var role = await _roleManager.FindByNameAsync(roleName);
                        var claims = await _roleManager.GetClaimsAsync(role);

                        // 关联的元素
                        var elementIds = claims.Where(c => c.Type == CustomClaimTypes.Element).Select(c => c.Value);
                        var elements = _elementRepository.GetByWhereNoTracking(e => elementIds.Contains(e.Id.ToString()));

                        // 角色拥有的识别
                        result.Add((PermissionTypes.Identities, string.Join(',', elements?.Select(e => e.Identification))));

                        // 角色拥有路由
                        result.Add((PermissionTypes.Routes, string.Join(',', elements.Select(e => e.Route))));

                        // 获取所有关联的Api
                        var apiIds = new List<Guid>();
                        elementIds.ToList().ForEach(id =>
                        {
                            var apiids = _elementApiRepository.GetByWhereNoTracking(ea => ea.ElementId == Guid.Parse(id)).Select(ea => ea.ApiId);
                            apiIds.AddRange(apiids);
                        });
                        var apis = _apiRepository.GetByWhereNoTracking(a => apiIds.Contains(a.Id)).Select(a => a.Address + '|' + a.Type);

                        // 角色可访问的API
                        result.Add((PermissionTypes.Apis, string.Join(',', apis)));

                        // 关联的数据规则
                        var ruleIds = claims.Where(c => c.Type == CustomClaimTypes.Rule).Select(c => c.Value);
                        var ruleGroups = _ruleGroupRespository.GetByWhereNoTracking(rg => ruleIds.Contains(rg.Id.ToString()))
                            .Select(a => a.DbContext + '|' + a.Entity + '|' + a.Lambda);

                        // 角色的数据访问规则
                        result.Add((PermissionTypes.DataRules, string.Join(',', ruleGroups)));

                        // 设置缓存
                        _memoryCache.Set(CommonConstants.RoleMemoryCacheKey + roleName, result);

                        return result;
                    }
                );
            }
            else
            {
                // 系统管理员的情况
                return _memoryCache.GetOrCreate(
                    CommonConstants.RoleMemoryCacheKey + roleName,
                    entity =>
                    {
                        // 结果
                        var result = new List<(string, string)>();

                        // 关联的元素
                        var elements = _elementRepository.GetAllNoTracking();

                        // 角色拥有的识别
                        result.Add((PermissionTypes.Identities, string.Join(',', elements?.Select(e => e.Identification))));

                        // 角色拥有路由
                        result.Add((PermissionTypes.Routes, string.Join(',', elements.Select(e => e.Route))));

                        // 角色的数据访问规则
                        result.Add((PermissionTypes.DataRules, string.Empty));

                        // Api权限
                        result.Add((PermissionTypes.Apis, string.Join(',', _apiRepository.GetAllNoTracking().Select(a => a.Address + '|' + a.Type))));

                        // 设置缓存
                        _memoryCache.Set(CommonConstants.RoleMemoryCacheKey + roleName, result);

                        return result;
                    });
            }
        }

        /// <summary>
        /// 更新角色拥有的所有权限的缓存
        /// </summary>
        /// <param name="roleName">角色名称</param>
        public async Task UpdateRolePermissionCache(string roleName)
        {
            _memoryCache.Remove(CommonConstants.RoleMemoryCacheKey + roleName);
            await GetRolePermission(roleName);
        }
    }
}
