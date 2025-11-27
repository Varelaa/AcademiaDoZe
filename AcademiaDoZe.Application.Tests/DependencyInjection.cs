// Gabriel Souza Varela

using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Infrastructure.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace AcademiaDoZe.Application.Tests
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureServices(string connectionString, AcademiaDoZe.Infrastructure.Data.DatabaseType databaseType)
        {
            var services = new ServiceCollection();

            services.AddApplicationServices();
            services.AddInfrastructure(connectionString, databaseType);

            return services;
        }

        public static IServiceProvider BuildServiceProvider(IServiceCollection services)
        {
            return services.BuildServiceProvider();
        }
    }
}
