// Gabriel Souza Varela

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Exceptions;
using Xunit;

namespace AcademiaDoZe.Domain.Tests
{
    public class AcessoDomainTests
    {

        private Aluno CriarAlunoFake()
        {
            return Aluno.Criar(
                1,
                "Aluno Teste",
                "12345678901",
                new DateOnly(2000, 1, 1),
                "11999999999",
                "aluno@teste.com",
                CriarLogradouroFake(),
                "123",
                "Apto 1",
                "Senha123",
                null
            );
        }

        private Colaborador CriarColaboradorFake()
        {
            return Colaborador.Criar(
                1,
                "Colaborador Teste",
                "98765432100",
                new DateOnly(1990, 1, 1),
                "11988887777",
                "colab@teste.com",
                CriarLogradouroFake(),
                "100",
                "Sala 2",
                "Senha123",
                null,
                new DateOnly(2020, 1, 1),
                EColaboradorTipo.Administrador,
                EColaboradorVinculo.CLT
            );
        }

        private Logradouro CriarLogradouroFake()
        {
            return Logradouro.Criar(
                1,
                "12345678",
                "Rua Teste",
                "Bairro Teste",
                "Cidade Teste",
                "SP",
                "Brasil"
            );
        }

       
        [Fact]
        public void Criar_DeveCriarAcesso_ParaAluno()
        {
            var aluno = CriarAlunoFake();
            var data = DateTime.Now;

            var acesso = Acesso.Criar(EPessoaTipo.Aluno, aluno, data);

            Assert.NotNull(acesso);
            Assert.Equal(EPessoaTipo.Aluno, acesso.Tipo);
            Assert.Equal(aluno, acesso.AlunoColaborador);
            Assert.Equal(data, acesso.DataHora);
        }

        [Fact]
        public void Criar_DeveCriarAcesso_ParaColaborador()
        {
            var colab = CriarColaboradorFake();
            var data = DateTime.Now;

            var acesso = Acesso.Criar(EPessoaTipo.Colaborador, colab, data);

            Assert.NotNull(acesso);
            Assert.Equal(EPessoaTipo.Colaborador, acesso.Tipo);
            Assert.Equal(colab, acesso.AlunoColaborador);
            Assert.Equal(data, acesso.DataHora);
        }

        [Fact]
        public void Criar_DeveLancarExcecao_QuandoTipoInvalido()
        {
            var aluno = CriarAlunoFake();

            Assert.Throws<DomainException>(() =>
                Acesso.Criar((EPessoaTipo)999, aluno, DateTime.Now)
            );
        }

        [Fact]
        public void Criar_DeveLancarExcecao_QuandoPessoaNula()
        {
            Assert.Throws<DomainException>(() =>
                Acesso.Criar(EPessoaTipo.Aluno, null!, DateTime.Now)
            );
        }

        [Fact]
        public void Criar_DeveLancarExcecao_QuandoDataHoraPadrao()
        {
            var aluno = CriarAlunoFake();

            Assert.Throws<DomainException>(() =>
                Acesso.Criar(EPessoaTipo.Aluno, aluno, default)
            );
        }

        [Fact]
        public void Criar_DevePopularCamposAdicionais()
        {
            var aluno = CriarAlunoFake();
            var data = DateTime.Now;

            var acesso = Acesso.Criar(EPessoaTipo.Aluno, aluno, data);

            Assert.Equal(EPessoaTipo.Aluno, acesso.Tipo);
            Assert.Equal(aluno, acesso.AlunoColaborador);
            Assert.Equal(data, acesso.DataHora);

            Assert.Null(acesso.Entrada);
            Assert.Null(acesso.Saida);
            Assert.Null(acesso.ColaboradorId);
        }
    }
}
