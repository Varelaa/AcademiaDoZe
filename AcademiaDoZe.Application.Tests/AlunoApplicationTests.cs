// Gabriel Souza Varela

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Infrastructure.Data;  
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AcademiaDoZe.Application.Tests
{
    public class AlunoApplicationTests
    {
        const string connectionString =
            "Server=localhost;Database=db_academia_do_ze;User Id=sa;Password=abcBolinhas12345;TrustServerCertificate=True;Encrypt=True;";

        
        const DatabaseType databaseType = DatabaseType.MySql;

        [Fact(Timeout = 60000)]
        public async Task AlunoService_Integracao_Adicionar_Obter_Atualizar_Remover()
        {
            var services = DependencyInjection.ConfigureServices(connectionString, databaseType);
            var provider = services.BuildServiceProvider();

            var alunoService = provider.GetRequiredService<IAlunoService>();
            var logradouroService = provider.GetRequiredService<ILogradouroService>();

            var logradouro = await logradouroService.ObterPorIdAsync(5);
            Assert.NotNull(logradouro);

            var cpf = GerarCpfFake();

            var dto = new AlunoDTO
            {
                Nome = "Aluno Teste",
                Cpf = cpf,
                DataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
                Telefone = "11999999999",
                Email = "aluno@teste.com",
                Endereco = logradouro,
                Numero = "100",
                Complemento = "Apto 1",
                Senha = "Senha@1"
            };

            AlunoDTO? criado = null;

            try
            {
                // CREATE
                criado = await alunoService.AdicionarAsync(dto);

                Assert.NotNull(criado);
                Assert.True(criado.Id > 0);
                Assert.Equal(cpf, criado.Cpf);

                // READ - CPF
                var porCpf = await alunoService.ObterPorCpfAsync(cpf);
                Assert.NotNull(porCpf);
                Assert.Equal(criado.Id, porCpf!.Id);

                // UPDATE
                var dtoUpdate = new AlunoDTO
                {
                    Id = criado.Id,
                    Nome = "Aluno Atualizado",
                    Cpf = criado.Cpf,
                    DataNascimento = criado.DataNascimento,
                    Telefone = criado.Telefone,
                    Email = criado.Email,
                    Endereco = criado.Endereco,
                    Numero = criado.Numero,
                    Complemento = criado.Complemento,
                    Senha = null
                };

                var atualizado = await alunoService.AtualizarAsync(dtoUpdate);
                Assert.NotNull(atualizado);
                Assert.Equal("Aluno Atualizado", atualizado.Nome);

                // DELETE
                var removido = await alunoService.RemoverAsync(criado.Id);
                Assert.True(removido);

                // VERIFICAR REMOÇÃO
                await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                    alunoService.ObterPorIdAsync(criado.Id));
            }
            finally
            {
                if (criado is not null)
                {
                    try { await alunoService.RemoverAsync(criado.Id); }
                    catch { }
                }
            }
        }

        private static string GerarCpfFake()
        {
            var rnd = new Random();
            return string.Concat(
                Enumerable.Range(0, 11).Select(_ => rnd.Next(0, 10).ToString())
            );
        }
    }
}
