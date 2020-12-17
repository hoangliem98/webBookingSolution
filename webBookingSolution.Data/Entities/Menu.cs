using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.Data.Entities
{
    public class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }

        public List<Book> Books { get; set; }
    }
}
