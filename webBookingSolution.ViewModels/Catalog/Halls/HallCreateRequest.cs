using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.ViewModels.Catalog.Halls
{
    public class HallCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinimumTables { get; set; }
        public int MaximumTables { get; set; }
        public decimal Price { get; set; }
        public List<IFormFile> ThumbnailImage { get; set; }
    }
}
