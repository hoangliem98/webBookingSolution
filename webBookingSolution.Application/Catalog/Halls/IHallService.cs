using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog;
using webBookingSolution.ViewModels.Catalog.HallImages;
using webBookingSolution.ViewModels.Catalog.Halls;
using webBookingSolution.ViewModels.Common;

namespace webBookingSolution.Application.Catalog.Halls
{
    public interface IHallService
    {
        //Hall Request
        Task<int> Create(HallCreateRequest request);

        Task<int> Update(HallUpdateRequest request);

        Task<int> Delete(HallDeleteRequest request);

        Task<List<HallViewModel>> GetAll();

        Task<HallViewModel> GetById(int id);

        Task<PagedResult<HallViewModel>> GetAllPaging(GetHallPagingRequest request);


        //Image Request
        Task<int> AddImages(int hallId, HallImageCreateRequest request);

        Task<int> UpdateImage(int hallId, HallImageUpdateRequest request);

        Task<int> RemoveImage(int hallId);

        Task<List<ViewModels.Catalog.HallImages.HallImageViewModel>> GetImagesByHall(int hallId);

        Task<ViewModels.Catalog.HallImages.HallImageViewModel> GetImageById(int imageId);
    }
}
