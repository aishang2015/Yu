
// HTTP CODE常量
export class HttpCodeConstant {

    // 成功
    static readonly Code200 : number = 200;

    // 请求错误
    static readonly Code400  : number = 400;

    // 没有授权
    static readonly Code401  : number = 401;

    // 没有授权
    static readonly Code403  : number = 403;

    // 不存在
    static readonly Code404  : number = 404;

    // 自定义状态code 代表服务器缓存的登录状态已被清除，需要再次登录才能操作。
    static readonly Code470  : number = 470;

    // 服务器错误
    static readonly Code500  : number = 500;
}