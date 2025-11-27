//Gabriel Souza Varela

using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface ILogradouroRepository : IRepository<Logradouro>
    {
        Task<Logradouro?> ObterPorCepAsync(string cep);

        Task<IEnumerable<Logradouro>> ObterPorCidadeAsync(string cidade);
    }
}
