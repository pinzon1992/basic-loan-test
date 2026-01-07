using Fundo.Applications.Infrastructure.Data;
using Fundo.Applications.Infrastructure.Options;
using Fundo.Applications.Infrastructure.Repositories.Loans;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fundo.Applications.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseOption>(configuration.GetSection(DatabaseOption.SectionName));
            var databaseOptions = configuration.GetSection(DatabaseOption.SectionName).Get<DatabaseOption>() ?? throw new InvalidDataException("Database is not configured");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(databaseOptions.ConnectionString));

            services.AddScoped<ILoanRepository, LoanRepository>();

            return services;
        }
    }
}