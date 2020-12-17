using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.Application.Common;
using webBookingSolution.Data.EF;
using webBookingSolution.Data.Entities;
using webBookingSolution.Utilities.Exceptions;
using webBookingSolution.ViewModels.Catalog;
using webBookingSolution.ViewModels.Catalog.Menus;
using webBookingSolution.ViewModels.Common;

namespace webBookingSolution.Application.Catalog.Menus
{
    public class MenuService : IMenuService
    {
        private readonly BookingDBContext _context;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _config;
        public MenuService(BookingDBContext context, IStorageService storageService, IConfiguration config)
        {
            _config = config;
            _context = context;
            _storageService = storageService;
        }

        public async Task<int> Create(MenuCreateRequest request)
        {
            var menu = new Menu()
            {
                Name = request.Name,
                Content = request.Content,
                Price = request.Price,
                Image = await this.SaveFile(request.Image)
            };
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
            return menu.Id;
        }

        public async Task<int> Delete(MenuDeleteRequest request)
        {
            var menu = await _context.Menus.FindAsync(request.Id);
            if (menu == null) throw new BookingException($"Không thể tìm được thực đơn: {request.Id}");
            _context.Menus.Remove(menu);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<MenuViewModel>> GetAll()
        {
            var query = from m in _context.Menus
                        orderby m.Id descending 
                        select new { m };

            //int totalRow = await query.CountAsync();
            var data = await query.Select(x => new MenuViewModel()
            {
                Id = x.m.Id,
                Name = x.m.Name,
                Content = x.m.Content,
                Image = _config["PathImage"] + x.m.Image,
                Price = x.m.Price
            }).ToListAsync();
            return data;
        }

        public async Task<PagedResult<MenuViewModel>> GetAllPaging(GetMenuPagingRequest request)
        {
            var query = from m in _context.Menus
                        select new { m };

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.m.Name.Contains(request.Keyword) ||
                                x.m.Content.Contains(request.Keyword) ||
                                x.m.Price.ToString().Contains(request.Keyword));
            }

            int totalRow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new MenuViewModel()
                {
                    Id = x.m.Id,
                    Name = x.m.Name,
                    Image = _config["PathImage"] + x.m.Image,
                    Content = x.m.Content,
                    Price = x.m.Price
                }).ToListAsync();

            var pageResult = new PagedResult<MenuViewModel>()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalRecords = totalRow,
                Items = data
            };
            return pageResult;
        }

        public async Task<MenuViewModel> GetById(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
                return null;

            var menuViewmodel = new MenuViewModel()
            {
                Id = menu.Id,
                Name = menu.Name,
                Content = menu.Content,
                Image = _config["PathImage"] + menu.Image,
                Price = menu.Price
            };
            return menuViewmodel;
        }

        public async Task<int> Update(MenuUpdateRequest request)
        {
            var menu = await _context.Menus.FirstOrDefaultAsync(x=>x.Id == request.Id);
            if (menu == null) throw new BookingException($"Không tìm thấy thực đơn {request.Id}");

            menu.Name = request.Name;
            menu.Content = request.Content;
            if (request.Image != null)
                menu.Image = await this.SaveFile(request.Image);
            menu.Price = request.Price;

            return await _context.SaveChangesAsync();
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var ogName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ogName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }
    }
}
