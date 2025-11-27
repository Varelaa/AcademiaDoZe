//Gabriel Souza Varela

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests
{
    public class MatriculaDomainServices
    {
        private Logradouro GetValidLogradouro() =>
            Logradouro.Criar("12345678", "Rua A", "Centro", "Cidade", "SP", "Brasil");

        private Arquivo GetValidArquivo() =>
            Arquivo.Criar(new byte[1], ".jpg");

        private Aluno GetValidAluno() =>
            Aluno.Criar(
                "Aluno Teste",
                "12345678901",
                new DateOnly(2000, 1, 1),
                "11999999999",
                "aluno@teste.com",
                GetValidLogradouro(),
                "123",
                "Apto 1",
                "Senha@123",
                GetValidArquivo()
            );

        [Fact]
        public void CriarMatricula_ComDadosValidos_DeveCriarObjeto()
        {
            var aluno = GetValidAluno();

            var matricula = Matricula.Criar(
                aluno,
                EMatriculaPlano.Mensal,
                DateOnly.FromDateTime(DateTime.Today),
                DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
                "Hipertrofia",
                EMatriculaRestricoes.None,
                null,
                ""
            );

            Assert.NotNull(matricula);
        }

        [Fact]
        public void CriarMatricula_ComPlanoInvalido_DeveLancarExcecao()
        {
            var aluno = GetValidAluno();

            var ex = Assert.Throws<DomainException>(() =>
                Matricula.Criar(
                    aluno,
                    (EMatriculaPlano)99,
                    DateOnly.FromDateTime(DateTime.Today),
                    DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
                    "Hipertrofia",
                    EMatriculaRestricoes.None,
                    null,
                    ""
                )
            );

            Assert.Equal("PLANO_INVALIDO", ex.Message);
        }
    }
}