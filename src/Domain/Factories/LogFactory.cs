using Domain.Entities;
using Domain.Entities.Logs;
using Domain.Enums;
using System.Net;

namespace Domain.Factories
{
    public static class LogFactory
    {
        public static Log CreateBackupSuccess() => new()
        {
            LogTypeId = (int)LogTypes.System,
            Action = "System Backup",
            Description = "Automated system backup completed successfully.",
            IpAddress = GetLocalIpAddress(),
            EventDate = DateTime.UtcNow,
            ActionSuccess = true
        };

        public static Log CreateBackupFailure(string errorMessage) => new()
        {
            LogTypeId = (int)LogTypes.System,
            Action = "System Backup (Failed)",
            Description = $"Backup process failed: {errorMessage}",
            IpAddress = GetLocalIpAddress(),
            EventDate = DateTime.UtcNow,
            ActionSuccess = false
        };

        public static Log CreateUserProfileUpdateSuccess(ApplicationUser user, string? ipAddress) => new()
        {
            LogTypeId = (int)LogTypes.User,
            UserId = user.Id,
            Action = "User profile updated successfully",
            Description = $"User {user.Email} updated their profile information.",
            EventDate = DateTime.UtcNow,
            IpAddress = ipAddress,
            ActionSuccess = true
        };

        public static Log CreateUserProfileUpdateFailure(ApplicationUser user, string? ipAddress, string? message) => new()
        {
            LogTypeId = (int)LogTypes.User,
            UserId = user.Id,
            Action = "User profile update failed",
            Description = $"User {user.Email} could not update their profile information. {message}",
            EventDate = DateTime.UtcNow,
            IpAddress = ipAddress,
            ActionSuccess = false
        };

        public static Log CreateSuccess(LogTypes type, string action, string message, string? userId = null, string? ip = null)
            => new Log
            {
                Action = action,
                LogTypeId = (int)type,
                Description = message,
                ActionSuccess = true,
                UserId = userId,
                IpAddress = ip,
                EventDate = DateTime.UtcNow
            };

        public static Log CreateFailure(LogTypes type, string action, string error, string? userId = null, string? ip = null)
            => new Log
            {
                Action = action,
                LogTypeId = (int)type,
                Description = error,
                ActionSuccess = false,
                UserId = userId,
                IpAddress = ip,
                EventDate = DateTime.UtcNow
            };

        /// <summary>
        /// Helper method to resolve local IPv4 address (best effort).
        /// </summary>
        private static string GetLocalIpAddress()
        {
            try
            {
                string hostName = Dns.GetHostName();
                var addresses = Dns.GetHostAddresses(hostName);

                foreach (var addr in addresses)
                {
                    if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        return addr.ToString();
                }
            }
            catch { }

            return "127.0.0.1";
        }
    }
}
