using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using webBookingSolution.ViewModels.Catalog.Users;
using webBookingSolution.ViewModels.System;
using webBookingSolution.Api.Users;
using static webBookingSolution.WebAdmin.Helper;
using webBookingSolution.Api.Customers;
using webBookingSolution.Api.Employees;

namespace webBookingSolution.WebAdmin.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserApiClient _userApiClient;
        private readonly ICustomerApiClient _customerApiClient;
        private readonly IEmployeeApiClient _employeeApiClient;
        private readonly IConfiguration _configuration;
        private readonly ICompositeViewEngine _viewEngine;
        public UserController(IUserApiClient userApiClient, IConfiguration configuration, ICompositeViewEngine viewEngine, ICustomerApiClient customerApiClient, IEmployeeApiClient employeeApiClient)
        {
            _customerApiClient = customerApiClient;
            _employeeApiClient = employeeApiClient;
            _viewEngine = viewEngine;
            _userApiClient = userApiClient;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (TempData["result"] != null)
                ViewBag.FailedMsg = TempData["result"];
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return View(ModelState);
            var token = await _userApiClient.Authencate(request);

            if (token == "Tài khoản không đủ quyền để vào trang này")
            {
                TempData["result"] = "Tài khoản không đủ quyền để vào trang này";
                return RedirectToAction("Login", "User");
            }
            else if (token == "Sai tài khoản hoặc mật khẩu")
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

                return RedirectToAction("Dashboard", "Home");

            }
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
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
        public async Task<IActionResult> List()
        {
            var data = await _userApiClient.GetAll();
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
        public async Task<IActionResult> Create(RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _userApiClient.Create(request);
            if (result == "88")
            {
                ModelState.AddModelError("", "Tài khoản đã tồn tại");
                return View();
            }
            else if (result == "77")
            {
                ModelState.AddModelError("", "Khách hàng đã tồn tại");
                return View();
            }
            else if (result == "66")
            {
                ModelState.AddModelError("", "Nhân viên đã tồn tại");
                return View();
            }
            else 
            {
                TempData["result"] = "Thêm tài khoản thành công";
                return RedirectToAction("List");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(int id)
        {
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
            else if(result == "0")
            {
                ModelState.AddModelError("", "Cập nhật tài khoản thất bại");
                return await ChangePassword(request.Id);
            }
            else
            {
                TempData["result"] = "Cập nhật tài khoản thành công";
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UserDeleteRequest request)
        {
            var user = _userApiClient.GetById(request.Id);
            var loginUser = User.FindFirst("Id")?.Value;

            if (user == null)
                return RedirectToAction("ErrorPage", "Home");
            
            await _userApiClient.Delete(request);
            if (request.Id == int.Parse(loginUser))
                return await Logout();

            var data = await _userApiClient.GetAll();
            return Json(new { html = RenderRazorViewToString(this, "DataTable", data) });
        }
    }
}
