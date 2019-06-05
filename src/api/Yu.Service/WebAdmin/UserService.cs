using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yu.Core.Utils;
using Yu.Data.Entities;
using Yu.Data.Infrasturctures;
using Yu.Model.WebAdmin.User.OutputModels;

namespace Yu.Service.WebAdmin
{
    public class UserService : IUserService
    {
        private UserManager<BaseIdentityUser> _userManager;

        public UserService(UserManager<BaseIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// 取得用户详细数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户数据</returns>
        public async Task<UserDetail> GetUserDetail(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return Mapper.Map<UserDetail>(user);
        }

        /// <summary>
        /// 取得用户概要数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns>用户数据</returns>
        public PagedData<UserOutline> GetUserOutlines(int pageIndex, int pageSize, string searchText)
        {
            // 生成过滤器
            var expressions = ExpressionUtil<BaseIdentityUser>.GetExpressions(new List<(string, object, ExpressionType)>
            {
                ("UserName",searchText,ExpressionType.StringContain),
                ("PhoneNumber",searchText,ExpressionType.StringContain),
                ("Email",searchText,ExpressionType.StringContain),
            });
            var expression = ExpressionUtil<BaseIdentityUser>.CombinExpressions(expressions, ExpressionCombineType.Or);
            var filter = ExpressionUtil<BaseIdentityUser>.GetLambda(expression);

            // 分页取得用户
            var skip = pageSize * (pageIndex - 1);
            var users = _userManager.Users.Where(filter).Skip(skip).Take(pageSize);

            // 生成结果
            return new PagedData<UserOutline>
            {
                Total = users.Where(filter).Count(),
                Data = Mapper.Map<List<UserOutline>>(users)
            };
        }
    }
}
