using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.HallImages;
using webBookingSolution.ViewModels.Catalog.Halls;
using webBookingSolution.ViewModels.Common;

namespace webBookingSolution.Api.Halls
{
    public class HallApiClient : IHallApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public HallApiClient(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> Create(HallCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);

            var requestContent = new MultipartFormDataContent();
            if (request.ThumbnailImage != null)
            {
                foreach(var item in request.ThumbnailImage)
                {
                    byte[] data;
                    using (var br = new BinaryReader(item.OpenReadStream()))
                    {
                        data = br.ReadBytes((int)item.OpenReadStream().Length);
                    }
                    ByteArrayContent bytes = new ByteArrayContent(data);
                    requestContent.Add(bytes, "ThumbnailImage", item.FileName);
                }
            }

            requestContent.Add(new StringContent(request.Name), "name");
            requestContent.Add(new StringContent(request.Description), "description");
            requestContent.Add(new StringContent(request.MinimumTables.ToString()), "minimumTables");
            requestContent.Add(new StringContent(request.MaximumTables.ToString()), "maximumTables");
            requestContent.Add(new StringContent(request.Price.ToString()), "price");

            var response = await client.PostAsync($"/api/hall", requestContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(HallDeleteRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.DeleteAsync($"/api/hall/{request.Id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<HallViewModel>> GetAll()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/hall");
            var body = await response.Content.ReadAsStringAsync();
            var halls = JsonConvert.DeserializeObject<List<HallViewModel>>(body);
            return halls;
        }

        public async Task<PagedResult<HallViewModel>> GetAllPaging(GetHallPagingRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/hall/paging?pageIndex=" + 
                $"{request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}");
            var body = await response.Content.ReadAsStringAsync();
            var halls = JsonConvert.DeserializeObject<PagedResult<HallViewModel>>(body);
            return halls;
        }

        public async Task<HallViewModel> GetById(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/hall/{id}");
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<HallViewModel>(body);
        }

        public async Task<List<HallImageViewModel>> GetImagesByHall(int hallId)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/hall/images/{hallId}");
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<HallImageViewModel>>(body);
        }

        public async Task<bool> Update(HallUpdateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);

            var requestContent = new MultipartFormDataContent();
            if (request.ThumbnailImage != null)
            {
                foreach (var item in request.ThumbnailImage)
                {
                    byte[] data;
                    using (var br = new BinaryReader(item.OpenReadStream()))
                    {
                        data = br.ReadBytes((int)item.OpenReadStream().Length);
                    }
                    ByteArrayContent bytes = new ByteArrayContent(data);
                    requestContent.Add(bytes, "ThumbnailImage", item.FileName);
                }
            }

            requestContent.Add(new StringContent(request.Id.ToString()), "id");
            requestContent.Add(new StringContent(request.Name), "name");
            requestContent.Add(new StringContent(request.Description), "description");
            requestContent.Add(new StringContent(request.MinimumTables.ToString()), "minimumTables");
            requestContent.Add(new StringContent(request.MaximumTables.ToString()), "maximumTables");
            requestContent.Add(new StringContent(request.Price.ToString()), "price");

            var response = await client.PutAsync($"/api/hall/{request.Id}", requestContent);
            return response.IsSuccessStatusCode;
        }
    }
}
