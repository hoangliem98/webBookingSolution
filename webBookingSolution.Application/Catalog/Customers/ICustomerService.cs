using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Customers;
using webBookingSolution.ViewModels.Common;

namespace webBookingSolution.Application.Catalog.Customers
{
    public interface ICustomerService
    {
        Task<CustomerViewModel> GetCustomerByAccount(int accountId);

        Task<int> Create(CustomerCreateRequest request);

        Task<int> Update(CustomerUpdateRequest request);

        Task<int> Delete(int id);

        Task<List<CustomerViewModel>> GetAll(int month);

        Task<CustomerViewModel> GetById(int id);

        //Task<PagedResult<CustomerViewModel>> GetAllPaging(GetCustomerPagingRequest request);
    }
}
