using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using webBookingSolution.Api.Dashboard;
using webBookingSolution.ViewModels.Catalog.Dashboard;
using webBookingSolution.WebAdmin.Models;

namespace webBookingSolution.WebAdmin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDashboardApiClient _dashboardApiClient;

        public HomeController(ILogger<HomeController> logger, IDashboardApiClient dashboardApiClient)
        {
            _logger = logger;
            _dashboardApiClient = dashboardApiClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ErrorPage()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(int year)
        {
            int current = DateTime.Now.Year;
            var dashboard = await _dashboardApiClient.Statistic(year == 0 ? current : year);
            var years = await _dashboardApiClient.GetListYear();
            ViewBag.Years = years.Select(x => new SelectListItem()
            {
                Text = x.ToString(),
                Value = x.ToString(),
                Selected = year == x,
            });
            var dashboardViewModel = new DashboardViewModel()
            {
                Jan = dashboard.Jan,
                Feb = dashboard.Feb,
                Mar = dashboard.Mar,
                Apr = dashboard.Apr,
                May = dashboard.May,
                Jun = dashboard.Jun,
                Jul = dashboard.Jul,
                Aug = dashboard.Aug,
                Sep = dashboard.Sep,
                Oct = dashboard.Oct,
                Nov = dashboard.Nov,
                Dec = dashboard.Dec,
            };
            return View(dashboardViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
