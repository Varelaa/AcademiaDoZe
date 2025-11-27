// Gabriel Souza Varela

using System.Text.RegularExpressions;

namespace AcademiaDoZe.Domain.Services
{
    public static partial class NormalizadoService

    {
        public static bool TextoVazioOuNulo(string? texto)
        {
            return string.IsNullOrWhiteSpace(texto);
        }

        public static string LimparEspacos(string texto)
        {
            return texto?.Trim() ?? "";
        }

        public static string LimparEDigitos(string texto)
        {
            return new string((texto ?? "").Where(char.IsDigit).ToArray());
        }

        public static bool ValidarFormatoEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return true; // inválido

            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return !regex.IsMatch(email); // true = inválido (mantém sua lógica atual)
        }

        public static bool ValidarFormatoSenha(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                return true; // inválida

            // Senha deve conter letras maiúsculas, minúsculas, número e caractere especial
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$");
            return !regex.IsMatch(senha); // true = inválida
        }

        public static string LimparTodosEspacos(string? texto)
        {
            if (texto == null) return string.Empty;

            return new string(texto.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }

        public static string ParaMaiusculo(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            return texto.Trim().ToUpperInvariant();
        }

    }
}
