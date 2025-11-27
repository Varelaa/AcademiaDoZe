//Gabriel Souza Varela

namespace AcademiaDoZe.Models
{
    public class Endereco
    {
        public string Cep { get; set; }
        public string Pais { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string NomeLogradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }

        public Endereco(string cep, string pais, string estado, string cidade, string bairro, string nomeLogradouro, string numero, string complemento)
        {
            Cep = cep;
            Pais = pais;
            Estado = estado;
            Cidade = cidade;
            Bairro = bairro;
            NomeLogradouro = nomeLogradouro;
            Numero = numero;
            Complemento = complemento;
        }
    }
}