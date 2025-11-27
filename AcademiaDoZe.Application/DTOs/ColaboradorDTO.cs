//Gabriel Souza Varela

using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Application.DTOs
{
    public class ColaboradorDTO
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Cpf { get; set; }
        public required DateOnly DataNascimento { get; set; }
        public required string Telefone { get; set; }
        public string? Email { get; set; }
        public required LogradouroDTO Endereco { get; set; }
        public required string Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Senha { get; set; }
        public ArquivoDTO? Foto { get; set; }

        public required DateOnly DataAdmissao { get; set; }
        public required EColaboradorTipo Tipo { get; set; }
        public required EColaboradorVinculo Vinculo { get; set; }
    }
}
