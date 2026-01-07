using Fundo.Applications.Application.Loans;
using Microsoft.Extensions.DependencyInjection;

namespace Fundo.Applications.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ILoanService, LoanService>();
            return services;
        }
    }
}
