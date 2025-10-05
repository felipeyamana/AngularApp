using Domain.Factories;
using Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure.Jobs
{
    [DisallowConcurrentExecution]
    public class BackupLogJob : IJob
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BackupLogJob> _logger;

        public BackupLogJob(ApplicationDbContext context, ILogger<BackupLogJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("BackupLogJob started at {time}", DateTime.UtcNow);

            try
            {
                var log = LogFactory.CreateBackupSuccess();

                _context.Logs.Add(log);
                await _context.SaveChangesAsync();

                _logger.LogInformation("BackupLogJob completed successfully at {time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BackupLogJob failed at {time}", DateTime.UtcNow);

                _context.Logs.Add(LogFactory.CreateBackupFailure(ex.Message));
                await _context.SaveChangesAsync();
            }
        }
    }
}
