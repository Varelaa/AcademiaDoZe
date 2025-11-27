// Gabriel Souza Varela

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using AcademiaDoZe.Infrastructure.Data; // IMPORTANTE

namespace AcademiaDoZe.Application.Tests
{
    public class LogradouroApplicationTests
    {
        const string connectionString =
            "Server=localhost;Database=db_academia_do_ze;User Id=sa;Password=abcBolinhas12345;TrustServerCertificate=True;Encrypt=True;";

        const DatabaseType databaseType = DatabaseType.SqlServer;  // CORRIGIDO

        [Fact(Timeout = 60000)]
        public async Task LogradouroService_Integracao_Adicionar_Obter_Atualizar_Remover()
        {
            var services = DependencyInjection.ConfigureServices(connectionString, databaseType);
            var provider = DependencyInjection.BuildServiceProvider(services);

            var logradouroService = provider.GetRequiredService<ILogradouroService>();

            var cepTeste = "99500001";

            var dto = new LogradouroDTO
            {
                Cep = cepTeste,
                Nome = "Rua Teste",
                Bairro = "Centro",
                Cidade = "Cidade X",
                Estado = "SP",
                Pais = "Brasil"
            };

            LogradouroDTO? criado = null;

            try
            {
                criado = await logradouroService.AdicionarAsync(dto);
                Assert.NotNull(criado);
                Assert.True(criado.Id > 0);

                var obtido = await logradouroService.ObterPorCepAsync(cepTeste);
                Assert.NotNull(obtido);
                Assert.Equal("Rua Teste", obtido!.Nome);

                var atualizar = new LogradouroDTO
                {
                    Id = criado.Id,
                    Cep = criado.Cep,
                    Nome = "Rua Atualizada",
                    Bairro = criado.Bairro,
                    Cidade = criado.Cidade,
                    Estado = "RJ",
                    Pais = criado.Pais
                };

                var atualizado = await logradouroService.AtualizarAsync(atualizar);
                Assert.NotNull(atualizado);
                Assert.Equal("Rua Atualizada", atualizado.Nome);
                Assert.Equal("RJ", atualizado.Estado);

                var removido = await logradouroService.RemoverAsync(criado.Id);
                Assert.True(removido);

                var aposRemocao = await logradouroService.ObterPorIdAsync(criado.Id);
                Assert.Null(aposRemocao);
            }
            finally
            {
                if (criado != null)
                {
                    try { await logradouroService.RemoverAsync(criado.Id); } catch { }
                }
            }
        }
    }
}
