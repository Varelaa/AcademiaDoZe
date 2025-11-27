// Gabriel Souza Varela

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.ValueObjects;
using Xunit;

namespace AcademiaDoZe.Domain.Tests
{
    public class AlunoDomainTests
    {  
        private Logradouro GetValidLogradouro()
        {
            return Logradouro.Criar(
                1,
                "12345678",
                "Rua A",
                "Centro",
                "Cidade",
                "SP",
                "Brasil"
            );
        }

        private Arquivo GetValidArquivo()
        {
            return Arquivo.Criar(new byte[1]);
        }

        private static DateOnly MaiorDeIdade()
        {
            return DateOnly.FromDateTime(DateTime.Today.AddYears(-20));
        }

        [Fact]
        public void CriarAluno_ComDadosValidos_DeveCriarObjeto()
        {
            var aluno = Aluno.Criar(
                "João da Silva",
                "12345678901",
                MaiorDeIdade(),
                "11999999999",
                "joao@email.com",
                GetValidLogradouro(),
                "123",
                "Apto 1",
                "Senha@123",
                GetValidArquivo()
            );

            Assert.NotNull(aluno);
            Assert.Equal("João da Silva", aluno.Nome);
            Assert.Equal("12345678901", aluno.Cpf);
        }

        [Fact]
        public void CriarAluno_ComNomeVazio_DeveLancarExcecao()
        {
            var ex = Assert.Throws<DomainException>(() =>
                Aluno.Criar(
                    "",
                    "12345678901",
                    MaiorDeIdade(),
                    "11999999999",
                    "teste@teste.com",
                    GetValidLogradouro(),
                    "123",
                    "Apto 1",
                    "Senha@123",
                    GetValidArquivo()
                )
            );

            Assert.Equal("NOME_OBRIGATORIO", ex.Message);
        }

        [Fact]
        public void CriarAluno_ComCpfInvalido_DeveLancarExcecao()
        {
            var ex = Assert.Throws<DomainException>(() =>
                Aluno.Criar(
                    "Fulano",
                    "123", 
                    MaiorDeIdade(),
                    "11999999999",
                    "email@email.com",
                    GetValidLogradouro(),
                    "123",
                    "",
                    "Senha@123",
                    GetValidArquivo()
                )
            );

            Assert.Equal("CPF_DIGITOS", ex.Message);
        }

        [Fact]
        public void CriarAluno_ComTelefoneInvalido_DeveLancarExcecao()
        {
            var ex = Assert.Throws<DomainException>(() =>
                Aluno.Criar(
                    "Fulano",
                    "12345678901",
                    MaiorDeIdade(),
                    "123", 
                    "email@email.com",
                    GetValidLogradouro(),
                    "123",
                    "",
                    "Senha@123",
                    GetValidArquivo()
                )
            );

            Assert.Equal("TELEFONE_DIGITOS", ex.Message);
        }

        [Fact]
        public void CriarAluno_ComEmailInvalido_DeveLancarExcecao()
        {
            var ex = Assert.Throws<DomainException>(() =>
                Aluno.Criar(
                    "Fulano",
                    "12345678901",
                    MaiorDeIdade(),
                    "11999999999",
                    "email_invalido",
                    GetValidLogradouro(),
                    "123",
                    "",
                    "Senha@123",
                    GetValidArquivo()
                )
            );

            Assert.Equal("EMAIL_FORMATO", ex.Message);
        }

        [Fact]
        public void CriarAluno_ComSenhaInvalida_DeveLancarExcecao()
        {
            var ex = Assert.Throws<DomainException>(() =>
                Aluno.Criar(
                    "Fulano",
                    "12345678901",
                    MaiorDeIdade(),
                    "11999999999",
                    "email@email.com",
                    GetValidLogradouro(),
                    "123",
                    "",
                    "abc", 
                    GetValidArquivo()
                )
            );

            Assert.Equal("SENHA_FORMATO", ex.Message);
        }

        [Fact]
        public void CriarAluno_ComDataNascimentoInvalida_DeveLancarExcecao()
        {
            var menorDeIdade = DateOnly.FromDateTime(DateTime.Today.AddYears(-10));

            var ex = Assert.Throws<DomainException>(() =>
                Aluno.Criar(
                    "Fulano",
                    "12345678901",
                    menorDeIdade,
                    "11999999999",
                    "email@email.com",
                    GetValidLogradouro(),
                    "123",
                    "",
                    "Senha@123",
                    GetValidArquivo()
                )
            );

            Assert.Equal("DATA_NASCIMENTO_MINIMA_INVALIDA", ex.Message);
        }

        [Fact]
        public void CriarAluno_ComLogradouroNulo_DeveLancarExcecao()
        {
            var ex = Assert.Throws<DomainException>(() =>
                Aluno.Criar(
                    "Fulano",
                    "12345678901",
                    MaiorDeIdade(),
                    "11999999999",
                    "email@email.com",
                    null!,
                    "123",
                    "",
                    "Senha@123",
                    GetValidArquivo()
                )
            );

            Assert.Equal("LOGRADOURO_OBRIGATORIO", ex.Message);
        }
    }
}