using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using webBookingSolution.Data.Entities;
using webBookingSolution.Utilities.Exceptions;
using webBookingSolution.ViewModels.System;
using webBookingSolution.Application.Common;
using webBookingSolution.Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using webBookingSolution.ViewModels.Catalog.Customers;
using webBookingSolution.ViewModels.Catalog.Employees;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using webBookingSolution.Application.Catalog.Employees;
using webBookingSolution.ViewModels.Catalog.Users;
using webBookingSolution.Application.Catalog.Customers;

namespace webBookingSolution.Application.System.Users
{
    public class UserService : IUserService
    {
        private readonly BookingDBContext _context;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _config;
        private readonly IEmployeeService _employeeService;
        private readonly ICustomerService _customerService;
        public UserService(BookingDBContext context,  IStorageService storageService, IConfiguration config, IEmployeeService employeeService, ICustomerService customerService) 
        {
            _context = context;
            _storageService = storageService;
            _config = config;
            _employeeService = employeeService;
            _customerService = customerService;
        }

        public async Task<string> Authencate(LoginRequest request)
        {
            var account = await GetAccountByUserName(request.UserName);
            if (account == null)
                return null;

            var checkPass = BCrypt.Net.BCrypt.Verify(request.Password, account.Password);

            if (!checkPass)
            {
                return null;
            }
            else if (account.Roles == 0)
            {
                return "Tài khoản không đủ quyền để vào trang này";
            }
            else
            {
                var acc = await GetById(account.Id);
                var employee = await _employeeService.GetEmployeeByAccount(account.Id);
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, employee.FirstName + " " + employee.LastName),
                    new Claim(ClaimTypes.Uri, employee.Image),
                    new Claim("UserId", acc.EmOrCusId.ToString()),
                    new Claim("Id", account.Id.ToString()),
                    new Claim(ClaimTypes.Role, account.Roles.ToString()),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                    _config["Tokens:Issuer"],
                    claims,
                    expires: DateTime.Now.AddHours(3),
                    signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        }

        public async Task<UserViewModel> GetAccountByUserName(string username)
        {
            var query = from p in _context.Accounts select new { p };

            int totalRow = await query.CountAsync();
            if(totalRow > 0)
            {
                var account = await _context.Accounts.SingleOrDefaultAsync(x=>x.UserName == username);
                if (account == null)
                    return null;

                var accountViewModel = new UserViewModel()
                {
                    Id = account.Id,
                    UserName = account.UserName,
                    Password = account.Password,
                    Roles = account.Roles
                };

                return accountViewModel;
            }
            else
                return null;
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

        public async Task<bool> CheckEmployeeContain(string email, string phoneNumber)
        {
            var query = from p in _context.Employees select new { p };

            int totalRow = await query.CountAsync();
            if (totalRow > 0)
            {
                var cusEmail = await _context.Employees.SingleOrDefaultAsync(x => x.Email == email);
                var cusPhone = await _context.Employees.SingleOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
                if (cusEmail == null && cusPhone == null)
                    return true;
                return false;
            }
            else
                return true;
        }

        public async Task<int> Register(RegisterRequest request)
        {
            if (await GetAccountByUserName(request.UserName) != null)
                return 88; //Tài khoản đã tồn tại

            var account = new Account();
            var pass = BCrypt.Net.BCrypt.HashPassword(request.Password);
            account.UserName = request.UserName;
            account.Password = pass;
            account.Roles = 0;
            _context.Accounts.Add(account);
            if (!await CheckCustomerContain(request.Email, request.PhoneNumber))
                return 77; //Khách hàng đã tồn tại

            await _context.SaveChangesAsync();
            var customer = new Customer()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DOB = request.DOB,
                Image = await this.SaveFile(request.Image),
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Address = request.Address,
                AccountId = account.Id
            };
            _context.Customers.Add(customer);
            return _context.SaveChanges();
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var ogName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ogName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }

        public async Task<List<UserViewModel>> GetAll()
        {
            var query = from u in _context.Accounts
                        join e in _context.Employees on u.Id equals e.AccountId into ne
                        from e in ne.DefaultIfEmpty()
                        join c in _context.Customers on u.Id equals c.AccountId into nc
                        from c in nc.DefaultIfEmpty()
                        select new { u, e, c };
            var data = await query.Select(x => new UserViewModel()
            {
                Id = x.u.Id,
                UserName = x.u.UserName,
                DisplayName = x.e.FirstName.Equals(null) ? x.c.FirstName + " " +  x.c.LastName : x.e.FirstName + " " + x.e.LastName,
                Email = x.e.Email.Equals(null) ? x.c.Email : x.e.Email,
                ImagePath = x.e.Email.Equals(null) ? _config["PathImage"] + x.c.Image : _config["PathImage"] + x.e.Image,
                Roles = x.u.Roles
            }).ToListAsync();
            return data;
        }

        public async Task<int> CreateUser(RegisterRequest request)
        {
            if (await GetAccountByUserName(request.UserName) != null)
                return 88;

            var account = new Account();
            var pass = BCrypt.Net.BCrypt.HashPassword(request.Password);
            account.UserName = request.UserName;
            account.Password = pass;
            account.Roles = request.Roles;
            var result = _context.Accounts.Add(account);
            if (account.Roles == 1)
            {
                if (!await CheckEmployeeContain(request.Email, request.PhoneNumber))
                    return 66; //Nhân viên đã tồn tại

                await _context.SaveChangesAsync();
                var employee = new Employee()
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    DOB = request.DOB,
                    Image = await this.SaveFile(request.Image),
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email,
                    Address = request.Address,
                    AccountId = account.Id
                };
                _context.Employees.Add(employee);
            }
            else if (request.Roles == 0)
            {
                if (!await CheckCustomerContain(request.Email, request.PhoneNumber))
                    return 77;

                await _context.SaveChangesAsync();
                var customer = new Customer()
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    DOB = request.DOB,
                    Image = await this.SaveFile(request.Image),
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email,
                    Address = request.Address,
                    AccountId = account.Id
                };
                _context.Customers.Add(customer);
            }
            return _context.SaveChanges();
        }

        public async Task<int> ChangePassword(ChangePasswordRequest request)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (account == null) throw new BookingException($"Không tìm thấy tài khoản {request.Id}");

            var checkPass = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, account.Password);

            if (checkPass)
            {
                account.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                return await _context.SaveChangesAsync();
            }
            else
                return 66;
        }

        public async Task<int> DeleteUser(UserDeleteRequest request)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.AccountId == request.Id);
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.AccountId == request.Id);
            if (employee == null && customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
            else if (employee != null && customer == null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            var user = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (user == null) throw new BookingException($"Không thể tìm được tài khoản: {request.Id}");
            _context.Accounts.Remove(user);
            return await _context.SaveChangesAsync();
        }

        public async Task<UserViewModel> GetById(int id)
        {
            int tmp = 0;
            var user = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return null;

            else if (user.Roles == 1)
            {
                var employee = await _employeeService.GetEmployeeByAccount(id);
                tmp = employee.Id;
            }
            else
            {
                var customer = await _customerService.GetCustomerByAccount(id);
                tmp = customer.Id;
            }
            var userViewModel = new UserViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Roles = user.Roles,
                EmOrCusId = tmp
            };
            return userViewModel;
        }

        public async Task<string> UserLogin(LoginRequest request)
        {
            var account = await GetAccountByUserName(request.UserName);
            if (account == null)
                return null;

            var checkPass = BCrypt.Net.BCrypt.Verify(request.Password, account.Password);

            if (!checkPass)
            {
                return null;
            }
            else
            {
                var acc = await GetById(account.Id);
                if(account.Roles == 1)
                {
                    var employee = await _employeeService.GetEmployeeByAccount(account.Id);
                    var claim = new[]
                    {
                        new Claim(ClaimTypes.Name, employee.FirstName + " " + employee.LastName),
                        new Claim(ClaimTypes.Uri, employee.Image),
                        new Claim("UserId", acc.EmOrCusId.ToString()),
                        new Claim("Id", account.Id.ToString()),
                        new Claim(ClaimTypes.Role, account.Roles.ToString()),
                    };
                    var keys = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                    var cred = new SigningCredentials(keys, SecurityAlgorithms.HmacSha256);

                    var tokens = new JwtSecurityToken(_config["Tokens:Issuer"],
                        _config["Tokens:Issuer"],
                        claim,
                        expires: DateTime.Now.AddHours(3),
                        signingCredentials: cred);

                    return new JwtSecurityTokenHandler().WriteToken(tokens);
                }

                var customer = await _customerService.GetCustomerByAccount(account.Id);
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, customer.FirstName + " " + customer.LastName),
                    new Claim(ClaimTypes.Uri, customer.Image),
                    new Claim("UserId", acc.EmOrCusId.ToString()),
                    new Claim("Id", account.Id.ToString()),
                    new Claim(ClaimTypes.Role, account.Roles.ToString()),
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                    _config["Tokens:Issuer"],
                    claims,
                    expires: DateTime.Now.AddHours(3),
                    signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        }
    }
}
