// Gabriel Souza Varela

using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Services;
using AcademiaDoZe.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AcademiaDoZe.Application.DependencyInjection
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Services
            services.AddScoped<IAlunoService, AlunoService>();
            services.AddScoped<ILogradouroService, LogradouroService>();
            services.AddScoped<IMatriculaService, MatriculaService>();
            services.AddScoped<IColaboradorService, ColaboradorService>();

            // FACTORIES (necessários para os Services)
            services.AddScoped<Func<IAlunoRepository>>(sp =>
                () => sp.GetRequiredService<IAlunoRepository>());

            services.AddScoped<Func<ILogradouroRepository>>(sp =>
                () => sp.GetRequiredService<ILogradouroRepository>());

            services.AddScoped<Func<IMatriculaRepository>>(sp =>
                () => sp.GetRequiredService<IMatriculaRepository>());

            services.AddScoped<Func<IColaboradorRepository>>(sp =>
                () => sp.GetRequiredService<IColaboradorRepository>());

            return services;
        }
    }
}
