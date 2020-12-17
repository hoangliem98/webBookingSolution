using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.ViewModels.Catalog.Services
{
    public class ServiceCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public IFormFile Image { get; set; }
    }
}
