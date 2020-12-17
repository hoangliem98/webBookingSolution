using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using webBookingSolution.Api.Books;
using webBookingSolution.Api.Customers;
using webBookingSolution.Api.Employees;
using webBookingSolution.Api.Halls;
using webBookingSolution.Api.Menus;
using webBookingSolution.Api.Sv;
using webBookingSolution.Api.Users;
using webBookingSolution.Data.EF;
using webBookingSolution.ViewModels.Catalog.Users;
using webBookingSolution.ViewModels.System;

namespace webBookingSolution.Web
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

            //services.AddIdentity<UserViewModel, IdentityRole>(options =>
            //{
            //    options.User.RequireUniqueEmail = false;
            //}).AddEntityFrameworkStores<BookingDBContext>().AddDefaultTokenProviders();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/User/Login";
                    options.AccessDeniedPath = "/Home/ErrorPage";
                })
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = "682443755993420";
                    facebookOptions.AppSecret = "4655c0b06486bb57384dbde5c383df88";
                    facebookOptions.Scope.Add("email");
                    facebookOptions.Scope.Add("user_gender");
                    facebookOptions.Scope.Add("user_birthday");
                    facebookOptions.Scope.Add("user_photos");
                })
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = "782058971026-g0e2rp1k5scqj7qkl157klfc3gutdbg0.apps.googleusercontent.com";
                    googleOptions.ClientSecret = "TKXH4fWRtv_CcDHBexdccScH";
                });

            services.AddControllersWithViews().AddFluentValidation(fv
                 => fv.RegisterValidatorsFromAssemblyContaining<RegisterRequestValidator>());

            services.AddTransient<IHallApiClient, HallApiClient>();
            services.AddTransient<IMenuApiClient, MenuApiClient>();
            services.AddTransient<IServiceApiClient, ServiceApiClient>();
            services.AddTransient<IUserApiClient, UserApiClient>();
            services.AddTransient<IEmployeeApiClient, EmployeeApiClient>();
            services.AddTransient<ICustomerApiClient, CustomerApiClient>();
            services.AddTransient<IBookApiClient, BookApiClient>();

            IMvcBuilder builder = services.AddRazorPages();
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            #if DEBUG
                if (env == Environments.Development)
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
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
