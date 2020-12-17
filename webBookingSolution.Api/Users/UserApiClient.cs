using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Users;
using webBookingSolution.ViewModels.System;

namespace webBookingSolution.Api.Users
{
    public class UserApiClient : IUserApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public UserApiClient(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<string> Authencate(LoginRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContext = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.PostAsync("/api/user/authenticate", httpContext);
            var token = await response.Content.ReadAsStringAsync();
            return token;
        }
        public async Task<List<UserViewModel>> GetAll()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/user");
            var body = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<UserViewModel>>(body);
            return users;
        }

        public async Task<string> Create(RegisterRequest request)
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

            requestContent.Add(new StringContent(request.UserName), "username");
            requestContent.Add(new StringContent(request.Password), "password");
            requestContent.Add(new StringContent(request.ConfirmPassword), "confirmpassword");
            requestContent.Add(new StringContent(request.Roles.ToString()), "roles");
            requestContent.Add(new StringContent(request.FirstName), "firstname");
            requestContent.Add(new StringContent(request.LastName), "lastname");
            requestContent.Add(new StringContent(request.DOB.ToString()), "dob");
            requestContent.Add(new StringContent(request.PhoneNumber), "phonenumber");
            requestContent.Add(new StringContent(request.Email), "email");
            requestContent.Add(new StringContent(request.Address), "address");
            requestContent.Add(new StringContent(request.AccountId.ToString()), "accountid");

            var response = await client.PostAsync($"/api/user", requestContent);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> Delete(UserDeleteRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.DeleteAsync($"/api/user/{request.Id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<UserViewModel> GetById(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/user/{id}");
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserViewModel>(body);
        }

        public async Task<string> ChangePassword(ChangePasswordRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);

            var requestContent = new MultipartFormDataContent();

            requestContent.Add(new StringContent(request.Id.ToString()), "id");
            requestContent.Add(new StringContent(request.CurrentPassword), "currentpassword");
            requestContent.Add(new StringContent(request.Password), "password");
            requestContent.Add(new StringContent(request.ConfirmPassword), "confirmpassword");
            requestContent.Add(new StringContent(request.UserName), "username");
            requestContent.Add(new StringContent(request.Roles.ToString()), "roles");
            requestContent.Add(new StringContent(request.EmOrCusId.ToString()), "emorcusid");

            var response = await client.PutAsync($"/api/user/{request.Id}", requestContent);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> UserLogin(LoginRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContext = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.PostAsync("/api/user/userlogin", httpContext);
            var token = await response.Content.ReadAsStringAsync();
            return token;
        }

        public async Task<string> Register(RegisterRequest request)
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

            requestContent.Add(new StringContent(request.UserName), "username");
            requestContent.Add(new StringContent(request.Password), "password");
            requestContent.Add(new StringContent(request.ConfirmPassword), "confirmpassword");
            requestContent.Add(new StringContent(request.Roles.ToString()), "roles");
            requestContent.Add(new StringContent(request.FirstName), "firstname");
            requestContent.Add(new StringContent(request.LastName), "lastname");
            requestContent.Add(new StringContent(request.DOB.ToString()), "dob");
            requestContent.Add(new StringContent(request.PhoneNumber), "phonenumber");
            requestContent.Add(new StringContent(request.Email), "email");
            requestContent.Add(new StringContent(request.Address), "address");
            requestContent.Add(new StringContent(request.AccountId.ToString()), "accountid");

            var response = await client.PostAsync($"/api/user/register", requestContent);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
