using System.Runtime.CompilerServices;

namespace ClientApplication
{
    public sealed class StartupException : System.Exception
    {
        public System.Type StartupType { get; private set; } = default!;
        public StartupException(string message, Type @class) : base(message) { this.StartupType = @class; }
    }

    public abstract class StartupBase : System.Object
    {
        protected virtual IConfiguration Configuration { get; private set; } = default!;
        public StartupBase(IConfiguration config) : base() { this.Configuration = config; }

        public static WebApplication StartApplication<TStartup>(string[] args) where TStartup : StartupBase
        {
            var builder = WebApplication.CreateBuilder(args);
            var startup = Activator.CreateInstance(typeof(TStartup), new[] { builder.Configuration }) as TStartup;

            if (startup == null) throw new StartupException("Невозможно создать объект загрузчика", typeof(TStartup));
            startup.ConfigureServices(builder.Services);

            WebApplication web_application = builder.Build();
            startup.ConfigureApplication(web_application, builder.Environment); 

            return web_application;
        }

        protected abstract void ConfigureServices(IServiceCollection services);
        protected abstract void ConfigureApplication(WebApplication app, IWebHostEnvironment env);
    }

    public partial class StartupApplication : ClientApplication.StartupBase
    {
        public StartupApplication(IConfiguration config) : base(config) { }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<DatabaseAccess.DatabaseContext>();
        }

        protected override void ConfigureApplication(WebApplication app, IWebHostEnvironment env)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
