using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yu.Core.FileManage;
using Yu.Core.Utils;
using Yu.Data.Entities;
using Yu.Data.Infrasturctures;
using Yu.Model.WebAdmin.User.OutputModels;

namespace Yu.Service.WebAdmin.User
{
    public class UserService : IUserService
    {
        private UserManager<BaseIdentityUser> _userManager;

        private IFileStore _fileStore;

        public UserService(UserManager<BaseIdentityUser> userManager, IFileStore fileStore)
        {
            _userManager = userManager;
            _fileStore = fileStore;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public async Task DeleteUser(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            await _userManager.DeleteAsync(user);
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
            // 生成表达式组
            var expressions = ExpressionUtil<BaseIdentityUser>.GetExpressions(new List<(string, object, ExpressionType)>
            {
                ("UserName",searchText,ExpressionType.StringContain),
                ("PhoneNumber",searchText,ExpressionType.StringContain),
                ("Email",searchText,ExpressionType.StringContain),
            });

            // 组合表达式
            var expression = ExpressionUtil<BaseIdentityUser>.CombinExpressions(expressions, ExpressionCombineType.Or);

            // 生成过滤器
            var filter = ExpressionUtil<BaseIdentityUser>.GetLambda(expression);

            // 分页取得用户
            var skip = pageSize * (pageIndex - 1);
            var users = _userManager.Users.Where(filter).Skip(skip).Take(pageSize);

            // 生成结果
            return new PagedData<UserOutline>
            {
                Total = _userManager.Users.Where(filter).Count(),
                Data = Mapper.Map<List<UserOutline>>(users)
            };
        }

        /// <summary>
        /// 更新用户头像
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="formFile">表单头像文件</param>
        /// <returns></returns>
        public async Task UpdateUserAvatar(Guid userId, IFormFile formFile)
        {
            // 生成新文件名
            var endfix = formFile.FileName.Split('.').Last();
            var newName = Path.Combine(DateTime.Now.ToString("MMddHHmmssfff") + '.' + endfix);

            // 可以替换成其他的文件保存方式，当前是直接利用静态文件目录的方式保存到服务器磁盘上。
            await _fileStore.CreateFile(newName, formFile.OpenReadStream());
            var user = await _userManager.FindByIdAsync(userId.ToString());

            // 删除旧头像
            _fileStore.DeleteFile(user.Avatar);
            user.Avatar = newName;
            await _userManager.UpdateAsync(user);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userDetail">用户信息</param>
        public async Task UpdateUserDetail(UserDetail userDetail)
        {
            var user = await _userManager.FindByIdAsync(userDetail.Id.ToString());
            Mapper.Map(userDetail, user);
            await _userManager.UpdateAsync(user);
        }
    }
}
