using Domain.Entities.Logs;
using Domain.Enums;

namespace Application.Logs.Services
{
    public interface ILogService
    {
        Task LogAsync(Log log, CancellationToken cancellationToken = default);

        Task LogSuccessAsync(
            LogTypes type,
            string action,
            string message,
            string? userId = null,
            string? ip = null,
            CancellationToken cancellationToken = default);

        Task LogFailureAsync(
            LogTypes type,
            string action,
            string error,
            string? userId = null,
            string? ip = null,
            CancellationToken cancellationToken = default);
    }
}
