using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.Data.Entities
{
    public class Hall
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinimumTables { get; set; }
        public int MaximumTables { get; set; }
        public decimal Price { get; set; }

        public List<Book> Books { get; set; }
        public List<HallImage> HallImages { get; set; }
    }
}
