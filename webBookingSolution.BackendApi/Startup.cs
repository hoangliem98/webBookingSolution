using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using webBookingSolution.Application.Catalog.Books;
using webBookingSolution.Application.Catalog.Customers;
using webBookingSolution.Application.Catalog.Dashboard;
using webBookingSolution.Application.Catalog.Employees;
using webBookingSolution.Application.Catalog.Halls;
using webBookingSolution.Application.Catalog.Menus;
using webBookingSolution.Application.Catalog.Payments;
using webBookingSolution.Application.Catalog.Services;
using webBookingSolution.Application.Common;
using webBookingSolution.Application.System.Users;
using webBookingSolution.Data.EF;
using webBookingSolution.Data.Entities;
using webBookingSolution.ViewModels.System;

namespace webBookingSolution.BackendApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BookingDBContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("webBookingSolutionDb")));

            //Declare DI
            services.AddTransient<IStorageService, FileStorageService>();
            services.AddTransient<IHallService, HallService>();
            services.AddTransient<IMenuService, MenuService>();
            services.AddTransient<ISvService, SvService>();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IBookService, BookService>();
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<IDashboardService, DashboardService>();
            services.AddTransient<IConvertToUnSign, ConvertToUnSign>();

            //services.AddTransient<IValidator<RegisterRequest>, RegisterRequestValidator>();

            services.AddControllers().AddFluentValidation(fv 
                => fv.RegisterValidatorsFromAssemblyContaining<RegisterRequestValidator>());

            services.AddControllersWithViews();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger", Version = "v1.0" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger Booking v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
