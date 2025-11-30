using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    [QueryProperty(nameof(ColaboradorId), "Id")]
    public partial class ColaboradorViewModel : BaseViewModel
    {
        private readonly IColaboradorService _servico;

        public ColaboradorViewModel(IColaboradorService servico)
        {
            _servico = servico;

            Colaborador = new ColaboradorDTO
            {
                Nome = "",
                Cpf = "",
                Email = "",
                Telefone = "",
                Numero = "",
                Complemento = "",
                DataAdmissao = DateOnly.FromDateTime(DateTime.Today),
                DataNascimento = DateOnly.FromDateTime(DateTime.Today),
                Endereco = new LogradouroDTO
                {
                    Cep = "88508180",
                    Nome = "teste",
                    Bairro = "teste1",
                    Cidade = "teste2",
                    Estado = "SP",
                    Pais = "Brasil"
                }

            };
        }

        // DTO
        private ColaboradorDTO _colaborador;
        public ColaboradorDTO Colaborador
        {
            get => _colaborador;
            set => SetProperty(ref _colaborador, value);
        }

        // Propriedades auxiliares
        public DateTime DataNascimentoDate
        {
            get => Colaborador.DataNascimento.ToDateTime(TimeOnly.MinValue);
            set
            {
                Colaborador.DataNascimento = DateOnly.FromDateTime(value);
                OnPropertyChanged();
            }
        }

        public DateTime DataAdmissaoDate
        {
            get => Colaborador.DataAdmissao.ToDateTime(TimeOnly.MinValue);
            set
            {
                Colaborador.DataAdmissao = DateOnly.FromDateTime(value);
                OnPropertyChanged();
            }
        }

        // ID recebido pela rota
        private int _colaboradorId;
        public int ColaboradorId
        {
            get => _colaboradorId;
            set
            {
                if (SetProperty(ref _colaboradorId, value))
                    Task.Run(InitializeAsync);
            }
        }

        public bool IsEditMode { get; set; }

        // Inicialização
        public async Task InitializeAsync()
        {
            if (ColaboradorId <= 0)
            {
                IsEditMode = false;
                return;
            }

            var data = await _servico.ObterPorIdAsync(ColaboradorId);
            if (data != null)
            {
                Colaborador = data;
                OnPropertyChanged(nameof(DataNascimentoDate));
                OnPropertyChanged(nameof(DataAdmissaoDate));
                IsEditMode = true;
            }
        }

        // CANCELAR
        [RelayCommand]
        public async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        // SALVAR
        [RelayCommand]
        public async Task SaveAsync()
        {
            if (IsBusy)
                return;

            if (!Validate(Colaborador))
                return;

            IsBusy = true;

            try
            {
                if (IsEditMode)
                    await _servico.AtualizarAsync(Colaborador);
                else
                    await _servico.AdicionarAsync(Colaborador);

                await Shell.Current.DisplayAlert("Sucesso",
                    IsEditMode ? "Colaborador atualizado!" : "Colaborador cadastrado!",
                    "OK");

                await Shell.Current.GoToAsync("..");
            }
            finally
            {
                IsBusy = false;
            }
        }


        private bool Validate(ColaboradorDTO c)
        {
            if (string.IsNullOrWhiteSpace(c.Nome))
            {
                Shell.Current.DisplayAlert("Erro", "Nome é obrigatório.", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(c.Cpf))
            {
                Shell.Current.DisplayAlert("Erro", "CPF é obrigatório.", "OK");
                return false;
            }

            return true;
        }
    }
}
