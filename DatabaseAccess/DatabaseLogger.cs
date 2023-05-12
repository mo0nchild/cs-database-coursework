using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess
{
    public sealed class DatabaseLoggerConfiguration : System.Object
    {
        public DatabaseLoggerConfiguration() : base() { }

        public System.String FilePath { get; set; } = "database.log";
        public Dictionary<LogLevel, System.ConsoleColor> LogLevelColorMap { get; set; } = new()
        {
            [LogLevel.Information] = ConsoleColor.Green, [LogLevel.Warning] = ConsoleColor.Yellow,
            [LogLevel.Error] = ConsoleColor.DarkRed
        };
    }
    internal partial class DatabaseLoggerProvider : System.Object, ILoggerProvider
    {
        protected readonly System.IDisposable? _onChangeToken;
        protected virtual DatabaseLoggerConfiguration Configuration { get; private set; } = new();
        public DatabaseLoggerProvider(IOptionsMonitor<DatabaseLoggerConfiguration> config) : base() 
        {
            this.Configuration = config.CurrentValue;
            this._onChangeToken = config.OnChange((updated_config) => this.Configuration = updated_config);
        }
        public virtual ILogger CreateLogger(string category_name) => new DatabaseLoggerProvider.DatabaseLogger(
            category_name, this.GetLoggerConfiguration);

        protected DatabaseLoggerConfiguration GetLoggerConfiguration() => this.Configuration;

        protected class DatabaseLogger : ILogger, System.IDisposable
        {
            protected System.String CategoryName { get; private set; } = default!;
            protected Func<DatabaseLoggerConfiguration> CurrentConfig { get; private set; } = default!;

            public DatabaseLogger(string category_name, Func<DatabaseLoggerConfiguration> config) : base() 
                => (this.CategoryName, this.CurrentConfig) = (category_name, config);

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => this;
            public bool IsEnabled(LogLevel logLevel) => this.CurrentConfig().LogLevelColorMap.ContainsKey(logLevel);
            public void Dispose() { }

            public /*async*/ void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, 
                Func<TState, Exception?, string> formatter)
            {
                var timestamp_value = string.Format("\n[DateTime]: [{0}]", DateTime.UtcNow);
                //Console.WriteLine(timestamp_value);

                //var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.CurrentConfig().FilePath);
                //using (var file_writer = new StreamWriter(new FileStream(filepath, FileMode.Append)))
                //{
                //    await file_writer.WriteLineAsync(timestamp_value);
                //    await file_writer.WriteLineAsync($"{formatter(state, exception)}\n");
                //}
                //Console.ForegroundColor = this.CurrentConfig().LogLevelColorMap[logLevel];

                //Console.WriteLine(formatter(state, exception));
                //Console.ForegroundColor = ConsoleColor.White; Console.WriteLine();
            }
        }
        public virtual void Dispose() { this._onChangeToken?.Dispose(); }
    }
}
