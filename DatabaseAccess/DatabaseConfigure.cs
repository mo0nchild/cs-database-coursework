using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess
{
    public sealed class TestOptionsMonitor<TOption> : IOptionsMonitor<TOption> where TOption : class, new()
    {
        public TOption CurrentValue { get; private set; } = default!;
        public TestOptionsMonitor(TOption currentValue) => this.CurrentValue = currentValue;

        TOption IOptionsMonitor<TOption>.Get(string? name) => this.CurrentValue;
        public IDisposable OnChange(Action<TOption, string> listener) { return default!; }
    }

    public partial class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContextFactory() : base() { }
        public interface DatabaseOptionsConfigure<TContext> where TContext : DbContext
        {
            public DbContextOptions<TContext> ConfigureOptions(System.String config_name);
        }
        public partial class DatabaseConfigure : DatabaseOptionsConfigure<DatabaseContext>
        {
            public DatabaseConfigure() : base() { }
            public DbContextOptions<DatabaseContext> ConfigureOptions(string config_name = "dbsettings.json")
            {
                var builder = new ConfigurationBuilder();
                var config = builder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(config_name).Build();

                var options_builder = new DbContextOptionsBuilder<DatabaseContext>();
                return options_builder.UseNpgsql(config.GetConnectionString("DefaultConnection")).Options;
            }
        }

        public DatabaseContext CreateDbContext(string[] args)
        {
            var logger_option = new TestOptionsMonitor<DatabaseLoggerConfiguration>(new DatabaseLoggerConfiguration());
            return new DatabaseContext(new DatabaseConfigure().ConfigureOptions(), logger_option);
        }
    }
}
