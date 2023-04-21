using DatabaseAccess;
using ClientApplication.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClientApplication.Configurations
{
    public partial class StartupApplication : Configurations.StartupBase
    {
        public StartupApplication(ConfigurationManager configuration) : base(configuration)
        {
            configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("dbsettings.json");
        }
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                (options) => { options.LoginPath = "/authorization"; options.AccessDeniedPath = "/authorization"; }
            );
            services.Configure<DatabaseLoggerConfiguration>(Configuration.GetSection("DatabaseLoggerConfiguration"));
            services.AddAuthorization((options) =>
            {
                options.AddPolicy("Administrator", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
                options.AddPolicy("DefaultUser", policy => policy.RequireClaim(ClaimTypes.Role, "User"));
            });
            services.AddControllersWithViews((MvcOptions options) =>
            {
                options.ModelBinderProviders.Insert(0, new RegistrationModelBinder.RegistrationModelBinderProvider());
            });
            services.AddDbContextFactory<DatabaseContext>((options) =>
            {
                if (options is DbContextOptionsBuilder<DatabaseContext> converted_options)
                { new DatabaseContextFactory.DatabaseConfigure(converted_options!).ConfigureOptions(); }
            });
        }
        protected override void ConfigureApplication(WebApplication application, IWebHostEnvironment env)
        {
            application.UseRouting().UseAuthentication().UseAuthorization();
            application.Map("/", (HttpContext context) => Results.RedirectToRoute("profile"));

            application.UseEndpoints((IEndpointRouteBuilder route_builder) => 
            {
                route_builder.MapControllers();
                route_builder.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
