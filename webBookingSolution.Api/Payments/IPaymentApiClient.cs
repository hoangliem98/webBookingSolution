using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Payments;

namespace webBookingSolution.Api.Payments
{
    public interface IPaymentApiClient
    {
        Task<string> Create(PaymentCreateRequest request);
        Task<List<PaymentViewModel>> GetAll();
    }
}
