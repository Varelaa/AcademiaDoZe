//Gabriel Souza Varela

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Application.Tests
{
    public class MoqColaboradorServiceTests
    {
        private readonly Mock<IColaboradorService> _colaboradorServiceMock;
        private readonly IColaboradorService _colaboradorService;

        public MoqColaboradorServiceTests()
        {
            _colaboradorServiceMock = new Mock<IColaboradorService>();
            _colaboradorService = _colaboradorServiceMock.Object;
        }

        private ColaboradorDTO CriarColaboradorPadrao(int id = 1)
        {
            return new ColaboradorDTO
            {
                Id = id,
                Nome = "Colaborador Teste",
                Cpf = "12345678901",
                DataNascimento = DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
                Telefone = "11999999999",
                Email = "colaborador@teste.com",

                Endereco = new LogradouroDTO
                {
                    Id = 1,
                    Cep = "12345678",
                    Nome = "Rua Teste",
                    Bairro = "Centro",
                    Cidade = "São Paulo",
                    Estado = "SP",
                    Pais = "Brasil"
                },

                Numero = "100",
                Complemento = "Apto 101",
                Senha = "Senha@123",

                DataAdmissao = DateOnly.FromDateTime(DateTime.Now.AddYears(-1)),
                Tipo = EColaboradorTipo.Administrador,
                Vinculo = EColaboradorVinculo.CLT
            };
        }

        [Theory]
        [InlineData("12345678901", null, true)]
        [InlineData("12345678901", 1, false)]
        [InlineData("99999999999", null, false)]
        public async Task CpfJaExisteAsync_DeveRetornarResultadoCorreto(string cpf, int? id, bool esperado)
        {
            _colaboradorServiceMock.Setup(s => s.CpfJaExisteAsync(cpf, id))
                .ReturnsAsync(esperado);

            var resultado = await _colaboradorService.CpfJaExisteAsync(cpf, id);

            Assert.Equal(esperado, resultado);
            _colaboradorServiceMock.Verify(s => s.CpfJaExisteAsync(cpf, id), Times.Once);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarColaborador_QuandoExistir()
        {
            var id = 1;
            var dto = CriarColaboradorPadrao(id);

            _colaboradorServiceMock.Setup(s => s.ObterPorIdAsync(id))
                .ReturnsAsync(dto);

            var resultado = await _colaboradorService.ObterPorIdAsync(id);

            Assert.NotNull(resultado);
            Assert.Equal(dto.Cpf, resultado.Cpf);
            _colaboradorServiceMock.Verify(s => s.ObterPorIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarNull_QuandoNaoExistir()
        {
            _colaboradorServiceMock.Setup(s => s.ObterPorIdAsync(999))
                .ReturnsAsync((ColaboradorDTO)null!);

            var resultado = await _colaboradorService.ObterPorIdAsync(999);

            Assert.Null(resultado);
            _colaboradorServiceMock.Verify(s => s.ObterPorIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarColaboradores()
        {
            var lista = new List<ColaboradorDTO>
            {
                CriarColaboradorPadrao(1),
                CriarColaboradorPadrao(2)
            };

            _colaboradorServiceMock.Setup(s => s.ObterTodosAsync())
                .ReturnsAsync(lista);

            var resultado = await _colaboradorService.ObterTodosAsync();

            Assert.Equal(2, resultado.Count());
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarVazio_QuandoNaoExistir()
        {
            _colaboradorServiceMock.Setup(s => s.ObterTodosAsync())
                .ReturnsAsync(new List<ColaboradorDTO>());

            var resultado = await _colaboradorService.ObterTodosAsync();

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task AdicionarAsync_DeveCriar_QuandoValido()
        {
            var dto = CriarColaboradorPadrao(0);
            var criado = CriarColaboradorPadrao(1);

            _colaboradorServiceMock.Setup(s => s.AdicionarAsync(It.IsAny<ColaboradorDTO>()))
                .ReturnsAsync(criado);

            var result = await _colaboradorService.AdicionarAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizar_QuandoValido()
        {
            var dto = CriarColaboradorPadrao(1);
            dto.Nome = "Atualizado";

            _colaboradorServiceMock.Setup(s => s.AtualizarAsync(It.IsAny<ColaboradorDTO>()))
                .ReturnsAsync(dto);

            var result = await _colaboradorService.AtualizarAsync(dto);

            Assert.Equal("Atualizado", result.Nome);
        }

        [Fact]
        public async Task RemoverAsync_DeveRemover_QuandoExiste()
        {
            _colaboradorServiceMock.Setup(s => s.RemoverAsync(1)).ReturnsAsync(true);

            var result = await _colaboradorService.RemoverAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task ObterPorCpfAsync_DeveRetornar_QuandoExiste()
        {
            var cpf = "12345678901";
            var dto = CriarColaboradorPadrao(1);
            dto.Cpf = cpf;

            _colaboradorServiceMock.Setup(s => s.ObterPorCpfAsync(cpf))
                .ReturnsAsync(dto);

            var result = await _colaboradorService.ObterPorCpfAsync(cpf);

            Assert.NotNull(result);
            Assert.Equal(cpf, result.Cpf);
        }

        [Fact]
        public async Task ObterPorCpfAsync_DeveRetornarNull_QuandoNaoExiste()
        {
            _colaboradorServiceMock.Setup(s => s.ObterPorCpfAsync("999"))
                .ReturnsAsync((ColaboradorDTO)null!);

            var result = await _colaboradorService.ObterPorCpfAsync("999");

            Assert.Null(result);
        }

        [Fact]
        public async Task TrocarSenhaAsync_DeveRetornarTrue_QuandoSucesso()
        {
            _colaboradorServiceMock.Setup(s => s.TrocarSenhaAsync(1, "abc")).ReturnsAsync(true);

            var result = await _colaboradorService.TrocarSenhaAsync(1, "abc");

            Assert.True(result);
        }
    }
}
