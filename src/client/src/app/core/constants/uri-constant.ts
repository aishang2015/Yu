
// api地址常量
export class UriConstant {

    // 服务应用地址
    static readonly ServerUri: string = 'https://localhost:44334/';

    // api地址
    static readonly BaseApiUri: string = 'https://localhost:44334/api/';

    // 头像地址
    static readonly AvatarBaseUri: string = UriConstant.ServerUri + 'avatar/';

    // 登录地址
    static readonly LoginUri = UriConstant.BaseApiUri + 'account/login';

    // 登录地址
    static readonly RefreshTokenUri = UriConstant.BaseApiUri + 'account/refreshToken';

    // 验证码图片地址
    static readonly CaptchaUri = UriConstant.BaseApiUri + 'captcha';

    // 用户概要数据地址
    static readonly UserOutlineUri = UriConstant.BaseApiUri + 'userOutline';

    // 用户详细数据地址
    static readonly UserDetailUri = UriConstant.BaseApiUri + 'userDetail';

    // 用户头像上传地址
    static readonly ManageAvatarUri = UriConstant.BaseApiUri + 'userAvatar';

    // 用户头像上传地址
    static readonly UserAvatarUri = UriConstant.BaseApiUri + 'userInfo/avatar';

    // API数据地址
    static readonly ApiUri = UriConstant.BaseApiUri + 'api';

    // 全体API数据地址
    static readonly AllApiUri = UriConstant.BaseApiUri + 'allApi';

    // 页面元素数据地址
    static readonly ElementUri = UriConstant.BaseApiUri + 'element';

    // 组织数据地址
    static readonly GroupUri = UriConstant.BaseApiUri + 'group';

    // 实体数据地址
    static readonly EntitiesUri = UriConstant.BaseApiUri + 'entities';

    // 实体数据地址
    static readonly EntityUri = UriConstant.BaseApiUri + 'entity';

    // 规则组
    static readonly RuleGroupUri = UriConstant.BaseApiUri + 'ruleGroup';

    // 规则组内容
    static readonly RuleDetailUri = UriConstant.BaseApiUri + 'ruleDetail';

    // 角色概要数据地址
    static readonly RoleOutlineUri = UriConstant.BaseApiUri + 'roleOutline';

    // 角色详细数据地址
    static readonly RoleUri = UriConstant.BaseApiUri + 'role';

    // 全部角色名称
    static readonly RoleNameUri = UriConstant.BaseApiUri + 'roleNames';

    // 当前用户信息
    static readonly UserInfoUri = UriConstant.BaseApiUri + 'userInfo';

    // 修改当前用户密码
    static readonly UserChangePwdUri = UriConstant.BaseApiUri + 'userInfo/password';

    // 重置密码
    static readonly ResetUserPwdByPhone = UriConstant.BaseApiUri + 'account/resetPwdByPhone';
}