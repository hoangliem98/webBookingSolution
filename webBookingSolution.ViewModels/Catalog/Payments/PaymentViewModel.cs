using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.ViewModels.Catalog.Payments
{
    public class PaymentViewModel
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrganizationDate { get; set; }
        public string EmployeeName { get; set; }
        public decimal TempPrice { get; set; }
        public decimal DelayPrice { get; set; }
        public string DelayContent { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
