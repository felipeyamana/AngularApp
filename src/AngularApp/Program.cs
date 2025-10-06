using Application;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Seeds;
using Microsoft.EntityFrameworkCore;

namespace AngularApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddCorsPolicies();
            builder.Services.AddHttpContextAccessor();

            //if (!builder.Environment.IsDevelopment())
            //{
            //    builder.Services
            //        .AddOpenTelemetry()
            //        .UseAzureMonitor();
            //}

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseCors("AllowAngularProd");

                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    await db.Database.MigrateAsync();
                    await db.SeedLogTypesAsync();
                }
            }
            else
            {
                app.UseCors("AllowAngularDev");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllers();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapFallbackToFile("index.html");

            app.MapRazorPages();

            app.Run();
        }
    }
}
