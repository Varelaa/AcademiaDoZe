//Gabriel Souza Varela

namespace AcademiaDoZe.Models
{
    public class RegistroEntradaSaida
    {
        public Pessoa Pessoa { get; set; }
        public DateTime? Entrada { get; set; }
        public DateTime? Saida { get; set; }

        public RegistroEntradaSaida(Pessoa pessoa)
        {
            Pessoa = pessoa;
        }

        public TimeSpan? TempoPermanencia()
        {
            if (Entrada.HasValue && Saida.HasValue)
            {
                return Saida - Entrada;
            }
            return null;
        }
    }
}
