using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Books;

namespace webBookingSolution.Api.Books
{
    public class BookApiClient : IBookApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public BookApiClient(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> Create(BookCreateRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);

            var requestContent = new MultipartFormDataContent();

            if (request.customerCreateRequest != null)
            {
                if (request.customerCreateRequest.Image != null)
                {
                    byte[] data;
                    using (var br = new BinaryReader(request.customerCreateRequest.Image.OpenReadStream()))
                    {
                        data = br.ReadBytes((int)request.customerCreateRequest.Image.OpenReadStream().Length);
                    }
                    ByteArrayContent bytes = new ByteArrayContent(data);
                    requestContent.Add(bytes, "customerCreateRequest.Image", request.customerCreateRequest.Image.FileName);
                }

                requestContent.Add(new StringContent(request.customerCreateRequest.FirstName), "customerCreateRequest.firstname");
                requestContent.Add(new StringContent(request.customerCreateRequest.LastName), "customerCreateRequest.lastname");
                requestContent.Add(new StringContent(request.customerCreateRequest.DOB.ToString()), "customerCreateRequest.dob");
                requestContent.Add(new StringContent(request.customerCreateRequest.PhoneNumber), "customerCreateRequest.phonenumber");
                requestContent.Add(new StringContent(request.customerCreateRequest.Email), "customerCreateRequest.email");
                requestContent.Add(new StringContent(request.customerCreateRequest.Address), "customerCreateRequest.address");
            }

            foreach (var item in request.Service)
            {
                if (item.IsChecked == true)
                {
                    requestContent.Add(new ByteArrayContent(Encoding.UTF8.GetBytes(item.Id.ToString())), "ServiceId");
                }
            }

            requestContent.Add(new StringContent(request.NumberTables.ToString()), "numbertables");
            requestContent.Add(new StringContent(request.OrganizationDate.ToString()), "organizationdate");
            requestContent.Add(new StringContent(request.BookDate.ToString()), "organizationdate");
            requestContent.Add(new StringContent(request.Season), "season");
            requestContent.Add(new StringContent(request.HallId.ToString()), "hallid");
            requestContent.Add(new StringContent(request.MenuId.ToString()), "menuid");
            requestContent.Add(new StringContent(request.CustomerId.ToString()), "customerid");

            var response = await client.PostAsync($"/api/book", requestContent);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<List<BookViewModel>> GetAll()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/book");
            var body = await response.Content.ReadAsStringAsync();
            var books = JsonConvert.DeserializeObject<List<BookViewModel>>(body);
            return books;
        }

        public async Task<BookViewModel> GetById(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/book/{id}");
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BookViewModel>(body);
        }
    }
}
