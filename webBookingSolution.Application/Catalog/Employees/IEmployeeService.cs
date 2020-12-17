using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.Data.Entities;
using webBookingSolution.ViewModels.Catalog.Employees;

namespace webBookingSolution.Application.Catalog.Employees
{
    public interface IEmployeeService
    {
        Task<EmployeeViewModel> GetEmployeeByAccount(int accountId);

        Task<int> Update(EmployeeUpdateRequest request);

        Task<int> Delete(EmployeeDeleteRequest request);

        Task<List<EmployeeViewModel>> GetAll(int month);

        Task<EmployeeViewModel> GetById(int id);

        Task<List<int>> GetListMonth();
    }
}
