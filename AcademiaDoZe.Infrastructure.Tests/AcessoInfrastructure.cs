// Gabriel Souza Varela

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.ValueObjects;
using System;
using Xunit;

namespace AcademiaDoZe.Tests
{
    public class AcessoTests
    {
        private Logradouro Endereco() =>
            Logradouro.Criar("12345678", "Rua A", "Centro", "Cidade", "SP", "Brasil");

        private Arquivo Foto() =>
            Arquivo.Criar(new byte[] { 1 }, ".jpg");

        private Aluno NovoAluno() =>
            Aluno.Criar(
                "Gabriel Souza",
                "12345678901",
                new DateOnly(2000, 1, 1),
                "11999999999",
                "gabriel@email.com",
                Endereco(),
                "123",
                "Ap",
                "Senha@123",
                Foto()
            );

        private Colaborador NovoColaborador() =>
            Colaborador.Criar(
                "Maria Silva",
                "10987654321",
                new DateOnly(1990, 5, 5),
                "11988888888",
                "maria@email.com",
                Endereco(),
                "12",
                "Sala 3",
                "Senha@123",
                Foto(),
                new DateOnly(2020, 1, 1),
                EColaboradorTipo.Instrutor,
                EColaboradorVinculo.CLT
            );

        [Fact]
        public void CriarAcesso_AlunoValido_DeveRetornarObjeto()
        {
            var aluno = NovoAluno();
            var dataHora = DateTime.Today.AddHours(10);

            var acesso = Acesso.Criar(EPessoaTipo.Aluno, aluno, dataHora);

            Assert.NotNull(acesso);
            Assert.Equal(EPessoaTipo.Aluno, acesso.Tipo);
            Assert.Equal(aluno, acesso.AlunoColaborador);
            Assert.Equal(dataHora, acesso.DataHora);
        }

        [Fact]
        public void CriarAcesso_ColaboradorValido_DeveRetornarObjeto()
        {
            var colaborador = NovoColaborador();
            var dataHora = DateTime.Today.AddHours(12);

            var acesso = Acesso.Criar(EPessoaTipo.Colaborador, colaborador, dataHora);

            Assert.NotNull(acesso);
            Assert.Equal(EPessoaTipo.Colaborador, acesso.Tipo);
            Assert.Equal(colaborador, acesso.AlunoColaborador);
            Assert.Equal(dataHora, acesso.DataHora);
        }

        [Fact]
        public void CriarAcesso_PessoaNula_DeveLancarDomainException()
        {
            Pessoa pessoa = null!;
            var dataHora = DateTime.Today.AddHours(10);

            var ex = Assert.Throws<DomainException>(() => Acesso.Criar(EPessoaTipo.Aluno, pessoa, dataHora));
            Assert.Equal("PESSOA_OBRIGATORIA", ex.Message);
        }

        [Fact]
        public void CriarAcesso_DataHoraForaDoIntervalo_DeveLancarDomainException()
        {
            var aluno = NovoAluno();

            var antes = DateTime.Today.AddHours(5);
            var depois = DateTime.Today.AddHours(23);

            var ex1 = Assert.Throws<DomainException>(() => Acesso.Criar(EPessoaTipo.Aluno, aluno, antes));
            Assert.Equal("DATAHORA_INTERVALO", ex1.Message);

            var ex2 = Assert.Throws<DomainException>(() => Acesso.Criar(EPessoaTipo.Aluno, aluno, depois));
            Assert.Equal("DATAHORA_INTERVALO", ex2.Message);
        }

        [Fact]
        public void CriarAcesso_DataHoraPassada_DeveLancarDomainException()
        {
            var aluno = NovoAluno();
            var dataPassada = DateTime.Today.AddDays(-1);

            var ex = Assert.Throws<DomainException>(() => Acesso.Criar(EPessoaTipo.Aluno, aluno, dataPassada));
            Assert.Equal("DATAHORA_INVALIDA", ex.Message);
        }
    }
}
