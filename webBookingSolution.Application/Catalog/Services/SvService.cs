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
using webBookingSolution.ViewModels.Catalog.Services;
using webBookingSolution.ViewModels.Common;

namespace webBookingSolution.Application.Catalog.Services
{
    public class SvService : ISvService
    {
        private readonly BookingDBContext _context;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _config;
        public SvService(BookingDBContext context, IStorageService storageService, IConfiguration config)
        {
            _config = config;
            _context = context;
            _storageService = storageService;
        }

        public async Task<int> Create(ServiceCreateRequest request)
        {
            var service = new Service()
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Image = await this.SaveFile(request.Image)
            };
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return service.Id;
        }

        public async Task<int> Delete(ServiceDeleteRequest request)
        {
            var service = await _context.Services.FindAsync(request.Id);
            if (service == null) throw new BookingException($"Không thể tìm được dịch vụ: {request.Id}");
            _context.Services.Remove(service);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<ServiceViewModel>> GetAll()
        {
            var query = from s in _context.Services
                        select new { s };

            //int totalRow = await query.CountAsync();
            var data = await query.Select(x => new ServiceViewModel()
                {
                    Id = x.s.Id,
                    Name = x.s.Name,
                    Image = _config["PathImage"] + x.s.Image,
                    Description = x.s.Description,
                    Price = x.s.Price,
                }).ToListAsync();
            return data;
        }

        public async Task<PagedResult<ServiceViewModel>> GetAllPaging(GetServicePagingRequest request)
        {
            var query = from s in _context.Services
                        select new { s };

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.s.Name.Contains(request.Keyword) || 
                        x.s.Description.Contains(request.Keyword) || 
                        x.s.Price.ToString().Contains(request.Keyword));
            }

            int totalRow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ServiceViewModel()
                {
                    Id = x.s.Id,
                    Name = x.s.Name,
                    Description = x.s.Description,
                    Image = _config["PathImage"] + x.s.Image,
                    Price = x.s.Price
                }).ToListAsync();

            var pageResult = new PagedResult<ServiceViewModel>()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalRecords = totalRow,
                Items = data
            };
            return pageResult;
        }

        public async Task<ServiceViewModel> GetById(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return null;

            var serviceViewmodel = new ServiceViewModel()
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Image = _config["PathImage"] + service.Image,
                Price = service.Price
            };
            return serviceViewmodel;
        }

        public async Task<int> Update(ServiceUpdateRequest request)
        {
            var service = await _context.Services.FindAsync(request.Id);
            if (service == null) throw new BookingException($"Không tìm thấy dịch vụ {request.Id}");

            service.Name = request.Name;
            service.Description = request.Description;
            if (request.Image != null)
                service.Image = await this.SaveFile(request.Image);
            service.Price = request.Price;

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
