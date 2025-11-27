//Gabriel Souza Varela

using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface IColaboradorRepository : IRepository<Colaborador>
    {
        Task<Colaborador?> ObterPorCpfAsync(string cpf);

        Task<bool> CpfJaExisteAsync(string cpf, int? id = null);

        Task<bool> TrocarSenhaAsync(int id, string novaSenha);
    }
}
