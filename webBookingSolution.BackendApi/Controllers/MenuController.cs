using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webBookingSolution.Application.Catalog.Menus;
using webBookingSolution.ViewModels.Catalog.Menus;

namespace webBookingSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var menus = await _menuService.GetAll();
            return Ok(menus);
        }

        [HttpGet("paging")]
        public async Task<IActionResult> Get([FromQuery] GetMenuPagingRequest request)
        {
            var menus = await _menuService.GetAllPaging(request);
            return Ok(menus);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var menus = await _menuService.GetById(id);
            if (menus == null)
                BadRequest("Không tìm thấy thực đơn cần");
            return Ok(menus);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] MenuCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var menuId = await _menuService.Create(request);
            if (menuId == 0)
                BadRequest();

            var menu = await _menuService.GetById(menuId);

            return CreatedAtAction(nameof(GetById), new { id = menuId }, menu);
        }

        [HttpPut("{menuId}")]
        public async Task<IActionResult> Update([FromRoute] int menuId, [FromForm] MenuUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = menuId;
            var affectedResult = await _menuService.Update(request);
            if (affectedResult == 0)
                BadRequest();

            return Ok();
        }

        [HttpDelete("{menuId}")]
        public async Task<IActionResult> Delete([FromRoute] int menuId, [FromQuery] MenuDeleteRequest request)
        {
            request.Id = menuId;
            var affectedResult = await _menuService.Delete(request);
            if (affectedResult == 0)
                BadRequest();

            return Ok();
        }
    }
}
