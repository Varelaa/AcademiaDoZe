// Gabriel Souza Varela

using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Application.DTOs
{
    public class ColaboradorDTO
    {
        public int Id { get; set; }

        // Campos básicos usados na tela
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string? Email { get; set; }

        // Como você tem DatePicker na tela, precisa iniciar com data válida
        public DateOnly DataNascimento { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly DataAdmissao { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        // Endereço NÃO está na tela → precisa ser opcional!
        public LogradouroDTO? Endereco { get; set; }

        public string Numero { get; set; } = string.Empty;
        public string? Complemento { get; set; }

        // Ainda não usados
        public string? Senha { get; set; }
        public ArquivoDTO? Foto { get; set; }

        // Tipo e Vínculo ainda não estão na UI → valores padrão
        public EColaboradorTipo Tipo { get; set; } = EColaboradorTipo.Instrutor;
        public EColaboradorVinculo Vinculo { get; set; } = EColaboradorVinculo.CLT;
    }
}
