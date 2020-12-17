using System;
using System.Collections.Generic;
using System.Text;
using webBookingSolution.ViewModels.Catalog.HallImages;

namespace webBookingSolution.ViewModels.Catalog.Halls
{
    public class HallViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinimumTables { get; set; }
        public int MaximumTables { get; set; }
        public decimal Price { get; set; }
        public List<HallImageViewModel> ListImage { get; set; }
    }
}
