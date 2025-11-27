// Gabriel Souza Varela

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Application.Security;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services
{
    public class ColaboradorService : IColaboradorService
    {
        private readonly Func<IColaboradorRepository> _repoFactory;

        public ColaboradorService(Func<IColaboradorRepository> repoFactory)
        {
            _repoFactory = repoFactory ?? throw new ArgumentNullException(nameof(repoFactory));
        }


        public async Task<ColaboradorDTO> ObterPorIdAsync(int id)
        {
            var colaborador = await _repoFactory().ObterPorId(id);
            return colaborador?.ToDto()
                ?? throw new KeyNotFoundException("Colaborador não encontrado.");
        }

        public async Task<IEnumerable<ColaboradorDTO>> ObterTodosAsync()
        {
            var colaboradores = await _repoFactory().ObterTodos();
            return colaboradores.Select(c => c.ToDto());
        }
        public async Task<ColaboradorDTO> AdicionarAsync(ColaboradorDTO dto)
        {
            // CPF duplicado
            if (await _repoFactory().CpfJaExisteAsync(dto.Cpf))
                throw new InvalidOperationException($"CPF {dto.Cpf} já está cadastrado.");

            // Hash da senha, se fornecida
            if (!string.IsNullOrWhiteSpace(dto.Senha))
                dto.Senha = PasswordHasher.Hash(dto.Senha);

            var colaborador = dto.ToEntity();
            await _repoFactory().Adicionar(colaborador);

            return colaborador.ToDto();
        }

        public async Task<ColaboradorDTO> AtualizarAsync(ColaboradorDTO dto)
        {
            var existente = await _repoFactory().ObterPorId(dto.Id)
                ?? throw new KeyNotFoundException("Colaborador não encontrado.");

            // Validação de CPF duplicado
            if (await _repoFactory().CpfJaExisteAsync(dto.Cpf, dto.Id))
                throw new InvalidOperationException($"CPF {dto.Cpf} já está em uso.");

            // Senha só se enviada
            if (!string.IsNullOrWhiteSpace(dto.Senha))
                dto.Senha = PasswordHasher.Hash(dto.Senha);
            else
                dto.Senha = existente.Senha;

            var atualizado = existente.UpdateFromDto(dto);

            await _repoFactory().Atualizar(atualizado);

            return atualizado.ToDto();
        }

        public async Task<bool> RemoverAsync(int id)
        {
            var existente = await _repoFactory().ObterPorId(id);
            if (existente == null)
                return false;

            await _repoFactory().Remover(id);
            return true;
        }

        public async Task<ColaboradorDTO> ObterPorCpfAsync(string cpf)
        {
            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            var colaborador = await _repoFactory().ObterPorCpfAsync(cpf);
            return colaborador?.ToDto() ?? null!;
        }

        public Task<bool> CpfJaExisteAsync(string cpf, int? id = null)
        {
            return _repoFactory().CpfJaExisteAsync(cpf, id);
        }
        public async Task<bool> TrocarSenhaAsync(int id, string novaSenha)
        {
            if (string.IsNullOrWhiteSpace(novaSenha))
                throw new ArgumentException("Senha inválida.");

            var hash = PasswordHasher.Hash(novaSenha);
            return await _repoFactory().TrocarSenhaAsync(id, hash);
        }
    }
}
