//Gabriel Souza Varela

using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface IMatriculaRepository : IRepository<Matricula>
    {
        Task<IEnumerable<Matricula>> ObterPorAlunoAsync(int alunoId);

        Task<IEnumerable<Matricula>> ObterAtivasAsync(int alunoId = 0);

        Task<IEnumerable<Matricula>> ObterVencendoEmDiasAsync(int dias);

        Task<IEnumerable<Matricula>> ObterPorAlunoIdAsync(int alunoId);
    }
}
