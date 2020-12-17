using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.ViewModels.Catalog.HallImages
{
    public class HallImageCreateRequest
    {
        public string Caption { get; set; }
        public bool IsDefault { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}
