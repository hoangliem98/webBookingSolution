using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Payments;

namespace webBookingSolution.Application.Catalog.Payments
{
    public interface IPaymentService
    {
        Task<List<PaymentViewModel>> GetAll();
        Task<int> Create(PaymentCreateRequest request);
    }
}
