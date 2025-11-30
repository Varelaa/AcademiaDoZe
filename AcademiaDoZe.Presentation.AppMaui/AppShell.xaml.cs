using AcademiaDoZe.Presentation.AppMaui.Views;

namespace AcademiaDoZe.Presentation.AppMaui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(ColaboradorPage), typeof(ColaboradorPage));
        Routing.RegisterRoute(nameof(ColaboradorListPage), typeof(ColaboradorListPage));
        Routing.RegisterRoute(nameof(LogradouroPage), typeof(LogradouroPage));
        Routing.RegisterRoute(nameof(LogradouroListPage), typeof(LogradouroListPage));
    }
}
