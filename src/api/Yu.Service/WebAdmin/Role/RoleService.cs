using AutoMapper;
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
using Yu.Data.Infrasturctures.BaseIdentity;
using Yu.Data.Infrasturctures.BaseIdentity.Pemission;
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

        private readonly IPermissionCacheService _permissionCacheService;

        private readonly IMemoryCache _memoryCache;

        private readonly IMapper _mapper;

        public RoleService(RoleManager<BaseIdentityRole> roleManager,
            IRepository<ElementEntity, Guid> elementRepository,
            IRepository<ElementTree, Guid> elementTreeRepository,
            IRepository<ElementApi, Guid> elementApiRepository,
            IRepository<ApiEntity, Guid> apiRepository,
            IPermissionCacheService permissionCacheService,
            IMemoryCache memoryCache,
            IMapper mapper)
        {
            _roleManager = roleManager;
            _elementRepository = elementRepository;
            _elementTreeRepository = elementTreeRepository;
            _elementApiRepository = elementApiRepository;
            _apiRepository = apiRepository;
            _permissionCacheService = permissionCacheService;
            _memoryCache = memoryCache;
            _mapper = mapper;
        }


        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="role">角色</param>
        public async Task<bool> AddRoleAsync(RoleDetail role)
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

                    await UpdateRoleClaimAsync(identityRole, elements.ToArray(), CustomClaimTypes.Element);
                    await UpdateRoleClaimAsync(identityRole, role.Elements.ToArray(), CustomClaimTypes.DisPlayElement);
                }

                // 保存关联数据规则
                if (role.DataRules != null)
                {
                    await UpdateRoleClaimAsync(identityRole, role.DataRules, CustomClaimTypes.Rule);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="role">角色</param>
        public async Task UpdateRoleAsync(RoleDetail role)
        {
            // 更新角色
            var identityRole = await _roleManager.FindByIdAsync(role.Id.ToString());
            identityRole.Name = role.Name;
            identityRole.Describe = role.Describe;
            await _roleManager.UpdateAsync(identityRole);

            // 找到关联元素
            var elementIds = new List<Guid>();
            foreach (var element in role.Elements)
            {
                var ancestorIds = from et in _elementTreeRepository.GetAllNoTracking()
                                  where et.Descendant.ToString() == element
                                  select et.Ancestor;
                elementIds.AddRange(ancestorIds);
                var descendantIds = from et in _elementTreeRepository.GetAllNoTracking()
                                    where et.Ancestor.ToString() == element
                                    select et.Descendant;
                elementIds.AddRange(descendantIds);
            }
            var ids = from id in elementIds.Distinct().ToList()
                      select id.ToString();

            // 更新声明
            await UpdateRoleClaimAsync(identityRole, ids.ToArray(), CustomClaimTypes.Element);
            await UpdateRoleClaimAsync(identityRole, role.Elements.ToArray(), CustomClaimTypes.DisPlayElement);
            await UpdateRoleClaimAsync(identityRole, role.DataRules, CustomClaimTypes.Rule);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">角色id</param>
        public async Task<bool> DeleteRoleAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (CommonConstants.SystemManagerRole.Equals(role.Name))
            {
                return false;
            }
            var claims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in claims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }
            await _roleManager.DeleteAsync(role);
            return true;
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
                Data = _mapper.Map<List<RoleOutline>>(roles)
            };
        }

        /// <summary>
        /// 取得角色
        /// </summary>
        /// <param name="id">角色id</param>
        public async Task<RoleDetail> GetRoleAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            var roleDetail = _mapper.Map<RoleDetail>(role);
            roleDetail.Elements = (await _permissionCacheService.GetRoleClaimValuesAsync(role.Name, CustomClaimTypes.DisPlayElement)).ToArray();
            roleDetail.DataRules = (await _permissionCacheService.GetRoleClaimValuesAsync(role.Name, CustomClaimTypes.Rule)).ToArray();
            return roleDetail;
        }

        /// <summary>
        /// 更新声明
        /// </summary>
        /// <param name="identityRole"></param>
        /// <param name="claimValues"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        private async Task UpdateRoleClaimAsync(BaseIdentityRole identityRole, string[] claimValues, string claimType)
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

            // 清理当前角色的权限缓存
            _permissionCacheService.ClearRoleClaimCache(identityRole.Name);
        }




    }
}