using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Customers;

namespace webBookingSolution.Api.Customers
{
    public interface ICustomerApiClient
    {
        Task<List<CustomerViewModel>> GetAll(int month);
        Task<CustomerViewModel> GetById(int id);
        Task<string> Create(CustomerCreateRequest request);
        Task<string> Update(CustomerUpdateRequest request);
        Task<bool> Delete(int id);
    }
}
