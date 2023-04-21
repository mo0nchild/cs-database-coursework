using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleToAttribute("ClientApplication")]
namespace DatabaseAccess
{
    public sealed class InitialOptionsMonitor<TOption> : IOptionsMonitor<TOption> where TOption : class, new()
    {
        public TOption CurrentValue { get; private set; } = default!;
        public InitialOptionsMonitor(TOption currentValue) => this.CurrentValue = currentValue;

        TOption IOptionsMonitor<TOption>.Get(string? name) => this.CurrentValue;
        public IDisposable OnChange(Action<TOption, string> listener) { return default!; }
    }

    internal partial class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContextFactory() : base() { }
        public interface DatabaseOptionsConfigure<TContext> where TContext : DbContext
        {
            public abstract DbContextOptionsBuilder<TContext> ContextOptions { get; set; }
            public abstract DbContextOptions<TContext> ConfigureOptions(System.String config_name);
        }
        public partial class DatabaseConfigure : DatabaseOptionsConfigure<DatabaseContext>
        {
            public DbContextOptionsBuilder<DatabaseContext> ContextOptions { get; set; } = default!;
            public DatabaseConfigure(DbContextOptionsBuilder<DatabaseContext> options) : base() => this.ContextOptions = options;

            public DbContextOptions<DatabaseContext> ConfigureOptions(string config_name = "dbsettings.json")
            {
                var builder = new ConfigurationBuilder();
                var config = builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile(config_name).Build();

                return this.ContextOptions.UseNpgsql(config.GetConnectionString("DefaultConnection")).Options;
            }
        }
        public virtual DatabaseContext CreateDbContext(string[] args)
        {
            var logger_option = new InitialOptionsMonitor<DatabaseLoggerConfiguration>(new DatabaseLoggerConfiguration());
            var options_builder = new DbContextOptionsBuilder<DatabaseAccess.DatabaseContext>();

            var database_options = (new DatabaseConfigure(options_builder)).ConfigureOptions();
            return new DatabaseContext(database_options, logger_option);
        }
    }
}
