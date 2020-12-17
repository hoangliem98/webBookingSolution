using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog;
using webBookingSolution.ViewModels.Catalog.Menus;
using webBookingSolution.ViewModels.Common;

namespace webBookingSolution.Application.Catalog.Menus
{
    public interface IMenuService
    {
        //Menu Request
        Task<int> Create(MenuCreateRequest request);

        Task<int> Update(MenuUpdateRequest request);

        Task<int> Delete(MenuDeleteRequest request);

        Task<List<MenuViewModel>> GetAll();

        Task<MenuViewModel> GetById(int id);

        Task<PagedResult<MenuViewModel>> GetAllPaging(GetMenuPagingRequest request);
    }
}
