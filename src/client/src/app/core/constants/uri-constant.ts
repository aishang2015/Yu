
// api地址常量
export class UriConstant {

    // api地址
    static readonly BaseApiUri: string = 'https://localhost:44334/api/';

    // 登录地址
    static readonly LoginUri = UriConstant.BaseApiUri + 'account/login';

    // 验证码图片地址
    static readonly CaptchaUri = UriConstant.BaseApiUri + 'captcha';

    // 用户概要数据地址
    static readonly UserOutlineUri = UriConstant.BaseApiUri + 'userOutline';

    // 用户详细数据地址
    static readonly UserDetailUri = UriConstant.BaseApiUri + 'userDetail';
}