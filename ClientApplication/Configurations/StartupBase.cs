using DatabaseAccess;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace ClientApplication.Configurations
{
    public sealed class StartupException : System.Exception
    {
        public Type StartupType { get; private set; } = default!;
        public StartupException(string message, Type @class) : base(message) { StartupType = @class; }
    }

    public abstract class StartupBase : System.Object
    {
        protected virtual IConfiguration Configuration { get; private set; } = default!;
        public StartupBase(IConfiguration configuration) : base() { Configuration = configuration; }

        public static Task StartApplication<TStartup>(string[] args) where TStartup : StartupBase
        {
            var builder = WebApplication.CreateBuilder(args);
            var startup = Activator.CreateInstance(typeof(TStartup), new[] { builder.Configuration }) as TStartup;

            if (startup == null) throw new StartupException("Невозможно создать объект загрузчика", typeof(TStartup));
            startup.ConfigureServices(builder.Services);

            WebApplication web_application = builder.Build();
            startup.ConfigureApplication(web_application, builder.Environment);

            return web_application.RunAsync();
        }
        protected abstract void ConfigureServices(IServiceCollection services);
        protected abstract void ConfigureApplication(WebApplication application, IWebHostEnvironment env);
    }
}
