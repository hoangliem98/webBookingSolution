using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webBookingSolution.Application.Catalog.Services;
using webBookingSolution.ViewModels.Catalog.Services;

namespace webBookingSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly ISvService _svService;
        public ServiceController(ISvService svService)
        {
            _svService = svService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var services = await _svService.GetAll();
            return Ok(services);
        }

        [HttpGet("paging")]
        public async Task<IActionResult> Get([FromQuery] GetServicePagingRequest request)
        {
            var services = await _svService.GetAllPaging(request);
            return Ok(services);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var services = await _svService.GetById(id);
            if (services == null)
                BadRequest("Không tìm thấy dịch vụ bạn cần");
            return Ok(services);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ServiceCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var serviceId = await _svService.Create(request);
            if (serviceId == 0)
                BadRequest();

            var service = await _svService.GetById(serviceId);

            return CreatedAtAction(nameof(GetById), new { id = serviceId }, service);
        }

        [HttpPut("{serviceId}")]
        public async Task<IActionResult> Update([FromRoute] int serviceId, [FromForm] ServiceUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = serviceId;
            var affectedResult = await _svService.Update(request);
            if (affectedResult == 0)
                BadRequest();

            return Ok();
        }

        [HttpDelete("{serviceId}")]
        public async Task<IActionResult> Delete([FromRoute] int serviceId, [FromQuery] ServiceDeleteRequest request)
        {
            request.Id = serviceId;
            var affectedResult = await _svService.Delete(request);
            if (affectedResult == 0)
                BadRequest();

            return Ok();
        }
    }
}
