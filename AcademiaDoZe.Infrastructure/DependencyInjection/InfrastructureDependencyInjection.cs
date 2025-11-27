// Gabriel Souza Varela

using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AcademiaDoZe.Infrastructure.DependencyInjection
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            string connectionString,
            DatabaseType databaseType)
        {
            // Repositories
            services.AddScoped<IAlunoRepository>(_ =>
                new AlunoRepository(connectionString, databaseType));

            services.AddScoped<ILogradouroRepository>(_ =>
                new LogradouroRepository(connectionString, databaseType));

            services.AddScoped<IMatriculaRepository>(_ =>
                new MatriculaRepository(connectionString, databaseType));

            services.AddScoped<IColaboradorRepository>(_ =>
                new ColaboradorRepository(connectionString, databaseType));

            return services;
        }
    }
}
