//Gabriel Souza Varela

using System.Collections.Generic;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Models
{
    public class Colaborador : Pessoa
    {
        public DateTime DataAdmissao { get; set; }
        public EColaboradorTipo Tipo { get; set; }
        public EColaboradorVinculo Vinculo { get; set; }
        public List<RegistroEntradaSaida> EntradaSaida { get; set; }

        public Colaborador(string nome,
                           string cpf,
                           DateTime dataNascimento,
                           string telefone,
                           string email,
                           string senha,
                           string foto,
                           Endereco endereco,
                           DateTime dataAdmissao,
                           EColaboradorTipo tipo,
                           EColaboradorVinculo vinculo)
            : base(nome, cpf, dataNascimento, telefone, email, senha, foto, endereco)
        {
            DataAdmissao = dataAdmissao;
            Tipo = tipo;
            Vinculo = vinculo;
            EntradaSaida = new List<RegistroEntradaSaida>();
        }
    }
}
