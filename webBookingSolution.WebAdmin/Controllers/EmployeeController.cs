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
using webBookingSolution.ViewModels.Catalog.Employees;
using webBookingSolution.Api.Employees;
using static webBookingSolution.WebAdmin.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace webBookingSolution.WebAdmin.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeApiClient _employeeApiClient;
        private readonly ICompositeViewEngine _viewEngine;

        public EmployeeController(IEmployeeApiClient employeeApiClient, ICompositeViewEngine viewEngine)
        {
            _viewEngine = viewEngine;
            _employeeApiClient = employeeApiClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List(int month)
        {
            var data = await _employeeApiClient.GetAll(month);
            var months = await _employeeApiClient.GetListMonth();
            ViewBag.Months = months.Select(x => new SelectListItem()
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
        public async Task<IActionResult> Update(int id)
        {
            var employee = await _employeeApiClient.GetById(id);
            if (employee == null)
                return RedirectToAction("ErrorPage", "Home");
            var employeeViewModel = new EmployeeUpdateRequest()
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DOB = employee.DOB,
                ImagePath = employee.Image,
                PhoneNumber = employee.PhoneNumber,
                Address = employee.Address,
                Email = employee.Email
            };
            return View(employeeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(EmployeeUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return await Update(request.Id);

            var result = await _employeeApiClient.Update(request);
            if (result == "66")
            {

                ModelState.AddModelError("", "Nhân viên đã tồn tại");
                return await Update(request.Id);
            }
            else
            {
                TempData["result"] = "Sửa thông tin nhân viên thành công";
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EmployeeDeleteRequest request)
        {
            var employee = _employeeApiClient.GetById(request.Id);
            var loginUser = User.FindFirst("UserId")?.Value;

            if (employee == null)
                return RedirectToAction("ErrorPage", "Home");

            await _employeeApiClient.Delete(request);

            if (request.Id == int.Parse(loginUser))
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "User");
            }

            var data = await _employeeApiClient.GetAll(0);
            return Json(new { html = RenderRazorViewToString(this, "DataTable", data) });
        }
    }
}
