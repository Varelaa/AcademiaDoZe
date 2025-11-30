using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using AcademiaDoZe.Presentation.AppMaui.Views;


namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    public partial class ColaboradorListViewModel : BaseViewModel
    {
        private readonly IColaboradorService _service;

        public ObservableCollection<ColaboradorDTO> Colaboradores { get; } = new();

        public ColaboradorListViewModel(IColaboradorService service)
        {
            _service = service;
            Title = "Colaboradores";
        }

        [RelayCommand]
        public async Task InitializeAsync()
        {
            await LoadAsync();
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                Colaboradores.Clear();
                var lista = await _service.ObterTodosAsync();

                foreach (var c in lista)
                    Colaboradores.Add(c);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task EditarAsync(ColaboradorDTO colaborador)
        {
            if (colaborador is null) return;

            await Shell.Current.GoToAsync($"{nameof(ColaboradorPage)}?Id={colaborador.Id}");
        }

        [RelayCommand]
        public async Task ExcluirAsync(ColaboradorDTO colaborador)
        {
            if (colaborador is null) return;

            bool confirmar = await Shell.Current.DisplayAlert(
                "Excluir",
                $"Deseja excluir {colaborador.Nome}?",
                "Sim", "Não");

            if (!confirmar)
                return;

            await _service.RemoverAsync(colaborador.Id);
            await LoadAsync();
        }
    }
}
