using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using Yu.Core.Mvc;

namespace Yu.Data.Infrasturctures.BaseIdentity.Mvc
{
    [Authorize("ApiPermission")]
    public class ApiAuthorizeController: AuthorizeController
    {
    }
}
