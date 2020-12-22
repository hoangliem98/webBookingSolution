using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.Application.Common;
using webBookingSolution.Data.EF;
using webBookingSolution.Data.Entities;
using webBookingSolution.ViewModels.Catalog.Books;
using System.Security.Claims;
using webBookingSolution.Application.Catalog.Halls;
using webBookingSolution.Application.Catalog.Menus;
using webBookingSolution.Application.Catalog.Customers;
using webBookingSolution.Application.Catalog.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using webBookingSolution.ViewModels.Catalog.Services;

namespace webBookingSolution.Application.Catalog.Books
{
    public class BookService : IBookService
    {
        private readonly BookingDBContext _context;
        private readonly IHallService _hallService;
        private readonly IMenuService _menuService;
        private readonly ISvService _svService;

        public BookService(BookingDBContext context, IHallService hallService, IMenuService menuService, ISvService svService)
        {
            _menuService = menuService;
            _hallService = hallService;
            _context = context;
            _svService = svService;
        }

        public async Task<bool> CheckHallContain(int hallId, DateTime ogDate, string Season)
        {
            var query = from b in _context.Books
                        where b.HallId == hallId && 
                        b.Season == Season && 
                        b.OrganizationDate.Equals(ogDate)
                        select new { b };
            int totalRow = await query.CountAsync();
            if (totalRow > 0)
                return false;
            else
                return true;
        }

        public async Task<int> Create(BookCreateRequest request)
        {
            if (!await CheckHallContain(request.HallId, request.OrganizationDate, request.Season))
                return -request.CustomerId;
            var book = new Book()
            {
                NumberTables = request.NumberTables,
                OrganizationDate = request.OrganizationDate,
                BookDate = DateTime.Now,
                Season = request.Season,
                Status = "Chưa thanh toán",
                HallId = request.HallId,
                MenuId = request.MenuId,
                CustomerId = request.CustomerId,
            };

            var hall = await _hallService.GetById(request.HallId);
            var menu = await _menuService.GetById(request.MenuId);

            decimal total = hall.Price + (menu.Price * request.NumberTables);
            if (request.ServiceId != null)
            {
                book.ServiceInBooks = new List<ServiceInBook>();
                foreach (var item in request.ServiceId)
                {
                    var service = await _svService.GetById(item.Id);
                    decimal tmp = total;
                    total = tmp + service.Price;
                    book.ServiceInBooks.Add(
                         new ServiceInBook()
                         {
                             ServiceId = item.Id
                         }
                     );
                }
                book.Price = total;
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return book.Id;
            }

            book.Price = total;
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book.Id;
        }

        public async Task<List<BookViewModel>> GetAll()
        {
            var query = from b in _context.Books
                        join c in _context.Customers on b.CustomerId equals c.Id
                        join h in _context.Halls on b.HallId equals h.Id
                        join m in _context.Menus on b.MenuId equals m.Id
                        select new { b, c, h, m };

            var book = await query.Select(x => new BookViewModel()
            {
                Id = x.b.Id,
                CustomerName = x.c.FirstName + " " + x.c.LastName,
                NumberTables = x.b.NumberTables,
                OrganizationDate = x.b.OrganizationDate,
                BookDate = x.b.BookDate,
                Price = x.b.Price,
                Season = x.b.Season,
                Status = x.b.Status,
                HallName = x.h.Name,
                HallPrice = x.h.Price,
                MenuName = x.m.Name,
                MenuPrice = x.m.Price,
            }).ToListAsync();

            var bookViewModel = new List<BookViewModel>();

            foreach(var item in book)
            {
                var service = await GetServiceById(item.Id);
                bookViewModel.Add(
                    new BookViewModel()
                    {
                        Id = item.Id,
                        Service = service,
                        CustomerName = item.CustomerName,
                        NumberTables = item.NumberTables,
                        OrganizationDate = item.OrganizationDate,
                        BookDate = item.BookDate,
                        Price = item.Price,
                        Season = item.Season,
                        Status = item.Status,
                        HallName = item.HallName,
                        HallPrice = item.HallPrice,
                        MenuName = item.MenuName,
                        MenuPrice = item.MenuPrice,
                    }
                );
            }

            return bookViewModel;
        }

        public async Task<List<ServiceViewModel>> GetServiceById(int id)
        {
            var query = from sib in _context.ServiceInBooks
                        join s in _context.Services on sib.ServiceId equals s.Id
                        where sib.BookId == id
                        select new { sib, s };
            var service = await query.Select(x => new ServiceViewModel()
            {
                Name = x.s.Name,
                Price = x.s.Price,
            }).ToListAsync();

            return service;
        }

        public async Task<BookViewModel> GetById(int id)
        {
            var book = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);
            if (book == null)
                return null;
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == book.CustomerId);
            var hall = await _context.Halls.FirstOrDefaultAsync(x => x.Id == book.HallId);
            var menu = await _context.Menus.FirstOrDefaultAsync(x => x.Id == book.MenuId);
            var query = from sib in _context.ServiceInBooks
                        join s in _context.Services on sib.ServiceId equals s.Id
                        where sib.BookId == id
                        select new { sib, s };
            if (customer == null || hall == null || menu == null)
                return null;
            var service = await query.Select(x => new ServiceViewModel()
            {
                Name = x.s.Name,
                Price = x.s.Price,
            }).ToListAsync();

            var bookViewModel = new BookViewModel()
            {
                Id = book.Id,
                CustomerName = customer.FirstName + " " + customer.LastName,
                Service = service,
                NumberTables = book.NumberTables,
                OrganizationDate = book.OrganizationDate,
                BookDate = book.BookDate,
                Price = book.Price,
                Season = book.Season,
                Status = book.Status,
                HallName = hall.Name,
                HallPrice = hall.Price,
                MenuName = menu.Name,
                MenuPrice = menu.Price,
            };
            return bookViewModel;
        }

        public async Task<int> UpdateStatus(int id)
        {
            var book = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);
            if (book == null)
                return 0;
            book.Status = "Đã thanh toán";
            return await _context.SaveChangesAsync();
        }
    }
}
