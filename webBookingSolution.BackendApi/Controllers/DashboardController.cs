using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webBookingSolution.Application.Catalog.Dashboard;

namespace webBookingSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetListYear()
        {
            var years = await _dashboardService.GetListYear();
            return Ok(years);
        }

        [HttpGet("{year}")]
        public async Task<IActionResult> Statistic(int year)
        {
            var dashboard = await _dashboardService.Statistic(year);
            return Ok(dashboard);
        }
    }
}
