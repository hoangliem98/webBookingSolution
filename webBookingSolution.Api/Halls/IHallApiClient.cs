using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.HallImages;
using webBookingSolution.ViewModels.Catalog.Halls;
using webBookingSolution.ViewModels.Common;

namespace webBookingSolution.Api.Halls
{
    public interface IHallApiClient
    {
        Task<List<HallViewModel>> GetAll();
        Task<PagedResult<HallViewModel>> GetAllPaging(GetHallPagingRequest request);
        Task<HallViewModel> GetById(int id);
        Task<List<ViewModels.Catalog.HallImages.HallImageViewModel>> GetImagesByHall(int hallId);
        //Task<HallImageViewModel> UpdateImage(int hallId, HallImageUpdateRequest request);
        Task<bool> Create(HallCreateRequest request);
        Task<bool> Update(HallUpdateRequest request);
        Task<bool> Delete(HallDeleteRequest request);
    }
}
