using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using webBookingSolution.ViewModels.Catalog.Services;
using webBookingSolution.Api.Sv;
using static webBookingSolution.WebAdmin.Helper;
using Microsoft.AspNetCore.Authorization;

namespace webBookingSolution.WebAdmin.Controllers
{
    [Authorize]
    public class ServiceController : Controller
    {
        private readonly IServiceApiClient _serviceApiClient;
        private readonly ICompositeViewEngine _viewEngine;

        public ServiceController(IServiceApiClient serviceApiClient, ICompositeViewEngine viewEngine)
        {
            _viewEngine = viewEngine;
            _serviceApiClient = serviceApiClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List()
        {
            var data = await _serviceApiClient.GetAll();
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
        public async Task<IActionResult> Create(ServiceCreateRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _serviceApiClient.Create(request);
            if (result)
            {
                TempData["result"] = "Thêm dịch vụ thành công";
                return RedirectToAction("List");
            }
            ModelState.AddModelError("", "Thêm dịch vụ thất bại");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var service = await _serviceApiClient.GetById(id);
            if (service == null)
                return RedirectToAction("ErrorPage", "Home");
            var serviceViewModel = new ServiceUpdateRequest()
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                ImagePath = service.Image
            };
            return View(serviceViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ServiceUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return await Update(request.Id);

            var result = await _serviceApiClient.Update(request);
            if (result)
            {
                TempData["result"] = "Cập nhật dịch vụ thành công";
                return RedirectToAction("List");
            }
            ModelState.AddModelError("", "Cập nhật dịch vụ thất bại");
            return await Update(request.Id);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ServiceDeleteRequest request)
        {
            await _serviceApiClient.Delete(request);
            var data = await _serviceApiClient.GetAll();
            return Json(new { html = RenderRazorViewToString(this, "DataTable", data) });
        }
    }
}
