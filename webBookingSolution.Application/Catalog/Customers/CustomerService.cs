using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.Application.Common;
using webBookingSolution.Data.EF;
using webBookingSolution.Data.Entities;
using webBookingSolution.Utilities.Exceptions;
using webBookingSolution.ViewModels.Catalog.Customers;

namespace webBookingSolution.Application.Catalog.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly BookingDBContext _context;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _config;
        public CustomerService(BookingDBContext context, IStorageService storageService, IConfiguration config)
        {
            _storageService = storageService;
            _config = config;
            _context = context;
        }

        public async Task<bool> CheckCustomerContain(string email, string phoneNumber)
        {
            var query = from p in _context.Customers select new { p };

            int totalRow = await query.CountAsync();
            if (totalRow > 0)
            {
                var cusEmail = await _context.Customers.SingleOrDefaultAsync(x => x.Email == email);
                var cusPhone = await _context.Customers.SingleOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
                if (cusEmail == null && cusPhone == null)
                    return true;
                return false;
            }
            else
                return true;
        }

        public async Task<int> Create(CustomerCreateRequest request)
        {
            if (!await CheckCustomerContain(request.Email, request.PhoneNumber))
                return 0;

            var customer = new Customer()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DOB = request.DOB,
                Image = await this.SaveFile(request.Image),
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Address = request.Address,
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer.Id;
        }

        public async Task<int> Delete(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x=>x.Id == id);
            if (customer == null) throw new BookingException($"Không thể tìm được khách hàng: {id}");
            _context.Customers.Remove(customer);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<CustomerViewModel>> GetAll(int month)
        {
            var query = from c in _context.Customers
                        select new { c };

            if(month != 0)
            {
                query = query.Where(x => x.c.DOB.Month == month);
            }

            //int totalRow = await query.CountAsync();
            var data = await query.Select(x => new CustomerViewModel()
            {
                Id = x.c.Id,
                LastName = x.c.LastName,
                FirstName = x.c.FirstName,
                DOB = x.c.DOB,
                Image = _config["PathImage"] + x.c.Image,
                PhoneNumber = x.c.PhoneNumber,
                Email =  x.c.Email,
                Address = x.c.Address
            }).ToListAsync();
            return data;
        }

        public async Task<CustomerViewModel> GetById(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
            if (customer == null)
                return null;
            var customerViewmodel = new CustomerViewModel()
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                DOB = customer.DOB,
                Image = _config["PathImage"] + customer.Image,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                Address = customer.Address,
                AccountId = customer.AccountId.Equals(null) ? null : customer.AccountId,
            };
            return customerViewmodel;
        }

        public async Task<CustomerViewModel> GetCustomerByAccount(int accountId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.AccountId == accountId);

            var customerViewmodel = new CustomerViewModel()
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                DOB = customer.DOB,
                Image = _config["PathImage"] + customer.Image,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                Address = customer.Address
            };
            return customerViewmodel;
        }

        public async Task<int> Update(CustomerUpdateRequest request)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (customer == null) throw new BookingException($"Không tìm thấy khách hàng {request.Id}");

            if (await _context.Customers.AnyAsync(x => x.Email == request.Email && x.Id != request.Id))
                return 77;
            else if (await _context.Customers.AnyAsync(x => x.PhoneNumber == request.PhoneNumber && x.Id != request.Id))
                return 77;

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            if (request.Image != null)
                customer.Image = await this.SaveFile(request.Image);
            customer.DOB = request.DOB;
            customer.PhoneNumber = request.PhoneNumber;
            customer.Email = request.Email;
            customer.Address = request.Address;

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
