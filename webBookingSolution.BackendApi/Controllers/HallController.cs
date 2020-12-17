using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webBookingSolution.Application.Catalog.Halls;
using webBookingSolution.ViewModels.Catalog.HallImages;
using webBookingSolution.ViewModels.Catalog.Halls;

namespace webBookingSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HallController : ControllerBase
    {
        //Halls
        private readonly IHallService _hallService;
        public HallController(IHallService hallService)
        {
            _hallService = hallService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var halls = await _hallService.GetAll();
            return Ok(halls);
        }

        [HttpGet("paging")]
        public async Task<IActionResult> Get([FromQuery] GetHallPagingRequest request)
        {
            var halls = await _hallService.GetAllPaging(request);
            return Ok(halls);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var halls = await _hallService.GetById(id);
            if (halls == null)
                BadRequest("Không tìm thấy sảnh bạn cần");
            return Ok(halls);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] HallCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var hallId = await _hallService.Create(request);
            if (hallId == 0)
                BadRequest();

            var hall = await _hallService.GetById(hallId);

            return CreatedAtAction(nameof(GetById), new { id = hallId }, hall);
        }

        [HttpPut("{hallId}")]
        public async Task<IActionResult> Update([FromRoute] int hallId, [FromForm] HallUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = hallId;
            var affectedResult = await _hallService.Update(request);
            if (affectedResult == 0)
                BadRequest();

            return Ok();
        }

        [HttpDelete("{hallId}")]
        public async Task<IActionResult> Delete([FromRoute] int hallId, [FromQuery]HallDeleteRequest request)
        {
            request.Id = hallId;
            var affectedResult = await _hallService.Delete(request);
            if (affectedResult == 0)
                BadRequest();

            return Ok();
        }

        //Images
        [HttpGet("images/{hallId}")]
        public async Task<IActionResult> GetImagesbyHall([FromRoute] int hallId)
        {
            var images = await _hallService.GetImagesByHall(hallId);
            if (images == null)
                BadRequest("Không tìm thấy ảnh bạn cần");
            return Ok(images);
        }

        [HttpPost("image")]
        public async Task<IActionResult> AddImage(int hallId, [FromBody] HallImageCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var imageId = await _hallService.AddImages(hallId, request);
            if (imageId == 0)
                return BadRequest();
            var image = await _hallService.GetImageById(imageId);
            return CreatedAtAction(nameof(GetImageById), new { id = imageId }, image);
        }

        [HttpPut("image/{imageId}")]
        public async Task<IActionResult> UpdateImage(int imageId, [FromBody] HallImageUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _hallService.UpdateImage(imageId, request);
            if (result == 0)
                return BadRequest();

            return Ok();
        }

        [HttpDelete("image/{imageId}")]
        public async Task<IActionResult> RemoveImage(int imageId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _hallService.RemoveImage(imageId);
            if (result == 0)
                return BadRequest();

            return Ok();
        }

        [HttpGet("image/{imageId}")]
        public async Task<IActionResult> GetImageById(int imageId)
        {
            var image = await _hallService.GetImageById(imageId);
            if (image == null)
                BadRequest("Không tìm thấy ảnh bạn cần");
            return Ok(image);
        }
    }
}
