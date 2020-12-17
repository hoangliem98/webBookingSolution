using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.Data.Entities
{
    public class HallImage
    {
        public int Id { get; set; }
        public int HallId { get; set; }
        public string Path { get; set; }
        public string Caption { get; set; }
        public bool IsDefault { get; set; }
        public DateTime DateCreated { get; set; }
        public long Size { get; set; }

        public Hall Hall { get; set; }
    }
}
