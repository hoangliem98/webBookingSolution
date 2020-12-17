using System;
using System.Collections.Generic;
using System.Text;

namespace webBookingSolution.ViewModels.Common
{
    public class CheckBoxItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool IsChecked { get; set; }
    }
}
