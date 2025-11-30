using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Services;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.DependencyInjection;
using AcademiaDoZe.Presentation.AppMaui.Configuration;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using AcademiaDoZe.Presentation.AppMaui.Views;
using Microsoft.Extensions.Logging;

namespace AcademiaDoZe.Presentation.AppMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                });

            // ===============================
            // CONFIGURAÇÃO DE SERVICES BÁSICOS
            // ===============================
            ConfigurationHelper.ConfigureServices(builder.Services);

            // -------------------------------
            // CONFIGURAÇÃO DA INFRAESTRUTURA
            // -------------------------------

            var connectionString =
                "Server=192.168.100.24;Port=3306;Database=db_academia_do_ze;Uid=root;Pwd=root;";
            var databaseType = DatabaseType.MySql;

            // Repositórios via extensão AddInfrastructure (Aluno, Logradouro, Matricula, etc.)
            builder.Services.AddInfrastructure(connectionString, databaseType);

            // Somente o service de LOGRADOURO por enquanto
            builder.Services.AddScoped<ILogradouroService, LogradouroService>();

            // ===============================
            // VIEWMODELS
            // ===============================
            builder.Services.AddTransient<DashboardListViewModel>();
            builder.Services.AddTransient<LogradouroListViewModel>();
            builder.Services.AddTransient<LogradouroViewModel>();

            // ===============================
            // VIEWS
            // ===============================
            builder.Services.AddTransient<DashboardListPage>();
            builder.Services.AddTransient<LogradouroListPage>();
            builder.Services.AddTransient<LogradouroPage>();
            builder.Services.AddTransient<ConfigPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
