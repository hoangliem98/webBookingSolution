using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using webBookingSolution.Application.Catalog.Books;
using webBookingSolution.Data.EF;
using webBookingSolution.Data.Entities;
using webBookingSolution.ViewModels.Catalog.Payments;
using Microsoft.EntityFrameworkCore;

namespace webBookingSolution.Application.Catalog.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly IBookService _bookService;
        private readonly BookingDBContext _context;

        public PaymentService(IBookService bookService, BookingDBContext context)
        {
            _bookService = bookService;
            _context = context;
        }

        public async Task<int> Create(PaymentCreateRequest request)
        {
            var book = await _bookService.GetById(request.BookId);
            var payment = new Payment()
            {
                BookId = request.BookId,
                EmployeeId = request.EmployeeId,
                TempPrice = request.TempPrice,
                PaymentDate = DateTime.Now,
            };
            decimal delay = (DateTime.Now - book.OrganizationDate).Days;
            if (delay > 0)
            {
                payment.DelayPrice = payment.TempPrice * delay / 100;
                payment.TotalPrice = payment.TempPrice + payment.DelayPrice;
                payment.DelayContent = "Trễ " + delay + " ngày";
            }
            else
            {
                payment.DelayContent = "";
                payment.DelayPrice = 0;
                payment.TotalPrice = payment.TempPrice;
            }
            _context.Payments.Add(payment);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<PaymentViewModel>> GetAll()
        {
            var query = from p in _context.Payments
                        join e in _context.Employees on p.EmployeeId equals e.Id
                        join b in _context.Books on p.BookId equals b.Id
                        join c in _context.Customers on b.CustomerId equals c.Id
                        select new { p, c, e, b };

            var paymentViewModel = await query.Select(x => new PaymentViewModel()
            {
                Id = x.p.Id,
                BookId = x.p.BookId,
                CustomerName = x.c.FirstName + " " + x.c.LastName,
                EmployeeName = x.e.FirstName + " " + x.e.LastName,
                TempPrice = x.p.TempPrice,
                DelayPrice = x.p.DelayPrice,
                DelayContent = x.p.DelayContent,
                TotalPrice = x.p.TotalPrice,
                OrganizationDate = x.b.OrganizationDate,
                PaymentDate = x.p.PaymentDate
            }).ToListAsync();

            return paymentViewModel;
        }
    }
}
