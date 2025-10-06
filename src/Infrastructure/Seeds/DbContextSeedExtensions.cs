using Domain.Entities.Logs;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seeds
{
    public static class DbContextSeedExtensions
    {
        public static async Task SeedLogTypesAsync(this ApplicationDbContext context)
        {
            var logTypes = new[]
            {
                new LogType
                {
                    Name = "User Logs",
                    Description = "Logs related to user actions and interactions.",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new LogType
                {
                    Name = "System Logs",
                    Description = "Logs related to system operations and background tasks.",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new LogType
                {
                    Name = "Security Logs",
                    Description = "Logs related to authentication, authorization, and access control.",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new LogType
                {
                    Name = "Integration Logs",
                    Description = "Logs related to external integrations and API interactions.",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new LogType
                {
                    Name = "Performance Logs",
                    Description = "Logs related to internal application performance.",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            foreach (var logType in logTypes)
            {
                if (!await context.LogTypesSet.AnyAsync(x => x.Name == logType.Name))
                {
                    context.LogTypesSet.Add(logType);
                }
            }

            await context.SaveChangesAsync();
        }

    }

}
