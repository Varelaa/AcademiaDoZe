// Gabriel Souza Varela

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    public partial class LogradouroListViewModel : BaseViewModel
    {
        private readonly ILogradouroService _logradouroService;

        public ObservableCollection<LogradouroDTO> Logradouros { get; } = new();

        // Filtros
        public List<string> FilterTypes { get; } = new() { "CEP", "Cidade" };

        private string _selectedFilterType = "CEP";
        public string SelectedFilterType
        {
            get => _selectedFilterType;
            set => SetProperty(ref _selectedFilterType, value);
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public LogradouroListViewModel(ILogradouroService logradouroService)
        {
            _logradouroService = logradouroService;
            Title = "Logradouros";
        }

        // =========================================================
        // MÉTODO QUE A TELA CHAMA NO OnAppearing / INICIALIZAÇÃO
        // =========================================================
        public async Task InitializeAsync()
        {
            await LoadLogradourosAsync();
        }

        // =========================================================
        // CARREGAR TODOS
        // Gera LoadLogradourosCommand (usado no code-behind)
        // =========================================================
        [RelayCommand]
        public async Task LoadLogradourosAsync()
        {
            try
            {
                IsBusy = true;
                Logradouros.Clear();

                var lista = await _logradouroService.ObterTodosAsync();

                foreach (var item in lista)
                    Logradouros.Add(item);
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        // =========================================================
        // ATUALIZAR (PULL TO REFRESH)
        // =========================================================
        [RelayCommand]
        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            await LoadLogradourosAsync();
        }

        // =========================================================
        // BUSCAR
        // =========================================================
        [RelayCommand]
        private async Task SearchLogradourosAsync()
        {
            try
            {
                IsBusy = true;
                Logradouros.Clear();

                IEnumerable<LogradouroDTO> lista = Enumerable.Empty<LogradouroDTO>();

                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    lista = await _logradouroService.ObterTodosAsync();
                }
                else
                {
                    switch (SelectedFilterType)
                    {
                        case "CEP":
                            var item = await _logradouroService.ObterPorCepAsync(SearchText);
                            if (item != null)
                                lista = new List<LogradouroDTO> { item };
                            break;

                        case "Cidade":
                            lista = await _logradouroService.ObterPorCidadeAsync(SearchText);
                            break;
                    }
                }

                foreach (var l in lista)
                    Logradouros.Add(l);
            }
            finally
            {
                IsBusy = false;
            }
        }

        // =========================================================
        // ADICIONAR
        // Gera AddLogradouroCommand (usado no XAML)
        // =========================================================
        [RelayCommand]
        private async Task AddLogradouroAsync()
        {
            await Shell.Current.GoToAsync("logradouro");
        }

        // =========================================================
        // EDITAR
        // Gera EditLogradouroCommand (usado no code-behind)
        // =========================================================
        [RelayCommand]
        private async Task EditLogradouroAsync(LogradouroDTO logradouro)
        {
            if (logradouro == null)
                return;

            await Shell.Current.GoToAsync($"logradouro?Id={logradouro.Id}");
        }

        // =========================================================
        // EXCLUIR
        // Gera DeleteLogradouroCommand (usado no code-behind)
        // =========================================================
        [RelayCommand]
        private async Task DeleteLogradouroAsync(LogradouroDTO logradouro)
        {
            if (logradouro == null)
                return;

            bool confirmar = await Shell.Current.DisplayAlert(
                "Confirmação",
                $"Deseja realmente excluir o logradouro {logradouro.Cep}?",
                "Sim",
                "Não");

            if (!confirmar)
                return;

            await _logradouroService.RemoverAsync(logradouro.Id);
            Logradouros.Remove(logradouro);
        }
    }
}
