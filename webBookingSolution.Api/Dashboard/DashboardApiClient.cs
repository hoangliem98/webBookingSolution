using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Dashboard;

namespace webBookingSolution.Api.Dashboard
{
    public class DashboardApiClient : IDashboardApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public DashboardApiClient(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<int>> GetListYear()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/dashboard");
            var body = await response.Content.ReadAsStringAsync();
            var years = JsonConvert.DeserializeObject<List<int>>(body);
            return years;
        }

        public async Task<DashboardViewModel> Statistic(int year)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync($"/api/dashboard/{year}");
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DashboardViewModel>(body);
        }
    }
}
