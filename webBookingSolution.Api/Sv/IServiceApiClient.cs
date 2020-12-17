using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Services;
using webBookingSolution.ViewModels.Common;

namespace webBookingSolution.Api.Sv
{
    public interface IServiceApiClient
    {
        Task<List<ServiceViewModel>> GetAll();
        Task<ServiceViewModel> GetById(int id);
        Task<PagedResult<ServiceViewModel>> GetAllPaging(GetServicePagingRequest request);
        Task<bool> Create(ServiceCreateRequest request);
        Task<bool> Update(ServiceUpdateRequest request);
        Task<bool> Delete(ServiceDeleteRequest request);
    }
}
