using Domain.Entities.Logs;
using Domain.Enums;
using Domain.Factories;
using Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Application.Logs.Services
{
    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LogService> _logger;

        public LogService(ApplicationDbContext context, ILogger<LogService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogAsync(Log log, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Logs.Add(log);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist log entry: {Message}", ex.Message);
            }
        }

        public async Task LogSuccessAsync(
            LogTypes type,
            string action,
            string message,
            string? userId = null,
            string? ip = null,
            CancellationToken cancellationToken = default)
        {
            var log = LogFactory.CreateSuccess(type, action, message, userId, ip);
            await LogAsync(log, cancellationToken);
        }

        public async Task LogFailureAsync(
            LogTypes type,
            string action,
            string error,
            string? userId = null,
            string? ip = null,
            CancellationToken cancellationToken = default)
        {
            var log = LogFactory.CreateFailure(type, action, error, userId, ip);
            await LogAsync(log, cancellationToken);
        }
    }
}
