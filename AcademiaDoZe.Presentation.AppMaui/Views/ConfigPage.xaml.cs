using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Presentation.AppMaui.Message;
using CommunityToolkit.Mvvm.Messaging;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class ConfigPage : ContentPage
{
    public ConfigPage()
    {
        InitializeComponent();
        CarregarTema();
        CarregarBanco();
    }

    // =========================
    //        TEMA
    // =========================
    private void CarregarTema()
    {
        // Carrega o tema salvo no Preferences
        TemaPicker.SelectedIndex = Preferences.Get("Tema", "system") switch
        {
            "light" => 0,
            "dark" => 1,
            _ => 2,
        };
    }

    private async void OnSalvarTemaClicked(object sender, EventArgs e)
    {
        string selectedTheme = TemaPicker.SelectedIndex switch
        {
            0 => "light",
            1 => "dark",
            _ => "system"
        };

        Preferences.Set("Tema", selectedTheme);

        // Dispara mensagem para recarregar dinamicamente
        WeakReferenceMessenger.Default.Send(new TemaPreferencesUpdatedMessage("TemaAlterado"));

        await DisplayAlert("Sucesso", "Tema salvo com sucesso!", "OK");

        await Shell.Current.GoToAsync("//dashboard");
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//dashboard");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Evita memory leaks
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    // =========================
    //    BANCO DE DADOS
    // =========================
    private void CarregarBanco()
    {
        foreach (var tipo in Enum.GetValues<EAppDatabaseType>())
        {
            DatabaseTypePicker.Items.Add(tipo.ToString());
        }

        ServidorEntry.Text = Preferences.Get("Servidor", "172.24.32.1");
        BancoEntry.Text = Preferences.Get("Banco", "db_academia_do_ze");
        UsuarioEntry.Text = Preferences.Get("Usuario", "sa");
        SenhaEntry.Text = Preferences.Get("Senha", "abcBolinhas12345");
        ComplementoEntry.Text = Preferences.Get("Complemento", "TrustServerCertificate=True;Encrypt=True;");
        DatabaseTypePicker.SelectedItem = Preferences.Get("DatabaseType", EAppDatabaseType.SqlServer.ToString());
    }

    private async void OnSalvarBdClicked(object sender, EventArgs e)
    {
        Preferences.Set("Servidor", ServidorEntry.Text);
        Preferences.Set("Banco", BancoEntry.Text);
        Preferences.Set("Usuario", UsuarioEntry.Text);
        Preferences.Set("Senha", SenhaEntry.Text);
        Preferences.Set("Complemento", ComplementoEntry.Text);
        Preferences.Set("DatabaseType", DatabaseTypePicker.SelectedItem?.ToString());

        WeakReferenceMessenger.Default.Send(new BancoPreferencesUpdatedMessage("BancoAlterado"));

        await DisplayAlert("Sucesso", "Configurações do banco salvas!", "OK");

        await Shell.Current.GoToAsync("//dashboard");
    }
}
