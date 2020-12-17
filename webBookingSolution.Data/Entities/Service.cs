using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.Data.Entities
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public List<ServiceInBook> ServiceInBooks { get; set; }
    }
}
