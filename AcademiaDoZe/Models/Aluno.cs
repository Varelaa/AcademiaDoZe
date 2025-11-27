//Gabriel Souza Varela

using System.Collections.Generic;

namespace AcademiaDoZe.Models
{
    public class Aluno : Pessoa
    {
        public List<Matricula> Matriculas { get; set; }
        public List<RegistroEntradaSaida> EntradaSaida { get; set; }

        public Aluno(string nome, string cpf, DateTime dataNascimento, string telefone, string email, string senha, string foto, Endereco endereco)
            : base(nome, cpf, dataNascimento, telefone, email, senha, foto, endereco)
        {
            Matriculas = new List<Matricula>();
            EntradaSaida = new List<RegistroEntradaSaida>();
        }
    }
}