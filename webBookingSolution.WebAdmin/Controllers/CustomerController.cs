using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using webBookingSolution.ViewModels.Catalog.Customers;
using webBookingSolution.Api.Customers;
using static webBookingSolution.WebAdmin.Helper;

namespace webBookingSolution.WebAdmin.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ICustomerApiClient _customerApiClient;
        private readonly ICompositeViewEngine _viewEngine;

        public CustomerController(ICustomerApiClient customerApiClient, ICompositeViewEngine viewEngine)
        {
            _viewEngine = viewEngine;
            _customerApiClient = customerApiClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ErrorPage()
        {
            return View();
        }

        public async Task<IActionResult> List(int month)
        {
            var data = await _customerApiClient.GetAll(month);
            var listMonth = new List<int>();
            for (int i = 1; i <= 12; i++)
            {
                listMonth.Add(i);
            }
            ViewBag.Months = listMonth.Select(x => new SelectListItem()
            {
                Text = "Tháng " + x.ToString(),
                Value = x.ToString(),
                Selected = month == x,
            });

            if (TempData["result"] != null)
                ViewBag.SuccessMsg = TempData["result"];
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CustomerCreateRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _customerApiClient.Create(request);
            if (result == "0")
            {
                ModelState.AddModelError("", "Khách hàng đã tồn tại");
                return View();
            }
            else
            {
                TempData["result"] = "Thêm khách hàng thành công";
                return RedirectToAction("List");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var customer = await _customerApiClient.GetById(id);
            if (customer == null)
                return RedirectToAction("ErrorPage", "Home");
            var customerViewModel = new CustomerUpdateRequest()
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                DOB = customer.DOB,
                ImagePath = customer.Image,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                Email = customer.Email
            };
            return View(customerViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(CustomerUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return await Update(request.Id);

            var result = await _customerApiClient.Update(request);
            if (result == "77")
            {

                ModelState.AddModelError("", "Khách hàng đã tồn tại");
                return await Update(request.Id);
            }
            else
            {
                TempData["result"] = "Thêm khách hàng thành công";
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _customerApiClient.Delete(id);
            var data = await _customerApiClient.GetAll(0);
            return Json(new { html = RenderRazorViewToString(this, "DataTable", data) });
        }
    }
}
