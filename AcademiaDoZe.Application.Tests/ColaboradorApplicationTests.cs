// Gabriel Souza Varela

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Infrastructure.Data; // IMPORTANTE
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AcademiaDoZe.Application.Tests
{
    public class ColaboradorApplicationTests
    {
        const string connectionString =
            "Server=localhost;Database=db_academia_do_ze;User Id=sa;Password=abcBolinhas12345;TrustServerCertificate=True;Encrypt=True;";

        // CORRIGIDO — usar enum da Infrastructure
        const DatabaseType databaseType = DatabaseType.SqlServer;

        [Fact(Timeout = 60000)]
        public async Task ColaboradorService_Integracao_Adicionar_Obter_Atualizar_Remover()
        {
            var services = DependencyInjection.ConfigureServices(connectionString, databaseType);
            var provider = services.BuildServiceProvider();

            var colaboradorService = provider.GetRequiredService<IColaboradorService>();
            var logradouroService = provider.GetRequiredService<ILogradouroService>();

            var cpf = GerarCpfFake();

            var logradouro = await logradouroService.ObterPorIdAsync(5);
            Assert.NotNull(logradouro);

            var caminhoFoto = Path.Combine("..", "..", "..", "foto_teste.png");
            ArquivoDTO foto = new();

            if (File.Exists(caminhoFoto))
            {
                foto.Conteudo = await File.ReadAllBytesAsync(caminhoFoto);
            }
            else
            {
                throw new FileNotFoundException("Foto de teste não encontrada.", caminhoFoto);
            }

            var dto = new ColaboradorDTO
            {
                Nome = "Colaborador Teste",
                Cpf = cpf,
                DataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
                Telefone = "11999999999",
                Email = "Colaborador@teste.com",
                Endereco = logradouro,
                Numero = "100",
                Complemento = "Apto 1",
                Senha = "Senha@1",
                Foto = foto,
                DataAdmissao = DateOnly.FromDateTime(DateTime.Today.AddDays(-30)),
                Tipo = EColaboradorTipo.Administrador,
                Vinculo = EColaboradorVinculo.CLT
            };

            ColaboradorDTO? criado = null;

            try
            {
                criado = await colaboradorService.AdicionarAsync(dto);

                Assert.NotNull(criado);
                Assert.True(criado.Id > 0);
                Assert.Equal(cpf, criado.Cpf);

                var obtido = await colaboradorService.ObterPorCpfAsync(criado.Cpf);

                Assert.NotNull(obtido);
                Assert.Equal(criado.Id, obtido!.Id);

                var atualizar = new ColaboradorDTO
                {
                    Id = criado.Id,
                    Nome = "Colaborador Atualizado",
                    Cpf = criado.Cpf,
                    DataNascimento = criado.DataNascimento,
                    Telefone = criado.Telefone,
                    Email = criado.Email,
                    Endereco = criado.Endereco,
                    Numero = criado.Numero,
                    Complemento = criado.Complemento,
                    Senha = null,
                    Foto = criado.Foto,
                    DataAdmissao = criado.DataAdmissao,
                    Tipo = criado.Tipo,
                    Vinculo = criado.Vinculo
                };

                var atualizado = await colaboradorService.AtualizarAsync(atualizar);

                Assert.NotNull(atualizado);
                Assert.Equal("Colaborador Atualizado", atualizado.Nome);

                var removido = await colaboradorService.RemoverAsync(criado.Id);
                Assert.True(removido);

                await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                    colaboradorService.ObterPorIdAsync(criado.Id));
            }
            finally
            {
                if (criado is not null)
                {
                    try { await colaboradorService.RemoverAsync(criado.Id); } catch { }
                }
            }
        }

        private static string GerarCpfFake()
        {
            var rnd = new Random();
            return string.Concat(Enumerable.Range(0, 11)
                .Select(_ => rnd.Next(0, 10).ToString()));
        }
    }
}
