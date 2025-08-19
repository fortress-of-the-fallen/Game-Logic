using Game.Application.Interface;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Game.Application.Infra.Logging
{
    public class Logger<T> : ILogger<T>
    {
        private readonly Serilog.ILogger _logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(theme: AnsiConsoleTheme.Code)
            .WriteTo.File(
                path: "logs/log-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss} [{SourceContext}] {Message:lj}{NewLine}{Exception}"
            )
            .CreateLogger()
            .ForContext<T>();

        public void LogInformation(string message)
        {
            _logger.Information(message);
        }

        public void LogWarning(string message)
        {
            _logger.Warning(message);
        }

        public void LogError(string message)
        {
            _logger.Error(message);
        }

        public void LogError(string message, params object[] args)
        {
            _logger.Error(message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.Information(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.Warning(message, args);
        }
    }
}