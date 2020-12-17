using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.ViewModels.Catalog.Users
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public int Roles { get; set; }
        public string Password { get; set; }
        public int EmOrCusId { get; set; }
    }
}
