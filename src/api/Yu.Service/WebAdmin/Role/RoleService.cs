﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Yu.Core.Constants;
using Yu.Core.Expressions;
using Yu.Data.Entities;
using Yu.Data.Infrasturctures;
using Yu.Model.WebAdmin.Role.InputOuputModels;
using Yu.Model.WebAdmin.Role.OutputModels;

namespace Yu.Service.WebAdmin.Role
{
    public class RoleService : IRoleService
    {
        private RoleManager<BaseIdentityRole> _roleManager;

        public RoleService(RoleManager<BaseIdentityRole> roleManager)
        {
            _roleManager = roleManager;
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
                foreach (var element in role.Elements)
                {
                    await _roleManager.AddClaimAsync(identityRole, new Claim(CustomClaimTypes.Element, element));
                }

                // 保存关联数据规则
                foreach (var rule in role.DataRules)
                {
                    await _roleManager.AddClaimAsync(identityRole, new Claim(CustomClaimTypes.Rule, rule));
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
            roleDetail.Elements = claims.Where(c => c.Type == CustomClaimTypes.Element).Select(c => c.Value).ToArray();
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

            // 更新声明
            await UpdateRoleClaim(identityRole, role.Elements, CustomClaimTypes.Element);
            await UpdateRoleClaim(identityRole, role.DataRules, CustomClaimTypes.Rule);
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
    }
}