using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog;
using webBookingSolution.ViewModels.Catalog.Services;
using webBookingSolution.ViewModels.Common;

namespace webBookingSolution.Application.Catalog.Services
{
    public interface ISvService
    {
        //Service Request
        Task<int> Create(ServiceCreateRequest request);

        Task<int> Update(ServiceUpdateRequest request);

        Task<int> Delete(ServiceDeleteRequest request);

        Task<List<ServiceViewModel>> GetAll();

        Task<ServiceViewModel> GetById(int id);

        Task<PagedResult<ServiceViewModel>> GetAllPaging(GetServicePagingRequest request);
    }
}
