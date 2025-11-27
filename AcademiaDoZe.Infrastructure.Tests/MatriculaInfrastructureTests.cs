// Gabriel Souza Varela

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class MatriculaInfrastructureTests : TestBase
    {
        private async Task EnsureNoMatricula(int alunoId)
        {
            var repo = new MatriculaRepository(ConnectionString, DatabaseType);
            var matriculas = await repo.ObterPorAlunoAsync(alunoId);

            foreach (var m in matriculas)
                await repo.Remover(m.Id);
        }

        [Fact]
        public async Task Matricula_Adicionar_Atualizar_Remover_ObterTodos()
        {
            var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);
            var alunos = await repoAluno.ObterTodos();
            var alunoTeste = alunos.FirstOrDefault();

            if (alunoTeste == null)
            {
                var foto = Domain.ValueObjects.Arquivo.Criar(new byte[] { 1, 2, 3 }, ".jpg");
                alunoTeste = Aluno.Criar(
                    "Aluno Matricula",
                    "98765432100",
                    new DateOnly(2005, 5, 5),
                    "49999999988",
                    "aluno@matricula.com",
                    CriarEnderecoPadrao(),
                    "101",
                    "Apt 2",
                    "Senha@123",
                    foto
                );

                alunoTeste = await repoAluno.Adicionar(alunoTeste);
            }

            var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
            await EnsureNoMatricula(alunoTeste.Id);

            var matricula = Matricula.Criar(
                alunoTeste,
                EMatriculaPlano.Mensal,
                DateOnly.FromDateTime(DateTime.Today),
                DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
                "Objetivo Teste",
                EMatriculaRestricoes.None,
                null,
                ""
            );

            var matriculaInserida = await repoMatricula.Adicionar(matricula);
            Assert.NotNull(matriculaInserida);
            Assert.True(matriculaInserida.Id > 0);

            var matriculaAtualizada = Matricula.Criar(
                alunoTeste,
                EMatriculaPlano.Semestral,
                matriculaInserida.DataInicio,
                matriculaInserida.DataFim,
                "Objetivo Atualizado",
                EMatriculaRestricoes.None,
                null,
                ""
            );

            typeof(Entity).GetProperty("Id")!.SetValue(matriculaAtualizada, matriculaInserida.Id);

            var resultadoAtualizacao = await repoMatricula.Atualizar(matriculaAtualizada);
            Assert.NotNull(resultadoAtualizacao);
            Assert.Equal("Objetivo Atualizado", resultadoAtualizacao.Objetivo);

            var removido = await repoMatricula.Remover(matriculaAtualizada.Id);
            Assert.True(removido);

            var apagada = await repoMatricula.ObterPorId(matriculaAtualizada.Id);
            Assert.Null(apagada);
        }

        [Fact]
        public async Task Matricula_ObterTodos()
        {
            var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
            var resultado = await repoMatricula.ObterTodos();
            Assert.NotNull(resultado);
        }

        private Logradouro CriarEnderecoPadrao()
        {
            return Logradouro.Criar(
                "12345678",
                "Rua Teste",
                "Centro",
                "Cidade Teste",
                "TS",
                "Brasil"
            );
        }
    }
}
