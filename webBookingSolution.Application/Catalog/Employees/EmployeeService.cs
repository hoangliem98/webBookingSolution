using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.Application.Common;
using webBookingSolution.Data.EF;
using webBookingSolution.Data.Entities;
using webBookingSolution.Utilities.Exceptions;
using webBookingSolution.ViewModels.Catalog.Employees;

namespace webBookingSolution.Application.Catalog.Employees
{
    public class EmployeeService : IEmployeeService
    {
        private readonly BookingDBContext _context;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _config;
        public EmployeeService(BookingDBContext context, IStorageService storageService, IConfiguration config)
        {
            _storageService = storageService;
            _config = config;
            _context = context;
        }

        public async Task<int> Delete(EmployeeDeleteRequest request)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (employee == null) throw new BookingException($"Không thể tìm được nhân viên: {request.Id}");
            _context.Employees.Remove(employee);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<EmployeeViewModel>> GetAll(int month)
        {
            var query = from e in _context.Employees
                        select new { e };

            if (month != 0)
            {
                query = query.Where(x => x.e.DOB.Month == month);
            }

            //int totalRow = await query.CountAsync();
            var data = await query.Select(x => new EmployeeViewModel()
            {
                Id = x.e.Id,
                LastName = x.e.LastName,
                FirstName = x.e.FirstName,
                DOB = x.e.DOB,
                Image = x.e.Image.Contains("https://graph.facebook.com/") ? x.e.Image : _config["PathImage"] + x.e.Image,
                PhoneNumber = x.e.PhoneNumber,
                Email = x.e.Email,
                Address = x.e.Address
            }).ToListAsync();
            return data;
        }

        public async Task<EmployeeViewModel> GetById(int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (employee == null)
                return null;

            var employeeViewmodel = new EmployeeViewModel()
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DOB = employee.DOB,
                Image = employee.Image.Contains("https://graph.facebook.com/") ? employee.Image : _config["PathImage"] + employee.Image,
                PhoneNumber = employee.PhoneNumber,
                Email = employee.Email,
                Address = employee.Address,
                AccountId = employee.AccountId.Equals(null) ? null : employee.AccountId,
            };
            return employeeViewmodel;
        }

        public async Task<EmployeeViewModel> GetEmployeeByAccount(int accountId)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.AccountId == accountId);

            var employeeViewmodel = new EmployeeViewModel()
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DOB = employee.DOB,
                Image = _config["PathImage"] + employee.Image,
                PhoneNumber = employee.PhoneNumber,
                Email = employee.Email,
                Address = employee.Address
            };
            return employeeViewmodel;
        }

        public async Task<int> Update(EmployeeUpdateRequest request)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (employee == null) throw new BookingException($"Không tìm thấy nhân viên {request.Id}");

            if (await _context.Employees.AnyAsync(x => x.Email == request.Email && x.Id != request.Id))
                return 66;
            else if (await _context.Employees.AnyAsync(x => x.PhoneNumber == request.PhoneNumber && x.Id != request.Id))
                return 66;

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            if (request.Image != null)
                employee.Image = await this.SaveFile(request.Image);
            employee.DOB = request.DOB;
            employee.PhoneNumber = request.PhoneNumber;
            employee.Email = request.Email;
            employee.Address = request.Address;
            return await _context.SaveChangesAsync();
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var ogName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ogName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }

        public async Task<List<int>> GetListMonth()
        {
            var query = from e in _context.Employees
                        select new { e };

            var allMonth = await query.Select(x => x.e.DOB).ToListAsync();

            int minMonth = 1;
            int maxMonth = 12;

            foreach (var months in allMonth)
            {
                int monthItem = months.Month;

                if (monthItem < minMonth)
                    minMonth = monthItem;
                if (monthItem > maxMonth)
                    maxMonth = monthItem;
            }

            var listMonth = new List<int>();

            for (int i = minMonth; i <= maxMonth; i++)
            {
                listMonth.Add(i);
            }

            return listMonth;
        }
    }
}
