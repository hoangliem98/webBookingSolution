using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Payments;

namespace webBookingSolution.Api.Payments
{
    public class PaymentApiClient : IPaymentApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public PaymentApiClient(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> Create(PaymentCreateRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContext = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.PostAsync("/api/payment", httpContext);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<List<PaymentViewModel>> GetAll()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var response = await client.GetAsync("/api/payment");
            var body = await response.Content.ReadAsStringAsync();
            var payments = JsonConvert.DeserializeObject<List<PaymentViewModel>>(body);
            return payments;
        }
    }
}
