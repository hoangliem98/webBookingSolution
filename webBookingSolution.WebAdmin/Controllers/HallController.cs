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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using webBookingSolution.Data.EF;
using webBookingSolution.ViewModels.Catalog.HallImages;
using webBookingSolution.ViewModels.Catalog.Halls;
using webBookingSolution.Api.Halls;
using static webBookingSolution.WebAdmin.Helper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace webBookingSolution.WebAdmin.Controllers
{
    [Authorize]
    public class HallController : Controller
    {
        private readonly IHallApiClient _hallApiClient;
        private readonly ICompositeViewEngine _viewEngine;

        public HallController(IHallApiClient hallApiClient, ICompositeViewEngine viewEngine)
        {
            _viewEngine = viewEngine;
            _hallApiClient = hallApiClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List()
        {
            var data = await _hallApiClient.GetAll();
            if (TempData["result"] != null)
                ViewBag.SuccessMsg = TempData["result"];
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ListImageModal(int id)
        {
            var data = await _hallApiClient.GetImagesByHall(id);
            return Json(new { html = RenderRazorViewToString(this, "ListImageModal", data) });
        }

        [HttpGet]
        public async Task<IActionResult> ImageList(int id)
        {
            var data = await _hallApiClient.GetImagesByHall(id);
            var path = new List<string>();
            foreach(var item in data)
            {
                path.Add(item.Path);
            }
            return Json(new { data = path });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(HallCreateRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _hallApiClient.Create(request);
            if (result) 
            {
                TempData["result"] = "Thêm sảnh tiệc thành công";
                return RedirectToAction("List");
            }
            ModelState.AddModelError("", "Thêm sảnh tiệc thất bại");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var hall = await _hallApiClient.GetById(id);
            if (hall == null)
                return RedirectToAction("ErrorPage", "Home");
            var hallViewModel = new HallUpdateRequest()
            {
                Id = hall.Id,
                Name = hall.Name,
                Description = hall.Description,
                MinimumTables = hall.MinimumTables,
                MaximumTables = hall.MaximumTables,
                Price = hall.Price,
                Image = hall.ListImage,
            };
            return View(hallViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(HallUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return await Update(request.Id);

            var result = await _hallApiClient.Update(request);
            if (result)
            {
                TempData["result"] = "Cập nhật sảnh tiệc thành công";
                return RedirectToAction("List");
            }
            ModelState.AddModelError("", "Cập nhật sảnh tiệc thất bại");
            return await Update(request.Id);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(HallDeleteRequest request)
        {
            await _hallApiClient.Delete(request);
            var data = await _hallApiClient.GetAll();
            return Json(new { html = RenderRazorViewToString(this, "DataTable", data) });
        }
    }
}
