using Domain.Entities;
using Domain.Entities.Logs;
using System.Net;

namespace Domain.Factories
{
    public static class LogFactory
    {
        /// <summary>
        /// Creates a system log representing a successful backup operation.
        /// </summary>
        public static Log CreateBackupSuccess()
        {
            return new Log
            {
                LogTypeId = 2, // System Logs
                Action = "System Backup",
                Description = "Automated system backup completed successfully.",
                IpAddress = GetLocalIpAddress(),
                EventDate = DateTime.UtcNow,
                ActionSuccess = true
            };
        }

        /// <summary>
        /// Creates a system log representing a failed backup operation.
        /// </summary>
        public static Log CreateBackupFailure(string errorMessage)
        {
            return new Log
            {
                LogTypeId = 2, // System Logs
                Action = "System Backup (Failed)",
                Description = $"Backup process failed: {errorMessage}",
                IpAddress = GetLocalIpAddress(),
                EventDate = DateTime.UtcNow,
                ActionSuccess = false
            };
        }

        /// <summary>
        /// Creates a log representing a successful user profile update.
        /// </summary>
        public static Log CreateUserProfileUpdateSuccess(ApplicationUser user, string? ipAddress)
        {
            return new Log
            {
                LogTypeId = 1,
                UserId = user.Id,
                Action = "User profile updated successfully",
                Description = $"User {user.Email} updated their profile information.",
                EventDate = DateTime.UtcNow,
                IpAddress = ipAddress,
                ActionSuccess = true
            };
        }

        /// <summary>
        /// Creates a log representing a failed user profile update.
        /// </summary>
        public static Log CreateUserProfileUpdateFailure(ApplicationUser user, string? ipAddress, string? message)
        {
            return new Log
            {
                LogTypeId = 1,
                UserId = user.Id,
                Action = "User profile update failed",
                Description = $"User {user.Email} could not update their profile information. {message}",
                EventDate = DateTime.UtcNow,
                IpAddress = ipAddress,
                ActionSuccess = false
            };
        }

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
