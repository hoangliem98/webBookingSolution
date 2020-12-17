using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using webBookingSolution.ViewModels.Catalog.Dashboard;

namespace webBookingSolution.Api.Dashboard
{
    public interface IDashboardApiClient
    {
        Task<List<int>> GetListYear();
        Task<DashboardViewModel> Statistic(int year);
    }
}
