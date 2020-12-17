using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Menus;
using webBookingSolution.ViewModels.Common;

namespace webBookingSolution.Api.Menus
{
    public class MenuApiClient : IMenuApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public MenuApiClient(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> Create(MenuCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);

            var requestContent = new MultipartFormDataContent();
            if (request.Image != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.Image.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.Image.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "image", request.Image.FileName);
            }

            requestContent.Add(new StringContent(request.Name), "name");
            requestContent.Add(new StringContent(request.Content), "content");
            requestContent.Add(new StringContent(request.Price.ToString()), "price");

            var response = await client.PostAsync($"/api/menu", requestContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(MenuDeleteRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.DeleteAsync($"/api/menu/{request.Id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<MenuViewModel>> GetAll()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/menu");
            var body = await response.Content.ReadAsStringAsync();
            var menus = JsonConvert.DeserializeObject<List<MenuViewModel>>(body);
            return menus;
        }

        public async Task<PagedResult<MenuViewModel>> GetAllPaging(GetMenuPagingRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/menu/paging?pageIndex=" +
                $"{request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}");
            var body = await response.Content.ReadAsStringAsync();
            var menus = JsonConvert.DeserializeObject<PagedResult<MenuViewModel>>(body);
            return menus;
        }

        public async Task<MenuViewModel> GetById(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/menu/{id}");
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MenuViewModel>(body);
        }

        public async Task<bool> Update(MenuUpdateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);

            var requestContent = new MultipartFormDataContent();
            if (request.Image != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.Image.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.Image.OpenReadStream().Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                requestContent.Add(bytes, "image", request.Image.FileName);
            }

            requestContent.Add(new StringContent(request.Id.ToString()), "id");
            requestContent.Add(new StringContent(request.Name), "name");
            requestContent.Add(new StringContent(request.Content), "content");
            requestContent.Add(new StringContent(request.Price.ToString()), "price");

            var response = await client.PutAsync($"/api/menu/{request.Id}", requestContent);
            return response.IsSuccessStatusCode;
        }
    }
}
