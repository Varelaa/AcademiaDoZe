//Gabriel Souza Varela

using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface IAlunoRepository : IRepository<Aluno>
    {
        Task<Aluno?> ObterPorCpfAsync(string cpf);

        Task<bool> CpfJaExisteAsync(string cpf, int? id = null);

        Task<bool> TrocarSenhaAsync(int id, string novaSenha);

        Task<IEnumerable<Matricula>> ObterMatriculasAsync(int alunoId);
    }
}
