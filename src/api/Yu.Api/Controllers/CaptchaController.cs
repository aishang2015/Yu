using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.ComponentModel;
using Yu.Core.Captcha;
using Yu.Core.Constants;
using Yu.Core.Mvc;

namespace Yu.Api.Controllers
{

    [Route("api/[controller]")]
    [Description("验证码")]
    public class CaptchaController : AnonymousController
    {

        private readonly CaptchaHelper _captchaHelper;

        private readonly IMemoryCache _memoryCache;

        public CaptchaController(CaptchaHelper captchaHelper, IMemoryCache memoryCache)
        {
            _captchaHelper = captchaHelper;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 取得验证码图片
        /// </summary>
        /// <returns>验证码图片</returns>
        [HttpGet]
        [Description("获取验证码")]
        public IActionResult GetCaptcha()
        {
            // 验证码的值
            var code = _captchaHelper.GetValidateCode();

            // 生成验证码图片流
            var stream = _captchaHelper.CreateImageStream(code);

            // 保存到缓存
            // todo 整理缓存服务，配置化过期时间
            var codeId = Guid.NewGuid().ToString();
            _memoryCache.Set(codeId, code, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(2)));

            // 将id保存到header返回客户端
            Response.Headers.Add(CommonConstants.CaptchaCodeId, codeId.ToString());
            return File(stream.ToArray(), @"image/png");
        }
    }
}