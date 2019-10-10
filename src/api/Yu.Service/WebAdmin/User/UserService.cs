using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yu.Core.Constants;
using Yu.Core.Expressions;
using Yu.Core.FileManage;
using Yu.Core.Jwt;
using Yu.Data.Entities;
using Yu.Data.Infrasturctures.BaseIdentity;
using Yu.Data.Repositories;
using Yu.Model.WebAdmin.User.OutputModels;
using Yu.Service.WebAdmin.Group;
using Yu.Service.WebAdmin.Positions;
using GroupEntity = Yu.Data.Entities.Right.Group;

namespace Yu.Service.WebAdmin.User
{
    public class UserService : IUserService
    {
        private UserManager<BaseIdentityUser> _userManager;

        private IGroupService _groupService;

        private IPositionService _positionService;

        private IFileStore _fileStore;

        private readonly IJwtFactory _jwtFactory;

        private string _serverFileRootPath;

        private readonly IMapper _mapper;

        public UserService(
            UserManager<BaseIdentityUser> userManager,
            IGroupService groupService,
            IPositionService positionService,
            IJwtFactory jwtFactory,
            IFileStore fileStore,
            IConfiguration configuration,
            IMapper mapper)
        {
            _userManager = userManager;
            _positionService = positionService;
            _groupService = groupService;
            _jwtFactory = jwtFactory;
            _fileStore = fileStore;
            _serverFileRootPath = configuration["AvatarFileOption:ServerFileStorePath"];
            _mapper = mapper;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        public async Task<bool> AddUserAsync(UserDetail userDetail)
        {
            var user = _mapper.Map<BaseIdentityUser>(userDetail);
            var result = await _userManager.CreateAsync(user, CommonConstants.Password);

            if (!result.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return false;
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
        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            await _userManager.DeleteAsync(user);

            // 清除用户登录缓存，强制用户下线
            _jwtFactory.RemoveToken(user.UserName);
        }

        /// <summary>
        /// 取得用户详细数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户数据</returns>
        public async Task<UserDetail> GetUserDetailAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            
            // 组织名称
            user.GroupName = string.IsNullOrEmpty(user.UserGroupId) ? string.Empty : _groupService.GetGroupNameById(Guid.Parse(user.UserGroupId));

            // 岗位名称
            user.PositionName = string.IsNullOrEmpty(user.PositionId) ? string.Empty : _positionService.GetPositionNameById(Guid.Parse(user.PositionId));

            var result = _mapper.Map<UserDetail>(user);

            // 角色名称
            var roles = await _userManager.GetRolesAsync(user);
            result.Roles = roles.ToArray();

            // 返回结果
            return result;
        }


        /// <summary>
        /// 取得用户详细数据
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>用户数据</returns>
        public async Task<UserDetail> GetUserDetailAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return await GetUserDetailAsync(user.Id);
        }

        /// <summary>
        /// 取得用户概要数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns>用户数据</returns>
        public async Task<PagedData<UserOutline>> GetUserOutlinesAsync(int pageIndex, int pageSize, string searchText)
        {
            // 生成表达式组
            var tupleList = new List<(string, object, ExpressionType)>
            {
                ("UserName",searchText,ExpressionType.StringContain),
                ("PhoneNumber",searchText,ExpressionType.StringContain),
                ("Email",searchText,ExpressionType.StringContain),
            };

            // 表达式组
            var group = new ExpressionGroup<BaseIdentityUser>(
                tupleList: tupleList,
                expressionCombineType: ExpressionCombineType.Or,
                expressionGroupsList: null);

            var filter = group.GetLambda();

            // 分页取得用户
            var skip = pageSize * (pageIndex - 1);
            var users = await _userManager.Users.Where(filter).Skip(skip).Take(pageSize).ToListAsync();

            users.ForEach(user =>
            {
                user.GroupName = string.IsNullOrEmpty(user.UserGroupId) ? string.Empty : _groupService.GetGroupNameById(Guid.Parse(user.UserGroupId));
                user.PositionName = string.IsNullOrEmpty(user.PositionId) ? string.Empty : _positionService.GetPositionNameById(Guid.Parse(user.PositionId));
            });

            // 结果数据
            var userOutlines = _mapper.Map<List<UserOutline>>(users);

            // 生成结果
            return new PagedData<UserOutline>
            {
                Total = _userManager.Users.Where(filter).Count(),
                Data = userOutlines
            };
        }

        /// <summary>
        /// 取得用户概要数据
        /// </summary>
        /// <param name="ids">id</param>
        /// <returns>用户数据</returns>
        public async Task<List<UserOutline>> GetUserOutlinesByIds(List<Guid> ids)
        {
            var users = await _userManager.Users.Where(u=> ids.Contains(u.Id)).ToListAsync();
            users.ForEach(user =>
            {
                user.GroupName = string.IsNullOrEmpty(user.UserGroupId) ? string.Empty : _groupService.GetGroupNameById(Guid.Parse(user.UserGroupId));
                user.PositionName = string.IsNullOrEmpty(user.PositionId) ? string.Empty : _positionService.GetPositionNameById(Guid.Parse(user.PositionId));
            });

            // 结果数据
            var userOutlines = _mapper.Map<List<UserOutline>>(users);
            return userOutlines;
        }

        /// <summary>
        /// 更新用户头像
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="formFile">表单头像文件</param>
        /// <returns></returns>
        public async Task<string> UpdateUserAvatarAsync(Guid userId, IFormFile formFile)
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
            return newName;
        }

        /// <summary>
        /// 更新用户头像
        /// </summary>
        /// <param name="useName">用户名</param>
        /// <param name="formFile">表单头像文件</param>
        /// <returns></returns>
        public async Task<string> UpdateUserAvatarAsync(string userName, IFormFile formFile)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return await UpdateUserAvatarAsync(user.Id, formFile);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userDetail">用户信息</param>
        public async Task<bool> UpdateUserDetailAsync(UserDetail userDetail)
        {
            var user = await _userManager.FindByIdAsync(userDetail.Id.ToString());
            _mapper.Map(userDetail, user);

            // 先删除再添加角色 
            // RemoveFromRolesAsync和AddToRolesAsync内部会保存用户信息
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            if (userDetail.Roles.Length > 0)
            {
                await _userManager.AddToRolesAsync(user, userDetail.Roles);
            }

            // 清除用户登录缓存，强制用户下线
            _jwtFactory.RemoveToken(user.UserName);

            return true;
        }

        /// <summary>
        /// 取得指定用户的角色
        /// </summary>
        public async Task<List<string>> GetUserRolesAsync(BaseIdentityUser baseIdentityUser)
        {
            var result = await _userManager.GetRolesAsync(baseIdentityUser);
            return result.ToList();
        }
    }
}
