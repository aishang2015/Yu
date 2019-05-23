using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Yu.Core.Mvc
{
    /// <summary>
    /// 授权访问API控制器
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AuthorizeController : ControllerBase
    {
    }
}
