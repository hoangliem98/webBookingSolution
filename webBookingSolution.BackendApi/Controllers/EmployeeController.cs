using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webBookingSolution.Application.Catalog.Employees;
using webBookingSolution.Application.System.Users;
using webBookingSolution.ViewModels.Catalog.Employees;
using webBookingSolution.ViewModels.Catalog.Users;

namespace webBookingSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IUserService _userService;
        public EmployeeController(IEmployeeService employeeService, IUserService userService)
        {
            _userService = userService;
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int month)
        {
            var employees = await _employeeService.GetAll(month);
            return Ok(employees);
        }

        //[HttpGet("paging")]
        //public async Task<IActionResult> Get([FromQuery] GetEmployeePagingRequest request)
        //{
        //    var employees = await _employeeService.GetAllPaging(request);
        //    return Ok(employees);
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employees = await _employeeService.GetById(id);
            if (employees == null)
                BadRequest("Không tìm thấy nhân viên cần");
            return Ok(employees);
        }

        [HttpPut("{employeeId}")]
        public async Task<IActionResult> Update([FromRoute] int employeeId, [FromForm] EmployeeUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = employeeId;
            var affectedResult = await _employeeService.Update(request);
            if (affectedResult == 66)
                return BadRequest("66");

            return Ok();
        }

        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> Delete([FromRoute] int employeeId, [FromQuery] EmployeeDeleteRequest request, [FromQuery] UserDeleteRequest userRequest)
        {
            request.Id = employeeId;
            var userAccount = await _employeeService.GetById(request.Id);
            if(userAccount != null && userAccount.AccountId != null)
            {
                userRequest.Id = (int)userAccount.AccountId;
                var affectedResult = await _userService.DeleteUser(userRequest); 
                if (affectedResult == 0)
                    BadRequest();

                return Ok();
            }
            else
            {
                var affectedResult = await _employeeService.Delete(request);
                if (affectedResult == 0)
                    BadRequest();

                return Ok();
            }
        }

        [HttpGet("listmonth")]
        public async Task<IActionResult> GetListMonth()
        {
            var months = await _employeeService.GetListMonth();
            return Ok(months);
        }
    }
}
