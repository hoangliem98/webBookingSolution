using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Books;

namespace webBookingSolution.Application.Catalog.Books
{
    public interface IBookService
    {
        Task<int> Create(BookCreateRequest request);
        Task<BookViewModel> GetById(int id);
        Task<List<BookViewModel>> GetAll();
        Task<int> UpdateStatus(int id);
    }
}
