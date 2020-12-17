using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.Data.Entities
{
    public class ServiceInBook
    {
        public int ServiceId { get; set; }

        public Service Service { get; set; }

        public int BookId { get; set; }

        public Book Book { get; set; }
    }
}
