using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using webBookingSolution.Api.Books;
using webBookingSolution.Api.Customers;
using webBookingSolution.Api.Halls;
using webBookingSolution.Api.Menus;
using webBookingSolution.Api.Sv;
using webBookingSolution.Data.EF;
using webBookingSolution.ViewModels.Catalog.Books;
using webBookingSolution.ViewModels.Catalog.Customers;
using webBookingSolution.ViewModels.Catalog.Halls;
using webBookingSolution.ViewModels.Catalog.Menus;
using webBookingSolution.ViewModels.Catalog.Services;
using webBookingSolution.ViewModels.Common;

namespace webBookingSolution.Web.Controllers
{
    public class PageController : Controller
    {
        private readonly IHallApiClient _hallApiClient;
        private readonly IMenuApiClient _menuApiClient;
        private readonly IServiceApiClient _serviceApiClient;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IBookApiClient _bookApiClient;
        private readonly ICustomerApiClient _customerApiClient;
        private readonly BookingDBContext _context;

        public PageController(IHallApiClient hallApiClient, ICompositeViewEngine viewEngine, IMenuApiClient menuApiClient, IServiceApiClient serviceApiClient, IBookApiClient bookApiClient, BookingDBContext context, ICustomerApiClient customerApiClient)
        {
            _context = context;
            _bookApiClient = bookApiClient;
            _menuApiClient = menuApiClient;
            _serviceApiClient = serviceApiClient;
            _viewEngine = viewEngine;
            _hallApiClient = hallApiClient;
            _customerApiClient = customerApiClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListHall(string keyword, int pageIndex = 1, int pageSize = 6)
        {
            var request = new GetHallPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _hallApiClient.GetAllPaging(request);
            ViewBag.Keyword = keyword;
            if (TempData["result"] != null)
                ViewBag.SuccessMsg = TempData["result"];
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> HallDetail(int id)
        {
            var data = await _hallApiClient.GetById(id);
            if (data == null)
                return RedirectToAction("ErrorPage", "Home");

            var hallViewModel = new HallViewModel()
            {
                Name = data.Name,
                Description = data.Description,
                MinimumTables = data.MinimumTables,
                MaximumTables = data.MaximumTables,
                Price = data.Price,
                ListImage = data.ListImage
            };
            return View(hallViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ListMenu(string keyword, int pageIndex = 1, int pageSize = 6)
        {
            var request = new GetMenuPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _menuApiClient.GetAllPaging(request);
            ViewBag.Keyword = keyword;
            if (TempData["result"] != null)
                ViewBag.SuccessMsg = TempData["result"];
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> MenuDetail(int id)
        {
            var data = await _menuApiClient.GetById(id);
            if (data == null)
                return RedirectToAction("ErrorPage", "Home");

            var menuViewModel = new MenuViewModel()
            {
                Name = data.Name,
                Content = data.Content,
                Image = data.Image,
                Price = data.Price,
            };
            return View(menuViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ListService(string keyword, int pageIndex = 1, int pageSize = 6)
        {
            var request = new GetServicePagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var data = await _serviceApiClient.GetAllPaging(request);
            ViewBag.Keyword = keyword;
            if (TempData["result"] != null)
                ViewBag.SuccessMsg = TempData["result"];
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ServiceDetail(int id)
        {
            var data = await _serviceApiClient.GetById(id);
            if (data == null)
                return RedirectToAction("ErrorPage", "Home");

            var serviceViewModel = new ServiceViewModel()
            {
                Name = data.Name,
                Description = data.Description,
                Image = data.Image,
                Price = data.Price,
            };
            return View(serviceViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Booking()
        {
            var hall = await _hallApiClient.GetAll();
            ViewBag.Halls = hall.Select(x => new SelectListItem()
            {
                Text = x.Name + " - " + x.Price.ToString("#,###") + " đồng",
                Value = x.Id.ToString()
            });

            var menu = await _menuApiClient.GetAll();
            ViewBag.Menus = menu.Select(x => new SelectListItem()
            {
                Text = x.Name + " - " + x.Price.ToString("#,###") + " đồng",
                Value = x.Id.ToString()
            });
            ViewBag.Services = await _context.Services.Select(x => new CheckBoxItem()
            {
                Id = x.Id,
                Name = x.Name + " ( " + x.Price.ToString("#,###") + " đồng )",
                Price = x.Price,
                IsChecked = false
            }).ToListAsync();
            var bookViewModel = new BookCreateRequest()
            {
                Service = ViewBag.Services
            };
            return View(bookViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Booking(BookCreateRequest request)
        {
            if (!ModelState.IsValid)
                return await Booking();

            if (User.Identity.IsAuthenticated && User.FindFirst(ClaimTypes.Role)?.Value == "0")
            {
                request.CustomerId = int.Parse(User.FindFirst("UserId").Value);
                var customer = await _customerApiClient.GetById(request.CustomerId);
                request.customerCreateRequest = new CustomerCreateRequest()
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    DOB = customer.DOB,
                    PhoneNumber = customer.PhoneNumber,
                    Email = customer.Email,
                    Address = customer.Address,
                    Image = null,
                };
            }
            var result = await _bookApiClient.Create(request);
            if (result.Length <= 10 && result.Contains("-"))
            {
                int customerId = 0 - int.Parse(result);
                var customer = await _customerApiClient.GetById(customerId);
                if(customer.AccountId == null)
                {
                    await _customerApiClient.Delete(customerId);
                }
                ModelState.AddModelError("", "Sảnh đã được đặt vào thời gian đó vui lòng chọn ngày hoặc buổi khác");
                return await Booking();
            }
            if(result == "Khách hàng đã tồn tại")
            {
                ModelState.AddModelError("", "Khách hàng đã tồn tại");
                return await Booking();
            }
            if (result == "0" || result.Length > 10)
            {
                ModelState.AddModelError("", "Đặt sảnh thất bại");
                return await Booking();
            }
            return RedirectToAction("BookResult", "Page", new { @id = result });
        }

        [HttpGet]
        public async Task<IActionResult> BookResult(int id)
        {
            var book = await _bookApiClient.GetById(id);
            if (book == null)
                return RedirectToAction("ErrorPage", "Home");
            var bookViewModel = new BookViewModel()
            {
                Id = book.Id,
                CustomerName = book.CustomerName,
                Service = book.Service,
                NumberTables = book.NumberTables,
                OrganizationDate = book.OrganizationDate,
                BookDate = book.BookDate,
                Price = book.Price,
                Season = book.Season,
                Status = book.Status,
                HallName = book.HallName,
                HallPrice = book.HallPrice,
                MenuName = book.MenuName,
                MenuPrice = book.MenuPrice,
            };
            return View(bookViewModel);
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }
    }
}
