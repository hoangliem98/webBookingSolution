using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.ViewModels.Catalog.Users
{
    public class ChangePasswordRequest
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string CurrentPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int Roles { get; set; }
        public int EmOrCusId { get; set; }
    }
}
