using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using webBookingSolution.Api.Customers;
using webBookingSolution.Api.Employees;
using webBookingSolution.Api.Users;
using webBookingSolution.ViewModels.Catalog.Customers;
using webBookingSolution.ViewModels.Catalog.Employees;
using webBookingSolution.ViewModels.Catalog.Users;
using webBookingSolution.ViewModels.System;

namespace webBookingSolution.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;
        private readonly IEmployeeApiClient _employeeApiClient;
        private readonly ICustomerApiClient _customerApiClient;
        //private readonly SignInManager<UserViewModel> _signInManager;

        public UserController(IUserApiClient userApiClient, IConfiguration configuration, IEmployeeApiClient employeeApiClient, ICustomerApiClient customerApiClient/*, SignInManager<UserViewModel> signInManager*/)
        {
            //_signInManager = signInManager;
            _employeeApiClient = employeeApiClient;
            _customerApiClient = customerApiClient;
            _userApiClient = userApiClient;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (TempData["result"] != null)
                ViewBag.FailedMsg = TempData["result"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return View(ModelState);
            var token = await _userApiClient.UserLogin(request);
            if (token == "Sai tài khoản hoặc mật khẩu")
            {
                TempData["result"] = "Sai tài khoản hoặc mật khẩu";
                return RedirectToAction("Login", "User");
            }
            else
            {
                var userPrincipal = this.ValidateToken(token);
                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    userPrincipal,
                    authProperties);

                return RedirectToAction("Index", "Home");

            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        //giai ma token
        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = _configuration["Tokens:Issuer"];
            validationParameters.ValidIssuer = _configuration["Tokens:Issuer"];
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(int id)
        {
            if(User.Identity.Name == null)
                return RedirectToAction("ErrorPage", "Home");
            if (User.FindFirst("Id").Value != id.ToString())
                return RedirectToAction("ErrorPage", "Home");
            var user = await _userApiClient.GetById(id);
            if (user == null)
                return RedirectToAction("ErrorPage", "Home");
            var userViewModel = new ChangePasswordRequest()
            {
                Id = user.Id,
                Roles = user.Roles,
                UserName = user.UserName,
                EmOrCusId = user.EmOrCusId
            };
            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return await ChangePassword(request.Id);

            var result = await _userApiClient.ChangePassword(request);
            if (result == "66")
            {
                ModelState.AddModelError("", "Mật khẩu hiện tại không đúng");
                return await ChangePassword(request.Id);
            }
            else if (result == "0")
            {
                ModelState.AddModelError("", "Cập nhật tài khoản thất bại");
                return await ChangePassword(request.Id);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EmployeeUpdate(int id)
        {
            if (User.Identity.Name == null)
                return RedirectToAction("ErrorPage", "Home");
            if (User.FindFirst(ClaimTypes.Role).Value != "1")
                return RedirectToAction("ErrorPage", "Home");
            if (User.FindFirst("UserId").Value != id.ToString())
                return RedirectToAction("ErrorPage", "Home");
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
                Email = employee.Email,
                AccountId = employee.AccountId.Equals(null) ? null : employee.AccountId,
            };
            if (TempData["result"] != null)
                ViewBag.SuccessMsg = TempData["result"];
            return View(employeeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EmployeeUpdate(EmployeeUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return await EmployeeUpdate(request.Id);

            var result = await _employeeApiClient.Update(request);
            if (result == "66")
            {

                ModelState.AddModelError("", "Nhân viên đã tồn tại");
                return await EmployeeUpdate(request.Id);
            }
            else
            {
                TempData["result"] = "Sửa thông tin nhân viên thành công";
                return RedirectToAction("EmployeeUpdate", "User");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CustomerUpdate(int id)
        {
            if (User.Identity.Name == null)
                return RedirectToAction("ErrorPage", "Home");
            if (User.FindFirst(ClaimTypes.Role).Value != "0")
                return RedirectToAction("ErrorPage", "Home");
            if (User.FindFirst("UserId").Value != id.ToString())
                return RedirectToAction("ErrorPage", "Home");
            var customer = await _customerApiClient.GetById(id);
            if (customer == null)
                return RedirectToAction("ErrorPage", "Home");
            var employeeViewModel = new CustomerUpdateRequest()
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                DOB = customer.DOB,
                ImagePath = customer.Image,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                Email = customer.Email,
                AccountId = customer.AccountId.Equals(null) ? null : customer.AccountId,
            }; 
            if (TempData["result"] != null)
                ViewBag.SuccessMsg = TempData["result"];
            return View(employeeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CustomerUpdate(CustomerUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return await CustomerUpdate(request.Id);

            var result = await _customerApiClient.Update(request);
            if (result == "66")
            {

                ModelState.AddModelError("", "Nhân viên đã tồn tại");
                return await CustomerUpdate(request.Id);
            }
            else
            {
                TempData["result"] = "Sửa thông tin nhân viên thành công";
                return RedirectToAction("CustomerUpdate", "User");
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _userApiClient.Register(request);
            if (result == "66")
            {
                ModelState.AddModelError("", "Nhân viên đã tồn tại");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider)
        {
            var properties = new AuthenticationProperties {
                //RedirectUri = Url.Action("Register", "User")
                RedirectUri = Url.Action("Index", "Home")
            };
            return Challenge(properties, provider);
        }
    }
}
