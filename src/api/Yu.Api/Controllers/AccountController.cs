using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Yu.Core.Mvc;

namespace Yu.Api.Controllers
{
    [Description("账户管理")]
    public class AccountController : AnonymousController
    {

        [HttpPost]
        public IActionResult Login()
        {
            return Ok();
        }

    }
}