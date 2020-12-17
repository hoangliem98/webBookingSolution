using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Employees;

namespace webBookingSolution.Api.Employees
{
    public class EmployeeApiClient : IEmployeeApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public EmployeeApiClient(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> Delete(EmployeeDeleteRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.DeleteAsync($"/api/employee/{request.Id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<EmployeeViewModel>> GetAll(int month)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/employee?month=" + $"{month}");
            var body = await response.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<List<EmployeeViewModel>>(body);
            return employees;
        }

        public async Task<EmployeeViewModel> GetById(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/employee/{id}");
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<EmployeeViewModel>(body);
        }

        public async Task<List<int>> GetListMonth()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/employee/listmonth");
            var body = await response.Content.ReadAsStringAsync();
            var months = JsonConvert.DeserializeObject<List<int>>(body);
            return months;
        }

        public async Task<string> Update(EmployeeUpdateRequest request)
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
            requestContent.Add(new StringContent(request.FirstName), "firstname");
            requestContent.Add(new StringContent(request.LastName), "lastname");
            requestContent.Add(new StringContent(request.DOB.ToString()), "dob");
            requestContent.Add(new StringContent(request.PhoneNumber), "phonenumber");
            requestContent.Add(new StringContent(request.Email), "email");
            requestContent.Add(new StringContent(request.Address), "address");

            var response = await client.PutAsync($"/api/employee/{request.Id}", requestContent);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
