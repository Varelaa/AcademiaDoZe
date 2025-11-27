// Gabriel Souza Varela

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Infrastructure.Data;          // >>> CORREÇÃO IMPORTANTE
using Microsoft.Extensions.DependencyInjection;

namespace AcademiaDoZe.Application.Tests
{
    public class MatriculaApplicationTests
    {
        const string connectionString =
            "Server=localhost;Database=db_academia_do_ze;User Id=sa;Password=abcBolinhas12345;TrustServerCertificate=True;Encrypt=True;";

        // >>> CORRIGIDO (antes era EAppDatabaseType)
        const DatabaseType databaseType = DatabaseType.SqlServer;

        [Fact(Timeout = 60000)]
        public async Task MatriculaService_Integracao_Adicionar_Obter_Atualizar_Remover()
        {
            var services = DependencyInjection.ConfigureServices(connectionString, databaseType);
            var provider = DependencyInjection.BuildServiceProvider(services);

            var matriculaService = provider.GetRequiredService<IMatriculaService>();
            var alunoService = provider.GetRequiredService<IAlunoService>();

            var aluno = await alunoService.ObterPorIdAsync(1);
            Assert.NotNull(aluno);

            var dto = new MatriculaDTO
            {
                AlunoId = aluno.Id,
                Plano = EMatriculaPlano.Mensal,
                DataInicio = DateOnly.FromDateTime(DateTime.Today),
                DataFim = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
                Objetivo = "Fitness",
                RestricoesMedicas = EMatriculaRestricoes.None,
                ObservacoesRestricoes = ""
            };

            MatriculaDTO? criado = null;

            try
            {
                // CREATE
                criado = await matriculaService.AdicionarAsync(dto);
                Assert.NotNull(criado);
                Assert.True(criado.Id > 0);

                // READ
                var obtido = await matriculaService.ObterPorIdAsync(criado.Id);
                Assert.NotNull(obtido);
                Assert.Equal(criado.Id, obtido!.Id);

                // UPDATE
                var atualizar = new MatriculaDTO
                {
                    Id = criado.Id,
                    AlunoId = criado.AlunoId,
                    Plano = criado.Plano,
                    DataInicio = criado.DataInicio,
                    DataFim = criado.DataFim.AddMonths(1),
                    Objetivo = "Hipertrofia",
                    RestricoesMedicas = criado.RestricoesMedicas,
                    ObservacoesRestricoes = criado.ObservacoesRestricoes
                };

                var atualizado = await matriculaService.AtualizarAsync(atualizar);
                Assert.NotNull(atualizado);
                Assert.Equal("Hipertrofia", atualizado.Objetivo);

                // DELETE
                var removido = await matriculaService.RemoverAsync(criado.Id);
                Assert.True(removido);

                var aposRemocao = await matriculaService.ObterPorIdAsync(criado.Id);
                Assert.Null(aposRemocao);
            }
            finally
            {
                if (criado != null)
                {
                    try { await matriculaService.RemoverAsync(criado.Id); } catch { }
                }
            }
        }
    }
}
