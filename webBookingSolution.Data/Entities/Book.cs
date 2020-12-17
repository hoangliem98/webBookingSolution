using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.Data.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public int NumberTables { get; set; }
        public DateTime OrganizationDate { get; set; }
        public DateTime BookDate { get; set; }
        public decimal Price { get; set; }
        public string Season { get; set; }
        public string Status { get; set; }
        public List<ServiceInBook> ServiceInBooks { get; set; }
        public int CustomerId { get; set; }
        public int HallId { get; set; }
        public int MenuId { get; set; }

        public Customer Customer { get; set; }
        public Hall Hall { get; set; }
        public Menu Menu { get; set; }
        public List<Payment> Payments { get; set; }
    }
}
