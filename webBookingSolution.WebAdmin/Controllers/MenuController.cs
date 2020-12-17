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
using webBookingSolution.ViewModels.Catalog.Menus;
using webBookingSolution.Api.Menus;
using static webBookingSolution.WebAdmin.Helper;

namespace webBookingSolution.WebAdmin.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        private readonly IMenuApiClient _menuApiClient;
        private readonly ICompositeViewEngine _viewEngine;

        public MenuController(IMenuApiClient menuApiClient, ICompositeViewEngine viewEngine)
        {
            _viewEngine = viewEngine;
            _menuApiClient = menuApiClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List()
        {
            var data = await _menuApiClient.GetAll();
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
        public async Task<IActionResult> Create(MenuCreateRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _menuApiClient.Create(request);
            if (result)
            {
                TempData["result"] = "Thêm thực đơn thành công";
                return RedirectToAction("List");
            }
            ModelState.AddModelError("", "Thêm thực đơn thất bại");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var menu = await _menuApiClient.GetById(id);
            if (menu == null)
                return RedirectToAction("ErrorPage", "Home");
            var menuViewModel = new MenuUpdateRequest()
            {
                Id = menu.Id,
                Name = menu.Name,
                Content = menu.Content,
                Price = menu.Price,
                ImagePath = menu.Image
            };
            return View(menuViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(MenuUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return await Update(request.Id);

            var result = await _menuApiClient.Update(request);
            if (result)
            {
                TempData["result"] = "Cập nhật thực đơn thành công";
                return RedirectToAction("List");
            }
            ModelState.AddModelError("", "Cập nhật thực đơn thất bại");
            return await Update(request.Id);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(MenuDeleteRequest request)
        {
            await _menuApiClient.Delete(request);
            var data = await _menuApiClient.GetAll();
            return Json(new { html = RenderRazorViewToString(this, "DataTable", data) });
        }
    }
}
