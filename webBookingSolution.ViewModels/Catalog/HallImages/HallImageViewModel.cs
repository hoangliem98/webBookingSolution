using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.ViewModels.Catalog.HallImages
{
    public class HallImageViewModel
    {
        public int Id { get; set; }
        public int HallId { get; set; }
        public string Path { get; set; }
        public string Caption { get; set; }
        public bool IsDefault { get; set; }
        public DateTime DateCreated { get; set; }
        public long Size { get; set; }
    }
}
