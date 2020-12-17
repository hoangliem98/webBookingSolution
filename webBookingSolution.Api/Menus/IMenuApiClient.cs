using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Menus;
using webBookingSolution.ViewModels.Common;

namespace webBookingSolution.Api.Menus
{
    public interface IMenuApiClient
    {
        Task<List<MenuViewModel>> GetAll();
        Task<MenuViewModel> GetById(int id);
        Task<PagedResult<MenuViewModel>> GetAllPaging(GetMenuPagingRequest request);
        Task<bool> Create(MenuCreateRequest request);
        Task<bool> Update(MenuUpdateRequest request);
        Task<bool> Delete(MenuDeleteRequest request);
    }
}
