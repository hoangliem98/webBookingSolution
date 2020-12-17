using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.ViewModels.Catalog.Payments
{
    public class PaymentCreateRequest
    {
        public int BookId { get; set; }
        public int EmployeeId { get; set; }
        public decimal TempPrice { get; set; }
        public decimal DelayPrice { get; set; }
        public string DelayContent { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
