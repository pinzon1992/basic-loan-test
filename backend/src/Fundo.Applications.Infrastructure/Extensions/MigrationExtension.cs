using Fundo.Applications.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Applications.Infrastructure.Extensions
{
    public static class MigrationExtension
    {
        public static void MigrateDatabase(this WebApplication app)
        {
            // Apply pending EF Core migrations at startup
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger(typeof(MigrationExtension).FullName ?? "MigrationsExtension");
                    logger.LogError(ex, "An error occurred while migrating the database.");
                    throw;
                }
            }
        }
    }
}
