using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using webBookingSolution.Data.EF;
using webBookingSolution.ViewModels.Catalog.Dashboard;
using Microsoft.EntityFrameworkCore;
using webBookingSolution.ViewModels.Catalog.Payments;

namespace webBookingSolution.Application.Catalog.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly BookingDBContext _context;

        public DashboardService(BookingDBContext context)
        {
            _context = context;
        }

        public async Task<List<int>> GetListYear()
        {
            var query = from p in _context.Payments
                        select new { p };

            var allYear = await query.Select(x => x.p.PaymentDate).ToListAsync();

            int minYear = DateTime.MaxValue.Year;
            int maxYear = DateTime.MinValue.Year;

            foreach (var years in allYear)
            {
                int yearItem = years.Year;

                if (yearItem < minYear)
                    minYear = yearItem;
                if (yearItem > maxYear)
                    maxYear = yearItem;
            }

            var listYear = new List<int>();

            for (int i = maxYear; i >= minYear; i--)
            {
                listYear.Add(i);
            }

            return listYear;
        }

        public async Task<DashboardViewModel> Statistic(int year)
        {
            var query = from p in _context.Payments
                        where p.PaymentDate.Year == year
                        select new { p };

            var payments = await query.Select(x => new PaymentViewModel()
            {
                TotalPrice = x.p.TotalPrice,
                PaymentDate = x.p.PaymentDate
            }).ToListAsync();

            var dashboard = new DashboardViewModel()
            {
                Jan = 0,
                Feb = 0,
                Mar = 0,
                Apr = 0,
                May = 0,
                Jun = 0,
                Jul = 0,
                Aug = 0,
                Sep = 0,
                Oct = 0,
                Nov = 0,
                Dec = 0
            };

            foreach (var payment in payments)
            {
                int month = payment.PaymentDate.Month;

                switch (month)
                {
                    case 1:
                        dashboard.Jan = dashboard.Jan + payment.TotalPrice;
                        break;
                    case 2:
                        dashboard.Feb = dashboard.Feb + payment.TotalPrice;
                        break;
                    case 3:
                        dashboard.Mar = dashboard.Mar + payment.TotalPrice;
                        break;
                    case 4:
                        dashboard.Apr = dashboard.Apr + payment.TotalPrice;
                        break;
                    case 5:
                        dashboard.May = dashboard.May + payment.TotalPrice;
                        break;
                    case 6:
                        dashboard.Jun = dashboard.Jun + payment.TotalPrice;
                        break;
                    case 7:
                        dashboard.Jul = dashboard.Jul + payment.TotalPrice;
                        break;
                    case 8:
                        dashboard.Aug = dashboard.Aug + payment.TotalPrice;
                        break;
                    case 9:
                        dashboard.Sep = dashboard.Sep + payment.TotalPrice;
                        break;
                    case 10:
                        dashboard.Oct = dashboard.Oct + payment.TotalPrice;
                        break;
                    case 11:
                        dashboard.Nov = dashboard.Nov + payment.TotalPrice;
                        break;
                    case 12:
                        dashboard.Dec = dashboard.Dec + payment.TotalPrice;
                        break;
                }
            }
            return dashboard;
        }
    }
}
