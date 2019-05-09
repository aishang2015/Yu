using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Yu.Core.Captcha;

namespace Yu.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CaptchaController : ControllerBase
    {
        private readonly CaptchaHelper _captchaHelper;

        public CaptchaController(CaptchaHelper captchaHelper)
        {
            _captchaHelper = captchaHelper;
        }

        /// <summary>
        /// 取得验证码图片
        /// </summary>
        /// <returns>验证码图片</returns>
        [HttpGet]
        public IActionResult GetCaptcha()
        {            
            var code = _captchaHelper.GetValidateCode();
            var stream = _captchaHelper.CreateImageStream(code);
            HttpContext.Session.SetString("ValidateCode", code);
            return File(stream.ToArray(), @"image/png");
        }
    }
}