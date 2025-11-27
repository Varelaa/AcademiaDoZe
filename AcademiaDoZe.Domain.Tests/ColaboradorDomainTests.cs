//Gabriel Souza Varela

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests
{
    public class ColaboradorDomainTests
    {
        private Logradouro GetValidLogradouro() => Logradouro.Criar("12345678", "Rua A", "Centro", "Cidade", "SP", "Brasil");
        private Arquivo GetValidArquivo() => Arquivo.Criar(new byte[1], ".jpg");

        [Fact]
        public void CriarColaborador_ComDadosValidos_DeveCriarObjeto()
        {
            var colaborador = Colaborador.Criar(
                "Fulano",
                "12345678901",
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
                "11999999999",
                "email@teste.com",
                GetValidLogradouro(),
                "123",
                "Apto 1",
                "Senha@123",
                GetValidArquivo(),
                DateOnly.FromDateTime(DateTime.Today.AddYears(-1)),
                EColaboradorTipo.Administrador,
                EColaboradorVinculo.Estagio
            );

            Assert.NotNull(colaborador);
        }

        [Fact]
        public void CriarColaborador_ComNomeVazio_DeveLancarExcecao()
        {
            var ex = Assert.Throws<DomainException>(() =>
                Colaborador.Criar(
                    "",
                    "12345678901",
                    DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
                    "11999999999",
                    "email@teste.com",
                    GetValidLogradouro(),
                    "123",
                    "Apto 1",
                    "Senha@123",
                    GetValidArquivo(),
                    DateOnly.FromDateTime(DateTime.Today.AddYears(-1)),
                    EColaboradorTipo.Administrador,
                    EColaboradorVinculo.CLT
                )
            );

            Assert.Equal("NOME_OBRIGATORIO", ex.Message);
        }
    }
}