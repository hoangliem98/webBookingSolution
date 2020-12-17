using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webBookingSolution.Application.Catalog.Customers;
using webBookingSolution.Application.System.Users;
using webBookingSolution.ViewModels.Catalog.Customers;
using webBookingSolution.ViewModels.Catalog.Users;

namespace webBookingSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IUserService _userService;
        public CustomerController(ICustomerService customerService, IUserService userService)
        {
            _userService = userService;
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int month)
        {
            var customers = await _customerService.GetAll(month);
            return Ok(customers);
        }

        //[HttpGet("paging")]
        //public async Task<IActionResult> Get([FromQuery] GetCustomerPagingRequest request)
        //{
        //    var customers = await _customerService.GetAllPaging(request);
        //    return Ok(customers);
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customers = await _customerService.GetById(id);
            if (customers == null)
                BadRequest("Không tìm thấy khách hàng cần");
            return Ok(customers);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CustomerCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var customerId = await _customerService.Create(request);

            if (customerId == 0)
                return BadRequest("0");
            else
                return Ok();
        }

        [HttpPut("{customerId}")]
        public async Task<IActionResult> Update([FromRoute] int customerId, [FromForm] CustomerUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = customerId;
            var affectedResult = await _customerService.Update(request);
            if (affectedResult == 77)
                return BadRequest("77");
            else
                return Ok();
        }

        [HttpDelete("{customerId}")]
        public async Task<IActionResult> Delete([FromRoute] int customerId, [FromQuery] UserDeleteRequest userRequest)
        {
            var userAccount = await _customerService.GetById(customerId);

            if (userAccount != null && userAccount.AccountId != null)
            {
                userRequest.Id = (int)userAccount.AccountId;
                var affectedResult = await _userService.DeleteUser(userRequest);
                if (affectedResult == 0)
                    BadRequest();

                return Ok();
            }
            else
            {
                var affectedResult = await _customerService.Delete(customerId);
                if (affectedResult == 0)
                    BadRequest();

                return Ok();
            }
        }
    }
}
