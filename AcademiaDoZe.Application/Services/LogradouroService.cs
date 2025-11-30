// Gabriel Souza Varela

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services
{
    public class LogradouroService : ILogradouroService
    {
        private readonly Func<ILogradouroRepository> _repo;

        public LogradouroService(Func<ILogradouroRepository> repoFactory)
        {
            _repo = repoFactory ?? throw new ArgumentNullException(nameof(repoFactory));
        }

        public async Task<LogradouroDTO> ObterPorIdAsync(int id)
        {
            var entity = await _repo().ObterPorId(id)
                ?? throw new KeyNotFoundException($"Logradouro ID {id} não encontrado.");

            return entity.ToDto();
        }

        public async Task<IEnumerable<LogradouroDTO>> ObterTodosAsync()
        {
            var lista = await _repo().ObterTodos();
            return lista.Select(l => l.ToDto());
        }

        public async Task<LogradouroDTO> AdicionarAsync(LogradouroDTO dto)
        {
            // Normaliza CEP antes de qualquer consulta
            dto.Cep = new string(dto.Cep.Where(char.IsDigit).ToArray());

            var existente = await _repo().ObterPorCepAsync(dto.Cep);
            if (existente != null)
                throw new InvalidOperationException(
                    $"Já existe um logradouro cadastrado com o CEP {dto.Cep} (ID {existente.Id}).");

            var entity = dto.ToEntity();

            await _repo().Adicionar(entity);

            return entity.ToDto();
        }

        public async Task<LogradouroDTO> AtualizarAsync(LogradouroDTO dto)
        {
            var atual = await _repo().ObterPorId(dto.Id)
                ?? throw new KeyNotFoundException($"Logradouro ID {dto.Id} não encontrado.");

            // Normaliza CEP para comparar
            var novoCep = new string(dto.Cep.Where(char.IsDigit).ToArray());

            if (!string.Equals(atual.Cep, novoCep, StringComparison.OrdinalIgnoreCase))
            {
                var existente = await _repo().ObterPorCepAsync(novoCep);
                if (existente != null && existente.Id != dto.Id)
                    throw new InvalidOperationException(
                        $"O CEP {novoCep} já está em uso pelo logradouro ID {existente.Id}.");
            }

            var atualizado = atual.UpdateFromDto(dto);

            await _repo().Atualizar(atualizado);

            return atualizado.ToDto();
        }

        public async Task<bool> RemoverAsync(int id)
        {
            var atual = await _repo().ObterPorId(id);
            if (atual == null)
                return false;

            await _repo().Remover(id);
            return true;
        }

        public async Task<LogradouroDTO> ObterPorCepAsync(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                throw new ArgumentException("CEP não pode ser vazio.", nameof(cep));

            cep = new string(cep.Where(char.IsDigit).ToArray());

            var entity = await _repo().ObterPorCepAsync(cep);
            return entity?.ToDto() ?? null!;
        }

        public async Task<IEnumerable<LogradouroDTO>> ObterPorCidadeAsync(string cidade)
        {
            if (string.IsNullOrWhiteSpace(cidade))
                throw new ArgumentException("Cidade não pode ser vazia.", nameof(cidade));

            cidade = cidade.Trim();

            var lista = await _repo().ObterPorCidadeAsync(cidade);
            return lista.Select(l => l.ToDto());
        }
    }
}
