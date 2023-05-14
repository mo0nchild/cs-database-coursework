using DatabaseAccess;
using ClientApplication.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

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
            services.AddRazorPages(); services.AddServerSideBlazor(); services.AddHttpClient();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie((options) => 
            {
                options.AccessDeniedPath = new PathString("/authorization"); 
                options.LoginPath = new PathString("/authorization"); 
            });
            services.Configure<DatabaseLoggerConfiguration>(Configuration.GetSection("DatabaseLoggerConfiguration"));
            services.AddAuthorization((AuthorizationOptions options) =>
            {
                options.AddPolicy("Administrator", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
                options.AddPolicy("DefaultUser", policy => policy.RequireClaim(ClaimTypes.Role, "User"));
            });
            services.AddControllersWithViews((MvcOptions options) =>
            {
                options.ModelBinderProviders.Insert(0, new ContractModelBinder.ContractModelBinderProvider());
            });
            services.AddDbContextFactory<DatabaseContext>((DbContextOptionsBuilder options) =>
            {
                if (options is DbContextOptionsBuilder<DatabaseContext> converted_options)
                { new DatabaseContextFactory.DatabaseConfigure(converted_options!).ConfigureOptions(); }
            });
            services.AddSignalR(options => options.MaximumReceiveMessageSize = 102400000L);
            services.AddTransient<Services.IDatabaseContact, Services.DatabaseContact>();
        }
        protected override void ConfigureApplication(WebApplication application, IWebHostEnvironment env)
        {
            application.Map("/", (HttpContext context) => Results.RedirectToRoute("profile"));
            application.UseStaticFiles();
            application.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Icons")),
                RequestPath = new PathString("/Icons")
            });
            application.UseRouting().UseAuthentication().UseAuthorization();

            application.UseMiddleware<Middleware.CheckProfileMiddleware>();
            application.UseEndpoints((IEndpointRouteBuilder route_builder) => 
            {
                route_builder.MapControllers();
                route_builder.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
            application.MapBlazorHub();
        }
    }
}
