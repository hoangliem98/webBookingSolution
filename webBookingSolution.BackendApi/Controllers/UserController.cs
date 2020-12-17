using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webBookingSolution.Application.System.Users;
using webBookingSolution.ViewModels.Catalog.Users;
using webBookingSolution.ViewModels.System;

namespace webBookingSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Authencate(request);

            if (string.IsNullOrEmpty(result))
                return BadRequest("Sai tài khoản hoặc mật khẩu");
            else if(result == "Tài khoản không đủ quyền để vào trang này")
                return BadRequest("Tài khoản không đủ quyền để vào trang này");
            else
                return Ok(result);
        }

        [HttpPost("userlogin")]
        public async Task<IActionResult> UserLogin([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.UserLogin(request);

            if (string.IsNullOrEmpty(result))
                return BadRequest("Sai tài khoản hoặc mật khẩu");
            else
                return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Register(request);

            if (result == 88)
                return BadRequest("Tài khoản đã tồn tại");
            else if (result == 77)
                return BadRequest("Khách hàng đã tồn tại");
            else
                return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var users = await _userService.GetById(id);
            if (users == null)
                BadRequest("Không tìm thấy tài khoản cần");
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _userService.CreateUser(request);
            if (result == 88)
                return BadRequest("88");
            else if (result == 77)
                return BadRequest("77");
            else if (result == 66)
                return BadRequest("66");
            else
                return Ok();
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> ChangePassword([FromRoute] int userId, [FromForm] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = userId;
            var affectedResult = await _userService.ChangePassword(request);
            if (affectedResult == 0)
                return BadRequest("0");
            else if (affectedResult == 66)
                return BadRequest("66");
            else
                return Ok();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete([FromRoute] int userId, [FromQuery] UserDeleteRequest request)
        {
            request.Id = userId;
            var affectedResult = await _userService.DeleteUser(request);
            if (affectedResult == 0)
                BadRequest();

            return Ok();
        }
    }
}
