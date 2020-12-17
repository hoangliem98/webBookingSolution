using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.ViewModels.Catalog.Menus
{
    public class MenuCreateRequest
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public decimal Price { get; set; }
        public IFormFile Image { get; set; }
    }
}
