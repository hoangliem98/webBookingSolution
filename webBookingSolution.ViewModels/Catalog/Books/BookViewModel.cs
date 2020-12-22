using System;
using System.Collections.Generic;
using System.Text;
using webBookingSolution.ViewModels.Catalog.Customers;
using webBookingSolution.ViewModels.Catalog.Services;

namespace webBookingSolution.ViewModels.Catalog.Books
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public int NumberTables { get; set; }
        public DateTime OrganizationDate { get; set; }
        public DateTime BookDate { get; set; }
        public decimal Price { get; set; }
        public string Season { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public CustomerCreateRequest Customer { get; set; }
        public int HallId { get; set; }
        public string HallName { get; set; }
        public decimal HallPrice { get; set; }
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public decimal MenuPrice { get; set; }
        public List<ServiceViewModel> Service { get; set; }
    }
}
