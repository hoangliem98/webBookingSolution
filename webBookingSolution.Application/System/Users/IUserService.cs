using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.Data.Entities;
using webBookingSolution.ViewModels.Catalog.Customers;
using webBookingSolution.ViewModels.Catalog.Employees;
using webBookingSolution.ViewModels.Catalog.Users;
using webBookingSolution.ViewModels.System;

namespace webBookingSolution.Application.System.Users
{
    public interface IUserService
    {
        Task<string> Authencate(LoginRequest request);
        Task<string> UserLogin(LoginRequest request);
        Task<int> Register(RegisterRequest request);
        Task<UserViewModel> GetAccountByUserName(string username);
        Task<bool> CheckEmployeeContain(string email, string phoneNumber);
        Task<bool> CheckCustomerContain(string email, string phoneNumber);
        Task<List<UserViewModel>> GetAll();
        Task<int> CreateUser(RegisterRequest request);
        Task<int> ChangePassword(ChangePasswordRequest request);
        Task<int> DeleteUser(UserDeleteRequest request);
        Task<UserViewModel> GetById(int id);
    }
}
