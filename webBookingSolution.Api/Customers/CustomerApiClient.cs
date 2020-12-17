using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Customers;
using webBookingSolution.Api.Customers;

namespace webBookingSolution.Api.Customers
{
    public class CustomerApiClient : ICustomerApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public CustomerApiClient(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> Create(CustomerCreateRequest request)
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

            requestContent.Add(new StringContent(request.FirstName), "firstname");
            requestContent.Add(new StringContent(request.LastName), "lastname");
            requestContent.Add(new StringContent(request.DOB.ToString()), "dob");
            requestContent.Add(new StringContent(request.PhoneNumber), "phonenumber");
            requestContent.Add(new StringContent(request.Email), "email");
            requestContent.Add(new StringContent(request.Address), "address");

            var response = await client.PostAsync($"/api/customer", requestContent);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.DeleteAsync($"/api/customer/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<CustomerViewModel>> GetAll(int month)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/customer?month={month}");
            var body = await response.Content.ReadAsStringAsync();
            var customers = JsonConvert.DeserializeObject<List<CustomerViewModel>>(body);
            return customers;
        }

        public async Task<CustomerViewModel> GetById(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/customer/{id}");
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CustomerViewModel>(body);
        }

        public async Task<string> Update(CustomerUpdateRequest request)
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

            var response = await client.PutAsync($"/api/customer/{request.Id}", requestContent);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
