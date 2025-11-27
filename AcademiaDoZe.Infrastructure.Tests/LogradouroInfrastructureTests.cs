// Gabriel Souza Varela

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class LogradouroInfrastructureTests : TestBase
    {
        [Fact]
        public async Task Logradouro_Adicionar()
        {
            var cep = "12345678";

            var logradouro = Logradouro.Criar(
                cep,
                "Rua dos Testes",
                "Bairro Teste",
                "Cidade Teste",
                "TS",
                "Pais Teste"
            );

            var repo = new LogradouroRepository(ConnectionString, DatabaseType);
            var inserido = await repo.Adicionar(logradouro);

            Assert.NotNull(inserido);
            Assert.True(inserido.Id > 0);
        }

        [Fact]
        public async Task Logradouro_ObterPorCep_Atualizar()
        {
            var cep = "12345678";

            var repo = new LogradouroRepository(ConnectionString, DatabaseType);
            var logradouro = await repo.ObterPorCepAsync(cep);

            Assert.NotNull(logradouro);

            var atualizado = Logradouro.Criar(
                cep,
                "Rua Atualizada",
                "Bairro Atualizado",
                "Cidade Atualizada",
                "AT",
                "Pais Atualizado"
            );

            typeof(Entity).GetProperty("Id")!.SetValue(atualizado, logradouro!.Id);

            var repoEdit = new LogradouroRepository(ConnectionString, DatabaseType);
            var resultado = await repoEdit.Atualizar(atualizado);

            Assert.NotNull(resultado);
            Assert.Equal("AT", resultado.Estado);
            Assert.Equal("Rua Atualizada", resultado.Nome);
        }

        [Fact]
        public async Task Logradouro_ObterPorCep_Remover_ObterPorId()
        {
            var cep = "12345678";

            var repo = new LogradouroRepository(ConnectionString, DatabaseType);
            var existente = await repo.ObterPorCepAsync(cep);

            Assert.NotNull(existente);

            var removido = await repo.Remover(existente!.Id);
            Assert.True(removido);

            var repoBusca = new LogradouroRepository(ConnectionString, DatabaseType);
            var check = await repoBusca.ObterPorId(existente.Id);

            Assert.Null(check);
        }

        [Fact]
        public async Task Logradouro_ObterPorCidade()
        {
            var cidade = "Lages";

            var repo = new LogradouroRepository(ConnectionString, DatabaseType);
            var lista = await repo.ObterPorCidadeAsync(cidade);

            Assert.NotNull(lista);
            Assert.NotEmpty(lista);
        }

        [Fact]
        public async Task Logradouro_ObterTodos()
        {
            var repo = new LogradouroRepository(ConnectionString, DatabaseType);
            var lista = await repo.ObterTodos();

            Assert.NotNull(lista);
        }
    }
}
