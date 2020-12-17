using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webBookingSolution.Api.Books;
using webBookingSolution.ViewModels.Catalog.Books;

namespace webBookingSolution.WebAdmin.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly IBookApiClient _bookApiClient;

        public BookController(IBookApiClient bookApiClient)
        {
            _bookApiClient = bookApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> List(BookBigViewModel tuple)
        {
            var data = await _bookApiClient.GetAll();
            tuple.bookViewModels = data;
            if (TempData["result"] != null)
                ViewBag.SuccessMsg = TempData["result"];
            return View(tuple);
        }
    }
}
