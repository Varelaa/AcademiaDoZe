//Gabriel Souza Varla

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class AlunoInfrastructureTests : TestBase
    {
        private Logradouro Endereco() =>
            Logradouro.Criar("12345678", "Rua A", "Centro", "Cidade", "SP", "Brasil");

        private Arquivo Foto(byte[] bytes) =>
            Arquivo.Criar(bytes, ".jpg");

        [Fact]
        public async Task Aluno_Adicionar_ObterPorCpf()
        {
            var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);

            var cpf = "98765432100";
            var aluno = Aluno.Criar(
                "João Teste",
                cpf,
                new DateOnly(2005, 5, 5),
                "49999999999",
                "joao@teste.com",
                Endereco(),
                "123",
                "Apto 1",
                "Senha@123",
                Foto(new byte[] { 1, 2, 3 })
            );

            var alunoInserido = await repoAluno.Adicionar(aluno);

            Assert.NotNull(alunoInserido);
            Assert.True(alunoInserido.Id > 0);

            var alunoObtido = await repoAluno.ObterPorCpfAsync(cpf);

            Assert.NotNull(alunoObtido);
            Assert.Equal("João Teste", alunoObtido.Nome);
        }

        [Fact]
        public async Task Aluno_Atualizar()
        {
            var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);

            var cpf = "98765432100";
            var alunoExistente = await repoAluno.ObterPorCpfAsync(cpf);

            Assert.NotNull(alunoExistente);

            var alunoAtualizado = Aluno.Criar(
                "João Atualizado",
                alunoExistente.Cpf,
                alunoExistente.DataNascimento,
                alunoExistente.Telefone,
                alunoExistente.Email,
                alunoExistente.Endereco,
                alunoExistente.Numero,
                alunoExistente.Complemento,
                alunoExistente.Senha,
                Foto(new byte[] { 4, 5, 6 })
            );

            typeof(Entity).GetProperty("Id")!.SetValue(alunoAtualizado, alunoExistente.Id);

            var result = await repoAluno.Atualizar(alunoAtualizado);

            Assert.NotNull(result);
            Assert.Equal("João Atualizado", result.Nome);
        }

        [Fact]
        public async Task Aluno_Remover_ObterPorId()
        {
            var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);

            var cpf = "98765432100";
            var alunoExistente = await repoAluno.ObterPorCpfAsync(cpf);

            Assert.NotNull(alunoExistente);

            var removido = await repoAluno.Remover(alunoExistente.Id);

            Assert.True(removido);

            var alunoRemovido = await repoAluno.ObterPorId(alunoExistente.Id);

            Assert.Null(alunoRemovido);
        }

        [Fact]
        public async Task Aluno_ObterTodos()
        {
            var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);

            var todos = await repoAluno.ObterTodos();

            Assert.NotNull(todos);
        }
    }
}
