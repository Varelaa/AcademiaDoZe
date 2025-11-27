//Gabriel Souza Varela

using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Exceptions;

namespace AcademiaDoZe.Domain.Entities
{
    public class Acesso : Entity
    {
        public EPessoaTipo Tipo { get; private set; }
        public Pessoa AlunoColaborador { get; private set; }
        public DateTime DataHora { get; private set; }
        public int? AlunoId { get; set; }
        public DateTime? Entrada { get; set; }
        public int? ColaboradorId { get; set; }
        public DateTime? Saida { get; set; }

        private Acesso(EPessoaTipo tipo, Pessoa pessoa, DateTime dataHora)
            : base()
        {
            Tipo = tipo;
            AlunoColaborador = pessoa;
            DataHora = dataHora;
        }

        public static Acesso Criar(EPessoaTipo tipo, Pessoa pessoa, DateTime dataHora)
        {
            if (!Enum.IsDefined(typeof(EPessoaTipo), tipo))
                throw new DomainException("TIPO_OBRIGATORIO");

            if (pessoa is null)
                throw new DomainException("PESSOA_OBRIGATORIA");

            if (dataHora == default)
                throw new DomainException("DATAHORA_OBRIGATORIA");

            if (pessoa is Aluno)
            {
            }
            else if (pessoa is Colaborador)
            {
            }

            return new Acesso(tipo, pessoa, dataHora);
        }
    }
}
