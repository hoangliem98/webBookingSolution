using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Users;
using webBookingSolution.ViewModels.System;

namespace webBookingSolution.Api.Users
{
    public interface IUserApiClient
    {
        Task<string> Authencate(LoginRequest request);
        Task<string> UserLogin(LoginRequest request);
        Task<List<UserViewModel>> GetAll();
        Task<UserViewModel> GetById(int id);
        Task<string> Register(RegisterRequest request);
        Task<string> Create(RegisterRequest request);
        Task<string> ChangePassword(ChangePasswordRequest request);
        Task<bool> Delete(UserDeleteRequest request);
    }
}
