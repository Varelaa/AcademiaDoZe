//Gabriel Souza Varela

using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.Services;
using System.Linq;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public sealed record Arquivo
    {
        public byte[] Conteudo { get; }
        public string Tipo { get; }

        private Arquivo(byte[] conteudo, string tipo)
        {
            Conteudo = conteudo;
            Tipo = tipo;
        }

        public static Arquivo Criar(byte[] conteudo, string tipoArquivo)
        {
            DomainException.ThrowIf(conteudo == null || conteudo.Length == 0, "ARQUIVO_VAZIO");

            DomainException.ThrowIf(NormalizadoService.TextoVazioOuNulo(tipoArquivo), "ARQUIVO_TIPO_OBRIGATORIO");

            var permitido = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".docx" };
            var tipo = NormalizeTipo(tipoArquivo);

            DomainException.ThrowIf(!permitido.Contains(tipo), "ARQUIVO_TIPO_INVALIDO");

            const int max = 5 * 1024 * 1024; // 5 MB

            DomainException.ThrowIf(conteudo.Length > max, "ARQUIVO_TAMANHO_INVALIDO");

            return new Arquivo(conteudo, tipo);
        }

        public static Arquivo Criar(byte[] conteudo)
        {
            DomainException.ThrowIf(conteudo == null || conteudo.Length == 0, "ARQUIVO_VAZIO");

            return new Arquivo(conteudo, ".bin");
        }

        private static string NormalizeTipo(string? tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo))
                return ".bin";

            var t = tipo.Trim().ToLowerInvariant();

            if (!t.StartsWith("."))
                t = "." + t;

            return t;
        }
    }
}