using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace webBookingSolution.Data.Entities
{ 
    public class Account
    {
        public int Id {get; set;} 
        public string UserName {get; set;}
        public string Password {get; set;}
        public int Roles {get; set;}

        public Customer Customer { get; set; }
        public Employee Employee { get; set; }
    }
}
