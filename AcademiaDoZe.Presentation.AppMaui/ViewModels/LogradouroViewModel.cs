//Gabriel Souza Varela

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    public partial class LogradouroViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string title = "Cadastro de Logradouro";

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private LogradouroDTO logradouro = new LogradouroDTO();

        public LogradouroViewModel()
        {
            // opcional: inicializar algo
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task SearchByCepAsync()
        {
            // Implementar busca de CEP depois
            await Task.Delay(1000);
        }

        [RelayCommand]
        private async Task SaveLogradouroAsync()
        {
            // Implementar salvamento depois
            await Task.Delay(1000);
        }
    }

    // modelo simples temporário
    public class LogradouroDTO
    {
        public int Id { get; set; }
        public string Cep { get; set; }
        public string Nome { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
    }
}
