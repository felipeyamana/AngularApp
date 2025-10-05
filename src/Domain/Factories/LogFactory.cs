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
                EventDate = DateTime.UtcNow
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
                EventDate = DateTime.UtcNow
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
