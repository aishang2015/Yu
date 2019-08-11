using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using Yu.Core.Mvc;

namespace Yu.Data.Infrasturctures.Mvc
{
    [Authorize("ApiPermission")]
    public class ApiAuthorizeController: AuthorizeController
    {
    }
}
