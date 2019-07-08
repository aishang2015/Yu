﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Data.Infrasturctures
{
    public class BaseIdentityRole : IdentityRole<Guid>
    {
        public string Describe { get; set; }
    }
}
