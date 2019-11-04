
// api地址常量
export class UriConstant {

    // 服务应用地址
    static readonly ServerUri: string = 'https://localhost:5001/';

    // api地址
    static readonly BaseApiUri: string = 'https://localhost:5001/api/';

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

    // 用户概要数据地址
    static readonly AssignOutlineUri = UriConstant.BaseApiUri + 'assignOutline';

    // 用户概要数据地址
    static readonly PositionOutlineUri = UriConstant.BaseApiUri + 'positionOutline';

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

    // 组织数据地址
    static readonly PositionUri = UriConstant.BaseApiUri + 'position';

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

    // 工作流类型
    static readonly WorkFlowTypeUri = UriConstant.BaseApiUri + 'workflowType';

    // 工作流定义
    static readonly WorkFlowDefineUri = UriConstant.BaseApiUri + 'workflowDefine';

    // 工作流定义
    static readonly AllWorkFlowDefineUri = UriConstant.BaseApiUri + 'allWorkflowDefines';

    // 工作流流程图
    static readonly WorkFlowFlowUri = UriConstant.BaseApiUri + 'workflowFlow';

    // 工作流表单
    static readonly WorkFlowFormUri = UriConstant.BaseApiUri + 'workflowForm';

    // 工作流表单元素
    static readonly WorkFlowFormElementUri = UriConstant.BaseApiUri + 'workflowFormElement';

    // 工作流实例
    static readonly WorkFlowInstanceUri = UriConstant.BaseApiUri + 'workflowInstance';
    
    // 待办工作流实例
    static readonly HandleWorkFlowInstanceUri = UriConstant.BaseApiUri + 'handleWorkflowInstance';

    // 工作流实例表单数据
    static readonly WorkFlowInstanceFormUri = UriConstant.BaseApiUri + 'workflowInstanceForm';

    // 工作流实例表单文件
    static readonly WorkFlowInstanceFormFileUri = UriConstant.BaseApiUri + 'workFlowInstanceFormFile';

    // 回收站工作流实例
    static readonly DeletedWorkFlowInstanceUri = UriConstant.BaseApiUri + 'deltetedWorkflowInstance';

    // 工作流实例节点处理数据
    static readonly WorkFlowInstanceNodeUri = UriConstant.BaseApiUri + 'workflowInstanceNode';

    // 工作流节点元素
    static readonly WorkFlowNodeElement = UriConstant.BaseApiUri + 'workflowNodeElement';

}