using System;
using System.Collections.Generic;
using System.Text;
using webBookingSolution.ViewModels.Catalog.Customers;
using webBookingSolution.ViewModels.Common;

namespace webBookingSolution.ViewModels.Catalog.Books
{
    public class BookCreateRequest
    {
        public int NumberTables { get; set; }
        public DateTime OrganizationDate { get; set; }
        public DateTime BookDate { get; set; }
        public decimal Price { get; set; }
        public string Season { get; set; }
        public string Status { get; set; }
        public CustomerCreateRequest customerCreateRequest { get; set; }
        public int CustomerId { get; set; }
        public int HallId { get; set; }
        public int MenuId { get; set; }
        public List<CheckBoxItem> Service { get; set; }
        public List<int> ServiceId { get; set; }
    }
}
