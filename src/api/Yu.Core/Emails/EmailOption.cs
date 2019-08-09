using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Core.Emails
{
    public class EmailOption
    {
        public string UseSsl { get; set; }

        public string DefaultCredentials { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ServerPort { get; set; }

        public string ServerName { get; set; }
    }
}
