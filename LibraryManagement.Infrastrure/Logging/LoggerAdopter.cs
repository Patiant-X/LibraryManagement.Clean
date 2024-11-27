using LibraryManagement.Application.Contracts.Logging;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Infrastructure.Logging
{
    public class LoggerAdopter<T> : IAppLogger<T>
    {
        private readonly ILogger<T> _logger;
        public LoggerAdopter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<T>();
        }
        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }
    }
}
