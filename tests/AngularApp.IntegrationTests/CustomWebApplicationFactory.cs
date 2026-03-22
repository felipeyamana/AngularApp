using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.Extensions.DependencyInjection;

namespace AngularApp.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // remove signalr injection from the default service collection to avoid sending notifications during tests
            var existingPublisher = services.SingleOrDefault(d => d.ServiceType == typeof(INotificationPublisher));
            if (existingPublisher != null)
            {
                services.Remove(existingPublisher);
            }

            services.AddSingleton<INotificationPublisher, NoOpNotificationPublisher>();
        });
    }
}

// no notifications will be sent to anyone as the default behaviour of PublishAsync has been overriden with the code below
internal sealed class NoOpNotificationPublisher : INotificationPublisher
{
    public Task PublishAsync(NotificationEnvelope message) => Task.CompletedTask;
}

