using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    public partial class LogradouroListViewModel : BaseViewModel
    {
        public ObservableCollection<string> FilterTypes { get; } =
            new() { "Cidade", "Id", "Cep" };

        private readonly ILogradouroService _logradouroService;

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private string _selectedFilterType = "Cidade";
        public string SelectedFilterType
        {
            get => _selectedFilterType;
            set => SetProperty(ref _selectedFilterType, value);
        }

        private ObservableCollection<LogradouroDTO> _logradouros = new();
        public ObservableCollection<LogradouroDTO> Logradouros
        {
            get => _logradouros;
            set => SetProperty(ref _logradouros, value);
        }

        private LogradouroDTO? _selectedLogradouro;
        public LogradouroDTO? SelectedLogradouro
        {
            get => _selectedLogradouro;
            set => SetProperty(ref _selectedLogradouro, value);
        }

        public LogradouroListViewModel(ILogradouroService logradouroService)
        {
            _logradouroService = logradouroService;
            Title = "Logradouros";
        }

        // ---------------------------------------------------------
        //  COMMANDS
        // ---------------------------------------------------------

        [RelayCommand]
        private async Task AddLogradouroAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("logradouro");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro",
                    $"Erro ao navegar para tela de cadastro: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task EditLogradouroAsync(LogradouroDTO logradouro)
        {
            try
            {
                if (logradouro == null)
                    return;

                await Shell.Current.GoToAsync($"logradouro?Id={logradouro.Id}");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro",
                    $"Erro ao navegar para tela de edição: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            await LoadLogradourosAsync();
        }

        [RelayCommand]
        private async Task SearchLogradourosAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Logradouros.Clear();
                });

                IEnumerable<LogradouroDTO> resultados = Enumerable.Empty<LogradouroDTO>();

                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    resultados = await _logradouroService.ObterTodosAsync()
                        ?? Enumerable.Empty<LogradouroDTO>();
                }
                else if (SelectedFilterType == "Cidade")
                {
                    resultados = await _logradouroService.ObterPorCidadeAsync(SearchText)
                        ?? Enumerable.Empty<LogradouroDTO>();
                }
                else if (SelectedFilterType == "Id" && int.TryParse(SearchText, out int id))
                {
                    var logradouro = await _logradouroService.ObterPorIdAsync(id);

                    if (logradouro != null)
                        resultados = new[] { logradouro };
                }
                else if (SelectedFilterType == "Cep")
                {
                    var logradouro = await _logradouroService.ObterPorCepAsync(SearchText);

                    if (logradouro != null)
                        resultados = new[] { logradouro };
                }

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    foreach (var item in resultados)
                        Logradouros.Add(item);

                    OnPropertyChanged(nameof(Logradouros));
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro",
                    $"Erro ao buscar logradouros: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task LoadLogradourosAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Logradouros.Clear();
                    OnPropertyChanged(nameof(Logradouros));
                });

                var logradourosList = await _logradouroService.ObterTodosAsync();

                if (logradourosList != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        foreach (var logradouro in logradourosList)
                            Logradouros.Add(logradouro);

                        OnPropertyChanged(nameof(Logradouros));
                    });
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro",
                    $"Erro ao carregar logradouros: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task DeleteLogradouroAsync(LogradouroDTO logradouro)
        {
            if (logradouro == null)
                return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Confirmar Exclusão",
                $"Deseja realmente excluir o logradouro {logradouro.Nome}?",
                "Sim", "Não");

            if (!confirm)
                return;

            try
            {
                IsBusy = true;

                bool success = await _logradouroService.RemoverAsync(logradouro.Id);

                if (success)
                {
                    Logradouros.Remove(logradouro);
                    await Shell.Current.DisplayAlert("Sucesso",
                        "Logradouro excluído com sucesso!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro",
                        "Não foi possível excluir o logradouro.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro",
                    $"Erro ao excluir logradouro: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
