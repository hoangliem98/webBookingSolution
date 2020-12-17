using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.ViewModels.System
{
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Roles { get; set; }
        public bool RememberMe { get; set; }
    }
}
