using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.ViewModels.Catalog.Menus
{
    public class MenuViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
    }
}
