using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webBookingSolution.Application.Catalog.Books;
using webBookingSolution.Application.Catalog.Customers;
using webBookingSolution.ViewModels.Catalog.Books;

namespace webBookingSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ICustomerService _customerService;
        public BookController(IBookService bookService, ICustomerService customerService)
        {
            _customerService = customerService;
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var books = await _bookService.GetAll();
            return Ok(books);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] BookCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (request.customerCreateRequest != null && request.CustomerId == 0)
            {
                var customerId = await _customerService.Create(request.customerCreateRequest);
                request.CustomerId = customerId;
                if (customerId == 0)
                {
                    return BadRequest("Khách hàng đã tồn tại");
                }
            }
            var book = await _bookService.Create(request);
            if (book == -request.CustomerId)
                return BadRequest(book);
            else if (book == 0)
                return BadRequest(0);
            else
                return Ok(book);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var books = await _bookService.GetById(id);
            if (books == null)
                BadRequest("Không tìm đơn hàng");
            return Ok(books);
        }
    }
}
