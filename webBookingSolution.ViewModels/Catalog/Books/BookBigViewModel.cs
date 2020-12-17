using System;
using System.Collections.Generic;
using System.Text;
using webBookingSolution.ViewModels.Catalog.Books;

namespace webBookingSolution.ViewModels.Catalog.Books
{
    public class BookBigViewModel
    {
        public List<BookViewModel> bookViewModels { get; set; }
        public BookViewModel bookViewModel { get; set; }
    }
}
