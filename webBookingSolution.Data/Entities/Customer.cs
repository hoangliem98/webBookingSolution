using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.Data.Entities
{
    public class Customer
    {                   
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public string Image { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public int? AccountId { get; set; }
        public Account Account { get; set; }
        public List<Book> Books { get; set; }
    }
}
