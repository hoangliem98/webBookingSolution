using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Employees;

namespace webBookingSolution.Api.Employees
{
    public interface IEmployeeApiClient
    {
        Task<List<EmployeeViewModel>> GetAll(int month);
        Task<EmployeeViewModel> GetById(int id);
        Task<string> Update(EmployeeUpdateRequest request);
        Task<bool> Delete(EmployeeDeleteRequest request);
        Task<List<int>> GetListMonth();
    }
}
