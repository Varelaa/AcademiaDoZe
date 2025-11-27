//Gabriel Souza Varela

using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface IAcessoRepository : IRepository<Acesso>
    {
        Task<IEnumerable<Acesso>> GetAcessosPorAlunoPeriodoAsync(
            int? alunoId = null, DateOnly? inicio = null, DateOnly? fim = null);

        Task<IEnumerable<Acesso>> GetAcessosPorColaboradorPeriodoAsync(
            int? colaboradorId = null, DateOnly? inicio = null, DateOnly? fim = null);

        Task<Dictionary<TimeOnly, int>> GetHorarioMaisProcuradoPorMesAsync(int mes);

        Task<Dictionary<int, TimeSpan>> GetPermanenciaMediaPorMesAsync(int mes);

        Task<IEnumerable<Aluno>> GetAlunosSemAcessoNosUltimosDiasAsync(int dias);
        Task RegistrarEntradaAsync(int pessoaId, DateTime dataHora);
        Task RegistrarSaidaAsync(int pessoaId, DateTime dataHora);
    }
}
