using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Services;
using webBookingSolution.ViewModels.Common;

namespace webBookingSolution.Api.Sv
{
    public class ServiceApiClient : IServiceApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public ServiceApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<bool> Create(ServiceCreateRequest request)
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
            requestContent.Add(new StringContent(request.Description), "description");
            requestContent.Add(new StringContent(request.Price.ToString()), "price");

            var response = await client.PostAsync($"/api/service", requestContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(ServiceDeleteRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.DeleteAsync($"/api/service/{request.Id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<ServiceViewModel>> GetAll()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/service");
            var body = await response.Content.ReadAsStringAsync();
            var services = JsonConvert.DeserializeObject<List<ServiceViewModel>>(body);
            return services;
        }

        public async Task<PagedResult<ServiceViewModel>> GetAllPaging(GetServicePagingRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/service/paging?pageIndex=" +
                $"{request.PageIndex}&pageSize={request.PageSize}&keyword={request.Keyword}");
            var body = await response.Content.ReadAsStringAsync();
            var services = JsonConvert.DeserializeObject<PagedResult<ServiceViewModel>>(body);
            return services;
        }

        public async Task<ServiceViewModel> GetById(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/service/{id}");
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ServiceViewModel>(body);
        }

        public async Task<bool> Update(ServiceUpdateRequest request)
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
            requestContent.Add(new StringContent(request.Description), "description");
            requestContent.Add(new StringContent(request.Price.ToString()), "price");

            var response = await client.PutAsync($"/api/service/{request.Id}", requestContent);
            return response.IsSuccessStatusCode;
        }
    }
}
