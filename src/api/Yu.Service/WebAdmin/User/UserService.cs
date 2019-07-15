using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Yu.Core.Constants;
using Yu.Core.Expressions;
using Yu.Core.FileManage;
using Yu.Data.Entities;
using Yu.Data.Infrasturctures;
using Yu.Data.Repositories;
using Yu.Model.WebAdmin.User.OutputModels;
using GroupEntity = Yu.Data.Entities.Right.Group;

namespace Yu.Service.WebAdmin.User
{
    public class UserService : IUserService
    {
        private UserManager<BaseIdentityUser> _userManager;

        private RoleManager<BaseIdentityRole> _roleManager;

        private IRepository<GroupEntity, Guid> _groupRepository;

        private IFileStore _fileStore;

        private string _serverFileRootPath;

        public UserService(UserManager<BaseIdentityUser> userManager,
            RoleManager<BaseIdentityRole> roleManager,
            IRepository<GroupEntity, Guid> groupRepository,
            IFileStore fileStore, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _groupRepository = groupRepository;
            _fileStore = fileStore;
            _serverFileRootPath = configuration["AvatarFileOption:ServerFileStorePath"];
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        public async Task<bool> AddUser(UserDetail userDetail)
        {
            // 查找组织名称
            if (!string.IsNullOrEmpty(userDetail.GroupId))
            {
                var group = _groupRepository.GetById(Guid.Parse(userDetail.GroupId));
                if (group == null)
                {
                    return false;
                }
                userDetail.GroupName = group.GroupName;
            }

            var user = Mapper.Map<BaseIdentityUser>(userDetail);
            var result = await _userManager.CreateAsync(user, CommonConstants.Password);

            if (!result.Succeeded)
            {
                return false;
            }

            // 组织数据
            if (!string.IsNullOrEmpty(userDetail.GroupId))
            {
                await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Group, userDetail.GroupId.ToString()));
            }

            // 角色数据
            if (userDetail.Roles != null && userDetail.Roles.Length > 0)
            {
                await _userManager.AddToRolesAsync(user, userDetail.Roles);
            }

            return true;
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
            var result = Mapper.Map<UserDetail>(user);

            // 组织
            var claims = await _userManager.GetClaimsAsync(user);
            var groupid = claims.Where(c => c.Type == CustomClaimTypes.Group).Select(c => c.Value).FirstOrDefault();
            result.GroupId = groupid;

            // 角色
            var roles = await _userManager.GetRolesAsync(user);
            result.Roles = roles.ToArray();
            return result;
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
            var tupleList = new List<(string, object, ExpressionType)>
            {
                ("UserName",searchText,ExpressionType.StringContain),
                ("PhoneNumber",searchText,ExpressionType.StringContain),
                ("Email",searchText,ExpressionType.StringContain),
                ("Roles",searchText,ExpressionType.StringContain),
                ("GroupName",searchText,ExpressionType.StringContain),
            };

            // 表达式组
            var group = new ExpressionGroup<BaseIdentityUser>(
                tupleList: tupleList,
                expressionCombineType: ExpressionCombineType.Or,
                expressionGroupsList: null);

            var filter = group.GetLambda();

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
            await _fileStore.CreateFile(newName, _serverFileRootPath, formFile.OpenReadStream());
            var user = await _userManager.FindByIdAsync(userId.ToString());

            // 删除旧头像
            _fileStore.DeleteFile(user.Avatar, _serverFileRootPath);
            user.Avatar = newName;
            await _userManager.UpdateAsync(user);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userDetail">用户信息</param>
        public async Task<bool> UpdateUserDetail(UserDetail userDetail)
        {
            var user = await _userManager.FindByIdAsync(userDetail.Id.ToString());
            Mapper.Map(userDetail, user);

            // 更新组织信息
            var claims = await _userManager.GetClaimsAsync(user);
            await _userManager.RemoveClaimsAsync(user, claims.Where(c => c.Type == CustomClaimTypes.Group));

            // 用户组织数据
            if (!string.IsNullOrEmpty(userDetail.GroupId))
            {
                await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Group, userDetail.GroupId.ToString()));
                var group = _groupRepository.GetById(Guid.Parse(userDetail.GroupId));
                if (group == null)
                {
                    return false;
                }
                user.GroupName = group.GroupName;
                user.GroupId = group.Id;
            }
            else
            {
                user.GroupName = string.Empty;
            }

            // 先删除再添加角色
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            if (userDetail.Roles.Length > 0)
            {
                await _userManager.AddToRolesAsync(user, userDetail.Roles);
            }

            user.Roles = string.Join(',', userDetail.Roles);

            // 保存用户信息
            await _userManager.UpdateAsync(user);

            return true;
        }
    }
}
