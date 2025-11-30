// Gabriel Souza Varela

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Enums;
using CommunityToolkit.Mvvm.Input;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    [QueryProperty(nameof(ColaboradorId), "Id")]
    public partial class ColaboradorViewModel : BaseViewModel
    {
        private readonly IColaboradorService _colaboradorService;

        private ColaboradorDTO _colaborador = new()
        {
            Nome = string.Empty,
            Cpf = string.Empty,
            Telefone = string.Empty,
            Email = string.Empty,
            Numero = string.Empty,
            Complemento = string.Empty,
            DataNascimento = DateOnly.FromDateTime(DateTime.Today),
            DataAdmissao = DateOnly.FromDateTime(DateTime.Today),
            Endereco = new LogradouroDTO()
        };


        public ColaboradorDTO Colaborador
        {
            get => _colaborador;
            set => SetProperty(ref _colaborador, value);
        }

        // Propriedades auxiliares para DatePicker (DateOnly -> DateTime)
        public DateTime DataNascimentoDate
        {
            get => Colaborador.DataNascimento.ToDateTime(TimeOnly.MinValue);
            set
            {
                if (Colaborador.DataNascimento != DateOnly.FromDateTime(value))
                {
                    Colaborador.DataNascimento = DateOnly.FromDateTime(value);
                    OnPropertyChanged();
                }
            }
        }

        public DateTime DataAdmissaoDate
        {
            get => Colaborador.DataAdmissao.ToDateTime(TimeOnly.MinValue);
            set
            {
                if (Colaborador.DataAdmissao != DateOnly.FromDateTime(value))
                {
                    Colaborador.DataAdmissao = DateOnly.FromDateTime(value);
                    OnPropertyChanged();
                }
            }
        }

        private int _colaboradorId;
        public int ColaboradorId
        {
            get => _colaboradorId;
            set
            {
                if (SetProperty(ref _colaboradorId, value))
                {
                    Task.Run(InitializeAsync);
                }
            }
        }

        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public ColaboradorViewModel(IColaboradorService colaboradorService)
        {
            _colaboradorService = colaboradorService;
            Title = "Novo Colaborador";
        }

        public async Task InitializeAsync()
        {
            if (ColaboradorId > 0)
            {
                IsEditMode = true;
                Title = "Editar Colaborador";
                await LoadAsync();
            }
            else
            {
                IsEditMode = false;
                Title = "Novo Colaborador";
            }
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            if (ColaboradorId <= 0)
                return;

            try
            {
                IsBusy = true;

                var data = await _colaboradorService.ObterPorIdAsync(ColaboradorId);

                if (data != null)
                {
                    Colaborador = data;
                    OnPropertyChanged(nameof(DataNascimentoDate));
                    OnPropertyChanged(nameof(DataAdmissaoDate));
                    IsEditMode = true;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar colaborador: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (IsBusy)
                return;

            if (!ValidateColaborador(Colaborador))
                return;

            try
            {
                IsBusy = true;

                if (IsEditMode)
                {
                    await _colaboradorService.AtualizarAsync(Colaborador);
                    await Shell.Current.DisplayAlert("Sucesso", "Colaborador atualizado com sucesso!", "OK");
                }
                else
                {
                    await _colaboradorService.AdicionarAsync(Colaborador);
                    await Shell.Current.DisplayAlert("Sucesso", "Colaborador cadastrado com sucesso!", "OK");
                }

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao salvar colaborador: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private static bool ValidateColaborador(ColaboradorDTO colaborador)
        {
            const string title = "Validação";

            if (string.IsNullOrWhiteSpace(colaborador.Cpf))
            {
                Shell.Current.DisplayAlert(title, "CPF é obrigatório.", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(colaborador.Nome))
            {
                Shell.Current.DisplayAlert(title, "Nome é obrigatório.", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(colaborador.Telefone))
            {
                Shell.Current.DisplayAlert(title, "Telefone é obrigatório.", "OK");
                return false;
            }

            // Aqui dá pra colocar mais validações (tipo, vínculo, etc.)

            return true;
        }
    }
}
