using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.ViewModels.Catalog.HallImages
{
    public class HallImageUpdateRequest
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public bool IsDefault { get; set; } 

        public IFormFile ImageFile { get; set; }
    }
}
