using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using webBookingSolution.Api.Customers;
using webBookingSolution.Data.EF;
using webBookingSolution.ViewModels.System;
using webBookingSolution.Api.Employees;
using webBookingSolution.Api.Halls;
using webBookingSolution.Api.Menus;
using webBookingSolution.Api.Sv;
using webBookingSolution.Api.Users;
using webBookingSolution.Api.Books;
using webBookingSolution.Api.Payments;
using webBookingSolution.Api.Dashboard;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace webBookingSolution.WebAdmin
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

            services.AddHttpClient(); 
            services.AddHttpContextAccessor();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/User/Login";
                options.AccessDeniedPath = "/Home/ErrorPage";
            });

            services.AddControllersWithViews().AddFluentValidation(fv
                 => fv.RegisterValidatorsFromAssemblyContaining<RegisterRequestValidator>());

            services.AddTransient<IUserApiClient, UserApiClient>();
            services.AddTransient<IHallApiClient, HallApiClient>();
            services.AddTransient<IMenuApiClient, MenuApiClient>();
            services.AddTransient<IEmployeeApiClient, EmployeeApiClient>();
            services.AddTransient<ICustomerApiClient, CustomerApiClient>();
            services.AddTransient<IServiceApiClient, ServiceApiClient>();
            services.AddTransient<IBookApiClient, BookApiClient>();
            services.AddTransient<IPaymentApiClient, PaymentApiClient>();
            services.AddTransient<IDashboardApiClient, DashboardApiClient>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

            IMvcBuilder builder = services.AddRazorPages();
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            #if DEBUG
                if(env == Environments.Development)
                {
                    builder.AddRazorRuntimeCompilation();
                }
            #endif
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
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Home/ErrorPage";
                    await next();
                }
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Dashboard}/{id?}");
            });
        }
    }
}
