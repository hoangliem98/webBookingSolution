using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.Application.Common;
using webBookingSolution.Data.EF;
using webBookingSolution.Data.Entities;
using webBookingSolution.Utilities.Exceptions;
using webBookingSolution.ViewModels.Catalog;
using webBookingSolution.ViewModels.Catalog.HallImages;
using webBookingSolution.ViewModels.Catalog.Halls;
using webBookingSolution.ViewModels.Common;

namespace webBookingSolution.Application.Catalog.Halls
{
    public class HallService : IHallService
    {
        private readonly BookingDBContext _context;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _config;
        private readonly IConvertToUnSign _convert;
        public HallService(BookingDBContext context, IStorageService storageService, IConfiguration config, IConvertToUnSign convert)
        {
            _convert = convert;
            _config = config;
            _context = context;
            _storageService = storageService;
        }

        public async Task<int> Create(HallCreateRequest request)
        {
            var hall = new Hall()
            {
                Name = request.Name,
                Description = request.Description,
                MinimumTables = request.MinimumTables,
                MaximumTables = request.MaximumTables,
                Price = request.Price
            };

            //SaveImage
            if(request.ThumbnailImage != null)
            {
                hall.HallImages = new List<HallImage>();
                foreach (var item in request.ThumbnailImage)
                {
                    hall.HallImages.Add(
                         new HallImage()
                         {
                             Caption = "Thumbnail Image",
                             DateCreated = DateTime.Now,
                             Size = item.Length,
                             Path = await this.SaveFile(item),
                             IsDefault = true,
                         }
                     );
                }
            }
            _context.Halls.Add(hall);
            await _context.SaveChangesAsync();
            return hall.Id;
        }

        public async Task<int> Delete(HallDeleteRequest request)
        {
            var hall = await _context.Halls.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (hall == null) throw new BookingException($"Không thể tìm được sảnh: {request.Id}");
            _context.Halls.Remove(hall);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<HallViewModel>> GetAll()
        {
            var query = from p in _context.Halls
                        select new { p };

            //int totalRow = await query.CountAsync();
            var tmpData = await query.Select(x => new HallViewModel()
            {
                Id = x.p.Id,
                Name = x.p.Name,
                Description = x.p.Description,
                MinimumTables = x.p.MinimumTables,
                MaximumTables = x.p.MaximumTables,
                Price = x.p.Price
            }).ToListAsync();

            var data = new List<HallViewModel>();

            foreach (var item in tmpData)
            {
                var image = await GetImagesByHall(item.Id);
                data.Add(
                    new HallViewModel()
                    {
                        Id = item.Id,
                        ListImage = image,
                        Name = item.Name,
                        Description = item.Description,
                        MinimumTables = item.MinimumTables,
                        MaximumTables = item.MaximumTables,
                        Price = item.Price,
                    }
                );
            }
            return data;
        }

        public async Task<PagedResult<HallViewModel>> GetAllPaging(GetHallPagingRequest request)
        {
            var query = from p in _context.Halls
                        select new { p };

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.p.Name.Contains(request.Keyword) ||
                                x.p.Description.Contains(request.Keyword) ||
                                x.p.Price.ToString().Contains(request.Keyword));
            }

            int totalRow = await query.CountAsync();
            var tmpData = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new HallViewModel()
                {
                    Id = x.p.Id,
                    Name = x.p.Name,
                    Description = x.p.Description,
                    MinimumTables = x.p.MinimumTables,
                    MaximumTables = x.p.MaximumTables,
                    Price = x.p.Price,             
                }).ToListAsync();

            var data = new List<HallViewModel>();

            foreach (var item in tmpData)
            {
                var image = await GetImagesByHall(item.Id);
                data.Add(
                    new HallViewModel()
                    {
                        Id = item.Id,
                        ListImage = image,
                        Name = item.Name,
                        Description = item.Description,
                        MinimumTables = item.MinimumTables,
                        MaximumTables = item.MaximumTables,
                        Price = item.Price,
                    }
                );
            }

            var pageResult = new PagedResult<HallViewModel>()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalRecords = totalRow,
                Items = data
            };
            return pageResult;
        }

        public async Task<HallViewModel> GetById(int id)
        {
            var hall = await _context.Halls.FirstOrDefaultAsync(x => x.Id == id);
            if (hall == null)
                return null;
            var query = from hi in _context.HallImages
                        where hi.HallId == hall.Id
                        select new { hi };

            var image = await query.Select(x => new HallImageViewModel()
            {
                Path = _config["PathImage"] + x.hi.Path,
            }).ToListAsync();

            var hallViewmodel = new HallViewModel()
            {
                Id = hall.Id,
                Name = hall.Name,
                Description = hall.Description,
                MinimumTables = hall.MinimumTables,
                MaximumTables = hall.MaximumTables,
                Price = hall.Price,
                ListImage = image
            };
            return hallViewmodel;
        }

        public async Task<int> Update(HallUpdateRequest request)
        {
            var hall = await _context.Halls.FindAsync(request.Id);
            if (hall == null) throw new BookingException($"Không tìm thấy sảnh {request.Id}");

            hall.Name = request.Name;
            hall.Description = request.Description;
            hall.MinimumTables = request.MinimumTables;
            hall.MaximumTables = request.MaximumTables;
            hall.Price = request.Price;

            //Save Image
            //if (request.ThumbnailImage != null)
            //{
            //    var thumbnailImage = await _context.HallImages.FirstOrDefaultAsync(i => i.IsDefault == true && i.HallId == request.Id);

            //    var query = from hi in _context.HallImages
            //                where hi.HallId == hall.Id
            //                select new { hi };

            //    var image = await query.Select(x => new HallImageViewModel()
            //    {
            //        Size = x.hi.Size,
            //        Path = x.hi.Path,
            //    }).ToListAsync();

            //    if (thumbnailImage != null)
            //    {
            //        thumbnailImage.Path = await this.SaveFile(request.ThumbnailImage);
            //        thumbnailImage.Size = request.ThumbnailImage.Length;
            //        _context.HallImages.Update(thumbnailImage);
            //    } 
            //}
            if (request.ThumbnailImage != null)
            {
                hall.HallImages = new List<HallImage>();
                foreach (var item in request.ThumbnailImage)
                {
                    hall.HallImages.Add(
                         new HallImage()
                         {
                             Caption = "Thumbnail Image",
                             DateCreated = DateTime.Now,
                             Size = item.Length,
                             Path = await this.SaveFile(item),
                             IsDefault = true,
                         }
                     );
                }
            }

            return await _context.SaveChangesAsync();
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var ogName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ogName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }

        //Image Request
        public async Task<int> AddImages(int hallId, HallImageCreateRequest request)
        {
            var hallImage = new HallImage()
            {
                Caption = request.Caption,
                DateCreated = DateTime.Now,
                IsDefault = request.IsDefault,
                HallId = hallId,
            };
            if(request.ImageFile != null)
            {
                hallImage.Path = await this.SaveFile(request.ImageFile);
                hallImage.Size = request.ImageFile.Length;
            }
            _context.HallImages.Add(hallImage);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateImage(int hallId, HallImageUpdateRequest request)
        {
            var hallImage = await _context.HallImages.FirstOrDefaultAsync(x => x.HallId == hallId);
            if (hallImage == null)
                throw new BookingException($"Không tìm thấy ảnh {hallId}");

            if (request.ImageFile != null)
            {
                hallImage.Path = await this.SaveFile(request.ImageFile);
                hallImage.Size = request.ImageFile.Length;
            }
            _context.HallImages.Update(hallImage);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> RemoveImage(int hallId)
        {
            var hallImage = await _context.HallImages.FirstOrDefaultAsync(x => x.HallId == hallId);
            if (hallImage == null)
                throw new BookingException($"Không tìm thấy ảnh {hallId}");
            _context.HallImages.Remove(hallImage);
            await _context.SaveChangesAsync();
            return hallImage.Id;
        }

        public async Task<List<HallImageViewModel>> GetImagesByHall(int hallId)
        {
            var query = from hi in _context.HallImages
                        where hi.HallId == hallId
                        select new { hi };
            var image = await query.Select(i => new HallImageViewModel()
            {
                Caption = i.hi.Caption,
                DateCreated = i.hi.DateCreated,
                Size = i.hi.Size,
                Id = i.hi.Id,
                Path = _config["PathImage"] + i.hi.Path,
                IsDefault = i.hi.IsDefault,
                HallId = i.hi.HallId
            }).ToListAsync();
            return image;
        }

        public async Task<HallImageViewModel> GetImageById(int imageId)
        {
            var image = await _context.HallImages.FirstOrDefaultAsync(x => x.Id == imageId);
            if (image == null)
                throw new BookingException($"Không tìm thấy ảnh {imageId}");
            var viewModel = new HallImageViewModel()
            {
                Caption = image.Caption,
                DateCreated = image.DateCreated,
                Size = image.Size,
                Id = image.Id,
                Path = _config["PathImage"] + image.Path,
                IsDefault = image.IsDefault,
                HallId = image.HallId
            };
            return viewModel;
        }
    }
}
