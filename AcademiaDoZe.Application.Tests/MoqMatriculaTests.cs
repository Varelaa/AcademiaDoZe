// Gabriel Souza Varela

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Services;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using Moq;

namespace AcademiaDoZe.Application.Tests
{
    public class MatriculaServiceTests
    {
        private readonly Mock<Func<IMatriculaRepository>> _matriculaRepoMock;
        private readonly Mock<Func<IAlunoRepository>> _alunoRepoMock;
        private readonly IMatriculaService _matriculaService;

        public MatriculaServiceTests()
        {
            _matriculaRepoMock = new Mock<Func<IMatriculaRepository>>();
            _alunoRepoMock = new Mock<Func<IAlunoRepository>>();

            _matriculaService = new MatriculaService(
                _matriculaRepoMock.Object,
                _alunoRepoMock.Object
            );
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarMatricula_QuandoExistir()
        {
            var entity = CriarMatriculaEntity();

            var repo = new Mock<IMatriculaRepository>();
            repo.Setup(r => r.ObterPorId(1))
                .ReturnsAsync(entity);

            _matriculaRepoMock.Setup(f => f()).Returns(repo.Object);

            var result = await _matriculaService.ObterPorIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarNull_QuandoNaoExistir()
        {
            var repo = new Mock<IMatriculaRepository>();
            repo.Setup(r => r.ObterPorId(999))
                .ReturnsAsync((Matricula?)null);

            _matriculaRepoMock.Setup(f => f()).Returns(repo.Object);

            var result = await _matriculaService.ObterPorIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task AdicionarAsync_DeveCriarMatricula_QuandoValida()
        {
            var dto = CriarMatriculaDto();
            var aluno = CriarAlunoEntity();

            var alunoRepo = new Mock<IAlunoRepository>();
            alunoRepo.Setup(r => r.ObterPorId(dto.AlunoId))
                     .ReturnsAsync(aluno);

            _alunoRepoMock.Setup(f => f()).Returns(alunoRepo.Object);

            var matriculaRepo = new Mock<IMatriculaRepository>();

            matriculaRepo.Setup(r => r.ObterTodos())
                .ReturnsAsync(new List<Matricula>());

            // CORREÇÃO AQUI 👇
            matriculaRepo.Setup(r => r.Adicionar(It.IsAny<Matricula>()))
                .ReturnsAsync((Matricula m) =>
                {
                    typeof(Matricula).GetProperty("Id")!.SetValue(m, 1);
                    return m;
                });

            _matriculaRepoMock.Setup(f => f()).Returns(matriculaRepo.Object);

            var result = await _matriculaService.AdicionarAsync(dto);

            Assert.NotNull(result);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarMatricula()
        {
            var dto = CriarMatriculaDto();
            dto.Id = 1;
            dto.Objetivo = "Novo Objetivo";

            var existente = CriarMatriculaEntity();

            var repo = new Mock<IMatriculaRepository>();

            repo.Setup(r => r.ObterPorId(dto.Id))
                .ReturnsAsync(existente);

            // CORREÇÃO AQUI 👇
            repo.Setup(r => r.Atualizar(It.IsAny<Matricula>()))
                .ReturnsAsync((Matricula m) => m);

            _matriculaRepoMock.Setup(f => f()).Returns(repo.Object);

            var result = await _matriculaService.AtualizarAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("Novo Objetivo", result.Objetivo);
        }

        private MatriculaDTO CriarMatriculaDto()
        {
            return new MatriculaDTO
            {
                Id = 0,
                AlunoId = 1,
                Plano = EMatriculaPlano.Mensal,
                DataInicio = DateOnly.FromDateTime(DateTime.Today),
                DataFim = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
                Objetivo = "Hipertrofia",
                RestricoesMedicas = EMatriculaRestricoes.None
            };
        }

        private Matricula CriarMatriculaEntity()
        {
            return Matricula.Criar(
                1,
                CriarAlunoEntity(),
                EMatriculaPlano.Mensal,
                DateOnly.FromDateTime(DateTime.Today),
                DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
                "Hipertrofia",
                EMatriculaRestricoes.None,
                null,
                ""
            );
        }

        private Aluno CriarAlunoEntity()
        {
            return Aluno.Criar(
                1,
                "Aluno Teste",
                "12345678901",
                new DateOnly(2000, 1, 1),
                "11999999999",
                "aluno@teste.com",
                CriarLogradouroEntity(),
                "123",
                "Apto 1",
                "Senha123",
                null
            );
        }

        private Logradouro CriarLogradouroEntity()
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
    }
}
