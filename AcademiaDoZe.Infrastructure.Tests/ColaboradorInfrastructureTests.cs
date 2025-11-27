//Gabriel Souza Varela 

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;
using AcademiaDoZe.Domain.Exceptions;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class ColaboradorInfrastructureTests : TestBase
    {
        private async Task RemoveIfExists(string cpf)
        {
            var repo = new ColaboradorRepository(ConnectionString, DatabaseType);
            var existente = await repo.ObterPorCpfAsync(cpf);
            if (existente != null)
                await repo.Remover(existente.Id);
        }

        private Logradouro FakeLogradouro() =>
            Logradouro.Criar("12345678", "Rua A", "Centro", "Cidade", "SP", "Brasil");

        private Arquivo FakeArquivo(byte[] bytes) =>
            Arquivo.Criar(bytes, ".jpg");

        [Fact]
        public async Task Colaborador_LogradouroPorId_CpfJaExiste_Adicionar()
        {
            var cpf = "08037547957";
            await RemoveIfExists(cpf);

            var repoLog = new LogradouroRepository(ConnectionString, DatabaseType);
            var logradouro = await repoLog.ObterPorId(4);

            var arquivo = FakeArquivo(new byte[] { 1, 2, 3 });

            var repo = new ColaboradorRepository(ConnectionString, DatabaseType);
            var existe = await repo.CpfJaExisteAsync(cpf);
            Assert.False(existe);

            var colaborador = Colaborador.Criar(
                "zé",
                cpf,
                new DateOnly(2010, 10, 09),
                "49999999999",
                "teste@com.br",
                logradouro!,
                "222",
                "complemento apto",
                "abecEbolinhas",
                arquivo,
                new DateOnly(2024, 05, 04),
                EColaboradorTipo.Atendente,
                EColaboradorVinculo.CLT
            );

            var inserido = await repo.Adicionar(colaborador);

            Assert.NotNull(inserido);
            Assert.True(inserido.Id > 0);

            await repo.Remover(inserido.Id);
        }

        [Fact]
        public async Task Colaborador_ObterPorCpf_Atualizar()
        {
            var cpf = "01734448903";
            await RemoveIfExists(cpf);

            var repoLog = new LogradouroRepository(ConnectionString, DatabaseType);
            var logradouro = await repoLog.ObterPorId(4);

            var repo = new ColaboradorRepository(ConnectionString, DatabaseType);

            var arquivo = FakeArquivo(new byte[] { 1, 2, 3 });

            var inicial = Colaborador.Criar(
                "Teste Inicial",
                cpf,
                new DateOnly(1990, 1, 1),
                "49999999989",
                "teste@teste.com",
                logradouro!,
                "124",
                "Comp",
                "Senha123!@#",
                arquivo,
                DateOnly.FromDateTime(DateTime.Today),
                EColaboradorTipo.Atendente,
                EColaboradorVinculo.CLT
            );

            var inserido = await repo.Adicionar(inicial);

            var existente = await repo.ObterPorCpfAsync(cpf);
            Assert.NotNull(existente);

            var atualizado = Colaborador.Criar(
                "Zé dos Testes Atualizado",
                existente.Cpf,
                existente.DataNascimento,
                existente.Telefone,
                existente.Email,
                existente.Endereco,
                existente.Numero,
                existente.Complemento,
                existente.Senha,
                arquivo,
                existente.DataAdmissao,
                existente.Tipo,
                existente.Vinculo
            );

            typeof(Entity).GetProperty("Id")!.SetValue(atualizado, existente.Id);

            var result = await repo.Atualizar(atualizado);

            Assert.NotNull(result);
            Assert.Equal("Zé dos Testes Atualizado", result.Nome);

            await repo.Remover(existente.Id);
        }

        [Fact]
        public async Task Colaborador_ObterPorCpf_TrocarSenha()
        {
            var cpf = "01734448903";
            await RemoveIfExists(cpf);

            var repoLog = new LogradouroRepository(ConnectionString, DatabaseType);
            var logradouro = await repoLog.ObterPorId(4);

            var repo = new ColaboradorRepository(ConnectionString, DatabaseType);

            var colaborador = Colaborador.Criar(
                "Teste TrocarSenha",
                cpf,
                new DateOnly(1990, 1, 1),
                "49999999900",
                "teste@troca.com",
                logradouro!,
                "10",
                "Comp",
                "Senha123!",
                FakeArquivo(new byte[] { 1, 2, 3 }),
                DateOnly.FromDateTime(DateTime.Today),
                EColaboradorTipo.Atendente,
                EColaboradorVinculo.CLT
            );

            var inserido = await repo.Adicionar(colaborador);

            var novaSenha = "novaSenha123";
            var ok = await repo.TrocarSenhaAsync(inserido.Id, novaSenha);

            Assert.True(ok);

            var atualizado = await repo.ObterPorId(inserido.Id);

            Assert.NotNull(atualizado);
            Assert.Equal(novaSenha, atualizado!.Senha);

            await repo.Remover(inserido.Id);
        }

        [Fact]
        public async Task Colaborador_ObterPorCpf_Remover_ObterPorId()
        {
            var cpf = "01734448903";
            await RemoveIfExists(cpf);

            var repoLog = new LogradouroRepository(ConnectionString, DatabaseType);
            var logradouro = await repoLog.ObterPorId(4);

            var repo = new ColaboradorRepository(ConnectionString, DatabaseType);

            var colaborador = Colaborador.Criar(
                "Teste Remover",
                cpf,
                new DateOnly(1990, 1, 1),
                "49999999989",
                "teste@remover.com",
                logradouro!,
                "1",
                "Comp",
                "Senha123!",
                FakeArquivo(new byte[] { 1 }),
                DateOnly.FromDateTime(DateTime.Today),
                EColaboradorTipo.Atendente,
                EColaboradorVinculo.CLT
            );

            var inserido = await repo.Adicionar(colaborador);

            var removed = await repo.Remover(inserido.Id);
            Assert.True(removed);

            var check = await repo.ObterPorId(inserido.Id);
            Assert.Null(check);
        }

        [Fact]
        public async Task Colaborador_ObterTodos()
        {
            var repo = new ColaboradorRepository(ConnectionString, DatabaseType);
            var lista = await repo.ObterTodos();
            Assert.NotNull(lista);
        }
    }
}
