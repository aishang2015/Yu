
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
    static readonly UserAvatarUri = UriConstant.BaseApiUri + 'userAvatar';
    
    // API数据地址
    static readonly ApiUri = UriConstant.BaseApiUri + 'api';

    // 页面元素数据地址
    static readonly ElementUri = UriConstant.BaseApiUri + 'element';

    // 组织数据地址
    static readonly GroupUri = UriConstant.BaseApiUri + 'group';
}