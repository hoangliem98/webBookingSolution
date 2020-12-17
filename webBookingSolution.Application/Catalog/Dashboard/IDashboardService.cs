using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Dashboard;

namespace webBookingSolution.Application.Catalog.Dashboard
{
    public interface IDashboardService
    {
        Task<List<int>> GetListYear();
        Task<DashboardViewModel> Statistic(int year);
    }
}
