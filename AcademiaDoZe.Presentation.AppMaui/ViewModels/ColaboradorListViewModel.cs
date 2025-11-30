// Gabriel Souza Varela

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    public partial class ColaboradorListViewModel : BaseViewModel
    {
        private readonly IColaboradorService _colaboradorService;

        [ObservableProperty]
        private ObservableCollection<ColaboradorDTO> colaboradores = new();

        public ColaboradorListViewModel(IColaboradorService colaboradorService)
        {
            _colaboradorService = colaboradorService;
            Title = "Colaboradores";
        }

        public async Task InitializeAsync()
        {
            await LoadAsync();
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                Colaboradores.Clear();

                var itens = await _colaboradorService.ObterTodosAsync();

                foreach (var item in itens)
                    Colaboradores.Add(item);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar colaboradores: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task NovoAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.ColaboradorPage));
        }

        [RelayCommand]
        private async Task EditarAsync(ColaboradorDTO? colaborador)
        {
            if (colaborador is null)
                return;

            var parametros = new Dictionary<string, object>
            {
                { "Id", colaborador.Id }
            };

            await Shell.Current.GoToAsync(nameof(Views.ColaboradorPage), parametros);
        }
    }
}
