using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Books;

namespace webBookingSolution.Api.Books
{
    public interface IBookApiClient
    {
        Task<List<BookViewModel>> GetAll();
        Task<BookViewModel> GetById(int id);
        Task<string> Create(BookCreateRequest request);
    }
}
