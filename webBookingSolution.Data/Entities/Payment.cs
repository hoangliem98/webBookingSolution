using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.Data.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int EmployeeId { get; set; }
        public decimal TempPrice { get; set; }
        public decimal DelayPrice { get; set; }
        public string DelayContent { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime PaymentDate { get; set; }

        public Employee Employee { get; set; }
        public Book Book { get; set; }
    }
}
