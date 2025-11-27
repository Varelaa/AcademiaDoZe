//Gabriel Souza Varela

namespace AcademiaDoZe.Models
{
    public class Pessoa
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Foto { get; set; }
        public Endereco Endereco { get; set; }

        public Pessoa(string nome, string cpf, DateTime dataNascimento, string telefone, string email, string senha, string foto, Endereco endereco)
        {
            Nome = nome;
            Cpf = cpf;
            DataNascimento = dataNascimento;
            Telefone = telefone;
            Email = email;
            Senha = senha;
            Foto = foto;
            Endereco = endereco;
        }
    }
}