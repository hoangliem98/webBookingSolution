using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webBookingSolution.Application.Catalog.Books;
using webBookingSolution.Application.Catalog.Payments;
using webBookingSolution.ViewModels.Catalog.Payments;

namespace webBookingSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private IPaymentService _paymentService;
        private IBookService _bookService;
        public PaymentController(IPaymentService paymentService, IBookService bookService)
        {
            _bookService = bookService;
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services = await _paymentService.GetAll();
            return Ok(services);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _paymentService.Create(request);
            if (result == 0)
                return BadRequest("Lỗi");
            else
            {
                var rs = await _bookService.UpdateStatus(request.BookId);
                if (rs == 0)
                    return BadRequest("NotFound");
                return Ok(result);
            }
        }
    }
}
